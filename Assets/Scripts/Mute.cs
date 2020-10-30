using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mute : MonoBehaviour
{
    public AudioSource mute;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
           mute.mute = !mute.mute;
        }
    }
}
