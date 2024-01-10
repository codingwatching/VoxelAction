using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCS : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    void Awake()
    {
        Debug.Log("EnemyCS Awake");

        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
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
                Vector3 reactVec = transform.position - other.transform.position; // 반작용 방향 구하기

                Debug.Log("OnTriggerEnter Melee : " + curHealth);
                StartCoroutine(OnDamage(reactVec));

            }
        }
        else if (other.tag == "Bullet")
        {
            BulletCS bullet = other.GetComponent<BulletCS>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position; // 반작용 방향 구하기
            Destroy(other.gameObject); // 총알과 적이 닿는다면 총알을 제거

            Debug.Log("OnTriggerEnter Bullet : " + curHealth);
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 14;
            
            // 반작용 넉백
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            
            Destroy(gameObject, 4);
        }
    }
}
