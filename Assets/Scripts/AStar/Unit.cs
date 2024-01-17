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

    void Start()
    {
        //target 을 씬에서 Character 이름으로 찾기
        target = GameObject.Find("Character(Clone)");
        
        if(isUpdate == true && target != null)
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
        if (pathSuccessful)
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
}
