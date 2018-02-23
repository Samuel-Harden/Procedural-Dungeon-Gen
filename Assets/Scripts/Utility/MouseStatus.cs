using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseStatus : MonoBehaviour
{
    public bool mouseLeftDown;
    public bool mouseRightDown;
    private bool mouseWheelDown;

    [SerializeField] Camera camera;
    [SerializeField] int maxZoom = 5;
    [SerializeField] int minZoom = 30;

    private float speed = 50; // Pan Speed


    private void Update()
    {
        HandleMouseButtons();

        HandlePan();

        HandleZoom();
    }


    private void HandleMouseButtons()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseLeftDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseLeftDown = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            mouseRightDown = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            mouseRightDown = false;
        }

        if (Input.GetMouseButtonDown(2))
        {
            mouseWheelDown = true;
        }

        if (Input.GetMouseButtonUp(2))
        {
            mouseWheelDown = false;
        }
    }


    private void HandlePan()
    {
        if (mouseWheelDown)
            camera.transform.position += new Vector3(Input.GetAxis("Mouse X") * Time.deltaTime * speed, 0.0f,
                Input.GetAxis("Mouse Y") * Time.deltaTime * speed);
    }


    private void HandleZoom()
    {
        float zoom = 0;

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            zoom++;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            zoom--;

        camera.transform.position = new Vector3(camera.transform.position.x,
            camera.transform.position.y + zoom, camera.transform.position.z);

        // Limit Zoom
        if (camera.transform.position.y < 5)
            camera.transform.position = new Vector3(camera.transform.position.x, maxZoom,
                camera.transform.position.z);

        else if (camera.transform.position.y > 30)
            camera.transform.position = new Vector3(camera.transform.position.x, minZoom,
                camera.transform.position.z);
    }
}
