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
            string nextScene = currentScene == "Play01" ? "Play02" : "Play01";

            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);  // �浹�� ���ϱ� ���� ��ü ���ӽ����̽� ���
        }
    }

/*    private void SavePlayerState(Collider player)
    {

    }*/
}