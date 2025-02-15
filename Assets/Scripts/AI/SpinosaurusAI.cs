using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class SpinosaurusAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 5f;
    public float chaseSpeed = 12f;
    public float detectionRange = 25f;
    public float hearingRange = 30f;
    public float waterDetectionRange = 40f;
    
    [Header("Behavior Settings")]
    public float waterPreference = 0.7f;
    public float territoryRadius = 50f;
    public float attackCooldown = 5f;
    public float roarCooldown = 15f;
    
    [Header("Horror Transform")]
    public GameObject normalModel;
    public GameObject horrorModel;
    public AudioSource roarSound;
    
    private NavMeshAgent agent;
    private Transform player;
    private PlayerMovement playerMovement;
    private SanitySystem playerSanity;
    private Vector3 territoryCenter;
    private Vector3 currentPatrolPoint;
    private bool isInHorrorMode = false;
    private bool canAttack = true;
    private bool canRoar = true;
    private State currentState = State.Patrolling;
    
    private enum State
    {
        Patrolling,
        Chasing,
        Attacking,
        Returning
    }
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerSanity = player.GetComponent<SanitySystem>();
        territoryCenter = transform.position;
        
        // Subscribe to horror events
        DinosaurHorrorEvent.OnHorrorStateChanged += OnHorrorStateChanged;
        
        StartCoroutine(StateMachine());
        StartCoroutine(RoarRoutine());
    }
    
    private void OnDestroy()
    {
        DinosaurHorrorEvent.OnHorrorStateChanged -= OnHorrorStateChanged;
    }
    
    private IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case State.Patrolling:
                    UpdatePatrolling();
                    break;
                case State.Chasing:
                    UpdateChasing();
                    break;
                case State.Attacking:
                    UpdateAttacking();
                    break;
                case State.Returning:
                    UpdateReturning();
                    break;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void UpdatePatrolling()
    {
        if (Vector3.Distance(transform.position, currentPatrolPoint) < 2f)
        {
            currentPatrolPoint = GetPatrolPoint();
        }
        
        agent.speed = patrolSpeed;
        agent.SetDestination(currentPatrolPoint);
        
        // Check for player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionRange && CanSeePlayer())
        {
            currentState = State.Chasing;
            if (canRoar)
            {
                StartCoroutine(Roar());
            }
        }
    }
    
    private void UpdateChasing()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer > detectionRange * 1.5f || !CanSeePlayer())
        {
            currentState = State.Returning;
        }
        else
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            NotifyPlayerOfSight(true);
            
            if (distanceToPlayer < agent.stoppingDistance && canAttack)
            {
                currentState = State.Attacking;
                StartCoroutine(AttackCooldown());
            }
        }
    }
    
    private void UpdateAttacking()
    {
        // Implement attack animation and damage here
        currentState = State.Chasing;
    }
    
    private void UpdateReturning()
    {
        float distanceToTerritory = Vector3.Distance(transform.position, territoryCenter);
        
        if (distanceToTerritory < 5f)
        {
            currentState = State.Patrolling;
            currentPatrolPoint = GetPatrolPoint();
        }
        else
        {
            agent.speed = patrolSpeed;
            agent.SetDestination(territoryCenter);
        }
        
        NotifyPlayerOfSight(false);
    }
    
    private Vector3 GetPatrolPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * territoryRadius;
        randomPoint.y = 0;
        randomPoint += territoryCenter;
        
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, territoryRadius, NavMesh.AllAreas);
        
        // Prefer points near water (you'd need to implement water detection)
        if (IsNearWater(hit.position) && Random.value < waterPreference)
        {
            return hit.position;
        }
        
        return hit.position;
    }
    
    private bool IsNearWater(Vector3 position)
    {
        // Implement water detection here (e.g., using layers or tags)
        return false;
    }
    
    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            return hit.transform == player;
        }
        
        return false;
    }
    
    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    
    private IEnumerator Roar()
    {
        canRoar = false;
        
        if (roarSound != null)
        {
            roarSound.Play();
        }
        
        // Affect player's sanity more during roar
        if (playerSanity != null)
        {
            playerSanity.OnDinosaurSound(true);
        }
        
        yield return new WaitForSeconds(2f); // Roar duration
        
        if (playerSanity != null)
        {
            playerSanity.OnDinosaurSound(false);
        }
        
        yield return new WaitForSeconds(roarCooldown - 2f);
        canRoar = true;
    }
    
    private IEnumerator RoarRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(roarCooldown, roarCooldown * 2));
            
            if (canRoar && currentState != State.Returning)
            {
                StartCoroutine(Roar());
            }
        }
    }
    
    private void NotifyPlayerOfSight(bool canSeePlayer)
    {
        if (playerSanity != null)
        {
            playerSanity.OnDinosaurSighted(canSeePlayer);
        }
    }
    
    private void OnHorrorStateChanged(bool horrorActive)
    {
        isInHorrorMode = horrorActive;
        
        if (normalModel != null && horrorModel != null)
        {
            normalModel.SetActive(!horrorActive);
            horrorModel.SetActive(horrorActive);
        }
        
        // Adjust behavior for horror mode
        if (horrorActive)
        {
            patrolSpeed *= 1.3f;
            chaseSpeed *= 1.4f;
            detectionRange *= 1.5f;
            hearingRange *= 1.5f;
            attackCooldown *= 0.7f;
            roarCooldown *= 0.5f;
        }
        else
        {
            // Reset to original values
            patrolSpeed = 5f;
            chaseSpeed = 12f;
            detectionRange = 25f;
            hearingRange = 30f;
            attackCooldown = 5f;
            roarCooldown = 15f;
        }
    }
} 