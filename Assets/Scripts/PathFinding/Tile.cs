using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    Color selectColor;
    public Tile Parent;
    public IdeaTile idea;
    public EventTrigger trigger;

    public CharacterControllerCS Owner { get; private set; }
    public bool hasOwner;

    public MeshRenderer Renderer;

    public int F { get { return G + H; } }
    public int G { get; private set; } // 시작지점부터 현재지점까지의 거리
    public int H { get; private set; } // 현재지점부터 도착지점까지의 거리

    public void Execute(Tile parent, Tile destination)
    {
        Parent = parent;
        G = CalcGValue(parent, this);
        int diffX = Mathf.Abs(destination.idea.coord.x - idea.coord.x);
        int diffY = Mathf.Abs(destination.idea.coord.y - idea.coord.y);
        H = (diffX + diffY) * 10;
    }

    public static int CalcGValue(Tile parent, Tile current)
    {
        int value = 10; // 상하좌우 이동 10
        return parent.G + value;
    }

    public void ClearPath()
    {
        Parent = null;
        G = H = 0;
    }

    public void SetSelected()
    {
        Renderer.material.color = selectColor;
    }

    public void ReleaseSelected()
    {
        Renderer.material.color = new Color(1, 1, 1, 0);
    }

    public void SetIdea(IdeaTile ideaTile)
    {
        idea = ideaTile;
    }

    public void SetOwner(CharacterControllerCS c)
    {
        Owner = c; // 타일 위에 에이전트가 있는가?
        hasOwner = true;
        idea.State = TileState.Full;
        // c.currentStand = this;
    }

    public void ReleaseOwner(CharacterControllerCS c)
    {
        Owner = null;
        hasOwner = false;
        idea.State = TileState.Empty;
    }

    public void SetSearchInfo(IdeaTile ideaTile)
    {
        idea.SetSearchInfo(ideaTile);
    }

    public void ClearSearch()
    {
        idea.ClearSearchInfo();
    }

    public void Click(PointerEventData data)
    {
        Map.Instance.CheckSelectTile(this);
    }

    private void Start()
    {
        if (trigger == null)
        {
            trigger = GetComponent<EventTrigger>();
        }
        EventTrigger.Entry entryClicker = new EventTrigger.Entry();
        entryClicker.eventID = EventTriggerType.PointerClick;
        entryClicker.callback.AddListener(data => Click((PointerEventData)data));
        trigger.triggers.Add(entryClicker);

        selectColor = Renderer.material.color;
        ReleaseSelected();
    }
}