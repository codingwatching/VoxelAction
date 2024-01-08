using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraCS : MonoBehaviour
{
    bool viewDown; //  1-3��Ī ���� ��ȯ KEY: V

    public Transform target; // �� ī�޶� ���󰡾� �� Ÿ��
    public Vector3 offset; // ī�޶�� Ÿ�� ������ �Ÿ�
    public Vector3 thirdPersonOffset; // Distance between camera and target in third-person view
    public Vector3 firstPersonOffset; // Offset for first-person view

    public float Yaxis;
    public float Xaxis;

    private float rotSensitive = 4f; // ī�޶� ȸ�� ����
    private float RotationMin = -90f; // ī�޶� ȸ������ �ּ�
    private float RotationMax = 90f; // ī�޶� ȸ������ �ִ�
    private float smoothTime = 0.2f; // ī�޶� ȸ���ϴµ� �ɸ��� �ð�

    private Vector3 targetRotation;
    private Vector3 currentVel;

    private bool isFirstPersonView = false; // 1-3��Ī ���� ��ȯ

    // Update is called once per frame
    void Update()
    {
        GetInput();
        if(viewDown)
        {
            isFirstPersonView = !isFirstPersonView;
        }
    } 

    // Player�� �����̰� �� �� ī�޶� ���󰡾� �ϹǷ� LateUpdate
    void LateUpdate() 
    {
        // Xaxis�� ���콺�� �Ʒ��� ������(�������� �Է� �޾�����) ���� �������� ī�޶� �Ʒ��� ȸ���Ѵ� 
        Yaxis += Input.GetAxis("Mouse X") * rotSensitive; // ���콺 �¿�������� �Է¹޾Ƽ� ī�޶��� Y���� ȸ����Ų��
        Xaxis -= Input.GetAxis("Mouse Y") * rotSensitive; // ���콺 ���Ͽ������� �Է¹޾Ƽ� ī�޶��� X���� ȸ����Ų��
        // X�� ȸ�� (�� ��, �Ʒ�) �� �Ѱ�ġ�� �����ʰ� �������ش�.
        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        // SmoothDamp�� ���� �ε巯�� ī�޶� ȸ��
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        // ī�޶��� ��ġ�� �÷��̾�� ������ ����ŭ �������ְ� ��� ����ȴ�.
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
