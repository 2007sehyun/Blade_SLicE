using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicing : MonoBehaviour
{

    public Transform grapPos;
    public Transform springPoint;
    public LayerMask PosibleSpring;
    private LineRenderer springLine;
    private BoxCollider hitBox;
    private Animator ani;
    private RaycastHit hit;
    private SpringJoint springJoint;
    private bool isGraping =false;
    private Vector3 spot;
    private void Awake()
    {
        hitBox = GetComponent<BoxCollider>();
        ani = GetComponent<Animator>();
        springLine = GetComponent<LineRenderer>();
        springJoint = GetComponent<SpringJoint>();
    }
    void Start()
    {
        
        hitBox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, 50f, PosibleSpring))
            {
                isGraping = true;
                springLine.positionCount = 2;
                springLine.SetPosition(0, grapPos.position);
                springLine.SetPosition(1, hit.point);

                springJoint.autoConfigureConnectedAnchor = false;
                springJoint.connectedAnchor = spot;
                float dis = Vector3.Distance(grapPos.position, spot);

                springJoint.maxDistance = dis;
                springJoint.minDistance = dis * 500f;
                springJoint.spring = 500;
                springJoint.damper= 500f;
                springJoint.massScale= 50f;
            }   
        }
        else if(Input.GetMouseButtonUp(1))
        {
            isGraping = false;
            springLine.positionCount = 0;
        }
        if(isGraping)
        {
            springLine.SetPosition(0, grapPos.position);

        }
        if (Input.GetMouseButtonDown(0))
        {
            ani.SetBool("Slicing", true);
            
        }
        else
        {
            ani.SetBool("Slicing",false);
            
        }

        if (Input.GetAxisRaw("Horizontal") != 0  ||Input.GetAxisRaw("Vertical") !=0 )
        {
            ani.SetBool("IsRunning", true);
        }
        else
        {
            ani.SetBool("IsRunning", false);
        }

    }
    void HitBoxOnAniEvent()
    {
        hitBox.enabled = true;
    }
    void HitBoxOffAniEvent()
    {
        hitBox.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        
        if(other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
