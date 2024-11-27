using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttributes", menuName = "ScriptableObjects/PlayerAttributes", order = 1)]
public class PlayerAttributes : ScriptableObject
{
    [Header("Health and Stamina")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaDrainRate = .1f;

    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;

    [Header("Jump Settings")]
    public float jumpForce = .7f; // Add jumpForce to configure jumping power
}
