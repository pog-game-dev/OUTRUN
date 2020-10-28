using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public abstract class Character : MonoBehaviour
{

    //pre initaliziation
    protected Rigidbody2D myRigidbody;

    //variables for movement
    public float gravityScale;
    public float jumpForce;
    public float speed;
    public float knockback;
    protected Vector2 direction;
    protected bool facingRight;

    //variables for character stats
    protected int health;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (facingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (!facingRight)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }


    //virtual method
    protected abstract IEnumerator hitCo();
    protected abstract IEnumerator deadCo();

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack"))
        {
            takeDamage();
        }
    }


    protected virtual void takeDamage()
    {
        health--;
        StartCoroutine(hitCo());

    }

}
