using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraCS : MonoBehaviour
{
    private Camera cam; 

    bool viewDown; //  1-3인칭 시점 변환 KEY: V

    public Transform target; // 이 카메라가 따라가야 할 타겟
    public Vector3 thirdPersonOffset;
    public Vector3 firstPersonOffset;

    public float Yaxis;
    public float Xaxis;

    private float rotSensitive = 5f; // 카메라 회전 감도
    private float clampAngle = 80f; // 카메라 회전각도 제한
    private float smoothTime = 0.1f; // 카메라가 회전하는데 걸리는 시간

    private Vector3 targetRotation;
    private Vector3 currentVel;

    private bool isFirstPersonView = false; // 1-3인칭 시점 변환

    void Awake()
    {
        cam = GetComponent<Camera>(); // 카메라 컴포넌트 참조
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    // Player가 움직이고 그 후 카메라가 따라가야 하므로 LateUpdate
    void LateUpdate()
    {
        // Xaxis는 마우스를 아래로 했을때(음수값이 입력 받아질때) 값이 더해져야 카메라가 아래로 회전한다 
        Yaxis += Input.GetAxis("Mouse X") * rotSensitive; // 마우스 좌우움직임을 입력받아서 카메라의 Y축을 회전시킨다
        Xaxis -= Input.GetAxis("Mouse Y") * rotSensitive; // 마우스 상하움직임을 입력받아서 카메라의 X축을 회전시킨다
        // X축 회전 (고개 위, 아래) 이 한계치를 넘지않게 제한해준다.
        Xaxis = Mathf.Clamp(Xaxis, -clampAngle, clampAngle);

        // SmoothDamp를 통해 부드러운 카메라 회전
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        if (isFirstPersonView)
        {
            transform.position = target.position + firstPersonOffset;
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer("MyPlayer")); // 'MyPlayer' 레이어 숨김
        }
        else
        {
            transform.position = target.position - transform.forward * thirdPersonOffset.magnitude + thirdPersonOffset.y * Vector3.up;
            cam.cullingMask |= 1 << LayerMask.NameToLayer("MyPlayer");  // 'MyPlayer' 레이어 포함
        }
    }
    void GetInput()
    {
        viewDown = Input.GetButtonDown("CameraView");
        if (viewDown)
        {
            isFirstPersonView = !isFirstPersonView;
        }
    }
}


