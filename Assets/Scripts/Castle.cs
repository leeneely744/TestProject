using UnityEngine;

public class Castle : MonoBehaviour
{
    [Header("Castle Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    
    [Header("Visual Effects")]
    public GameObject damageEffect;
    public GameObject destroyedEffect;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (damageEffect)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }
        
        if (currentHealth <= 0)
        {
            DestroyCastle();
        }
    }
    
    void DestroyCastle()
    {
        if (destroyedEffect)
        {
            Instantiate(destroyedEffect, transform.position, Quaternion.identity);
        }
        
        // Game Over
        GameManager.Instance.gameActive = false;
        // Show game over screen
        FindObjectOfType<GameManager>().gameOverPanel.SetActive(true);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            TakeDamage(enemy.attackPower);
            Destroy(other.gameObject);
        }
    }
}