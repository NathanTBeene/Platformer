using System;
using System.Collections;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float terminalVelocityY = 15f;

    [Header ("Coyote Time")]
    [SerializeField] private float coyoteTimeDuration = 0.15f;
    private Coroutine coyoteTimeCoroutine;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    public bool isOnFloor;
    public bool enableInputMovement = true;
    public bool canMove = true;
    public bool canJump = true;
    private float directionX;
    private float defaultJumpForce;

    public void SetJumpForce(float newJumpForce)
    {
        jumpForce = newJumpForce;
    }

    public void ResetJumpForce()
    {
        jumpForce = defaultJumpForce;
    }

    private int jumpPadCount = 0;
    private float currentJumpForce;
    private float originalJumpForce;

    void OnEnable()
    {
        InputManager.onMove += OnMove;
        InputManager.onJump += OnJump;
    }

    void OnDisable()
    {
        InputManager.onMove -= OnMove;
        InputManager.onJump -= OnJump;
    }

    void Start()
    {
        defaultJumpForce = jumpForce;
        originalJumpForce = jumpForce;
        currentJumpForce = jumpForce; // Initialize currentJumpForce!
    }

    void Update()
    {
        _movePlayer();
        _handleAnimation();
    }

    void FixedUpdate()
    {
        _checkIfGrounded();
    }

    private void _handleAnimation()
    {
        animator.SetBool("isOnFloor", isOnFloor);
        animator.SetFloat("velocityX", rb.linearVelocityX);
        animator.SetFloat("velocityY", rb.linearVelocityY);

        _flipSprite();
    }

    private void _flipSprite()
    {
        if (directionX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (directionX < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void _movePlayer()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (Math.Abs(directionX) > 0.01f)
        {
            if (isOnFloor)
                rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(directionX * moveSpeed * 0.8f, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (rb.linearVelocity.y < -terminalVelocityY)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -terminalVelocityY);
        }
    }

    private void _jump()
    {
        if (!canJump) return; // Add this check!
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
    }

    void OnMove(float direction)
    {
        if (!enableInputMovement) return;
        directionX = direction;
    }

    void OnJump()
    {
        if (!enableInputMovement) return;
        _jump();
    }

    private void _checkIfGrounded()
    {
        // Prevents coyote time activating when input movement is disabled
        // We don't need it when manually controlling the player.
        if (!enableInputMovement) return;

        isOnFloor = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (isOnFloor)
        {
            if (coyoteTimeCoroutine != null)
            {
                StopCoroutine(coyoteTimeCoroutine);
                coyoteTimeCoroutine = null;
            }
            setCanJump(true);
        }
        else
        {
            if (canJump && coyoteTimeCoroutine == null)
            {
                coyoteTimeCoroutine = StartCoroutine(_startCoyoteTimer(coyoteTimeDuration));
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    public void setCanMove(bool value) => canMove = value;
    public void setCanJump(bool value) => canJump = value;

    private IEnumerator _startCoyoteTimer(float duration)
    {
        Debug.Log("Coyote time started");
        setCanJump(true);
        yield return new WaitForSeconds(duration);
        Debug.Log("Coyote time ended");
        setCanJump(false);
        coyoteTimeCoroutine = null;
    }

    public void IncrementJumpPadCount(float jumpForce)
    {
        jumpPadCount++;
        if (jumpPadCount == 1)
        {
            currentJumpForce = jumpForce;
        }
    }

    public void DecrementJumpPadCount()
    {
        jumpPadCount--;
        if (jumpPadCount <= 0)
        {
            jumpPadCount = 0;
            currentJumpForce = originalJumpForce;
        }
    }

    // Movement control methods for external calls

    public void StartMovingLeft()
    {
        setCanMove(true);
        setCanJump(true);
        directionX = -1;
    }

    public void StartMovingRight()
    {
        setCanMove(true);
        setCanJump(true);
        directionX = 1;
    }

    public void StopMoving()
    {
        setCanMove(false);
        setCanJump(false);
        directionX = 0;
    }

    public void Jump()
    {
        _jump();
    }
}
