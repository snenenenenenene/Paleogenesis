using UnityEngine;

public class RadioSystem : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip[] musicTracks;
    public float volume = 0.5f;
    public bool playRandomTracks = true;
    
    [Header("Radio Settings")]
    public float batteryDrainRate = 5f;
    public float maxBattery = 100f;
    public float currentBattery;
    public KeyCode toggleKey = KeyCode.R;
    
    private AudioSource audioSource;
    private SanitySystem sanitySystem;
    private bool _isPlaying = false;
    private int currentTrackIndex = 0;
    
    // Public property to access isPlaying
    public bool IsPlaying => _isPlaying;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        sanitySystem = GetComponent<SanitySystem>();
        currentBattery = maxBattery;
        
        audioSource.volume = volume;
        audioSource.loop = !playRandomTracks;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleRadio();
        }
        
        if (_isPlaying)
        {
            UpdateBattery();
            CheckTrackChange();
        }
    }
    
    private void ToggleRadio()
    {
        if (currentBattery <= 0)
        {
            return;
        }
        
        _isPlaying = !_isPlaying;
        
        if (_isPlaying)
        {
            PlayRadio();
        }
        else
        {
            StopRadio();
        }
        
        if (sanitySystem != null)
        {
            sanitySystem.isListeningToRadio = _isPlaying;
        }
    }
    
    private void PlayRadio()
    {
        if (musicTracks != null && musicTracks.Length > 0)
        {
            audioSource.clip = musicTracks[currentTrackIndex];
            audioSource.Play();
        }
    }
    
    private void StopRadio()
    {
        audioSource.Stop();
    }
    
    private void UpdateBattery()
    {
        currentBattery -= batteryDrainRate * Time.deltaTime;
        
        if (currentBattery <= 0)
        {
            currentBattery = 0;
            StopRadio();
            _isPlaying = false;
            
            if (sanitySystem != null)
            {
                sanitySystem.isListeningToRadio = false;
            }
        }
    }
    
    private void CheckTrackChange()
    {
        if (playRandomTracks && !audioSource.isPlaying)
        {
            currentTrackIndex = Random.Range(0, musicTracks.Length);
            PlayRadio();
        }
    }
    
    public void AddBattery(float amount)
    {
        currentBattery = Mathf.Min(currentBattery + amount, maxBattery);
    }
    
    public float GetBatteryPercentage()
    {
        return currentBattery / maxBattery;
    }
} 