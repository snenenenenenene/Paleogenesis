using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 3f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    
    [Header("Sound Emission")]
    public float walkingSoundLevel = 1f;
    public float sprintingSoundLevel = 2f;
    public float crouchingSoundLevel = 0.5f;
    
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 10f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isSprinting;
    private float currentStamina;
    private float currentNoiseLevel;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;
    }
    
    private void Update()
    {
        HandleMovement();
        HandleStamina();
        UpdateNoiseLevel();
    }
    
    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Handle movement states
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching && currentStamina > 0;
        
        float currentSpeed = walkSpeed;
        if (isSprinting) currentSpeed = sprintSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // Update character height for crouching
        if (isCrouching)
        {
            controller.height = 1f;
            controller.center = new Vector3(0, 0.5f, 0);
        }
        else
        {
            controller.height = 2f;
            controller.center = new Vector3(0, 1f, 0);
        }
    }
    
    private void HandleStamina()
    {
        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }
        
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }
    
    private void UpdateNoiseLevel()
    {
        if (controller.velocity.magnitude > 0.1f)
        {
            if (isSprinting)
                currentNoiseLevel = sprintingSoundLevel;
            else if (isCrouching)
                currentNoiseLevel = crouchingSoundLevel;
            else
                currentNoiseLevel = walkingSoundLevel;
        }
        else
        {
            currentNoiseLevel = 0f;
        }
    }
    
    public float GetCurrentNoiseLevel()
    {
        return currentNoiseLevel;
    }
    
    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }
}
