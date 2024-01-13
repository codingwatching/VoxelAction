using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCS : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate; // ���ݼӵ�
    public int maxAmmo; // ���� ������ �ִ� ź�� ����
    public int curAmmo; // ���� ź�� ��

    public BoxCollider meleeArea; // ���� ����
    public TrailRenderer trailRendererEffect; // ���� ȿ��

    public Transform bulletPos;
    public GameObject bullet;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; // ź�� ���� ��ġ
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public GameObject characterController;
    public Camera playerCamera; // �÷��̾��� ī�޶�

    private CasingMemoryPool casingMemoryPool; // ź�� ���� �� Ȱ��, ��Ȱ�� ����

    private void Awake()
    {
        casingMemoryPool = GetComponent<CasingMemoryPool>();
    }

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main; // ���� ī�޶� �ڵ����� ã��
        }
    }

    public void Use()
    {
        // ���� ���� (Hammer)
        if (type==Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        // ���Ÿ� ���� (HandGun, SubMachineGun)
        else if (type==Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() // IEnmerator: ������ �Լ� Ŭ���� yield : ����� �����ϴ� Ű����
    {
        // 1
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        meleeArea.enabled = true;
        trailRendererEffect.enabled = true;

        // 2
        yield return new WaitForSeconds(0.3f); // 0.3�� ���
        meleeArea.enabled = false;

        // 3
        yield return new WaitForSeconds(0.3f); // 1������ ���
        trailRendererEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        if (characterController != null)
        {
            characterController.GetComponent<CharacterControllerCS>().ShotTurn(); // �ν��Ͻ��� ����Ͽ� ShotTurn ȣ��
        }

        // 1. �Ѿ� �߻�
        if(curAmmo > 0)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(1000); // Ray�� �ƹ��͵� ���� �ʾ��� ���� �⺻ ����
            }

            Vector3 targetDirection = targetPoint - bulletPos.position;
            GameObject instantBullet = Instantiate(bullet, bulletPos.position, Quaternion.LookRotation(targetDirection)); // TPS ����
                                                                                                                          // GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); // ���ͺ� ����
            Rigidbody bulletRigidBody = instantBullet.GetComponent<Rigidbody>();
            //bulletRigidBody.velocity = bulletPos.forward * 500; // ���ͺ� ����
            bulletRigidBody.velocity = targetDirection.normalized * 400; // TPS ����
            yield return null;

            // 2. ź�� ����
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right+new Vector3(5,3,0));
            /*
            GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
            Rigidbody bulletCaseRigidBody = instantCase.GetComponent<Rigidbody>();
            Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
            bulletCaseRigidBody.AddForce(caseVec, ForceMode.Impulse);
            bulletCaseRigidBody.AddTorque(Vector3.up * 10, ForceMode.Impulse);
            */
        
        }


    }
}

