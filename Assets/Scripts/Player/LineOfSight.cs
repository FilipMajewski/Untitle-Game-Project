using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{

    GameObject[] enemies;

    int numberOfEnemies;



    // Use this for initialization
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length != 0 && enemies != null)
        {

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (enemies != null)
            StartCoroutine("ShootRay");
    }

    IEnumerator ShootRay()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, enemies[i].transform.position - (transform.position + Vector3.up)
                , out hit, 200f))
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    enemies[i].GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    enemies[i].GetComponent<MeshRenderer>().enabled = false;
                }
            }

            Debug.DrawRay(transform.position + Vector3.up, enemies[i].transform.position - transform.position, Color.red);

        }

        yield return new WaitForSeconds(.1f);
    }
}
