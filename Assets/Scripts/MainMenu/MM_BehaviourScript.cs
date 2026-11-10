using UnityEngine;
using UnityEngine.SceneManagement;

public class MM_BehaviourScript : MonoBehaviour
{
    public void LoadScene(string sceneName) {
        SceneManager.Instance.ChangeScene(sceneName);
        //UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
