using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCS : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;

    void Awake()
    {
        Debug.Log("EnemyCS Awake");

        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("EnemyCS OnTriggerEnter");

        if (other.tag == "Melee")
        {
            WeaponCS weapon = other.GetComponent<WeaponCS>();
            if (weapon != null)
            {
                curHealth -= weapon.damage;
                Debug.Log("Melee : " + curHealth);
            }
        }
        else if (other.tag == "Bullet")
        {
            BulletCS bullet = other.GetComponent<BulletCS>();
            curHealth -= bullet.damage;

            Debug.Log("Bullet : " + curHealth);
        }
    }
}
