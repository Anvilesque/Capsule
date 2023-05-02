using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleMouseDrag : MonoBehaviour
{
    private Camera cam;
    private Vector3 mousePos;
    private Vector2 offset;
    private bool offsetCalibrated;
    public bool isEnabled;
    public bool dragSnapObjToCenter;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        dragSnapObjToCenter = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;
        // Constantly update mousePos with mouse position based on camera
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        if (!isEnabled) return;
        if (dragSnapObjToCenter)
        {
            transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
        }
        else
        {
            if (!offsetCalibrated)
            {
                offset = transform.position - mousePos;
                offsetCalibrated = true;
            }
            transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, 0f);
        }
    }

    private void OnMouseUp()
    {
        if (!isEnabled) return;
        offsetCalibrated = false;
    }
}
