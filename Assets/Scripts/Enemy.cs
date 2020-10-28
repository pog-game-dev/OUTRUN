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
    private bool isAttacking;
    private bool isPatrolling;
    public float patrolDistance;
    public float patrolSpeed;
    public float detectionLength;

    private Animator anim;
    public GameObject enemy;

    private EnemyState currentState;

    // Start is called before the first frame update
    protected override void Start()
    {
        health = 2;
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
        //Debug.Log("currentState: " + currentState);
        Debug.Log(isPatrolling);

        isPatrolling = !getIsPatrolling();
        health = Mathf.Clamp(health, 0, 2);

        if (currentState != EnemyState.dead)
        {
            if (currentState == EnemyState.walk && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                StartCoroutine(attackCo());
            }

            if (isPatrolling)
            {
                StartCoroutine(patrolCo());
            }

        }

        
        base.Update();
    }

    private IEnumerator attackCo()
    {
        isAttacking = false;
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

        if (facingRight)
        {
            myRigidbody.MovePosition(new Vector2(myRigidbody.position.x - knockback, myRigidbody.position.y));
        }
        else if (!facingRight)
        {
            myRigidbody.MovePosition(new Vector2(myRigidbody.position.x + knockback, myRigidbody.position.y));
        }

        currentState = EnemyState.damaged;
        anim.SetTrigger("hit");
        yield return new WaitForSeconds(0.25f);

        if(health == 0 && currentState != EnemyState.dead)
        {
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
        anim.SetBool("isDead", true);
        enemy.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(2.0f);
        //enemy.SetActive(false);
    }

    protected override void takeDamage()
    {
        if (currentState != EnemyState.dead)
        {
            base.takeDamage();
        }
    }

    private IEnumerator patrolCo()
    {
        //transform.Translate(Vector2.)
        yield return null;
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
