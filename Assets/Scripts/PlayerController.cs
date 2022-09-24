using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed = 5;
    public int jumpSpeed = 10;
    public float slideSpeed = 1;
    public float jumpOffSpeed = 0.5f;
    public float jumpOffTime = 0.25f;

    public LayerMask groundLayer;
    public Transform groundChecker;
    public Transform leftSideChecker;
    public Transform rightSideChecker;

    private bool doubleJumped;
    private float currentJumpOffTime;
    private bool jumpedOff;
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        currentJumpOffTime = jumpOffTime;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");

        HandleMovement(horizontalAxis);

        HandleJump(horizontalAxis);

        HandleSlide();
    }

    private void HandleMovement(float horizontalAxis)
    {
        int flipScale = 1;

        if (horizontalAxis > 0)
        {
            flipScale = 1;
        }
        else if (horizontalAxis < 0)
        {
            flipScale = -1;
        }

        if (jumpedOff)
        {
            if (horizontalAxis != 0)
            {
                transform.localScale = new Vector3(flipScale, transform.localScale.y, transform.localScale.z);
                rb.velocity = new Vector2(speed * horizontalAxis, rb.velocity.y);
            }
        }
        else
        {
            if (horizontalAxis != 0)
            {
                transform.localScale = new Vector3(flipScale, transform.localScale.y, transform.localScale.z);
            }

            rb.velocity = new Vector2(speed * horizontalAxis, rb.velocity.y);

            animator.SetBool("running", horizontalAxis != 0);
        }
    }

    private void HandleJump(float horizontalAxis)
    {

        if (IsGrounded())
        {
            animator.SetBool("fall", false);
            animator.SetBool("jump", false);
            animator.SetBool("sliding", false);

            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                animator.SetBool("running", horizontalAxis != 0);
                animator.SetBool("jump", true);
            }
            doubleJumped = false;
        }
        else if (!IsGrounded())
        {
            animator.SetBool("jump", rb.velocity.y >= 0);
            animator.SetBool("fall", rb.velocity.y < 0);

            if (!TouchesWall())
            {
                if (!doubleJumped && Input.GetButtonDown("Jump"))
                {
                    animator.SetBool("jump", false);
                    animator.SetBool("fall", false);
                    animator.SetTrigger("doubleJump");
                    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                    doubleJumped = true;

                }
            }
        }
    }

    private void HandleSlide()
    {
        if (!IsGrounded() && TouchesWall())
        {
            if (rb.velocity.y <= -slideSpeed)
            {
                doubleJumped = false;
                animator.SetBool("sliding", true);
                rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);

                if (Input.GetButtonDown("Jump"))
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                    rb.velocity = new Vector2(transform.localScale.x * jumpOffSpeed, jumpSpeed);
                    doubleJumped = true;
                    jumpedOff = true;
                    animator.SetBool("sliding", false);

                    animator.SetBool("doubleJump", true);
                }
            }
        }

        if (!TouchesWall())
        {
            animator.SetBool("sliding", false);
        }

        if (jumpedOff)
        {
            currentJumpOffTime -= Time.deltaTime;

            if (currentJumpOffTime <= 0)
            {
                currentJumpOffTime = jumpOffTime;

                animator.SetBool("doubleJump", false);

                animator.SetBool("jump", rb.velocity.y >= 0);

                animator.SetBool("fall", rb.velocity.y < 0);

                jumpedOff = false;
            }

            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        Collider2D collider = Physics2D.OverlapBox(groundChecker.position, new Vector2(0.8f, 0.1f), 0, groundLayer);
        return collider != null;
    }

    private bool TouchesWall()
    {
        Collider2D leftCollider = Physics2D.OverlapBox(leftSideChecker.transform.position, new Vector2(0.1f, 1f), 0, groundLayer);

        Collider2D rightCollider = Physics2D.OverlapBox(rightSideChecker.transform.position, new Vector2(0.1f, 1f), 0, groundLayer);

        return leftCollider != null || rightCollider != null;

    }
}
