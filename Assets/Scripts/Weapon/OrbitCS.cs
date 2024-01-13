using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCS : MonoBehaviour
{
    public Transform target; // 공전 목표
    public float orbitSpeed; // 공전 속도
    Vector3 offSet; // 목표와의 거리

    // Start is called before the first frame update
    void Start()
    {
        offSet = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            // target이 없다면, 필요한 조치를 취합니다 (예: 스크립트 비활성화)
            return;
        }

        // target이 유효할 때만 실행
        transform.position = target.position + offSet;
        transform.RotateAround(target.position, Vector3.up, orbitSpeed*Time.deltaTime);
        offSet = transform.position - target.position;
    }
}
