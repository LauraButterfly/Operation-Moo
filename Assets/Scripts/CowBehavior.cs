using UnityEngine;

// Controls the cow's idle, walking, sleeping, stunned,
// and abduction behavior.
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

    // Chance that the cow will choose to sleep
    // instead of performing another normal action.
    [Range(0f, 1f)]
    public float sleepChance = 0.1f;

    // Amount of time the cow remains stunned
    // after a failed abduction attempt.
    public float stunDuration = 3f;

    // Spinning star effect displayed while the cow is stunned.
    public GameObject stunEffect;

    // Components used for physics, animation, and sprite direction.
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Current direction the cow moves while walking.
    private Vector2 moveDirection;

    // Tracks how much time remains in the current behavior state.
    private float stateTimer;

    // Stores the cow's current AI state.
    private CowState currentState;

    // Stores the direction the cow last faced.
    // This is used to choose the correct idle and sleeping animations.
    private FacingDirection facingDirection = FacingDirection.Down;

    // Prevents normal cow behavior while it is being abducted.
    private bool isBeingAbducted = false;

    // Allows other scripts to check whether
    // this cow is currently stunned.
    public bool IsStunned
    {
        get { return currentState == CowState.Stunned; }
    }

    // Possible behavior states for the cow.
    enum CowState
    {
        Idle,
        Walking,
        Sleeping,
        Stunned
    }

    // Possible directions the cow can face.
    // This is mainly used to select the correct animation.
    enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    void Start()
    {
        // Store references to the components used by the cow.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Make sure the stun effect is hidden when the game starts.
        if (stunEffect != null)
        {
            stunEffect.SetActive(false);
        }

        // Start the cow in its idle state.
        StartIdle();
    }

    void Update()
    {
        // Do not run the normal AI while the cow
        // is currently being pulled by the tractor beam.
        if (isBeingAbducted)
            return;

        // Count down the amount of time left
        // in the cow's current state.
        stateTimer -= Time.deltaTime;

        // Choose what happens when the current state finishes.
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
        // Stop all normal movement while the cow
        // is being controlled by the tractor beam.
        if (isBeingAbducted)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Keep the cow completely still while stunned.
        if (currentState == CowState.Stunned)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Move the cow only while it is in the walking state.
        if (currentState == CowState.Walking)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Chooses the cow's next action after being idle.
    void ChooseNextAction()
    {
        float randomValue = Random.value;

        // Give the cow a small chance to sleep.
        if (randomValue < sleepChance)
        {
            StartSleeping();
        }
        else
        {
            StartWalking();
        }
    }

    // Places the cow into the idle state.
    void StartIdle()
    {
        currentState = CowState.Idle;

        // Choose a random idle duration.
        stateTimer = Random.Range(
            minIdleTime,
            maxIdleTime
        );

        PlayIdleAnimation();
    }

    // Starts a new walking period in a random direction.
    void StartWalking()
    {
        currentState = CowState.Walking;

        // Choose how long the cow will walk.
        stateTimer = Random.Range(
            minMoveTime,
            maxMoveTime
        );

        // Randomly select one of four movement directions.
        int direction = Random.Range(0, 4);

        switch (direction)
        {
            // Walk upward.
            case 0:
                moveDirection = Vector2.up;
                facingDirection = FacingDirection.Up;

                animator.Play("Cow_Walk_Up");
                spriteRenderer.flipX = false;
                break;

            // Walk downward.
            case 1:
                moveDirection = Vector2.down;
                facingDirection = FacingDirection.Down;

                animator.Play("Cow_Walk_Down");
                spriteRenderer.flipX = false;
                break;

            // Walk left.
            case 2:
                moveDirection = Vector2.left;
                facingDirection = FacingDirection.Left;

                animator.Play("Cow_Walk_Left");
                spriteRenderer.flipX = false;
                break;

            // Walk right by reusing and flipping
            // the cow's left-facing walking animation.
            case 3:
                moveDirection = Vector2.right;
                facingDirection = FacingDirection.Right;

                animator.Play("Cow_Walk_Left");
                spriteRenderer.flipX = true;
                break;
        }
    }

    // Places the cow into its sleeping state.
    void StartSleeping()
    {
        currentState = CowState.Sleeping;

        // Choose a random amount of time to sleep.
        stateTimer = Random.Range(
            minSleepTime,
            maxSleepTime
        );

        // Use different sleeping poses depending
        // on the direction the cow was facing.
        if (facingDirection == FacingDirection.Left)
        {
            spriteRenderer.flipX = false;
            animator.Play("Cow_Sleep");
        }
        else if (facingDirection == FacingDirection.Right)
        {
            // Reuse the left-facing sleep sprite by flipping it.
            spriteRenderer.flipX = true;
            animator.Play("Cow_Sleep");
        }
        else
        {
            // Up and down both use the downward sleep pose.
            spriteRenderer.flipX = false;
            animator.Play("Cow_Sleep_Down");
        }
    }

    // Plays an idle animation based on the cow's
    // most recent facing direction.
    void PlayIdleAnimation()
    {
        if (facingDirection == FacingDirection.Left)
        {
            spriteRenderer.flipX = false;
            animator.Play("Cow_Idle_Left");
        }
        else if (facingDirection == FacingDirection.Right)
        {
            // Flip the left-facing idle animation for right.
            spriteRenderer.flipX = true;
            animator.Play("Cow_Idle_Left");
        }
        else
        {
            // Up and down both use the downward idle animation.
            spriteRenderer.flipX = false;
            animator.Play("Cow_Idle_Down");
        }
    }

    // Chooses what the cow should do after finishing a walk.
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

    // Reacts when the cow collides with an obstacle.
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore collisions while the tractor beam
        // is controlling the cow.
        if (isBeingAbducted)
            return;

        // Ignore normal collision behavior while stunned.
        if (currentState == CowState.Stunned)
            return;

        // If the cow hits something while walking,
        // reverse its movement direction.
        if (currentState == CowState.Walking)
        {
            moveDirection = -moveDirection;

            // Give the cow a new walking duration.
            stateTimer = Random.Range(
                minMoveTime,
                maxMoveTime
            );

            // Update the cow's animation so it matches
            // its new movement direction.
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

                // Reuse the left walking animation
                // and flip it horizontally.
                animator.Play("Cow_Walk_Left");
                spriteRenderer.flipX = true;
            }
        }
    }

    // Called when the tractor beam begins abducting this cow.
    public void StartAbduction()
    {
        // Stunned cows cannot be abducted.
        if (IsStunned)
            return;

        isBeingAbducted = true;

        // Immediately stop any existing movement or rotation.
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Switch to Kinematic so the tractor beam
        // can control the cow's position directly.
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Freeze the current cow animation during abduction.
        animator.speed = 0f;
    }

    // Returns the cow to normal behavior if an abduction ends.
    public void StopAbduction()
    {
        isBeingAbducted = false;

        // Restore normal Rigidbody physics.
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Resume normal cow animation.
        animator.speed = 1f;

        // Make sure the stun effect is hidden.
        if (stunEffect != null)
        {
            stunEffect.SetActive(false);
        }

        // Return the cow to its normal idle behavior.
        StartIdle();
    }

    // Stuns the cow after a failed abduction attempt.
    public void Stun()
    {
        // The tractor beam no longer controls the cow.
        isBeingAbducted = false;

        // Restore normal Rigidbody physics.
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Make sure the cow is completely stationary.
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Enter the stunned state for the specified duration.
        currentState = CowState.Stunned;
        stateTimer = stunDuration;

        // Freeze the cow's normal animation.
        animator.speed = 0f;

        // Display the spinning stars above the cow.
        if (stunEffect != null)
        {
            stunEffect.SetActive(true);
        }
    }

    // Ends the stunned state and returns the cow
    // to its normal behavior.
    void EndStun()
    {
        // Hide the spinning star effect.
        if (stunEffect != null)
        {
            stunEffect.SetActive(false);
        }

        // Resume the cow's normal animations.
        animator.speed = 1f;

        // Return to the idle state.
        StartIdle();
    }
}