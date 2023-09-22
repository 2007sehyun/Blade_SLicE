using UnityEngine;

public class GptMove : MonoBehaviour
{
    public Transform playerCam;
    public Transform orientation;
    private Collider playerCollider;
    public Rigidbody rb;
    public LayerMask whatIsGround;
    public LayerMask whatIsWallrunnable;
    public float sensitivity = 50f;
    public float moveSpeed = 4500f;
    public float walkSpeed = 20f;
    public float runSpeed = 10f;
    public float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    private float x;
    private float y;
    private float xRotation;
    private float desiredX;
    private float wallRunRotation;
    private float actualWallRotation;
    private float wallRotationVel;
    private float sensMultiplier = 1f;
    private bool readyToJump;
    private bool jumping;
    private bool sprinting;
    private bool crouching;
    private bool wallRunning;
    private bool cancelling;
    private bool readyToWallrun = true;
    private bool grounded;
    private bool onWall;
    private bool airborne;
    private bool onGround;
    private bool surfing;
    private bool cancellingGrounded;
    private bool cancellingWall;
    private bool cancellingSurf;
    private Vector3 grapplePoint;
    private Vector3 normalVector;
    private Vector3 wallNormalVector;
    private Vector3 wallRunPos;
    private Vector3 previousLookdir;
    private int nw;

    public static GptMove Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerCollider = GetComponent<Collider>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        readyToJump = true;
        wallNormalVector = Vector3.up;
    }

    private void LateUpdate()
    {
        WallRunning();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        if (transform.position.y <= -7)
            transform.position = Vector3.zero;
        MyInput();
        Look();
    }

    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            StopCrouch();
    }

    private void StartCrouch()
    {
        float num = 400f;
        transform.localScale = new Vector3(1f, 0.5f, 1f);
        transform.position -= new Vector3(0f, 0.5f, 0f);

        if (rb.velocity.magnitude > 0.1f && grounded)
            rb.AddForce(orientation.forward * num);
    }

    private void StopCrouch()
    {
        transform.localScale = new Vector3(1f, 1.5f, 1f);
        transform.position += new Vector3(0f, 0.5f, 0f);
    }

    private void Movement()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 10f);
        Vector2 mag = FindVelRelativeToLook();
        CounterMovement(x, y, mag);

        if (readyToJump && jumping)
            Jump();

        float speed = crouching && grounded && readyToJump ? walkSpeed : (sprinting ? runSpeed : walkSpeed);

        if (x > 0f && mag.x > speed)
            x = 0f;
        if (x < 0f && mag.x < -speed)
            x = 0f;
        if (y > 0f && mag.y > speed)
            y = 0f;
        if (y < 0f && mag.y < -speed)
            y = 0f;

        float moveMultiplier = grounded ? 1f : 0.5f;
        float airMultiplier = grounded ? 0.5f : 0.3f;

        rb.AddForce(orientation.forward * y * moveSpeed * Time.deltaTime * moveMultiplier);
        rb.AddForce(orientation.right * x * moveSpeed * Time.deltaTime * moveMultiplier);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Jump()
    {
        if ((grounded || wallRunning || surfing) && readyToJump)
        {
            Debug.Log("Jumping");
            Vector3 velocity = rb.velocity;
            readyToJump = false;
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
            else if (rb.velocity.y > 0f)
                rb.velocity = new Vector3(velocity.x, velocity.y / 2f, velocity.z);

            if (wallRunning)
                rb.AddForce(wallNormalVector * jumpForce * 3f);

            Invoke("ResetJump", jumpCooldown);

            if (wallRunning)
                wallRunning = false;
        }
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        desiredX = playerCam.transform.localRotation.eulerAngles.y + mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        FindWallRunRotation();
        actualWallRotation = Mathf.SmoothDamp(actualWallRotation, wallRunRotation, ref wallRotationVel, 0.2f);
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, actualWallRotation);
        orientation.transform.localRotation = Quaternion.Euler(0f, desiredX, 0f);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping)
            return;

        float slideMultiplier = crouching ? moveSpeed : 1f;

        if ((Mathf.Abs(mag.x) > 0.01f && Mathf.Abs(x) < 0.05f) || (mag.x < -0.01f && x > 0f) || (mag.x > 0.01f && x < 0f))
            rb.AddForce(orientation.right * -mag.x * moveSpeed * Time.deltaTime * 0.16f * slideMultiplier);

        if ((Mathf.Abs(mag.y) > 0.01f && Mathf.Abs(y) < 0.05f) || (mag.y < -0.01f && y > 0f) || (mag.y > 0.01f && y < 0f))
            rb.AddForce(orientation.forward * -mag.y * moveSpeed * Time.deltaTime * 0.16f * slideMultiplier);

        float currentSpeed = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2f) + Mathf.Pow(rb.velocity.z, 2f));

        if (currentSpeed > walkSpeed)
        {
            float newY = rb.velocity.y;
            Vector3 newVelocity = rb.velocity.normalized * walkSpeed;
            rb.velocity = new Vector3(newVelocity.x, newY, newVelocity.z);
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        float current = orientation.eulerAngles.y;
        float target = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;
        float angle = Mathf.DeltaAngle(current, target);
        float angle2 = 90f - angle;
        float magnitude = rb.velocity.magnitude;

        return new Vector2(magnitude * Mathf.Cos(angle * Mathf.Deg2Rad), magnitude * Mathf.Cos(angle2 * Mathf.Deg2Rad));
    }

    private void FindWallRunRotation()
    {
        if (!wallRunning)
        {
            wallRunRotation = 0f;
            return;
        }

        float current = playerCam.transform.rotation.eulerAngles.y;
        float num = Vector3.SignedAngle(Vector3.forward, wallNormalVector, Vector3.up);
        float angleDelta = Mathf.DeltaAngle(current, num);

        wallRunRotation = (-angleDelta / 90f) * 15f;

        if (!readyToWallrun)
            return;

        if ((Mathf.Abs(wallRunRotation) < 4f && y > 0f && Mathf.Abs(x) < 0.1f) || (Mathf.Abs(wallRunRotation) > 22f && y < 0f && Mathf.Abs(x) < 0.1f))
        {
            if (!cancelling)
            {
                cancelling = true;
                CancelInvoke("CancelWallrun");
                Invoke("CancelWallrun", 0.2f);
            }
        }
        else
        {
            cancelling = false;
            CancelInvoke("CancelWallrun");
        }
    }

    private void CancelWallrun()
    {
        Debug.Log("Cancelled");
        Invoke("GetReadyToWallrun", 0.1f);
        rb.AddForce(wallNormalVector * 600f);
        readyToWallrun = false;
    }

    private void GetReadyToWallrun()
    {
        readyToWallrun = true;
    }

    private void WallRunning()
    {
        if (wallRunning)
        {
            rb.AddForce(-wallNormalVector * Time.deltaTime * moveSpeed);
            rb.AddForce(Vector3.up * Time.deltaTime * rb.mass * 100f );
        }
    }
}
