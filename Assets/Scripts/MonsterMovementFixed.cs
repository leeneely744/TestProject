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
        Debug.Log($"MonsterMovement Start() on {gameObject.name}");
        
        if (pathManager != null)
        {
            Debug.Log($"PathManager found: {pathManager.name}");
            StartMovement();
        }
        else
        {
            Debug.LogWarning($"PathManager is null on {gameObject.name}");
        }
    }
    
    public void SetPath(PathManager path)
    {
        Debug.Log($"SetPath called with: {(path != null ? path.name : "NULL")}");
        pathManager = path;
        currentPointIndex = 0;
        
        if (pathManager != null && pathManager.PathLength > 0)
        {
            Debug.Log($"Starting movement with path length: {pathManager.PathLength}");
            StartMovement();
        }
        else
        {
            Debug.LogWarning($"Invalid path: PathManager={pathManager != null}, PathLength={pathManager?.PathLength ?? 0}");
        }
    }
    
    private void StartMovement()
    {
        Debug.Log($"StartMovement called on {gameObject.name}");
        
        if (pathManager == null || pathManager.PathLength == 0)
        {
            Debug.LogWarning($"Monster {gameObject.name}: No valid path found!");
            Debug.LogWarning($"  PathManager null: {pathManager == null}");
            Debug.LogWarning($"  PathLength: {pathManager?.PathLength ?? 0}");
            return;
        }
        
        isMoving = true;
        SetNextTarget();
        Debug.Log($"Movement started. Moving to point {currentPointIndex}: {targetPosition}");
    }
    
    private void Update()
    {
        if (!isMoving || pathManager == null) return;
        
        MoveTowardsTarget();
    }
    
    private void MoveTowardsTarget()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetPosition;
        
        // Y座標は地面に固定（簡単なアプローチ）
        targetPos.y = currentPos.y;
        
        float distance = Vector3.Distance(currentPos, targetPos);
        
        if (distance <= stopDistance)
        {
            Debug.Log($"Reached waypoint {currentPointIndex}");
            OnReachedWaypoint();
            return;
        }
        
        // 移動
        Vector3 direction = (targetPos - currentPos).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // 地面の高さをレイキャストで調整
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 5f))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + 0.1f;
            transform.position = pos;
        }
        
        // 回転
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void OnReachedWaypoint()
    {
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
            Debug.Log($"Reached destination! Calling OnReachedDestination()");
            OnReachedDestination();
        }
        else
        {
            SetNextTarget();
            Debug.Log($"Moving to next point {currentPointIndex}: {targetPosition}");
        }
    }
    
    private void SetNextTarget()
    {
        if (currentPointIndex < pathManager.PathLength)
        {
            targetPosition = pathManager.GetPointPosition(currentPointIndex);
            Debug.Log($"Target set to point {currentPointIndex}: {targetPosition}");
        }
    }
    
    private void OnReachedDestination()
    {
        Debug.Log($"Monster {gameObject.name} reached World Tree!");
        
        TargetPoint targetPoint = FindObjectOfType<TargetPoint>();
        if (targetPoint != null)
        {
            targetPoint.OnMonsterReached(gameObject);
        }
        else
        {
            Debug.LogWarning("No TargetPoint found, destroying monster directly");
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
        Debug.Log($"Movement stopped for {gameObject.name}");
    }
    
    public void ResumeMovement()
    {
        isMoving = true;
        Debug.Log($"Movement resumed for {gameObject.name}");
    }
    
    // デバッグ用
    private void OnDrawGizmos()
    {
        if (isMoving && pathManager != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
            
            // 現在のターゲットを強調
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetPosition, 0.1f);
        }
    }
}
