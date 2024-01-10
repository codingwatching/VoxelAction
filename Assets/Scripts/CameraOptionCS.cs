using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOptionCS : MonoBehaviour
{
    private Camera cam; 

    bool viewDown; //  1-3인칭 시점 변환 KEY: V

    public Transform target; // 이 카메라가 따라가야 할 타겟
    public Vector3 thirdPersonOffset;
    public Vector3 firstPersonOffset;

    private bool isFirstPersonView = false; // 1-3인칭 시점 변환


    void Awake()
    {
        cam = GetComponent<Camera>(); // 카메라 컴포넌트 참조
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ChangeCameraView();
    }

    void GetInput()
    {
        viewDown = Input.GetButtonDown("CameraView");
        if (viewDown)
        {
            isFirstPersonView = !isFirstPersonView;
        }
    }

    // Option: 1-3인칭 시점 변환
    void ChangeCameraView()
    {
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

    // Option: Self Cam
}


