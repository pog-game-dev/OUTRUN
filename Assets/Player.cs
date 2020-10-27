using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState
{
    run,
    attack,
    jump
}

public class Player : Character
{

    [SerializeField] private LayerMask groundLayerMask;

    private PlayerState currentState;
    
    //movement variables
    private bool isDashing;
    private bool isAttacking;
    private bool isRunning;
    private bool jump;
    private bool isShooting;

    //stats
    public float jumpForce;
    public float jumpTime;

    //components
    private Animator anim;

    private int numOfClicks = 0;
    private float lastClickedTime = 0f;
    private float maxComboDelay = 0f;


    // Start is called before the first frame update

    protected override void Start()
    {
        currentState = PlayerState.run;
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

        //Debug
        Debug.Log(isAttacking);
        Debug.Log(isShooting);


        //animations
        if (isRunning && !isAttacking && !isShooting)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }


        if (isAttacking && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            StartCoroutine(Attack1Co());
        }

        if (isShooting && !anim.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
        {
            StartCoroutine(ShootCo());
        }


        if (jump && isGrounded())
        {
            StartCoroutine(JumpCo());
        }


        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    //This method changes the direction based on what key is being pressed
    //It also flips the character model
    protected override void GetDirection()
    {
        direction = Vector2.zero;

        if (currentState != PlayerState.attack)
        {
            
            direction.x = Input.GetAxisRaw("Horizontal");


            //flips character model
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
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
    //animation coroutines
    private IEnumerator Attack1Co()
    {
        anim.SetBool("isRunning", false);
        anim.SetTrigger("attack1");
        yield return null;
        isAttacking = false;
        yield return new WaitForSeconds(0.21f);
        currentState = PlayerState.run;
    }

    private IEnumerator ShootCo()
    {
        anim.SetBool("isRunning", false);
        anim.SetTrigger("shoot");
        yield return null;
        isShooting = false;
        yield return new WaitForSeconds(0.21f);
        currentState = PlayerState.run;
    }

    private IEnumerator JumpCo()
    {
        myRigidbody.velocity = Vector2.up * jumpForce;
        anim.SetBool("isRunning", false);
        anim.SetTrigger("jump");
        yield return null;
        jump = false;
        yield return new WaitForSeconds(jumpTime);
        currentState = PlayerState.run;

    }

    //Boolean Activators
    void setJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            currentState = PlayerState.jump;
        }
        
    }
    
    
    void attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isAttacking = true;
            currentState = PlayerState.attack;
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
        }
        else
        {
            isDashing = false;
        }

    }
    bool isGrounded()
    {
        return transform.Find("GroundCheck").GetComponent<GroundCheck>().isGrounded;
    }

}
