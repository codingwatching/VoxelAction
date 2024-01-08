using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed; // 인스펙터에서 설정 가능하도록 public
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

        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 방향 값 1로 보정

        transform.position += moveVec * speed * (runDown ? 2f : 1f) * Time.deltaTime; // 달리기 시 이동 속도 증가

        animator.SetBool("isWalk", moveVec != Vector3.zero); // 비교 연산자 설정
        animator.SetBool("isRun", moveVec != Vector3.zero && runDown); 
    }
}
