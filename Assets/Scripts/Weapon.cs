using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public int attackPower = 5;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int activationCost = 2;
    
    [Header("Upgrade System")]
    public int upgradeLevel = 1;
    public int maxUpgradeLevel = 5;
    public int upgradeCost = 3;
    
    [Header("Visual Effects")]
    public GameObject attackEffect;
    public GameObject upgradeEffect;
    public SpriteRenderer weaponRenderer;
    public Color[] upgradeColors;
    
    [Header("Weapon State")]
    public bool isActive = false;
    public bool canUpgrade = true;
    
    private float lastAttackTime;
    private Enemy targetEnemy;
    
    void Start()
    {
        UpdateVisuals();
    }
    
    void Update()
    {
        if (!GameManager.Instance.gameActive || !isActive) return;
        
        FindAndAttackEnemies();
    }
    
    void FindAndAttackEnemies()
    {
        // Find nearest enemy in range
        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= attackRange && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        
        // Attack the nearest enemy
        if (nearestEnemy != null && Time.time - lastAttackTime >= attackCooldown)
        {
            AttackEnemy(nearestEnemy);
        }
    }
    
    void AttackEnemy(Enemy enemy)
    {
        lastAttackTime = Time.time;
        
        // Deal damage to enemy
        enemy.TakeDamage(attackPower);
        
        // Spawn attack effect
        if (attackEffect)
        {
            Vector3 effectPos = (transform.position + enemy.transform.position) / 2;
            Instantiate(attackEffect, effectPos, Quaternion.identity);
        }
    }
    
    public void Activate()
    {
        if (GameManager.Instance.SpendCoins(activationCost))
        {
            isActive = true;
            UpdateVisuals();
        }
    }
    
    public bool CanUpgrade()
    {
        return canUpgrade && upgradeLevel < maxUpgradeLevel;
    }
    
    public int GetUpgradeCost()
    {
        return upgradeCost * upgradeLevel;
    }
    
    public void Upgrade()
    {
        if (CanUpgrade())
        {
            upgradeLevel++;
            attackPower += 2;
            attackRange += 0.2f;
            
            if (upgradeEffect)
            {
                Instantiate(upgradeEffect, transform.position, Quaternion.identity);
            }
            
            UpdateVisuals();
            
            if (upgradeLevel >= maxUpgradeLevel)
            {
                canUpgrade = false;
            }
        }
    }
    
    void UpdateVisuals()
    {
        if (weaponRenderer && upgradeColors.Length > 0)
        {
            int colorIndex = Mathf.Clamp(upgradeLevel - 1, 0, upgradeColors.Length - 1);
            weaponRenderer.color = upgradeColors[colorIndex];
            
            // Make weapon more visible when active
            if (isActive)
            {
                weaponRenderer.color = Color.Lerp(weaponRenderer.color, Color.white, 0.3f);
            }
        }
    }
    
    void OnMouseDown()
    {
        if (!isActive && GameManager.Instance.gameActive)
        {
            Activate();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = isActive ? Color.red : Color.gray;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw upgrade level
        Gizmos.color = Color.yellow;
        for (int i = 0; i < upgradeLevel; i++)
        {
            Gizmos.DrawWireCube(transform.position + Vector3.up * (i * 0.3f + 1f), Vector3.one * 0.2f);
        }
    }
}