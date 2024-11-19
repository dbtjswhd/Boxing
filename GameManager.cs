using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 현재 씬을 다시 로드하는 함수
    public void ReloadScene()
    {
        // 현재 활성화된 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // "MainScene"으로 이동하는 함수
    public void MoveScene()
    {
        // "MainScene"이 빌드 설정에 있는지 확인하고 로드
        if (Application.CanStreamedLevelBeLoaded("MainScene"))
        {
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            Debug.LogError("\"MainScene\" 씬이 빌드 설정에 없습니다.");
        }
    }
}
