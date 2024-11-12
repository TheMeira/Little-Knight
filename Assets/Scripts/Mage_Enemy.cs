using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mage_Enemy : MonoBehaviour
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
  
    [SerializeField] private bool playerDetected;
   
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

    private float knockbackProbability;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float damage;
    [SerializeField] private float speedBullet;
    [SerializeField] private float attackDelay;

    [SerializeField] private AudioSource damageSFX;

    public GameObject coinPrefab;
    public int minCoinsToDrop = 5;
    public int maxCoinsToDrop = 7;


    private void Start()
    {



        currentHealth = maxHealth;
        aliverb = GetComponent<Rigidbody>();
        aliveAnim = GetComponent<Animator>();
        currentState = State.Idle;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        knockbackProbability = UnityEngine.Random.Range(0f, 1f);

     


    }

    private void Update()

    {
        switch (currentState)
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

        if (playerDetected)
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
        StopAllCoroutines();
        StartCoroutine(startAnimation());
        // Instancier la balle au point de spawn de la balle
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(damage, "Player");
        Vector3 direction = (player.position - bulletSpawnPoint.position).normalized;
        bullet.GetComponent<Bullet>().LaunchBullet(direction, speedBullet);

        
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

        yield return new WaitForSeconds(attackDelay);
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

        // Move the golem towards the player in X and Z directions
        Vector3 movementDirection = new Vector3(directionToPlayer.normalized.x, 0f, directionToPlayer.normalized.z);
        movement.Set(movementDirection.x * movementSpeed, aliverb.velocity.y, movementDirection.z * movementSpeed);
        aliverb.velocity = movement;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < attackDistance)
        {
            SwitchState(State.Attack);
        }
    }
    private void ExitWalkingState()
    {

    }

    // KNOCKBACK STATE--------------------------------------------------------
    private void EnterKnockbackState()
    {
        damageSFX.Play();
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
            GameObject coin = Instantiate(coinPrefab, alive.transform.position, Quaternion.identity);
            Rigidbody rb = coin.GetComponent<Rigidbody>();

            // Apply some random force to the coin to make it scatter
            Vector3 force = new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(5f, 10f), UnityEngine.Random.Range(-2f, 2f));
          //  rb.AddForce(force, ForceMode.Impulse);
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
        currentHealth -= attackDetails[0];

        if (attackDetails[1] > transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }
        if (currentHealth > 0f)
        {
            slider.value -= attackDetails[0];
            Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
            SwitchState(State.Knockback);

        }
        else
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
        switch (currentState)
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

 
    public void AttackSound()
    {
        //   FindObjectOfType<AudioManager>().golem[1].Play();
    }
}
