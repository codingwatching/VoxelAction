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
    private float rotSensitivity = 10f; // ī�޶� ȸ�� ����
    public Camera followCam;
    public GameObject f;

    // Jump
    private float lastJumpTime;
    private const float doubleJumpDelay = 0.5f;
    private int jumpCount = 0;

    public float speed; // �ν����Ϳ��� ���� �����ϵ��� public
    public float rotateSpeed;
    public float jumpPower; // �ν����Ϳ��� ���� �����ϵ��� public
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject grenadeObject;

    public float grenadeChargeForce = 0f; // ����ź ������ ��
    private float grenadeChargeTime = 1f;
    private const float maxGrenadeChargeTime = 4f;

    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject leftzet;
    public GameObject rightzet;

    // �Ҹ�ǰ
    public int ammo;
    public int coin;
    public int health;
    // ���� ������ �ִ�ġ
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
    bool isSwap; // ��ü �ð����� ���� �÷��� ����
    bool isFireReady = true; // fireDelayTime �� ��ٸ��� ���� ����!
    bool isReload;
    bool isBorder; // �� �浹 �÷���
    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidbody;
    Animator animator;

    GameObject nearObject;
    WeaponCS equippedWeapon;
    int equippedWeaponIndex = -1; // �κ��丮 �ʱ�ȭ

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

    // Update is called once per frame �Է� ó���� �ִϸ��̼� ���� �ڵ� 
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

    // ���� ����� ���õ� �ڵ�
    void FixedUpdate()
    {
        Move();
        Turn();
        Dodge();
        
        FreezeRotation();
        StopToWall();
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
        fireDown = Input.GetButton("Fire1");
        reloadDown = Input.GetButtonDown("Reload");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
        grenadeDown = Input.GetButton("Fire2"); // �� ������ ����ź ���� ���� ������ �� �ֽ��ϴ�.
        grenadeUp = Input.GetButtonUp("Fire2"); // ��� ����ź �߻�
    }

    /** ���콺 �����ӿ� ���� ī�޶� ȸ�� **/
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
        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x * rotSensitivity, camAngle.z); // ���������� ��ġ�Ѵ�
        // cameraArm.rotation = Quaternion.Euler(camAngle.x + mouseDelta.y, camAngle.y + mouseDelta.x, camAngle.z); // �ؿ�
    }

    /** �̵� **/
    void Move()
    {
        if (currentState == PlayerState.Reloading) return; // Reloading ������ �� �̵� ����

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isWalking = moveInput.magnitude != 0;

        if (isWalking)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
            characterBody.forward = moveDir;

            if(!isBorder)
                transform.position += moveDir * speed * (runDown ? 2f : 1f) * Time.deltaTime; // �޸��� �� �̵� �ӵ� ����

            isWalking = moveDir != Vector3.zero;

            // ȸ�� �߿��� ������ ���Ϳ��� ȸ�ǹ��� ���ͷ� �ٲ�� ����
            if (isDodge)
                moveDir = dodgeVec;

            // ���� ��ü, ��ġ�� �ֵθ��� �ִ� �߿��� ����
            if (isSwap || !isFireReady || isReload)
                moveDir = Vector3.zero;
        }
        animator.SetBool("isWalk", isWalking); // �� ������ ����
        animator.SetBool("isRun", characterBody.forward != Vector3.zero && runDown);
    }

    // ȸ�� 
    void Turn()
    {
        if (currentState == PlayerState.Reloading) return; // Reloading ������ �� ȸ�� ����

        // ī�޶��� ������ �������� ĳ���Ͱ� ȸ���ϵ��� ����
        Vector3 lookDirection = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            characterBody.rotation = Quaternion.Slerp(characterBody.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }

        /*// ���콺�� ���� ȸ��
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
                targetPoint.y = characterBody.position.y; // ĳ������ ���̸� �����ϵ��� Y �� ����
                Debug.Log(targetPoint.y);
                Vector3 directionToLook = targetPoint - characterBody.position; // �ٶ� ���� ���
                Debug.Log(directionToLook);
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                Debug.Log("���콺�� ���� ȸ�� 7 ");
                characterBody.rotation = Quaternion.Slerp(characterBody.rotation, targetRotation, Time.deltaTime * rotateSpeed);
                Debug.Log("���콺�� ���� ȸ�� 8 ");
            }
        }*/
    }

    /** ����, ���� ���� **/
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

    void SetJetActive(bool active)
    {
        leftzet.SetActive(active);
        rightzet.SetActive(active);
    }

    /** ȸ�� **/
    void Dodge()
    {
        if (dodgeDown && characterBody.forward != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = characterBody.forward;
            originalSpeed = speed; // ���� �ӵ� ����

            speed *= 2; // ȸ�Ǵ� �̵� �ӵ��� 2��
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f); // �ð����� �ξ �Լ��� ȣ��
        }
    }

    void DodgeOut()
    {
        speed = originalSpeed; // ���� �ӵ��� ����
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

        fireDelayTime += Time.deltaTime; // ���ݵ����̿� �ð��� �����ְ� ���� ���� ���θ� Ȯ��
        isFireReady = equippedWeapon.rate < fireDelayTime; // ���ݼӵ����� �ð��� Ŀ����, ���� ����

        if (fireDown && isFireReady && !isDodge && !isSwap)
        {
            equippedWeapon.Use();
            animator.SetTrigger(equippedWeapon.type == WeaponCS.Type.Melee ?  "doSwing" : "doShot"); // HandGun �Ǵ� SubMachineGun
            fireDelayTime = 0; // ���������� �ʱ�ȭ
            Debug.Log("Swing");
        }
    }

    void Reload()
    {
        // ���������̰ų� �Ѿ��̳� ���Ⱑ ���ٸ� return
        if (equippedWeapon == null) return;
        if (equippedWeapon.type == WeaponCS.Type.Melee) return;
        if (ammo == 0) return;

        // ������ ����
        if (reloadDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            currentState = PlayerState.Reloading; // ���¸� Reloading���� ����
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
        currentState = PlayerState.Idle; // �������� ������ Idle ���·� ����
    }

    /** ����ź **/
    void Grenade()
    {
        // ���콺 ��Ŭ���� ������ �ִ� ����, grenadeChargeTime�� ������ŵ�ϴ�.
        if (grenadeDown && !isReload && !isSwap)
        {
            // ���� �߿��� ����ź�� �߻���� �ʵ��� �մϴ�.
            if (grenadeChargeTime < maxGrenadeChargeTime)
            {
                grenadeChargeTime += Time.deltaTime;
            }
        }

        // ���콺 ��Ŭ���� ������ ��, ������ ������ ����ź�� �߻��մϴ�.
        if (grenadeUp && !isReload && !isSwap)
        {
            if (grenadeChargeTime > 1f && grenadeChargeTime <= maxGrenadeChargeTime)
            {
                ThrowGrenade();
                Debug.Log("grenadeChargeTime" + grenadeChargeTime);
            }
            // ����ź �߻� �Ŀ��� ���� �ð��� 1���� �ʱ�ȭ�մϴ�.
            grenadeChargeTime = 1f;
        }
    }
    /** ����ź ������ **/
    void ThrowGrenade()
    {
        if (grenadeObject != null && hasGrenades > 0)
        {
            // ��ǥ ���� ���
            Vector3 targetPosition = characterBody.position + characterBody.forward * 5f; // ĳ���� ������ 5 ����
            Vector3 throwDirection = (targetPosition - characterBody.position).normalized;

            // �߻� ���� �� �� ���
            float throwAngle = CalculateThrowAngle(characterBody.position, targetPosition);
            float throwForce = CalculateThrowForce(characterBody.position, targetPosition, throwAngle);

            // ����ź �߻� ��ġ ����
            Vector3 grenadeSpawnPosition = characterBody.position + characterBody.up * 1.5f + characterBody.forward * 1.5f;
            GameObject grenade = Instantiate(grenadeObject, grenadeSpawnPosition, Quaternion.identity);
            Rigidbody grenadeRigidbody = grenade.GetComponent<Rigidbody>();

            // ����ź�� ���� ���մϴ�.
            Vector3 throwVector = Quaternion.AngleAxis(throwAngle, characterBody.right) * throwDirection;
            grenadeRigidbody.AddForce(throwVector * throwForce, ForceMode.VelocityChange);
            grenadeRigidbody.AddTorque(Vector3.back * 10, ForceMode.Impulse);

            // ����ź ���� ����
            hasGrenades--;
            grenades[hasGrenades].SetActive(false);
        }
    }

    /** ����ź �߻� ���� ��� �Լ� **/
    float CalculateThrowAngle(Vector3 startPosition, Vector3 targetPosition)
    {
        // ���⿡ �߻� ���� ��� ������ �߰�
        return 45.0f; // �ӽ� ��
    }

    /** ����ź �߻� �� ��� �Լ� **/
    float CalculateThrowForce(Vector3 startPosition, Vector3 targetPosition, float angle)
    {
        // ���⿡ �߻� �� ��� ������ �߰�
        return 10.0f * grenadeChargeTime; // �ӽ� ��
    }

    /** ��ȣ�ۿ�: EŰ **/
    void Interaction()
    {
        if (iteractionDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon") // ���� �Լ�
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
        rigidbody.angularVelocity = Vector3.zero; // ���� ȸ�� �ӵ��� 0���� ����
    } 

    /* �÷��̾ ���� �հ� ������ ���� �ذ� */
    void StopToWall()
    {
        Debug.DrawRay(characterBody.position, characterBody.forward *5, Color.green);
        isBorder = Physics.Raycast(characterBody.position, characterBody.forward, 5, LayerMask.GetMask("Wall")); // Wall �� �ε����� True
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
        // ������ �Լ�
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
        // ���� �Լ�
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
