using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCS : MonoBehaviour
{
    public GameObject meshObject;
    public GameObject effectObject;
    public Rigidbody rigid;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        
        // 물리적 속도를 모두 Vector3.zero 로 초기화
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        // 수류탄 제거
        meshObject.SetActive(false);
        effectObject.SetActive(true);

        // 수류탄 피격
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 
                                                                                15, 
                                                                                Vector3.up, 
                                                                                0f, 
                                                                                LayerMask.GetMask("Enemy")); // 구체 모양의 레이캐스팅 (모든 오브젝트)

        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<EnemyCS>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
