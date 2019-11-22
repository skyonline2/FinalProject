using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }
    public PerformAction battleStates;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> PlayerInBattle = new List<GameObject>();
    public List<GameObject> EnemyInBattle = new List<GameObject>();

    public enum PlayerGUI
    {
        ACTIVE,
        WAITING,
        DONE
    }
    public PlayerGUI PlayerInput;

    public List<GameObject> HeroToManager = new List<GameObject>();
    private HandleTurn HeroChoice;
    [SerializeField]
    private GameObject enemyButton = null;
    public Transform Spacer;  //enemy spacer
    public GameObject ActionPanel;
    public GameObject EnemySelectPanel;
    public GameObject MagicPanel;
    /// <summary>
    /// attack of hero
    /// </summary>
    public GameObject magicSkillButton;
    public GameObject actionButton;
    private List<GameObject> atkBtns = new List<GameObject>();

    //enemy button
    private List<GameObject> enemyBtns = new List<GameObject>();

    //spawn points
    public List<Transform> spawnPoints = new List<Transform>();
    //player
    private Player thePlayer;
    private void Awake()
    {
        IsPlayerTeamSpawn(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        IsPlayerTeamSpawn(true);
        battleStates = PerformAction.WAIT;
        //EnemyInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        SortListByHierarchyOrder(EnemyInBattle);
        PlayerInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        PlayerInput = PlayerGUI.ACTIVE;
        ActionPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleStates)
        {
            case (PerformAction.WAIT):
                if(PerformList.Count >0)
                {
                    battleStates = PerformAction.TAKEACTION;
                }
                break;
            case (PerformAction.TAKEACTION):
                GameObject performer = PerformList[0].AttackerGameObject;
                if (PerformList[0].type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    for(int i=0; i<PlayerInBattle.Count; i++)
                    {
                        if(PerformList[0].AttackerTarget == PlayerInBattle[i])
                        {
                            ESM.PlayerToAttack = PerformList[0].AttackerTarget;
                            ESM.currentState = EntityStateMachine.TurnState.ACTION;
                            break;
                        }
                        else
                        {
                            PerformList[0].AttackerTarget = PlayerInBattle[Random.Range(0, PlayerInBattle.Count)];
                            ESM.PlayerToAttack = PerformList[0].AttackerTarget;
                            ESM.currentState = EntityStateMachine.TurnState.ACTION;
                        }
                    }
                }
                if (PerformList[0].type == "Player")
                {
                    PlayerStateMachine PSM = performer.GetComponent<PlayerStateMachine>();
                    PSM.EnemyToAttack = PerformList[0].AttackerTarget;
                    PSM.currentState = EntityStateMachine.TurnState.ACTION;
                }
                battleStates = PerformAction.PERFORMACTION;
                break;
            case (PerformAction.PERFORMACTION):
                //idle
                break;
            case (PerformAction.CHECKALIVE):
                if(PlayerInBattle.Count < 1)
                {
                    //Lose the battle
                    battleStates = PerformAction.LOSE;
                }
                else if (EnemyInBattle.Count < 1)
                {
                    //Win the battle
                    battleStates = PerformAction.WIN;
                }
                else
                {
                    clearAttackPanel();
                    PlayerInput = PlayerGUI.ACTIVE;
                }
                break;
            case (PerformAction.LOSE):
                Debug.Log("You lost the battle");
                break;
            case (PerformAction.WIN):
                Debug.Log("You won the battle");
                for (int i = 0; i < PlayerInBattle.Count; i++)
                {
                    PlayerInBattle[i].GetComponent<PlayerStateMachine>().currentState = EntityStateMachine.TurnState.WAITING;
                }
                PlayerBattleStatus(false);
                GameManager.instance.LoadSceneAfterBattle();
                GameManager.instance.gameState = GameManager.GameStates.WORLD_STATE;
                GameManager.instance.enemiesToBattle.Clear();
                break;
        }
        switch (PlayerInput)
        {
            case (PlayerGUI.ACTIVE):
                if (HeroToManager.Count > 0)
                {
                    HeroToManager[0].transform.Find("Selector").gameObject.SetActive(true);
                    //create new handleturn instance
                    HeroChoice = new HandleTurn();
                    ActionPanel.SetActive(true);
                    //populate action buttons
                    CreateAttackButtons();
                    //switch to WAITING
                    PlayerInput = PlayerGUI.WAITING;
                }
                break;
            case (PlayerGUI.WAITING):
                break;
            case (PlayerGUI.DONE):
                HeroInputDone();
                break;
        }
    }
    public void CollectAction(HandleTurn input)
    {
        PerformList.Add(input);
    }
    public void EnemyButtons()
    {
        //clean up
        foreach(GameObject enemyBtn in enemyBtns)
        {
            enemyBtn.SetActive(false);
            Destroy(enemyBtn);            
        }
        enemyBtns.Clear();
        //create buttons
        foreach(GameObject enemy in EnemyInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();
            Text buttonText = newButton.GetComponentInChildren<Text>();
            buttonText.text = cur_enemy.nameEntity;
            button.EnemyPrefab = enemy;
            newButton.transform.SetParent(Spacer, false);
            enemyBtns.Add(newButton);
        }
    }
    public void Input1() //attack button
    {
        HeroChoice.nameOfAttacker = HeroToManager[0].GetComponent<PlayerStateMachine>().nameEntity;
        HeroChoice.AttackerGameObject = HeroToManager[0];
        HeroChoice.type = "Player";
        HeroChoice.choosenSkill = HeroToManager[0].GetComponent<PlayerStateMachine>().physicAttacks[0];
        ActionPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    public void Input2(GameObject choosenEnemy)//enemy selection
    {
        HeroChoice.AttackerTarget = choosenEnemy;
        PlayerInput = PlayerGUI.DONE;
    }
    void HeroInputDone()
    {
        PerformList.Add(HeroChoice);
        //clean the action panel
        clearAttackPanel();

        HeroToManager[0].transform.Find("Selector").gameObject.SetActive(false);
        HeroToManager.RemoveAt(0);
        PlayerInput = PlayerGUI.ACTIVE;
    }
    void clearAttackPanel()
    {
        EnemySelectPanel.SetActive(false);
        //Clear hand cursor when target is already dead
        for (int i = 0; i < EnemyInBattle.Count; i++)
        {
            EnemyInBattle[i].gameObject.transform.Find("HandCursor").gameObject.SetActive(false);
        }
        ActionPanel.SetActive(false);
        MagicPanel.SetActive(false);
        foreach (GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        atkBtns.Clear();
    }
    public void SortListByHierarchyOrder(List<GameObject> gameobjects)
    {
        gameobjects.Sort((e1, e2) => e1.transform.GetSiblingIndex().CompareTo(e2.transform.GetSiblingIndex()));
    }
    //create action buttons
    void CreateAttackButtons()
    {
        //create attack button
        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        Text AttackButtonText = AttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(()=> Input1());
        AttackButton.transform.SetParent(ActionPanel.transform,false);
        atkBtns.Add(AttackButton);
        //create magic attack button
        GameObject MagicAttackButton = Instantiate(actionButton) as GameObject;
        Text MagicAttackButtonText = MagicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        MagicAttackButtonText.text = "Magic";
        MagicAttackButton.GetComponent<Button>().onClick.AddListener(()=> Input3());
        MagicAttackButton.transform.SetParent(ActionPanel.transform,false);
        atkBtns.Add(MagicAttackButton);
        
        if (HeroToManager[0].GetComponent<PlayerStateMachine>().magicAttacks.Count>0)
        {
            foreach(BaseSkill magicAtk in HeroToManager[0].GetComponent<PlayerStateMachine>().magicAttacks)
            {
                GameObject MagicSkillButton = Instantiate(magicSkillButton) as GameObject;
                Text MagicSkillButtonText = MagicSkillButton.transform.Find("Text").gameObject.GetComponent<Text>();
                MagicSkillButtonText.text = magicAtk.attackName;
                MagicSkillButton ATB = MagicSkillButton.GetComponent<MagicSkillButton>();
                ATB.magicAttackToPerform = magicAtk;
                MagicSkillButton.transform.SetParent(MagicPanel.transform, false);
                atkBtns.Add(MagicSkillButton);
            }
        }
        //To avoid null exception if there are no hero or button on the list
        else
        {
            MagicAttackButton.GetComponent<Button>().interactable = false;
        }
    }
    public void Input4(BaseSkill choosenMagic)//choosen magic attack
    {
        HeroChoice.nameOfAttacker = HeroToManager[0].GetComponent<PlayerStateMachine>().nameEntity;
        HeroChoice.AttackerGameObject = HeroToManager[0];
        HeroChoice.type = "Player";

        HeroChoice.choosenSkill = choosenMagic;
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }
    public void Input3() //switching to magic attack
    {
        ActionPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }
    private void IsPlayerTeamSpawn(bool isPlayerTeam)
    {
        if (isPlayerTeam)
        {
            thePlayer = FindObjectOfType<Player>();
            thePlayer.Stop(); // stop velocity
            thePlayer.transform.position = new Vector3(100, 25);
            PlayerBattleStatus(true);
        }
        else
        {
            for (int i = 0; i < GameManager.instance.enemyAmount; i++)
            {
                GameObject NewEnemy = Instantiate(GameManager.instance.enemiesToBattle[i], spawnPoints[i].position, Quaternion.Euler(0, 180, 0)) as GameObject;
                NewEnemy.name = NewEnemy.GetComponent<EnemyStateMachine>().nameEntity + " " + (i + 1);
                NewEnemy.GetComponent<EnemyStateMachine>().nameEntity = NewEnemy.name;
                EnemyInBattle.Add(NewEnemy);
            }
        }
    }
    private void PlayerBattleStatus(bool isInBattle)
    {
        if (isInBattle)
        {
            thePlayer.GetComponent<Player>().enabled = false;
            thePlayer.GetComponent<CharacterBattle>().enabled = true;
            thePlayer.GetComponent<PlayerStateMachine>().enabled = true;
        }
        else
        {
            thePlayer.GetComponent<Player>().enabled = true;
            thePlayer.GetComponent<CharacterBattle>().enabled = false;
            thePlayer.GetComponent<PlayerStateMachine>().enabled = false;
        }
    }
}
