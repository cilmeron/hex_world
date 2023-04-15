using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public float translationSpeed = 60f;
    public float altitude = 40f;

    private Camera _camera;
    private RaycastHit _hit;
    private Ray _ray;

    private Vector3 _forwardDir;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _forwardDir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            _TranslateCamera(0);
        if (Input.GetKey(KeyCode.RightArrow))
            _TranslateCamera(1);
        if (Input.GetKey(KeyCode.DownArrow))
            _TranslateCamera(2);
        if (Input.GetKey(KeyCode.LeftArrow))
            _TranslateCamera(3);
        if (Input.GetMouseButton(1))
        {
            float mousex = Input.GetAxis("Mouse X");
            float mousey = Input.GetAxis("Mouse Y");
            transform.Rotate(Vector3.down *(-mousex) * 10f);
            transform.Rotate(Vector3.right *(-mousey) * 10f);
        }
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0.0f)
        {
            wheel *= 5.0f;
            if (altitude >= 20f && altitude <= 80f)
            {
                altitude += -wheel;
            }
            else if (altitude <= 20f)
            {
                altitude = 20f;
            }
            else if (altitude >= 80f)
            {
                altitude = 80f;
            }
            _TranslateCamera(4);
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
                1000f
              //  Globals.TERRAIN_LAYER_MASK
            ))
            transform.position = _hit.point + Vector3.up * altitude;
    }
}