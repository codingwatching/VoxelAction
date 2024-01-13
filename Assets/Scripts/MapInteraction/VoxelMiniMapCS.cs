using UnityEngine;
// ���� ������ ������ �����ϰ� �پ��� ũ��� ������ �������� ��ġ�� �ʰ� ��ġ
public class VoxelMiniMap : MonoBehaviour
{
    public GameObject cubePrefab; // ť�� ������
    public int width = 0; // Ÿ�ϸ��� �ʺ�
    public int height = 0; // Ÿ�ϸ��� ����

    void Start()
    {
        GenerateTilemap();
    }

    void GenerateTilemap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // ť���� ��ġ ���
                Vector3 position = new Vector3(x * 2, 1, z * 2);
                // ť�� ���� �� ��ġ
                Instantiate(cubePrefab, position, Quaternion.identity);
            }
        }
    }

    /* public int width = 100;
     public int depth = 100;
     public float scale = 20f;
     public VoxelPrefabData[] voxelPrefabs; // �����հ� �ش� ������ �����ϴ� �迭

     private void Start()
     {
         GenerateMiniMap();
     }

     // �̴ϸ��� ����. ��ġ�� �ʰ� �������� ��ġ
     void GenerateMiniMap()
     {
         bool[,] placed = new bool[width, depth]; // ������ ��ġ ���θ� �����ϴ� �迭

         for (int x = 0; x < width; x++)
         {
             for (int z = 0; z < depth; z++)
             {
                 if (!placed[x, z])
                 {
                     VoxelPrefabData selectedPrefab = SelectPrefab();
                     CreateVoxel(x, z, selectedPrefab, ref placed);
                 }
             }
         }
     }
     // �̴ϸʿ� ��ġ�� �������� �������� ����. �� �������� ������ ����Ͽ� ����
     VoxelPrefabData SelectPrefab()
     {
         float totalWeight = 0;
         foreach (var prefab in voxelPrefabs)
         {
             totalWeight += prefab.weight;
         }

         float randomValue = Random.Range(0, totalWeight);
         float currentWeight = 0;

         foreach (var prefab in voxelPrefabs)
         {
             currentWeight += prefab.weight;
             if (randomValue <= currentWeight)
             {
                 return prefab;
             }
         }

         return voxelPrefabs[0]; // �⺻�� ��ȯ
     }
     // ���õ� �������� �־��� ��ġ�� �ν��Ͻ�ȭ�ϰ�, �ش� �������� �����ϴ� ������ placed �迭�� ����Ͽ� ��ġ�� �ʵ��� �մϴ�
     void CreateVoxel(int x, int z, VoxelPrefabData prefabData, ref bool[,] placed)
     {
         float y = Mathf.PerlinNoise(x / scale, z / scale) * prefabData.heightMultiplier;
         Vector3 position = new Vector3(x, y, z);
         GameObject voxel = Instantiate(prefabData.prefab, position, Quaternion.identity);

         int sizeX = Mathf.RoundToInt(voxel.transform.localScale.x);
         int sizeZ = Mathf.RoundToInt(voxel.transform.localScale.z);

         // �ֺ� ������ ��ġ ���θ� ������Ʈ
         for (int i = x; i < x + sizeX && i < width; i++)
         {
             for (int j = z; j < z + sizeZ && j < depth; j++)
             {
                 placed[i, j] = true;
             }
         }
     }*/
}

[System.Serializable]
public class VoxelPrefabData
{
    public GameObject prefab;
    public float weight; // ������ ���� ����
    public float heightMultiplier; // ���� ����
}
