using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �̴ϸ� ī�޶��� ��ġ ����
public class CopyPosition : MonoBehaviour
{
    [SerializeField]
    private bool x, y, z; // �� ���� true �̸� target�� ��ǥ, false�̸� ���� ��ǥ�� �״�� ����մϴ�.
    [SerializeField]
    private GameObject target; // �Ѿư����� ��� transform

    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player");

        // �Ѿư����� ����� ������ ����
        if (!target) return;

        transform.position = new Vector3(
            (x ? target.transform.position.x : transform.position.x),
            (y ? target.transform.position.y : transform.position.y),
            (z ? target.transform.position.z : transform.position.z));
    }
}
