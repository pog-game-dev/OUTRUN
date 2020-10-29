using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCheck : MonoBehaviour
{
    public bool canTeleport;
    // Start is called before the first frame update
    void Start()
    {
        canTeleport = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canTeleport = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canTeleport = true;
    }

}
