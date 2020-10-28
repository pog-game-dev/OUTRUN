using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransfer : MonoBehaviour
{
    public float cameraChangeMaxX;
    public float cameraChangeMinX;
    public float cameraChangeMaxY;
    public float cameraChangeMinY;
    private bool shift;
    private CameraMovement cam;

    // Start is called before the first frame update
    void Start()
    {
        bool shift = true;
        cam = Camera.main.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            shift = !shift;
            if (shift)
            {
                cam.maxPosition.x += cameraChangeMaxX;
                cam.minPosition.x += cameraChangeMinX;
                cam.maxPosition.y += cameraChangeMaxY;
            }
            else if (!shift)
            {
                cam.maxPosition.x -= cameraChangeMaxX;
                cam.minPosition.x -= cameraChangeMinX;
                cam.maxPosition.y -= cameraChangeMaxY;
                cam.maxPosition.y -= cameraChangeMinY;
            }
        }
    }
}
