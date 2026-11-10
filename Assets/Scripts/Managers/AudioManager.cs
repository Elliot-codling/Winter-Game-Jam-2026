using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    [SerializeField] private AudioClip BackgroundMusic;
    private AudioSource BackgroundMusicSource;
    
    [SerializeField] private float AudioVolume = 0.2f;
    
    [SerializeField] private float FadeInSpeed = 0.002f;
    
    
    private void Awake() {
        DontDestroyOnLoad(this);
    }

    private void Start() {
        // add audio source component
        BackgroundMusicSource = gameObject.AddComponent<AudioSource>();
        BackgroundMusicSource.clip = BackgroundMusic;
        BackgroundMusicSource.loop = true;
        BackgroundMusicSource.volume = AudioVolume;
    }
    
    public void InitialMusicStart() {
        BackgroundMusicSource.volume = 0.0f;
        BackgroundMusicSource.Play();

        StartCoroutine(FadeInMusic());
    }

    private IEnumerator FadeInMusic() {
        while (BackgroundMusicSource.volume < AudioVolume) {
            BackgroundMusicSource.volume += FadeInSpeed;

            yield return new WaitForEndOfFrame();
        }
        
        BackgroundMusicSource.volume = AudioVolume;
        Debug.Log("Music volume fade complete");
    }
}
