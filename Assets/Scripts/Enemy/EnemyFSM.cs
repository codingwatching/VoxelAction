using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { None = -1, Idle = 0, Wander, }

public class EnemyFSM : MonoBehaviour
{
    private EnemyState enemyState = EnemyState.None; // 현재 적 행동
    private Status status; // 이동속도 등의 정보
    private Unit unit; // 패스파인딩을 위한 정보

    private void Awake()
    {
        status = GetComponent<Status>();
        unit = GetComponent<Unit>();
    }

    private void OnEnable()
    {
        // 적이 활성화될 때 상태를 "대기"로 설정
        enemyState = EnemyState.None;        
    }

    public void ChangeState(EnemyState newState)
    {
        // 현재 재생 중인 상태와 바꾸려고 하는 상태가 같으면 바꿀 필요가 없으므로 return
        if (enemyState == newState) return;

        // 이전에 재생중이던 상태 종료
        StopCoroutine(enemyState.ToString());
        // 현재 적의 상태를 newState 로 설정
        enemyState = newState;
        // 새로운 상태 재생
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // n초 후 "배회" 상태로 변경하는 코루틴 실행
        StartCoroutine("AutoChangeFromIdleToWander");
        while(true)
        {
            // "대기" 상태일 때 하는 행동
            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4초 대기
        int changeTime = Random.Range(1, 5);
        yield return new WaitForSeconds(changeTime);

        // 상태를 "배회"로 변경
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        // 이동 속도 설정

        yield return null;
    }
}
