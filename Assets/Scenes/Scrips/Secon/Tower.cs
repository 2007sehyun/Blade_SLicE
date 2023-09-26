using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform player;

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < 50)
        {
            transform.LookAt(player.position);
        }

    }

    
}
