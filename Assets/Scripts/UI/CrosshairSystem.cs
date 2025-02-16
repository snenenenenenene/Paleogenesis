using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CrosshairSystem : MonoBehaviour
{
    private Sprite defaultCrosshair;
    private Sprite interactableCrosshair;
    public float interactionRange = 3f;
    public Vector2 crosshairSize = new Vector2(32, 32);
    public Color crosshairColor = Color.white;
    
    [Header("Layer Settings")]
    public LayerMask interactableLayer;
    
    private Image crosshairImage;
    private Camera mainCamera;
    
    private void Start()
    {
        // Load crosshair sprites
        #if UNITY_EDITOR
        defaultCrosshair = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Icons/Dot.png");
        interactableCrosshair = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Icons/Interactable.png");
        
        if (defaultCrosshair == null || interactableCrosshair == null)
        {
            Debug.LogError("Could not load crosshair sprites. Make sure Dot.png and Interactable.png exist in the Icons folder.");
            return;
        }
        #endif
        
        // Create crosshair UI
        GameObject crosshairObj = new GameObject("Crosshair");
        crosshairObj.transform.SetParent(transform);
        crosshairImage = crosshairObj.AddComponent<Image>();
        
        // Set up the crosshair image
        crosshairImage.sprite = defaultCrosshair;
        crosshairImage.color = crosshairColor;
        
        // Center the crosshair
        RectTransform rectTransform = crosshairImage.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = crosshairSize;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        
        // Get the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }
    
    private void Update()
    {
        if (mainCamera == null || crosshairImage == null || defaultCrosshair == null || interactableCrosshair == null) return;
        
        // Cast a ray from the center of the screen
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // Check if we're looking at an interactable object
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            // Change to interactable crosshair
            crosshairImage.sprite = interactableCrosshair;
        }
        else
        {
            // Change back to default crosshair
            crosshairImage.sprite = defaultCrosshair;
        }
    }
} 