using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCS : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage = 10;
    public float rate; // 공격속도
    public int maxAmmo; // 소지 가능한 최대 탄약 개수
    public int curAmmo; // 실제 탄약 수

    public BoxCollider meleeArea; // 공격 범위
    public TrailRenderer trailRendererEffect; // 무기 효과

    public GameObject bullet;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; // 탄피 생성 위치 (메모리풀)
    [SerializeField] 
    private Transform bulletSpawnPoint; // 총알 생성 위치 (메모리풀)
    // public Transform bulletCasePos;
    public GameObject bulletCase;

    public GameObject characterController;
    public Camera playerCamera; // 플레이어의 카메라

    private CasingMemoryPool casingMemoryPool; // 탄피 생성 후 활성, 비활성 관리
    private ImpactMemoryPool impactMemoryPool; // 공격 효과 생성 후 활성, 비황성 관리
    private Camera mainCamera; // 광선 발사

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
            playerCamera = Camera.main; // 메인 카메라를 자동으로 찾음
        }
    }

    public void Use()
    {
        // 근접 공격 (Hammer)
        if (type==Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        // 원거리 공격 (HandGun, SubMachineGun)
        else if (type==Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() // IEnmerator: 열거형 함수 클래스 yield : 결과를 전달하는 키워드
    {
        // 1
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        meleeArea.enabled = true;
        trailRendererEffect.enabled = true;

        // 2
        yield return new WaitForSeconds(0.3f); // 0.3초 대기
        meleeArea.enabled = false;

        // 3
        yield return new WaitForSeconds(0.3f); // 1프레임 대기
        trailRendererEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        if (characterController != null)
        {
            characterController.GetComponent<CharacterControllerCS>().ShotTurn(); // 인스턴스를 사용하여 ShotTurn 호출
        }

        // 1. 총알 발사

            // 광선을 발사해 원하는 위치 공격 (+Impact Effect)
            TwoStepRaycast();
            yield return null;

            // 2. 탄피 배출 (메모리풀)
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right+new Vector3(5,3,0));
    }

    // 총구의 위치에서 공격 시 타격이 제대로 이뤄지지 않을 수 있습니다.
    public void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // 화면의 중앙 좌표 (Aim 기준으로 Raycast 연산)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        // 공격 시 targetPoint 는 광선에 부딪힌 위치
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            targetPoint = hit.point;
        }
        // 만약 부딪히는 오브젝트가 없으면
        else
        {
            Debug.Log("None");
        }

        // 첫번째 Raycast 연산으로 얻어진 targetPoint 를 목표지점으로 설정하고,
        // 총구를 시작지점으로 하여 Raycast 연산
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

