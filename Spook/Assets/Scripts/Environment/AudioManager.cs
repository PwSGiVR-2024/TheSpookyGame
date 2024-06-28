using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [Header("Imports")]
    public AudioMixer audioMixer;

    [Header("-- AudioSource --")]
    [SerializeField] private AudioSource MusicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("-- Audio Clip --")]
    public AudioClip BackgroundTrack; // For Main Menu
    public AudioClip BackgroundTrack2; // For Main Level
    public AudioClip ButtonClick;
    public AudioClip ButtonHover;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("AudioManager");
                    instance = go.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Play initial background track
        PlayMusicForCurrentScene();

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        // Get the current scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Check the scene name and play the corresponding audio
        if (currentScene.name == "Main Menu")
        {
            MusicSource.clip = BackgroundTrack;
        }
        else if (currentScene.name == "Main Level")
        {
            MusicSource.clip = BackgroundTrack2;
        }
        else
        {
            MusicSource.clip = null;
        }

        // Play the audio if there is a clip
        if (MusicSource.clip != null)
        {
            MusicSource.Play();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void SetMasterVolume(float Volume)
    {
        Volume = Mathf.Clamp(Volume, 0.0001F, 1);
        audioMixer.SetFloat("MasterVolume", Volume > 0 ? Mathf.Log10(Volume) * 20 : -80);
    }

    public void SetMusicVolume(float Volume)
    {
        Volume = Mathf.Clamp(Volume, 0.0001F, 1);
        audioMixer.SetFloat("MusicVolume", Volume > 0 ? Mathf.Log10(Volume) * 20 : -80);
    }

    public void SetSFXVolume(float Volume)
    {
        Volume = Mathf.Clamp(Volume, 0.0001F, 1);
        audioMixer.SetFloat("SFXVolume", Volume > 0 ? Mathf.Log10(Volume) * 20 : -80);
    }
}
