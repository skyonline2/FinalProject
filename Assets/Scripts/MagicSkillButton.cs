using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSkillButton : MonoBehaviour
{
    public BaseSkill magicAttackToPerform;
    public void CastMagicAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input4(magicAttackToPerform);
    }
}
