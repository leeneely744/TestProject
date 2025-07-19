using UnityEngine;
using System.Collections;

public enum EnemyType
{
    Orc,
    Ogre,
    Boss
}

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public EnemyType enemyType;
    public int maxHealth;
    public int currentHealth;
    public int attackPower;
    public float moveSpeed = 2f;
    public int coinValue = 1;
    
    [Header("Pathfinding")]
    public Transform[] waypoints;
    public int currentWaypointIndex = 0;
    
    [Header("Combat")]
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    private float lastAttackTime;
    
    [Header("Visual Effects")]
    public GameObject deathEffect;
    public GameObject hitEffect;
    
    private bool isMoving = true;
    private Castle targetCastle;
    
    void Start()
    {
        SetupEnemyStats();
        targetCastle = FindObjectOfType<Castle>();
        
        // Find waypoints if not assigned
        if (waypoints == null || waypoints.Length == 0)
        {
            GameObject waypointContainer = GameObject.Find("Waypoints");
            if (waypointContainer)
            {
                waypoints = new Transform[waypointContainer.transform.childCount];
                for (int i = 0; i < waypoints.Length; i++)
                {
                    waypoints[i] = waypointContainer.transform.GetChild(i);
                }
            }
        }
    }
    
    void SetupEnemyStats()
    {
        switch (enemyType)
        {
            case EnemyType.Orc:
                maxHealth = 5;
                attackPower = 5;
                moveSpeed = 2f;
                coinValue = 1;
                break;
            case EnemyType.Ogre:
                maxHealth = 20;
                attackPower = 10;
                moveSpeed = 1.5f;
                coinValue = 2;
                break;
            case EnemyType.Boss:
                maxHealth = 80;
                attackPower = 20;
                moveSpeed = 1f;
                coinValue = 5;
                break;
        }
        
        currentHealth = maxHealth;
    }
    
    void Update()
    {
        if (isMoving)
        {
            MoveAlongPath();
        }
        
        // Check if close to castle for attack
        if (targetCastle && Vector2.Distance(transform.position, targetCastle.transform.position) <= attackRange)
        {
            AttackCastle();
        }
    }
    
    void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        
        if (currentWaypointIndex < waypoints.Length)
        {
            Vector2 targetPosition = waypoints[currentWaypointIndex].position;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // Reached the end, attack castle
            isMoving = false;
        }
    }
    
    void AttackCastle()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            if (targetCastle)
            {
                targetCastle.TakeDamage(attackPower);
                Destroy(gameObject); // Enemy dies after attacking castle
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (hitEffect)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // Drop coins
        GameManager.Instance.AddCoins(coinValue);
        
        // Notify game manager
        GameManager.Instance.EnemyDefeated();
        
        // Spawn death effect
        if (deathEffect)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw path
        if (waypoints != null && waypoints.Length > 1)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] && waypoints[i + 1])
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }
    }
}