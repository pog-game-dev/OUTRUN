using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject menu;
    public bool paused;
    // Start is called before the first frame update
    private void Start()
    {
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButtonDown("Cancel"))
        {
            paused = !paused;
        }

        PauseGame();

        menu.SetActive(paused);
    }
    void PauseGame()
    {
        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

}
