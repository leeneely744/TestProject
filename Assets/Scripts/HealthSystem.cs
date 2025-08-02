using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    
    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnDeath;
    
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0;
    
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    
    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        if (!IsAlive) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"{gameObject.name} healed {healAmount}. Health: {currentHealth}/{maxHealth}");
    }
    
    private void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        OnDeath?.Invoke();
    }
    
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
}
