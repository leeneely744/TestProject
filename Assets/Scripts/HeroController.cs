using UnityEngine;

public class HeroController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // degrees per second
    
    [Header("Animation Settings")]
    public AnimationClip runningAnimation; // Drag Fire_Running.anim here
    
    [Header("Input Settings")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    
    private Vector3 moveDirection;
    private Rigidbody rb;
    private Animator animator;
    private bool isMoving;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("Added Rigidbody component to hero");
        }
        
        // Get the Animator component (should already exist with Fire_AC)
        animator = GetComponent<Animator>();
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            Debug.Log($"Using Animator Controller: {animator.runtimeAnimatorController.name}");
            
            // List all parameters in Fire_AC
            var parameters = animator.parameters;
            Debug.Log($"Fire_AC has {parameters.Length} parameters:");
            foreach (var param in parameters)
            {
                Debug.Log($"  - {param.name} ({param.type})");
            }
            
            Debug.Log("Fire_AC controller is loaded. We'll test which states work at runtime.");
        }
        else
        {
            Debug.LogError("No Animator or Animator Controller found!");
        }
        
        SetupAnimation();
    }
    
    void SetupAnimation()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            Debug.Log("Fire_AC controller is ready to use!");
        }
        else
        {
            Debug.LogWarning("Fire_AC controller not properly set up!");
        }
    }
    
    void Update()
    {
        HandleInput();
        UpdateAnimation();
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
        
        // Update movement state
        isMoving = moveDirection != Vector3.zero;
    }
    
    void MoveHero()
    {
        if (rb != null && moveDirection != Vector3.zero)
        {
            Vector3 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + movement);
        }
    }
    
    void UpdateAnimation()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // Your Fire_AC controller has: Blend (Float), Attack (Trigger), Jump (Trigger)
            // We'll use the Blend parameter to control movement animation
            
            if (isMoving)
            {
                // Set Blend to 1.0 when moving (this should trigger running animation)
                animator.SetFloat("Blend", 1.0f);
                Debug.Log("Setting Blend parameter to 1.0 (moving)");
            }
            else
            {
                // Set Blend to 0.0 when idle
                animator.SetFloat("Blend", 0.0f);
                Debug.Log("Setting Blend parameter to 0.0 (idle)");
            }
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