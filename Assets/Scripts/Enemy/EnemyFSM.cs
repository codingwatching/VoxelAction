using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { None = -1, Idle = 0, Wander = 1, }

public class EnemyFSM : MonoBehaviour
{
    private EnemyState enemyState = EnemyState.None; // ���� �� �ൿ
    private Status status; // �̵��ӵ� ���� ����
    private Unit unit; // �н����ε��� ���� ����
    Animator animator;
    Vector3 wanderPosition = Vector3.zero;

    private void Awake()
    {
        status = GetComponent<Status>();
        unit = GetComponent<Unit>();
        animator = GetComponentInChildren<Animator>();

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
        animator.SetBool("isWalk", false);

        // StartCoroutine("AutoChangeFromIdleToWander");
        while (true)
        {
            yield return StartCoroutine("AutoChangeFromIdleToWander");
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

            yield return null;
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

    private void OnDrawGizmos(){
        // 배회 상태일 때 이동 경로 표시
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, wanderPosition - transform.position);
    }
}
