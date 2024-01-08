using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; // 인스펙터에서 설정 가능하도록 public
    public float jumpPower; // 인스펙터에서 설정 가능하도록 public
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

        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 방향 값 1로 보정

        // 회피 중에는 움직임 벡터에서 회피방향 벡터로 바뀌도록 설정
        if (isDodge)
            moveVec = dodgeVec;

        transform.position += moveVec * speed * (runDown ? 2f : 1f) * Time.deltaTime; // 달리기 시 이동 속도 증가

        animator.SetBool("isWalk", isWalking); // 비교 연산자 설정
        animator.SetBool("isRun", moveVec != Vector3.zero && runDown);
    }

    /**회전**/
    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // 나아가는 방향을 바라보도록 설정
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

    /** 회피 **/
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge) // 움직임을 조건으로 추가해서 Jump 와 Dodge 를 나누었습니다.
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

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }
}
