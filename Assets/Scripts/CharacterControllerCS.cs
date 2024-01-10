using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerCS : MonoBehaviour { 
    // State
    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Reloading
    }
    private PlayerState currentState;

    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;
    [SerializeField]
    private float rotSensitivity = 10f; // 카메라 회전 감도
    public Camera followCam;
    public GameObject f;

    // Jump
    private float lastJumpTime;
    private const float doubleJumpDelay = 0.5f;
    private int jumpCount = 0;

    public float speed; // 인스펙터에서 설정 가능하도록 public
    public float rotateSpeed;
    public float jumpPower; // 인스펙터에서 설정 가능하도록 public
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject grenadeObject;

    public float grenadeChargeForce = 0f; // 수류탄 던지는 힘
    private float grenadeChargeTime = 1f;
    private const float maxGrenadeChargeTime = 4f;

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
    bool reloadDown; // Key: R
    bool grenadeDown; // Key: (default) right mouse hold
    bool grenadeUp; // Key: (default) right mouse up

    bool isWalking;
    bool isJump;
    bool isDodge;
    bool isSwap; // 교체 시간차를 위한 플래그 로직
    bool isFireReady = true; // fireDelayTime 을 기다리면 공격 가능!
    bool isReload;
    bool isBorder; // 벽 충돌 플래그
    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidbody;
    Animator animator;

    GameObject nearObject;
    WeaponCS equippedWeapon;
    int equippedWeaponIndex = -1; // 인벤토리 초기화

    float fireDelayTime;
    private float originalSpeed; // Store original speed for dodging

    // Initialization
    void Awake()
    {
        currentState = PlayerState.Idle;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        originalSpeed = speed; // Store the original speed
    }

    // Update is called once per frame 입력 처리나 애니메이션 관련 코드 
    void Update()
    {
        GetInput();
        Swap();
        Interaction();
        Attack();
        Reload();
        LookAround();
        Jump();
        Grenade();
    }

    // 물리 연산과 관련된 코드
    void FixedUpdate()
    {
        Move();
        Turn();
        Dodge();
        
        FreezeRotation();
        StopToWall();
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
        fireDown = Input.GetButton("Fire1");
        reloadDown = Input.GetButtonDown("Reload");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
        grenadeDown = Input.GetButton("Fire2"); // 꾹 눌러서 수류탄 던질 힘을 축적할 수 있습니다.
        grenadeUp = Input.GetButtonUp("Fire2"); // 떼어서 수류탄 발사
    }

    /** 마우스 움직임에 따라 카메라 회전 **/
    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 80f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x * rotSensitivity, camAngle.z); // 국내에서는 일치한다
        // cameraArm.rotation = Quaternion.Euler(camAngle.x + mouseDelta.y, camAngle.y + mouseDelta.x, camAngle.z); // 해외
    }

    /** 이동 **/
    void Move()
    {
        if (currentState == PlayerState.Reloading) return; // Reloading 상태일 때 이동 방지

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isWalking = moveInput.magnitude != 0;

        if (isWalking)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
            characterBody.forward = moveDir;

            if(!isBorder)
                transform.position += moveDir * speed * (runDown ? 2f : 1f) * Time.deltaTime; // 달리기 시 이동 속도 증가

            isWalking = moveDir != Vector3.zero;

            // 회피 중에는 움직임 벡터에서 회피방향 벡터로 바뀌도록 설정
            if (isDodge)
                moveDir = dodgeVec;

            // 무기 교체, 망치를 휘두르고 있는 중에는 정지
            if (isSwap || !isFireReady || isReload)
                moveDir = Vector3.zero;
        }
        animator.SetBool("isWalk", isWalking); // 비교 연산자 설정
        animator.SetBool("isRun", characterBody.forward != Vector3.zero && runDown);
    }

    // 회전 
    void Turn()
    {
        if (currentState == PlayerState.Reloading) return; // Reloading 상태일 때 회전 방지

        // 카메라의 방향을 기준으로 캐릭터가 회전하도록 설정
        Vector3 lookDirection = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            characterBody.rotation = Quaternion.Slerp(characterBody.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }

        /*// 마우스에 의한 회전
        if(fireDown)
        {
            Ray cameraRay = followCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(cameraRay.origin, cameraRay.direction*20, Color.red, 10f);
                Debug.Log(hit);
                Vector3 targetPoint = hit.point;
                Debug.Log(targetPoint);
                targetPoint.y = characterBody.position.y; // 캐릭터의 높이를 유지하도록 Y 축 조정
                Debug.Log(targetPoint.y);
                Vector3 directionToLook = targetPoint - characterBody.position; // 바라볼 방향 계산
                Debug.Log(directionToLook);
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                Debug.Log("마우스에 의한 회전 7 ");
                characterBody.rotation = Quaternion.Slerp(characterBody.rotation, targetRotation, Time.deltaTime * rotateSpeed);
                Debug.Log("마우스에 의한 회전 8 ");
            }
        }*/
    }

    /** 점프, 더블 점프 **/
    void Jump()
    {
        SetJetActive(isJump);
        bool isDoubleJump = (jumpDown && (Time.time - lastJumpTime < doubleJumpDelay));
        if ((jumpDown && !isJump && !isDodge && !isSwap) || isDoubleJump && jumpCount < 2)
        {
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

    void SetJetActive(bool active)
    {
        leftzet.SetActive(active);
        rightzet.SetActive(active);
    }

    /** 회피 **/
    void Dodge()
    {
        if (dodgeDown && characterBody.forward != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = characterBody.forward;
            originalSpeed = speed; // 현재 속도 저장

            speed *= 2; // 회피는 이동 속도의 2배
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f); // 시간차를 두어서 함수를 호출
        }
    }

    void DodgeOut()
    {
        speed = originalSpeed; // 원래 속도로 복원
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
        if (swapDown3) weaponIndex = 2;

        if (swapDown1 || swapDown2 || swapDown3 && !isJump && !isDodge)
        {
            if (equippedWeapon != null)
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

        if (fireDown && isFireReady && !isDodge && !isSwap)
        {
            equippedWeapon.Use();
            animator.SetTrigger(equippedWeapon.type == WeaponCS.Type.Melee ?  "doSwing" : "doShot"); // HandGun 또는 SubMachineGun
            fireDelayTime = 0; // 공격했으니 초기화
            Debug.Log("Swing");
        }
    }

    void Reload()
    {
        // 근접무기이거나 총알이나 무기가 없다면 return
        if (equippedWeapon == null) return;
        if (equippedWeapon.type == WeaponCS.Type.Melee) return;
        if (ammo == 0) return;

        // 재장전 가능
        if (reloadDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            currentState = PlayerState.Reloading; // 상태를 Reloading으로 설정
            animator.SetTrigger("doReload");
            isReload = true;
            Invoke("ReloadOut", 2.3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equippedWeapon.maxAmmo ? ammo : equippedWeapon.maxAmmo;
        equippedWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
        currentState = PlayerState.Idle; // 재장전이 끝나면 Idle 상태로 복귀
    }

    /** 수류탄 **/
    void Grenade()
    {
        // 마우스 우클릭을 누르고 있는 동안, grenadeChargeTime을 증가시킵니다.
        if (grenadeDown && !isReload && !isSwap)
        {
            // 충전 중에는 수류탄이 발사되지 않도록 합니다.
            if (grenadeChargeTime < maxGrenadeChargeTime)
            {
                grenadeChargeTime += Time.deltaTime;
            }
        }

        // 마우스 우클릭을 떼었을 때, 충전된 힘으로 수류탄을 발사합니다.
        if (grenadeUp && !isReload && !isSwap)
        {
            if (grenadeChargeTime > 1f && grenadeChargeTime <= maxGrenadeChargeTime)
            {
                ThrowGrenade();
                Debug.Log("grenadeChargeTime" + grenadeChargeTime);
            }
            // 수류탄 발사 후에는 충전 시간을 1으로 초기화합니다.
            grenadeChargeTime = 1f;
        }
    }
    /** 수류탄 던지기 **/
    void ThrowGrenade()
    {
        if (grenadeObject != null && hasGrenades > 0)
        {
            // 목표 지점 계산
            Vector3 targetPosition = characterBody.position + characterBody.forward * 5f; // 캐릭터 앞으로 5 미터
            Vector3 throwDirection = (targetPosition - characterBody.position).normalized;

            // 발사 각도 및 힘 계산
            float throwAngle = CalculateThrowAngle(characterBody.position, targetPosition);
            float throwForce = CalculateThrowForce(characterBody.position, targetPosition, throwAngle);

            // 수류탄 발사 위치 조정
            Vector3 grenadeSpawnPosition = characterBody.position + characterBody.up * 1.5f + characterBody.forward * 1.5f;
            GameObject grenade = Instantiate(grenadeObject, grenadeSpawnPosition, Quaternion.identity);
            Rigidbody grenadeRigidbody = grenade.GetComponent<Rigidbody>();

            // 수류탄에 힘을 가합니다.
            Vector3 throwVector = Quaternion.AngleAxis(throwAngle, characterBody.right) * throwDirection;
            grenadeRigidbody.AddForce(throwVector * throwForce, ForceMode.VelocityChange);
            grenadeRigidbody.AddTorque(Vector3.back * 10, ForceMode.Impulse);

            // 수류탄 개수 감소
            hasGrenades--;
            grenades[hasGrenades].SetActive(false);
        }
    }

    /** 수류탄 발사 각도 계산 함수 **/
    float CalculateThrowAngle(Vector3 startPosition, Vector3 targetPosition)
    {
        // 여기에 발사 각도 계산 로직을 추가
        return 45.0f; // 임시 값
    }

    /** 수류탄 발사 힘 계산 함수 **/
    float CalculateThrowForce(Vector3 startPosition, Vector3 targetPosition, float angle)
    {
        // 여기에 발사 힘 계산 로직을 추가
        return 10.0f * grenadeChargeTime; // 임시 값
    }

    /** 상호작용: E키 **/
    void Interaction()
    {
        if (iteractionDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon") // 무기 입수
            {
                ItemCS item = nearObject.GetComponent<ItemCS>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void FreezeRotation()
    {
        rigidbody.angularVelocity = Vector3.zero; // 물리 회전 속도를 0으로 설정
    } 

    /* 플레이어가 벽을 뚫고 나가는 문제 해결 */
    void StopToWall()
    {
        Debug.DrawRay(characterBody.position, characterBody.forward *5, Color.green);
        isBorder = Physics.Raycast(characterBody.position, characterBody.forward, 5, LayerMask.GetMask("Wall")); // Wall 과 부딪히면 True
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJump", false);
            isJump = false;
            jumpCount = 0;

            SetJetActive(isJump);
            Debug.Log("SetActive False!!!!!!!!!!");
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
