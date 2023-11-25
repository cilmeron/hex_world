using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public NetworkManager networkManager;
    public float translationSpeed = 60f;
    public float altitude = 4f;

    private Camera _camera;
    private RaycastHit _hit;
    private Ray _ray;
    private float distancetoground = 4f;
    private Vector3 _forwardDir;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _forwardDir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        altitude = transform.position.y;
    }

    void Update()
    {
        if (!networkManager.chatactive)
        {
            if (Input.GetAxis("Vertical") > 0)
                _TranslateCamera(0);
            if (Input.GetAxis("Horizontal") > 0)
                _TranslateCamera(1);
            if (Input.GetAxis("Vertical") < 0)
                _TranslateCamera(2);
            if (Input.GetAxis("Horizontal") < 0)
                _TranslateCamera(3);
            
        }
        float wheel = 0f;
        if (Input.GetAxis("Height") != 0f && !networkManager.chatactive)
        {
            wheel = Input.GetAxis("Height")*0.01f;
        }
        else
        {
            wheel = Input.GetAxis("Mouse ScrollWheel");
        }
        if (wheel != 0.0f)
        {
            wheel *= (translationSpeed/3f);
            if (altitude >= 4f && altitude <= 80f)
            {
                altitude += -wheel;
            }
            else if (altitude <= 3f)
            {
                altitude = 4f;
            }
            else if (altitude >= 80f)
            {
                altitude = 80f;
            }
            _TranslateCamera(4);
        }
        if (Input.GetMouseButton(1))
        {
            float mousex = Input.GetAxis("Mouse X");
            float mousey = Input.GetAxis("Mouse Y");
            transform.Rotate(Vector3.up *(mousex) * 10f, Space.World);
            transform.Rotate(Vector3.right *(-mousey) * 10f);
        }
       
    }

    private void _TranslateCamera(int dir)
    {
        if (dir == 0)       // top
            transform.Translate(Vector3.forward * Time.deltaTime * translationSpeed);
        else if (dir == 1)  // right
            transform.Translate(Vector3.right * Time.deltaTime * translationSpeed);
        else if (dir == 2)  // bottom
            transform.Translate(Vector3.back * Time.deltaTime * translationSpeed);
        else if (dir == 3)  // left
            transform.Translate(Vector3.left * Time.deltaTime * translationSpeed);
        
        // translate camera at proper altitude: cast a ray to the ground
        // and move up the hit point
        _ray = new Ray(transform.position, Vector3.up * -1000f);
        if (Physics.Raycast(
                _ray,
                out _hit,
                5000f,
              1 << 8
            ))
            {    
                transform.position = _hit.point + Vector3.up * altitude;  
            }
    }
}