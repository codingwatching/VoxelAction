using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // SavePlayerState(other);

            // 현재 활성화된 씬의 이름을 얻음
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // 씬 이름에 따라 다음 씬을 결정
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
            // 다음 씬 로드
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);  // 충돌을 피하기 위해 전체 네임스페이스 사용
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