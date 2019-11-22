using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public static Player Instance;
    public string currentMapName;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this) //Already have instance, destroy the new object 
        {
            Destroy(gameObject);
        }
    }
    protected override void Start()
    {      
        base.Start();
    }
    // Update is called once per frame
    protected override void Update()
    {
        //execute get input function
        GetInput();
        base.Update();
    }

    private void FixedUpdate()
    {
        //execute move function
        Move();
    }

    private void GetInput()
    {
        direction = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector2.down;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector2.right;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector2.left;
        }
        if (Input.GetKey(KeyCode.X))
        {
            applyRunSpeed = runSpeed;
        }
        else
        {
            applyRunSpeed = 0;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "region1")
        {
            GameManager.instance.canGetEncounter = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "region1")
        {
            GameManager.instance.canGetEncounter = false;
        }
    }
}
