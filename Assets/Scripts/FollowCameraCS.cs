using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraCS : MonoBehaviour
{
    // �� ī�޶� ���󰡾� �� Ÿ��
    public Transform target;
    // ������
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
