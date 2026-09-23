using UnityEngine;

// Controls the cow's idle, walking, and sleeping behavior.
public class CowBehavior : MonoBehaviour
{
    // Movement speed used while the cow is walking.
    public float moveSpeed = 1f;

    // Minimum and maximum duration of each walking period.
    public float minMoveTime = 1f;
    public float maxMoveTime = 4f;

    // Minimum and maximum duration of each idle period.
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    // Minimum and maximum duration of each sleeping period.
    public float minSleepTime = 3f;
    public float maxSleepTime = 6f;

    // Chance of choosing sleep when the cow selects a new action.
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

    private bool isBeingAbducted = false;

    // Simple behavior states for the cow AI.
    enum CowState
    {
        Idle,
        Walking,
        Sleeping
    }

    // Used to keep the cow's idle and sleep poses facing the right way.
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
        // Stop normal AI updates while the cow is being abducted by the tractor beam.
        if (isBeingAbducted)
            return;

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
        // Freeze the cow immediately when the tractor beam begins abducting it.
        if (isBeingAbducted)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

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
        // When the cow finishes an idle period, choose a new behavior.
        // It will either rest or start wandering again.
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
        // The cow pauses in place and waits before selecting its next action.
        currentState = CowState.Idle;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);

        PlayIdleAnimation();
    }

    void StartWalking()
    {
        // Choose a random direction and begin moving in that direction.
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
        // Enter the sleep state and play the matching sleep animation based on facing direction.
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
        // Choose the correct idle pose using the cow's last facing direction.
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
        // Once walking ends, the cow may sleep or simply stop and idle again.
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If the cow is being abducted, ignore collision changes so it can continue moving toward the beam.
        if (isBeingAbducted)
            return;

        // If the cow is walking and hits something, reverse direction so it keeps roaming around the area.
        if (currentState == CowState.Walking)
        {
            moveDirection = -moveDirection;
            stateTimer = Random.Range(minMoveTime, maxMoveTime);


            // Refresh the animation and sprite flip to match the new direction.
            if (moveDirection == Vector2.up)
            {
                facingDirection = FacingDirection.Up;
                animator.Play("Cow_Walk_Up");
                spriteRenderer.flipX = false;
            }
            else if (moveDirection == Vector2.down)
            {
                facingDirection = FacingDirection.Down;
                animator.Play("Cow_Walk_Down");
                spriteRenderer.flipX = false;
            }
            else if (moveDirection == Vector2.left)
            {
                facingDirection = FacingDirection.Left;
                animator.Play("Cow_Walk_Left");
                spriteRenderer.flipX = false;
            }
            else if (moveDirection == Vector2.right)
            {
                facingDirection = FacingDirection.Right;
                animator.Play("Cow_Walk_Left");
                spriteRenderer.flipX = true;
            }
        }
    }


    public void StartAbduction()
    {
        // Freeze the cow in place and pause its animation when the tractor beam starts pulling it.
        isBeingAbducted = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.bodyType = RigidbodyType2D.Kinematic;

        animator.speed = 0f;
    }

    public void StopAbduction()
    {
        // Return the cow to normal movement and AI behavior when the abduction ends.
        isBeingAbducted = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        animator.speed = 1f;

        StartIdle();
    }
}
