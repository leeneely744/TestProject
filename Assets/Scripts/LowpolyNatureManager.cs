using UnityEngine;

public class LowpolyNatureManager : MonoBehaviour
{
    [Header("Organization")]
    public bool organizeIntoFolders = true;
    
    [Header("Map Setup")]
    public bool clearExistingOnStart = false;
    
    private Transform natureParent;
    private Transform treesParent;
    private Transform rocksParent;
    private Transform grassParent;
    private Transform flowersParent;
    private Transform structuresParent;
    private Transform terrainParent;
    
    void Start()
    {
        if (clearExistingOnStart)
        {
            OrganizeExistingNatureAssets();
        }
    }
    
    [ContextMenu("Organize Existing Nature Assets")]
    public void OrganizeExistingNatureAssets()
    {
        SetupFolders();
        OrganizeAssetsByType();
        Debug.Log("Organized all Lowpoly nature assets into folders!");
    }
    
    void SetupFolders()
    {
        // Clear existing organization
        GameObject existing = GameObject.Find("Lowpoly Nature Scene");
        if (existing != null)
        {
            DestroyImmediate(existing);
        }
        
        // Create folder structure
        natureParent = new GameObject("Lowpoly Nature Scene").transform;
        treesParent = new GameObject("🌲 Trees").transform;
        rocksParent = new GameObject("🪨 Rocks & Cliffs").transform;
        grassParent = new GameObject("🌱 Grass & Plants").transform;
        flowersParent = new GameObject("🌸 Flowers").transform;
        structuresParent = new GameObject("🏗️ Structures").transform;
        terrainParent = new GameObject("🏔️ Terrain").transform;
        
        treesParent.SetParent(natureParent);
        rocksParent.SetParent(natureParent);
        grassParent.SetParent(natureParent);
        flowersParent.SetParent(natureParent);
        structuresParent.SetParent(natureParent);
        terrainParent.SetParent(natureParent);
    }
    
    void OrganizeAssetsByType()
    {
        // Find and organize all nature assets in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.transform.parent != null) continue; // Skip child objects
            
            string name = obj.name.ToLower();
            
            // Organize trees
            if (name.Contains("tree") || name.Contains("pine"))
            {
                obj.transform.SetParent(treesParent);
                continue;
            }
            
            // Organize rocks and cliffs
            if (name.Contains("rock") || name.Contains("cliff"))
            {
                obj.transform.SetParent(rocksParent);
                continue;
            }
            
            // Organize grass and mushrooms
            if (name.Contains("grass") || name.Contains("mashroom") || name.Contains("mushroom"))
            {
                obj.transform.SetParent(grassParent);
                continue;
            }
            
            // Organize flowers
            if (name.Contains("flower"))
            {
                obj.transform.SetParent(flowersParent);
                continue;
            }
            
            // Organize structures
            if (name.Contains("bridge") || name.Contains("camp") || name.Contains("fire") || name.Contains("water"))
            {
                obj.transform.SetParent(structuresParent);
                continue;
            }
            
            // Organize terrain
            if (name.Contains("terrain") || name.Contains("plane") || name.Contains("ground"))
            {
                obj.transform.SetParent(terrainParent);
                continue;
            }
        }
    }
    
    [ContextMenu("Add Random Nature Elements")]
    public void AddRandomNatureElements()
    {
        if (natureParent == null) SetupFolders();
        
        // Add some random elements around the existing scene
        AddRandomTrees(5);
        AddRandomRocks(8);
        AddRandomGrass(15);
        AddRandomFlowers(6);
        
        Debug.Log("Added random nature elements to the scene!");
    }
    
    void AddRandomTrees(int count)
    {
        string[] treePaths = {
            "Assets/Oode studios/Lowpoly nature/Prefabs/Trees/Tree 001.prefab",
            "Assets/Oode studios/Lowpoly nature/Prefabs/Trees/Tree 002.prefab",
            "Assets/Oode studios/Lowpoly nature/Prefabs/Trees/Tree 003.prefab",
            "Assets/Oode studios/Lowpoly nature/Prefabs/Pine trees/Pine tree 001.prefab",
            "Assets/Oode studios/Lowpoly nature/Prefabs/Pine trees/Pine tree 002.prefab"
        };
        
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-25f, 25f),
                0,
                Random.Range(-25f, 25f)
            );
            
            // Note: In actual Unity editor, you would use AssetDatabase to load these
            // For runtime, we create placeholder objects
            CreatePlaceholderTree(randomPos);
        }
    }
    
    void AddRandomRocks(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-30f, 30f),
                0,
                Random.Range(-30f, 30f)
            );
            
            CreatePlaceholderRock(randomPos);
        }
    }
    
    void AddRandomGrass(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-20f, 20f),
                0,
                Random.Range(-20f, 20f)
            );
            
            CreatePlaceholderGrass(randomPos);
        }
    }
    
    void AddRandomFlowers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-15f, 15f),
                0,
                Random.Range(-15f, 15f)
            );
            
            CreatePlaceholderFlower(randomPos);
        }
    }
    
    // Placeholder creation methods for runtime
    GameObject CreatePlaceholderTree(Vector3 position)
    {
        GameObject tree = new GameObject("Tree (Placeholder)");
        tree.transform.position = position;
        tree.transform.SetParent(treesParent);
        
        // Simple visual representation
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = Vector3.up * 2;
        trunk.transform.localScale = new Vector3(0.5f, 2f, 0.5f);
        trunk.GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0f);
        
        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.transform.SetParent(tree.transform);
        leaves.transform.localPosition = Vector3.up * 4;
        leaves.transform.localScale = Vector3.one * 2.5f;
        leaves.GetComponent<Renderer>().material.color = new Color(0.1f, 0.7f, 0.1f);
        
        return tree;
    }
    
    GameObject CreatePlaceholderRock(Vector3 position)
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rock.name = "Rock (Placeholder)";
        rock.transform.position = position;
        rock.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
        rock.transform.rotation = Quaternion.Euler(
            Random.Range(-15f, 15f),
            Random.Range(0f, 360f),
            Random.Range(-15f, 15f)
        );
        rock.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
        rock.transform.SetParent(rocksParent);
        
        return rock;
    }
    
    GameObject CreatePlaceholderGrass(Vector3 position)
    {
        GameObject grass = new GameObject("Grass (Placeholder)");
        grass.transform.position = position;
        grass.transform.SetParent(grassParent);
        
        // Create multiple grass blades
        for (int i = 0; i < Random.Range(3, 7); i++)
        {
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.transform.SetParent(grass.transform);
            blade.transform.localPosition = new Vector3(
                Random.Range(-0.3f, 0.3f),
                0.3f,
                Random.Range(-0.3f, 0.3f)
            );
            blade.transform.localScale = new Vector3(0.1f, 0.6f, 0.1f);
            blade.GetComponent<Renderer>().material.color = new Color(0.1f, Random.Range(0.7f, 1f), 0.1f);
        }
        
        return grass;
    }
    
    GameObject CreatePlaceholderFlower(Vector3 position)
    {
        GameObject flower = new GameObject("Flower (Placeholder)");
        flower.transform.position = position;
        flower.transform.SetParent(flowersParent);
        
        // Stem
        GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stem.transform.SetParent(flower.transform);
        stem.transform.localPosition = Vector3.up * 0.3f;
        stem.transform.localScale = new Vector3(0.05f, 0.3f, 0.05f);
        stem.GetComponent<Renderer>().material.color = new Color(0.1f, 0.6f, 0.1f);
        
        // Flower head
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.transform.SetParent(flower.transform);
        head.transform.localPosition = Vector3.up * 0.6f;
        head.transform.localScale = Vector3.one * 0.2f;
        
        Color[] flowerColors = { Color.red, Color.yellow, Color.magenta, Color.white, Color.cyan };
        head.GetComponent<Renderer>().material.color = flowerColors[Random.Range(0, flowerColors.Length)];
        
        return flower;
    }
    
    [ContextMenu("Create Nature Map Summary")]
    public void CreateNatureMapSummary()
    {
        if (natureParent == null)
        {
            Debug.Log("No nature scene found. Please organize assets first.");
            return;
        }
        
        int treeCount = treesParent.childCount;
        int rockCount = rocksParent.childCount;
        int grassCount = grassParent.childCount;
        int flowerCount = flowersParent.childCount;
        int structureCount = structuresParent.childCount;
        int terrainCount = terrainParent.childCount;
        
        Debug.Log($"=== LOWPOLY NATURE SCENE SUMMARY ===");
        Debug.Log($"🌲 Trees: {treeCount}");
        Debug.Log($"🪨 Rocks & Cliffs: {rockCount}");
        Debug.Log($"🌱 Grass & Plants: {grassCount}");
        Debug.Log($"🌸 Flowers: {flowerCount}");
        Debug.Log($"🏗️ Structures: {structureCount}");
        Debug.Log($"🏔️ Terrain: {terrainCount}");
        Debug.Log($"Total Nature Elements: {treeCount + rockCount + grassCount + flowerCount + structureCount + terrainCount}");
    }
}
