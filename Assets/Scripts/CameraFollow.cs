using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // The player or object to follow
    public Vector3 offset = new Vector3(0, 3, -6); // Camera offset position
    public float followSpeed = 5f;    // Speed of camera following the target

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow target not set. Please assign the player.");
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            transform.LookAt(target); // Ensures the camera always looks at the player
        }
    }
}
