using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowPlayer : MonoBehaviour
{
    static public FollowPlayer instance;
    private Player tPlayer;
    //private GameObject tPlayer;
    private Transform tFollowTarget;
    private GameObject confinderObject;
    private PolygonCollider2D boundingShape2D;
    private CinemachineVirtualCamera vcam;
    private CinemachineConfiner cconfiner;
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
       
    }
    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        cconfiner = GetComponent<CinemachineConfiner>();
        tPlayer = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //test
        if(vcam.Follow == null)
        {
            
        }
        //Qua canh, bound se thanh null, nen phai tim lai bound
        if (cconfiner.m_BoundingShape2D == null)
        {
            //Tim object voi tag bound
            confinderObject = GameObject.FindWithTag("Bound");
            //Kiem tra truong null de tranh loi~ null exception;
            if (confinderObject != null)
            {
                //Lay polygonCollider tu trong game object voi tag "bound"
                cconfiner.m_BoundingShape2D = confinderObject.GetComponent<PolygonCollider2D>();
            }
            else 
            {
                cconfiner.m_BoundingShape2D = null;
            }
        }
    }
}
