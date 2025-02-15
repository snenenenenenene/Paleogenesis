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
    
    [Header("Attack Settings")]
    public float attackDamage = 25f;
    public float attackCooldown = 1.5f;
    
    private NavMeshAgent agent;
    private Transform player;
    private Camera playerCamera;
    private bool isPlayerLooking;
    private bool canAttack = true;
    private float lastAttackTime;
    
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
        
        // Handle movement based on player's view and distance
        if (isPlayerLooking)
        {
            // Freeze when player is looking
            agent.isStopped = true;
            
            // Notify the SanitySystem that player is seeing a dinosaur
            SanitySystem sanitySystem = player.GetComponent<SanitySystem>();
            if (sanitySystem != null)
            {
                sanitySystem.OnDinosaurSighted(true);
            }
        }
        else
        {
            // Move when player isn't looking
            agent.isStopped = false;
            agent.SetDestination(player.position);
            
            // Adjust speed based on distance
            agent.speed = (distanceToPlayer <= attackRange * 1.5f) ? attackingSpeed : stalkingSpeed;
            
            // Attack if in range
            if (distanceToPlayer <= attackRange && canAttack)
            {
                Attack();
            }
            
            // Reset sanity effect when not looking
            SanitySystem sanitySystem = player.GetComponent<SanitySystem>();
            if (sanitySystem != null)
            {
                sanitySystem.OnDinosaurSighted(false);
            }
        }
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
        
        // Draw freeze distance
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, freezeDistance);
        
        // Draw view cone
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 forward = transform.forward;
            Vector3 right = Quaternion.Euler(0, viewAngle * 0.5f, 0) * forward;
            Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * forward;
            
            Gizmos.DrawRay(transform.position, right * freezeDistance);
            Gizmos.DrawRay(transform.position, left * freezeDistance);
        }
    }
} 