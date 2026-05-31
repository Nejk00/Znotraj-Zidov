using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioSource enemyAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    
    [Header("Player Sounds")]
    public AudioClip playerFootstep;
    
    [Header("Enemy Sounds")]
    public AudioClip enemyPatrolFootstep;  // Slow, calm footsteps
    public AudioClip enemyChaseFootstep;   // Fast, intense footsteps
    
    [Header("Other Sounds")]
    public AudioClip doorOpenSound;
    public AudioClip doorLockedSound;
    public AudioClip slidingDoor;
    
    [Header("Audio Settings")]
    [Range(0f, 500f)] public float minDistance = 5f;
    [Range(0f, 500f)] public float maxDistance = 50f;
    [Range(0f, 1f)] public float defaultVolume = 0.7f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        SetupAudioSources();
    }
    
    private void SetupAudioSources()
    {
        if (playerAudioSource == null)
        {
            GameObject playerAudioObj = new GameObject("PlayerAudioSource");
            playerAudioObj.transform.parent = transform;
            playerAudioSource = playerAudioObj.AddComponent<AudioSource>();
        }
        
        if (enemyAudioSource == null)
        {
            GameObject enemyAudioObj = new GameObject("EnemyAudioSource");
            enemyAudioObj.transform.parent = transform;
            enemyAudioSource = enemyAudioObj.AddComponent<AudioSource>();
        }
        
        ConfigureAudioSource(playerAudioSource);
        ConfigureAudioSource(enemyAudioSource);
    }
    
    private void ConfigureAudioSource(AudioSource source)
    {
        source.spatialBlend = 1f;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.playOnAwake = false;
        source.loop = false;
        
        if (VolumeManager.Instance != null)
        {
            VolumeManager.Instance.OnSFXVolumeChanged += (volume) => 
            {
                source.volume = volume / 100f;
            };
            source.volume = VolumeManager.Instance?.CurrentSFXVolume / 100f ?? 0.8f;
        }
    }
    
    public void PlayPlayerFootstep(Vector3 position, float additionalVolume = 1f)
    {
        if (playerFootstep == null) return;
        
        playerAudioSource.transform.position = position;
        playerAudioSource.clip = playerFootstep;
        
        float finalVolume = (VolumeManager.Instance?.CurrentSFXVolume ?? 80f) / 100f * additionalVolume;
        playerAudioSource.volume = finalVolume;
        playerAudioSource.Play();
    }
    
    public void PlayEnemyFootstep(Vector3 position, bool isChasing, float additionalVolume = 1f)
    {
        // Choose the correct sound based on enemy state
        AudioClip clipToPlay = isChasing ? enemyChaseFootstep : enemyPatrolFootstep;
        
        if (clipToPlay == null) return;
        
        enemyAudioSource.transform.position = position;
        enemyAudioSource.clip = clipToPlay;
        
        // Different volume for chase vs patrol (chase is louder)
        float stateVolume = isChasing ? 1f : 0.7f;
        float finalVolume = (VolumeManager.Instance?.CurrentSFXVolume ?? 80f) / 100f * stateVolume * additionalVolume;
        
        enemyAudioSource.volume = finalVolume;
        enemyAudioSource.Play();
    }
    public void PlaySound(AudioClip clip, Vector3 position, float volume = -1f)
    {
        if (clip == null) return;
        
        float finalVolume = volume < 0 ? defaultVolume : volume;
        
        if (sfxAudioSource != null)
        {
            sfxAudioSource.transform.position = position;
            sfxAudioSource.PlayOneShot(clip, finalVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, position, finalVolume);
        }
    }
    
    public void PlayDoorSound(Vector3 position)
    {
        PlaySound(doorOpenSound, position);
    }
    
    public void PlayLockedSound(Vector3 position)
    {
        PlaySound(doorLockedSound, position);
    }
    public void PlaySlidingDoorSound(Vector3 position)
    {
        PlaySound(slidingDoor, position);
    }
}