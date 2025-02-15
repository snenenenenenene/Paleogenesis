using UnityEngine;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public float intensity = 2f;
    public float range = 20f;
    public float spotAngle = 60f;
    public Color lightColor = Color.white;
    public Vector3 positionOffset = new Vector3(0.5f, -0.3f, 0.3f); // Bottom right position
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    
    private Light flashlight;
    private bool isOn = false;
    private Transform cameraTransform;
    
    private void Start()
    {
        // Create flashlight object
        GameObject flashlightObj = new GameObject("Flashlight");
        flashlightObj.transform.SetParent(transform);
        
        // Add and setup light component
        flashlight = flashlightObj.AddComponent<Light>();
        flashlight.type = LightType.Spot;
        flashlight.intensity = intensity;
        flashlight.range = range;
        flashlight.spotAngle = spotAngle;
        flashlight.color = lightColor;
        
        // Position the flashlight
        cameraTransform = GetComponentInChildren<Camera>().transform;
        flashlightObj.transform.SetParent(cameraTransform);
        flashlightObj.transform.localPosition = positionOffset;
        flashlightObj.transform.localRotation = Quaternion.Euler(rotationOffset);
        
        // Start with flashlight off
        flashlight.enabled = false;
    }
    
    private void Update()
    {
        // Toggle flashlight with F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
        }
    }
}
