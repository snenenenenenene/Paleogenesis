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
    public float groundDistance = 0.1f; // Keep it small
    public LayerMask groundLayer;

    [Header("References")]
    public Transform playerBody;

    private float sprintDuration = 3f;
    private float cooldownDuration = 5f;
    private float staminaDrainRate = 33.3f;
    private float staminaRechargeRate = 6.67f;
    private float jumpForce = 7f;

    private float sprintTimer = 0f;
    private float cooldownTimer = 0f;
    private bool canSprint = true;

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
        CheckGroundStatus();
    }

    public void ApplyMovement(Vector3 moveDirection, float speed)
    {
        Vector3 horizontalMove = moveDirection * speed;
        Vector3 velocity = rb.velocity;
        velocity.x = horizontalMove.x;
        velocity.z = horizontalMove.z;
        velocity.y = rb.velocity.y; // Preserve vertical velocity
        rb.velocity = velocity;
    }

    public void Jump()
    {
        if (isGrounded && HasStamina())
        {
            Vector3 jumpVelocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            rb.velocity = jumpVelocity;
            playerAttributes.currentStamina -= 10f; // Example stamina cost
            Debug.Log($"Jumping. Stamina: {playerAttributes.currentStamina}");
        }
    }

    private void CheckGroundStatus()
    {
        Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundDistance, Color.red); // Visualize ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public bool HasStamina(float amount = 0)
    {
        return playerAttributes.currentStamina > amount;
    }
}
