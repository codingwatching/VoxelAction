using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotationToRawImg : MonoBehaviour
{
    [SerializeField]
    private bool x, y, z; // 이 값이 true 이면 target의 좌표, false이면 현재 좌표를 그대로 사용합니다.
    [SerializeField]
    private GameObject target; // 따라야할 대상 transform

    void Start()
    {
        target = GameObject.Find("Camera Arm");
    }

    void Update()
    {
        // 카메라 오브젝트가 없으면 종료
        if (!target) return;

        // 카메라의 회전 값을 가져와서 미니맵의 회전에 적용합니다.
        // 여기서는 y축 회전만을 고려합니다.
        float yRotation = target.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, 0, yRotation);
    }
}
