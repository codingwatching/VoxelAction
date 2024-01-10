using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public enum GameState
    {
        Lobby,
        Exploration,
        Combat,
        Dialogue,
        Pause
    }

    private GameState currentState;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.Lobby:
                // Lobby Scene에 있을 때의 로직
                break;
            case GameState.Exploration:
                // 탐험 상태로 전환할 때의 로직
                break;
            case GameState.Combat:
                // 전투 상태로 전환할 때의 로직
                break;
            case GameState.Dialogue:
                // 대화 상태로 전환할 때의 로직
                break;
        }
    }

    public void CompleteLevel()
    {
        // 레벨이 완료될 때 호출되며, SceneManager의 LoadNextScene을 사용하여 다음 씬으로 전환
        SceneManager.instance.LoadNextScene();
    }

    // 추가 게임 관리 로직
}