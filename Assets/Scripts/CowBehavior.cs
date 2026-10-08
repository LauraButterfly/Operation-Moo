using UnityEngine;

// Controls the cow's idle, walking, sleeping, and stunned behavior.
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

    // How long the cow remains stunned after a failed abduction.
    public float stunDuration = 3f;

    // The spinning star effect shown while stunned.
    public GameObject stunEffect;

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

    // Lets other scripts check whether this cow is currently stunned.
    public bool IsStunned
    {
        get { return currentState == CowState.Stunned; }
    }

    // Simple behavior states for the cow AI.
    enum CowState
    {
        Idle,
        Walking,
        Sleeping,
        Stunned
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

        // Make sure the stun effect starts hidden.
        if (stunEffect != null)
        {
            stunEffect.SetActive(false);
        }

        // Begin in an idle state.
        StartIdle();
    }

    void Update()
    {
        // Stop normal AI updates while the cow is being abducted.
        if (isBeingAbducted)
            return;

        // Count down until the current behavior should end.
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            switch (currentState)
            {
                case CowState.Walking:
                    ChooseAfterWalking();
                    break;

                case CowState.Idle:
                    ChooseNextAction();
                    break;

                case CowState.Sleeping:
                    StartIdle();
                    break;

                case CowState.Stunned:
                    EndStun();
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

        // Keep the cow frozen while stunned.
        if (currentState == CowState.Stunned)
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
        currentState = CowState.Idle;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);

        PlayIdleAnimation();
    }

    void StartWalking()
    {
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore collision behavior while being abducted.
        if (isBeingAbducted)
            return;

        // Ignore collision behavior while stunned.
        if (currentState == CowState.Stunned)
            return;

        // Reverse direction when the cow hits something while walking.
        if (currentState == CowState.Walking)
        {
            moveDirection = -moveDirection;
            stateTimer = Random.Range(minMoveTime, maxMoveTime);

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
        // Safety check: stunned cows cannot be abducted.
        if (IsStunned)
            return;

        isBeingAbducted = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.bodyType = RigidbodyType2D.Kinematic;

        animator.speed = 0f;
    }

    public void StopAbduction()
    {
        isBeingAbducted = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        animator.speed = 1f;

        if (stunEffect != null)
        {
            stunEffect.SetActive(false);
        }

        StartIdle();
    }

    public void Stun()
    {
        // The cow is no longer being pulled by the beam.
        isBeingAbducted = false;

        // Restore normal physics.
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Enter the stunned state.
        currentState = CowState.Stunned;
        stateTimer = stunDuration;

        // Freeze the cow's normal animation.
        animator.speed = 0f;

        // Show the spinning stars.
        if (stunEffect != null)
        {
            stunEffect.SetActive(true);
        }
    }

    void EndStun()
    {
        // Hide the stars.
        if (stunEffect != null)
        {
            stunEffect.SetActive(false);
        }

        // Resume cow animations.
        animator.speed = 1f;

        // Return to normal behavior.
        StartIdle();
    }
}