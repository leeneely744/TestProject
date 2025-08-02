using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField] private List<Transform> pathPoints = new List<Transform>();
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color pathColor = Color.yellow;
    [SerializeField] private float gizmoSphereSize = 0.5f;
    
    public List<Transform> PathPoints => pathPoints;
    public int PathLength => pathPoints.Count;
    
    private void Awake()
    {
        // 子オブジェクトからパスポイントを自動収集
        if (pathPoints.Count == 0)
        {
            CollectPathPointsFromChildren();
        }
    }
    
    public void CollectPathPointsFromChildren()
    {
        pathPoints.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Point") || child.name.StartsWith("Waypoint"))
            {
                pathPoints.Add(child);
            }
        }
        
        // 名前順でソート
        pathPoints.Sort((a, b) => a.name.CompareTo(b.name));
    }
    
    public Transform GetPoint(int index)
    {
        if (index >= 0 && index < pathPoints.Count)
        {
            return pathPoints[index];
        }
        return null;
    }
    
    public Vector3 GetPointPosition(int index)
    {
        Transform point = GetPoint(index);
        return point != null ? point.position : Vector3.zero;
    }
    
    public Vector3 GetDirection(int fromIndex, int toIndex)
    {
        if (fromIndex >= 0 && fromIndex < pathPoints.Count && 
            toIndex >= 0 && toIndex < pathPoints.Count)
        {
            return (pathPoints[toIndex].position - pathPoints[fromIndex].position).normalized;
        }
        return Vector3.forward;
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos || pathPoints.Count < 2) return;
        
        Gizmos.color = pathColor;
        
        // パスポイントを球で表示
        foreach (Transform point in pathPoints)
        {
            if (point != null)
            {
                Gizmos.DrawSphere(point.position, gizmoSphereSize);
            }
        }
        
        // パスライン描画
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
            }
        }
    }
}
