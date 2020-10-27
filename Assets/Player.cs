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
    dash
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
    private bool facingRight;

    //stats
    public float jumpForce;
    public float dashForce;

    //components
    private Animator anim;

    public int numOfClicks = 0;
    private float lastClickedTime = 0f;
    private float maxComboDelay = 0.5f;


    // Start is called before the first frame update
    
    protected override void Start()
    {
        currentState = PlayerState.run;
        currentAttack = AttackState.none;
        base.Start();
        anim = GetComponent<Animator>();
    }

    //methods called every frame
    protected override void Update()
    {
        //movement
        attack();
        GetDirection();
        setJump();
        dash();
        shoot();
        setIsGrounded();

        //Debug
        Debug.Log("numOfClicks: " + numOfClicks);
        Debug.Log("currentATtack: " + currentAttack);
        Debug.Log("currentState: " + currentState);

        //Combo timer
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numOfClicks = 0;
            currentAttack = AttackState.none;
        }


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

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
    }

    
    //animation coroutines
    private IEnumerator Attack1Co()
    {
        currentAttack = AttackState.attack1;
        isAttacking = false;
        anim.SetBool("isRunning", false);
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
        anim.SetBool("isRunning", false);
        anim.SetTrigger("shoot");
        isShooting = false;
        yield return new WaitForSeconds(0.2f);
        currentState = PlayerState.run;
    }

    private IEnumerator JumpCo()
    {
        myRigidbody.velocity = Vector2.up * jumpForce;
        anim.SetBool("isRunning", false);
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
        anim.SetBool("isRunning", false);
        anim.SetTrigger("dash");
        yield return null;
        isDashing = false;
        yield return new WaitForSeconds(.3f);
        currentState = PlayerState.fall;

    }

    //This method changes the direction based on what key is being pressed
    //It also flips the character model
    protected override void GetDirection()
    {
        direction = Vector2.zero;

        if (currentState != PlayerState.attack && currentState != PlayerState.dash)
        {

            direction.x = Input.GetAxisRaw("Horizontal");
            

            //flips character model
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                facingRight = true;
            }

            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
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

}
