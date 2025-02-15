using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class SpinosaurusAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 15f;
    public float roamRadius = 50f;
    public float minRoamWaitTime = 3f;
    public float maxRoamWaitTime = 8f;
    
    [Header("Detection Settings")]
    public float detectionRange = 30f;
    public float attackRange = 4f;
    public float hearingRange = 15f;
    public LayerMask detectionMask;
    
    [Header("Attack Settings")]
    public float attackDamage = 40f;
    public float attackCooldown = 2f;
    
    private NavMeshAgent agent;
    private Transform player;
    private Vector3 roamCenter;
    private bool isHunting = false;
    private bool canAttack = true;
    private float lastAttackTime;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        roamCenter = transform.position;
        
        if (player == null)
        {
            Debug.LogError("SpinosaurusAI: Player not found!");
            enabled = false;
            return;
        }
        
        // Set initial agent settings
        agent.speed = walkSpeed;
        agent.stoppingDistance = attackRange * 0.8f;
        
        // Start roaming behavior
        StartCoroutine(RoamingBehavior());
    }
    
    private void Update()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check if player is in detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Check if there's a clear line of sight
            if (HasLineOfSightToPlayer())
            {
                isHunting = true;
                Hunt();
            }
        }
        
        // Check if player is making noise
        if (!isHunting && distanceToPlayer <= hearingRange)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.GetCurrentNoiseLevel() > 0.5f)
            {
                isHunting = true;
                Hunt();
            }
        }
        
        // Attack if in range
        if (distanceToPlayer <= attackRange && canAttack)
        {
            Attack();
        }
    }
    
    private IEnumerator RoamingBehavior()
    {
        while (true)
        {
            if (!isHunting)
            {
                // Find a random point within roaming radius
                Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
                randomDirection += roamCenter;
                NavMeshHit hit;
                
                // Find nearest valid position on NavMesh
                if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
                {
                    agent.speed = walkSpeed;
                    agent.SetDestination(hit.position);
                }
                
                // Wait at destination
                float waitTime = Random.Range(minRoamWaitTime, maxRoamWaitTime);
                yield return new WaitForSeconds(waitTime);
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
    
    private void Hunt()
    {
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
    }
    
    private bool HasLineOfSightToPlayer()
    {
        if (player == null) return false;
        
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange, detectionMask))
        {
            return hit.transform.CompareTag("Player");
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
        // Draw roaming radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(roamCenter, roamRadius);
        
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw hearing range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
} 