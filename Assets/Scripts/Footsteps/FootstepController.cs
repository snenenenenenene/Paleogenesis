using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource footstepAudioSource; // The AudioSource for playing footstep sounds
    public AudioClip walkSound; // Single walking sound
    public AudioClip runSound;  // Single running sound

    [Header("Footstep Timing")]
    public float walkStepInterval = 0.5f; // Time between steps while walking
    public float runStepInterval = 0.3f;  // Time between steps while running

    [Header("Movement Settings")]
    public float runThreshold = 5f; // Speed threshold to determine running

    private CharacterController characterController;
    private Coroutine footstepCoroutine;
    private float movementSpeed = 0f; // Current speed of the player

    void Start()
    {
        // Ensure the CharacterController is attached
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController is missing!");
            return;
        }

        // Ensure the AudioSource is assigned
        if (footstepAudioSource == null)
        {
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("No AudioSource found. A default AudioSource has been added.");
        }

        // Check if walkSound and runSound are assigned
        if (walkSound == null || runSound == null)
        {
            Debug.LogError("Footstep sounds are missing! Please assign walkSound and runSound in the Inspector.");
        }
    }

    void Update()
    {
        movementSpeed = characterController.velocity.magnitude;

        if (IsMoving() && footstepCoroutine == null)
        {
            Debug.Log("Starting footstep coroutine.");
            footstepCoroutine = StartCoroutine(PlayFootstepSound());
        }
        else if (!IsMoving() && footstepCoroutine != null)
        {
            Debug.Log("Stopping footstep coroutine.");
            StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }
    }

    private bool IsMoving()
    {
        // The player is considered moving if their speed is above a small threshold and they're grounded
        return movementSpeed > 0.1f && characterController.isGrounded;
    }

    private IEnumerator PlayFootstepSound()
    {
        while (IsMoving())
        {
            // Play walking or running sound based on movement speed
            if (movementSpeed > runThreshold)
            {
                if (runSound != null)
                {
                    Debug.Log("Playing run sound.");
                    footstepAudioSource.PlayOneShot(runSound);
                }
                else
                {
                    Debug.LogError("Run sound is not assigned!");
                }
                yield return new WaitForSeconds(runStepInterval);
            }
            else
            {
                if (walkSound != null)
                {
                    Debug.Log("Playing walk sound.");
                    footstepAudioSource.PlayOneShot(walkSound);
                }
                else
                {
                    Debug.LogError("Walk sound is not assigned!");
                }
                yield return new WaitForSeconds(walkStepInterval);
            }
        }
    }
}
