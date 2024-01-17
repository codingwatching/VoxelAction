using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CharacterData currentCharacterData;
    public UIManager UIManager;

    public GameObject characterPrefab;
    public Transform characterSpawnPos;

    public enum GameState
    {
        Lobby,
        Exploration,
        Combat,
        Dialogue,
        Pause
    }

    private GameState currentState;

    // GameState ���� �� ȣ��Ǵ� �̺�Ʈ
    public delegate void GameStateChangeHandler(GameState newState); // ��������Ʈ�� C#���� �޼��忡 ���� ������ �����ϴ� Ÿ��
    public static event GameStateChangeHandler OnGameStateChange; // GameState�� ����� ������ �˸��� ���� �� �ִ�


    public GameObject gameCam; // main cam
    public CharacterControllerCS player;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;

    private void Awake()
    {
        GameObject myCharacter = Instantiate(characterPrefab, characterSpawnPos.position, characterSpawnPos.rotation); // ���ͺ� ����

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
        OnGameStateChange?.Invoke(newState); // ���� ���� ���� �˸�. ?. �����ڴ� OnGameStateChange �̺�Ʈ�� null�� �ƴ� ��쿡�� �̺�Ʈ�� ȣ��

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
            case GameState.Pause:
                // ���� �Ͻ����� ���·� ��ȯ�� ���� ����
                break;
        }
    }

/*    public void CompleteLevel()
    {
        // ������ �Ϸ�� �� ȣ��Ǹ�, SceneManager�� LoadNextScene�� ����Ͽ� ���� ������ ��ȯ
        SceneManager.instance.LoadNextScene();
    }*/

    // �߰� ���� ���� ����

    // ���� ���¸� �������� �Լ�
    public GameState GetCurrentState()
    {
        return currentState;
    }

/*    public void UpdateCharacterData(CharacterControllerCS character)
    {
        currentCharacterData = new CharacterData(character);
    }*/

/*    public void LoadCharacterDataIntoScene(CharacterControllerCS character)
    {
        if (currentCharacterData != null)
        {
            character.health = currentCharacterData.health;
            character.ammo = currentCharacterData.ammo;
            character.coin = currentCharacterData.coin;
            // �ٸ� �ʿ��� �����͸� ���⿡ �����մϴ�.
        }
    }*/
}