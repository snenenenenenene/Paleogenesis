using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider sanitySlider;
    public Slider staminaSlider;
    public Slider batterySlider;
    public Image sanityFillImage;
    public TextMeshProUGUI radioStatusText;
    
    [Header("Color Settings")]
    public Color normalSanityColor = Color.green;
    public Color lowSanityColor = Color.red;
    public float lowSanityThreshold = 0.3f;
    
    private PlayerMovement playerMovement;
    private SanitySystem sanitySystem;
    private RadioSystem radioSystem;
    
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            sanitySystem = player.GetComponent<SanitySystem>();
            radioSystem = player.GetComponent<RadioSystem>();
        }
        
        // Initialize UI elements
        if (sanitySlider != null)
            sanitySlider.value = 1f;
        if (staminaSlider != null)
            staminaSlider.value = 1f;
        if (batterySlider != null)
            batterySlider.value = 1f;
    }
    
    private void Update()
    {
        UpdateSanityUI();
        UpdateStaminaUI();
        UpdateRadioUI();
    }
    
    private void UpdateSanityUI()
    {
        if (sanitySystem != null && sanitySlider != null)
        {
            float sanityPercentage = sanitySystem.GetSanityPercentage();
            sanitySlider.value = sanityPercentage;
            
            if (sanityFillImage != null)
            {
                sanityFillImage.color = Color.Lerp(lowSanityColor, normalSanityColor, 
                    Mathf.InverseLerp(0, lowSanityThreshold, sanityPercentage));
            }
        }
    }
    
    private void UpdateStaminaUI()
    {
        if (playerMovement != null && staminaSlider != null)
        {
            staminaSlider.value = playerMovement.GetStaminaPercentage();
        }
    }
    
    private void UpdateRadioUI()
    {
        if (radioSystem != null)
        {
            if (batterySlider != null)
            {
                batterySlider.value = radioSystem.GetBatteryPercentage();
            }
            
            if (radioStatusText != null)
            {
                if (radioSystem.IsPlaying)
                {
                    radioStatusText.text = "Radio: ON";
                    radioStatusText.color = Color.green;
                }
                else
                {
                    radioStatusText.text = "Radio: OFF";
                    radioStatusText.color = Color.red;
                }
            }
        }
    }
} 