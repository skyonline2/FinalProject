using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class BaseSkill : ScriptableObject
{
    public string attackName;
    public string attackDescription;
    public float attackDmg;
    public float attackCost;
    public enum Type
    {
        Physical,
        Magic
    }
    public Type attackType;
}
