using UnityEngine;
using System.Collections;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    public float maxSanity = 100f;
    public float currentSanity;
    public float sanityDecayRate = 5f;
    public float sanityRecoveryRate = 10f;
    public float darknessSanityDrainRate = 8f;
    public float dinosaurSightDrainRate = 15f;
    public float dinosaurSoundDrainRate = 10f;
    
    [Header("Light Detection")]
    public float lightCheckRadius = 5f;
    public LayerMask lightLayer;
    public float minLightIntensityForSafety = 0.3f;
    
    [Header("Radio Settings")]
    public bool isListeningToRadio = false;
    public float radioHealRate = 15f;
    
    private bool isDinosaurVisible = false;
    private bool isDinosaurAudible = false;
    
    private void Start()
    {
        currentSanity = maxSanity;
        StartCoroutine(SanityCheck());
    }
    
    private void Update()
    {
        // Toggle radio with R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            isListeningToRadio = !isListeningToRadio;
        }
        
        UpdateSanity();
    }
    
    private void UpdateSanity()
    {
        float sanityChange = 0f;
        
        // Check if in darkness
        if (!IsInLight())
        {
            sanityChange -= darknessSanityDrainRate * Time.deltaTime;
        }
        
        // Check dinosaur effects
        if (isDinosaurVisible)
        {
            sanityChange -= dinosaurSightDrainRate * Time.deltaTime;
        }
        
        if (isDinosaurAudible)
        {
            sanityChange -= dinosaurSoundDrainRate * Time.deltaTime;
        }
        
        // Radio healing
        if (isListeningToRadio)
        {
            sanityChange += radioHealRate * Time.deltaTime;
        }
        
        // Apply changes
        currentSanity = Mathf.Clamp(currentSanity + sanityChange, 0f, maxSanity);
    }
    
    private bool IsInLight()
    {
        Collider[] lights = Physics.OverlapSphere(transform.position, lightCheckRadius, lightLayer);
        
        foreach (Collider lightCollider in lights)
        {
            Light light = lightCollider.GetComponent<Light>();
            if (light != null)
            {
                float distance = Vector3.Distance(transform.position, light.transform.position);
                float intensity = light.intensity * (1 - (distance / light.range));
                
                if (intensity >= minLightIntensityForSafety)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private IEnumerator SanityCheck()
    {
        while (true)
        {
            // Trigger horror effects when sanity is below 50%
            if (GetSanityPercentage() < 0.5f)
            {
                TriggerHorrorEffects();
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
    
    public void OnDinosaurSighted(bool isSighted)
    {
        isDinosaurVisible = isSighted;
    }
    
    public void OnDinosaurSound(bool isHeard)
    {
        isDinosaurAudible = isHeard;
    }
    
    public float GetSanityPercentage()
    {
        return currentSanity / maxSanity;
    }
    
    private void TriggerHorrorEffects()
    {
        // Notify any subscribers that horror effects should be triggered
        // This will be used by the dinosaur models to morph into their horror versions
        DinosaurHorrorEvent.Trigger(GetSanityPercentage() < 0.5f);
    }
}

// Event system for horror effects
public static class DinosaurHorrorEvent
{
    public delegate void HorrorStateChanged(bool isHorrorActive);
    public static event HorrorStateChanged OnHorrorStateChanged;
    
    public static void Trigger(bool isHorrorActive)
    {
        OnHorrorStateChanged?.Invoke(isHorrorActive);
    }
}
