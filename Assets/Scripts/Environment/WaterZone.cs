using UnityEngine;

public class WaterZone : MonoBehaviour
{
    [Header("Water Physics")]
    public float buoyancyForce = 15f;
    public float waterDrag = 3f;
    public float waterAngularDrag = 2f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.EnterWater();
            }
            
            // Apply water physics to the player's rigidbody
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.drag = waterDrag;
                rb.angularDrag = waterAngularDrag;
                rb.useGravity = false; // Disable gravity while in water
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply buoyancy force
                float waterLevel = transform.position.y + transform.localScale.y * 0.5f;
                float objectBottom = other.bounds.min.y;
                float submergedDepth = Mathf.Clamp01((waterLevel - objectBottom) / other.bounds.size.y);
                
                Vector3 buoyancyForceVector = Vector3.up * buoyancyForce * submergedDepth;
                rb.AddForce(buoyancyForceVector, ForceMode.Force);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ExitWater();
            }
            
            // Restore normal physics
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.drag = 0f;
                rb.angularDrag = 0.05f;
                rb.useGravity = true;
            }
        }
    }
} 