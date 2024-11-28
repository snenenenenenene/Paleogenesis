using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Attributes")]
    public PlayerAttributes playerAttributes;

    private Rigidbody rb;
    private bool isRunning = false;
    private bool isCrouching = false;
    private bool isGrounded;
    private Vector3 moveDirection;
    private float currentSpeed;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("References")]
    public Transform playerBody;

    private float sprintDuration = 3f;          // Max time the player can sprint
    private float cooldownDuration = 5f;        // Cooldown time before sprinting is allowed again
    private float staminaDrainRate = 33.3f;     // Stamina drains fully in 3 seconds (100 / 3)
    private float staminaRechargeRate = 6.67f;  // Stamina recharges fully in 15 seconds (100 / 15)
    private float jumpStaminaCost = 10f;        // Stamina cost per jump

    private float sprintTimer = 0f;             // Tracks time spent sprinting
    private float cooldownTimer = 0f;           // Tracks cooldown time after sprinting
    private bool canSprint = true;              // Controls whether sprinting is allowed

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerAttributes.currentHealth = playerAttributes.maxHealth;
        playerAttributes.currentStamina = playerAttributes.maxStamina;

        Debug.Log($"Starting Health: {playerAttributes.currentHealth} / {playerAttributes.maxHealth}");
        Debug.Log($"Starting Stamina: {playerAttributes.currentStamina} / {playerAttributes.maxStamina}");
    }

    void Update()
    {
        HandleInput();
        CheckGroundStatus();
        HandleStamina();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        moveDirection = (playerBody.right * moveHorizontal + playerBody.forward * moveVertical).normalized;

        // Sprinting logic with duration limit and cooldown check
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && HasStamina() && sprintTimer < sprintDuration && canSprint)
        {
            isRunning = true;
            currentSpeed = playerAttributes.runSpeed;
            sprintTimer += Time.deltaTime;
            cooldownTimer = 0f;  // Reset cooldown timer while sprinting
        }
        else
        {
            isRunning = false;
            sprintTimer = 0f;  // Reset sprint timer when not sprinting
            currentSpeed = isCrouching ? playerAttributes.crouchSpeed : playerAttributes.walkSpeed;

            // Start cooldown if the player has stopped sprinting
            if (!canSprint)
            {
                cooldownTimer += Time.deltaTime;

                if (cooldownTimer >= cooldownDuration)
                {
                    canSprint = true;
                    cooldownTimer = 0f;  // Reset cooldown timer
                    Debug.Log("Sprinting is now available again.");
                }
            }
        }

        // Crouching logic
        if (Input.GetKey(KeyCode.C))
        {
            isCrouching = true;
            currentSpeed = playerAttributes.crouchSpeed;
        }
        else
        {
            isCrouching = false;
        }

        // Jumping with stamina cost
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && HasStamina(jumpStaminaCost))
        {
            Jump();
        }
    }

    private void ApplyMovement()
    {
        Vector3 horizontalMove = moveDirection * currentSpeed;
        rb.velocity = new Vector3(horizontalMove.x, rb.velocity.y, horizontalMove.z);
    }

    private void Jump()
    {
        if (isGrounded && HasStamina(jumpStaminaCost))
        {
            rb.velocity = new Vector3(rb.velocity.x, playerAttributes.jumpForce, rb.velocity.z);
            playerAttributes.currentStamina -= jumpStaminaCost;
            Debug.Log($"Jumping. Stamina: {playerAttributes.currentStamina}");
        }
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void HandleStamina()
    {
        Debug.Log($"isRunning: {isRunning}, Current Stamina: {playerAttributes.currentStamina}");

        // Handle stamina drain while sprinting
        if (isRunning)
        {
            playerAttributes.currentStamina -= staminaDrainRate * Time.deltaTime;
            playerAttributes.currentStamina = Mathf.Max(playerAttributes.currentStamina, 0);

            if (playerAttributes.currentStamina <= 0)
            {
                Debug.Log("Stamina depleted!");
                isRunning = false;
                canSprint = false;  // Start cooldown when stamina is fully depleted
            }
        }
        // Allow stamina to recharge during the cooldown
        else if (playerAttributes.currentStamina < playerAttributes.maxStamina)
        {
            playerAttributes.currentStamina += staminaRechargeRate * Time.deltaTime;
            playerAttributes.currentStamina = Mathf.Min(playerAttributes.currentStamina, playerAttributes.maxStamina);

            if (playerAttributes.currentStamina == playerAttributes.maxStamina)
            {
                Debug.Log("Stamina fully regenerated.");
            }
        }

        Debug.Log($"Current Stamina after HandleStamina: {playerAttributes.currentStamina} / {playerAttributes.maxStamina}");
    }

    public bool HasStamina(float amount = 0)
    {
        return playerAttributes.currentStamina > amount;
    }

    public void TakeDamage(float amount)
    {
        playerAttributes.currentHealth -= amount;
        playerAttributes.currentHealth = Mathf.Max(playerAttributes.currentHealth, 0);

        Debug.Log($"Health: {playerAttributes.currentHealth} / {playerAttributes.maxHealth}");
        if (playerAttributes.currentHealth <= 0)
        {
            Debug.Log("Player is dead.");
        }
        else if (playerAttributes.currentHealth < playerAttributes.maxHealth * 0.2f)
        {
            Debug.Log("Warning: Health is critically low!");
        }
    }

    public void Move(Vector3 direction, float speed)
    {
        Vector3 movement = direction * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }
}
