using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Ammo : MonoBehaviour
{
    public Player player;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("ammo", player.getAmmo());
    }
}
