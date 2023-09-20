using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private GameObject effectObj;
    private Rigidbody rigid;
    private Animator ani;
    private bool isGround;
    private Vector3 move;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
    }
    void Start()
    {
        StartCoroutine(Sliced());
    }

    void Update()
    {
        
        Move();
        CameraRotate();
        Jump();
    }

    

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        if(Input.GetAxisRaw("Vertical")>0 || Input.GetAxisRaw("Vertical") < 0)
        {
            ani.SetBool("Running",true);
        }
        else
            ani.SetBool("Running", false);

        

        move = transform.forward * z + transform.right * x;
        move.y = 0;

        transform.position += move.normalized*Time.deltaTime * 20;

       /* Vector3 horizontal = transform.right * x;
        Vector3 vertical = transform.forward * z;

        Vector3 _velocity = (horizontal + vertical).normalized * 40;
        rigid.MovePosition(transform.position+= _velocity * Time.deltaTime);*/

    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rigid.AddForce(Vector3.up * 30, ForceMode.Impulse);
            
            isGround = false;
        }
    }



    void CameraRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");

        transform.Rotate(transform.rotation.x, mouseX, 0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator Sliced()
    {
        while(true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ani.SetTrigger("Slicing");
                yield return new WaitForSeconds(0.45f);
                effectObj.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                effectObj.SetActive(false);
                
            }
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
            isGround = true;
    }
}
