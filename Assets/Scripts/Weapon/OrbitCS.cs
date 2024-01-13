using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCS : MonoBehaviour
{
    public Transform target; // ���� ��ǥ
    public float orbitSpeed; // ���� �ӵ�
    Vector3 offSet; // ��ǥ���� �Ÿ�

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
            // target�� ���ٸ�, �ʿ��� ��ġ�� ���մϴ� (��: ��ũ��Ʈ ��Ȱ��ȭ)
            return;
        }

        // target�� ��ȿ�� ���� ����
        transform.position = target.position + offSet;
        transform.RotateAround(target.position, Vector3.up, orbitSpeed*Time.deltaTime);
        offSet = transform.position - target.position;
    }
}
