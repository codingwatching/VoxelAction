using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 미니맵 카메라의 위치 제어
public class CopyPosition : MonoBehaviour
{
    [SerializeField]
    private bool x, y, z; // 이 값이 true 이면 target의 좌표, false이면 현재 좌표를 그대로 사용합니다.
    [SerializeField]
    private GameObject target; // 쫓아가야할 대상 transform

    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player");

        // 쫓아가야할 대상이 없으면 종료
        if (!target) return;

        transform.position = new Vector3(
            (x ? target.transform.position.x : transform.position.x),
            (y ? target.transform.position.y : transform.position.y),
            (z ? target.transform.position.z : transform.position.z));
    }
}
