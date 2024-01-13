using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingVoxelCS : MonoBehaviour
{
    public float rotationSpeed = 50f; // ȸ�� �ӵ�
    public float floatingSpeed = 2f; // ���ٴϴ� �ӵ�
    public float floatingHeight = 0.5f; // ���ٴϴ� ����

    private Vector3 startPosition;
    private float originalY;

    private void Start()
    {
        startPosition = transform.position;
        originalY = startPosition.y;
    }

    private void Update()
    {
        // ȸ��
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // ���ٴϱ�
        float newY = originalY + (Mathf.Sin(Time.time * floatingSpeed) * floatingHeight);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
