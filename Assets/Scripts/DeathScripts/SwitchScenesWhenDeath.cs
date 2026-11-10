using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenesWhenDeath : MonoBehaviour
{
    [SerializeField] private float m_delayedExitInSeconds = 2f;
    [SerializeField] private string m_mainMenu;

    private bool HasAlreaySwitched = false;
    public void SwitchScene() {
        if (HasAlreaySwitched) {
            return;
        }
        
        StartCoroutine(ExitGame());
        HasAlreaySwitched = true;
    }


    public IEnumerator ExitGame() {
        yield return new WaitForSeconds(m_delayedExitInSeconds);
        SceneManager.Instance.ChangeScene(m_mainMenu);
    }
}
