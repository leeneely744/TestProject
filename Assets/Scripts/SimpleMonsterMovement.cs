using UnityEngine;
using System.Collections;

public class SimpleMonsterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stopDistance = 0.5f;
    
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
        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetPosition;
        
        // Y座標は地面に固定（簡単なアプローチ）
        targetPos.y = currentPos.y;
        
        float distance = Vector3.Distance(currentPos, targetPos);
        
        if (distance <= stopDistance)
        {
            OnReachedWaypoint();
            return;
        }
        
        // 移動
        Vector3 direction = (targetPos - currentPos).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
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
        
        TargetPoint targetPoint = FindObjectOfType<TargetPoint>();
        if (targetPoint != null)
        {
            targetPoint.OnMonsterReached(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
