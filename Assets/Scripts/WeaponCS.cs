using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCS : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate; // ���ݼӵ�
    public int maxAmmo; // ���� ������ �ִ� ź�� ����
    public int curAmmo; // ���� ź�� ��

    public BoxCollider meleeArea; // ���� ����
    public TrailRenderer trailRendererEffect; // ���� ȿ��

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        // ���� ���� (Hammer)
        if (type==Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        // ���Ÿ� ���� (HandGun, SubMachineGun)
        else if (type==Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() // IEnmerator: ������ �Լ� Ŭ���� yield : ����� �����ϴ� Ű����
    {
        // 1
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        meleeArea.enabled = true;
        trailRendererEffect.enabled = true;

        // 2
        yield return new WaitForSeconds(0.3f); // 0.3�� ���
        meleeArea.enabled = false;

        // 3
        yield return new WaitForSeconds(0.3f); // 1������ ���
        trailRendererEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // 1. �Ѿ� �߻�
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigidBody = instantBullet.GetComponent<Rigidbody>();
        bulletRigidBody.velocity = bulletPos.forward * 500;

        yield return null;
        // 2. ź�� ����
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigidBody = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        bulletCaseRigidBody.AddForce(caseVec, ForceMode.Impulse);
        bulletCaseRigidBody.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
