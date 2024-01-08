using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraCS : MonoBehaviour
{
    // 이 카메라가 따라가야 할 타겟
    public Transform target;
    // 오프셋
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
