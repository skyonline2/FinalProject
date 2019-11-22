using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    [SerializeField]
    private Transform pfCharacterBattle = null;
    public Texture2D characterSpritesheet;
    public Texture2D enemySpritesheet;

    // Start is called before the first frame update
    void Start()
    {
        SpawnCharacter(true);
        SpawnCharacter(false);
    }

    private void SpawnCharacter(bool isPlayerTeam)
    {
        Vector3 position;
        Quaternion flipping;
        if(isPlayerTeam)
        {
            position = new Vector3(100, 0);
            flipping = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            position = new Vector3(-100, 0);
            flipping = Quaternion.Euler(0, 180, 0);
        }
        Instantiate(pfCharacterBattle, position, flipping);
    }
}
