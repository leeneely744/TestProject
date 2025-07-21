using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // degrees per second
    
    [Header("Input Settings")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    
    private Vector3 moveDirection;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is required for HeroController!");
        }
    }
    
    void Update()
    {
        HandleInput();
        RotateHero();
    }
    
    void FixedUpdate()
    {
        MoveHero();
    }
    
    void HandleInput()
    {
        moveDirection = Vector3.zero;
        
        // Handle movement input
        if (Input.GetKey(leftKey))
        {
            moveDirection += Vector3.left; // X direction negative
        }
        if (Input.GetKey(rightKey))
        {
            moveDirection += Vector3.right; // X direction positive
        }
        if (Input.GetKey(upKey))
        {
            moveDirection += Vector3.forward; // Z direction positive
        }
        if (Input.GetKey(downKey))
        {
            moveDirection += Vector3.back; // Z direction negative
        }
        
        // Normalize diagonal movement
        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }
    }
    
    void MoveHero()
    {
        if (rb != null && moveDirection != Vector3.zero)
        {
            Vector3 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + movement);
        }
    }
    
    void RotateHero()
    {
        if (moveDirection != Vector3.zero)
        {
            // Calculate target rotation based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            
            // Smoothly rotate towards the target direction
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
}

// Alternative version using instant rotation instead of smooth rotation
public class HeroControllerInstantRotation : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Input Settings")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    
    private Vector3 moveDirection;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        HandleInput();
        RotateHeroInstantly();
    }
    
    void FixedUpdate()
    {
        MoveHero();
    }
    
    void HandleInput()
    {
        moveDirection = Vector3.zero;
        
        if (Input.GetKey(leftKey))
        {
            moveDirection += Vector3.left;
        }
        if (Input.GetKey(rightKey))
        {
            moveDirection += Vector3.right;
        }
        if (Input.GetKey(upKey))
        {
            moveDirection += Vector3.forward;
        }
        if (Input.GetKey(downKey))
        {
            moveDirection += Vector3.back;
        }
        
        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }
    }
    
    void MoveHero()
    {
        if (rb != null && moveDirection != Vector3.zero)
        {
            Vector3 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + movement);
        }
    }
    
    void RotateHeroInstantly()
    {
        if (moveDirection != Vector3.zero)
        {
            // Instantly rotate to face movement direction
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
}