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
    bool isDodge;
    bool runDown; // left shift
    bool jDown; // space

    Vector3 moveVec;
    Vector3 dodgeVec;

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
        Dodge();
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

        // ȸ�� �߿��� ������ ���Ϳ��� ȸ�ǹ��� ���ͷ� �ٲ�� ����
        if (isDodge)
            moveVec = dodgeVec;

        transform.position += moveVec * speed * (runDown ? 2f : 1f) * Time.deltaTime; // �޸��� �� �̵� �ӵ� ����

        animator.SetBool("isWalk", isWalking); // �� ������ ����
        animator.SetBool("isRun", moveVec != Vector3.zero && runDown);
    }

    /**ȸ��**/
    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // ���ư��� ������ �ٶ󺸵��� ����
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true); 
            animator.SetTrigger("doJump"); 
            isJump = true;
        }
    }

    /** ȸ�� **/
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge) // �������� �������� �߰��ؼ� Jump �� Dodge �� ���������ϴ�.
        {
            dodgeVec = moveVec;
            speed *= 2; // ȸ�Ǵ� �̵� �ӵ��� 2��
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f); // �ð����� �ξ �Լ��� ȣ��
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f; // ȸ�� ���� �Ŀ� �ӵ��� ������� �����ϴ�.
        isDodge = false;
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
