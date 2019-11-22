using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject EnemyPrefab;


    private void SelectEnemy()
    {
        //save input enemy prefab
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input2(EnemyPrefab);
        SetActiveHandCursor(false);
    }
   
    public void SetActiveHandCursor(bool value)
    {
        EnemyPrefab.transform.Find("HandCursor").gameObject.SetActive(value);
    }
}
