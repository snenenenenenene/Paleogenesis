using UnityEngine;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlight; // Reference to the flashlight Light component
    public KeyCode toggleKey = KeyCode.F; // Key to toggle the flashlight
    public Transform cameraTransform; // Reference to the player's camera
    public Vector3 flashlightOffset = new Vector3(0f, -0.2f, 0.5f); // Offset from the camera position
    public float followSpeed = 10f; // Speed at which the flashlight follows the camera
    public float bounceIntensity = 0.1f; // Intensity of the flashlight bounce effect
    public float bounceSpeed = 2f; // Speed of the bounce effect

    [Header("Battery Settings (Optional)")]
    public bool useBattery = false; // Set to true to enable battery life
    public float maxBatteryLife = 100f; // Maximum battery life
    public float batteryDrainRate = 10f; // Battery drain per second when the flashlight is on
    private float currentBatteryLife;

    private bool isFlashlightOn;
    private Vector3 targetPosition;

    void Start()
    {
        // Initialize battery life and flashlight state
        currentBatteryLife = maxBatteryLife;

        if (flashlight == null)
        {
            Debug.LogError("Flashlight component is not assigned! Please assign a Light component.");
            return;
        }

        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform is not assigned! Please assign the player's camera.");
            return;
        }

        flashlight.enabled = false; // Ensure the flashlight starts off
    }

    void Update()
    {
        HandleFlashlightToggle();

        if (isFlashlightOn)
        {
            SmoothFollowCamera();

            if (useBattery)
            {
                HandleBatteryDrain();
            }
        }
    }

    private void HandleFlashlightToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isFlashlightOn = !isFlashlightOn;
            flashlight.enabled = isFlashlightOn;

            if (useBattery && currentBatteryLife <= 0f)
            {
                flashlight.enabled = false; // Automatically turn off flashlight if battery is depleted
                Debug.Log("Flashlight battery is depleted!");
                isFlashlightOn = false;
            }
        }
    }

    private void HandleBatteryDrain()
    {
        if (isFlashlightOn && currentBatteryLife > 0f)
        {
            currentBatteryLife -= batteryDrainRate * Time.deltaTime;
            currentBatteryLife = Mathf.Max(currentBatteryLife, 0f);

            if (currentBatteryLife <= 0f)
            {
                Debug.Log("Flashlight battery is depleted!");
                flashlight.enabled = false;
                isFlashlightOn = false;
            }
        }
    }

    public void RechargeBattery(float amount)
    {
        currentBatteryLife += amount;
        currentBatteryLife = Mathf.Min(currentBatteryLife, maxBatteryLife);
        Debug.Log($"Battery recharged. Current battery life: {currentBatteryLife}");
    }

    public float GetBatteryPercentage()
    {
        return (currentBatteryLife / maxBatteryLife) * 100f;
    }

    private void SmoothFollowCamera()
    {
        // Calculate the target position slightly offset from the camera
        targetPosition = cameraTransform.position + cameraTransform.rotation * flashlightOffset;

        // Smoothly move the flashlight to the target position
        flashlight.transform.position = Vector3.Lerp(flashlight.transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate the flashlight to match the camera's rotation
        flashlight.transform.rotation = Quaternion.Slerp(flashlight.transform.rotation, cameraTransform.rotation, followSpeed * Time.deltaTime);

        // Add a slight bounce effect to simulate handheld movement
        Vector3 bounce = flashlight.transform.position;
        bounce.y += Mathf.Sin(Time.time * bounceSpeed) * bounceIntensity;
        flashlight.transform.position = Vector3.Lerp(flashlight.transform.position, bounce, followSpeed * Time.deltaTime);
    }
}
