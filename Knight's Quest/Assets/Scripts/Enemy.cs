using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    // Config
    [Header("Physics Specs")]
    [SerializeField] GameObject idlingPlace;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float agroRange = 5f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float timeBetweenAttack = 1f;
    [SerializeField] int healthPoints = 100;
    [SerializeField] int attackDamage = 20;
    [SerializeField] float activeRayRange = 8f;
    [SerializeField] float delayBeforeDeath = 2f;
    [SerializeField] int minCoinsDrop = 3;
    [SerializeField] int maxCoinsDrop = 10;

    [Header("SFX")]
    [SerializeField] AudioClip emptyAttackSFX;
    [SerializeField] AudioClip deathSFX;

    // Cached component references
    Rigidbody2D myRigidBoby;
    Animator myAnimator;
    AudioSource myAudioSource;
    Player player;
    Transform playerTransform;
    GameSession gameSession;
    RaycastHit2D hitVerticalUp;

    // States
    bool isLookingRight = true;
    bool isOnTheEdge;
    bool attackHasStarted;
    bool canSeePlayer;
    bool canMove = true;

    readonly int backgroundLayerMask = ~(1 << 9);
    readonly int enemyLayerMask = ~(1 << 11);

    void Start()
    {
        myRigidBoby = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();
        playerTransform = FindObjectOfType<Player>().GetComponent<Transform>();
        gameSession = FindObjectOfType<GameSession>();
    }

    void Update()
    {
        CheckEnemyVisibitily();
        CheckMovement();
        IsCloseToPlayer();
        PrepareToAttack();
    }

    private void CheckEnemyVisibitily()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) < activeRayRange)
        {
            // create ray from enemy to player
            hitVerticalUp = Physics2D.Raycast(transform.position,
                playerTransform.position - transform.position,
                Vector2.Distance(transform.position, playerTransform.position),
                backgroundLayerMask & enemyLayerMask);

            // check if ray bumps into collider
            if (hitVerticalUp.collider != null)
            {
                if (hitVerticalUp.collider.gameObject == FindObjectOfType<Player>().gameObject) // if collider == player
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }

            // Debug.DrawRay(transform.position, playerTransform.position - transform.position, Color.red);
        }
    }

    private void CheckMovement()
    {
        // check if enemy is on the edge of platform
        if (isOnTheEdge) 
        {
            StopMoving();
        }

        // check if enemy is close enough to the player to start chasing him
        if (IsAgroTriggered() && canSeePlayer)
        {
            if (!isOnTheEdge)
            {
                Move();
            }
            FlipSprite();
        }

        // enemy returns to his start idling position
        else
        {
            ReturnToIdlePosition();
        }
    }

    private void StopMoving()
    {
        myAnimator.SetBool("isRunning", false);
        myRigidBoby.velocity = new Vector2(0f, 0f);
        if (!IsAgroTriggered())
        {
            isOnTheEdge = false;
        }
    }

    private bool IsAgroTriggered()
    {
        return Vector3.Distance(transform.position, playerTransform.position) < agroRange;
    }

    private bool IsCloseToPlayer()
    {
        return Vector3.Distance(transform.position, playerTransform.position) < attackRange;
    }

    private void Move()
    {
        if (canMove)
        {
            // check if current enemy animation is not 'attack' to prevent enemy moving during attack animation
            if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("LightBandit_Attack"))
            {
                myAnimator.SetBool("isRunning", true);

                // check which way to move
                if (!isLookingRight)
                {
                    myRigidBoby.velocity = new Vector2(-moveSpeed, myRigidBoby.velocity.y);
                }
                else
                {
                    myRigidBoby.velocity = new Vector2(moveSpeed, myRigidBoby.velocity.y);
                }
            }
        }
    }

    private void ReturnToIdlePosition()
    {
        if (canMove)
        {
            if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("LightBandit_Attack"))
            {
                myAnimator.SetBool("isRunning", true);

                // change localScale depending on the difference of player's and AI's positions
                transform.localScale = new Vector2(Mathf.Sign(transform.position.x - idlingPlace.transform.position.x), 1f);

                // return back to start idling position
                myRigidBoby.velocity = new Vector2(0f, 0f);
                transform.position = Vector2.MoveTowards(
                    transform.position, new Vector2(idlingPlace.transform.position.x, transform.position.y),
                    1.5f * moveSpeed * Time.deltaTime);

                if (IsOnIdlePosition())
                {
                    myAnimator.SetBool("isRunning", false);
                }
            }
        }
    }

    private void FlipSprite()
    {
        if (canMove)
        {
            // check which way to look
            if (transform.localScale.x == -1 && playerTransform.position.x < transform.position.x) // if enemy x-scale == -1 and player position < enemy position
            {
                transform.localScale = new Vector2(1f, 1f);
                isLookingRight = false;

                if (isOnTheEdge)
                {
                    isOnTheEdge = false;
                }
            }
            else if (transform.localScale.x == 1 && playerTransform.position.x > transform.position.x) // if enemy x-scale == 1 and player position > enemy position
            {
                transform.localScale = new Vector2(-1f, 1f);
                isLookingRight = true;

                if (isOnTheEdge)
                {
                    isOnTheEdge = false;
                }
            }
            else if (transform.localScale.x == 1 && playerTransform.position.x < transform.position.x) // if enemy x-scale == 1 and player position < enemy position
            {
                isLookingRight = false;
            }
        }
    }

    private bool IsOnIdlePosition()
    {
        return transform.position.x == idlingPlace.transform.position.x;
    }

    private void PrepareToAttack()
    {
        if (IsCloseToPlayer())
        {
            player = FindObjectOfType<Player>();

            StopMoving();
            StartCoroutine(Attack());
        }
        else
        {
            player = null;
        }
    }

    IEnumerator Attack()
    {
        if (!attackHasStarted)
        {
            while (true)
            {
                attackHasStarted = true; // set bool to prevent Coroutine running during another active one 

                // check if x-distance between enemy and player is greater than attackRange
                if (Mathf.Abs(playerTransform.position.x - transform.position.x) > attackRange) 
                {
                    attackHasStarted = false;
                    break;
                }

                if (PlayerStats.HealthPoints <= 0)
                {
                    break;
                }

                myAnimator.SetTrigger("attack");
                yield return new WaitForSeconds(timeBetweenAttack);
            }
        }
    }

    public void DealDamage()
    {
        if (player == null)
        {
            myAudioSource.PlayOneShot(emptyAttackSFX);
        }
        else if (player.GetComponent<CircleCollider2D>())
        {
            player.TriggerGetHurtAnimation();
            gameSession.TakePlayerHealthPoints(attackDamage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (canMove)
        {
            myAnimator.SetTrigger("getHurt");
            healthPoints -= amount;
            if (healthPoints <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    IEnumerator Die()
    {
        canMove = false;
        myAnimator.SetTrigger("die");
        myAudioSource.clip = deathSFX;
        myAudioSource.Play();

        PlayerStats.Coins += Random.Range(minCoinsDrop, maxCoinsDrop);
        yield return new WaitForSeconds(delayBeforeDeath);
        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // check if the enemy is on the edge of the platform to stop moving
        if (collision == collision.GetComponent<CompositeCollider2D>())
        {
            myAnimator.SetBool("isRunning", false);
            isOnTheEdge = true;
        }
    }
}
