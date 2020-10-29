using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappear : MonoBehaviour
{

    public GameObject map;
    public float animationTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        map.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(animationTime);
        map.SetActive(false);
    }
}
