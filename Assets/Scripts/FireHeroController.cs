using UnityEngine;

public class FireHeroController : MonoBehaviour
{
    [Header("Animation Control")]
    public string fireAttackAnimationName = "Fire_Attack";
    public KeyCode attackKey = KeyCode.Space;
    
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float maxSpeed = 8f;
    
    [Header("Auto Bounce")]
    public bool enableAutoBounce = true;
    public float autoBounceDelay = 2f;
    
    private Animator animator;
    private Rigidbody rb;
    private bool isOnTrampoline = false;
    private float lastBounceTime = 0f;
    private float highestPoint = 0f;
    private bool isAttacking = false;
    private float attackCooldown = 0f;
    private float attackCooldownTime = 1f;
    
    void Start()
    {
        // Get components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        // Add Rigidbody if it doesn't exist
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.linearDamping = 0.1f;
            rb.mass = 1f;
            rb.angularDamping = 0.1f;
        }
        
        if (animator == null)
        {
            Debug.LogError("❌ No Animator component found on " + gameObject.name);
        }
        else
        {
            Debug.Log("🔥 Fire Hero Controller started!");
        }
    }
    
    void Update()
    {
        HandleMovement();
        HandleFireAttack();
        TrackHeight();
        UpdateCooldown();
        
        // Auto bounce when on trampoline
        if (enableAutoBounce && isOnTrampoline && Time.time - lastBounceTime > autoBounceDelay)
        {
            if (Mathf.Abs(rb.linearVelocity.y) < 1f && !isAttacking)
            {
                PerformJump();
                Debug.Log("🔥 Fire Hero auto bounce!");
            }
        }
    }
    
    void HandleMovement()
    {
        // Don't move while attacking
        if (isAttacking) return;
        
        Vector3 moveDirection = Vector3.zero;
        
        // Movement keys
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveDirection.z += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveDirection.z -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveDirection.x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveDirection.x += 1f;
        
        // Apply movement
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            
            // Rotate to face movement direction
            transform.rotation = Quaternion.LookRotation(moveDirection);
            
            // Apply movement force
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(moveDirection * moveSpeed * Time.deltaTime * 100f);
            }
            
            // Set movement animation parameter if available
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
            }
        }
        else
        {
            // Set idle animation
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }
    
    void HandleFireAttack()
    {
        // Check for Space key press and cooldown
        if (Input.GetKeyDown(attackKey) && attackCooldown <= 0f && !isAttacking)
        {
            TriggerFireAttack();
        }
    }
    
    void TriggerFireAttack()
    {
        if (animator != null)
        {
            // Try different ways to trigger the animation
            
            // Method 1: Direct animation play
            animator.Play(fireAttackAnimationName);
            
            // Method 2: Trigger parameter (if exists)
            if (HasParameter("FireAttack"))
            {
                animator.SetTrigger("FireAttack");
            }
            
            // Method 3: Bool parameter (if exists)
            if (HasParameter("IsAttacking"))
            {
                animator.SetBool("IsAttacking", true);
            }
            
            isAttacking = true;
            attackCooldown = attackCooldownTime;
            
            Debug.Log("🔥 FIRE ATTACK activated!");
            
            // Stop the attack after animation duration
            Invoke("StopAttack", 1f); // Adjust timing based on your animation length
        }
        else
        {
            Debug.LogWarning("❌ No Animator component found!");
        }
    }
    
    void StopAttack()
    {
        isAttacking = false;
        
        if (animator != null && HasParameter("IsAttacking"))
        {
            animator.SetBool("IsAttacking", false);
        }
        
        Debug.Log("🔥 Fire attack finished");
    }
    
    void UpdateCooldown()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }
    
    void PerformJump()
    {
        if (!isAttacking) // Don't jump while attacking
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastBounceTime = Time.time;
        }
    }
    
    void TrackHeight()
    {
        if (transform.position.y > highestPoint)
        {
            highestPoint = transform.position.y;
        }
    }
    
    // Helper method to check if animator has a parameter
    bool HasParameter(string paramName)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "TrampolineSurface")
        {
            isOnTrampoline = true;
            Debug.Log("🔥 Fire Hero landed on trampoline!");
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "TrampolineSurface")
        {
            isOnTrampoline = false;
            Debug.Log("🔥 Fire Hero left trampoline!");
        }
    }
    
    void OnGUI()
    {
        // Fire Hero Game UI
        GUI.Box(new Rect(10, 10, 320, 160), "🔥 Fire Hero Trampoline Game");
        GUI.Label(new Rect(20, 35, 280, 20), "WASD/Arrows: Move Fire Hero");
        GUI.Label(new Rect(20, 55, 280, 20), "SPACE: Fire Attack! 🔥");
        GUI.Label(new Rect(20, 75, 280, 20), $"Speed: {(rb != null ? rb.linearVelocity.magnitude : 0):F1} m/s");
        GUI.Label(new Rect(20, 95, 280, 20), $"Highest Jump: {highestPoint:F1} m");
        GUI.Label(new Rect(20, 115, 280, 20), $"On Trampoline: {isOnTrampoline}");
        GUI.Label(new Rect(20, 135, 280, 20), $"Attack Cooldown: {Mathf.Max(0, attackCooldown):F1}s");
        
        // Control buttons
        if (GUI.Button(new Rect(10, 180, 150, 40), "🔥 Fire Attack"))
        {
            if (attackCooldown <= 0f && !isAttacking)
                TriggerFireAttack();
        }
        
        if (GUI.Button(new Rect(170, 180, 120, 40), "Reset Hero"))
        {
            transform.position = new Vector3(0, 5, 0);
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            transform.rotation = Quaternion.identity;
            highestPoint = 0f;
            isAttacking = false;
            attackCooldown = 0f;
            
            Debug.Log("🔥 Fire Hero reset!");
        }
    }
}