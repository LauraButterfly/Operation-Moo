using UnityEngine;

// Controls the cow's idle, walking, and sleeping behavior.
public class CowBehavior : MonoBehaviour
{
    // Movement speed used while the cow is walking.
    public float moveSpeed = 1f;

    public float minMoveTime = 1f;
    public float maxMoveTime = 4f;

    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    public float minSleepTime = 3f;
    public float maxSleepTime = 6f;

    [Range(0f, 1f)]
    public float sleepChance = 0.1f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Current direction of movement for the walking state.
    private Vector2 moveDirection;
    // Time left before the cow changes behavior.
    private float stateTimer;

    // Current AI state.
    private CowState currentState;
    // Last facing direction used for animation and sleep pose.
    private FacingDirection facingDirection = FacingDirection.Down;

    // Simple behavior states for the cow AI.
    enum CowState
    {
        Idle,
        Walking,
        Sleeping
    }

    enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    void Start()
    {
        // Cache the components we need to move and animate the cow.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Begin in an idle state.
        StartIdle();
    }

    void Update()
    {
        // Count down until the current behavior should end.
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            switch (currentState)
            {
                case CowState.Walking:
                // After walking, the cow may sleep or idle.
                ChooseAfterWalking();
                break;

                case CowState.Idle:
                    // Idle time is over; choose the next action.
                    ChooseNextAction();
                    break;

                case CowState.Sleeping:
                    // Sleep ends; return to idle.
                    StartIdle();
                    break;
            }
        }
    }

    void FixedUpdate()
    {
        // Apply movement only while the cow is walking.
        if (currentState == CowState.Walking)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void ChooseNextAction()
    {
        // Randomly choose whether the cow should sleep or walk next.
        float randomValue = Random.value;

        if (randomValue < sleepChance)
        {
            StartSleeping();
        }
        else
        {
            StartWalking();
        }
    }

    void StartIdle()
    {
        // Pause in place and wait for the next action.
        currentState = CowState.Idle;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);

        PlayIdleAnimation();
    }

    void StartWalking()
    {
        // Pick a random direction and begin moving.
        currentState = CowState.Walking;
        stateTimer = Random.Range(minMoveTime, maxMoveTime);

        int direction = Random.Range(0, 4);

        switch (direction)
        {
            case 0:
                moveDirection = Vector2.up;
                facingDirection = FacingDirection.Up;
                animator.Play("Cow_Walk_Up");
                spriteRenderer.flipX = false;
                break;

            case 1:
                moveDirection = Vector2.down;
                facingDirection = FacingDirection.Down;
                animator.Play("Cow_Walk_Down");
                spriteRenderer.flipX = false;
                break;

            case 2:
                moveDirection = Vector2.left;
                facingDirection = FacingDirection.Left;
                animator.Play("Cow_Walk_Left");
                spriteRenderer.flipX = false;
                break;

            case 3:
                moveDirection = Vector2.right;
                facingDirection = FacingDirection.Right;
                animator.Play("Cow_Walk_Left");
                spriteRenderer.flipX = true;
                break;
        }
    }

    void StartSleeping()
    {
        // Enter the sleep state and choose the correct sleeping animation.
        currentState = CowState.Sleeping;
        stateTimer = Random.Range(minSleepTime, maxSleepTime);

        if (facingDirection == FacingDirection.Left)
        {
            spriteRenderer.flipX = false;
            animator.Play("Cow_Sleep");
        }
        else if (facingDirection == FacingDirection.Right)
        {
            spriteRenderer.flipX = true;
            animator.Play("Cow_Sleep");
        }
        else
        {
            spriteRenderer.flipX = false;
            animator.Play("Cow_Sleep_Down");
        }
    }

    void PlayIdleAnimation()
    {
        // Use the last facing direction to play the correct idle pose.
        if (facingDirection == FacingDirection.Left)
        {
            spriteRenderer.flipX = false;
            animator.Play("Cow_Idle_Left");
        }
        else if (facingDirection == FacingDirection.Right)
        {
            spriteRenderer.flipX = true;
            animator.Play("Cow_Idle_Left");
        }
        else
        {
            spriteRenderer.flipX = false;
            animator.Play("Cow_Idle_Down");
        }
    }

    void ChooseAfterWalking()
{
    float randomValue = Random.value;

    if (randomValue < sleepChance)
    {
        StartSleeping();
    }
    else
    {
        StartIdle();
    }
}
}