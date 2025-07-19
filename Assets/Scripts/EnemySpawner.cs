using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject orcPrefab;
    public GameObject ogrePrefab;
    public GameObject bossPrefab;
    
    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public int enemiesPerStage = 15;
    
    [Header("Stage Configuration")]
    public AnimationCurve orcSpawnRate;
    public AnimationCurve ogreSpawnRate;
    public AnimationCurve bossSpawnRate;
    
    private int enemiesSpawned = 0;
    private bool isSpawning = false;
    
    void Start()
    {
        StartSpawning();
    }
    
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            enemiesSpawned = 0;
            isSpawning = true;
            StartCoroutine(SpawnEnemies());
        }
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }
    
    IEnumerator SpawnEnemies()
    {
        while (isSpawning && enemiesSpawned < enemiesPerStage && GameManager.Instance.gameActive)
        {
            SpawnRandomEnemy();
            enemiesSpawned++;
            
            // Gradually increase spawn rate
            float spawnRate = spawnInterval * (1f - (enemiesSpawned / (float)enemiesPerStage) * 0.5f);
            yield return new WaitForSeconds(spawnRate);
        }
        
        isSpawning = false;
    }
    
    void SpawnRandomEnemy()
    {
        GameObject enemyPrefab = SelectEnemyType();
        
        if (enemyPrefab && spawnPoint)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            
            // Set waypoints for the enemy
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript)
            {
                GameObject waypointContainer = GameObject.Find("Waypoints");
                if (waypointContainer)
                {
                    Transform[] waypoints = new Transform[waypointContainer.transform.childCount];
                    for (int i = 0; i < waypoints.Length; i++)
                    {
                        waypoints[i] = waypointContainer.transform.GetChild(i);
                    }
                    enemyScript.waypoints = waypoints;
                }
            }
        }
    }
    
    GameObject SelectEnemyType()
    {
        int stageNumber = GameManager.Instance.stageNumber;
        float stageProgress = stageNumber / 10f; // 0 to 1
        
        float random = Random.Range(0f, 1f);
        
        // Early stages: mostly orcs
        if (stageNumber <= 3)
        {
            if (random < 0.8f) return orcPrefab;
            else return ogrePrefab;
        }
        // Mid stages: mix of orcs and ogres
        else if (stageNumber <= 7)
        {
            if (random < 0.5f) return orcPrefab;
            else if (random < 0.9f) return ogrePrefab;
            else return bossPrefab;
        }
        // Late stages: more ogres and bosses
        else
        {
            if (random < 0.3f) return orcPrefab;
            else if (random < 0.7f) return ogrePrefab;
            else return bossPrefab;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (spawnPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
        }
    }
}