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
                // Lobby Scene�� ���� ���� ����
                break;
            case GameState.Exploration:
                // Ž�� ���·� ��ȯ�� ���� ����
                break;
            case GameState.Combat:
                // ���� ���·� ��ȯ�� ���� ����
                break;
            case GameState.Dialogue:
                // ��ȭ ���·� ��ȯ�� ���� ����
                break;
        }
    }

    public void CompleteLevel()
    {
        // ������ �Ϸ�� �� ȣ��Ǹ�, SceneManager�� LoadNextScene�� ����Ͽ� ���� ������ ��ȯ
        SceneManager.instance.LoadNextScene();
    }

    // �߰� ���� ���� ����
}