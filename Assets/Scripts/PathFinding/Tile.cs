using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Coord coord;

    public void SetCoord(int horizontal, int vertical)
    {
        coord = new Coord(horizontal, vertical);
    }
}

[System.Serializable]
public class Coord
{
    public int horizontal; // ����
    public int vertiacl; // ����

    public Coord(int horizontal, int vertiacl)
    {
        this.horizontal = horizontal;
        this.vertiacl = vertiacl;
    }
}
