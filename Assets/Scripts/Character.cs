using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    //Character movement speed
    [SerializeField]
    public float speed = 100f;
    protected Animator myAnimator;
    //Character direction
    protected Vector2 direction;
    /// <summary>
    /// Character's rigid body
    /// </summary>
    private Rigidbody2D rb;
    /// <summary>
    /// Indicates if character moving or not
    /// </summary>
    public bool isMoving
    {
        get
        {
            return direction != Vector2.zero;
        }
    }
    [SerializeField]
    protected float runSpeed;
    protected float applyRunSpeed;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
        myAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        HandleLayer();
    }

    public void Move()
    {
        //Makes player moves
        rb.velocity = direction.normalized * (speed + applyRunSpeed);
    }
    public void Stop()
    {
        //Makes player stops
        rb.velocity = new Vector2(0, 0);
    }
    public void HandleLayer()
    {
        //Check if player is moving or standing, if moving then playing moving animation
        if (isMoving)
        {
            ActiveLayer("WalkingLayer");
            //Sets the animation parameter so that she faces the correct direction
            myAnimator.SetFloat("x", direction.x);
            myAnimator.SetFloat("y", direction.y);
        }
        //Otherwise, go back to idle animation when the button was not pressed.
        else
        {
            ActiveLayer("IdleLayer");
        }
    }

    public void ActiveLayer(string layerName)
    {
        for (int i = 0; i < myAnimator.layerCount; i++)
        {
            myAnimator.SetLayerWeight(i, 0);
        }
        myAnimator.SetLayerWeight(myAnimator.GetLayerIndex(layerName), 1);
    }
}
