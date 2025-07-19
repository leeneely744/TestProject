using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game Settings")]
    public float stageDuration = 180f; // 3 minutes
    public int stageNumber = 1;
    public int totalStages = 10;
    
    [Header("UI References")]
    public Text timerText;
    public Text coinsText;
    public Text stageText;
    public Text castleHealthText;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    
    [Header("Game State")]
    public int playerCoins = 0;
    public bool gameActive = true;
    
    private float timeRemaining;
    private Castle castle;
    private int enemiesRemaining;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        timeRemaining = stageDuration;
        castle = FindObjectOfType<Castle>();
        enemiesRemaining = GetTotalEnemiesForStage();
        UpdateUI();
    }
    
    void Update()
    {
        if (!gameActive) return;
        
        timeRemaining -= Time.deltaTime;
        
        if (timeRemaining <= 0)
        {
            GameOver();
        }
        
        UpdateUI();
    }
    
    public void AddCoins(int amount)
    {
        playerCoins += amount;
        UpdateUI();
    }
    
    public bool SpendCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }
    
    public void EnemyDefeated()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            StageComplete();
        }
    }
    
    void StageComplete()
    {
        gameActive = false;
        if (stageNumber >= totalStages)
        {
            Victory();
        }
        else
        {
            // Load next stage
            stageNumber++;
            // Reset for next stage
            StartCoroutine(LoadNextStage());
        }
    }
    
    IEnumerator LoadNextStage()
    {
        yield return new WaitForSeconds(2f);
        timeRemaining = stageDuration;
        enemiesRemaining = GetTotalEnemiesForStage();
        gameActive = true;
        // Spawn enemies for next stage
        FindObjectOfType<EnemySpawner>().StartSpawning();
    }
    
    void GameOver()
    {
        gameActive = false;
        gameOverPanel.SetActive(true);
    }
    
    void Victory()
    {
        gameActive = false;
        victoryPanel.SetActive(true);
    }
    
    void UpdateUI()
    {
        if (timerText) timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
        if (coinsText) coinsText.text = "Coins: " + playerCoins.ToString();
        if (stageText) stageText.text = "Stage: " + stageNumber.ToString();
        if (castleHealthText && castle) castleHealthText.text = "Castle HP: " + castle.currentHealth.ToString();
    }
    
    int GetTotalEnemiesForStage()
    {
        // Simple formula: more enemies each stage
        return 10 + (stageNumber * 5);
    }
    
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}