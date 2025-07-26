using UnityEngine;

public class NatureMapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapSize = 50;
    public float treeSpacing = 8f;  // Increased spacing between trees
    public float rockSpacing = 3f;
    
    [Header("Spawn Chances")]
    public float treeSpawnChance = 0.1f;  // Reduced from 0.25f to spawn fewer trees
    public float rockSpawnChance = 0.3f;
    public float grassSpawnChance = 0.5f;
    public float flowerSpawnChance = 0.2f;
    
    [Header("Scale Ranges")]
    public Vector2 treeScaleRange = new Vector2(0.6f, 0.9f);  // Reduced from 0.8f-1.2f
    public Vector2 rockScaleRange = new Vector2(0.5f, 1.5f);
    
    [Header("Special Locations")]
    public Vector3 waterPosition = new Vector3(20, 0, 20);
    public Vector3 waterScale = new Vector3(12, 1, 12);
    public Vector3 campfirePosition = new Vector3(-15, 0, -15);
    
    [Header("Organization")]
    public bool organizeIntoFolders = true;
    
    private Transform natureParent;
    private Transform treesParent;
    private Transform rocksParent;
    private Transform grassParent;
    private Transform flowersParent;
    private Transform structuresParent;
    
    void Start()
    {
        // Disabled automatic map generation - use the manually created battlefield instead
        // CreateNatureMap();
    }
    
    [ContextMenu("Generate Nature Map")]
    public void CreateNatureMap()
    {
        // Clear existing nature map if any
        ClearExistingNature();
        
        // Setup organization folders
        SetupFolders();
        
        // Generate the nature elements
        Debug.Log("Starting nature map generation with Lowpoly nature assets...");
        CreateGround();
        AddTreesFromAssets();
        AddRocksFromAssets();
        AddGrassFromAssets();
        AddFlowersFromAssets();
        AddWaterFromAssets();
        AddCampfireFromAssets();
        
        Debug.Log("Nature map generation complete!");
    }
    
    void ClearExistingNature()
    {
        GameObject existing = GameObject.Find("Nature Map");
        if (existing != null)
        {
            DestroyImmediate(existing);
        }
    }
    
    void SetupFolders()
    {
        natureParent = new GameObject("Nature Map").transform;
        treesParent = new GameObject("Trees").transform;
        rocksParent = new GameObject("Rocks").transform;
        grassParent = new GameObject("Grass & Plants").transform;
        flowersParent = new GameObject("Flowers").transform;
        structuresParent = new GameObject("Structures").transform;
        
        treesParent.SetParent(natureParent);
        rocksParent.SetParent(natureParent);
        grassParent.SetParent(natureParent);
        flowersParent.SetParent(natureParent);
        structuresParent.SetParent(natureParent);
    }
    
    void CreateGround()
    {
        // Use existing terrain instead of creating a new GroundPlane
        Terrain existingTerrain = FindObjectOfType<Terrain>();
        
        if (existingTerrain != null)
        {
            Debug.Log("Using existing terrain as battlefield ground");
            existingTerrain.name = "Battlefield Terrain";
            
            if (organizeIntoFolders && natureParent != null)
            {
                existingTerrain.transform.SetParent(natureParent);
            }
        }
        else
        {
            // If no terrain exists, check for terrain objects from the Lowpoly nature pack
            GameObject terrainObject = GameObject.Find("terrain");
            if (terrainObject != null)
            {
                Debug.Log("Using Lowpoly nature terrain as battlefield");
                terrainObject.name = "Battlefield Terrain";
                
                if (organizeIntoFolders && natureParent != null)
                {
                    terrainObject.transform.SetParent(natureParent);
                }
            }
            else
            {
                Debug.LogWarning("No terrain found! Creating a simple ground plane as fallback");
                CreateFallbackGround();
            }
        }
    }
    
    void CreateFallbackGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Battlefield Ground (Fallback)";
        ground.transform.localScale = new Vector3(mapSize, 1, mapSize);
        ground.transform.position = Vector3.zero;
        
        // Create a nice battlefield material
        Material groundMaterial = new Material(Shader.Find("Standard"));
        groundMaterial.color = new Color(0.3f, 0.5f, 0.2f); // Darker green for battlefield
        ground.GetComponent<Renderer>().material = groundMaterial;
        
        if (organizeIntoFolders && natureParent != null)
        {
            ground.transform.SetParent(natureParent);
        }
    }
    
    void AddTreesFromAssets()
    {
        string[] treePrefabPaths = {
            "Assets/Oode studios/Lowpoly nature/Prefabs/Trees/Tree 001.prefab",
            "Assets/Oode studios/Lowpoly nature/Prefabs/Trees/Tree 002.prefab",
            "Assets/Oode studios/Lowpoly nature/Prefabs/Trees/Tree 003.prefab"
        };
        
        // Add trees in a natural pattern
        for (int x = -mapSize/2; x < mapSize/2; x += (int)treeSpacing)
        {
            for (int z = -mapSize/2; z < mapSize/2; z += (int)treeSpacing)
            {
                if (Random.value < treeSpawnChance)
                {
                    Vector3 position = new Vector3(
                        x + Random.Range(-treeSpacing/2, treeSpacing/2),
                        0,
                        z + Random.Range(-treeSpacing/2, treeSpacing/2)
                    );
                    
                    // Skip if too close to water or campfire
                    if (IsPositionBlocked(position, 8f))
                        continue;
                    
                    // Note: In runtime, we create simple trees since we can't load prefabs directly by path
                    // For the editor, you would drag the prefabs into public arrays
                    CreateSimpleTree(position);
                }
            }
        }
    }
    
    void CreateSimpleTree(Vector3 position)
    {
        GameObject tree = new GameObject("Tree");
        tree.transform.position = position;
        
        // Create trunk
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = Vector3.up * 2.5f;
        trunk.transform.localScale = new Vector3(0.6f, 2.5f, 0.6f);
        
        Material trunkMaterial = new Material(Shader.Find("Standard"));
        trunkMaterial.color = new Color(0.4f, 0.2f, 0.1f);
        trunk.GetComponent<Renderer>().material = trunkMaterial;
        
        // Create leaves
        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.name = "Leaves";
        leaves.transform.SetParent(tree.transform);
        leaves.transform.localPosition = Vector3.up * 6f;
        leaves.transform.localScale = Vector3.one * Random.Range(3f, 4.5f);
        
        Material leavesMaterial = new Material(Shader.Find("Standard"));
        leavesMaterial.color = new Color(0.1f, Random.Range(0.6f, 0.9f), 0.1f);
        leaves.GetComponent<Renderer>().material = leavesMaterial;
        
        // Add some randomness
        float scale = Random.Range(treeScaleRange.x, treeScaleRange.y);
        tree.transform.localScale = Vector3.one * scale;
        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        
        tree.transform.SetParent(treesParent);
    }
    
    void AddRocksFromAssets()
    {
        int rockCount = mapSize * 3;
        for (int i = 0; i < rockCount; i++)
        {
            if (Random.value < rockSpawnChance)
            {
                Vector3 position = new Vector3(
                    Random.Range(-mapSize/2f, mapSize/2f),
                    0,
                    Random.Range(-mapSize/2f, mapSize/2f)
                );
                
                if (IsPositionBlocked(position, 3f))
                    continue;
                
                CreateSimpleRock(position);
            }
        }
    }
    
    void CreateSimpleRock(Vector3 position)
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rock.name = "Rock";
        rock.transform.position = position + Vector3.up * 0.2f;
        
        // Make rocks look more natural
        float scale = Random.Range(rockScaleRange.x, rockScaleRange.y);
        rock.transform.localScale = new Vector3(
            scale * Random.Range(0.8f, 1.2f),
            scale * Random.Range(0.5f, 0.8f),
            scale * Random.Range(0.8f, 1.2f)
        );
        
        rock.transform.rotation = Quaternion.Euler(
            Random.Range(-20f, 20f),
            Random.Range(0f, 360f),
            Random.Range(-20f, 20f)
        );
        
        Material rockMaterial = new Material(Shader.Find("Standard"));
        rockMaterial.color = new Color(
            Random.Range(0.4f, 0.6f),
            Random.Range(0.4f, 0.6f),
            Random.Range(0.4f, 0.6f)
        );
        rock.GetComponent<Renderer>().material = rockMaterial;
        
        rock.transform.SetParent(rocksParent);
    }
    
    void AddGrassFromAssets()
    {
        int grassCount = mapSize * 8;
        for (int i = 0; i < grassCount; i++)
        {
            if (Random.value < grassSpawnChance)
            {
                Vector3 position = new Vector3(
                    Random.Range(-mapSize/2f, mapSize/2f),
                    0.01f,
                    Random.Range(-mapSize/2f, mapSize/2f)
                );
                
                CreateSimpleGrass(position);
            }
        }
    }
    
    void CreateSimpleGrass(Vector3 position)
    {
        GameObject grass = GameObject.CreatePrimitive(PrimitiveType.Cube);
        grass.name = "Grass";
        grass.transform.position = position + Vector3.up * 0.3f;
        grass.transform.localScale = new Vector3(
            Random.Range(0.1f, 0.2f),
            Random.Range(0.5f, 0.8f),
            Random.Range(0.1f, 0.2f)
        );
        
        Material grassMaterial = new Material(Shader.Find("Standard"));
        grassMaterial.color = new Color(0.1f, Random.Range(0.8f, 1f), 0.1f);
        grass.GetComponent<Renderer>().material = grassMaterial;
        
        grass.transform.SetParent(grassParent);
    }
    
    void AddFlowersFromAssets()
    {
        int flowerCount = mapSize * 2;
        for (int i = 0; i < flowerCount; i++)
        {
            if (Random.value < flowerSpawnChance)
            {
                Vector3 position = new Vector3(
                    Random.Range(-mapSize/2f, mapSize/2f),
                    0.01f,
                    Random.Range(-mapSize/2f, mapSize/2f)
                );
                
                CreateSimpleFlower(position);
            }
        }
    }
    
    void CreateSimpleFlower(Vector3 position)
    {
        GameObject flower = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        flower.name = "Flower";
        flower.transform.position = position + Vector3.up * 0.3f;
        flower.transform.localScale = Vector3.one * Random.Range(0.2f, 0.4f);
        
        Color[] flowerColors = {
            Color.red, Color.yellow, Color.magenta, Color.blue, Color.white, Color.cyan
        };
        
        Material flowerMaterial = new Material(Shader.Find("Standard"));
        flowerMaterial.color = flowerColors[Random.Range(0, flowerColors.Length)];
        flower.GetComponent<Renderer>().material = flowerMaterial;
        
        flower.transform.SetParent(flowersParent);
    }
    
    void AddWaterFromAssets()
    {
        GameObject water = GameObject.CreatePrimitive(PrimitiveType.Cube);
        water.name = "Water Lake";
        water.transform.position = waterPosition;
        water.transform.localScale = waterScale;
        
        Material waterMaterial = new Material(Shader.Find("Standard"));
        waterMaterial.color = new Color(0.2f, 0.6f, 1f, 0.8f);
        water.GetComponent<Renderer>().material = waterMaterial;
        
        water.transform.SetParent(structuresParent);
    }
    
    void AddCampfireFromAssets()
    {
        GameObject campfire = new GameObject("Campfire");
        campfire.transform.position = campfirePosition;
        
        // Create fire pit
        GameObject pit = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pit.name = "Fire Pit";
        pit.transform.SetParent(campfire.transform);
        pit.transform.localPosition = Vector3.zero;
        pit.transform.localScale = new Vector3(2.5f, 0.3f, 2.5f);
        
        Material pitMaterial = new Material(Shader.Find("Standard"));
        pitMaterial.color = new Color(0.2f, 0.1f, 0.05f);
        pit.GetComponent<Renderer>().material = pitMaterial;
        
        // Create fire
        GameObject fire = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fire.name = "Fire";
        fire.transform.SetParent(campfire.transform);
        fire.transform.localPosition = Vector3.up * 0.8f;
        fire.transform.localScale = Vector3.one * 1.2f;
        
        Material fireMaterial = new Material(Shader.Find("Standard"));
        fireMaterial.color = new Color(1f, 0.4f, 0f);
        fire.GetComponent<Renderer>().material = fireMaterial;
        
        campfire.transform.SetParent(structuresParent);
    }
    
    bool IsPositionBlocked(Vector3 position, float minDistance)
    {
        return Vector3.Distance(position, waterPosition) < minDistance ||
               Vector3.Distance(position, campfirePosition) < minDistance;
    }
}
