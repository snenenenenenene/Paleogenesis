using UnityEngine;
using TMPro;
using System.Collections;

public class MissionSystem : MonoBehaviour
{
    [Header("Mission Settings")]
    public Transform startPoint;
    public Transform endPoint;
    public float completionDistance = 3f;
    public string missionDescription = "Reach the evacuation point while avoiding the dinosaurs.";
    
    [Header("UI References")]
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI distanceText;
    public GameObject missionCompleteUI;
    
    [Header("Navigation")]
    public GameObject directionIndicator;
    public float indicatorRotationSpeed = 5f;
    
    private Transform player;
    private bool isMissionComplete = false;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (objectiveText != null)
        {
            objectiveText.text = missionDescription;
        }
        
        if (missionCompleteUI != null)
        {
            missionCompleteUI.SetActive(false);
        }
        
        StartCoroutine(CheckMissionCompletion());
    }
    
    private void Update()
    {
        if (!isMissionComplete)
        {
            UpdateDistanceDisplay();
            UpdateDirectionIndicator();
        }
    }
    
    private void UpdateDistanceDisplay()
    {
        if (distanceText != null && endPoint != null)
        {
            float distance = Vector3.Distance(player.position, endPoint.position);
            distanceText.text = $"Distance to objective: {distance:F1}m";
        }
    }
    
    private void UpdateDirectionIndicator()
    {
        if (directionIndicator != null && endPoint != null)
        {
            Vector3 directionToTarget = endPoint.position - player.position;
            directionToTarget.y = 0;
            
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            directionIndicator.transform.rotation = Quaternion.Slerp(
                directionIndicator.transform.rotation,
                targetRotation,
                indicatorRotationSpeed * Time.deltaTime
            );
        }
    }
    
    private IEnumerator CheckMissionCompletion()
    {
        while (!isMissionComplete)
        {
            if (Vector3.Distance(player.position, endPoint.position) < completionDistance)
            {
                CompleteMission();
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private void CompleteMission()
    {
        isMissionComplete = true;
        
        if (missionCompleteUI != null)
        {
            missionCompleteUI.SetActive(true);
        }
        
        if (objectiveText != null)
        {
            objectiveText.text = "Mission Complete!";
        }
        
        if (directionIndicator != null)
        {
            directionIndicator.SetActive(false);
        }
        
        // You can add additional completion logic here
        // Such as saving progress, unlocking new missions, etc.
    }
    
    public bool IsMissionComplete()
    {
        return isMissionComplete;
    }
    
    public Vector3 GetObjectivePosition()
    {
        return endPoint != null ? endPoint.position : Vector3.zero;
    }
} 