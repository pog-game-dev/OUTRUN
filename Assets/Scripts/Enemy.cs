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
    private bool isPatrolling;
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
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        //Debug
        //Debug.Log("health: " + health);
        Debug.Log("currentState: " + currentState);
        Debug.Log(isPatrolling);


        health = Mathf.Clamp(health, 0, 10);
        

        if (!isDead && currentState != EnemyState.dead)
        {
            isPatrolling = !getIsPatrolling();

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

    }
    protected override IEnumerator hitCo()
    {
        Debug.Log("hit");

        currentState = EnemyState.damaged;
        anim.SetTrigger("hit");
        yield return new WaitForSeconds(0.25f);

        if(health == 0 && currentState != EnemyState.dead)
        {
            isDead = true; ;
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
        currentState = EnemyState.dead;
        transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
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

            if (Mathf.Abs(Vector2.Distance(transform.position, target.position)) > 1.6f)
            {
                currentState = EnemyState.walk;
                //transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
                myRigidbody.velocity = new Vector2(patrolSpeed, myRigidbody.velocity.y);
            }
            else
            {
                myRigidbody.velocity = new Vector2(0, 0);
                StartCoroutine(attackCo());
            }
        }
    }

    
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
    

}
