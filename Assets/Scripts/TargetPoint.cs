using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private bool isMainTarget = true;
    [SerializeField] private int damageToTarget = 1;
    
    private HealthSystem healthSystem;
    
    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError($"TargetPoint on {gameObject.name} requires a HealthSystem component!");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // モンスターがWorld Treeに到達した時の処理
        if (other.CompareTag("Monster"))
        {
            OnMonsterReached(other.gameObject);
        }
    }
    
    public void OnMonsterReached(GameObject monster)
    {
        Debug.Log($"Monster {monster.name} reached the target!");
        
        // World Treeにダメージを与える
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damageToTarget);
        }
        
        // モンスターを削除
        Destroy(monster);
    }
    
    public bool IsMainTarget => isMainTarget;
    public int DamagePerMonster => damageToTarget;
}
