using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateMachine : EntityStateMachine
{
    //For the progressBar
    [SerializeField]
    private Image ProgressBar =null;
    private float maxCooldown = 5f;
    // Ienumrator
    public GameObject EnemyToAttack;
    
    // heroPanel
    private HeroPanelStats panelStats;
    public GameObject heroBar;
    private Transform heroPanelSpacer;

    protected override void Start()
    {
        base.Start();
        //find spacer
        heroPanelSpacer = GameObject.Find("HeroPanelSpacer").transform;
        //create panel, fill in info
        CreateHeroPanel();
        //inherit from entity state machine       
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;
            case (TurnState.ADDTOLIST):
                BSM.HeroToManager.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                //idle
                break;
            case (TurnState.ACTION):
                if (curHP > 0)
                {
                    StartCoroutine(TimeForAction());
                }
                else
                {
                    currentState = TurnState.DEAD;
                }
                break;
            case (TurnState.DEAD):
                if(!isAlive)
                {
                    return;
                }
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadPlayer";
                    //not attackable by enemy
                    BSM.PlayerInBattle.Remove(this.gameObject);
                    //not managable
                    BSM.HeroToManager.Remove(this.gameObject);
                    //deactive the selector
                    Selector.SetActive(false);
                    //reset GUI
                    BSM.ActionPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    //Clear choosen hand cursor when player is dead
                    for (int i = 0; i < BSM.EnemyInBattle.Count; i++)
                    {
                        BSM.EnemyInBattle[i].gameObject.transform.Find("HandCursor").gameObject.SetActive(false);
                    }
                    //remove item from performlist
                    if (BSM.PlayerInBattle.Count >0 )
                    {
                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (i!=0)
                            {
                                if (BSM.PerformList[i].AttackerGameObject == this.gameObject)
                                {
                                    BSM.PerformList.Remove(BSM.PerformList[i]);
                                }
                                else if (BSM.PerformList[i].AttackerTarget == this.gameObject)
                                {
                                    BSM.PerformList[i].AttackerTarget = BSM.PlayerInBattle[Random.Range(0, BSM.PlayerInBattle.Count)];
                                }
                            }
                        }
                    }
                    //change color / play animation
                    this.gameObject.GetComponent<SpriteRenderer>().material.color = new Color32(105, 105, 105, 255);
                    ThisEntity.DeadInBattle(true);
                    //reset heroinput
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    isAlive = false;                
                }
                break;
        }
    }
    protected void UpgradeProgressBar()
    {
        currentCooldown = currentCooldown + Time.deltaTime;
        float calcCooldown = currentCooldown / maxCooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calcCooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        if (currentCooldown >= maxCooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;
        //make player come near to enemy to attack
        oppositeEntityPosition = new Vector3(EnemyToAttack.transform.position.x + 50f, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        while (Moving(oppositeEntityPosition))
        {
            ThisEntity.MovingForwardInBattle();
            yield return null;
        }
        //waiting
        yield return new WaitForSeconds(0.5f);
        //do dmg
        DoDamge();
        //make player go back to original position
        Vector3 firstPosition = startposition;
        while (Moving(firstPosition)) //Move back to original position
        {
            ThisEntity.MovingBackwardInBattle();
            yield return null;
        }
        //remove this peformer from the list in BSM
        BSM.PerformList.RemoveAt(0);
        //reset bsm -> wait
        if(BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
        {
            BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

            //reset this enemy state;
            currentCooldown = 0f;
            currentState = TurnState.PROCESSING;
        }
        else
        {
            currentState = TurnState.WAITING;
        }
        // end courountine
        actionStarted = false;
    }
    public void TakeDamage(float getDamageAmout)
    {
        CurHP -= getDamageAmout;
        if(CurHP <=0)
        {
            currentState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }
    //create panel
    void CreateHeroPanel()
    {
            heroBar = Instantiate(heroBar) as GameObject;
            panelStats = heroBar.GetComponent<HeroPanelStats>();
            panelStats.heroName.text = nameEntity;
            panelStats.heroHP.text = CurHP + "/" + maxHP;
            panelStats.heroMP.text = CurMP + "/" + maxMP;
            ProgressBar = panelStats.progressBar;
            heroBar.transform.SetParent(heroPanelSpacer, false);
    }
    void UpdateHeroPanel()
    {
        panelStats.heroHP.text = CurHP + "/" + maxHP;
        panelStats.heroMP.text = CurMP + "/" + maxMP;
    }
    //Do Damge
    void DoDamge()
    {
        float calc_damage = curATK + BSM.PerformList[0].choosenSkill.attackDmg;
        EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
    }
}
