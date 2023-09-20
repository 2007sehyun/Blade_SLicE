using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = -Input.GetAxis("Mouse Y");
        float rotateX = Mathf.Clamp(mouseY, -20, 20);
        transform.eulerAngles += new Vector3(rotateX, 0, 0);
    }   
}
