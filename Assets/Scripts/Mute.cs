using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mute : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetComponent<AudioSource>().mute = !GetComponent<AudioSource>().mute;
        }
    }
}
