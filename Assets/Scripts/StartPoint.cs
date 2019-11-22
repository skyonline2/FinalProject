using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPoint : MonoBehaviour
{
    public string mapName;
    private Player thePlayer;
    private void Start()
    {
        thePlayer = FindObjectOfType<Player>();
        if(mapName == thePlayer.currentMapName )
        {
            thePlayer.transform.position = this.transform.position;
        }
    }
}
