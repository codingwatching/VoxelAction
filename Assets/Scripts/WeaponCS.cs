using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCS : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate; // 공격속도
    public BoxCollider meleeArea; // 공격 범위
    public TrailRenderer trailRendererEffect; // 무기 효과

    public void Use()
    {
        // 근접 공격 (망치)
        if (type==Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
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
        yield returnnew WaitForSeconds(0.3f); // 1프레임 대기
        trailRendererEffect.enabled = false;

        yield break;
    }
}
