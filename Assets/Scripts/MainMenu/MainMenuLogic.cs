using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLogic : MonoBehaviour {
    [SerializeField] private AudioManager GAudioManager;
    private void Start() {
        if (GameDataManager.Instance.FirstTimeInMainMenu) {
            TransitionManager.Instance.StartTransitionFade(TransitionManager.TransitionInOutState.FADE_IN);
            GAudioManager.InitialMusicStart();
        }

        GameDataManager.Instance.FirstTimeInMainMenu = false;
    }
}
