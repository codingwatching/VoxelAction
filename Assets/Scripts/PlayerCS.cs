using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // State
    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping
    }
    private PlayerState currentState;

    // Jump
    private float lastJumpTime;
    private const float doubleJumpDelay = 0.5f;
    private int jumpCount = 0;

    public float speed; // �ν����Ϳ��� ���� �����ϵ��� public
    public float jumpPower; // �ν����Ϳ��� ���� �����ϵ��� public
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAxis;
    float vAxis;
    bool isWalking;
    bool isJump;
    bool isDodge;
    bool runDown; // KEY: left shift
    bool jumpDown; // KEY: space
    bool dodgeDown; // KEY: left ctrl
    bool iteractionDown; // KEY: e

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidbody;
    Animator animator;

    GameObject nearObject;

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

    // Update is called once per frame �Է� ó���� �ִϸ��̼� ���� �ڵ� 
    void Update()
    {
        GetInput();
        Turn();
        Jump();
        Dodge();
        Interaction();
    }

    // ���� ����� ���õ� �ڵ�
    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    /** �Է� **/
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        jumpDown = Input.GetButtonDown("Jump");
        dodgeDown = Input.GetButtonDown("Dodge");
        iteractionDown = Input.GetButtonDown("Interaction");
    }
    
    /** �̵� **/
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

    /** ȸ�� **/
    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // ���ư��� ������ �ٶ󺸵��� ����
    }

    /** ���� **/
    void Jump()
    {
        bool isDoubleJump = (jumpDown && (Time.time - lastJumpTime < doubleJumpDelay));
        if ((jumpDown && !isJump && !isDodge) || isDoubleJump && jumpCount<2)
        {
            float jumpForce = jumpPower;

            if (isDoubleJump && jumpCount == 1)
            {
                jumpForce *= 1.5f; // ���� ���� �ÿ��� ������ ���� ���
            }

            rigidbody.velocity = Vector3.zero; // �ӵ� �ʱ�ȭ
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
            jumpCount++;
            lastJumpTime = Time.time;
        }
    }

    /** ȸ�� **/
    void Dodge()
    {
        if (dodgeDown && moveVec != Vector3.zero && !isJump && !isDodge)
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

    /** ��ȣ�ۿ�: EŰ **/
    void Interaction()
    {
        if(iteractionDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag=="Weapon") // ���� �Լ�
            {
                ItemCS item = nearObject.GetComponent<ItemCS>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
            jumpCount = 0;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
