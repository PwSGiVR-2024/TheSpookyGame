using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [Header("Imports")]
    public AudioMixer audioMixer;

    [Header("-- AudioSource --")]
    [SerializeField] private AudioSource MusicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("-- Audio Clip --")]
    public AudioClip BackgroundTrack;
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
        MusicSource.clip = BackgroundTrack;
        MusicSource.Play();
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
