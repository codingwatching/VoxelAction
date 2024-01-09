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

    public float speed; // �ν����Ϳ��� ���� �����ϵ��� public
    public float rotateSpeed;
    public float jumpPower; // �ν����Ϳ��� ���� �����ϵ��� public
    public GameObject[] weapons;
    public bool[] hasWeapons;
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

    bool isWalking;
    bool isJump;
    bool isDodge;
    bool isSwap; // ��ü �ð����� ���� �÷��� ����
    bool isFireReady; // fireDelayTime �� ��ٸ��� ���� ����!
    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidbody;
    Animator animator;

    GameObject nearObject;
    WeaponCS equippedWeapon;
    int equippedWeaponIndex = -1; // �κ��丮 �ʱ�ȭ

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

    // Update is called once per frame �Է� ó���� �ִϸ��̼� ���� �ڵ� 
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

    // ���� ����� ���õ� �ڵ�
    private void FixedUpdate()
    {
        Move();
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
        fireDown = Input.GetButtonDown("Fire1");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");

    }
    
    /** �̵� **/
    void Move()
    {
        isWalking = moveVec != Vector3.zero;

        // moveVec = new Vector3(hAxis, 0, vAxis).normalized; // ���� �� 1�� ����
        // ī�޶��� ���⿡ ���� �̵� ���͸� �����մϴ�.
        moveVec = cameraTransform.right * hAxis + cameraTransform.forward * vAxis;
        moveVec.y = 0; // y�� �̵� ����

        // ȸ�� �߿��� ������ ���Ϳ��� ȸ�ǹ��� ���ͷ� �ٲ�� ����
        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap)
            moveVec = Vector3.zero;

        transform.position += moveVec * speed * (runDown ? 2f : 1f) * Time.deltaTime; // �޸��� �� �̵� �ӵ� ����
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVec), Time.deltaTime * rotateSpeed);

        animator.SetBool("isWalk", isWalking); // �� ������ ����
        animator.SetBool("isRun", moveVec != Vector3.zero && runDown);
    }

    /** ȸ�� **/
    void Turn()
    {
        // transform.LookAt(transform.position + moveVec); // ���ư��� ������ �ٶ󺸵��� ����
        
        // ī�޶��� y�� ȸ�� ���� �����ɴϴ�.
        Vector3 cameraRotation = cameraTransform.eulerAngles;
        cameraRotation.x = 0;
        cameraRotation.z = 0;

        // �÷��̾ �̵� ���� ���� ȸ���մϴ�.
        if (moveVec != Vector3.zero)
        {
            // �÷��̾ �̵��ϴ� �������� ȸ���մϴ�.
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);

            // ������ ����Ͽ� �ε巯�� ȸ���� �����մϴ�.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    /** ����, ���� ���� **/
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
        if (dodgeDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
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

        fireDelayTime += Time.deltaTime; // ���ݵ����̿� �ð��� �����ְ� ���� ���� ���θ� Ȯ��
        isFireReady = equippedWeapon.rate < fireDelayTime; // ���ݼӵ����� �ð��� Ŀ����, ���� ����
        
        if(fireDown && isFireReady && !isDodge && !isSwap)
        {
            equippedWeapon.Use();
            animator.SetTrigger("doSwing");
            fireDelayTime = 0; // ���������� �ʱ�ȭ
            Debug.Log("Swing");
        }
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

            leftzet.SetActive(false);
            rightzet.SetActive(false);
            Debug.Log("SetActive False");
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
