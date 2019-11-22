using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //CLASS RANDOM MONSTER
    [System.Serializable]
    public class RegionData
    {
        public string RegionName;
        public int maxAmountEnemies;
        public string battleScene;
        public List<GameObject> possibleEnemies = new List<GameObject>();
    }
    public int curRegion;
    public List<RegionData> Regions = new List<RegionData>();
    //Player
    public Player thePlayer;
    //POSITIONS
    public Vector2 nextPlayerPosition;
    public Vector2 lastPlayerPosition; //BATTLE
    //SCENES
    public string sceneToLoad;
    public string LastScene;//BATTLE
    //BOOL
    public bool canGetEncounter = false;
    public bool gotAttacked = false;
    //ENUM
    public enum GameStates
    {
        WORLD_STATE,
        BATTLE_STATE,
        IDLE
    }
    //Battle
    public int enemyAmount;
    public List<GameObject> enemiesToBattle = new List<GameObject>();
    public GameStates gameState;
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this) //If there is another instance, destroy that one
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        thePlayer = FindObjectOfType<Player>();
    }
    private void Update()
    {
        switch(gameState)
        {
            case (GameStates.WORLD_STATE):
                if (thePlayer.isMoving)
                {
                    RandomEncounter();
                }
                if (gotAttacked)
                {
                    gameState = GameStates.BATTLE_STATE;
                }
                break;
            case (GameStates.BATTLE_STATE):
                StartBattle();
                gameState = GameStates.IDLE;
                break;
            case (GameStates.IDLE):
                break;
        }
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void LoadSceneAfterBattle()
    {
        SceneManager.LoadScene(LastScene);
        thePlayer.GetComponent<PlayerStateMachine>().StopAllCoroutines();
        thePlayer.GetComponent<PlayerStateMachine>().currentState = PlayerStateMachine.TurnState.PROCESSING;
        thePlayer.transform.position = nextPlayerPosition;
    }
    private void RandomEncounter()
    {
        if (thePlayer.isMoving && canGetEncounter)
        {
            if (Random.Range(0, 10) < 1)
            {
                Debug.Log("Got attacked!");
                gotAttacked = true;
            }
        }
    }
    private void StartBattle()
    {
        enemyAmount = Random.Range(1, Regions[curRegion].maxAmountEnemies+1);
        // ENEMIES
        for (int i = 0; i < enemyAmount; i++)
        {
            enemiesToBattle.Add(Regions[curRegion].possibleEnemies[Random.Range(0, Regions[curRegion].possibleEnemies.Count)]);
        }
        // PLAYER
        lastPlayerPosition = GameObject.FindGameObjectWithTag("Player").gameObject.transform.position;
        nextPlayerPosition = lastPlayerPosition;
        LastScene = SceneManager.GetActiveScene().name;
        //LOAD LEVEL
        SceneManager.LoadScene(Regions[curRegion].battleScene);
        //RESET
        gotAttacked = false;
        canGetEncounter = false;
    }

}
