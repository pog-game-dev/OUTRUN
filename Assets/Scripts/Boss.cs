using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum BossState
{
    walk,
    attack,
    dead
}

public class Boss : Character
{
    private BossState currentState;
    private Animator anim;
    public GameObject boss;
    public Player target;

    public GameController controller;

    public bool isDead;
    public bool isActive;
    public bool inMeleeRange;
    private bool isWalking;
    private bool invincible;
    private bool isAttacking;

    public float minAttackDelay;
    public float maxAttackDelay;
    private float attackDelay;

    public float teleportChance;
    private float teleportCounter = 0f;
    // Start is called before the first frame update
    protected override void Start()
    {
        attackDelay = maxAttackDelay;
        isWalking = false;
        facingRight = false;
        health = 15;
        isActive = false;
        isDead = false;
        anim = GetComponent<Animator>();
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Debug.Log("isactive " + isActive);
        //Debug.Log("CurrentState" + currentState);
        //Debug.Log("health" + health);
        //Debug.Log("dead " + isDead);
        if (!isDead && isActive)
        {
            bossBehaviour();
            

            //walking
            if (currentState == BossState.walk && isWalking && !inMeleeRange)
            {
                anim.SetBool("isWalking", true);
            }
            else if(currentState == BossState.walk && !isWalking)
            {
                anim.SetBool("isWalking", false);
            }
            else if (currentState != BossState.walk)
            {
                anim.SetBool("isWalking", false);
            }


            if (!isWalking)
            {
                myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            }
        }

        base.Update();
    }

    //coroutines

    private IEnumerator attackCo()
    {
        isAttacking = true;
        isWalking = false;
        currentState = BossState.attack;
        if (inMeleeRange)
        {
            swingSound.Play();
            int attack = UnityEngine.Random.Range(1, 3);
            anim.SetTrigger("attack" + attack);
        }
        else if (!inMeleeRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Laser"))
        {
            anim.SetTrigger("attack3");
            yield return new WaitForSeconds(0.6f);
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Laser"))
            {
                shootSound.Play();
            }
        }
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
        invincible = false;
        yield return new WaitForSeconds(0.5f);
        if (currentState != BossState.dead)
        {
            currentState = BossState.walk;
        }
        isWalking = true;

    }

    protected override IEnumerator hitCo()
    {
        isWalking = false;
        invincible = true;
        anim.SetTrigger("hit");
        yield return new WaitForSeconds(0.15f);
        damagedSound.Play();
        invincible = false;


        if (health <= 0 && currentState != BossState.dead)
        {
            isDead = true; ;
            currentState = BossState.dead;
            StartCoroutine(deadCo());
        }
        else
        {
            currentState = BossState.walk;
        }
        yield return new WaitForSeconds(0.3f);
        isWalking = true;
    }

    private IEnumerator teleportCo()
    {
        isWalking = false;
        anim.SetTrigger("teleport");
        yield return new WaitForSeconds(0.6f);
        facingRight = target.getFacingRight();
        transform.position = target.transform.Find("Teleport").transform.position;
        inMeleeRange = true;
        StartCoroutine(attackCo());
        attackDelay = UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);

    }


    protected override IEnumerator deadCo()
    {
        isWalking = false;
        isDead = true;
        anim.SetBool("isDead", true);
        transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        boss.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.6f);
    }

    protected override void takeDamage()
    {
        if (!invincible && currentState != BossState.dead)
        {
            isActive = true;
            base.takeDamage();
        }
    }

    private void bossBehaviour()
    {
        if (currentState == BossState.walk)
        {
            teleportTimer();
            attackTimer();
            if (isWalking && !inMeleeRange)
            {
                setFacingRight();

                if (facingRight)
                {
                    myRigidbody.velocity = new Vector2(speed, myRigidbody.velocity.y);
                }
                else if (!facingRight)
                {
                    myRigidbody.velocity = new Vector2(-speed, myRigidbody.velocity.y);
                }
            }
        }

    }

    private void attackTimer()
    {
        if (controller.getGameState() == GameState.genocide)
        {
            if (attackDelay <= 0.0f || inMeleeRange)
            {
                invincible = true;
                isWalking = false;
                StartCoroutine(attackCo());
                attackDelay = UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);
            }
            else
            {
                attackDelay -= Time.deltaTime;
            }
        }
        else
        {
            if (attackDelay <= 0.0f)
            {
                invincible = true;
                isWalking = false;
                StartCoroutine(attackCo());
                attackDelay = UnityEngine.Random.Range(minAttackDelay, maxAttackDelay);
            }
            else
            {
                attackDelay -= Time.deltaTime;
            }
        }
    }

    private void teleportTimer()
    {
        float foo = 10;
        teleportCounter += Time.deltaTime;
        if(teleportCounter >= 1f)
        {
            teleportCounter = 0;
            foo = UnityEngine.Random.Range(0.0f, 10.0f);
            //Debug.Log("chance calculated" + foo);

        }
        if (foo < teleportChance && currentState != BossState.attack && target.transform.Find("Teleport").GetComponent<TeleportCheck>().canTeleport == true && !isAttacking)
        {
            //Debug.Log("TELEPORT");
            isWalking = false;
            StartCoroutine(teleportCo());
        }
    }

    public void setActive()
    {
        isActive = true;
        isWalking = true;
        target.transform.Find("Teleport").GetComponent<BoxCollider2D>().enabled = true;
    }

    public void damage()
    {
        takeDamage();
    }

    private void setFacingRight()
    {
        if (target.transform.position.x - transform.position.x > 0)
        {
            facingRight = true;
        }
        else
        {
            facingRight = false;
        }
    }
}
