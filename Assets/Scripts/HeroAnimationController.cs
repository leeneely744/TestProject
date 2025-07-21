using UnityEngine;
using UnityEngine.Animations;

public class HeroAnimationController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    
    [Header("Animation Settings")]
    public AnimationClip runningAnimation; // Drag Fire_Running.anim here
    public string runningStateName = "Running";
    
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
        // Get or add required components
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("Added Rigidbody component to hero");
        }
        
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
            Debug.Log("Added Animator component to hero");
        }
        
        // Set up animation controller if we have the animation clip
        SetupAnimationController();
    }
    
    void SetupAnimationController()
    {
        if (runningAnimation != null)
        {
            // Create a runtime animator controller
            UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/HeroAnimatorController.controller");
            
            if (controller != null)
            {
                // Add the running animation to the controller
                var stateMachine = controller.layers[0].stateMachine;
                var runningState = stateMachine.AddState(runningStateName);
                runningState.motion = runningAnimation;
                
                // Add parameters
                controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);
                
                // Set up transitions
                var idleState = stateMachine.AddState("Idle");
                
                // Transition from Idle to Running
                var toRunning = idleState.AddTransition(runningState);
                toRunning.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "IsMoving");
                toRunning.hasExitTime = false;
                
                // Transition from Running to Idle
                var toIdle = runningState.AddTransition(idleState);
                toIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "IsMoving");
                toIdle.hasExitTime = false;
                
                // Assign the controller to the animator
                animator.runtimeAnimatorController = controller;
                
                Debug.Log("Animation controller created and assigned successfully!");
            }
        }
        else
        {
            Debug.LogWarning("Running animation clip not assigned! Please drag Fire_Running.anim to the Running Animation field.");
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
    
    void RotateHero()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }
}

// Simplified version without editor code (for runtime use)
public class HeroAnimationControllerSimple : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    
    [Header("Animation Settings")]
    public RuntimeAnimatorController animatorController; // Assign your animator controller here
    
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
        }
        
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
        
        if (animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
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
    
    void RotateHero()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }
}