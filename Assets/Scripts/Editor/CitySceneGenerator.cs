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
            
            GameObject playerModel = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerModel.transform.SetParent(player.transform);
            playerModel.transform.localPosition = Vector3.zero;
            playerModel.transform.localScale = new Vector3(1f, 1f, 1f);
            
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
            
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            head.transform.SetParent(utahraptor.transform);
            head.transform.localPosition = new Vector3(0, 0.5f, 1f);
            head.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
            
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
            
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            head.transform.SetParent(spinosaurus.transform);
            head.transform.localPosition = new Vector3(0, 1f, 1.5f);
            head.transform.localScale = new Vector3(0.8f, 1.2f, 1f);
            
            GameObject sail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sail.transform.SetParent(spinosaurus.transform);
            sail.transform.localPosition = new Vector3(0, 2f, 0);
            sail.transform.localScale = new Vector3(0.2f, 2f, 3f);
            
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
        // Create ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.localScale = new Vector3(20, 1, 20);
        ground.name = "Ground";
        
        // Create city blocks
        CreateCityBlocks();
        
        // Create streets
        CreateStreets();
        
        // Add props and details
        CreateCityProps();
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
        // This is a simplified version
        GameObject streets = new GameObject("Streets");
        streets.transform.position = Vector3.zero;
        
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
    
    private GameObject CreatePlayer()
    {
        GameObject player;
        if (playerPrefab != null)
        {
            player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
        }
        else
        {
            player = new GameObject("Player");
            player.tag = "Player";
            
            // Add required components
            player.AddComponent<CharacterController>();
            player.AddComponent<PlayerMovement>();
            player.AddComponent<SanitySystem>();
            player.AddComponent<RadioSystem>();
            
            // Add capsule for visualization
            GameObject playerModel = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerModel.transform.SetParent(player.transform);
            playerModel.transform.localPosition = Vector3.zero;
        }
        
        player.transform.position = new Vector3(0, 1, 0);
        
        // Add camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        cameraObj.AddComponent<AudioListener>();
        cameraObj.transform.SetParent(player.transform);
        cameraObj.transform.localPosition = new Vector3(0, 1.6f, 0);
        
        // Setup UI
        CreatePlayerUI();
        
        return player;
    }
    
    private void CreatePlayerUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("UI Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create status panel
        GameObject statusPanel = new GameObject("Status Panel");
        statusPanel.transform.SetParent(canvasObj.transform);
        RectTransform statusRect = statusPanel.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 1);
        statusRect.anchorMax = new Vector2(0, 1);
        statusRect.pivot = new Vector2(0, 1);
        statusRect.anchoredPosition = new Vector2(20, -20);
        
        // Add UI components
        CreateStatusBar(statusPanel, "Sanity", new Vector2(0, 0));
        CreateStatusBar(statusPanel, "Stamina", new Vector2(0, -30));
        CreateStatusBar(statusPanel, "Radio Battery", new Vector2(0, -60));
        
        // Add PlayerStatusUI component
        PlayerStatusUI statusUI = statusPanel.AddComponent<PlayerStatusUI>();
        
        // Setup references
        statusUI.sanitySlider = statusPanel.transform.Find("Sanity/Slider").GetComponent<Slider>();
        statusUI.staminaSlider = statusPanel.transform.Find("Stamina/Slider").GetComponent<Slider>();
        statusUI.batterySlider = statusPanel.transform.Find("Radio Battery/Slider").GetComponent<Slider>();
    }
    
    private void CreateStatusBar(GameObject parent, string label, Vector2 position)
    {
        GameObject container = new GameObject(label);
        container.transform.SetParent(parent.transform);
        RectTransform rect = container.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 20);
        
        // Create label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform);
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label + ":";
        labelText.fontSize = 14;
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchoredPosition = new Vector2(-100, 0);
        labelRect.sizeDelta = new Vector2(80, 20);
        
        // Create slider
        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(container.transform);
        Slider slider = sliderObj.AddComponent<Slider>();
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchoredPosition = new Vector2(50, 0);
        sliderRect.sizeDelta = new Vector2(100, 20);
        
        // Add slider background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObj.transform);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // Add slider fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(sliderObj.transform);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.green;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        
        // Setup slider
        slider.targetGraphic = fillImage;
        slider.fillRect = fillRect;
        slider.direction = Slider.Direction.LeftToRight;
        slider.value = 1f;
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
            utahraptor.AddComponent<UtahraptorAI>();
            utahraptor.AddComponent<NavMeshAgent>();
            utahraptor.transform.localScale = new Vector3(1, 2, 3);
        }
        utahraptor.transform.position = new Vector3(20, 1, 20);
        
        // Create Spinosaurus
        GameObject spinosaurus;
        if (spinosaurusPrefab != null)
        {
            spinosaurus = PrefabUtility.InstantiatePrefab(spinosaurusPrefab) as GameObject;
        }
        else
        {
            spinosaurus = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spinosaurus.name = "Spinosaurus";
            spinosaurus.AddComponent<SpinosaurusAI>();
            spinosaurus.AddComponent<NavMeshAgent>();
            spinosaurus.transform.localScale = new Vector3(2, 4, 6);
        }
        spinosaurus.transform.position = new Vector3(-20, 1, -20);
    }
    
    private void SetupLighting()
    {
        // Create ambient light
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 0.3f; // Darker for horror atmosphere
        lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
        
        // Set ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.15f);
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
        
        // Create direction indicator
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        indicator.name = "Direction Indicator";
        indicator.transform.SetParent(player.transform);
        indicator.transform.localPosition = new Vector3(0, 2, 0);
        indicator.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f);
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
        objectiveText.fontSize = 16;
        objectiveText.color = Color.white;
        RectTransform objectiveRect = objectiveObj.GetComponent<RectTransform>();
        objectiveRect.anchorMin = Vector2.zero;
        objectiveRect.anchorMax = Vector2.one;
        objectiveRect.sizeDelta = Vector2.zero;
        
        // Create distance text
        GameObject distanceObj = new GameObject("Distance Text");
        distanceObj.transform.SetParent(missionPanel.transform);
        TextMeshProUGUI distanceText = distanceObj.AddComponent<TextMeshProUGUI>();
        distanceText.fontSize = 14;
        distanceText.color = Color.yellow;
        RectTransform distanceRect = distanceObj.GetComponent<RectTransform>();
        distanceRect.anchorMin = new Vector2(0, 0);
        distanceRect.anchorMax = new Vector2(1, 0);
        distanceRect.anchoredPosition = new Vector2(0, 20);
        distanceRect.sizeDelta = new Vector2(0, 20);
        
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