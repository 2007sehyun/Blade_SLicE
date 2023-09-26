using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform playerTrm;
    public Vector3 dir;

    private void Start()
    {
        playerTrm = GameObject.Find("Player").GetComponent<Transform>();
         dir = (playerTrm.transform.position - transform.position).normalized;
    }
    void Update()
    {
        
        transform.Translate(dir*15*Time.deltaTime);
        Destroy(gameObject, 30f);
    }

    
}
