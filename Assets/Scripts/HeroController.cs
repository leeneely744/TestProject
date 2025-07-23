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
    private Animation animationComponent;
    private bool isMoving;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("Added Rigidbody component to hero");
        }
        
        // Try to get Animator first, if not available, use Animation component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animationComponent = GetComponent<Animation>();
            if (animationComponent == null)
            {
                animationComponent = gameObject.AddComponent<Animation>();
                Debug.Log("Added Animation component to hero");
            }
        }
        
        SetupAnimation();
    }
    
    void SetupAnimation()
    {
        if (runningAnimation != null)
        {
            if (animationComponent != null)
            {
                // Add the animation clip to the Animation component
                animationComponent.AddClip(runningAnimation, "Running");
                animationComponent.wrapMode = WrapMode.Loop;
                Debug.Log("Fire_Running animation added to Animation component");
            }
        }
        else
        {
            Debug.LogWarning("Please assign Fire_Running.anim to the Running Animation field in the inspector!");
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
        if (runningAnimation == null) return;
        
        if (animationComponent != null)
        {
            if (isMoving)
            {
                // Play running animation if not already playing
                if (!animationComponent.IsPlaying("Running"))
                {
                    animationComponent.Play("Running");
                }
            }
            else
            {
                // Stop running animation when not moving
                if (animationComponent.IsPlaying("Running"))
                {
                    animationComponent.Stop("Running");
                }
            }
        }
        else if (animator != null)
        {
            // If using Animator component instead
            animator.SetBool("IsMoving", isMoving);
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