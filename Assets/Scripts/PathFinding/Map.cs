using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    Tile[,] totalMap;
    public Tile prefabTile;

    public void TestTileCreate()
    {
        int horizontal = prefabTile.coord.horizontal; // 가로
        int vertical = prefabTile.coord.vertiacl; // 세로

        totalMap = new Tile[horizontal, vertical];

        for(int i = 0; i < horizontal; i++)
        {
            for(int j = 0; j < vertical; j++)
            {
                var tile = GameObject.Instantiate(prefabTile);
                tile.transform.localPosition = new Vector3(i * 2, 2, j * 2);
                tile.SetCoord(i, j);
                totalMap[i, j] = tile;
            }
        }
    }

    public bool test;
    private void Update()
    {
        if (test == true)
        {
            test = false;
            TestTileCreate();
        }
    }
}
