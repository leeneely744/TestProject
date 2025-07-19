using UnityEngine;

public class HumanoidControllerScript : MonoBehaviour
{
    [Header("Movement")]
    public float moveForce = 10f;
    public float jumpForce = 15f;
    public float maxSpeed = 8f;
    
    [Header("Auto Bounce")]
    public bool enableAutoBounce = true;
    public float autoBounceDelay = 2f;
    
    [Header("Manual Controls")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    
    private Rigidbody rb;
    private bool isOnTrampoline = false;
    private float lastBounceTime = 0f;
    private float highestPoint = 0f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = false;
        Debug.Log("HumanoidController started - Use WASD to move, Space to jump!");
    }
    
    void Update()
    {
        HandleMovementNewInput();
        HandleJumpingNewInput();
        TrackHeight();
        
        // Auto bounce when on trampoline
        if (enableAutoBounce && isOnTrampoline && Time.time - lastBounceTime > autoBounceDelay)
        {
            if (Mathf.Abs(rb.linearVelocity.y) < 1f)
            {
                PerformJump();
                Debug.Log("Auto bounce triggered!");
            }
        }
    }
    
    void HandleMovementNewInput()
    {
        Vector3 moveDirection = Vector3.zero;
        
        // Check each key individually
        if (Input.GetKey(upKey) || Input.GetKey(KeyCode.UpArrow))
            moveDirection.z += 1f;
        if (Input.GetKey(downKey) || Input.GetKey(KeyCode.DownArrow))
            moveDirection.z -= 1f;
        if (Input.GetKey(leftKey) || Input.GetKey(KeyCode.LeftArrow))
            moveDirection.x -= 1f;
        if (Input.GetKey(rightKey) || Input.GetKey(KeyCode.RightArrow))
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
    
    void HandleJumpingNewInput()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            PerformJump();
            Debug.Log("Manual jump performed!");
        }
    }
    
    void PerformJump()
    {
        // Add upward force regardless of current state
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        lastBounceTime = Time.time;
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
            Debug.Log("Landed on trampoline!");
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "TrampolineSurface")
        {
            isOnTrampoline = false;
            Debug.Log("Left trampoline!");
        }
    }
    
    void OnGUI()
    {
        // Game UI
        GUI.Box(new Rect(10, 10, 280, 140), "Trampoline Bounce Game");
        GUI.Label(new Rect(20, 35, 250, 20), "WASD/Arrows: Move");
        GUI.Label(new Rect(20, 55, 250, 20), "SPACE: Jump (works anytime!)");
        GUI.Label(new Rect(20, 75, 250, 20), $"Speed: {rb.linearVelocity.magnitude:F1} m/s");
        GUI.Label(new Rect(20, 95, 250, 20), $"Highest: {highestPoint:F1} m");
        GUI.Label(new Rect(20, 115, 250, 20), $"On Trampoline: {isOnTrampoline}");
        
        // Reset button
        if (GUI.Button(new Rect(10, 160, 120, 30), "Reset Position"))
        {
            transform.position = new Vector3(0, 5, 0);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            highestPoint = 0f;
            Debug.Log("Position reset!");
        }
        
        // Extra jump button for testing
        if (GUI.Button(new Rect(140, 160, 100, 30), "Force Jump"))
        {
            PerformJump();
        }
    }
}