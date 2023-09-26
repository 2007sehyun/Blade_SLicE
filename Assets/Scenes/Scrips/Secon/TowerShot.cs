using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerShot : MonoBehaviour
{

    public Transform player;
    public GameObject shotPrefabs;
    public Transform[] shotPos;
    int num = 0;
    float coolTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Shoting());
    }

    // Update is called once per frame

    IEnumerator Shoting()
    {
        while (true)
        {



            if (Vector3.Distance(transform.position, player.position) < 50)
            {
                transform.LookAt(player.position);    
                coolTime -=1;
                num = 0;
                Instantiate(shotPrefabs, shotPos[num].position, Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
                num = 1;
                Instantiate(shotPrefabs, shotPos[num].position, Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
                if (coolTime <= 0)
                {
                    yield return new WaitForSeconds(5f);
                    coolTime = 10;
                }
                yield return null;
            }
            yield return null;
        }
    }
}


