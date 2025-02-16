using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class CitySceneGenerator : EditorWindow
{
    private GameObject playerPrefab;
    private GameObject utahraptorPrefab;
    private GameObject spinosaurusPrefab;
    private GameObject[] buildingPrefabs;
    private Material defaultMaterial;
    private AudioClip[] musicTracks;
    private bool showPrefabs = true;
    private bool showAudio = true;
    private Vector2 scrollPosition;
    
    [MenuItem("ORNITHO/Generate Horror City Scene")]
    public static void ShowWindow()
    {
        GetWindow<CitySceneGenerator>("City Scene Generator");
    }
    
    private void CreateDefaultPrefabs()
    {
        // Create Player Prefab
        if (playerPrefab == null)
        {
            GameObject player = new GameObject("PlayerPrefab");
            player.tag = "Player";
            player.AddComponent<CharacterController>();
            player.AddComponent<PlayerMovement>();
            player.AddComponent<SanitySystem>();
            player.AddComponent<RadioSystem>();
            player.AddComponent<FlashlightSystem>();
            
            GameObject playerModel = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerModel.transform.SetParent(player.transform);
            playerModel.transform.localPosition = Vector3.zero;
            playerModel.transform.localScale = new Vector3(1f, 1f, 1f);
            
            // Add material to player model
            Shader standardShader = Shader.Find("Universal Render Pipeline/Lit");
            if (standardShader == null) standardShader = Shader.Find("Standard");
            if (standardShader != null)
            {
                Material playerMat = new Material(standardShader);
                playerMat.color = new Color(0.2f, 0.6f, 1f); // Blue color
                playerModel.GetComponent<Renderer>().material = playerMat;
            }
            
            GameObject camera = new GameObject("MainCamera");
            camera.AddComponent<Camera>();
            camera.AddComponent<AudioListener>();
            camera.transform.SetParent(player.transform);
            camera.transform.localPosition = new Vector3(0, 1.6f, 0);
            
            playerPrefab = player;
        }
        
        // Create Utahraptor Prefab
        if (utahraptorPrefab == null)
        {
            GameObject utahraptor = new GameObject("UtahraptorPrefab");
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(utahraptor.transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(1f, 1.2f, 2f);
            body.transform.localRotation = Quaternion.Euler(90, 0, 0);
            
            // Add material to utahraptor
            Shader standardShader = Shader.Find("Universal Render Pipeline/Lit");
            if (standardShader == null) standardShader = Shader.Find("Standard");
            if (standardShader != null)
            {
                Material raptorMat = new Material(standardShader);
                raptorMat.color = new Color(0.8f, 0.2f, 0.2f); // Red color
                body.GetComponent<Renderer>().material = raptorMat;
            }
            
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            head.transform.SetParent(utahraptor.transform);
            head.transform.localPosition = new Vector3(0, 0.5f, 1f);
            head.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
            if (standardShader != null) head.GetComponent<Renderer>().material = body.GetComponent<Renderer>().material;
            
            utahraptor.AddComponent<NavMeshAgent>();
            utahraptor.AddComponent<UtahraptorAI>();
            
            utahraptorPrefab = utahraptor;
        }
        
        // Create Spinosaurus Prefab
        if (spinosaurusPrefab == null)
        {
            GameObject spinosaurus = new GameObject("SpinosaurusPrefab");
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.transform.SetParent(spinosaurus.transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(1.5f, 2f, 3f);
            body.transform.localRotation = Quaternion.Euler(90, 0, 0);
            
            // Add material to spinosaurus
            Shader standardShader = Shader.Find("Universal Render Pipeline/Lit");
            if (standardShader == null) standardShader = Shader.Find("Standard");
            if (standardShader != null)
            {
                Material spinoMat = new Material(standardShader);
                spinoMat.color = new Color(0.8f, 0.4f, 0.1f); // Orange color
                body.GetComponent<Renderer>().material = spinoMat;
            }
            
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            head.transform.SetParent(spinosaurus.transform);
            head.transform.localPosition = new Vector3(0, 1f, 1.5f);
            head.transform.localScale = new Vector3(0.8f, 1.2f, 1f);
            if (standardShader != null) head.GetComponent<Renderer>().material = body.GetComponent<Renderer>().material;
            
            GameObject sail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sail.transform.SetParent(spinosaurus.transform);
            sail.transform.localPosition = new Vector3(0, 2f, 0);
            sail.transform.localScale = new Vector3(0.2f, 2f, 3f);
            if (standardShader != null) sail.GetComponent<Renderer>().material = body.GetComponent<Renderer>().material;
            
            spinosaurus.AddComponent<NavMeshAgent>();
            spinosaurus.AddComponent<SpinosaurusAI>();
            
            spinosaurusPrefab = spinosaurus;
        }
    }
    
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Label("Horror City Scene Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // Create default prefabs if they don't exist
        if (GUILayout.Button("Create Default Prefabs"))
        {
            CreateDefaultPrefabs();
        }
        
        EditorGUILayout.Space();
        showPrefabs = EditorGUILayout.Foldout(showPrefabs, "Prefabs");
        if (showPrefabs)
        {
            EditorGUI.indentLevel++;
            playerPrefab = EditorGUILayout.ObjectField("Player Prefab", playerPrefab, typeof(GameObject), true) as GameObject;
            utahraptorPrefab = EditorGUILayout.ObjectField("Utahraptor Prefab", utahraptorPrefab, typeof(GameObject), true) as GameObject;
            spinosaurusPrefab = EditorGUILayout.ObjectField("Spinosaurus Prefab", spinosaurusPrefab, typeof(GameObject), true) as GameObject;
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        showAudio = EditorGUILayout.Foldout(showAudio, "Audio");
        if (showAudio)
        {
            EditorGUI.indentLevel++;
            
            // Handle the music tracks array manually
            if (musicTracks == null)
            {
                musicTracks = new AudioClip[0];
            }
            
            int newSize = EditorGUILayout.IntField("Size", musicTracks.Length);
            if (newSize != musicTracks.Length)
            {
                AudioClip[] newArray = new AudioClip[newSize];
                System.Array.Copy(musicTracks, newArray, Mathf.Min(musicTracks.Length, newSize));
                musicTracks = newArray;
            }
            
            for (int i = 0; i < musicTracks.Length; i++)
            {
                musicTracks[i] = EditorGUILayout.ObjectField($"Track {i}", musicTracks[i], typeof(AudioClip), false) as AudioClip;
            }
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Generate Scene", GUILayout.Width(200), GUILayout.Height(30)))
        {
            GenerateScene();
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        EditorGUILayout.EndScrollView();
    }
    
    private void GenerateScene()
    {
        // Create a new scene
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // Create city environment
        CreateCityEnvironment();
        
        // Create player
        GameObject player = CreatePlayer();
        
        // Create dinosaurs
        CreateDinosaurs();
        
        // Setup lighting
        SetupLighting();
        
        // Setup mission
        SetupMission(player);
        
        // Setup NavMesh
        GenerateNavMesh();
        
        Debug.Log("Scene generated successfully!");
    }
    
    private void CreateCityEnvironment()
    {
        // Create ground - make it much larger than the city
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.localScale = new Vector3(100, 1, 100); // Increased from 20 to 100
        ground.name = "Ground";
        
        // Set the ground tag to Ground (will use Rock sounds by default)
        ground.tag = "Ground";
        Debug.Log($"Ground object created with tag: {ground.tag}");
        
        // Create water area
        CreateWaterArea();
        
        // Create city blocks
        CreateCityBlocks();
        
        // Create streets
        CreateStreets();
        
        // Add props and details
        CreateCityProps();
    }
    
    private void CreateWaterArea()
    {
        // Create a large water area outside the city
        GameObject waterArea = GameObject.CreatePrimitive(PrimitiveType.Cube);
        waterArea.name = "WaterArea";
        waterArea.tag = "Water";  // Water area is water
        
        // Position the water area outside the city (assuming city is centered at 0,0)
        // Raise it higher for better testing
        waterArea.transform.position = new Vector3(120f, 2f, 120f); // Raised from 0f to 2f
        waterArea.transform.localScale = new Vector3(40f, 4f, 40f); // Increased depth from 2f to 4f
        
        // Create containing walls for the water area
        CreateWaterContainment(waterArea.transform.position, waterArea.transform.localScale);
        
        // Create water material
        Shader waterShader = Shader.Find("Universal Render Pipeline/Lit");
        if (waterShader == null) waterShader = Shader.Find("Standard");
        
        if (waterShader != null)
        {
            Material waterMaterial = new Material(waterShader);
            waterMaterial.color = new Color(0.2f, 0.5f, 0.8f, 0.8f);
            waterMaterial.SetFloat("_Glossiness", 0.9f);
            waterMaterial.SetFloat("_Metallic", 0.0f);
            waterArea.GetComponent<Renderer>().material = waterMaterial;
        }
        
        // Add water trigger zone
        GameObject waterTrigger = new GameObject("WaterTrigger");
        waterTrigger.transform.position = waterArea.transform.position;
        waterTrigger.transform.localScale = waterArea.transform.localScale;
        
        // Add box collider as trigger
        BoxCollider triggerCollider = waterTrigger.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        
        // Add WaterZone component
        waterTrigger.AddComponent<WaterZone>();
        
        // Create water surface for better visuals
        GameObject waterSurface = GameObject.CreatePrimitive(PrimitiveType.Quad);
        waterSurface.name = "WaterSurface";
        waterSurface.transform.position = new Vector3(120f, 4f, 120f); // Adjusted to be at the top of the water
        waterSurface.transform.localScale = new Vector3(40f, 40f, 1f);
        waterSurface.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        
        if (waterShader != null)
        {
            Material surfaceMaterial = new Material(waterShader);
            surfaceMaterial.color = new Color(0.2f, 0.5f, 0.8f, 0.4f);
            surfaceMaterial.SetFloat("_Glossiness", 1.0f);
            surfaceMaterial.SetFloat("_Metallic", 0.0f);
            waterSurface.GetComponent<Renderer>().material = surfaceMaterial;
        }
    }
    
    private void CreateWaterContainment(Vector3 waterPosition, Vector3 waterScale)
    {
        float wallHeight = 6f; // Increased from 4f to 6f to match deeper water
        float wallThickness = 1f;
        
        // Create parent object for walls
        GameObject containment = new GameObject("WaterContainment");
        containment.transform.position = waterPosition;
        
        // Create four walls around the water area
        CreateContainmentWall(containment.transform, new Vector3(0, 0, waterScale.z/2 + wallThickness/2), 
            new Vector3(waterScale.x + wallThickness*2, wallHeight, wallThickness)); // North wall
        
        CreateContainmentWall(containment.transform, new Vector3(0, 0, -waterScale.z/2 - wallThickness/2), 
            new Vector3(waterScale.x + wallThickness*2, wallHeight, wallThickness)); // South wall
        
        CreateContainmentWall(containment.transform, new Vector3(waterScale.x/2 + wallThickness/2, 0, 0), 
            new Vector3(wallThickness, wallHeight, waterScale.z + wallThickness*2)); // East wall
        
        CreateContainmentWall(containment.transform, new Vector3(-waterScale.x/2 - wallThickness/2, 0, 0), 
            new Vector3(wallThickness, wallHeight, waterScale.z + wallThickness*2)); // West wall
        
        // Create ramp for easy access
        CreateAccessRamp(containment.transform, waterScale);
    }
    
    private void CreateContainmentWall(Transform parent, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(parent);
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;
        
        // Add material
        Shader standardShader = Shader.Find("Universal Render Pipeline/Lit");
        if (standardShader == null) standardShader = Shader.Find("Standard");
        if (standardShader != null)
        {
            Material wallMaterial = new Material(standardShader);
            wallMaterial.color = new Color(0.3f, 0.3f, 0.3f); // Dark gray
            wall.GetComponent<Renderer>().material = wallMaterial;
        }
    }
    
    private void CreateAccessRamp(Transform parent, Vector3 waterScale)
    {
        GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ramp.transform.SetParent(parent);
        
        // Position the ramp on the south side, adjusted for higher water level
        ramp.transform.localPosition = new Vector3(0, -2f, -waterScale.z/2 - 5f);
        ramp.transform.localScale = new Vector3(10f, 0.5f, 15f); // Made ramp longer
        ramp.transform.localRotation = Quaternion.Euler(-20f, 0, 0); // Steeper angle to reach higher water
        
        // Add material
        Shader standardShader = Shader.Find("Universal Render Pipeline/Lit");
        if (standardShader == null) standardShader = Shader.Find("Standard");
        if (standardShader != null)
        {
            Material rampMaterial = new Material(standardShader);
            rampMaterial.color = new Color(0.4f, 0.4f, 0.4f);
            ramp.GetComponent<Renderer>().material = rampMaterial;
        }
    }
    
    private void CreateCityBlocks()
    {
        int gridSize = 5;
        float blockSize = 40f;
        float spacing = 20f; // Street width
        float totalSize = gridSize * (blockSize + spacing);
        Vector3 startPos = new Vector3(-totalSize/2, 0, -totalSize/2);
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (Random.value < 0.8f) // 80% chance to create a block
                {
                    Vector3 blockPos = startPos + new Vector3(
                        x * (blockSize + spacing),
                        0,
                        z * (blockSize + spacing)
                    );
                    CreateCityBlock(blockPos, blockSize);
                }
            }
        }
    }
    
    private void CreateCityBlock(Vector3 position, float size)
    {
        int buildingsPerSide = Random.Range(2, 4);
        float buildingSize = size / buildingsPerSide;
        
        for (int x = 0; x < buildingsPerSide; x++)
        {
            for (int z = 0; z < buildingsPerSide; z++)
            {
                if (Random.value < 0.8f) // 80% chance to create a building
                {
                    Vector3 buildingPos = position + new Vector3(
                        x * buildingSize - size/2 + buildingSize/2,
                        0,
                        z * buildingSize - size/2 + buildingSize/2
                    );
                    CreateBuilding(buildingPos, buildingSize);
                }
            }
        }
    }
    
    private void CreateBuilding(Vector3 position, float size)
    {
        float height = Random.Range(10f, 30f);
        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
        building.name = "Building";
        building.tag = "Metal";  // Buildings are metal
        building.transform.position = position + new Vector3(0, height/2, 0);
        building.transform.localScale = new Vector3(size * 0.8f, height, size * 0.8f);
        
        // Add some variation to building appearance
        Shader standardShader = Shader.Find("Universal Render Pipeline/Lit");
        if (standardShader == null)
            standardShader = Shader.Find("Standard");
            
        if (standardShader != null)
        {
            Material buildingMat = new Material(standardShader);
            buildingMat.color = new Color(
                Random.Range(0.2f, 0.4f),
                Random.Range(0.2f, 0.4f),
                Random.Range(0.2f, 0.4f)
            );
            building.GetComponent<Renderer>().material = buildingMat;
        }
    }
    
    private void CreateStreets()
    {
        // Add street details like sidewalks, barriers, etc.
        GameObject streets = new GameObject("Streets");
        streets.transform.position = Vector3.zero;
        streets.tag = "Tile";  // Streets are tile
        
        Shader standardShader = Shader.Find("Universal Render Pipeline/Lit");
        if (standardShader == null)
            standardShader = Shader.Find("Standard");
            
        if (standardShader != null)
        {
            Material streetMat = new Material(standardShader);
            streetMat.color = Color.black;
        }
    }
    
    private void CreateCityProps()
    {
        // Add street lights
        float range = 200f;
        int numLights = 50;
        
        for (int i = 0; i < numLights; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-range/2, range/2),
                5f,
                Random.Range(-range/2, range/2)
            );
            CreateStreetLight(position);
        }
        
        // Add other props (simplified)
        CreateObstacles();
    }
    
    private void CreateStreetLight(Vector3 position)
    {
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "StreetLight_Pole";
        pole.transform.position = position;
        pole.transform.localScale = new Vector3(0.2f, 5f, 0.2f);
        
        GameObject light = new GameObject("StreetLight_Light");
        Light pointLight = light.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.range = 15f;
        pointLight.intensity = 2f;
        pointLight.color = new Color(1f, 0.95f, 0.8f);
        light.transform.position = position + new Vector3(0, 4.5f, 0);
    }
    
    private void CreateObstacles()
    {
        int numObstacles = 50;
        float range = 100f;
        
        for (int i = 0; i < numObstacles; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-range, range),
                1f,
                Random.Range(-range, range)
            );
            
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = "Prop_" + i;
            obstacle.transform.position = position;
            obstacle.transform.localScale = new Vector3(
                Random.Range(1f, 3f),
                Random.Range(1f, 2f),
                Random.Range(1f, 3f)
            );
            obstacle.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        }
    }
    
    private Vector3 FindValidSpawnPosition(Vector3 desiredPosition, float radius)
    {
        int maxAttempts = 30;
        Vector3 position = desiredPosition;
        
        // Check if the position is valid
        for (int i = 0; i < maxAttempts; i++)
        {
            bool isValid = true;
            Collider[] colliders = Physics.OverlapSphere(position + Vector3.up, radius);
            
            if (colliders.Length == 0)
            {
                return position;
            }
            
            // If position is invalid, try a new random position
            position = new Vector3(
                Random.Range(-100f, 100f),
                desiredPosition.y,
                Random.Range(-100f, 100f)
            );
        }
        
        Debug.LogWarning("Could not find valid spawn position after " + maxAttempts + " attempts");
        return position;
    }
    
    private GameObject CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        
        // Add required components
        player.AddComponent<CharacterController>();
        player.AddComponent<PlayerMovement>();
        player.AddComponent<SanitySystem>();
        player.AddComponent<RadioSystem>();
        FlashlightSystem flashlight = player.AddComponent<FlashlightSystem>();
        
        // Set up default flashlight sounds
        AudioClip flashlightSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Flashlight.mp3");
        if (flashlightSound != null)
        {
            flashlight.toggleOnSound = flashlightSound;
            flashlight.toggleOffSound = flashlightSound;
            Debug.Log("Flashlight toggle sound loaded successfully");
        }
        else
        {
            Debug.LogWarning("Could not load flashlight sound at Assets/Audio/Flashlight.mp3");
        }
        
        // Add audio components
        player.AddComponent<AudioListener>();
        player.AddComponent<FootstepSystem>();
        player.AddComponent<FootstepLoader>(); // This will handle loading the sounds
        
        // Create player model
        GameObject playerModel = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        playerModel.transform.SetParent(player.transform);
        playerModel.transform.localPosition = Vector3.zero;
        playerModel.transform.localScale = new Vector3(1f, 1f, 1f);
        
        // Add camera
        GameObject cameraHolder = new GameObject("CameraHolder");
        cameraHolder.transform.SetParent(player.transform);
        cameraHolder.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        
        GameObject cameraObj = new GameObject("PlayerCamera");
        cameraObj.transform.SetParent(cameraHolder.transform);
        cameraObj.transform.localPosition = Vector3.zero;
        Camera camera = cameraObj.AddComponent<Camera>();
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 1000f;

        return player;
    }
    
    private void CreateDinosaurs()
    {
        // Create Utahraptor
        GameObject utahraptor;
        if (utahraptorPrefab != null)
        {
            utahraptor = PrefabUtility.InstantiatePrefab(utahraptorPrefab) as GameObject;
        }
        else
        {
            utahraptor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            utahraptor.name = "Utahraptor";
            
            // Add required components
            NavMeshAgent utahAgent = utahraptor.AddComponent<NavMeshAgent>();
            utahAgent.speed = 8f;
            utahAgent.acceleration = 12f;
            utahAgent.angularSpeed = 120f;
            utahAgent.stoppingDistance = 2f;
            
            UtahraptorAI utahAI = utahraptor.AddComponent<UtahraptorAI>();
            
            // Configure AI settings
            utahAI.stalkingSpeed = 8f;
            utahAI.attackingSpeed = 12f;
            utahAI.attackRange = 3f;
            utahAI.freezeDistance = 20f;
            utahAI.viewAngle = 90f;
            utahAI.noiseDetectionRange = 15f;
            utahAI.scentDetectionRange = 10f;
            utahAI.investigationTime = 5f;
            utahAI.windStrength = 1f;
            utahAI.windDirection = new Vector3(1f, 0f, 0f);
            utahAI.attackDamage = 25f;
            utahAI.attackCooldown = 1.5f;
            
            // Set up obstacle layer
            utahAI.obstacleLayer = LayerMask.GetMask("Default", "Environment", "Obstacles");
            
            utahraptor.transform.localScale = new Vector3(1f, 1.2f, 2f);
        }
        
        // Find valid spawn position for Utahraptor
        Vector3 utahraptorPosition = FindValidSpawnPosition(new Vector3(20, 1, 20), 2f);
        utahraptor.transform.position = utahraptorPosition;
        
        // Create Spinosaurus near water area
        GameObject spinosaurus;
        if (spinosaurusPrefab != null)
        {
            spinosaurus = PrefabUtility.InstantiatePrefab(spinosaurusPrefab) as GameObject;
        }
        else
        {
            spinosaurus = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spinosaurus.name = "Spinosaurus";
            
            // Add required components
            NavMeshAgent spinoAgent = spinosaurus.AddComponent<NavMeshAgent>();
            spinoAgent.speed = 5f;
            spinoAgent.acceleration = 8f;
            spinoAgent.angularSpeed = 90f;
            spinoAgent.stoppingDistance = 3f;
            spinoAgent.radius = 2f;
            spinoAgent.height = 4f;
            
            SpinosaurusAI spinoAI = spinosaurus.AddComponent<SpinosaurusAI>();
            
            // Configure AI settings
            spinoAI.walkSpeed = 5f;
            spinoAI.runSpeed = 15f;
            spinoAI.roamRadius = 50f;
            spinoAI.minRoamWaitTime = 3f;
            spinoAI.maxRoamWaitTime = 8f;
            spinoAI.detectionRange = 30f;
            spinoAI.attackRange = 4f;
            spinoAI.hearingRange = 15f;
            spinoAI.attackDamage = 40f;
            spinoAI.attackCooldown = 2f;
            
            // Set up detection mask
            spinoAI.detectionMask = LayerMask.GetMask("Default", "Environment", "Obstacles", "Player");
            
            spinosaurus.transform.localScale = new Vector3(2f, 4f, 6f);
        }
        
        // Find valid spawn position for Spinosaurus near water
        Vector3 spinoPosition = FindValidSpawnPosition(new Vector3(35, 1, 35), 3f);
        spinosaurus.transform.position = spinoPosition;
        
        // Create layers if they don't exist
        CreateRequiredLayers();
    }
    
    private void CreateRequiredLayers()
    {
        // Create Obstacles layer if it doesn't exist
        if (LayerMask.NameToLayer("Obstacles") == -1)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            
            // Find first empty layer slot
            for (int i = 8; i < layers.arraySize; i++) // User layers start at 8
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (layerSP.stringValue == "")
                {
                    layerSP.stringValue = "Obstacles";
                    break;
                }
            }
            
            tagManager.ApplyModifiedProperties();
        }
        
        // Create Environment layer if it doesn't exist
        if (LayerMask.NameToLayer("Environment") == -1)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            
            // Find first empty layer slot
            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (layerSP.stringValue == "")
                {
                    layerSP.stringValue = "Environment";
                    break;
                }
            }
            
            tagManager.ApplyModifiedProperties();
        }
    }
    
    private void SetupLighting()
    {
        // Create ambient light (moonlight)
        GameObject lightObj = new GameObject("Moon Light");
        Light moonLight = lightObj.AddComponent<Light>();
        moonLight.type = LightType.Directional;
        moonLight.intensity = 0.2f; // Very dim for night atmosphere
        moonLight.color = new Color(0.7f, 0.7f, 1f); // Blueish moonlight
        lightObj.transform.rotation = Quaternion.Euler(45, -30, 0);
        
        // Set ambient lighting to dark night
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.1f);
        
        // Setup fog
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = new Color(0.05f, 0.05f, 0.075f); // Dark blueish fog
        RenderSettings.fogDensity = 0.02f;
        
        // Modify street lights to be more atmospheric
        Light[] streetLights = GameObject.FindObjectsOfType<Light>();
        foreach (Light light in streetLights)
        {
            if (light.type == LightType.Point)
            {
                // Make street lights dimmer and more atmospheric
                light.intensity = 1.5f;
                light.range = 20f;
                light.color = new Color(1f, 0.9f, 0.7f); // Warm, slightly yellow
                
                // Add volumetric light effect
                GameObject volumetricObj = new GameObject("VolumetricLight");
                volumetricObj.transform.SetParent(light.transform);
                volumetricObj.transform.localPosition = Vector3.zero;
                
                // Add particle system for volumetric effect
                ParticleSystem ps = volumetricObj.AddComponent<ParticleSystem>();
                var main = ps.main;
                main.loop = true;
                main.duration = 1f;
                main.startLifetime = 2f;
                main.startSize = 10f;
                main.startColor = new Color(1f, 1f, 1f, 0.1f);
                
                var emission = ps.emission;
                emission.rateOverTime = 10f;
                
                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 30f;
                
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                renderer.material = new Material(Shader.Find("Particles/Additive"));
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }
        }
    }
    
    private void SetupMission(GameObject player)
    {
        // Create mission system
        GameObject missionObj = new GameObject("Mission System");
        MissionSystem missionSystem = missionObj.AddComponent<MissionSystem>();
        
        // Create start and end points
        GameObject startPoint = new GameObject("Start Point");
        startPoint.transform.position = player.transform.position;
        
        GameObject endPoint = new GameObject("End Point");
        endPoint.transform.position = new Vector3(80, 0, 80);
        
        // Setup mission parameters
        missionSystem.startPoint = startPoint.transform;
        missionSystem.endPoint = endPoint.transform;
        
        // Create direction indicator (invisible)
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        indicator.name = "Direction Indicator";
        indicator.transform.SetParent(player.transform);
        indicator.transform.localPosition = new Vector3(0, 2, 0);
        indicator.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f);
        indicator.GetComponent<MeshRenderer>().enabled = false; // Make it invisible
        missionSystem.directionIndicator = indicator;
        
        // Setup mission UI
        CreateMissionUI(missionSystem);
    }
    
    private void CreateMissionUI(MissionSystem missionSystem)
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // Create mission panel
        GameObject missionPanel = new GameObject("Mission Panel");
        missionPanel.transform.SetParent(canvas.transform);
        RectTransform missionRect = missionPanel.AddComponent<RectTransform>();
        missionRect.anchorMin = new Vector2(1, 1);
        missionRect.anchorMax = new Vector2(1, 1);
        missionRect.pivot = new Vector2(1, 1);
        missionRect.anchoredPosition = new Vector2(-20, -20);
        missionRect.sizeDelta = new Vector2(300, 100);
        
        // Create objective text
        GameObject objectiveObj = new GameObject("Objective Text");
        objectiveObj.transform.SetParent(missionPanel.transform);
        TextMeshProUGUI objectiveText = objectiveObj.AddComponent<TextMeshProUGUI>();
        objectiveText.text = "Objective: Escape the City";
        objectiveText.fontSize = 16;
        objectiveText.color = Color.white;
        objectiveText.alignment = TextAlignmentOptions.TopLeft;
        RectTransform objectiveRect = objectiveObj.GetComponent<RectTransform>();
        objectiveRect.anchorMin = Vector2.zero;
        objectiveRect.anchorMax = Vector2.one;
        objectiveRect.sizeDelta = Vector2.zero;
        objectiveRect.offsetMin = new Vector2(10, 40);
        objectiveRect.offsetMax = new Vector2(-10, -10);
        
        // Create distance text
        GameObject distanceObj = new GameObject("Distance Text");
        distanceObj.transform.SetParent(missionPanel.transform);
        TextMeshProUGUI distanceText = distanceObj.AddComponent<TextMeshProUGUI>();
        distanceText.text = "Distance: 0m";
        distanceText.fontSize = 14;
        distanceText.color = Color.yellow;
        distanceText.alignment = TextAlignmentOptions.TopLeft;
        RectTransform distanceRect = distanceObj.GetComponent<RectTransform>();
        distanceRect.anchorMin = new Vector2(0, 0);
        distanceRect.anchorMax = new Vector2(1, 0);
        distanceRect.anchoredPosition = new Vector2(10, 10);
        distanceRect.sizeDelta = new Vector2(-20, 20);
        
        // Add background image to make text more visible
        GameObject background = new GameObject("Panel Background");
        background.transform.SetParent(missionPanel.transform);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // Setup references
        missionSystem.objectiveText = objectiveText;
        missionSystem.distanceText = distanceText;
    }
    
    private void GenerateNavMesh()
    {
        // Add NavMeshSurface component to the ground
        GameObject ground = GameObject.Find("Ground");
        if (ground != null)
        {
            NavMeshSurface surface = ground.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.BuildNavMesh();
        }
    }
} 