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

    // Camera
    private Transform cameraTransform;

    // Jump
    private float lastJumpTime;
    private const float doubleJumpDelay = 0.5f;
    private int jumpCount = 0;

    public float speed; // 인스펙터에서 설정 가능하도록 public
    public float rotateSpeed;
    public float jumpPower; // 인스펙터에서 설정 가능하도록 public
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject leftzet;
    public GameObject rightzet;

    // 소모품
    public int ammo;
    public int coin;
    public int health;
    // 소유 가능한 최대치
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis;
    float vAxis;

    bool runDown; // KEY: left shift
    bool jumpDown; // KEY: space
    bool dodgeDown; // KEY: left ctrl
    bool iteractionDown; // KEY: e
    bool swapDown1; // Weapon KEY: 1
    bool swapDown2; // Weapon KEY: 2
    bool swapDown3; // Weapon KEY: 3
    bool fireDown; // Attack KEY: (default) left mouse click

    bool isWalking;
    bool isJump;
    bool isDodge;
    bool isSwap; // 교체 시간차를 위한 플래그 로직
    bool isFireReady; // fireDelayTime 을 기다리면 공격 가능!
    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidbody;
    Animator animator;

    GameObject nearObject;
    WeaponCS equippedWeapon;
    int equippedWeaponIndex = -1; // 인벤토리 초기화

    float fireDelayTime;

    // Initialization
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame 입력 처리나 애니메이션 관련 코드 
    void Update()
    {
        GetInput();
        Turn();
        Dodge();
        Swap();
        Interaction();
        Jump();
        Attack();
    }

    // 물리 연산과 관련된 코드
    private void FixedUpdate()
    {
        Move();
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
        fireDown = Input.GetButtonDown("Fire1");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");

    }
    
    /** 이동 **/
    void Move()
    {
        isWalking = moveVec != Vector3.zero;

        // moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 방향 값 1로 보정
        // 카메라의 방향에 따라 이동 벡터를 조정합니다.
        moveVec = cameraTransform.right * hAxis + cameraTransform.forward * vAxis;
        moveVec.y = 0; // y축 이동 제거

        // 회피 중에는 움직임 벡터에서 회피방향 벡터로 바뀌도록 설정
        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap)
            moveVec = Vector3.zero;

        transform.position += moveVec * speed * (runDown ? 2f : 1f) * Time.deltaTime; // 달리기 시 이동 속도 증가
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVec), Time.deltaTime * rotateSpeed);

        animator.SetBool("isWalk", isWalking); // 비교 연산자 설정
        animator.SetBool("isRun", moveVec != Vector3.zero && runDown);
    }

    /** 회전 **/
    void Turn()
    {
        // transform.LookAt(transform.position + moveVec); // 나아가는 방향을 바라보도록 설정
        
        // 카메라의 y축 회전 값만 가져옵니다.
        Vector3 cameraRotation = cameraTransform.eulerAngles;
        cameraRotation.x = 0;
        cameraRotation.z = 0;

        // 플레이어가 이동 중일 때만 회전합니다.
        if (moveVec != Vector3.zero)
        {
            // 플레이어가 이동하는 방향으로 회전합니다.
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);

            // 보간을 사용하여 부드러운 회전을 구현합니다.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    /** 점프, 더블 점프 **/
    void Jump()
    {
        bool isDoubleJump = (jumpDown && (Time.time - lastJumpTime < doubleJumpDelay));
        if ((jumpDown && !isJump && !isDodge && !isSwap) || isDoubleJump && jumpCount<2)
        {
            leftzet.SetActive(true);
            rightzet.SetActive(true);
            Debug.Log("SetActive True");
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
        if (dodgeDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
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
        if (swapDown1 && (!hasWeapons[0] || equippedWeaponIndex == 0))
            return;
        if (swapDown2 && (!hasWeapons[1] || equippedWeaponIndex == 1))
            return;
        if (swapDown3 && (!hasWeapons[2] || equippedWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (swapDown1) weaponIndex = 0;
        if (swapDown2) weaponIndex = 1;
        if(swapDown3) weaponIndex = 2;

        if(swapDown1 || swapDown2 || swapDown3 && !isJump && !isDodge)
        {
            if(equippedWeapon!=null)
                equippedWeapon.gameObject.SetActive(false);

            equippedWeaponIndex = weaponIndex;
            equippedWeapon = weapons[weaponIndex].GetComponent<WeaponCS>();
            equippedWeapon.gameObject.SetActive(true);

            animator.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Attack()
    {
        
        if (equippedWeapon == null) return;

        fireDelayTime += Time.deltaTime; // 공격딜레이에 시간을 더해주고 공격 가능 여부를 확인
        isFireReady = equippedWeapon.rate < fireDelayTime; // 공격속도보다 시간이 커지면, 공격 가능
        
        if(fireDown && isFireReady && !isDodge && !isSwap)
        {
            equippedWeapon.Use();
            animator.SetTrigger("doSwing");
            fireDelayTime = 0; // 공격했으니 초기화
            Debug.Log("Swing");
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

            leftzet.SetActive(false);
            rightzet.SetActive(false);
            Debug.Log("SetActive False");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 아이템 입수
        if (other.tag == "Item")
        {
            ItemCS item = other.GetComponent<ItemCS>();
            switch (item.type)
            {
                case ItemCS.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case ItemCS.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case ItemCS.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case ItemCS.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        // 무기 입수
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
