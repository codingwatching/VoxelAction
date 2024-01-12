using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SavePlayerState(other);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Play02");  // �浹�� ���ϱ� ���� ��ü ���ӽ����̽� ���
        }
    }

    private void SavePlayerState(Collider player)
    {
        CharacterControllerCS characterController = player.GetComponent<CharacterControllerCS>();
        if (characterController != null)
        {
            // ���� ĳ���� ���¸� ����
            GameManager.instance.UpdateCharacterData(characterController);
        }
    }
}