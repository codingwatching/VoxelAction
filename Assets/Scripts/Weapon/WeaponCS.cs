using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCS : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate; // 공격속도
    public int maxAmmo; // 소지 가능한 최대 탄약 개수
    public int curAmmo; // 실제 탄약 수

    public BoxCollider meleeArea; // 공격 범위
    public TrailRenderer trailRendererEffect; // 무기 효과

    public Transform bulletPos;
    public GameObject bullet;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; // 탄피 생성 위치
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public GameObject characterController;
    public Camera playerCamera; // 플레이어의 카메라

    private CasingMemoryPool casingMemoryPool; // 탄피 생성 후 활성, 비활성 관리

    private void Awake()
    {
        casingMemoryPool = GetComponent<CasingMemoryPool>();
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
                targetPoint = ray.GetPoint(1000); // Ray가 아무것도 맞지 않았을 때의 기본 지점
            }

            Vector3 targetDirection = targetPoint - bulletPos.position;
            GameObject instantBullet = Instantiate(bullet, bulletPos.position, Quaternion.LookRotation(targetDirection)); // TPS 버전
                                                                                                                          // GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); // 쿼터뷰 버전
            Rigidbody bulletRigidBody = instantBullet.GetComponent<Rigidbody>();
            //bulletRigidBody.velocity = bulletPos.forward * 500; // 쿼터뷰 버전
            bulletRigidBody.velocity = targetDirection.normalized * 400; // TPS 버전
            yield return null;

            // 2. 탄피 배출
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

