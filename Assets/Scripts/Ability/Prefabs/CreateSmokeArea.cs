using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSmokeArea : MonoBehaviour
{

    public GameObject smokePrefab;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(smokePrefab, transform.position, Quaternion.AngleAxis(-90, Vector3.right), collision.transform);
        Destroy(gameObject);
    }
}
