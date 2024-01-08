using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraCS : MonoBehaviour
{
    public Transform target; // �� ī�޶� ���󰡾� �� Ÿ��
    public Vector3 offset; // ������

    public float Yaxis;
    public float Xaxis;

    private float rotSensitive = 3f; // ī�޶� ȸ�� ����

    private float RotationMin = -10f; // ī�޶� ȸ������ �ּ�
    private float RotationMax = 80f; // ī�޶� ȸ������ �ִ�
    private float smoothTime = 0.12f; // ī�޶� ȸ���ϴµ� �ɸ��� �ð�

    private Vector3 targetRotation;
    private Vector3 currentVel;

    // Update is called once per frame
    void Update()
    {
    } 

    // Player�� �����̰� �� �� ī�޶� ���󰡾� �ϹǷ� LateUpdate
    void LateUpdate() 
    {
        // Xaxis�� ���콺�� �Ʒ��� ������(�������� �Է� �޾�����) ���� �������� ī�޶� �Ʒ��� ȸ���Ѵ� 
        Yaxis = Yaxis + Input.GetAxis("Mouse X") * rotSensitive; // ���콺 �¿�������� �Է¹޾Ƽ� ī�޶��� Y���� ȸ����Ų��
        Xaxis = Xaxis - Input.GetAxis("Mouse Y") * rotSensitive; // ���콺 ���Ͽ������� �Է¹޾Ƽ� ī�޶��� X���� ȸ����Ų��
        
        // X��ȸ���� �Ѱ�ġ�� �����ʰ� �������ش�.
        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        // SmoothDamp�� ���� �ε巯�� ī�޶� ȸ��
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        // ī�޶��� ��ġ�� �÷��̾�� ������ ����ŭ �������ְ� ��� ����ȴ�.
        transform.position = target.position + offset;
    }
}
