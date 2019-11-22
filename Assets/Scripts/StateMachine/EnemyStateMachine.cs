using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : EntityStateMachine
{
    //For the progressBar
    private float maxCooldown = 5f;
    // Ienumrator
    public GameObject PlayerToAttack;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;
            case (TurnState.CHOOSEACTION):
                if (BSM.PlayerInBattle.Count == 0)
                {
                    BSM.battleStates = BattleStateMachine.PerformAction.LOSE;
                }
                else
                {
                    ChooseAction();
                    currentState = TurnState.WAITING;
                }
                break;
            case (TurnState.WAITING):
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                if(!isAlive)
                {
                    return;
                }
                else
                {
                    //Change tag of enemy
                    this.gameObject.tag = "DeadEnemy";
                    //Not selectable by player
                    BSM.EnemyInBattle.Remove(this.gameObject);
                    //disable hand cursor
                    HandCursor.SetActive(false);
                    //remove all input player attack
                    if (BSM.EnemyInBattle.Count >0 )
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
                                    BSM.PerformList[i].AttackerTarget = BSM.EnemyInBattle[Random.Range(0, BSM.EnemyInBattle.Count)];
                                }
                            }
                        }
                    }
                    //change color / play animation
                    this.gameObject.GetComponent<SpriteRenderer>().material.color = new Color32(105, 105, 105, 255);
                    ThisEntity.DeadInBattle(true);
                    //reset enemy buttons
                    BSM.EnemyButtons();
                    //check alive
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    isAlive = false;
                }
                break;
        }
    }
    private void UpgradeProgressBar()
    {
        currentCooldown = currentCooldown + Time.deltaTime;
        if (currentCooldown >= maxCooldown)
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }
    private void ChooseAction()
    {
        
        HandleTurn myAttack = new HandleTurn();

        myAttack.nameOfAttacker = this.nameEntity;
        myAttack.type = "Enemy";
        myAttack.AttackerGameObject = this.gameObject;
        myAttack.AttackerTarget = BSM.PlayerInBattle[Random.Range(0, BSM.PlayerInBattle.Count)];

        int num = Random.Range(0, physicAttacks.Count);
        myAttack.choosenSkill = physicAttacks[num];

        Debug.Log(this.gameObject.GetComponent<EnemyStateMachine>().nameEntity + " has choosen " + myAttack.choosenSkill.attackName + " and do " + myAttack.choosenSkill.attackDmg + " damage!" );
        BSM.CollectAction(myAttack);
    }
    protected IEnumerator TimeForAction()
    {
        if(actionStarted)
        {
            yield break;
        }
        actionStarted = true;
        //make enemy come near to player to attack
        oppositeEntityPosition = new Vector3 (PlayerToAttack.transform.position.x -50f, PlayerToAttack.transform.position.y, PlayerToAttack.transform.position.z);
        while (Moving(oppositeEntityPosition)) 
        {
            ThisEntity.MovingForwardInBattle();
            yield return null;
        }
        //waiting
        yield return new WaitForSeconds(0.5f);
        //do dmg
        DoDamge();
        //make enemy go back to original position
        Vector3 firstPosition = startposition;
        while (Moving(firstPosition)) //Move back to original position
        {
            ThisEntity.MovingBackwardInBattle();
            yield return null;
        }
        //remove this peformer from the list in BSM
        BSM.PerformList.RemoveAt(0);
        //reset bsm -> wait
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        // end courountine
        actionStarted = false;
        //reset this enemy state;
        currentCooldown = 0f;
        currentState = TurnState.PROCESSING;
    }
    void DoDamge()
    {
        float calc_damage = curATK + BSM.PerformList[0].choosenSkill.attackDmg;
        PlayerToAttack.GetComponent<PlayerStateMachine>().TakeDamage(calc_damage);
    }
    public void TakeDamage(float getDamageAmout)
    {
        CurHP -= getDamageAmout;
        if (CurHP <= 0)
        {
            currentState = TurnState.DEAD;
        }
    }
}
