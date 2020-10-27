using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    //pre initaliziation
    protected Rigidbody2D myRigidbody;

    //variables for movement
    public float speed;
    protected Vector2 direction;

    //variables for character stats
    protected float health;

    // Start is called before the first frame update
    protected virtual void Start()
    {

        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        MoveCharacter();
    }

    //Simple Code for movement of the character's rigid body
    void MoveCharacter()
    {

        myRigidbody.velocity = new Vector2(direction.x * speed * Time.deltaTime, myRigidbody.velocity.y);

        //myRigidbody.MovePosition(myRigidbody.position + direction * speed * Time.deltaTime);

        //transform.Translate(direction * speed * Time.deltaTime);
    }

    //virtual method
    protected abstract void GetDirection();

    //Take Damage Algorithm
    void TakeDamage(float damage, float armor)
    {
        health -= (damage * (1 - armor/100));
    }

    //getter method for health
    float GetHealth()
    {
        return health;
    }
}
