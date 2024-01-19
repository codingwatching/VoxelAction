using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public GameObject target;
    public float speed = 20;
    public float pathUpdateSeconds = 0.5f; // Time interval for updating the path

    public bool isUpdate = false; 

    Vector3[] path;
    int targetIndex;

    void OnEnable()
    {
        //target 을 씬에서 Character 이름으로 찾기
        // target = GameObject.Find("Character(Clone)");
        target = GameObject.Find("Character(Clone)");

        if (isUpdate == true && target != null)
            StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        PathRequestManager.RequestPath(transform.position, target.transform.position, OnPathFound);

        float sqrMoveThreshold = pathUpdateSeconds * pathUpdateSeconds;
        Vector3 targetPosOld = target.transform.position;

        while (true)
        {
            yield return new WaitForSeconds(pathUpdateSeconds);
            if ((target.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(transform.position, target.transform.position, OnPathFound);
                targetPosOld = target.transform.position;
            }
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful && gameObject.active)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        if (path.Length == 0)
        {
            yield break;
        }

        targetIndex = 0;
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void StopMovement()
    {
        StopCoroutine("FollowPath"); // 현재 진행 중인 FollowPath 코루틴을 중단합니다.
        path = new Vector3[0]; // 경로를 비워서 목표지점이 없도록 설정합니다.
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    // 새로운 목표 위치를 설정하는 메서드
    public void SetDestination(Vector3 destination)
    {
        PathRequestManager.RequestPath(transform.position, destination, OnPathFound);
    }
}
