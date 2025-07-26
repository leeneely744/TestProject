using UnityEngine;

public class MaterialFixer : MonoBehaviour
{
    [Header("Material Fix Settings")]
    public bool fixOnStart = true;
    public bool createNewMaterials = true;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixAllMaterials();
        }
    }
    
    [ContextMenu("Fix All Purple Materials")]
    public void FixAllMaterials()
    {
        Debug.Log("🔧 Starting material fix for all objects...");
        
        // Get all renderers in the scene
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        int fixedCount = 0;
        
        foreach (Renderer renderer in allRenderers)
        {
            if (HasPurpleMaterial(renderer))
            {
                FixRenderer(renderer);
                fixedCount++;
            }
        }
        
        Debug.Log($"✅ Fixed {fixedCount} objects with purple materials!");
    }
    
    bool HasPurpleMaterial(Renderer renderer)
    {
        // Check if any materials are missing or using the default magenta shader
        foreach (Material mat in renderer.materials)
        {
            if (mat == null || mat.shader.name.Contains("Hidden/InternalErrorShader"))
            {
                return true;
            }
        }
        return false;
    }
    
    void FixRenderer(Renderer renderer)
    {
        GameObject obj = renderer.gameObject;
        string objName = obj.name.ToLower();
        
        // Create appropriate materials based on object type
        if (objName.Contains("tree") && !objName.Contains("pine"))
        {
            FixTreeMaterials(renderer);
        }
        else if (objName.Contains("pine"))
        {
            FixPineTreeMaterials(renderer);
        }
        else if (objName.Contains("rock"))
        {
            FixRockMaterials(renderer);
        }
        else if (objName.Contains("grass"))
        {
            FixGrassMaterials(renderer);
        }
        else if (objName.Contains("flower"))
        {
            FixFlowerMaterials(renderer);
        }
        else if (objName.Contains("mashroom") || objName.Contains("mushroom"))
        {
            FixMushroomMaterials(renderer);
        }
        else if (objName.Contains("water"))
        {
            FixWaterMaterials(renderer);
        }
        else if (objName.Contains("bridge"))
        {
            FixWoodMaterials(renderer);
        }
        else if (objName.Contains("cliff"))
        {
            FixCliffMaterials(renderer);
        }
        else if (objName.Contains("terrain"))
        {
            FixTerrainMaterials(renderer);
        }
        else
        {
            // Default fix
            FixGenericMaterials(renderer);
        }
        
        Debug.Log($"🎨 Fixed materials for: {obj.name}");
    }
    
    void FixTreeMaterials(Renderer renderer)
    {
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            if (i == 0) // Usually trunk
            {
                newMaterials[i] = CreateMaterial("TreeTrunk", new Color(0.4f, 0.2f, 0.1f));
            }
            else // Usually leaves
            {
                newMaterials[i] = CreateMaterial("TreeLeaves", new Color(0.1f, 0.7f, 0.1f));
            }
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixPineTreeMaterials(Renderer renderer)
    {
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            if (i == 0) // Usually trunk
            {
                newMaterials[i] = CreateMaterial("PineTrunk", new Color(0.3f, 0.15f, 0.05f));
            }
            else // Usually pine needles
            {
                newMaterials[i] = CreateMaterial("PineNeedles", new Color(0.05f, 0.4f, 0.1f));
            }
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixRockMaterials(Renderer renderer)
    {
        Material rockMat = CreateMaterial("Rock", new Color(0.5f, 0.5f, 0.5f));
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = rockMat;
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixGrassMaterials(Renderer renderer)
    {
        Material grassMat = CreateMaterial("Grass", new Color(0.1f, 0.8f, 0.1f));
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = grassMat;
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixFlowerMaterials(Renderer renderer)
    {
        Color[] flowerColors = {
            Color.red, Color.yellow, Color.magenta, Color.white, Color.cyan, Color.blue
        };
        
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            if (i == 0) // Usually stem
            {
                newMaterials[i] = CreateMaterial("FlowerStem", new Color(0.1f, 0.6f, 0.1f));
            }
            else // Usually petals
            {
                Color flowerColor = flowerColors[Random.Range(0, flowerColors.Length)];
                newMaterials[i] = CreateMaterial("FlowerPetals", flowerColor);
            }
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixMushroomMaterials(Renderer renderer)
    {
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            if (i == 0) // Usually stem
            {
                newMaterials[i] = CreateMaterial("MushroomStem", new Color(0.9f, 0.9f, 0.8f));
            }
            else // Usually cap
            {
                newMaterials[i] = CreateMaterial("MushroomCap", new Color(0.8f, 0.3f, 0.1f));
            }
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixWaterMaterials(Renderer renderer)
    {
        Material waterMat = CreateWaterMaterial();
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = waterMat;
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixWoodMaterials(Renderer renderer)
    {
        Material woodMat = CreateMaterial("Wood", new Color(0.6f, 0.4f, 0.2f));
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = woodMat;
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixCliffMaterials(Renderer renderer)
    {
        Material cliffMat = CreateMaterial("Cliff", new Color(0.4f, 0.35f, 0.3f));
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = cliffMat;
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixTerrainMaterials(Renderer renderer)
    {
        Material terrainMat = CreateMaterial("Terrain", new Color(0.3f, 0.5f, 0.2f));
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = terrainMat;
        }
        
        renderer.materials = newMaterials;
    }
    
    void FixGenericMaterials(Renderer renderer)
    {
        Material genericMat = CreateMaterial("Generic", new Color(0.7f, 0.7f, 0.7f));
        Material[] newMaterials = new Material[renderer.materials.Length];
        
        for (int i = 0; i < newMaterials.Length; i++)
        {
            newMaterials[i] = genericMat;
        }
        
        renderer.materials = newMaterials;
    }
    
    Material CreateMaterial(string name, Color color)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = name + "_Fixed";
        mat.color = color;
        
        // Make materials look better
        mat.SetFloat("_Smoothness", 0.1f); // Less shiny
        mat.SetFloat("_Metallic", 0.0f);   // Not metallic
        
        return mat;
    }
    
    Material CreateWaterMaterial()
    {
        Material waterMat = new Material(Shader.Find("Standard"));
        waterMat.name = "Water_Fixed";
        waterMat.color = new Color(0.2f, 0.6f, 1f, 0.7f);
        
        // Make water transparent
        waterMat.SetFloat("_Mode", 3); // Transparent mode
        waterMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        waterMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        waterMat.SetInt("_ZWrite", 0);
        waterMat.DisableKeyword("_ALPHATEST_ON");
        waterMat.EnableKeyword("_ALPHABLEND_ON");
        waterMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        waterMat.renderQueue = 3000;
        
        waterMat.SetFloat("_Smoothness", 0.9f); // Very smooth
        waterMat.SetFloat("_Metallic", 0.1f);   // Slightly metallic
        
        return waterMat;
    }
    
    [ContextMenu("Reset All Materials to Default")]
    public void ResetAllMaterials()
    {
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        
        foreach (Renderer renderer in allRenderers)
        {
            Material defaultMat = CreateMaterial("Default", Color.white);
            Material[] newMaterials = new Material[renderer.materials.Length];
            
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = defaultMat;
            }
            
            renderer.materials = newMaterials;
        }
        
        Debug.Log("Reset all materials to default white");
    }
}
