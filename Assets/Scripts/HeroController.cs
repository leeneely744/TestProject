using UnityEngine;

public class HeroController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool useRigidbody = true;
    
    private Rigidbody2D rb2d;
    private Vector2 movement;
    
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
        float vertical = 0f;
        
        // Check for WASD input
        if (Input.GetKey(KeyCode.A))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D))
            horizontal = 1f;
            
        if (Input.GetKey(KeyCode.W))
            vertical = 1f;
        else if (Input.GetKey(KeyCode.S))
            vertical = -1f;
        
        // Store movement vector
        movement = new Vector2(horizontal, vertical).normalized;
    }
    
    void FixedUpdate()
    {
        // Move the hero
        if (useRigidbody && rb2d != null)
        {
            // Use Rigidbody2D for physics-based movement
            rb2d.MovePosition(rb2d.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Use Transform for direct movement
            transform.position += (Vector3)(movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
    
    // Optional: Visual feedback in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        if (movement != Vector2.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, movement);
        }
    }
}