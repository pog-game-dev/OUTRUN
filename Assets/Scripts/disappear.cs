using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappear : MonoBehaviour
{

    public GameObject map;
    public float animationTime;
    public AudioSource carClick;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        map.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(1.28f);
        carClick.Play();
        yield return new WaitForSeconds(animationTime - 1.28f);
        map.SetActive(false);
    }
}
