using UnityEngine;
using System.Collections;

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stopDistance = 0.1f;
    
    private PathManager pathManager;
    private int currentPointIndex = 0;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private Rigidbody rb;
    
    public float MoveSpeed 
    { 
        get => moveSpeed; 
        set => moveSpeed = Mathf.Max(0, value); 
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Rigidbodyの設定
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.freezeRotation = true;
        rb.mass = 1f;
        rb.linearDamping = 2f; // 空気抵抗を増やして安定化
        rb.angularDamping = 10f;
        
        // Colliderの確認と追加
        EnsureCollider();
        
        if (pathManager != null)
        {
            StartMovement();
        }
    }
    
    private void EnsureCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            // Colliderがない場合はCapsule Colliderを追加
            CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
            capsule.radius = 0.5f;
            capsule.height = 2f;
            capsule.center = new Vector3(0, 1f, 0);
        }
        
        // Colliderがトリガーになっていないことを確認
        GetComponent<Collider>().isTrigger = false;
    }
    
    public void SetPath(PathManager path)
    {
        pathManager = path;
        currentPointIndex = 0;
        
        if (pathManager != null && pathManager.PathLength > 0)
        {
            StartMovement();
        }
    }
    
    private void StartMovement()
    {
        if (pathManager == null || pathManager.PathLength == 0)
        {
            Debug.LogWarning($"Monster {gameObject.name}: No valid path found!");
            return;
        }
        
        isMoving = true;
        SetNextTarget();
    }
    
    private void Update()
    {
        if (!isMoving || pathManager == null) return;
        
        MoveTowardsTarget();
    }
    
    private void MoveTowardsTarget()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(targetPosition.x, currentPos.y, targetPosition.z); // Y軸は現在の高さを維持
        
        if (Vector3.Distance(currentPos, targetPos) <= stopDistance)
        {
            // 現在のポイントに到達
            OnReachedWaypoint();
            return;
        }
        
        // ターゲットに向かって移動（Y軸は除外）
        Vector3 direction = (targetPos - currentPos).normalized;
        Vector3 velocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
        rb.linearVelocity = velocity;
        
        // 移動方向を向く
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void OnReachedWaypoint()
    {
        // ウェイポイントでの待機時間チェック
        Transform currentPoint = pathManager.GetPoint(currentPointIndex);
        if (currentPoint != null)
        {
            Waypoint waypoint = currentPoint.GetComponent<Waypoint>();
            if (waypoint != null && waypoint.WaitTime > 0)
            {
                StartCoroutine(WaitAtWaypoint(waypoint.WaitTime));
                return;
            }
        }
        
        // 次のポイントへ
        MoveToNextPoint();
    }
    
    private IEnumerator WaitAtWaypoint(float waitTime)
    {
        isMoving = false;
        yield return new WaitForSeconds(waitTime);
        isMoving = true;
        MoveToNextPoint();
    }
    
    private void MoveToNextPoint()
    {
        currentPointIndex++;
        
        if (currentPointIndex >= pathManager.PathLength)
        {
            // パス終了 - World Treeに到達
            OnReachedDestination();
        }
        else
        {
            SetNextTarget();
        }
    }
    
    private void SetNextTarget()
    {
        if (currentPointIndex < pathManager.PathLength)
        {
            targetPosition = pathManager.GetPointPosition(currentPointIndex);
        }
    }
    
    private void OnReachedDestination()
    {
        Debug.Log($"Monster {gameObject.name} reached World Tree!");
        
        // World Treeにダメージを与える処理を探す
        TargetPoint targetPoint = FindObjectOfType<TargetPoint>();
        if (targetPoint != null)
        {
            targetPoint.OnMonsterReached(gameObject);
        }
        else
        {
            // TargetPointが見つからない場合は直接削除
            Destroy(gameObject);
        }
    }
    
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Max(0, newSpeed);
    }
    
    public void StopMovement()
    {
        isMoving = false;
    }
    
    public void ResumeMovement()
    {
        isMoving = true;
    }
    
    // デバッグ用
    private void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
        }
    }
}
