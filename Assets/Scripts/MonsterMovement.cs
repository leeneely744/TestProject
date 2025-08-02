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
    
    public float MoveSpeed 
    { 
        get => moveSpeed; 
        set => moveSpeed = Mathf.Max(0, value); 
    }
    
    private void Start()
    {
        if (pathManager != null)
        {
            StartMovement();
        }
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
        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            // 現在のポイントに到達
            OnReachedWaypoint();
            return;
        }
        
        // ターゲットに向かって移動
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
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
