using UnityEngine;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(FootstepSystem))]
public class FootstepLoader : MonoBehaviour
{
    private void Start()
    {
        var footstepSystem = GetComponent<FootstepSystem>();
        if (footstepSystem == null) return;

        // Make sure we have an AudioSource
        if (footstepSystem.footstepSource == null)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 1f;
            source.playOnAwake = false;
            source.volume = 1f;
            footstepSystem.footstepSource = source;
        }

        // Load rock walking sounds
        #if UNITY_EDITOR
        var soundSets = new List<FootstepSystem.FootstepSoundSet>();
        string walkSoundsPath = "Assets/Audio/Footsteps/Footsteps_Rock/Footsteps_Rock_Walk";
        
        if (Directory.Exists(walkSoundsPath))
        {
            var clips = new List<AudioClip>();
            string[] audioFiles = Directory.GetFiles(walkSoundsPath, "*.wav");
            
            foreach (string audioFile in audioFiles)
            {
                string unityPath = audioFile.Replace("\\", "/");
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(unityPath);
                if (clip != null)
                {
                    clips.Add(clip);
                    Debug.Log($"Loaded footstep sound: {clip.name}");
                }
            }
            
            if (clips.Count > 0)
            {
                soundSets.Add(new FootstepSystem.FootstepSoundSet 
                { 
                    surfaceTag = "Rock",
                    clips = clips.ToArray()
                });
                Debug.Log($"Added {clips.Count} rock footstep sounds");
            }
        }
        
        footstepSystem.footstepSounds = soundSets.ToArray();
        #endif

        // Set default values
        footstepSystem.minTimeBetweenSteps = 0.3f;
        footstepSystem.sprintStepMultiplier = 0.7f;
        footstepSystem.crouchStepMultiplier = 1.5f;
        footstepSystem.baseVolume = 1f;
        footstepSystem.volumeVariation = 0.1f;
        footstepSystem.pitchVariation = 0.1f;
    }
} 