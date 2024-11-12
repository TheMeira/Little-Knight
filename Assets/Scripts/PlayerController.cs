using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxMana;
    [SerializeField] private float manaConsomation;
    [SerializeField] private float damageMagic;
    [SerializeField] private float speedMagicBall;
    [SerializeField] private Transform handTransform;
    [SerializeField] private GameObject heavyAttackVFX;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundDist;
    [SerializeField] private Transform feetTransform;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private Animator anim;
    private int damageDirection = 1;
    [SerializeField] private GameObject VFX_hit, VFX_death;
    [SerializeField] private Vector3 knockBackSpeed;
    [SerializeField] private float knockbackDuration;

    private Resource_Controller healthController = new Resource_Controller();
    private Resource_Controller ManaController = new Resource_Controller();


    


    private enum State
    {
        Idle,
        Walking,
        LightAttack,
        HeavyAttack,
        Knockback,
        Dead,
        Jump,
        Fall
    }

    private State currentState;

    private Rigidbody rb;
    private SpriteRenderer sr;
    private bool isFacingRight = true;
    [SerializeField] private bool hasJumped = false;

    private bool canAttack;

    [SerializeField] private CapsuleCollider attackCollider;
    [SerializeField] private LayerMask targetMask;

    [SerializeField] private float _attackForce;
    public float attackForce { get { return _attackForce; } private set { _attackForce = value; } }
    private float currentTime;
    [SerializeField] private float idleAttackTime;
    [SerializeField] private float heavyAttackTime;
    private bool isKnockback;

    private bool isDead;

    [Header("sounds")]
    public AudioSource jump;
    public AudioSource attackLight;
    public AudioSource attackHeavy;
    public AudioSource move;
    public AudioSource damage;
    public AudioSource footstepsSFX;
    

    public void ChangePositionPlayer(Transform newPos)
    {
        transform.position = newPos.position;
    }

    public void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        sr = GetComponent<SpriteRenderer>();

        healthController.Init(maxHealth, UIManager.instance.sliderHealth, UIManager.instance.healthText);
        ManaController.Init(maxMana, UIManager.instance.sliderMana, UIManager.instance.manaText);

        canAttack = true;
        ExitDeadState();
        ExitLightAttackState();
        ExitHeavyAttackState();
        currentState = State.Idle;
        EnterIdleState();
        


    }

    void Update()
    {
        if (isDead) return;

        switch (currentState)
        {
            case State.Idle:
                UpdateIdleState();
                break;
            case State.Walking:
                UpdateWalkingState();
                break;
            case State.HeavyAttack:
                UpdateHeavyAttackState();
                break;

            case State.LightAttack:
                UpdateLightAttackState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
            case State.Jump:
                UpdateJumpState();
                break;
            case State.Fall:
                UpdateFallState();
                break;
        }

       




        if (Input.GetButtonDown("Jump") && !hasJumped)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
           

            SwitchState(State.Jump);


        }

        if (Input.GetButtonDown("Fire1") && canAttack)
        {
            SwitchState(State.LightAttack);
        }
        if (ManaController.currentValue >= manaConsomation && Input.GetButtonDown("Fire2") && canAttack)
        {
            SwitchState(State.HeavyAttack);

        }
        hasJumped = !CheckGround();


    }

    private void FixedUpdate()
    {
        if (isDead) return;
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical") * 1.2f;
        Vector3 moveDir = (new Vector3(x, 0, y) * speed);
        if (!isKnockback)
            rb.velocity = new Vector3(moveDir.x * Time.deltaTime, rb.velocity.y, moveDir.z * Time.deltaTime);


        if (x > 0 && !isFacingRight || x < 0 && isFacingRight)
        {
            Flip();
        }

        if (CheckGround())
        {
            hasJumped = false;
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void EnterIdleState()
    {
        anim.SetBool("idle", true);
    }
    private void UpdateIdleState()
    {
        if (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) > 0.1)
        {
            SwitchState(State.Walking);
        }


    }
    private void ExitIdleState()
    {
        anim.SetBool("idle", false);
    }

    private void EnterJumpState()
    {
        anim.SetBool("jump", true);
        hasJumped = true;
        playSound(jump);
    }
    private void UpdateJumpState()
    {
        if (rb.velocity.y < 0f)
        {
            SwitchState(State.Fall);
        }


    }
    private void ExitJumpState()
    {
        anim.SetBool("jump", false);
    }


    private void ExitFallState()
    {
        anim.SetBool("fall", false);
    }

    private void EnterFallState()
    {
        anim.SetBool("fall", true);
    }
    private void UpdateFallState()
    {

        //  castPos.y += 0.1f; // Adjust the cast position slightly above the player
        if (!hasJumped)
        {

            SwitchState(State.Idle);
        }


    }

 

    private bool CheckGround()
    {
        return Physics.Raycast(feetTransform.position, -transform.up, groundDist, terrainLayer);
    }

    private void EnterLightAttackState()
    {
        canAttack = false;
        anim.SetBool("attack", true);
        currentTime = Time.time;
        playSound(attackLight);
    }
    private void ExitLightAttackState()
    {
        anim.SetBool("attack", false);
        canAttack = true;
    }
    private void UpdateLightAttackState()
    {

        if ((currentTime + idleAttackTime) < Time.time)
        {
            SwitchState(State.Idle);
        }


    }


    private void EnterHeavyAttackState()
    {
        canAttack = false;
        anim.SetBool("attackHeavy", true);
        currentTime = Time.time;
        ManaController.ModifyValue(-manaConsomation);
        GameObject fireBall = Instantiate(heavyAttackVFX, handTransform.position, Quaternion.identity);
        fireBall.transform.rotation = Quaternion.Euler(0f, -90f, transform.rotation.z);
        fireBall.GetComponent<Bullet>().Init(damageMagic, "Enemy");
        fireBall.GetComponent<Bullet>().LaunchBullet(transform.right, speedMagicBall);
        playSound(attackHeavy);
    }
    private void ExitHeavyAttackState()
    {
        anim.SetBool("attackHeavy", false);
        canAttack = true;
    }
    private void UpdateHeavyAttackState()
    {

        if ((currentTime + heavyAttackTime) < Time.time)
        {
            SwitchState(State.Idle);
        }


    }




    // WALKING STATE--------------------------------------------------------
    private void EnterWalkingState()
    {
        anim.SetBool("walk", true);
        playSound(move);

    }
    private void UpdateWalkingState()
    {
        if (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) <= 0)
        {
            SwitchState(State.Idle);
        }
    }
    private void ExitWalkingState()
    {
        anim.SetBool("walk", false);
        StartCoroutine(stopWalkingSound());
        
    }

    IEnumerator stopWalkingSound()
    {
        yield return new WaitForSeconds(0.2f);
        move.Stop();

    }

    // KNOCKBACK STATE--------------------------------------------------------
    private void EnterKnockbackState()
    {
        anim.SetBool("knockback", true);
        isKnockback = true;
        //FindObjectOfType<AudioManager>().golem[0].Play();
        StartCoroutine(KnockBackTime());
        rb.velocity = new Vector3(knockBackSpeed.x * damageDirection, knockBackSpeed.y, knockBackSpeed.z);
     



    }

    IEnumerator KnockBackTime()
    {
        yield return new WaitForSeconds(knockbackDuration);
        SwitchState(State.Idle);

    }


    private void UpdateKnockbackState()
    {

    }
    private void ExitKnockbackState()
    {
        isKnockback = false;
        anim.SetBool("false", true);
    }

    // DEAD STATE--------------------------------------------------------

    private void EnterDeadState()
    {
        isDead = true;
        anim.SetBool("dead", true);
        Invoke("restart", 2);
        rb.velocity = Vector3.zero;
    }
    private void UpdateDeadState()
    {

    }
    private void ExitDeadState()
    {
        anim.SetBool("dead", false);
        isDead = false;
    }

    private void restart()
    {
        GameManager.instance.RestartGame();
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
            case State.Jump:
                ExitJumpState();
                break;
            case State.Fall:
                ExitFallState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.LightAttack:
                ExitLightAttackState();
                break;
            case State.HeavyAttack:
                ExitHeavyAttackState();
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
            case State.Jump:
                EnterJumpState();
                break;
            case State.Fall:
                EnterFallState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.LightAttack:
                EnterLightAttackState();
                break;
            case State.HeavyAttack:
                EnterHeavyAttackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }
        currentState = state;
    }

    private void Damage(float[] attackDetails)
    {
        playSound(damage);
        healthController.ModifyValue(-attackDetails[0]);

        if (attackDetails[1] > transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }
        if (healthController.currentValue > 0f)
        {
            //slider.value -= attackDetails[0];
            Instantiate(VFX_hit, transform.position, Quaternion.identity);
            //   SwitchState(State.Knockback);

        }
        else
        {
            SwitchState(State.Dead);
            Instantiate(VFX_death, transform.position, Quaternion.identity);
        }
    }

    public void addHealth(int value)
    {
        healthController.ModifyValue(value);
    }
    public void addMana(int value)
    {
        ManaController.ModifyValue(value);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ArrowForward")
        {
            UIManager.instance.setMoveRight(false);
        }
    }

    private void playSound(AudioSource s)
    {
     //   if(!s.isPlaying)
       // {
            s.Play();
        //}
    }

    public void playFootSteps()
    {
        footstepsSFX.Play();
    }

}
