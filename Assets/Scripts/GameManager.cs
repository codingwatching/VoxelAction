using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CharacterData currentCharacterData;

    public enum GameState
    {
        Lobby,
        Exploration,
        Combat,
        Dialogue,
        Pause
    }

    private GameState currentState;

    // GameState 변경 시 호출되는 이벤트
    public delegate void GameStateChangeHandler(GameState newState); // 델리게이트는 C#에서 메서드에 대한 참조를 보관하는 타입
    public static event GameStateChangeHandler OnGameStateChange; // GameState가 변경될 때마다 알림을 받을 수 있다


    public GameObject gameCam; // main cam
    public CharacterControllerCS player;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public TMP_Text maxScoreText;


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
        OnGameStateChange?.Invoke(newState); // 게임 상태 변경 알림. ?. 연산자는 OnGameStateChange 이벤트가 null이 아닐 경우에만 이벤트를 호출

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
            case GameState.Pause:
                // 게임 일시정지 상태로 전환할 때의 로직
                break;
        }
    }

/*    public void CompleteLevel()
    {
        // 레벨이 완료될 때 호출되며, SceneManager의 LoadNextScene을 사용하여 다음 씬으로 전환
        SceneManager.instance.LoadNextScene();
    }*/

    // 추가 게임 관리 로직

    // 게임 상태를 가져오는 함수
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
            // 다른 필요한 데이터를 여기에 복사합니다.
        }
    }*/
}