using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UtahraptorAI : MonoBehaviour
{
    [Header("Stalking Settings")]
    public float stalkingSpeed = 8f;
    public float attackingSpeed = 12f;
    public float attackRange = 3f;
    public float freezeDistance = 20f;
    public float viewAngle = 90f; // Angle at which the player can be seen
    public LayerMask obstacleLayer; // Layer for obstacles that block line of sight
    
    [Header("Detection Settings")]
    public float noiseDetectionRange = 15f;
    public float scentDetectionRange = 10f;
    public float investigationTime = 5f;
    public float windStrength = 1f;
    public Vector3 windDirection = Vector3.right;
    
    [Header("Attack Settings")]
    public float attackDamage = 25f;
    public float attackCooldown = 1.5f;
    
    private NavMeshAgent agent;
    private Transform player;
    private Camera playerCamera;
    private bool isPlayerLooking;
    private bool canAttack = true;
    private float lastAttackTime;
    private Vector3 investigationPoint;
    private float investigationTimer;
    private AIState currentState = AIState.Patrolling;
    
    private enum AIState
    {
        Patrolling,
        Investigating,
        Chasing,
        Frozen
    }
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (player == null)
        {
            Debug.LogError("UtahraptorAI: Player not found!");
            enabled = false;
            return;
        }
        
        // Find player's camera
        playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("UtahraptorAI: Player camera not found!");
        }
        
        // Set initial agent settings
        agent.speed = stalkingSpeed;
        agent.stoppingDistance = attackRange * 0.8f;
    }
    
    private void Update()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check if player is looking at raptor
        isPlayerLooking = IsPlayerLookingAtRaptor();
        
        // Update state based on conditions
        UpdateAIState(distanceToPlayer);
        
        // Handle behavior based on current state
        switch (currentState)
        {
            case AIState.Frozen:
                HandleFrozenState();
                break;
            case AIState.Investigating:
                HandleInvestigationState();
                break;
            case AIState.Chasing:
                HandleChaseState(distanceToPlayer);
                break;
            case AIState.Patrolling:
                HandlePatrolState();
                break;
        }
        
        // Check for noise detection
        CheckNoiseDetection();
        
        // Check for scent detection
        CheckScentDetection();
    }
    
    private void UpdateAIState(float distanceToPlayer)
    {
        if (isPlayerLooking)
        {
            currentState = AIState.Frozen;
        }
        else if (CanSeePlayer() || distanceToPlayer <= attackRange)
        {
            currentState = AIState.Chasing;
        }
        else if (investigationTimer > 0)
        {
            currentState = AIState.Investigating;
        }
        else
        {
            currentState = AIState.Patrolling;
        }
    }
    
    private void HandleFrozenState()
    {
        agent.isStopped = true;
        
        // Notify the SanitySystem that player is seeing a dinosaur
        SanitySystem sanitySystem = player.GetComponent<SanitySystem>();
        if (sanitySystem != null)
        {
            sanitySystem.OnDinosaurSighted(true);
        }
    }
    
    private void HandleInvestigationState()
    {
        agent.isStopped = false;
        agent.speed = stalkingSpeed;
        agent.SetDestination(investigationPoint);
        
        investigationTimer -= Time.deltaTime;
        
        // If we've reached the investigation point or timer is up, return to patrolling
        if (investigationTimer <= 0 || Vector3.Distance(transform.position, investigationPoint) < 1f)
        {
            investigationTimer = 0;
            currentState = AIState.Patrolling;
        }
    }
    
    private void HandleChaseState(float distanceToPlayer)
    {
        agent.isStopped = false;
        agent.speed = attackingSpeed;
        agent.SetDestination(player.position);
        
        // Attack if in range
        if (distanceToPlayer <= attackRange && canAttack)
        {
            Attack();
        }
    }
    
    private void HandlePatrolState()
    {
        // Simple patrol behavior - can be enhanced with waypoints
        if (!agent.hasPath || (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance < 0.5f))
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 10f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }
    
    private void CheckNoiseDetection()
    {
        if (currentState == AIState.Frozen) return;
        
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            float noiseLevel = playerMovement.GetCurrentNoiseLevel();
            float detectionRange = noiseDetectionRange * noiseLevel;
            
            if (Vector3.Distance(transform.position, player.position) <= detectionRange)
            {
                investigationPoint = player.position;
                investigationTimer = investigationTime;
                currentState = AIState.Investigating;
            }
        }
    }
    
    private void CheckScentDetection()
    {
        if (currentState == AIState.Frozen) return;
        
        // Calculate scent direction based on wind
        Vector3 playerToRaptor = transform.position - player.position;
        float windInfluence = Vector3.Dot(playerToRaptor.normalized, windDirection.normalized);
        
        // Stronger detection when downwind from player
        float adjustedRange = scentDetectionRange * (1 + windInfluence * windStrength);
        
        if (Vector3.Distance(transform.position, player.position) <= adjustedRange)
        {
            currentState = AIState.Investigating;
            investigationPoint = player.position;
            investigationTimer = investigationTime;
        }
    }
    
    private bool CanSeePlayer()
    {
        if (player == null) return false;
        
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        
        if (angle <= viewAngle * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, 
                out hit, directionToPlayer.magnitude, obstacleLayer))
            {
                return hit.transform.CompareTag("Player");
            }
            return true;
        }
        
        return false;
    }
    
    private bool IsPlayerLookingAtRaptor()
    {
        if (player == null || playerCamera == null) return false;
        
        Vector3 directionToRaptor = transform.position - playerCamera.transform.position;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToRaptor);
        
        // First check if raptor is in player's view cone
        if (angle <= viewAngle * 0.5f)
        {
            // Then check if there are any obstacles blocking the view
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, directionToRaptor.normalized, 
                out hit, directionToRaptor.magnitude, obstacleLayer))
            {
                // View is blocked by obstacle
                return false;
            }
            return true;
        }
        
        return false;
    }
    
    private void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        
        // Get player's controller component and apply damage
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
        }
        
        lastAttackTime = Time.time;
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }
    
    private void ResetAttack()
    {
        canAttack = true;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw detection ranges
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, noiseDetectionRange);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, scentDetectionRange);
        
        // Draw wind direction
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, windDirection.normalized * 5f);
        
        // Draw freeze distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, freezeDistance);
        
        // Draw view cone
        if (player != null)
        {
            Gizmos.color = Color.green;
            Vector3 forward = transform.forward;
            Vector3 right = Quaternion.Euler(0, viewAngle * 0.5f, 0) * forward;
            Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * forward;
            
            Gizmos.DrawRay(transform.position, right * freezeDistance);
            Gizmos.DrawRay(transform.position, left * freezeDistance);
        }
    }
} 