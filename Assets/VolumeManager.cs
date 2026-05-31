using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }
    
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Volume Parameters")]
    [SerializeField] private string masterVolumeParam = "MasterVolume";
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";
    
    [Header("Default Volumes (0-100)")]
    [SerializeField] private float defaultMasterVolume = 80f;
    [SerializeField] private float defaultMusicVolume = 70f;
    [SerializeField] private float defaultSFXVolume = 80f;
    
    // Current volumes (0-100)
    public float CurrentMasterVolume { get; private set; }
    public float CurrentMusicVolume { get; private set; }
    public float CurrentSFXVolume { get; private set; }
    
    // Events for UI updates
    public System.Action<float> OnMasterVolumeChanged;
    public System.Action<float> OnMusicVolumeChanged;
    public System.Action<float> OnSFXVolumeChanged;
    
    private const string MASTER_VOL_KEY = "MasterVolume";
    private const string MUSIC_VOL_KEY = "MusicVolume";
    private const string SFX_VOL_KEY = "SFXVolume";
    
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
        
        LoadVolumes();
    }
    
    private void LoadVolumes()
    {
        // Load saved values or use defaults
        CurrentMasterVolume = PlayerPrefs.GetFloat(MASTER_VOL_KEY, defaultMasterVolume);
        CurrentMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, defaultMusicVolume);
        CurrentSFXVolume = PlayerPrefs.GetFloat(SFX_VOL_KEY, defaultSFXVolume);
        
        // Apply to audio mixer
        SetMasterVolume(CurrentMasterVolume, false);
        SetMusicVolume(CurrentMusicVolume, false);
        SetSFXVolume(CurrentSFXVolume, false);
    }
    
    public void SetMasterVolume(float volume01, bool save = true)
    {
        // volume01 should be 0-100
        CurrentMasterVolume = Mathf.Clamp(volume01, 0f, 100f);
        
        // Convert to decibels (-80dB to 0dB)
        float dB = ConvertToDecibels(CurrentMasterVolume);
        audioMixer?.SetFloat(masterVolumeParam, dB);
        
        if (save)
            PlayerPrefs.SetFloat(MASTER_VOL_KEY, CurrentMasterVolume);
        
        OnMasterVolumeChanged?.Invoke(CurrentMasterVolume);
    }
    
    public void SetMusicVolume(float volume01, bool save = true)
    {
        CurrentMusicVolume = Mathf.Clamp(volume01, 0f, 100f);
        float dB = ConvertToDecibels(CurrentMusicVolume);
        audioMixer?.SetFloat(musicVolumeParam, dB);
        
        if (save)
            PlayerPrefs.SetFloat(MUSIC_VOL_KEY, CurrentMusicVolume);
        
        OnMusicVolumeChanged?.Invoke(CurrentMusicVolume);
    }
    
    public void SetSFXVolume(float volume01, bool save = true)
    {
        CurrentSFXVolume = Mathf.Clamp(volume01, 0f, 100f);
        float dB = ConvertToDecibels(CurrentSFXVolume);
        audioMixer?.SetFloat(sfxVolumeParam, dB);
        
        if (save)
            PlayerPrefs.SetFloat(SFX_VOL_KEY, CurrentSFXVolume);
        
        OnSFXVolumeChanged?.Invoke(CurrentSFXVolume);
    }
    
    private float ConvertToDecibels(float volume01)
    {
        if (volume01 <= 0) return -80f; // Muted
        return Mathf.Log10(volume01 / 100f) * 20f;
    }
    
    // Helper method for UI sliders (0-1 range)
    public void SetMasterVolumeNormalized(float normalizedVolume)
    {
        SetMasterVolume(normalizedVolume * 100f);
    }
    
    public void SetMusicVolumeNormalized(float normalizedVolume)
    {
        SetMusicVolume(normalizedVolume * 100f);
    }
    
    public void SetSFXVolumeNormalized(float normalizedVolume)
    {
        SetSFXVolume(normalizedVolume * 100f);
    }
    
    public float GetMasterVolumeNormalized() => CurrentMasterVolume / 100f;
    public float GetMusicVolumeNormalized() => CurrentMusicVolume / 100f;
    public float GetSFXVolumeNormalized() => CurrentSFXVolume / 100f;
}