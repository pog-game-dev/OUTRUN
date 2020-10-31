using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum EnemyState
{
    walk,
    attack,
    damaged,
    dead
}

public class Enemy : Character
{
    public GameController gameController;

    private bool isWalking;
    public bool isPatrolling;
    public float patrolSpeed;
    private float patrolTime;
    public float patrolMaxTime;
    public float detectionLength;

    private bool isDead;

    private Transform target;

    private Animator anim;
    public GameObject enemy;

    private EnemyState currentState;

    // Start is called before the first frame update
    protected override void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        patrolTime = patrolMaxTime;
        health = 3;
        anim = GetComponent<Animator>();
        currentState = EnemyState.walk;
        isPatrolling = true;
        isWalking = true;
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        //Debug
        //Debug.Log("health: " + health);
        //Debug.Log("currentState: " + currentState);
        //Debug.Log(isPatrolling);


        if (!isDead && currentState != EnemyState.dead)
        {
            //isPatrolling = !getIsPatrolling();

            if (isPatrolling && currentState == EnemyState.walk)
            {
                patrol();

                if (patrolTime > 0)
                {
                    patrolTime -= Time.deltaTime;
                }
                else
                {
                    patrolTime = patrolMaxTime;
                    facingRight = !facingRight;
                }
            }
            else if (!isPatrolling && currentState == EnemyState.walk)
            {
                searchAttack();
            }

            if (currentState == EnemyState.walk)
            {
                anim.SetBool("isWalking", true);
            }
            else if (currentState != EnemyState.walk)
            {
                anim.SetBool("isWalking", false);
            }

        }
        else if (isDead)
        {
            StartCoroutine(deadCo());
            transform.Find("Vision").GetComponent<BoxCollider2D>().enabled = false;
        }

        base.Update();
    }

    private IEnumerator attackCo()
    {
        currentState = EnemyState.attack;
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(2.25f);
        if (currentState != EnemyState.dead)
        {
            currentState = EnemyState.walk;
        }
        isWalking = true;
    }
    protected override IEnumerator hitCo()
    {
        StopCoroutine(attackCo());
        Debug.Log("hit");

        currentState = EnemyState.damaged;
        anim.SetTrigger("hit");
        yield return new WaitForSeconds(0.1f);
        damagedSound.Play();
        yield return new WaitForSeconds(0.15f);

        if(health <= 0 && currentState != EnemyState.dead)
        {
            isDead = true;
            gameController.deadEnemyUp();
            currentState = EnemyState.dead;
            StartCoroutine(deadCo());
        }
        else
        {
            currentState = EnemyState.walk;
        }
    }
    
    protected override IEnumerator deadCo()
    {
        StopCoroutine(attackCo());
        currentState = EnemyState.dead;
        myRigidbody.bodyType = RigidbodyType2D.Static;
        anim.SetBool("isDead", true);
        enemy.GetComponent<BoxCollider2D>().enabled = false;
        yield return null;
        //enemy.SetActive(false);
    }

    protected override void takeDamage()
    {
        if (currentState != EnemyState.dead)
        {
            base.takeDamage();
        }
    }

    public void damage()
    {
        takeDamage();
    }

    void patrol()
    {
        if (patrolTime > 0)
        {
            currentState = EnemyState.walk;
            if (facingRight)
            {
                myRigidbody.velocity = new Vector2(patrolSpeed, myRigidbody.velocity.y);
            }

            else if (!facingRight)
            {
                myRigidbody.velocity = new Vector2(-patrolSpeed, myRigidbody.velocity.y);
            }
        }
    }

    void searchAttack()
    {
        if (currentState != EnemyState.dead)
        {

            if (Mathf.Abs(Vector2.Distance(transform.position, target.position)) > 2.0f)
            {
                currentState = EnemyState.walk;
                //transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
                if (facingRight && isWalking) 
                { 
                    myRigidbody.velocity = new Vector2(patrolSpeed, myRigidbody.velocity.y); 
                }
                else if (!facingRight && isWalking)
                {
                    myRigidbody.velocity = new Vector2(-patrolSpeed, myRigidbody.velocity.y);
                }
            }
            else
            {
                isWalking = false;
                myRigidbody.velocity = new Vector2(0, 0);
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                    StartCoroutine(attackCo());
            }
        }
    }

    /*
    private bool getIsPatrolling()
    {
        
        RaycastHit2D hit;
        if (facingRight) 
        {
            hit = Physics2D.Raycast(transform.Find("Vision").GetComponent<Transform>().position, Vector2.right, detectionLength);
        }
        else
        {
            hit = Physics2D.Raycast(transform.Find("Vision").GetComponent<Transform>().position, Vector2.left, detectionLength);
        }
        return hit.collider != null && hit.collider.CompareTag("Player");
        
    }
    */
    

}
