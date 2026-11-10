using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class SceneManager {
    private static SceneManager SInstance = null;
    public static SceneManager Instance {
        get { return SInstance ??= new SceneManager(); }
    }

    private Scene CurrentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

    public async Task ChangeScene(string _sceneID) {
        // Play fade
        TransitionManager.Instance.StartTransitionFade(TransitionManager.TransitionInOutState.FADE_OUT);

        await TransitionManager.Instance.CurrentFadeTask;
        Debug.Log("Fade complete, starting loading and unloading scenes");

        Stopwatch stopwatch = Stopwatch.StartNew();

        var asyncOp0 = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_sceneID, LoadSceneMode.Additive);
        
        // Wait until new scene is loaded
        Debug.Log("Started Loading Scene " + _sceneID);
        
        await asyncOp0;

        // Start unloading past scene, will keep this later on as we want to keep adjacent rooms
        var asyncOp1 = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(CurrentScene.name);

        // Wait for unloading to finish
        Debug.Log("Started Unloading Old Scene ");
        await asyncOp1;

        // Unload unused assets
        await Resources.UnloadUnusedAssets();

        stopwatch.Stop();

        // Play fade
        TransitionManager.Instance.StartTransitionFade(TransitionManager.TransitionInOutState.FADE_IN);

        CurrentScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(_sceneID);
        Debug.Log("Done changing scene to " + _sceneID + ". Took " + stopwatch.ElapsedMilliseconds + "ms");
    }
}
