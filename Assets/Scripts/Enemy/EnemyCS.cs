using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCS : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target; // 추적할 상대 (플레이어 캐릭터)
    public bool isChase=false;
    public bool isAttack; // 공격 중입니다
    public BoxCollider meleeArea; // 일반형 몬스터 EnemyA 타입의 EnemyBullet

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    Animator animator;
    // NavMeshAgent nav;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        animator = GetComponentInChildren<Animator>();
        // nav = GetComponent<NavMeshAgent>();
        Invoke("ChaseStart", 2); // 생성되면 2초 후에 추적을 시작
    }

    void ChaseStart()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    void Update()
    {
        /*if(nav.enabled)
        {
            // nav.SetDestination(target.position)
            // Navigation.isStopped = !isChase;
        }*/
    }

    void FixedUpdate()
    {
        FreezeVelocity();
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos; // 반작용 방향 구하기 (뒤로 밀려남)

        StartCoroutine(OnDamage(reactVec, true));
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
                StartCoroutine(OnDamage(reactVec, false));

            }
        }
        else if (other.tag == "Bullet")
        {
            BulletCS bullet = other.GetComponent<BulletCS>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position + other.transform.position; // 반작용 방향 구하기 (뒤로 밀려남)
            Destroy(other.gameObject); // 총알과 적이 닿는다면 총알을 제거

            Debug.Log("OnTriggerEnter Bullet : " + curHealth);
            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12; // Layer Idx of "EnemyDead" 

            isChase = false;
            // nav.enabled = false;
            animator.SetTrigger("doDie");

            // 반작용 넉백
            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3f; // 넉백 벡터에 약간의 위 방향 추가
                
                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec*15, ForceMode.Impulse);
            }
            else { 
             reactVec = reactVec.normalized;
             reactVec += Vector3.up * 2f; // 넉백 벡터에 약간의 위 방향 추가
             rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            Destroy(gameObject, 3);
        }
    }
}
