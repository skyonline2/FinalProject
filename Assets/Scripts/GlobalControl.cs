using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalControl : MonoBehaviour
{
    public float HP;
    public static GlobalControl Instance;
    void Awake()
    {
        if (Instance == null) 
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this) //If there is another instance, destroy that one
        {
            Destroy(gameObject);
        }
    }
}
