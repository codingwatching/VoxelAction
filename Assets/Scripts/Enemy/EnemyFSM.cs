using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { None = -1, Idle = 0, Wander = 1, Pursuit, Attack, Dead}

public class EnemyFSM : MonoBehaviour
{
    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 15; // 인식 범위 (이 범위 안에 들어오면 "Pursuit"상태로 변경)
    [SerializeField]
    private float pursuitLimitRange = 25; // 추적 범위 (이 범위 바깥으로 나가면 "Wander" 상태로 변경)

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab; // 발사체 프리팹
    [SerializeField]
    private Transform projectileSpawnPoint; // 발사체 생성 위치
    [SerializeField]
    private float attackRange = 15; // 공격 범위  (이 범위 안에 들어오면 "Attack" 상태로 변경)
    [SerializeField]
    private float attackRate = 1; // 공격 속도
    private float lastAttackTime = 0; // 공격 주기 계산용 변수

    private EnemyState enemyState = EnemyState.None; // 현재 적 행동
    
    private Status status; // 이동속도 등 정보
    private Unit unit; 
    Animator animator;

    Vector3 wanderPosition = Vector3.zero;
    [SerializeField]
    private GameObject target; // 적의 공격 대상 (플레이어)
    private EnemyMemoryPool enemyMemoryPool; // 적 메모리 풀 (적 오브젝트 비활성화에 사용)


    // private void Awake()
    public void Setup(GameObject target, EnemyMemoryPool enemyMemoryPool)
    {
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;

        status = GetComponent<Status>();
        unit = GetComponent<Unit>();
        animator = GetComponentInChildren<Animator>();

        // 추가: animator가 제대로 설정되었는지 확인
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the child of this GameObject.");
        }
    }

    // 활성화 될 때마다 호출되는 함수입니다
    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);      
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;

        StopCoroutine(enemyState.ToString());
        enemyState = newState;
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        Debug.Log("FSM Idle");
        if (animator != null)
        {
            animator.SetBool("isWalk", false);
        }
        // n초 후에 배회 상태로 변경하는 코루틴 실행
        StartCoroutine("AutoChangeFromIdleToWander");
        while (true)
        {
            if (target == null) // 타겟이 null이면 반복 중단
            {
                Debug.LogWarning("Target is null in Idle state.");
                yield break;
            }
            // yield return StartCoroutine("AutoChangeFromIdleToWander");

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectedState();
            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        Debug.Log("FSM AutoChangeFromIdleToWander");
        // 1~4
        int changeTime = Random.Range(1, 5);
        Debug.Log("FSM WaitForSeconds " + changeTime);

        yield return new WaitForSeconds(changeTime);
        Debug.Log("FSM BEFORE Wander"); 

        ChangeState(EnemyState.Wander);
        Debug.Log("FSM GOTO Wander");
    }

    private IEnumerator Wander()
    {
        Debug.Log("FSM Wander");

        float currentTime = 0;
        float maxTime = 2;
        float rotationSpeed = 2.0f; // 회전 속도 조절을 위한 변수
        float arrivalDistance = 1f; // 도착으로 간주할 거리

        unit.speed = status.WalkSpeed;
        wanderPosition = CalculateWanderPosition();
        unit.SetDestination(wanderPosition);

        animator.SetBool("isWalk", true); // 이동 시작 시 isWalk를 true로 설정

        while (true)
        {
            currentTime += Time.deltaTime;

            // 목표 위치로 회전
            Quaternion targetRotation = Quaternion.LookRotation(wanderPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // 목적지에 근접하면
            if ((wanderPosition - transform.position).sqrMagnitude < arrivalDistance * arrivalDistance)
            {
                animator.SetBool("isWalk", false); // 이동 중지 시 isWalk를 false로 설정
                ChangeState(EnemyState.Idle);
                break;
            }

            // 최대 시간 초과 시
            if (currentTime >= maxTime)
            {
                animator.SetBool("isWalk", false);
                ChangeState(EnemyState.Idle);
                break;
            }

            // 타겟과의 거리에 따라 행동 선택 (배회, 추격, 원거리 공격)
            CalculateDistanceToTargetAndSelectedState();
            yield return null;
        }
    }

    private IEnumerator Pursuit()
    {
        Debug.Log("FSM Pursuit");

        while (true)
        {
            unit.SetDestination(target.transform.position);
            LookRotationToTarget();
            CalculateDistanceToTargetAndSelectedState();
            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        // 공격할 때는 이동을 멈추고 현재 위치에 고정
        unit.StopMovement(); 

        while (true)
        {
            // 타겟 방향 주시
            LookRotationToTarget();

            // 타겟과의 거리에 따라 행동을 선택 (배회, 추격, 공격)
            CalculateDistanceToTargetAndSelectedState();
            
/*            if(Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;

                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.transform.position);
            }*/
            yield return null;
        }
    }
    private void LookRotationToTarget()
    {
        Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        transform.rotation = Quaternion.LookRotation(to - from);
    }

    private void CalculateDistanceToTargetAndSelectedState()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is null in CalculateDistanceToTargetAndSelectedState.");
            return;
        }

        // target(플레이어) 과의 거리에 따라 상태 변경
        float distance = Vector3.Distance(target.transform.position, transform.position);

        if(distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
        }

        else if (distance <= targetRecognitionRange)
        {
            ChangeState(EnemyState.Pursuit);
        }

        else if (distance >= targetRecognitionRange)
        {
            ChangeState(EnemyState.Wander);
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10; // 현재 위치를 원점으로 하는 원의 반지름
        int wanderJitter = 0; // 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
        int wanderJitterMin = 0; // 최소 각도
        int wanderJitterMax = 360; // 최대 각도

        // 현재 적 캐릭터가 있는 월드의 중심 위치와 크기 (구역을 벗어난 행동을 하지 않도록)
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f; // (1, 1, 1) * 100

        // 자신의 위치를 중심으로 반지름(wanderRadius) 거리, 선택된 각도(wanderJitter) 에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        // 생성된 목표 위치가 자신의 이동 구역을 벗어나지 않도록 조절
        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x-rangeScale.x*0.5f, rangePosition.x+rangeScale.x*0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z-rangeScale.z*0.5f, rangePosition.z+rangeScale.z*0.5f);
    
        return targetPosition;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private void OnDrawGizmos()
    {
        // 배회 상태일 때 이동 경로 표시
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, wanderPosition - transform.position);

        // 목표 인식 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

        // 추적 범위
        Gizmos.color=Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

        // 공격 범위
        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);
        if(isDie == true)
        {
            animator.SetTrigger("doDie");            
            GameManager.instance.player.transform.GetComponent<Status>().IncreaseEnemyCount();
            StartCoroutine(DeactivateEnemyAfterDelay(1f));
        }
    }

    IEnumerator DeactivateEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyMemoryPool.DeactivateEnemy(gameObject);
    }
}
