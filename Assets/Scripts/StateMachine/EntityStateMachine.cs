using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateMachine : BaseEntity
{
    protected GameObject Selector;
    protected GameObject HandCursor;
    public CharacterBattle ThisEntity;
    protected BattleStateMachine BSM;
    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }
    public TurnState currentState;
    //For the progressBar
    protected float currentCooldown = 0f;
    //Time for action stuff
    protected bool actionStarted = false;
    protected Vector3 startposition;
    protected Vector3 oppositeEntityPosition;
    // dead
    protected bool isAlive = true;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ThisEntity = GetComponent<CharacterBattle>();
        currentState = TurnState.PROCESSING;
        BSM = FindObjectOfType<BattleStateMachine>();
        startposition = transform.position;
        currentCooldown = Random.Range(0, 2.5f);
        Selector = this.gameObject.transform.Find("Selector").gameObject;
        HandCursor = this.gameObject.transform.Find("HandCursor").gameObject;
        Selector.SetActive(false);
        HandCursor.SetActive(false);
    }

    protected bool Moving(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, ThisEntity.speed * Time.deltaTime));
    }

}
