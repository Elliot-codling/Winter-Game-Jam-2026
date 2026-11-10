using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager {
    private static TransitionManager SInstance = null;
    public static TransitionManager Instance
    {
        get { return SInstance ??= new TransitionManager(); }
    }

    public enum TransitionFadeState {
        NONE,
        IN_PROGRESS,
        COMPLETED
    }

    public TransitionFadeState FadeState { get; private set; } = TransitionFadeState.NONE;

    public enum TransitionInOutState {
        NONE,
        FADE_IN,
        FADE_OUT
    }

    public Task CurrentFadeTask { get; private set; }

    // Canvas
    private Canvas FadeCanvas;
    private Image FadeImage;

    // Fade Settings
    readonly float FadeSpeed = 3.0f;

    private readonly float FadeMinTol = 0.0001f;
    private readonly float FadeMaxTol = 0.9999f;

    private void GetTransitionFadeImage() {
        FadeCanvas = GameObject.FindGameObjectWithTag("TransitionCanvas").GetComponent<Canvas>();
        FadeImage = FadeCanvas.gameObject.GetComponentInChildren<Image>();
    }

    // FADE_IN shows game, FADE_OUT hides game
    public void StartTransitionFade(TransitionInOutState _inOutState) {
        // Has task already started?
        if (FadeState == TransitionFadeState.IN_PROGRESS) {
            Debug.Log("FadeTask already started");
            return;
        }

        GetTransitionFadeImage();

        // Does Image exist?
        if (FadeImage == null) {
            Debug.Log("FadeImage is null");
            return;
        }


        Debug.Log("Starting FadeTask");
        // Start fade from state parameter
        switch (_inOutState) {
            case (TransitionInOutState.FADE_IN):
                CurrentFadeTask = FadeInTask();
                break;
            case (TransitionInOutState.FADE_OUT):
                CurrentFadeTask = FadeOutTask();
                break;
            default:
                Debug.Log("Invalid in/out state passed in");
                break;
        }
    }

    #region FadeTasks
    private async Task FadeInTask() {
        FadeState = TransitionFadeState.IN_PROGRESS;

        while (FadeImage.color.a > FadeMinTol) {
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b,
                FadeImage.color.a - FadeSpeed * Time.deltaTime);

            await Task.Yield();
        }

        FadeState = TransitionFadeState.COMPLETED;
    }
    private async Task FadeOutTask() {
        FadeState = TransitionFadeState.IN_PROGRESS;

        while (FadeImage.color.a < FadeMaxTol)
        {
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b,
                FadeImage.color.a + FadeSpeed * Time.deltaTime);

            await Task.Yield();
        }

        FadeState = TransitionFadeState.COMPLETED;
    }
    #endregion

    public void ForceFadeImageState(bool _shouldGameBeVisible) {
        if (_shouldGameBeVisible) {
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b,
                                        0.0f);

            return;
        }

        FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b,
                                    1.0f);
    }
}
