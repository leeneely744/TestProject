using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private bool isSpawnPoint = false;
    [SerializeField] private bool isEndPoint = false;
    [SerializeField] private float waitTime = 0f; // モンスターがここで待機する時間
    
    public bool IsSpawnPoint => isSpawnPoint;
    public bool IsEndPoint => isEndPoint;
    public float WaitTime => waitTime;
    
    private void OnDrawGizmos()
    {
        // スポーンポイントは緑、エンドポイントは赤、通常は青
        if (isSpawnPoint)
        {
            Gizmos.color = Color.green;
        }
        else if (isEndPoint)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
        
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        
        // 名前表示（エディター内）
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, gameObject.name);
        #endif
    }
}
