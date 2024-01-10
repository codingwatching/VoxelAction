using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingVoxelCS : MonoBehaviour
{
    public float rotationSpeed = 50f; // 회전 속도
    public float floatingSpeed = 2f; // 떠다니는 속도
    public float floatingHeight = 0.5f; // 떠다니는 높이

    private Vector3 startPosition;
    private float originalY;

    private void Start()
    {
        startPosition = transform.position;
        originalY = startPosition.y;
    }

    private void Update()
    {
        // 회전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // 떠다니기
        float newY = originalY + (Mathf.Sin(Time.time * floatingSpeed) * floatingHeight);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
