using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button play, tutorial, quit;
    public GameObject tutorialMessage;

    public void Start()
    {
        play.onClick.AddListener(PlayGame);
        tutorial.onClick.AddListener(Tutorial);
        quit.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            tutorialMessage.SetActive(false);
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void Tutorial()
    {
        tutorialMessage.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
