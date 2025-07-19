using UnityEngine;

public class RobotControllerScript : MonoBehaviour
{
    [Header("Movement")]
    public float moveForce = 10f;
    public float jumpForce = 15f;
    public float maxSpeed = 8f;
    
    [Header("Auto Bounce")]
    public bool enableAutoBounce = true;
    public float autoBounceDelay = 2f;
    
    [Header("Robot Animation")]
    public Transform robotHead;
    public Transform robotArmLeft;
    public Transform robotArmRight;
    public float headRotationSpeed = 50f;
    public float armSwingSpeed = 2f;
    
    private Rigidbody rb;
    private bool isOnTrampoline = false;
    private float lastBounceTime = 0f;
    private float highestPoint = 0f;
    private float armSwingTime = 0f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = false;
        
        // Auto-find robot parts if not assigned
        if (robotHead == null)
            robotHead = GameObject.Find("RobotHead")?.transform;
        if (robotArmLeft == null)
            robotArmLeft = GameObject.Find("RobotArmLeft")?.transform;
        if (robotArmRight == null)
            robotArmRight = GameObject.Find("RobotArmRight")?.transform;
        
        Debug.Log("🤖 Robot Controller started - Use WASD to move, Space to jump!");
    }
    
    void Update()
    {
        HandleMovement();
        HandleJumping();
        TrackHeight();
        AnimateRobot();
        
        // Auto bounce when on trampoline
        if (enableAutoBounce && isOnTrampoline && Time.time - lastBounceTime > autoBounceDelay)
        {
            if (Mathf.Abs(rb.linearVelocity.y) < 1f)
            {
                PerformJump();
                Debug.Log("🤖 Robot auto bounce!");
            }
        }
    }
    
    void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;
        
        // Check movement keys
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveDirection.z += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveDirection.z -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveDirection.x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveDirection.x += 1f;
        
        // Apply movement force
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            
            // Limit max speed
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(moveDirection * moveForce * Time.deltaTime * 100f);
            }
        }
    }
    
    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformJump();
            Debug.Log("🤖 Robot manual jump!");
        }
    }
    
    void PerformJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        lastBounceTime = Time.time;
        
        // Robot jump animation - make head spin
        if (robotHead != null)
        {
            robotHead.Rotate(0, 360f, 0);
        }
    }
    
    void AnimateRobot()
    {
        armSwingTime += Time.deltaTime;
        
        // Animate robot head rotation when moving
        if (rb.linearVelocity.magnitude > 0.1f && robotHead != null)
        {
            robotHead.Rotate(0, headRotationSpeed * Time.deltaTime, 0);
        }
        
        // Animate robot arms swinging
        if (robotArmLeft != null && robotArmRight != null)
        {
            float armAngle = Mathf.Sin(armSwingTime * armSwingSpeed) * 20f;
            robotArmLeft.localRotation = Quaternion.Euler(armAngle, 0, 0);
            robotArmRight.localRotation = Quaternion.Euler(-armAngle, 0, 0);
        }
    }
    
    void TrackHeight()
    {
        if (transform.position.y > highestPoint)
        {
            highestPoint = transform.position.y;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "TrampolineSurface")
        {
            isOnTrampoline = true;
            Debug.Log("🤖 Robot landed on trampoline!");
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "TrampolineSurface")
        {
            isOnTrampoline = false;
            Debug.Log("🤖 Robot left trampoline!");
        }
    }
    
    void OnGUI()
    {
        // Robot Game UI
        GUI.Box(new Rect(10, 10, 300, 140), "🤖 Robot Trampoline Game");
        GUI.Label(new Rect(20, 35, 250, 20), "WASD/Arrows: Move Robot");
        GUI.Label(new Rect(20, 55, 250, 20), "SPACE: Robot Jump");
        GUI.Label(new Rect(20, 75, 250, 20), $"Robot Speed: {rb.linearVelocity.magnitude:F1} m/s");
        GUI.Label(new Rect(20, 95, 250, 20), $"Highest Jump: {highestPoint:F1} m");
        GUI.Label(new Rect(20, 115, 250, 20), $"On Trampoline: {isOnTrampoline}");
        
        // Robot control buttons
        if (GUI.Button(new Rect(10, 160, 150, 40), "🚀 Robot Jump"))
            PerformJump();
        
        if (GUI.Button(new Rect(170, 160, 120, 40), "Reset Robot"))
        {
            transform.position = new Vector3(0, 5, 0);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            highestPoint = 0f;
            
            // Reset robot parts rotation
            if (robotHead != null)
                robotHead.localRotation = Quaternion.identity;
            if (robotArmLeft != null)
                robotArmLeft.localRotation = Quaternion.identity;
            if (robotArmRight != null)
                robotArmRight.localRotation = Quaternion.identity;
                
            Debug.Log("🤖 Robot reset!");
        }
    }
}