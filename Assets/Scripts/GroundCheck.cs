using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    public bool isGrounded;
  
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log(collision);

        isGrounded = collision.CompareTag("Ground") || collision.CompareTag("Enemy") || collision.CompareTag("Boss");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }
}
