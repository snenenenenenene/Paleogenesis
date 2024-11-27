using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Adjust for sensitivity preference
    public Transform playerBody; // Reference to the player's main body

    private float xRotation = 0f;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player body horizontally based on the X-axis mouse movement
        playerBody.Rotate(Vector3.up * mouseX);

        // Adjust the camera's vertical rotation based on Y-axis mouse movement
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit vertical look angle to prevent flipping
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
