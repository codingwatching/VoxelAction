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
    public float playTime = 181f; // 3분 (180초)으로 초기 설정
    public bool isBattle = true;
    public int enemyCntA;

    private void Awake()
    {
        GameObject myCharacter = Instantiate(characterPrefab, characterSpawnPos.position + new Vector3(0,0,-10), characterSpawnPos.rotation); // ���ͺ� ����

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

    void Update()
    {
        // playTime += Time.deltaTime;

        if (playTime > 0)
        {
            playTime -= Time.deltaTime; // 시간 감소
            if (playTime < 0)
            {
                playTime = 0; // 시간이 0보다 작아지지 않도록 처리
                // 시간이 끝났을 때 수행할 코드
            }
        }
    }

    void LateUpdate()
    {
        PlayTimeCheck();
    }

    void PlayTimeCheck()
    {
        /*
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        UIManager.instance.playTimeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        */

        int min = (int)(playTime / 60);
        int second = (int)(playTime % 60);
        UIManager.instance.playTimeText.text = string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        OnGameStateChange?.Invoke(newState);

        switch (newState)
        {
            case GameState.Lobby:
                break;
            case GameState.Exploration:
                break;
            case GameState.Combat:
                break;
            case GameState.Dialogue:
                break;
            case GameState.Pause:
                break;
        }
    }



    public GameState GetCurrentState()
    {
        return currentState;
    }
    /*    public void CompleteLevel()
    {
        SceneManager.instance.LoadNextScene();
    }
       public void UpdateCharacterData(CharacterControllerCS character)
        {
            currentCharacterData = new CharacterData(character);
        }

        public void LoadCharacterDataIntoScene(CharacterControllerCS character)
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