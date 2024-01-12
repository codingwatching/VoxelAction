using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    Color selectColor;
    public Tile Parent;
    // public IdeaTile idea;
    public EventTrigger trigger;

    /*    public CharacterControllerCS Owner { get; private set; }
        public bool hasOwner;
        public MeshRenderer Renderer;
        public int F { get { return global G + H} }
        public int G { get; private set; } // 시작지점부터 현재지점까지의 거리
        public int H { get; private set; } // 현재지점부터 도착지점까지의 거리
    */

    /*   public void Execute(Tile parent, Tile destination) {
            Parent = parent;

        }

        public static int CalcGValue(Tile paretn, Tile current)
        {
            int value = 10; // 상하좌우 이동 10
            return parent.G + value;
        }*/




    /*    public Coord coord;

        public void SetCoord(int horizontal, int vertical)
        {
            coord = new Coord(horizontal, vertical);
        }*/

}

/*[System.Serializable]
public class Coord
{
    public int horizontal; // 가로
    public int vertiacl; // 세로

    public Coord(int horizontal, int vertiacl)
    {
        this.horizontal = horizontal;
        this.vertiacl = vertiacl;
    }
}
*/