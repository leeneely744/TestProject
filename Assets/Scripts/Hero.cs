using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{
    [Header("Hero Stats")]
    public int attackPower = 5;
    public float moveSpeed = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    
    [Header("Visual Effects")]
    public GameObject attackEffect;
    public GameObject levelUpEffect;
    
    private Camera mainCamera;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private float lastAttackTime;
    private Enemy targetEnemy;
    
    void Start()
    {
        mainCamera = Camera.main;
        targetPosition = transform.position;
    }
    
    void Update()
    {
        if (!GameManager.Instance.gameActive) return;
        
        HandleInput();
        MoveToTarget();
        FindAndAttackEnemies();
    }
    
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            
            targetPosition = worldPos;
            isMoving = true;
        }
    }
    
    void MoveToTarget()
    {
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
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
        
        // Stop moving when attacking
        isMoving = false;
    }
    
    public void UpgradeAttackPower(int upgrade)
    {
        attackPower += upgrade;
        
        if (levelUpEffect)
        {
            Instantiate(levelUpEffect, transform.position, Quaternion.identity);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check for weapon upgrades
        Weapon weapon = other.GetComponent<Weapon>();
        if (weapon != null && weapon.CanUpgrade())
        {
            int upgradeCost = weapon.GetUpgradeCost();
            if (GameManager.Instance.SpendCoins(upgradeCost))
            {
                weapon.Upgrade();
                UpgradeAttackPower(1); // Hero also gets stronger
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw target position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetPosition, 0.2f);
    }
}