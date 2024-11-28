using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;
    private Rigidbody playerRigidbody;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Ensure the playerBody has a Rigidbody
        playerRigidbody = playerBody.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.freezeRotation = true; // Prevent physics-driven rotation
        }
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculate vertical rotation for the camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Calculate horizontal rotation for the player body
        if (playerRigidbody != null)
        {
            // Use Rigidbody to rotate the player body for smooth physics handling
            Quaternion deltaRotation = Quaternion.Euler(0f, mouseX, 0f);
            playerRigidbody.MoveRotation(playerRigidbody.rotation * deltaRotation);
        }
        else
        {
            // Fallback if no Rigidbody is present
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
