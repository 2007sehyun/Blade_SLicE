using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public Transform playerCam;
    public LayerMask isWall;
    public LayerMask isGround;
    private float moveSpeed;
}