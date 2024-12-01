using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TRexAI : MonoBehaviour
{
    public Transform player; // Player's Transform (set in Inspector)
    private NavMeshAgent agent;
    private Animator animator;

    [Header("T-Rex Settings")]
    public float detectionRange = 60f; // Detection range
    public float chaseRange = 40f; // Chasing range
    public float stopDistance = 3f; // Attack distance
    public float roarRange = 50f; // Distance at which the T-Rex roars upon spotting the player
    public LayerMask playerLayer; // Layer for player detection
    public AudioSource roarAudio; // Roar sound effect
    public AudioSource footstepAudio; // Footstep sound effect

    [Header("T-Rex Characteristics")]
    public float roarCooldown = 10f; // Time between roars
    private float lastRoarTime = -10f;

    private bool isStalking = false; // Whether the T-Rex is stalking
    private bool hasRoaredAtPlayer = false; // Ensures the roar happens once upon detection

    private enum TRexState { Idle, Stalking, Chasing, Attacking }
    private TRexState currentState = TRexState.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned! Assign it in the Inspector.");
        }

        if (roarAudio == null || footstepAudio == null)
        {
            Debug.LogError("Roar and footstep AudioSources must be assigned!");
        }

        // Set initial state
        SetState(TRexState.Idle);
    }

    void Update()
    {
        switch (currentState)
        {
            case TRexState.Idle:
                IdleBehavior();
                break;

            case TRexState.Stalking:
                StalkPlayer();
                break;

            case TRexState.Chasing:
                ChasePlayer();
                break;

            case TRexState.Attacking:
                AttackPlayer();
                break;
        }

        DetectPlayer();
    }

    private void IdleBehavior()
    {
        SetWalking(false);
        // Play idle animation (handled via Animator)
    }

    private void StalkPlayer()
    {
        if (!isStalking)
        {
            isStalking = true;
            SetWalking(true);
            Debug.Log("T-Rex is stalking...");
        }

        // Move closer but stay just outside chase range
        float targetDistance = Mathf.Clamp(Vector3.Distance(transform.position, player.position) - 1f, chaseRange + 5f, chaseRange);
        Vector3 stalkingPosition = Vector3.MoveTowards(transform.position, player.position, targetDistance);
        agent.SetDestination(stalkingPosition);

        // Transition to chase if too close
        if (Vector3.Distance(transform.position, player.position) <= chaseRange)
        {
            SetState(TRexState.Chasing);
        }
    }

    private void ChasePlayer()
    {
        SetWalking(true);
        Debug.Log("T-Rex is chasing the player!");

        agent.SetDestination(player.position);

        // Play footsteps if not already playing
        if (!footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }

        // Attack if close enough
        if (Vector3.Distance(transform.position, player.position) <= stopDistance)
        {
            SetState(TRexState.Attacking);
        }
    }

    private void AttackPlayer()
    {
        SetWalking(false);
        Debug.Log("T-Rex is attacking the player!");

        // Face the player before attacking
        Vector3 direction = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // If player escapes attack range, go back to chase
        if (Vector3.Distance(transform.position, player.position) > stopDistance)
        {
            SetState(TRexState.Chasing);
        }
    }

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (currentState == TRexState.Idle)
            {
                SetState(TRexState.Stalking);
            }

            // Roar when detecting the player (once per cooldown)
            if (distanceToPlayer <= roarRange && Time.time - lastRoarTime >= roarCooldown)
            {
                Roar();
                lastRoarTime = Time.time;
                hasRoaredAtPlayer = true;
            }
        }
    }

    private void Roar()
    {
        if (roarAudio != null && !roarAudio.isPlaying)
        {
            Debug.Log("T-Rex roars loudly!");
            roarAudio.Play();

            // Play roar animation
            if (animator != null)
            {
                animator.SetTrigger("Roar");
            }
        }
    }

    private void SetState(TRexState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case TRexState.Idle:
                SetWalking(false);
                break;

            case TRexState.Stalking:
                isStalking = true;
                SetWalking(true);
                break;

            case TRexState.Chasing:
                isStalking = false;
                SetWalking(true);
                break;

            case TRexState.Attacking:
                SetWalking(false);
                break;
        }
    }

    private void SetWalking(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking);
        }

        if (!isWalking && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }
}
