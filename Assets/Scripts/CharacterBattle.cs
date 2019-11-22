using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattle : Character
{
    protected override void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        ActiveLayer("BattleIdleLayer");
    }
   
    public void MovingForwardInBattle()
    {
        myAnimator.SetFloat("x", direction.x = -1);
        myAnimator.SetFloat("y", direction.y = 0);
        ActiveLayer("WalkingLayer");
    }
    public void MovingBackwardInBattle()
    {
        myAnimator.SetFloat("x", direction.x = 1);
        myAnimator.SetFloat("y", direction.y = 0);
        ActiveLayer("WalkingLayer");
    }
    public void DeadInBattle(bool condition)
    {
        myAnimator.SetBool("dead",condition);
    }
}
