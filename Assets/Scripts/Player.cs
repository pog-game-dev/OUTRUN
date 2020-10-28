using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerState
{
    run,
    attack,
    jump,
    fall,
    dash,
    damaged,
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
    private bool isDashing;
    private bool isAttacking;
    private bool isRunning;
    private bool jump;
    private bool isShooting;
    private bool isGrounded;
    private bool invincible;

    //stats
    public float dashForce;

    //components
    private Animator anim;

    public int numOfClicks = 0;
    private float lastClickedTime = 0f;
    private float maxComboDelay = 0.5f;


    // Start is called before the first frame update
    
    protected override void Start()
    {
        invincible = false;
        health = 3;
        currentState = PlayerState.run;
        currentAttack = AttackState.none;
        base.Start();
        anim = GetComponent<Animator>();
    }

    //methods called every frame
    protected override void Update()
    {

        health = Mathf.Clamp(health, 0, 3);

        //Debug
        //Debug.Log("currentState: " + currentState);
        //Debug.Log("health: " + health);

        //Combo timer
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numOfClicks = 0;
            currentAttack = AttackState.none;
        }

        if(currentState != PlayerState.run)
        {
            anim.SetBool("isRunning", false);
        }

        if (currentState != PlayerState.dead)
        {
            //movement
            attack();
            GetDirection();
            setJump();
            dash();
            shoot();
            setIsGrounded();

            //animations
            if (isRunning && !isAttacking && !isShooting)
            {
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }

            if (currentState != PlayerState.fall && currentState != PlayerState.jump)
            {
                if (isAttacking && numOfClicks >= 1 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && currentAttack == AttackState.none)
                {
                    isAttacking = false;
                    StartCoroutine(Attack1Co());
                }

                else if (isAttacking && numOfClicks >= 2 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && currentAttack == AttackState.attack1)
                {
                    StartCoroutine(Attack2Co());
                }


                if (isShooting && !anim.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
                {
                    StartCoroutine(ShootCo());
                }

                if (isDashing && !anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
                {
                    StartCoroutine(DashCo());
                }
            }
            base.Update();
        }

        else if(currentState == PlayerState.dead)
        {
            direction = Vector2.zero;
        }


        if (jump && isGrounded)
        {
            StartCoroutine(JumpCo());
            
        }

        if (currentState == PlayerState.fall && isGrounded)
        {

             anim.SetTrigger("land");
             currentState = PlayerState.run;
            
        }

        base.Update();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void MoveCharacter()
    {

        myRigidbody.velocity = new Vector2(direction.x * speed * Time.deltaTime, myRigidbody.velocity.y);
        //myRigidbody.velocity += new Vector2(0, myRigidbody.velocity.y);


        //myRigidbody.MovePosition(myRigidbody.position + direction * speed * Time.deltaTime);

        //myRigidbody.MovePosition(new Vector2(myRigidbody.position.x + direction.x * speed * Time.deltaTime, myRigidbody.position.y));


        //transform.Translate(direction * speed * Time.deltaTime);
    }
    //animation coroutines
    private IEnumerator Attack1Co()
    {
        currentAttack = AttackState.attack1;
        isAttacking = false;
        anim.SetTrigger("attack1");
        yield return new WaitForSeconds(0.1f);
        currentState = PlayerState.run;
    }

    private IEnumerator Attack2Co()
    {
        currentAttack = AttackState.attack2;
        numOfClicks = 0;
        isAttacking = false;
        anim.SetTrigger("attack2");
        yield return new WaitForSeconds(0.1f);
        currentAttack = AttackState.none;
        currentState = PlayerState.run;
    }

    private IEnumerator ShootCo()
    {
        anim.SetTrigger("shoot");
        isShooting = false;
        yield return new WaitForSeconds(0.2f);
        currentState = PlayerState.run;
    }

    private IEnumerator JumpCo()
    {
        myRigidbody.velocity = Vector2.up * jumpForce;
        anim.SetTrigger("jump");
        jump = false;
        yield return new WaitForSeconds(.3f);
        currentState = PlayerState.fall;
    }

    private IEnumerator DashCo()
    {
        if (facingRight)
        {
            myRigidbody.MovePosition(transform.position + Vector3.right * dashForce);
        }
        else
        {
            myRigidbody.MovePosition(transform.position + Vector3.left * dashForce);
        }
        anim.SetTrigger("dash");
        yield return null;
        isDashing = false;
        yield return new WaitForSeconds(.3f);
        currentState = PlayerState.fall;

    }

    protected override IEnumerator hitCo()
    {

        if (facingRight)
        {
            myRigidbody.MovePosition(new Vector2(myRigidbody.position.x - knockback, myRigidbody.position.y));
        }
        else if (!facingRight)
        {
            myRigidbody.MovePosition(new Vector2(myRigidbody.position.x + knockback, myRigidbody.position.y));
        }

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
        yield return null;
    }

    //This method changes the direction based on what key is being pressed
    //It also flips the character model
    void GetDirection()
    {
        direction = Vector2.zero;

        if (currentState != PlayerState.attack && currentState != PlayerState.dash)
        {

            direction.x = Input.GetAxisRaw("Horizontal");
            

            //flips character model
            if (direction.x > 0)
            {
                facingRight = true;
            }

            else if (direction.x < 0)
            {
                facingRight = false;
            }


            //This activates isWalking boolean for animations and other logic
            if (direction != Vector2.zero)
            {
                isRunning = true;
            }

            else
            {
                isRunning = false;
            }

        }
    }

    //Boolean Activators
    void setJump()
    {
        if (Input.GetButtonDown("Jump") && currentState != PlayerState.jump)
        {
            jump = true;
            currentState = PlayerState.jump;
        }
        
    }
    
    
    void attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            lastClickedTime = Time.time;
            if (currentAttack != AttackState.attack2)
            {
                numOfClicks++;
            }
            isAttacking = true;
            currentState = PlayerState.attack;
            numOfClicks = Mathf.Clamp(numOfClicks, 0, 2);
        }

    }

    void shoot()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            isShooting = true;
            currentState = PlayerState.attack;
        }

    }
    void dash()
    {
        if (Input.GetButtonDown("Dash"))
        {
            isDashing = true;
            currentState = PlayerState.dash;
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

}
