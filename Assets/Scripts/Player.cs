using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerState
{
    idle,
    run,
    attack,
    jump,
    fall,
    damaged,
    dash,
    dead
}

public enum AttackState
{
    none,
    attack1,
    attack2,
}

public class Player : Character
{

    [SerializeField] private LayerMask groundLayerMask;

    private PlayerState currentState;
    private AttackState currentAttack;
    
    //movement variables
    private bool isGrounded;
    private bool invincible;
    private bool isDead;

    //stats
    public float dashForce;

    //components
    private Animator anim;

    public int numOfClicks = 0;
    private float lastClickedTime = 0f;
    private float maxComboDelay = 0.3f;

    private float ammo;
    [SerializeField] private float reloadTime;
    private float reloadTimeCounter;


    // Start is called before the first frame update

    protected override void Start()
    {
        isDead = false;
        facingRight = true;
        reloadTimeCounter = reloadTime;
        ammo = 3;
        invincible = false;
        health = 3;
        currentState = PlayerState.idle;
        currentAttack = AttackState.none;
        base.Start();
        anim = GetComponent<Animator>();
    }

    //methods called every frame
    protected override void Update()
    {

        health = Mathf.Clamp(health, 0, 3);
        ammo = Mathf.Clamp(ammo, 0, 3);

        //Debug
        Debug.Log("currentState: " + currentState);
        //Debug.Log("health: " + health);
        Debug.Log("numofclicks" + numOfClicks);

        
        //Reload timer
        if(ammo < 3)
        {
            if(reloadTimeCounter > 0)
            {
                reloadTimeCounter -= Time.deltaTime;
            }
            else
            {
                ammo++;
                reloadTimeCounter = reloadTime;
            }
        }

        if (currentState != PlayerState.dead && !isDead)
        {
            //movement
            attack();
            jump();
            setIsGrounded();
            shoot();
            dash();

            //Debug
            Debug.Log("isgrounded" + isGrounded);

            if (currentState == PlayerState.idle || currentState != PlayerState.run)
            {
                anim.SetBool("isRunning", false);
            }
            else if (currentState == PlayerState.run)
            {
                anim.SetBool("isRunning", true);
            }

            if(currentState == PlayerState.fall)
            {
                anim.SetBool("isFalling", true);
            }
            else if (currentState != PlayerState.fall)
            {
                anim.SetBool("isFalling", false);
            }


            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fall") && isGrounded)
            {
                anim.SetTrigger("land");
                currentState = PlayerState.idle;
            }

        }


        else if (currentState == PlayerState.dead)
        {
            direction = Vector2.zero;
        }

        base.Update();
    }

    private void FixedUpdate()
    {
        Move();
    }
    //animation coroutines
    private IEnumerator Attack1Co()
    {
        currentAttack = AttackState.attack1;
        anim.SetTrigger("attack1");
        yield return new WaitForSeconds(0.3f);
        currentState = PlayerState.idle;
    }

    private IEnumerator Attack2Co()
    {
        currentAttack = AttackState.attack2;
        numOfClicks = 0;
        anim.SetTrigger("attack2");
        yield return new WaitForSeconds(0.3f);
        currentAttack = AttackState.none;
        currentState = PlayerState.idle;
    }

    private IEnumerator ShootCo()
    {
        ammo--;
        anim.SetTrigger("shoot");
        yield return new WaitForSeconds(0.3f);
        currentState = PlayerState.idle;
    }

    private IEnumerator JumpCo()
    {
        myRigidbody.velocity = Vector2.up * jumpForce;
        anim.SetTrigger("jump");
        yield return new WaitForSeconds(.3f);
        currentState = PlayerState.fall;
    }

    private IEnumerator DashCo()
    {
        currentState = PlayerState.dash;
        if (facingRight)
        {
            myRigidbody.MovePosition(transform.position + Vector3.right * dashForce);
        }
        else
        {
            myRigidbody.MovePosition(transform.position + Vector3.left * dashForce);
        }
        anim.SetTrigger("dash");
        yield return new WaitForSeconds(0.5f);
        currentState = PlayerState.fall;

    }

    protected override IEnumerator hitCo()
    {

        invincible = true;
        currentState = PlayerState.damaged;
        anim.SetTrigger("hit");
        yield return new WaitForSeconds(0.15f);
        if (health == 0 && currentState != PlayerState.dead)
        {
            currentState = PlayerState.dead;
            StartCoroutine(deadCo());
        }
        else
        {
            currentState = PlayerState.run;
        }
        yield return new WaitForSeconds(0.5f);
        invincible = false;
    }

    protected override IEnumerator deadCo()
    {
        anim.SetBool("isDead", true);
        isDead = true;
        yield return null;
    }

    void Move()
    {
            direction.x = Input.GetAxisRaw("Horizontal");
            myRigidbody.velocity = new Vector2(direction.x * speed * Time.deltaTime, myRigidbody.velocity.y);

            if (direction.x > 0)
            {
                facingRight = true;
            }
            else if (direction.x < 0)
            {
                facingRight = false;
            }
            
            if (direction.x != 0 && direction.y == 0)
            {
                currentState = PlayerState.run ;
            }
            else if(direction.x == 0 && direction.y == 0)
            {
                currentState = PlayerState.idle;
            }
        
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && currentState != PlayerState.jump && isGrounded)
        {
            StartCoroutine(JumpCo());
        }
    }
    
    void attack()
    {
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numOfClicks = 0;
            currentAttack = AttackState.none;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            lastClickedTime = Time.time;
            if (currentAttack != AttackState.attack2)
            {
                numOfClicks++;
            }

            if(numOfClicks >= 1 && currentAttack == AttackState.none)
            {
                StartCoroutine(Attack1Co());
            }

            else if (numOfClicks >= 2 && currentAttack == AttackState.attack1)
            {
                StartCoroutine(Attack2Co());
            }
            numOfClicks = Mathf.Clamp(numOfClicks, 0, 2);
        }


    }

    void shoot()
    {
        if (Input.GetButtonDown("Fire2") && ammo > 0)
        {
            StartCoroutine(ShootCo());
        }
    }
    void dash()
    {
        if (Input.GetButtonDown("Dash"))
        {
            StartCoroutine(DashCo());
        }
    }
    void setIsGrounded()
    {
        isGrounded = transform.Find("GroundCheck").GetComponent<GroundCheck>().isGrounded;   
    }

    protected override void takeDamage()
    {
        if (!invincible && currentState != PlayerState.dead)
        {
            health--;
            StartCoroutine(hitCo());
        }
    }

    public float getHealth()
    {
        return health;
    }

    public float getAmmo()
    {
        return ammo;
    }
    public bool getFacingRight()
    {
        return facingRight;
    }
}
