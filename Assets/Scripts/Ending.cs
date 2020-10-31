using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{

    public GameObject background;
    public GameObject fade;
    public GameObject credit;
    public GameObject names;
    public GameObject thanks;
    // Start is called before the first frame update

    public float creditAppear;
    public float creditTime;
    public float namesTime;
    public float fadeTime;
    void Start()
    {
        StartCoroutine(sequence());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private IEnumerator sequence()
    {
        yield return new WaitForSeconds(creditAppear);
        credit.SetActive(true);
        yield return new WaitForSeconds(creditTime);
        names.SetActive(true);
        yield return new WaitForSeconds(namesTime);
        fade.GetComponent<Animator>().SetTrigger("fade");
        yield return new WaitForSeconds(fadeTime);
        thanks.SetActive(true);
        yield return new WaitForSeconds(36.0f - creditAppear - creditTime - namesTime - fadeTime);
        SceneManager.LoadScene("MainMenu");
    }

}
