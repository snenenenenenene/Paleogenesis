using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerFeedback : MonoBehaviour
{
    public PlayerAttributes playerAttributes;

    [Header("Audio Feedback")]
    public AudioSource heartbeatAudio;
    public AudioSource heavyBreathingAudio;
    
    [Header("Post-Processing Effects")]
    public PostProcessVolume postProcessVolume;
    private Vignette vignetteEffect;
    private ChromaticAberration chromaticAberrationEffect;

    private float lowHealthThreshold = 30f;
    private float lowStaminaThreshold = 20f;

    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out vignetteEffect);
        postProcessVolume.profile.TryGetSettings(out chromaticAberrationEffect);
    }

    void Update()
    {
        UpdateHealthFeedback();
        UpdateStaminaFeedback();
    }

    private void UpdateHealthFeedback()
    {
        float healthPercent = playerAttributes.currentHealth / playerAttributes.maxHealth;

        // Increase Vignette effect as health decreases
        if (vignetteEffect != null)
        {
            vignetteEffect.intensity.value = Mathf.Lerp(0f, 0.5f, 1f - healthPercent);
        }

        // Chromatic Aberration for low health
        if (chromaticAberrationEffect != null)
        {
            chromaticAberrationEffect.intensity.value = healthPercent < 0.3f ? 0.3f : 0f;
        }

        // Play heartbeat sound based on low health threshold
        if (playerAttributes.currentHealth <= lowHealthThreshold && !heartbeatAudio.isPlaying)
        {
            heartbeatAudio.Play();
        }
        else if (playerAttributes.currentHealth > lowHealthThreshold && heartbeatAudio.isPlaying)
        {
            heartbeatAudio.Stop();
        }
    }

    private void UpdateStaminaFeedback()
    {
        float staminaPercent = playerAttributes.currentStamina / playerAttributes.maxStamina;

        // Screen pulse effect (simulated with Vignette) as stamina gets low
        if (vignetteEffect != null && playerAttributes.currentStamina <= lowStaminaThreshold)
        {
            vignetteEffect.intensity.value = Mathf.PingPong(Time.time, 0.1f);  // Light pulsing effect
        }

        // Heavy breathing based on low stamina threshold
        if (playerAttributes.currentStamina <= lowStaminaThreshold && !heavyBreathingAudio.isPlaying)
        {
            heavyBreathingAudio.Play();
        }
        else if (playerAttributes.currentStamina > lowStaminaThreshold && heavyBreathingAudio.isPlaying)
        {
            heavyBreathingAudio.Stop();
        }
    }
}
