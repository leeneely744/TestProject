using UnityEngine;

public class BattlefieldTerrainSetup : MonoBehaviour
{
    [Header("Battlefield Settings")]
    public bool setupOnStart = true;
    public string battlefieldName = "Hero Battlefield";
    
    [Header("Terrain Properties")]
    public bool adjustTerrainSettings = true;
    public float terrainWidth = 100f;
    public float terrainLength = 100f;
    public float terrainHeight = 30f;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupBattlefield();
        }
    }
    
    [ContextMenu("Setup Battlefield Terrain")]
    public void SetupBattlefield()
    {
        // Find existing terrain
        Terrain terrain = FindObjectOfType<Terrain>();
        
        if (terrain != null)
        {
            SetupUnityTerrain(terrain);
        }
        else
        {
            // Look for Lowpoly nature terrain objects
            GameObject terrainObj = GameObject.Find("terrain");
            if (terrainObj != null)
            {
                SetupLowpolyTerrain(terrainObj);
            }
            else
            {
                Debug.LogWarning("No terrain found to set up as battlefield!");
            }
        }
    }
    
    void SetupUnityTerrain(Terrain terrain)
    {
        Debug.Log("Setting up Unity Terrain as battlefield...");
        
        terrain.name = battlefieldName;
        
        if (adjustTerrainSettings)
        {
            TerrainData terrainData = terrain.terrainData;
            
            // Adjust terrain size for battlefield
            terrainData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);
            
            // Make terrain more suitable for battles
            terrainData.heightmapResolution = 513; // Higher resolution for detail
            
            Debug.Log($"Terrain configured as battlefield: {terrainWidth}x{terrainLength}m, height: {terrainHeight}m");
        }
        
        // Position terrain at world center
        terrain.transform.position = new Vector3(-terrainWidth/2, 0, -terrainLength/2);
        
        // Add battlefield-specific components if needed
        SetupBattlefieldComponents(terrain.gameObject);
    }
    
    void SetupLowpolyTerrain(GameObject terrainObj)
    {
        Debug.Log("Setting up Lowpoly terrain as battlefield...");
        
        terrainObj.name = battlefieldName;
        
        // Scale the lowpoly terrain appropriately
        terrainObj.transform.localScale = Vector3.one * 2f; // Make it bigger for battles
        terrainObj.transform.position = Vector3.zero; // Center it
        
        // Add battlefield-specific components
        SetupBattlefieldComponents(terrainObj);
    }
    
    void SetupBattlefieldComponents(GameObject terrainObj)
    {
        // Add a collider if it doesn't have one
        if (terrainObj.GetComponent<Collider>() == null && terrainObj.GetComponent<Terrain>() == null)
        {
            MeshCollider collider = terrainObj.AddComponent<MeshCollider>();
            Debug.Log("Added MeshCollider to battlefield terrain");
        }
        
        // Tag as battlefield
        terrainObj.tag = "Ground"; // Useful for hero movement scripts
        
        // Add a marker component for identification
        if (terrainObj.GetComponent<BattlefieldMarker>() == null)
        {
            terrainObj.AddComponent<BattlefieldMarker>();
        }
    }
    
    [ContextMenu("Remove Ground Planes")]
    public void RemoveGroundPlanes()
    {
        // Find and remove any plane objects that might be acting as ground
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        int removedCount = 0;
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("plane") && obj.name.ToLower().Contains("ground"))
            {
                Debug.Log($"Removing ground plane: {obj.name}");
                DestroyImmediate(obj);
                removedCount++;
            }
            else if (obj.GetComponent<MeshFilter>() != null)
            {
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter.sharedMesh != null && meshFilter.sharedMesh.name == "Plane")
                {
                    // This is likely a ground plane
                    if (obj.name.ToLower().Contains("ground") || obj.transform.localScale.x > 10)
                    {
                        Debug.Log($"Removing suspected ground plane: {obj.name}");
                        DestroyImmediate(obj);
                        removedCount++;
                    }
                }
            }
        }
        
        Debug.Log($"Removed {removedCount} ground plane objects");
    }
    
    [ContextMenu("Get Battlefield Info")]
    public void GetBattlefieldInfo()
    {
        Terrain terrain = FindObjectOfType<Terrain>();
        
        if (terrain != null)
        {
            TerrainData data = terrain.terrainData;
            Debug.Log($"=== BATTLEFIELD TERRAIN INFO ===");
            Debug.Log($"Name: {terrain.name}");
            Debug.Log($"Size: {data.size.x}x{data.size.z}m");
            Debug.Log($"Height: {data.size.y}m");
            Debug.Log($"Position: {terrain.transform.position}");
            Debug.Log($"Heightmap Resolution: {data.heightmapResolution}");
        }
        else
        {
            GameObject terrainObj = GameObject.Find("terrain");
            if (terrainObj != null)
            {
                Debug.Log($"=== LOWPOLY BATTLEFIELD INFO ===");
                Debug.Log($"Name: {terrainObj.name}");
                Debug.Log($"Position: {terrainObj.transform.position}");
                Debug.Log($"Scale: {terrainObj.transform.localScale}");
                Debug.Log($"Has Collider: {terrainObj.GetComponent<Collider>() != null}");
            }
            else
            {
                Debug.Log("No battlefield terrain found!");
            }
        }
    }
}

// Marker component to identify battlefield terrain
public class BattlefieldMarker : MonoBehaviour
{
    [Header("Battlefield Properties")]
    public bool isMainBattlefield = true;
    public string battlefieldType = "Nature";
    
    void Start()
    {
        Debug.Log($"Battlefield {gameObject.name} is ready for combat!");
    }
}
