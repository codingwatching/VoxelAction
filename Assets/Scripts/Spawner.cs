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

    // ��  ������ Item ������Ʈ�� ��ü �����Դϴ�. 0~1000 ������ ���� ������ �� �ֽ��ϴ�.
    [SerializeField][Range(0, 1000)]
    private int maxItemCount = 100; 

    // ȭ���� �ּ� ��ǥ�� �ִ� ��ǥ ������ �����մϴ�.
    [SerializeField]
    private Vector2 min = new Vector2(-8f, -4f);
    [SerializeField]
    private Vector2 max = new Vector2(8f, 4f);

    private float accumulateWeights;

    private void Awake()
    {
        CalculateWeights();
    }

    // ������ ��ġ�� Item �� �����մϴ�.
    private IEnumerator Start()
    {
        int count = 0;

        while(count < maxItemCount)
        {
            SpawnItem(new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y)));
            count++;

            // Item ���� 0.01�� ������ �ΰ� �����˴ϴ�.
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
