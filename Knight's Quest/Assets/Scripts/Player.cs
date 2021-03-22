using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Config
    [Header("Physics Stats")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] int attackDamage = 50;
    [SerializeField] int waitAfterDeathForSeconds = 2;

    [Header("SFX")]
    [SerializeField] AudioClip[] myFootstepsSFX;
    [SerializeField] AudioClip myFallDownSFX;
    [SerializeField] AudioClip myEmptyAttackSFX;
    [SerializeField] AudioClip[] myStrikeSFX;
    [SerializeField] AudioClip myJumpSFX;
    [SerializeField] AudioClip myTakeDamageSFX;
    [SerializeField] AudioClip myDeathScreamSFX;

    // Cached component references
    [Header("Colliding Stuff")]
    public CircleCollider2D myTopCollider;
    public BoxCollider2D myButtomCollider;
    public CapsuleCollider2D myAttackTriggerCollider;

    Rigidbody2D myRigidBody;
    Animator myAnimator;
    AudioSource myAudioSource;
    Enemy enemy;
    GameSession gameSession;
    Wizard wizard;
    DialogueController dialogueController;

    // States
    bool isAlive = true;
    bool isTriggered = false;
    bool hasStepped;
    bool hasFell;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();

        myTopCollider = GetComponent<CircleCollider2D>();
        myButtomCollider = GetComponent<BoxCollider2D>();
        myAttackTriggerCollider = GetComponent<CapsuleCollider2D>();

        gameSession = FindObjectOfType<GameSession>();
        wizard = FindObjectOfType<Wizard>();
        dialogueController = FindObjectOfType<DialogueController>();
    }

    void Update()
    {
        if (!isAlive) { return; }

        Run();
        Jump();
        Attack();
        FlipSprite();
        StartCoroutine(Die());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (FindObjectOfType<Enemy>() == null) { return; }
        if (collision.CompareTag("Enemy"))
        {
            enemy = collision.gameObject.GetComponent<Enemy>();
            StartCoroutine(Die());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        enemy = null;
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // get x-velocity between -1 to +1
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y); // initialize player velocity
        myRigidBody.velocity = playerVelocity; // set player velocity
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon; // check player speed
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);

        CheckFootsteps();
    }

    
    private void CheckFootsteps()
    {
        if (myAnimator.GetBool("isRunning") == true && myAnimator.GetBool("isOnGround") == true)
        {
            if (!hasStepped)
            {
                StartCoroutine(PlayFootstepsAudio());
            }
        }
    }

    IEnumerator PlayFootstepsAudio()
    {
        hasStepped = true;
        myAudioSource.PlayOneShot(myFootstepsSFX[Random.Range(0, myFootstepsSFX.Length)]);
        yield return new WaitForSeconds(0.3f);
        hasStepped = false;
    }

    private void Jump()
    {
        if (!myButtomCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) // check if feet are touching ground
        {
            hasFell = false;
            myAnimator.SetBool("isOnGround", false);
            return;
        }
        else 
        {
            myAnimator.SetBool("isOnGround", true);
            if (!hasFell)
            {
                hasFell = true;
                myAudioSource.clip = myFallDownSFX;
                myAudioSource.Play();
            }
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0, jumpSpeed); // initialize player's jump velocity
            myRigidBody.velocity += jumpVelocityToAdd; // set player's jump velocity 
            myAnimator.SetTrigger("jump");
            myAudioSource.PlayOneShot(myJumpSFX);
        }
    }

    public void DealDamage()
    {
        if (enemy == null)
        {
            myAudioSource.PlayOneShot(myEmptyAttackSFX);
        }
        else if (enemy.GetComponent<CapsuleCollider2D>())
        {
            myAudioSource.PlayOneShot(myStrikeSFX[Random.Range(0, myStrikeSFX.Length)]);
            enemy.TakeDamage(attackDamage);
        }
    }

    private void Attack()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            myAnimator.SetTrigger("attack");
        }
    }
    
    public void TriggerGetHurtAnimation()
    {
        myAnimator.SetTrigger("getHurt");
    }

    public void PlayTakeDamageSFX()
    {
        myAudioSource.clip = myTakeDamageSFX;
        myAudioSource.Play();
    }

    public IEnumerator Die()
    {
        if (PlayerStats.HealthPoints <= 0 || myButtomCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            if (!isTriggered)
            {
                myTopCollider.enabled = false; // disable collider to prevent enemy attacking animation after player's death
                isTriggered = true; // set bool to prevent Coroutine running during another active one 
                myAnimator.SetTrigger("die");
                isAlive = false; // disable player actions
                myRigidBody.velocity = new Vector2(0f, 0f); // disable player movement
                myAudioSource.PlayOneShot(myDeathScreamSFX);
                yield return new WaitForSeconds(waitAfterDeathForSeconds);
                FindObjectOfType<GameSession>().ProccessPlayerDeath();
            }
        }
    }

    public void TryToGiveCoins()
    {
        GiveCoins(wizard.GetAmountOfCoinsToPass());
    }

    public void TryToGiveCrystals()
    {
        GiveCrystals(wizard.GetAmountOfCrystalsToPass());
    }

    private void GiveCoins(int amount)
    {
        if (PlayerStats.Coins - amount >= 0)
        {
            gameSession.DecreaseCoins(amount);
            dialogueController.Purchase();
        }
        else
        {
            dialogueController.FailDialogue();
        }
    }

    private void GiveCrystals(int amount)
    {
        if (PlayerStats.Crystals - amount >= 0 && FindObjectsOfType<Enemy>().Length <= 0)
        {
            gameSession.DecreaseCrystals(amount);
            dialogueController.Purchase();
        }
        else
        {
            dialogueController.FailDialogue();
        }
    }

    private void FlipSprite()
    {
        // check if player has x-velocity
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon; 
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f); // change scale depending on x-velocity sign
        }
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }
}
