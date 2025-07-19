using UnityEngine;

public class GUIControllerScript : MonoBehaviour
{
    [Header("Movement")]
    public float moveForce = 10f;
    public float jumpForce = 15f;
    public float maxSpeed = 8f;
    
    [Header("Auto Bounce")]
    public bool enableAutoBounce = true;
    public float autoBounceDelay = 2f;
    
    private Rigidbody rb;
    private bool isOnTrampoline = false;
    private float lastBounceTime = 0f;
    private float highestPoint = 0f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = false;
        Debug.Log("GUI Controller started - Use buttons to control!");
    }
    
    void Update()
    {
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
    
    public void PerformJump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        lastBounceTime = Time.time;
        Debug.Log("Jump performed!");
    }
    
    public void MoveForward()
    {
        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(Vector3.forward * moveForce, ForceMode.Impulse);
    }
    
    public void MoveBackward()
    {
        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(Vector3.back * moveForce, ForceMode.Impulse);
    }
    
    public void MoveLeft()
    {
        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(Vector3.left * moveForce, ForceMode.Impulse);
    }
    
    public void MoveRight()
    {
        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(Vector3.right * moveForce, ForceMode.Impulse);
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
        // Game UI with bigger buttons
        GUI.Box(new Rect(10, 10, 280, 200), "Trampoline Bounce Game");
        GUI.Label(new Rect(20, 35, 250, 20), "Use GUI buttons to control:");
        GUI.Label(new Rect(20, 55, 250, 20), $"Speed: {rb.linearVelocity.magnitude:F1} m/s");
        GUI.Label(new Rect(20, 75, 250, 20), $"Highest: {highestPoint:F1} m");
        GUI.Label(new Rect(20, 95, 250, 20), $"On Trampoline: {isOnTrampoline}");
        
        // Movement buttons
        if (GUI.Button(new Rect(130, 120, 60, 30), "↑ W"))
            MoveForward();
        if (GUI.Button(new Rect(60, 160, 60, 30), "← A"))
            MoveLeft();
        if (GUI.Button(new Rect(130, 160, 60, 30), "↓ S"))
            MoveBackward();
        if (GUI.Button(new Rect(200, 160, 60, 30), "→ D"))
            MoveRight();
        
        // Jump button (big and prominent)
        if (GUI.Button(new Rect(10, 220, 150, 40), "🚀 JUMP (Space)"))
            PerformJump();
        
        // Reset button
        if (GUI.Button(new Rect(170, 220, 120, 40), "Reset Position"))
        {
            transform.position = new Vector3(0, 5, 0);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            highestPoint = 0f;
            Debug.Log("Position reset!");
        }
    }
}