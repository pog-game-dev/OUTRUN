using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

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
    private PlayerState currentState;
    private AttackState currentAttack;
    
    //movement variables
    private bool isGrounded;
    private bool invincible;
    private bool isDead;
    private bool canDash;
    private bool canAttack;
    private bool isLoading;

    //stats
    public float dashForce;

    //components
    private Animator anim;
    private BoxCollider2D collider;

    public LayerMask groundLayer;
    public LayerMask gunLayer;

    public int numOfClicks = 0;
    private float lastClickedTime = 0f;
    private float maxComboDelay = 0.75f;

    private float ammo;
    public float reloadTime;
    private float reloadTimeCounter;

    


    // Start is called before the first frame update

    protected override void Start()
    {
        isLoading = true;
        canAttack = true;
        knockback = 1;
        canDash = true;
        isDead = false;
        isGrounded = true;
        facingRight = true;
        reloadTimeCounter = reloadTime;
        ammo = 3;
        invincible = false;
        health = 3;
        currentState = PlayerState.idle;
        currentAttack = AttackState.none;
        base.Start();
        anim = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        StartCoroutine(LoadCo());

    }

    //methods called every frame
    protected override void Update()
    {

        health = Mathf.Clamp(health, 0, 3);
        ammo = Mathf.Clamp(ammo, 0, 3);

        //Debug
        //Debug.Log("currentState: " + currentState);
        //Debug.Log("health: " + health);
        //Debug.Log("numofclicks" + numOfClicks);
        //Debug.Log("isDead" + isDead);

        
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

        if (!isDead && !isLoading)
        {
            //movement
            attack();
            jump();
            setIsGrounded();
            shoot();
            dash();

            //Debug
            //Debug.Log("isgrounded" + isGrounded);

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


        else if (isDead)
        {
            knockback = 0;
            myRigidbody.velocity = Vector2.zero;
        }

        base.Update();
    }

    private void FixedUpdate()
    {
        if(!isDead && canMove() && !isLoading)
            Move();
    }
    //animation coroutines
    private IEnumerator Attack1Co()
    {
        canAttack = false;
        currentAttack = AttackState.attack1;
        anim.SetTrigger("attack1");
        swingSound.Play();
        yield return new WaitForSeconds(0.1f);
        currentState = PlayerState.idle;
        yield return new WaitForSeconds(0.2f);
        canAttack = true;
    }

    private IEnumerator Attack2Co()
    {
        canAttack = false;
        currentAttack = AttackState.attack2;
        numOfClicks = 0;
        anim.SetTrigger("attack2");
        swingSound.Play();
        yield return new WaitForSeconds(0.3f);
        canAttack = true;
        currentAttack = AttackState.none;
        currentState = PlayerState.idle;
    }

    private IEnumerator ShootCo()
    {
        ammo--;
        anim.SetTrigger("shoot");
        shootSound.Play();

        RaycastHit2D hit;

        if (facingRight)
        {
            hit = Physics2D.Raycast(transform.Find("Gun").GetComponent<Transform>().position, Vector2.right, gunLayer);
            Debug.Log(hit.collider);

            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<Enemy>().damage();
            }
            else if (hit.collider.CompareTag("Boss"))
            {
                hit.collider.GetComponent<Boss>().damage();
            }

        }

        else if (!facingRight)
        {
            hit = Physics2D.Raycast(transform.Find("Gun").GetComponent<Transform>().position, Vector2.left, gunLayer);
            Debug.Log(hit.collider);

            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<Enemy>().damage();
            }
            else if (hit.collider.CompareTag("Boss"))
            {
                hit.collider.GetComponent<Boss>().damage();
            }
        }

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
        canDash = false;
        currentState = PlayerState.dash;
        if (facingRight)
        {
            //myRigidbody.MovePosition(transform.position + Vector3.right * dashForce);
            myRigidbody.MovePosition(new Vector2(transform.position.x + dashForce, transform.position.y));
        }
        else
        {
            //myRigidbody.MovePosition(transform.position + Vector3.left * dashForce);
            myRigidbody.MovePosition(new Vector2(transform.position.x - dashForce, transform.position.y));
        }
        anim.SetTrigger("dash");
        swingSound.Play();
        yield return new WaitForSeconds(0.25f);
        currentState = PlayerState.run;
        yield return new WaitForSeconds(0.5f);
        canDash = true;

    }

    protected override IEnumerator hitCo()
    {
        invincible = true;
        currentState = PlayerState.damaged;
        anim.SetTrigger("hit");
        yield return new WaitForSeconds(0.1f);
        damagedSound.Play(); 
        yield return new WaitForSeconds(0.05f);
        if (health <= 0 && currentState != PlayerState.dead)
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
        isGrounded = true;
    }

    protected override IEnumerator deadCo()
    {
        anim.SetBool("isDead", true);
        isDead = true;
        yield return null;
    }

    private IEnumerator LoadCo()
    {
        yield return new WaitForSeconds(3.0f);
        GetComponent<SpriteRenderer>().enabled = true;
        isLoading = false;
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

        if (Input.GetButtonDown("Fire1") && isGrounded && canAttack)
        {
            myRigidbody.velocity = Vector2.zero;
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
            
        }


    }

    void shoot()
    {
        if (Input.GetButtonDown("Fire2") && ammo > 0 && isGrounded)
        {
            myRigidbody.velocity = Vector2.zero;
            StartCoroutine(ShootCo());
        }
    }
    void dash()
    {
        if (Input.GetButtonDown("Dash") && canDash)
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

    private bool canMove()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") || anim.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
        {
            return false;
        }
        else
        {
            return true;
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

    public void damage()
    {
        takeDamage();
    }
    public bool getIsDead()
    {
        return isDead;
    }
}
