using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepController : MonoBehaviour
{
    [Header("Footstep Settings")]
    public AudioSource footstepAudioSource; // The AudioSource for playing footstep sounds

    [Header("Default Footstep Sounds (Concrete/Tile)")]
    public AudioClip[] defaultWalkSounds; // Default walk sounds
    public AudioClip[] defaultRunSounds;  // Default run sounds
    public AudioClip[] defaultJumpSounds; // Default jump sounds
    public float walkStepInterval = 0.5f; // Time between steps while walking
    public float runStepInterval = 0.3f;  // Time between steps while running
    public AudioClip defaultLandSound;    // Landing sound

    [Header("Surface-Specific Footsteps (Optional)")]
    public AudioClip[] grassWalkSounds;
    public AudioClip[] grassRunSounds;
    public AudioClip[] grassJumpSounds;

    [Header("Movement Settings")]
    public float runThreshold = 5f;       // Speed threshold to determine running
    public float movementSpeed = 0f;     // Current speed of the player (set dynamically)

    private CharacterController characterController; // Reference to the CharacterController
    private bool isMoving = false;
    private bool isGrounded = true;

    void Start()
    {
        // Ensure an AudioSource is assigned
        if (footstepAudioSource == null)
        {
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // Ensure a CharacterController is attached
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("No CharacterController found on the GameObject!");
        }
    }

    void Update()
    {
        movementSpeed = characterController.velocity.magnitude; // Update current movement speed
        isGrounded = characterController.isGrounded;

        HandleFootsteps();

        // Play jump or landing sounds when relevant
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            PlayJumpSound();
        }
        else if (!isGrounded && characterController.isGrounded) // Land detection
        {
            PlayLandingSound();
        }
    }

    private void HandleFootsteps()
    {
        // Check if the player is moving and grounded
        isMoving = movementSpeed > 0.1f && isGrounded;

        if (isMoving && !footstepAudioSource.isPlaying)
        {
            StartCoroutine(PlayFootstepSounds());
        }
    }

    private IEnumerator PlayFootstepSounds()
    {
        while (isMoving)
        {
            float stepInterval = movementSpeed > runThreshold ? runStepInterval : walkStepInterval;

            // Determine which sounds to play
            AudioClip[] currentSounds = GetCurrentFootstepSounds();

            // Play a random footstep sound from the current array
            if (currentSounds.Length > 0)
            {
                footstepAudioSource.clip = currentSounds[Random.Range(0, currentSounds.Length)];
                footstepAudioSource.Play();
            }

            // Wait for the interval before playing the next sound
            yield return new WaitForSeconds(stepInterval);
        }
    }

    private AudioClip[] GetCurrentFootstepSounds()
    {
        // Default to concrete/tile sounds
        AudioClip[] currentWalkSounds = defaultWalkSounds;
        AudioClip[] currentRunSounds = defaultRunSounds;

        // Surface-specific logic (optional)
        // For example, you could implement raycast detection to switch sounds dynamically:
        // if (IsOnGrass()) { currentWalkSounds = grassWalkSounds; currentRunSounds = grassRunSounds; }

        return movementSpeed > runThreshold ? currentRunSounds : currentWalkSounds;
    }

    private void PlayJumpSound()
    {
        AudioClip[] currentJumpSounds = defaultJumpSounds;

        // Surface-specific jump logic (optional, as above)
        // if (IsOnGrass()) { currentJumpSounds = grassJumpSounds; }

        if (currentJumpSounds.Length > 0)
        {
            footstepAudioSource.clip = currentJumpSounds[Random.Range(0, currentJumpSounds.Length)];
            footstepAudioSource.Play();
        }
    }

    private void PlayLandingSound()
    {
        if (defaultLandSound != null)
        {
            footstepAudioSource.clip = defaultLandSound;
            footstepAudioSource.Play();
        }
    }
}
