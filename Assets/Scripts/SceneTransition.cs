using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SavePlayerState(other);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Play02");  // 충돌을 피하기 위해 전체 네임스페이스 사용
        }
    }

    private void SavePlayerState(Collider player)
    {
        CharacterControllerCS characterController = player.GetComponent<CharacterControllerCS>();
        if (characterController != null)
        {
            // 현재 캐릭터 상태를 저장
            GameManager.instance.UpdateCharacterData(characterController);
        }
    }
}