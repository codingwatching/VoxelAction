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
        public int G { get; private set; } // ������������ �������������� �Ÿ�
        public int H { get; private set; } // ������������ �������������� �Ÿ�
    */

    /*   public void Execute(Tile parent, Tile destination) {
            Parent = parent;

        }

        public static int CalcGValue(Tile paretn, Tile current)
        {
            int value = 10; // �����¿� �̵� 10
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
    public int horizontal; // ����
    public int vertiacl; // ����

    public Coord(int horizontal, int vertiacl)
    {
        this.horizontal = horizontal;
        this.vertiacl = vertiacl;
    }
}
*/