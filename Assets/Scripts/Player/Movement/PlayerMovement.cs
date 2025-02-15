using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 3f;
    public float jumpForce = 2f;
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
    
    [Header("Head Bob Settings")]
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.1f;
    public float sprintBobMultiplier = 1.5f;
    public float crouchBobMultiplier = 0.5f;
    
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 15f;
    public float staminaRegenRate = 8f;
    public float staminaRegenDelay = 1f;
    public float underwaterStaminaDrainRate = 15f;
    
    private CharacterController controller;
    private Camera playerCamera;
    private Transform cameraHolder;
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
    private float defaultCameraY;
    private float bobTimer;
    private float lastStaminaUseTime;
    private float defaultCameraHolderY;
    private float targetCameraY;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        cameraHolder = playerCamera.transform.parent;
        currentStamina = maxStamina;
        defaultCameraHolderY = cameraHolder.localPosition.y;
        targetCameraY = defaultCameraHolderY;
        
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
        HandleHeadBob();
        
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
    
    private void HandleHeadBob()
    {
        if (!controller.isGrounded || isInWater) return;

        Vector3 localMove = transform.InverseTransformDirection(controller.velocity);
        float speedFactor = Mathf.Clamp01(localMove.magnitude / walkSpeed);
        
        if (isSprinting) speedFactor *= sprintBobMultiplier;
        if (isCrouching) speedFactor *= crouchBobMultiplier;
        
        if (speedFactor > 0.01f)
        {
            bobTimer += Time.deltaTime * bobFrequency * speedFactor;
            float bobAmount = Mathf.Sin(bobTimer) * bobAmplitude * speedFactor;
            
            // Apply bob to camera holder position
            Vector3 camPos = cameraHolder.localPosition;
            camPos.y = targetCameraY + bobAmount;
            cameraHolder.localPosition = camPos;
        }
        else
        {
            // Reset camera position when not moving
            bobTimer = 0;
            Vector3 camPos = cameraHolder.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * 5f);
            cameraHolder.localPosition = camPos;
        }
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
        isCrouching = Input.GetKey(KeyCode.C);
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching && currentStamina > 0;
        
        float currentSpeed = walkSpeed;
        if (isSprinting) 
        {
            currentSpeed = sprintSpeed;
            lastStaminaUseTime = Time.time;
        }
        if (isCrouching) currentSpeed = crouchSpeed;
        
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        // Handle jumping with reduced height
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            lastStaminaUseTime = Time.time;
            currentStamina -= 10f; // Jump stamina cost
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // Update character height and camera position for crouching
        float targetHeight = isCrouching ? 1f : 2f;
        float targetCenter = isCrouching ? 0.5f : 1f;
        targetCameraY = isCrouching ? defaultCameraHolderY * 0.5f : defaultCameraHolderY;
        
        // Smoothly adjust controller height and center
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 10f);
        controller.center = Vector3.Lerp(controller.center, new Vector3(0, targetCenter, 0), Time.deltaTime * 10f);
    }
    
    private void HandleStamina()
    {
        if (isSprinting || isUnderwater)
        {
            float drainRate = isUnderwater ? underwaterStaminaDrainRate : staminaDrainRate;
            currentStamina -= drainRate * Time.deltaTime;
            lastStaminaUseTime = Time.time;
        }
        else if (Time.time - lastStaminaUseTime >= staminaRegenDelay)
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
