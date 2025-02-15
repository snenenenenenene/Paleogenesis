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
    public float swimSpeed = 4f;
    public float swimUpForce = 5f;
    public float underwaterDrag = 3f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public bool invertY = false;
    public float minVerticalAngle = -89f;
    public float maxVerticalAngle = 89f;
    
    [Header("Sound Emission")]
    public float walkingSoundLevel = 1f;
    public float sprintingSoundLevel = 2f;
    public float crouchingSoundLevel = 0.5f;
    public float swimmingSoundLevel = 0.8f;
    
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 10f;
    public float underwaterStaminaDrainRate = 15f;
    
    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isSprinting;
    private bool isInWater;
    private bool isUnderwater;
    private float currentStamina;
    private float currentNoiseLevel;
    private float verticalRotation = 0f;
    private float waterSurfaceHeight;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        currentStamina = maxStamina;
        
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        HandleMouseLook();
        
        if (isInWater)
        {
            HandleSwimming();
        }
        else
        {
            HandleMovement();
        }
        
        HandleStamina();
        UpdateNoiseLevel();
        
        // Allow cursor unlock with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    private void HandleMouseLook()
    {
        if (playerCamera == null) return;
        
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Rotate the player (horizontal rotation)
        transform.Rotate(Vector3.up * mouseX);
        
        // Rotate the camera (vertical rotation)
        verticalRotation += invertY ? mouseY : -mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
    
    private void HandleSwimming()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Vertical swimming movement
        if (Input.GetKey(KeyCode.Space)) // Swim up
        {
            velocity.y = swimUpForce;
        }
        else if (Input.GetKey(KeyCode.LeftControl)) // Swim down
        {
            velocity.y = -swimUpForce;
        }
        else
        {
            velocity.y = 0f;
        }
        
        // Apply swimming movement
        controller.Move(move * swimSpeed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
        
        // Check if underwater
        isUnderwater = transform.position.y + controller.height * 0.5f < waterSurfaceHeight;
        
        // Drain stamina while underwater
        if (isUnderwater)
        {
            currentStamina -= underwaterStaminaDrainRate * Time.deltaTime;
            
            // Force player to surface if out of stamina
            if (currentStamina <= 0)
            {
                velocity.y = swimUpForce;
            }
        }
    }
    
    public void EnterWater()
    {
        isInWater = true;
        velocity.y = 0f; // Reset vertical velocity
        
        // Store water surface height
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 10f, Vector3.down, out hit, 20f))
        {
            if (hit.collider.CompareTag("Water"))
            {
                waterSurfaceHeight = hit.point.y;
            }
        }
    }
    
    public void ExitWater()
    {
        isInWater = false;
        isUnderwater = false;
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
        if (isSprinting || isUnderwater)
        {
            float drainRate = isUnderwater ? underwaterStaminaDrainRate : staminaDrainRate;
            currentStamina -= drainRate * Time.deltaTime;
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
            if (isInWater)
                currentNoiseLevel = swimmingSoundLevel;
            else if (isSprinting)
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
    
    public bool IsUnderwater()
    {
        return isUnderwater;
    }
}
