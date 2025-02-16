using UnityEngine;
using System.Collections.Generic;

public class FootstepSystem : MonoBehaviour
{
    [System.Serializable]
    public class FootstepSoundSet
    {
        public string surfaceTag;
        public AudioClip[] clips;
    }

    [Header("Audio Settings")]
    public AudioSource footstepSource;
    public FootstepSoundSet[] footstepSounds;
    public float minTimeBetweenSteps = 0.3f;
    public float sprintStepMultiplier = 0.7f;
    public float crouchStepMultiplier = 1.5f;
    
    [Header("Volume Settings")]
    public float baseVolume = 0.7f;
    public float volumeVariation = 0.1f;
    public float pitchVariation = 0.1f;

    private PlayerMovement playerMovement;
    private CharacterController characterController;
    private float lastStepTime;
    private string currentSurface = "Rock"; // Default surface

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();
        
        if (footstepSource == null)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.spatialBlend = 1f; // 3D sound
            footstepSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (!characterController.isGrounded || playerMovement.IsUnderwater())
            return;

        // Check if player is moving
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        if (horizontalVelocity.magnitude > 0.1f)
        {
            float stepDelay = minTimeBetweenSteps;
            
            // Adjust step timing based on movement state
            if (playerMovement.isSprinting)
                stepDelay *= sprintStepMultiplier;
            else if (playerMovement.isCrouching)
                stepDelay *= crouchStepMultiplier;

            // Check if enough time has passed for next step
            if (Time.time - lastStepTime > stepDelay)
            {
                PlayFootstep();
                lastStepTime = Time.time;
            }
        }
    }

    private void PlayFootstep()
    {
        FootstepSoundSet soundSet = GetSoundSetForSurface(currentSurface);
        if (soundSet == null || soundSet.clips.Length == 0)
            return;

        // Get random clip from the set
        AudioClip clip = soundSet.clips[Random.Range(0, soundSet.clips.Length)];
        
        // Randomize volume and pitch slightly
        footstepSource.volume = baseVolume + Random.Range(-volumeVariation, volumeVariation);
        footstepSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        
        footstepSource.PlayOneShot(clip);
    }

    private FootstepSoundSet GetSoundSetForSurface(string surface)
    {
        // If we're on a surface tagged as "Ground", use Rock sounds as default
        if (surface == "Ground")
        {
            foreach (FootstepSoundSet set in footstepSounds)
            {
                if (set.surfaceTag == "Rock")
                    return set;
            }
        }

        // Otherwise try to find exact surface match
        foreach (FootstepSoundSet set in footstepSounds)
        {
            if (set.surfaceTag.Equals(surface, System.StringComparison.OrdinalIgnoreCase))
                return set;
        }

        // If no match found and we have Rock sounds, use those as ultimate fallback
        foreach (FootstepSoundSet set in footstepSounds)
        {
            if (set.surfaceTag == "Rock")
                return set;
        }

        return null;
    }

    // Call this method when the player changes surfaces
    public void UpdateSurface(string newSurface)
    {
        currentSurface = newSurface;
    }
} 