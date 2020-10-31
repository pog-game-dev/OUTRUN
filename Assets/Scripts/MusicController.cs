using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource mainIntro;
    public AudioSource mainLoop;
    public AudioSource bossIntro;
    public AudioSource bossLoop;

    private bool check;

    private void Start()
    {
        check = true;
        mainIntro.Play();
        mainLoop.PlayDelayed(mainIntro.clip.length);
    }
    
    public void switchMusic()
    {
        mainIntro.Stop();
        mainLoop.Stop();
        mainIntro.enabled = false;
        mainLoop.enabled = false;

        if (check)
        {
            bossIntro.Play();
            bossLoop.PlayDelayed(bossIntro.clip.length);
            check = false;
        }
    }

    public void changePitchTo(float foo)
    {
        bossIntro.pitch = foo;
        bossLoop.pitch = foo;
        mainIntro.pitch = foo;
        mainLoop.pitch = foo;
    }

    public void stopAll()
    {
        mainIntro.Stop();
        mainLoop.Stop();
        bossIntro.Stop();
        bossLoop.Stop();
    }

    
}

