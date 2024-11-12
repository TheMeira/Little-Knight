using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Golem : MonoBehaviour
{
    private enum State
    {
        Idle,
        Walking,
        Attack,
        Knockback,
        Dead
    }
    private State currentState;
    [SerializeField]
    private GameObject alive;
    private bool groundDetected, wallDetected;
    [SerializeField] private bool playerDetected;
    [SerializeField]
    private Transform
        groundCheck,
        wallCheck;
    [SerializeField]
    private LayerMask
        whatIsGround;
    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        knockbackDuration;
    [SerializeField]
    private Vector3 knockBackSpeed;

    [SerializeField]
    private GameObject
        hitParticle, deathChunkParticle, deathBloodParticle;

    private Vector3 movement;

    private int facingDirection = 1,
        damageDirection;

    private float
        currentHealth,
        knockBackStartTime;


    private Rigidbody aliverb;
    private Animator aliveAnim;
    [SerializeField]
    private float distance;
    [SerializeField]
    private Transform player;

    public Slider slider;

    [SerializeField]
    private Transform playerCheck;
    [SerializeField]
    public float playerCheckDistance;
    [SerializeField]
    private LayerMask whatIsPlayer;
    public bool animationFinish = true;

    [SerializeField]
    private float viewRadius;

    [SerializeField]
    private float attackDistance;
    [SerializeField] private bool probabiltyKnockBack;    
    
    [SerializeField] private float knockbackProbability = 0.6f;


    [SerializeField] private AudioSource damageSFX;
    public GameObject coinPrefab;
    public int minCoinsToDrop = 5;
    public int maxCoinsToDrop = 7;
    public float coinSpeed = 5f;



    private void Start()
    {
        
        

        currentHealth = maxHealth;
        aliverb = GetComponent<Rigidbody>();
        aliveAnim = GetComponent<Animator>();
        currentState = State.Idle;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        if(!probabiltyKnockBack)
        knockbackProbability = UnityEngine.Random.Range(0f, 1f);


    }

    private void Update()

    {
        switch(currentState)
        {
            case State.Idle:
                UpdateIdleState();
                break;
            case State.Walking:
                UpdateWalkingState();
                break;

            case State.Attack:
                UpdateAttackState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;

               
        }
        Collider[] colliders = Physics.OverlapSphere(playerCheck.position, viewRadius, whatIsPlayer);
        playerDetected = colliders.Length > 0;
        //  slider.value = currentHealth;
    }

    private void EnterIdleState()
    {
        currentState = State.Idle;
        aliveAnim.SetBool("idle", true);

    }
    private void UpdateIdleState()
    {

        aliverb.velocity = new Vector2(0, 0);

    if(playerDetected)
        {
            player = FindObjectOfType<PlayerController>().gameObject.transform;
            SwitchState(State.Walking);
        }


    }
    private void ExitIdleState()
    {

    }


    private void EnterAttackState()
    {
        currentState = State.Attack;
        aliveAnim.SetBool("attack", true);
        animationFinish = false;
        StartCoroutine(startAnimation());
    }
    private void ExitAttackState()
    {
        aliveAnim.SetBool("attack", false);
    }
    private void UpdateAttackState()
    {

        aliverb.velocity = new Vector2(0, 0);
        if (animationFinish == true)
        {
            SwitchState(State.Walking);

        }


    }

    IEnumerator startAnimation()
    {
       
        yield return new WaitForSeconds(0.9f);
        animationFinish = true;

       
    }

    

    // WALKING STATE--------------------------------------------------------
    private void EnterWalkingState()
    {
        aliveAnim.SetBool("move", true);
        currentState = State.Walking;

    }
    private void UpdateWalkingState()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        // Flip the golem if needed
        if (directionToPlayer.x < 0 && facingDirection > 0 || directionToPlayer.x > 0 && facingDirection < 0)
        {
            flip();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < attackDistance)
        {
            SwitchState(State.Attack);
        }

        // Move the golem towards the player in X and Z directions
        Vector3 movementDirection = new Vector3(directionToPlayer.normalized.x, 0f, directionToPlayer.normalized.z);
        movement.Set(movementDirection.x * movementSpeed, aliverb.velocity.y, movementDirection.z * movementSpeed);
        aliverb.velocity = movement;

       
    }
    private void ExitWalkingState()
    {

    }

    // KNOCKBACK STATE--------------------------------------------------------
    private void EnterKnockbackState()
    {
        currentState = State.Knockback;
        aliveAnim.SetBool("knockback", true);
        //FindObjectOfType<AudioManager>().golem[0].Play();
        StartCoroutine(KnockBackTime());
        movement.Set(knockBackSpeed.x * damageDirection, knockBackSpeed.y, knockBackSpeed.z);
        aliverb.velocity = movement;
    


    }

    IEnumerator KnockBackTime()
    {
        yield return new WaitForSeconds(knockbackDuration);
        SwitchState(State.Walking);
        aliveAnim.SetBool("knockback", false);
    }

    private void UpdateKnockbackState()
    {
     
    }
    private void ExitKnockbackState()
    {
        
    }

    // DEAD STATE--------------------------------------------------------

    private void EnterDeadState()
    {
        //    FindObjectOfType<AudioManager>().death.Play();
        currentState = State.Dead;
        Instantiate(deathBloodParticle, alive.transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
        Die();
    }

    void Die()
    {
        int numCoinsToDrop = UnityEngine.Random.Range(minCoinsToDrop, maxCoinsToDrop + 1);

        for (int i = 0; i < numCoinsToDrop; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Rigidbody rb = coin.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calculate direction towards a random point
                Vector3 direction = UnityEngine.Random.insideUnitSphere.normalized;

                // Set velocity based on direction and coinSpeed
                rb.velocity = direction * coinSpeed;
            }
            else
            {
                Debug.LogError("Rigidbody component not found on the coin prefab.");
            }
        }

        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }
    private void ExitDeadState()
    {

    }

    // OTHER FUNTIONS-----------------------------------------------------

        private void Damage(float[] attackDetails)
    {
        damageSFX.Play();
        currentHealth -= attackDetails[0];
       
        if(attackDetails[1] > transform.position.x)
        {
            damageDirection = -1;
        } else
        {
            damageDirection = 1;
        }
        if (currentHealth > 0f)
        {
            slider.value -= attackDetails[0];
            Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
            if(UnityEngine.Random.value < knockbackProbability)
            {

            SwitchState(State.Knockback);
            }
            
        } else
        {
            SwitchState(State.Dead);
        }
    }

    //HIT particle 

       
        private void flip()
    {
        facingDirection *= -1;
        transform.Rotate(0, 180, 0);
    }

    private void walk()
    {
      //  movement.Set(movementSpeed * facingDirection , aliverb.velocity.y);
        aliverb.velocity = movement;

        if (playerDetected && animationFinish == true)
        {
          //  StartAttackState();
            currentState = State.Attack;
            aliveAnim.SetBool("attack", true);


        }


    }
       
    private void SwitchState(State state)
    {
        switch(currentState)
        {
            case State.Idle:
                ExitIdleState();
                break;
            case State.Walking:
                ExitWalkingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Attack:
                ExitAttackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Idle:
                EnterIdleState();
                break;
            case State.Walking:
                EnterWalkingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Attack:
                EnterAttackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }
        currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x ,groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawLine(playerCheck.position, new Vector2(playerCheck.position.x + playerCheckDistance, playerCheck.position.y ));
      

    }
    public void AttackSound()
    {
     //   FindObjectOfType<AudioManager>().golem[1].Play();
    }
}
