using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class UtahraptorAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float stalkingSpeed = 3f;
    public float chaseSpeed = 8f;
    public float stealthSpeed = 2f;
    public float detectionRange = 15f;
    public float hearingRange = 20f;
    public float minStalkDistance = 8f;
    public float maxStalkDistance = 12f;
    
    [Header("Stealth Settings")]
    public float stealthThreshold = 0.3f;
    public float ambushProbability = 0.3f;
    public float ambushCooldown = 15f;
    
    [Header("Horror Transform")]
    public GameObject normalModel;
    public GameObject horrorModel;
    
    private NavMeshAgent agent;
    private Transform player;
    private PlayerMovement playerMovement;
    private SanitySystem playerSanity;
    private Vector3 lastKnownPosition;
    private bool isInHorrorMode = false;
    private bool canAmbush = true;
    private State currentState = State.Stalking;
    
    private enum State
    {
        Stalking,
        Chasing,
        Ambushing,
        Searching
    }
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerSanity = player.GetComponent<SanitySystem>();
        
        // Subscribe to horror events
        DinosaurHorrorEvent.OnHorrorStateChanged += OnHorrorStateChanged;
        
        StartCoroutine(StateMachine());
        StartCoroutine(SoundCheck());
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
                case State.Stalking:
                    UpdateStalking();
                    break;
                case State.Chasing:
                    UpdateChasing();
                    break;
                case State.Ambushing:
                    UpdateAmbushing();
                    break;
                case State.Searching:
                    UpdateSearching();
                    break;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void UpdateStalking()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer < detectionRange && CanSeePlayer())
        {
            if (Random.value < ambushProbability && canAmbush)
            {
                currentState = State.Ambushing;
                StartCoroutine(AmbushCooldown());
            }
            else
            {
                currentState = State.Chasing;
            }
        }
        else
        {
            Vector3 stalkPosition = GetStalkPosition();
            agent.speed = stalkingSpeed;
            agent.SetDestination(stalkPosition);
        }
    }
    
    private void UpdateChasing()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer > detectionRange || !CanSeePlayer())
        {
            lastKnownPosition = player.position;
            currentState = State.Searching;
        }
        else
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            NotifyPlayerOfSight(true);
        }
    }
    
    private void UpdateAmbushing()
    {
        agent.speed = 0;
        
        // Wait for player to get closer or lose sight
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionRange * 0.5f)
        {
            currentState = State.Chasing;
        }
        else if (!CanSeePlayer())
        {
            currentState = State.Stalking;
        }
    }
    
    private void UpdateSearching()
    {
        float distanceToLastKnown = Vector3.Distance(transform.position, lastKnownPosition);
        
        if (distanceToLastKnown < 2f)
        {
            currentState = State.Stalking;
        }
        else
        {
            agent.speed = stealthSpeed;
            agent.SetDestination(lastKnownPosition);
        }
    }
    
    private Vector3 GetStalkPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * maxStalkDistance;
        randomDirection += player.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, maxStalkDistance, NavMesh.AllAreas);
        
        return hit.position;
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
    
    private IEnumerator SoundCheck()
    {
        while (true)
        {
            float playerNoise = playerMovement.GetCurrentNoiseLevel();
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            if (distanceToPlayer < hearingRange * playerNoise)
            {
                lastKnownPosition = player.position;
                if (currentState != State.Chasing && currentState != State.Ambushing)
                {
                    currentState = State.Searching;
                }
                NotifyPlayerOfSound(true);
            }
            else
            {
                NotifyPlayerOfSound(false);
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private IEnumerator AmbushCooldown()
    {
        canAmbush = false;
        yield return new WaitForSeconds(ambushCooldown);
        canAmbush = true;
    }
    
    private void NotifyPlayerOfSight(bool canSeePlayer)
    {
        if (playerSanity != null)
        {
            playerSanity.OnDinosaurSighted(canSeePlayer);
        }
    }
    
    private void NotifyPlayerOfSound(bool canHearDinosaur)
    {
        if (playerSanity != null)
        {
            playerSanity.OnDinosaurSound(canHearDinosaur);
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
            stalkingSpeed *= 1.5f;
            chaseSpeed *= 1.2f;
            detectionRange *= 1.3f;
            hearingRange *= 1.3f;
            ambushProbability *= 1.5f;
        }
        else
        {
            // Reset to original values
            stalkingSpeed = 3f;
            chaseSpeed = 8f;
            detectionRange = 15f;
            hearingRange = 20f;
            ambushProbability = 0.3f;
        }
    }
} 