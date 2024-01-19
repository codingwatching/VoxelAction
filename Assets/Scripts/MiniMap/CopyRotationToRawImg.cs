using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotationToRawImg : MonoBehaviour
{
    [SerializeField]
    private bool x, y, z; // �� ���� true �̸� target�� ��ǥ, false�̸� ���� ��ǥ�� �״�� ����մϴ�.
    [SerializeField]
    private GameObject target; // ������� ��� transform

    void Start()
    {
        target = GameObject.Find("Camera Arm");
    }

    void Update()
    {
        // ī�޶� ������Ʈ�� ������ ����
        if (!target) return;

        // ī�޶��� ȸ�� ���� �����ͼ� �̴ϸ��� ȸ���� �����մϴ�.
        // ���⼭�� y�� ȸ������ ����մϴ�.
        float yRotation = target.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, 0, yRotation);
    }
}
