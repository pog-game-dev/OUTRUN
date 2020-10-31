using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public abstract class Character : MonoBehaviour
{

    //pre initaliziation
    public Rigidbody2D myRigidbody;

    //variables for movement
    public float jumpForce;
    public float speed;
    public float knockback;
    protected Vector2 direction;
    protected bool facingRight;

    //Audio Cues
    public AudioSource swingSound;
    public AudioSource damagedSound;
    public AudioSource shootSound;

    //variables for character stats
    public int health;

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
        float direction = (myRigidbody.position.x - collision.transform.position.x) / Mathf.Abs(myRigidbody.position.x - collision.transform.position.x);
        if (collision.CompareTag("Attack"))
        {
            myRigidbody.MovePosition(new Vector2(myRigidbody.position.x + direction * knockback, myRigidbody.position.y));
            takeDamage();
        }
    }

    protected virtual void takeDamage()
    {
        health--;
        StartCoroutine(hitCo());

    }

}
