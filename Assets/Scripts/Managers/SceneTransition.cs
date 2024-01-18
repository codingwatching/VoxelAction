using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // SavePlayerState(other);

            // ���� Ȱ��ȭ�� ���� �̸��� ����
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // �� �̸��� ���� ���� ���� ����
            string nextScene = currentScene;
            Debug.Log("SceneTransition nextScene" + nextScene);

            if (currentScene == "Lobby")
            {
                nextScene = "Play01";
            }
            else if (currentScene == "Play01")
            {
                nextScene = "Play02";
            }
            else if (currentScene == "Play02")
            {
                nextScene = "Play01";
            }
            // ���� �� �ε�
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);  // �浹�� ���ϱ� ���� ��ü ���ӽ����̽� ���
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("SceneTransition OnTriggerExit");
    }
        /*    private void SavePlayerState(Collider player)
            {

            }*/
    }