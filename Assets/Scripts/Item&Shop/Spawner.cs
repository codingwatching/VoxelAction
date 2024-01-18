using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    [Range(0, 100)]
    private float chance = 0;

    public float Weight { set; get; }

    public GameObject Prefab=>prefab;
    public float Chance => chance;
}

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Item[] items;

    // 총  생성할 Item 오브젝트의 전체 개수입니다. 0~1000 사이의 값만 설정할 수 있습니다.
    [SerializeField][Range(0, 1000)]
    private int maxItemCount = 100; 

    private float accumulateWeights;

    // 맵 사이즈
    private Vector3 mapSize = new Vector3(100, 20, 100); // 높이도 고려한 3D 맵 크기

    private void Awake()
    {
        CalculateWeights();
    }

    // 임의의 위치에 Item 을 생성합니다.
    private IEnumerator Start()
    {
        int count = 0;

        while(count < maxItemCount)
        {
            // 3D 환경에 맞게 Vector3를 사용하여 랜덤 위치 생성
            Vector3 position = new Vector3(
                Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f),
                Random.Range(0, mapSize.y), // 높이는 0부터 mapSize.y까지 랜덤
                Random.Range(-mapSize.z * 0.49f, mapSize.z * 0.49f)
            );

            SpawnItem(position);

            count++;

            // Item 들은 0.01초 간격을 두고 생성됩니다.
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void CalculateWeights()
    {
        accumulateWeights = 0;
        foreach(var item in items)
        {
            accumulateWeights += item.Chance;
            item.Weight = accumulateWeights;
        }
    }

    private void SpawnItem(Vector2 position)
    {
        var clone = items[GetRandomIndex()];

        Instantiate(clone.Prefab, position, Quaternion.identity);
    }

    private int GetRandomIndex()
    {
        float random = Random.value * accumulateWeights;

        for(int i=0; i<items.Length; i++)
        {
            if (items[i].Weight >= random)
            {
                return i;
            }
        }
        return 0;
    }
}
