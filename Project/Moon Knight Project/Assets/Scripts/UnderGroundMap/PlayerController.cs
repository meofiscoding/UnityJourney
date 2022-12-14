using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    //Accessible by inspector
    public float movementSpeed = 5.0f;
    //Jumping Float
    public float jumpForce = 8.0f;
    public float fallMutiplier;
    public float lowJumpMultiplier;

    public int availableJump = 1;
    private int availableJumpLeft;
    private bool canJump;

    //Jump Stuff
    private float moveSpeed;
    private float dirX;
    private float dirY;
    public bool ClimbingAllowed { get; set; }

    public bool isTouchingWalls;
    public bool climb;
    public bool ladleHold;

    public Transform wallCheck;

    //Not accessible by Inspector
    private float InputDirection;

    //Related to running and flipping
    private bool isRunning;
    private bool isFacingRight = true;
    private bool isGrounded;
    //Groundcheck / Bool animator
    public float groundCheckCircle;
    public Transform groundCheck;
    public LayerMask groundLayerMask;
    public LayerMask laddleLayerMask;

    //Pick up item Stuff
    public GameObject[] objToDestroy;
    public List<GameObject> PlayersInTrigger;
    public bool isCollectSword = false;
    public bool isCollectBow = false;

    //Attack Animation
    public Animator attack;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 40;

    public float attackRate = 2f;
    float nextAttackTime = 0f;

    //Slash effect
    [SerializeField]
    private GameObject Slash;

    //Level Trigger
    public LayerMask levelLayers;

    //SlashSound
    public AudioSource slashSound;

    //WalkSound
    public AudioSource walkSound;

    //Shooting
    public Transform shootingPoint;
    public GameObject arrowPrefabs;

    //Others
    private Rigidbody2D rb;
    private Animator animator;

    //Health bar
    public HealthBar healthbar;
    public float hitPoint;
    public int maxHitPoint = 5;

    //Game over
    public GameOver gameover;
    // Start is called before the first frame update
    void Start()
    {
        healthbar.SetHealth(100);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        availableJumpLeft = availableJump;
        //Climb Stuff
        moveSpeed = 5f;
    }


    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        onPickUpItem();

        //Play audio for Walking
        if (isRunning && isGrounded)
        {
            if (!walkSound.isPlaying)
            {
                walkSound.GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            walkSound.GetComponent<AudioSource>().Stop();
        }

        //Attack Stuff
        if (Time.time >= nextAttackTime)
        {
            //Test mouse click
            if (Input.GetMouseButtonDown(0))
            {
                if (isCollectSword)
                {
                    attack.SetTrigger("SwordAttack1");
                    Invoke("SlashImpact", 0.2f);
                    slashSound.GetComponent<AudioSource>().Play();
                    Invoke("NoSlash", 0.4f);
                    nextAttackTime = Time.time + 1f / attackRate;
                    Attack();
                }
            }

            //Attack 2
            if (Input.GetMouseButtonDown(1))
            {
                if (isCollectSword)
                {
                    attack.SetTrigger("SwordAttack2");
                    Invoke("SlashImpact", 0.2f);
                    slashSound.GetComponent<AudioSource>().Play();
                    Invoke("NoSlash", 0.4f);
                    nextAttackTime = Time.time + 1f / attackRate;
                    Attack();
                }
            }

            //Attack 3
            if (Input.GetMouseButtonDown(2))
            {
                if (isCollectSword)
                {
                    attack.SetTrigger("SwordAttack3");
                    Invoke("SlashImpact", 0.2f);
                    slashSound.GetComponent<AudioSource>().Play();
                    Invoke("NoSlash", 0.4f);
                    nextAttackTime = Time.time + 1f / attackRate;
                    Attack();
                }
            }
        }

        //Climb
        if (ClimbingAllowed)
        {
            if (isTouchingWalls && !isGrounded && Input.anyKey)
            {
                climb = true;
                dirY = Input.GetAxisRaw("Vertical") * moveSpeed;
            }
            if (isTouchingWalls && !isGrounded && !Input.anyKey)
            {
                ladleHold = true;
                climb = false;
                dirY = 0;
            }
            if (!isTouchingWalls && isGrounded)
            {
                climb = false;
            }
        }
        else
        {
            walkSound.GetComponent<AudioSource>().Stop();
        }

        if (Time.time >= nextAttackTime)
        {
            //Test mouse click
            if (Input.GetMouseButtonDown(0))
            {
                if (isCollectSword)
                {

                    attack.SetTrigger("SwordAttack1");
                    Invoke("SlashImpact", 0.2f);
                    slashSound.GetComponent<AudioSource>().Play();
                    Invoke("NoSlash", 0.4f);
                    nextAttackTime = Time.time + 1f / attackRate;
                    Attack();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (isCollectSword)
                {
                    attack.SetTrigger("SwordAttack2");
                    Invoke("SlashImpact", 0.2f);
                    slashSound.GetComponent<AudioSource>().Play();
                    Invoke("NoSlash", 0.4f);
                    nextAttackTime = Time.time + 1f / attackRate;
                    Attack();
                }
            }


            if (Input.GetMouseButtonDown(2))
            {
                if (isCollectSword)
                {
                    attack.SetTrigger("SwordAttack3");
                    Invoke("SlashImpact", 0.2f);
                    slashSound.GetComponent<AudioSource>().Play();
                    Invoke("NoSlash", 0.4f);
                    nextAttackTime = Time.time + 1f / attackRate;
                    Attack();
                }
            }
        }


        if (ClimbingAllowed)
        {
            if (isTouchingWalls && !isGrounded && Input.anyKey)
            {
                climb = true;
                dirY = Input.GetAxisRaw("Vertical") * moveSpeed;
            }
            if (isTouchingWalls && !isGrounded && !Input.anyKey)
            {
                ladleHold = true;
                climb = false;
                dirY = 0;
            }
            if (!isTouchingWalls && isGrounded)
            {
                climb = false;
            }
        }
        if (isGrounded)
        {
            ladleHold = false;
            climb = false;
        }

        //Variable Jump Height
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMutiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        //Shooting
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            if (isCollectBow)
            {
                //Play an attack animation
                attack.SetTrigger("BowAttack");
                Invoke("ShootArrow", 0.3f);
            }
        }
    }

    private void ShootArrow()
    {
        Instantiate(arrowPrefabs, shootingPoint.position, transform.rotation);
    }

    private void SlashImpact()
    {
        Slash.SetActive(true);
    }

    private void NoSlash()
    {
        Slash.SetActive(false);
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        Collider2D[] hitLevel = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, levelLayers);
        //Damage them
        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyController>().TakeDamage(20);
        }

        foreach (var level in hitLevel)
        {
            level.GetComponent<LevelController>().SwitchOn();
        }

    }

    //update every second
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckEnvironment();
        //Climb Stuff
        if (ClimbingAllowed)
        {
            rb.isKinematic = true;
            rb.velocity = new Vector2(dirX, dirY);
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    private void CheckInput()
    {
        InputDirection = Input.GetAxisRaw("Horizontal");

        //Check input for jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            availableJumpLeft--;
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 3)
        {
            availableJumpLeft = availableJump;
        }
        if (availableJumpLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void ApplyMovement()
    {
        rb.velocity = new Vector2(movementSpeed * InputDirection, rb.velocity.y);
    }

    private void CheckMovementDirection()
    {
        //is Moving to Left
        if (isFacingRight && InputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && InputDirection > 0)
        {
            Flip();
        }

        if (rb.velocity.x <= -0.5f || rb.velocity.x >= 0.5f)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    private void UpdateAnimation()
    {
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isClimb", climb);
        animator.SetBool("isnotClimb", ladleHold);
        animator.SetBool("isCollectSword", isCollectSword);
        animator.SetBool("isCollectBow", isCollectBow);

    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void CheckEnvironment()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckCircle, groundLayerMask);
        isTouchingWalls = Physics2D.OverlapCircle(wallCheck.position, groundCheckCircle, laddleLayerMask);
    }

    // Pickup item
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Sword" || collision.tag == "Health" || collision.tag == "Bow")
        {
            PlayersInTrigger.Add(collision.gameObject);
        }
        if (collision.tag == "Enemy")
        {
            healthbar.loseHealth(10);
            if (healthbar.GetHealth() <= 0)
            {
                animator.SetTrigger("Die");
                gameover.Setup(maxHitPoint);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayersInTrigger.Remove(collision.gameObject);
    }

    public void onPickUpItem()
    {
        if (Input.GetKey(KeyCode.G) && PlayersInTrigger.Count != 0)
        {
            var audio = PlayersInTrigger[0].GetComponent<AudioSource>();
            audio.transform.parent = null;
            audio.Play();
            if (PlayersInTrigger[0].tag == "Sword")
            {
                isCollectSword = true;
            }
            if (PlayersInTrigger[0].tag == "Bow")
            {
                isCollectBow = true;
            }
            if (PlayersInTrigger[0].tag == "Health")
            {
                healthbar.AddHealth(20);
            }
            Destroy(PlayersInTrigger[0], audio.clip.length);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckCircle);
        //Gizmos.DrawWireSphere(wallCheck.position, groundCheckCircle);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
