using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; // �ν����Ϳ��� ���� �����ϵ��� public
    public float jumpPower; // �ν����Ϳ��� ���� �����ϵ��� public
    float hAxis;
    float vAxis;
    bool isWalking;
    bool isJump;
    bool runDown; // left shift
    bool jDown; // jump

    Vector3 moveVec;
    Rigidbody rigidbody;
    Animator animator;

    // Initialization
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        jDown = Input.GetButtonDown("Jump");
    }
    
    void Move()
    {
        isWalking = moveVec != Vector3.zero;

        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // ���� �� 1�� ����

        transform.position += moveVec * speed * (runDown ? 2f : 1f) * Time.deltaTime; // �޸��� �� �̵� �ӵ� ����

        animator.SetBool("isWalk", isWalking); // �� ������ ����
        animator.SetBool("isRun", moveVec != Vector3.zero && runDown);
    }

    void Turn()
    {
        // ȸ��
        transform.LookAt(transform.position + moveVec); // ���ư��� ������ �ٶ󺸵��� ����
    }

    void Jump()
    {
        if (jDown && !isJump)
        {
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true); 
            animator.SetTrigger("doJump"); 
            isJump = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }
}
