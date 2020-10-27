using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBoxBehaviour : MonoBehaviour
{
    public GameObject chat;

    private bool isVisible;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isVisible)
            chat.GetComponent<SpriteRenderer>().enabled = true;
        else
            chat.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Player"))
        {
            isVisible = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isVisible = false;
        }
    }
}
