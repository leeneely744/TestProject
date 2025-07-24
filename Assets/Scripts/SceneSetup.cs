using UnityEngine;
using UnityEngine.UI;

public class SceneSetup : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject heroPrefab;
    public GameObject castlePrefab;
    public GameObject orcPrefab;
    public GameObject ogrePrefab;
    public GameObject bossPrefab;
    public GameObject weaponPrefab;
    
    [Header("UI Prefabs")]
    public GameObject canvasPrefab;
    public GameObject gameOverPanelPrefab;
    public GameObject victoryPanelPrefab;
    
    [Header("Scene Layout")]
    public Vector2 castlePosition = new Vector2(8, 0);
    public Vector2 heroStartPosition = new Vector2(0, 0);
    public Vector2 spawnPosition = new Vector2(-8, 0);
    
    [Header("Waypoints")]
    public Vector2[] waypointPositions = {
        new Vector2(-8, 0),
        new Vector2(-4, 0),
        new Vector2(0, 0),
        new Vector2(4, 0),
        new Vector2(8, 0)
    };
    
    [Header("Weapon Positions")]
    public Vector2[] weaponPositions = {
        new Vector2(-6, 2),
        new Vector2(-2, -2),
        new Vector2(2, 2),
        new Vector2(6, -2)
    };
    
    void Start()
    {
        SetupScene();
    }
    
    void SetupScene()
    {
        // Create main camera if it doesn't exist
        if (Camera.main == null)
        {
            GameObject cameraGO = new GameObject("Main Camera");
            Camera camera = cameraGO.AddComponent<Camera>();
            camera.tag = "MainCamera";
            cameraGO.AddComponent<AudioListener>();
            camera.orthographic = true;
            camera.orthographicSize = 5;
            camera.transform.position = new Vector3(0, 0, -10);
        }
        
        // Create waypoints
        CreateWaypoints();
        
        // Create castle
        CreateCastle();
        
        // Create hero
        CreateHero();
        
        // Create weapons
        CreateWeapons();
        
        // Create UI
        CreateUI();
        
        // Create enemy spawner
        CreateEnemySpawner();
        
        // Create game manager
        CreateGameManager();
    }
    
    void CreateWaypoints()
    {
        GameObject waypointContainer = new GameObject("Waypoints");
        
        for (int i = 0; i < waypointPositions.Length; i++)
        {
            GameObject waypoint = new GameObject("Waypoint_" + i);
            waypoint.transform.position = waypointPositions[i];
            waypoint.transform.parent = waypointContainer.transform;
            
            // Add visual indicator
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = waypoint.transform;
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = Vector3.one * 0.2f;
            cube.GetComponent<Renderer>().material.color = Color.yellow;
            Destroy(cube.GetComponent<Collider>());
        }
    }
    
    void CreateCastle()
    {
        GameObject castle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        castle.name = "Castle";
        castle.transform.position = castlePosition;
        castle.transform.localScale = new Vector3(2, 2, 1);
        castle.GetComponent<Renderer>().material.color = Color.blue;
        
        // Add castle script
        castle.AddComponent<Castle>();
        
        // Add trigger collider
        BoxCollider trigger = castle.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
    }
    
    void CreateHero()
    {
        GameObject hero = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        hero.name = "Hero";
        hero.transform.position = heroStartPosition;
        hero.GetComponent<Renderer>().material.color = Color.green;
        
        // Add hero script
        hero.AddComponent<Hero>();
        
        // Add rigidbody and collider
        Rigidbody2D rb = hero.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        
        CircleCollider2D collider = hero.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }
    
    void CreateWeapons()
    {
        for (int i = 0; i < weaponPositions.Length; i++)
        {
            GameObject weapon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            weapon.name = "Weapon_" + i;
            weapon.transform.position = weaponPositions[i];
            weapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            weapon.GetComponent<Renderer>().material.color = Color.red;
            
            // Add weapon script
            Weapon weaponScript = weapon.AddComponent<Weapon>();
            weaponScript.upgradeColors = new Color[] { Color.red, Color.yellow, Color.orange, Color.magenta, Color.cyan };
            
            // Add collider
            weapon.AddComponent<BoxCollider2D>();
        }
    }
    
    void CreateUI()
    {
        // Create canvas
        GameObject canvas = new GameObject("Canvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        
        // Create UI texts
        CreateUIText(canvas, "TimerText", new Vector2(-200, 200), "Time: 180");
        CreateUIText(canvas, "CoinsText", new Vector2(-200, 160), "Coins: 0");
        CreateUIText(canvas, "StageText", new Vector2(-200, 120), "Stage: 1");
        CreateUIText(canvas, "CastleHealthText", new Vector2(-200, 80), "Castle HP: 100");
        
        // Create game over panel
        GameObject gameOverPanel = CreatePanel(canvas, "GameOverPanel", "GAME OVER");
        gameOverPanel.SetActive(false);
        
        // Create victory panel
        GameObject victoryPanel = CreatePanel(canvas, "VictoryPanel", "VICTORY!");
        victoryPanel.SetActive(false);
    }
    
    GameObject CreateUIText(GameObject parent, string name, Vector2 position, string text)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.parent = parent.transform;
        
        RectTransform rectTransform = textGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(200, 30);
        
        Text textComponent = textGO.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 16;
        textComponent.color = Color.white;
        
        return textGO;
    }
    
    GameObject CreatePanel(GameObject parent, string name, string text)
    {
        GameObject panel = new GameObject(name);
        panel.transform.parent = parent.transform;
        
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);
        
        // Add text
        CreateUIText(panel, text + "Text", Vector2.zero, text);
        
        return panel;
    }
    
    void CreateEnemySpawner()
    {
        GameObject spawner = new GameObject("EnemySpawner");
        spawner.transform.position = spawnPosition;
        
        EnemySpawner spawnerScript = spawner.AddComponent<EnemySpawner>();
        
        // Create enemy prefabs
        spawnerScript.orcPrefab = CreateEnemyPrefab(EnemyType.Orc, Color.red);
        spawnerScript.ogrePrefab = CreateEnemyPrefab(EnemyType.Ogre, Color.magenta);
        spawnerScript.bossPrefab = CreateEnemyPrefab(EnemyType.Boss, Color.black);
        
        // Set spawn point
        GameObject spawnPoint = new GameObject("SpawnPoint");
        spawnPoint.transform.position = spawnPosition;
        spawnPoint.transform.parent = spawner.transform;
        spawnerScript.spawnPoint = spawnPoint.transform;
    }
    
    GameObject CreateEnemyPrefab(EnemyType type, Color color)
    {
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemy.name = type.ToString();
        enemy.GetComponent<Renderer>().material.color = color;
        
        // Scale based on enemy type
        switch (type)
        {
            case EnemyType.Orc:
                enemy.transform.localScale = Vector3.one * 0.8f;
                break;
            case EnemyType.Ogre:
                enemy.transform.localScale = Vector3.one * 1.2f;
                break;
            case EnemyType.Boss:
                enemy.transform.localScale = Vector3.one * 1.5f;
                break;
        }
        
        // Add enemy script
        Enemy enemyScript = enemy.AddComponent<Enemy>();
        enemyScript.enemyType = type;
        
        // Add rigidbody and collider
        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        
        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        
        return enemy;
    }
    
    void CreateGameManager()
    {
        GameObject gameManager = new GameObject("GameManager");
        GameManager gmScript = gameManager.AddComponent<GameManager>();
        
        // Connect UI references
        gmScript.timerText = GameObject.Find("TimerText").GetComponent<Text>();
        gmScript.coinsText = GameObject.Find("CoinsText").GetComponent<Text>();
        gmScript.stageText = GameObject.Find("StageText").GetComponent<Text>();
        gmScript.castleHealthText = GameObject.Find("CastleHealthText").GetComponent<Text>();
        gmScript.gameOverPanel = GameObject.Find("GameOverPanel");
        gmScript.victoryPanel = GameObject.Find("VictoryPanel");
    }
}