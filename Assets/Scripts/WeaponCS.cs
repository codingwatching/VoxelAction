using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCS : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate; // 공격속도
    public int maxAmmo; // 소지 가능한 최대 탄약 개수
    public int curAmmo; // 실제 탄약 수

    public BoxCollider meleeArea; // 공격 범위
    public TrailRenderer trailRendererEffect; // 무기 효과

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        // 근접 공격 (Hammer)
        if (type==Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        // 원거리 공격 (HandGun, SubMachineGun)
        else if (type==Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() // IEnmerator: 열거형 함수 클래스 yield : 결과를 전달하는 키워드
    {
        // 1
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        meleeArea.enabled = true;
        trailRendererEffect.enabled = true;

        // 2
        yield return new WaitForSeconds(0.3f); // 0.3초 대기
        meleeArea.enabled = false;

        // 3
        yield return new WaitForSeconds(0.3f); // 1프레임 대기
        trailRendererEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // 1. 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigidBody = instantBullet.GetComponent<Rigidbody>();
        bulletRigidBody.velocity = bulletPos.forward * 500;

        yield return null;
        // 2. 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigidBody = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        bulletCaseRigidBody.AddForce(caseVec, ForceMode.Impulse);
        bulletCaseRigidBody.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
