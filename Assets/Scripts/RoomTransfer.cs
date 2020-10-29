using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransfer : MonoBehaviour
{
    public float MaxX;
    public float MinX;
    public float MaxY;
    public float MinY;
    private CameraMovement cam;

    public RoomTransfer area1, area2, area3, area4, area5;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.GetComponent<CameraMovement>();

    }

    // Update is called once per frame


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cam.maxPosition = new Vector2(MaxX, MaxY);
            cam.minPosition = new Vector2(MinX, MinY);

            area1.GetComponent<BoxCollider2D>().enabled = true;
            area2.GetComponent<BoxCollider2D>().enabled = true;
            area3.GetComponent<BoxCollider2D>().enabled = true;
            area4.GetComponent<BoxCollider2D>().enabled = true;
            area5.GetComponent<BoxCollider2D>().enabled = true;

            GetComponent<BoxCollider2D>().enabled = false;

        }
    }

}
