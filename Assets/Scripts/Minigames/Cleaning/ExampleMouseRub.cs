using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleMouseRub : MonoBehaviour
{
    private Camera cam;
    private Vector3 mousePos;
    private Vector3 prevPos;
    public bool isEnabled;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;
        // Constantly update mousePos with mouse position based on camera
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        
    }
}
