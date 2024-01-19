using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCS : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage = 10;
    public float rate; // ���ݼӵ�
    public int maxAmmo; // ���� ������ �ִ� ź�� ����
    public int curAmmo; // ���� ź�� ��

    public BoxCollider meleeArea; // ���� ����
    public TrailRenderer trailRendererEffect; // ���� ȿ��

    public GameObject bullet;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; // ź�� ���� ��ġ (�޸�Ǯ)
    [SerializeField] 
    private Transform bulletSpawnPoint; // �Ѿ� ���� ��ġ (�޸�Ǯ)
    // public Transform bulletCasePos;
    public GameObject bulletCase;

    public GameObject characterController;
    public Camera playerCamera; // �÷��̾��� ī�޶�

    private CasingMemoryPool casingMemoryPool; // ź�� ���� �� Ȱ��, ��Ȱ�� ����
    private ImpactMemoryPool impactMemoryPool; // ���� ȿ�� ���� �� Ȱ��, ��Ȳ�� ����
    private Camera mainCamera; // ���� �߻�

    private void Awake()
    {
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;
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

            // ������ �߻��� ���ϴ� ��ġ ���� (+Impact Effect)
            TwoStepRaycast();
            yield return null;

            // 2. ź�� ���� (�޸�Ǯ)
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right+new Vector3(5,3,0));
    }

    // �ѱ��� ��ġ���� ���� �� Ÿ���� ����� �̷����� ���� �� �ֽ��ϴ�.
    public void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // ȭ���� �߾� ��ǥ (Aim �������� Raycast ����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        // ���� �� targetPoint �� ������ �ε��� ��ġ
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            targetPoint = hit.point;
        }
        // ���� �ε����� ������Ʈ�� ������
        else
        {
            Debug.Log("None");
        }

        // ù��° Raycast �������� ����� targetPoint �� ��ǥ�������� �����ϰ�,
        // �ѱ��� ������������ �Ͽ� Raycast ����
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if(Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, Mathf.Infinity)) 
        {
            impactMemoryPool.SpawnImpact(hit);

            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(damage);
            }
        }
    }
}

