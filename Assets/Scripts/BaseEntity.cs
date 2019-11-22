using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseEntity: MonoBehaviour
{ 
    public string nameEntity;
    public float maxHP;
    public float CurHP
    {
        get
        {
            return curHP;
        }
        set
        {
            if (value > maxHP)
            {
                curHP = maxHP;
            }
            else if (value <0)
            {
                curHP = 0;
            }
            else
            {
                curHP = value;
            }    
        }
    }
    [SerializeField]
    protected float curHP;
    public float maxMP;
    public float CurMP
    {
        get
        {
            return curMP;
        }
        set
        {
            if (value > maxMP)
            {
                curMP = maxHP;
            }
            else if (value < 0)
            {
                curMP = 0;
            }
            else
            {
                curMP = value;
            }
        }
    }
    [SerializeField]
    protected float curMP;
    public float baseATK;
    public float curATK;
    public float baseDEF;
    public float curDEF;
    public int strength;
    public int magic;
    public int speed;
    public enum Type
    {
        WATER,
        GRASS,
        FIRE,
        NORMAL
    }
    public Type entityType;
    public List<BaseSkill> physicAttacks = new List<BaseSkill>();

    public List<BaseSkill> magicAttacks = new List<BaseSkill>();
    
}
