using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCS : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator animator;
    CharacterControllerCS enterPlayer;

    public void Enter(CharacterControllerCS player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        Debug.Log("DO HELLO 0 ");

        animator.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
        Debug.Log("DO HELLO 1");
    }
}
