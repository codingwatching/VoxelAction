using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraCS : MonoBehaviour
{
    bool viewDown; //  1-3인칭 시점 변환 KEY: V

    public Transform target; // 이 카메라가 따라가야 할 타겟
    public Vector3 offset; // 카메라와 타겟 사이의 거리
    public Vector3 thirdPersonOffset; // Distance between camera and target in third-person view
    public Vector3 firstPersonOffset; // Offset for first-person view

    public float Yaxis;
    public float Xaxis;

    private float rotSensitive = 4f; // 카메라 회전 감도
    private float RotationMin = -90f; // 카메라 회전각도 최소
    private float RotationMax = 90f; // 카메라 회전각도 최대
    private float smoothTime = 0.2f; // 카메라가 회전하는데 걸리는 시간

    private Vector3 targetRotation;
    private Vector3 currentVel;

    private bool isFirstPersonView = false; // 1-3인칭 시점 변환

    // Update is called once per frame
    void Update()
    {
        GetInput();
        if(viewDown)
        {
            isFirstPersonView = !isFirstPersonView;
        }
    } 

    // Player가 움직이고 그 후 카메라가 따라가야 하므로 LateUpdate
    void LateUpdate() 
    {
        // Xaxis는 마우스를 아래로 했을때(음수값이 입력 받아질때) 값이 더해져야 카메라가 아래로 회전한다 
        Yaxis += Input.GetAxis("Mouse X") * rotSensitive; // 마우스 좌우움직임을 입력받아서 카메라의 Y축을 회전시킨다
        Xaxis -= Input.GetAxis("Mouse Y") * rotSensitive; // 마우스 상하움직임을 입력받아서 카메라의 X축을 회전시킨다
        // X축 회전 (고개 위, 아래) 이 한계치를 넘지않게 제한해준다.
        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        // SmoothDamp를 통해 부드러운 카메라 회전
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        // 카메라의 위치는 플레이어보다 설정한 값만큼 떨어져있게 계속 변경된다.
        // transform.position = target.position + offset;
        // transform.position = target.position - transform.forward * offset.magnitude + offset.y * Vector3.up;

        if (isFirstPersonView)
        {
            // For first-person view, position the camera at the target's position plus the first-person offset
            transform.position = target.position + firstPersonOffset;
        }
        else
        {
            // For third-person view, adjust the position as originally implemented
            transform.position = target.position - transform.forward * thirdPersonOffset.magnitude + thirdPersonOffset.y * Vector3.up;
        }
    }
    void GetInput()
    {
        viewDown = Input.GetButtonDown("CameraView");

    }
}
