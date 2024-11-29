using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool readyToJump;
    [Header("Dodging")]
    public float dodgeDistance;
    private bool isDodging;
    private bool isInvincible;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dodgeKey = KeyCode.LeftShift;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    private Rigidbody rb;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MovementInput();
        SpeedControl();

        //animator.SetBool("IsGround", grounded);

        if(grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        animator.SetFloat("Speed", speed);

        if(Input.GetKeyDown(dodgeKey) && !isDodging)
        {
            StartCoroutine(PerformDodge());
        }
    }

    private void FixedUpdate()
    {
        if(!isDodging)
        {
            MovePlayer();
        }
    }

    private void MovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        if (moveDirection.magnitude > 0.1f)
        {
            // Apply movement force
            if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
            else if (!grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if(flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x,0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        animator.SetTrigger("Jump");
    }

    private void ResetJump()
    {
        readyToJump = true; 
    }

    private IEnumerator PerformDodge()
{
    isDodging = true;
    isInvincible = true;

    animator.SetTrigger("Dodge");

    // Determine the dodge direction (default to forward if no input)
    Vector3 dodgeDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;
    if (dodgeDirection == Vector3.zero)
    {
        dodgeDirection = transform.forward; // Default to forward if no input
    }

    // Calculate dodge target position
    Vector3 startPosition = transform.position;
    Vector3 targetPosition = startPosition + dodgeDirection * dodgeDistance;

    // Sync dodge movement to animation duration
    float dodgeDuration = animator.GetCurrentAnimatorStateInfo(0).length; // Assumes the dodge animation is on Layer 0
    float elapsedTime = 0f;

    while (elapsedTime < dodgeDuration)
    {
        // Lerp position for smooth movement
        float progress = elapsedTime / dodgeDuration;
        transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Finalize position and reset states
    transform.position = targetPosition;
    isDodging = false;

    yield return new WaitForSeconds(dodgeDuration * 0.8f);
    isInvincible = false;
}


    private void OnDrawGizmos()
    {
        Gizmos.color = grounded ? Color.green : Color.red;

        Gizmos.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.2f));

        Gizmos.DrawSphere(transform.position + Vector3.down * (playerHeight * 0.5f + 0.2f), 0.05f);
    }
}
