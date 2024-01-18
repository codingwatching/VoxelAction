using System.Buffers;
using System.Collections;
using UnityEngine;

// �޸� Ǯ�� �̿��� �� ĳ���� ����
public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject target; // ���� ��ǥ (�÷��̾�)
    [SerializeField]
    private GameObject enemySpawnPointPrefab; // ���� �����ϱ� �� ���� ���� ��ġ�� �˷��ִ� ������
    [SerializeField]
    private GameObject enemyPrefab; // �����Ǵ� �� ������
    [SerializeField]
    private float enemySpawnTime = 1; // �� ���� �ֱ�
    [SerializeField]
    private float enemySpawnLatency = 1; // Ÿ�� ���� �� ���� �����ϱ���� ��� �ð�

    private MemoryPool spawnPointMemoryPool; // �� ���� ��ġ�� �˷��ִ� ������Ʈ ����, Ȱ��/��Ȱ�� ����
    private MemoryPool enemyMemoryPool; // �� ����, Ȱ��/��Ȱ�� ����
    private int numberOfEnemiesSpawnedAtOnce = 1; // ���ÿ� �����Ǵ� ���� ����
    private Vector2Int mapSize = new Vector2Int(100, 100); // �� ũ��

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);
        StartCoroutine("SpawnTile");
    }
    
    public void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        if (target == null)
        {
            Debug.LogError("Player target not found in the scene.");
        }
    }

    // ������ ��ġ�� �� ���� ��ġ�� �˷��ݴϴ�. ó������ �ϳ�, �ð��� �������� ���� ���� ���� �����մϴ�.
    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 3;

        while (true)
        {
            // ���ÿ� numberOfEnemiesSpawnedAtOnce ���ڸ�ŭ ���� �����ǵ��� �ݺ��� ���
            for(int i= 0; i < numberOfEnemiesSpawnedAtOnce; ++i)
            {
                // �� �� ������ ��ġ�� �� ���� ��ġ�� �˷��ִ� ������ ����
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();
                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f), 1,
                                                                           Random.Range(-mapSize.y * 0.49f, mapSize.y * 0.49f));
                StartCoroutine("SpawnEnemy", item);
            }

            currentNumber++;

            if(currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++;
            }
            //enemySpawnTime ��ŭ �ݺ� ����
            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);
        if (target == null)
        {
            target = GameObject.Find("Character(Clone)");
            yield break; // Ÿ���� ������ �� ���� �ߴ�
        }
        // �� ������Ʈ�� �����ϰ�, ���� ��ġ�� ���� �����Ǿ��ִ� Ÿ�ϰ� ���� point�� ��ġ�� ����
        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;

        item.GetComponent<EnemyFSM>().Setup(target, this);

        // Ÿ�� ������Ʈ�� ��Ȱ��ȭ
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
    }
}
