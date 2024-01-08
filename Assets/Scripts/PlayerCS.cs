using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; // �ν����Ϳ��� ���� �����ϵ��� public
    float hAxis;
    float vAxis;
    bool runDown; // left shift
    Vector3 moveVec;
    Animator animator;

    // Initialization
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // ���� �� 1�� ����

        transform.position += moveVec * speed * (runDown ? 2f : 1f) * Time.deltaTime; // �޸��� �� �̵� �ӵ� ����

        animator.SetBool("isWalk", moveVec != Vector3.zero); // �� ������ ����
        animator.SetBool("isRun", moveVec != Vector3.zero && runDown); 
    }
}
