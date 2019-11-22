using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamereFinding : MonoBehaviour
{
    private Canvas tCanvas;
    // Start is called before the first frame update
    private void Start()
    {
        tCanvas = GetComponent<Canvas>();
        tCanvas.worldCamera = Camera.main;
        tCanvas.sortingLayerName = "Player";
    }
}
