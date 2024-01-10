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
        
        // ������ �ӵ��� ��� Vector3.zero �� �ʱ�ȭ
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        // ����ź ����
        meshObject.SetActive(false);
        effectObject.SetActive(true);

        // ����ź �ǰ�
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 
                                                                                15, 
                                                                                Vector3.up, 
                                                                                0f, 
                                                                                LayerMask.GetMask("Enemy")); // ��ü ����� ����ĳ���� (��� ������Ʈ)

        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<EnemyCS>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
