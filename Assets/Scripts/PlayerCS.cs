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

    public float speed; // 인스펙터에서 설정 가능하도록 public
    public float jumpPower; // 인스펙터에서 설정 가능하도록 public
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
    bool swapDown1; // Weapon KEY: 1
    bool swapDown2; // Weapon KEY: 2
    bool swapDown3; // Weapon KEY: 3

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

    // Update is called once per frame 입력 처리나 애니메이션 관련 코드 
    void Update()
    {
        GetInput();
        Turn();
        Jump();
        Dodge();
        Swap();
        Interaction();
    }

    // 물리 연산과 관련된 코드
    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    /** 입력 **/
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        runDown = Input.GetButton("Run");
        jumpDown = Input.GetButtonDown("Jump");
        dodgeDown = Input.GetButtonDown("Dodge");
        iteractionDown = Input.GetButtonDown("Interaction");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
    }
    
    /** 이동 **/
    void Move()
    {
        isWalking = moveVec != Vector3.zero;

        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 방향 값 1로 보정

        // 회피 중에는 움직임 벡터에서 회피방향 벡터로 바뀌도록 설정
        if (isDodge)
            moveVec = dodgeVec;

        transform.position += moveVec * speed * (runDown ? 2f : 1f) * Time.deltaTime; // 달리기 시 이동 속도 증가

        animator.SetBool("isWalk", isWalking); // 비교 연산자 설정
        animator.SetBool("isRun", moveVec != Vector3.zero && runDown);
    }

    /** 회전 **/
    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // 나아가는 방향을 바라보도록 설정
    }

    /** 점프 **/
    void Jump()
    {
        bool isDoubleJump = (jumpDown && (Time.time - lastJumpTime < doubleJumpDelay));
        if ((jumpDown && !isJump && !isDodge) || isDoubleJump && jumpCount<2)
        {
            float jumpForce = jumpPower;

            if (isDoubleJump && jumpCount == 1)
            {
                jumpForce *= 1.5f; // 더블 점프 시에만 증가된 힘을 사용
            }

            rigidbody.velocity = Vector3.zero; // 속도 초기화
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
            jumpCount++;
            lastJumpTime = Time.time;
        }
    }

    /** 회피 **/
    void Dodge()
    {
        if (dodgeDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2; // 회피는 이동 속도의 2배
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f); // 시간차를 두어서 함수를 호출
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f; // 회피 동작 후에 속도를 원래대로 돌립니다.
        isDodge = false;
    }

    void Swap()
    {
        int weaponIndex = -1;
        if (swapDown1) weaponIndex = 0;
        if (swapDown2) weaponIndex = 1;
        if(swapDown3) weaponIndex = 2;

        if(swapDown1 || swapDown2 || swapDown3 && !isJump && !isDodge)
        {
            weapons[weaponIndex].SetActive(true);
        }
    }

    /** 상호작용: E키 **/
    void Interaction()
    {
        if(iteractionDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag=="Weapon") // 무기 입수
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
