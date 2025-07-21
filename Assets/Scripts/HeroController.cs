using UnityEngine;

public class HeroController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool useRigidbody = true;
    
    private Rigidbody2D rb2d;
    private Vector3 movement;
    
    void Start()
    {
        // Try to get Rigidbody2D component
        rb2d = GetComponent<Rigidbody2D>();
        
        // If no Rigidbody2D found, add one
        if (rb2d == null && useRigidbody)
        {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
            rb2d.gravityScale = 0f; // For top-down movement
        }
    }
    
    void Update()
    {
        // Get input from WASD keys
        float horizontal = 0f;
        float forward = 0f;
        
        // Check for WASD input
        if (Input.GetKey(KeyCode.A))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D))
            horizontal = 1f;
            
        if (Input.GetKey(KeyCode.W))
            forward = 1f;   // W key moves forward (positive Z)
        else if (Input.GetKey(KeyCode.S))
            forward = -1f;  // S key moves backward (negative Z)
        
        // Store movement vector (X, Y, Z)
        movement = new Vector3(horizontal, 0f, forward).normalized;
    }
    
    void FixedUpdate()
    {
        // Move the hero
        if (useRigidbody && rb2d != null)
        {
            // For 2D Rigidbody, convert 3D movement to 2D
            Vector2 movement2D = new Vector2(movement.x, movement.z);
            rb2d.MovePosition(rb2d.position + movement2D * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Use Transform for direct 3D movement
            transform.position += movement * moveSpeed * Time.fixedDeltaTime;
        }
    }
    
    // Optional: Visual feedback in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        if (movement != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, movement);
        }
    }
}