using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCS : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate; // ���ݼӵ�
    public BoxCollider meleeArea; // ���� ����
    public TrailRenderer trailRendererEffect; // ���� ȿ��

    public void Use()
    {
        // ���� ���� (��ġ)
        if (type==Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
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
        yield returnnew WaitForSeconds(0.3f); // 1������ ���
        trailRendererEffect.enabled = false;

        yield break;
    }
}
