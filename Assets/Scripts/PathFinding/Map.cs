using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map instance;

    /*  
     Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
      public Tile currentFocus;
      public CharacterControllerCS testCharacter;
      public EnemyCS testEnemy;
      public int NumColumns, NumRows;

      Vector2Int[] checkDir = new Vector2Int[4]
      {
          new Vector2Int(0,1),
          new Vector2Int(0,-1),
          new Vector2Int(1,0),
          new Vector2Int(-1,0)
      };

      public Tile prefabTile;

      public void Initialize()
      {
          tiles.Clear();
          NumColumns = transform.childCount;
          NumRows = transform.GetChild(0).childCount;

          for (int i = 0; i < transform.childCount; i++)
          {
              var child = transform.GetChild(i);
              for (int j = 0; j < child.childCount; j++)
              {
                  var final = child.GetChild(j);
                  var tile = final.GetComponent<Tile>();
                  Vector2Int coord = new Vector2Int(j, i);
                  IdeaTile idea = tile.idea;
                  idea.coord = coord;
                  tiles.Add(coord, tile);
              }
          }

          tiles[Vector2Int.zero].SetOwner(testCharacter);
          testCharacter.transform.position = tiles[Vector2Int.zero].transform.position;

          var enemyTile = tiles[new Vector2Int(8, 8)];
          enemyTile.SetOwner(testEnemy);
          testEnemy.transform.position = enemyTile.transform.position;
      }

      public void ClearTiles()
      {
          foreach (Transform child in transform)
          {
              Destroy(child.gameObject);
          }
          transform.DetachChildren(); // 부모-자식 관계를 모두 해제
      }

      private Tile CreateTile(int row, int column, IdeaTile idea)
      {
          var tile = Instantiate(prefabTile, transform);
          tile.transform.localPosition = new Vector3(-1 - 2 * row, 0, 1 + 2 * column);
          tile.transform.localEulerAngles = Vector3.zero;
          tile.name=string.Format("{0}{1}x{2}", tile.name, row, column);
          tile.SetIdea(idea);
          return tile;
      }

      public void CheckSelectTile(Tile tile)
      {
          if(currentFocus != null)
          {
              if(currentFocus.Owner.CheckMove(tile))
                  currentFocus=null;
          }
          else
          {
              if(tile.hasOwner)
              {
                  if(tile.Owner.Side == CharacterSide.Enemy)
                  {
                      var path = GetMinimumPath(tile.Owner.currentStand, testc.currentStand);
                      tile.Owner.SetMoveableArea(path);
                  }
  *//*                else
                  {
                      currentFocus = tile;
                      var moveArea = GetMoveArea(tile, tile.Owner.Stat.Mov);
                      tile.Owner.SetMoveableArea(moveArea);

                  }*//*
              }
          }
      }
      public List<Tile> GetMoveArea(Tile moveStart, int moveDistance)
      {
          ClearSearch();

          List<Tile> rtv = new List<Tile>();
          rtv.Add(moveStart);

          Queue<Tile> checkNext = new Queue<Tile>();
          Queue<Tile> checkNow = new Queue<Tile>();

          moveStart.idea.distance = 0;
          checkNow.Enqueue(moveStart);

          while (checkNow.Count > 0)
          {
              Tile t = checkNow.Dequeue();
              for(int i=0; i<checkDir.Length; i++)
              {
                  Tile next = GetTile(t.idea.coord + checkDir[i]);
                  if (next == null || next.idea.distance < t.idea.distance + 1 || next.idea.State != TileState.Empty)
                      continue;

              }
          }
      }
      public 
      private void Start()
      {
          Initialize();
      }
          private void Awake()
      {
          Instance = this;
      }
  */

}


public enum TileState 
{
    Empty,
    Full,
    Hurdle
}

/*public enum GameState
{
    MyTurn,
    EnemyTurn
}

[System.Serializable]
public class IdeaTile
{
    public TileState State;
    public Vector2Int coord;
    public IdeaTile prev;
    public IdeaTile Parent;
    public int distance;

    public void ClearSearchInfo()
    {
        prev = null;
        distance = int.MaxValue;
    }

    public void SetSearchInfo(IdeaTile idea)
    {
        prev = idea;
        distance = int.distance + 1;
    }

    public IdeaTile(Vector2Int coord, TileState state)
    {
        this.coord = coord;
        this.State = state;
        ClearSearchInfo();
    }
}

public enum Directions
{
    Up,
    Right,
    Down,
    Left
}

public static class DirectionsExtensions
{
    public static Directions GetDirections(this Tile t1, Tile t2)
    {
        if(t1.idea.coord.y )
    }
}

*/








/*    Tile[,] totalMap;
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
    }*/

