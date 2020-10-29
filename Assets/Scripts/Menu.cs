using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public Button retry, quit;

    // Start is called before the first frame update
    public void Start()
    {
        retry.onClick.AddListener(Retry);
        quit.onClick.AddListener(QuitGame);
    }

    public void Retry()
    {
        SceneManager.LoadScene("Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
