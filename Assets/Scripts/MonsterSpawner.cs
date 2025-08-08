using UnityEngine;
using System.Collections;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private PathManager pathManager;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxMonsters = 10;
    [SerializeField] private bool autoStart = true;
    
    [Header("Wave Settings")]
    [SerializeField] private int monstersPerWave = 5;
    [SerializeField] private float timeBetweenWaves = 10f;
    
    private int spawnedMonsters = 0;
    private int currentWave = 1;
    private bool isSpawning = false;
    
    private void Start()
    {
        Debug.Log("MonsterSpawner Start() called");
        Debug.Log($"MonsterPrefab: {(monsterPrefab != null ? monsterPrefab.name : "NULL")}");
        Debug.Log($"PathManager: {(pathManager != null ? pathManager.name : "NULL")}");
        Debug.Log($"AutoStart: {autoStart}");
        
        if (autoStart)
        {
            StartSpawning();
        }
        else
        {
            Debug.Log("AutoStart is disabled");
        }
    }
    
    public void StartSpawning()
    {
        if (!isSpawning && pathManager != null)
        {
            StartCoroutine(SpawnWave());
        }
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }
    
    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        Debug.Log($"Wave {currentWave} started!");
        
        for (int i = 0; i < monstersPerWave && spawnedMonsters < maxMonsters; i++)
        {
            SpawnMonster();
            yield return new WaitForSeconds(spawnInterval);
        }
        
        Debug.Log($"Wave {currentWave} completed!");
        currentWave++;
        
        // 次のウェーブまで待機
        yield return new WaitForSeconds(timeBetweenWaves);
        
        // まだモンスターを生成する余地があれば次のウェーブを開始
        if (spawnedMonsters < maxMonsters)
        {
            StartCoroutine(SpawnWave());
        }
        else
        {
            isSpawning = false;
            Debug.Log("All monsters spawned!");
        }
    }
    
    private void SpawnMonster()
    {
        Debug.Log("SpawnMonster() called");
        Debug.Log($"MonsterPrefab: {(monsterPrefab != null ? "Found" : "NULL")}");
        Debug.Log($"PathManager: {(pathManager != null ? "Found" : "NULL")}");
        Debug.Log($"PathLength: {(pathManager != null ? pathManager.PathLength : 0)}");
        
        if (monsterPrefab == null || pathManager == null || pathManager.PathLength == 0)
        {
            Debug.LogWarning("Cannot spawn monster: missing prefab or path!");
            Debug.LogWarning($"  MonsterPrefab null: {monsterPrefab == null}");
            Debug.LogWarning($"  PathManager null: {pathManager == null}");
            Debug.LogWarning($"  PathLength: {(pathManager != null ? pathManager.PathLength : 0)}");
            return;
        }
        
        Vector3 spawnPosition = pathManager.GetPointPosition(0);
        Debug.Log($"Spawning monster at position: {spawnPosition}");
        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        
        // モンスターにパス情報を渡す
        MonsterMovement movement = monster.GetComponent<MonsterMovement>();
        SimpleMonsterMovement simpleMovement = monster.GetComponent<SimpleMonsterMovement>();
        
        if (movement != null)
        {
            movement.SetPath(pathManager);
            Debug.Log("Path assigned to MonsterMovement component");
        }
        else if (simpleMovement != null)
        {
            simpleMovement.SetPath(pathManager);
            Debug.Log("Path assigned to SimpleMonsterMovement component");
        }
        else
        {
            Debug.LogWarning($"No movement component found on {monster.name}!");
        }
        
        spawnedMonsters++;
        Debug.Log($"Monster spawned! Total: {spawnedMonsters}/{maxMonsters}");
    }
    
    public void SetMonsterPrefab(GameObject prefab)
    {
        monsterPrefab = prefab;
    }
    
    public void SetPathManager(PathManager path)
    {
        pathManager = path;
    }
}
