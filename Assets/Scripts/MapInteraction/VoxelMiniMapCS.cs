using UnityEngine;
// 맵의 랜덤한 지형을 생성하고 다양한 크기와 형태의 프리팹을 겹치지 않게 배치
public class VoxelMiniMap : MonoBehaviour
{
    public GameObject cubePrefab; // 큐브 프리팹
    public int width = 0; // 타일맵의 너비
    public int height = 0; // 타일맵의 높이

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
                // 큐브의 위치 계산
                Vector3 position = new Vector3(x * 2, 1, z * 2);
                // 큐브 생성 및 배치
                Instantiate(cubePrefab, position, Quaternion.identity);
            }
        }
    }

    /* public int width = 100;
     public int depth = 100;
     public float scale = 20f;
     public VoxelPrefabData[] voxelPrefabs; // 프리팹과 해당 비중을 저장하는 배열

     private void Start()
     {
         GenerateMiniMap();
     }

     // 미니맵을 생성. 겹치지 않게 프리팹을 배치
     void GenerateMiniMap()
     {
         bool[,] placed = new bool[width, depth]; // 프리팹 배치 여부를 추적하는 배열

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
     // 미니맵에 배치할 프리팹을 무작위로 선택. 각 프리팹의 비중을 고려하여 선택
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

         return voxelPrefabs[0]; // 기본값 반환
     }
     // 선택된 프리팹을 주어진 위치에 인스턴스화하고, 해당 프리팹이 차지하는 영역을 placed 배열에 기록하여 겹치지 않도록 합니다
     void CreateVoxel(int x, int z, VoxelPrefabData prefabData, ref bool[,] placed)
     {
         float y = Mathf.PerlinNoise(x / scale, z / scale) * prefabData.heightMultiplier;
         Vector3 position = new Vector3(x, y, z);
         GameObject voxel = Instantiate(prefabData.prefab, position, Quaternion.identity);

         int sizeX = Mathf.RoundToInt(voxel.transform.localScale.x);
         int sizeZ = Mathf.RoundToInt(voxel.transform.localScale.z);

         // 주변 프리팹 배치 여부를 업데이트
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
    public float weight; // 프리팹 선택 비중
    public float heightMultiplier; // 높이 배율
}
