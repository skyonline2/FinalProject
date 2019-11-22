using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string nameOfAttacker;//Name of attacker
    public string type; //2 types for now: Enemy & Player
    public GameObject AttackerGameObject;//who attack
    public GameObject AttackerTarget;//who is going to be attacked
    /// <summary>
    /// Which attack to perform
    /// </summary>
    public BaseSkill choosenSkill;
}
