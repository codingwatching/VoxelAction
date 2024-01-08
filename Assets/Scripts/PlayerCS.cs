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

        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 방향 값 1로 보정

        transform.position += moveVec * speed * (runDown ? 2f : 1f) * Time.deltaTime; // 달리기 시 이동 속도 증가

        animator.SetBool("isWalk", isWalking); // 비교 연산자 설정
        animator.SetBool("isRun", moveVec != Vector3.zero && runDown);
    }

    void Turn()
    {
        // 회전
        transform.LookAt(transform.position + moveVec); // 나아가는 방향을 바라보도록 설정
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
