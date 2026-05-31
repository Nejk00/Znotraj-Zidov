using UnityEngine;

public class SimpleMusicManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private float musicVolume = 0.4f;
    
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.clip = gameplayMusic;
        audioSource.loop = true;
        audioSource.volume = musicVolume;
        audioSource.Play();
    }
    
    void Update()
    {
        // Update volume based on master volume
        if (VolumeManager.Instance != null)
        {
            audioSource.volume = musicVolume * (VolumeManager.Instance.CurrentMasterVolume / 100f);
        }
    }
}