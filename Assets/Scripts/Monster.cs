using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Monster Stats")]
    [SerializeField] private int health = 100;
    [SerializeField] private int damage = 1;
    [SerializeField] private int goldReward = 10;
    
    private MonsterMovement movement;
    private int currentHealth;
    
    public int Health => currentHealth;
    public int MaxHealth => health;
    public int Damage => damage;
    public int GoldReward => goldReward;
    public bool IsAlive => currentHealth > 0;
    
    private void Awake()
    {
        currentHealth = health;
        movement = GetComponent<MonsterMovement>();
        
        // モンスタータグを設定（エラーハンドリング付き）
        try
        {
            gameObject.tag = "Monster";
        }
        catch (UnityException e)
        {
            Debug.LogWarning($"Monster tag not found: {e.Message}. Please create 'Monster' tag in Project Settings.");
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        if (!IsAlive) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        Debug.Log($"Monster {gameObject.name} took {damageAmount} damage. Health: {currentHealth}/{health}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        Debug.Log($"Monster {gameObject.name} defeated! Gold reward: {goldReward}");
        
        // TODO: ゴールド報酬の処理
        // GameManager.Instance.AddGold(goldReward);
        
        Destroy(gameObject);
    }
    
    public void SetStats(int newHealth, int newDamage, int newGoldReward)
    {
        health = newHealth;
        currentHealth = newHealth;
        damage = newDamage;
        goldReward = newGoldReward;
    }
    
    public void Heal(int healAmount)
    {
        if (!IsAlive) return;
        
        currentHealth = Mathf.Min(health, currentHealth + healAmount);
    }
    
    public void SlowDown(float slowFactor, float duration)
    {
        if (movement != null)
        {
            StartCoroutine(ApplySlowEffect(slowFactor, duration));
        }
    }
    
    private System.Collections.IEnumerator ApplySlowEffect(float slowFactor, float duration)
    {
        float originalSpeed = movement.MoveSpeed;
        movement.SetSpeed(originalSpeed * slowFactor);
        
        yield return new WaitForSeconds(duration);
        
        movement.SetSpeed(originalSpeed);
    }
}
