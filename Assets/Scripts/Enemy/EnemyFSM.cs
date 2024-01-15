using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { None = -1, Idle = 0, Wander, }

public class EnemyFSM : MonoBehaviour
{
    private EnemyState enemyState = EnemyState.None; // ���� �� �ൿ
    private Status status; // �̵��ӵ� ���� ����
    private Unit unit; // �н����ε��� ���� ����

    private void Awake()
    {
        status = GetComponent<Status>();
        unit = GetComponent<Unit>();
    }

    private void OnEnable()
    {
        // ���� Ȱ��ȭ�� �� ���¸� "���"�� ����
        enemyState = EnemyState.None;        
    }

    public void ChangeState(EnemyState newState)
    {
        // ���� ��� ���� ���¿� �ٲٷ��� �ϴ� ���°� ������ �ٲ� �ʿ䰡 �����Ƿ� return
        if (enemyState == newState) return;

        // ������ ������̴� ���� ����
        StopCoroutine(enemyState.ToString());
        // ���� ���� ���¸� newState �� ����
        enemyState = newState;
        // ���ο� ���� ���
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        // n�� �� "��ȸ" ���·� �����ϴ� �ڷ�ƾ ����
        StartCoroutine("AutoChangeFromIdleToWander");
        while(true)
        {
            // "���" ������ �� �ϴ� �ൿ
            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // 1~4�� ���
        int changeTime = Random.Range(1, 5);
        yield return new WaitForSeconds(changeTime);

        // ���¸� "��ȸ"�� ����
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        // �̵� �ӵ� ����

        yield return null;
    }
}
