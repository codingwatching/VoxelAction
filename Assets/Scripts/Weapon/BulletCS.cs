using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCS : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3); //3초 뒤 삭제
        } 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(gameObject.transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}
