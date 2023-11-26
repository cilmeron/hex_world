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
      if (IsMouseInGameWindow() && !networkManager.chatactive)
        {
            if (Input.GetAxis("Vertical") > 0)
                _TranslateCamera(0);
            if (Input.GetAxis("Horizontal") > 0)
                _TranslateCamera(1);
            if (Input.GetAxis("Vertical") < 0)
                _TranslateCamera(2);
            if (Input.GetAxis("Horizontal") < 0)
                _TranslateCamera(3);

            float wheel = 0f;
            if (Input.GetAxis("Height") != 0f && !networkManager.chatactive)
            {
                wheel = Input.GetAxis("Height") * 0.01f;
            }
            else
            {
                wheel = Input.GetAxis("Mouse ScrollWheel");
            }
            if (wheel != 0.0f)
            {
                wheel *= (translationSpeed / 3f);
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
                transform.Rotate(Vector3.up * (mousex) * 10f, Space.World);
                transform.Rotate(Vector3.right * (-mousey) * 10f);
            }
        }
    }
    private bool IsMouseInGameWindow()
    {
        // Get the mouse position in pixels
        Vector3 mousePosition = Input.mousePosition;

        // Check if the mouse position is within the game window
        return mousePosition.x >= 0 && mousePosition.x <= Screen.width &&
            mousePosition.y >= 0 && mousePosition.y <= Screen.height;
    }
    private void _TranslateCamera(int dir)
    {
        Vector3 translation = Vector3.zero;
        
        if (dir == 0)       // top
            translation = Vector3.forward;
        else if (dir == 1)  // right
            translation = Vector3.right;
        else if (dir == 2)  // bottom
            translation = Vector3.back;
        else if (dir == 3)  // left
            translation = Vector3.left;

        // Apply translation with speed
        transform.Translate(translation * Time.deltaTime * translationSpeed);

        // Calculate the camera's target position at the proper altitude
        Vector3 targetPosition = transform.position;
        
        // Cast a ray to the ground and move up to the hit point
        _ray = new Ray(transform.position, Vector3.up * -1000f);

        if (Physics.Raycast(_ray, out _hit, 5000f, 1 << 8))
        {
            targetPosition.y = _hit.point.y + altitude;
        }

        // Clamp the camera position based on the boundary GameObject
         
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(targetPosition.x, -240f, 240f),
                targetPosition.y,
                Mathf.Clamp(targetPosition.z, -240f, 240f)
            );

            // Apply the clamped position to the camera
            transform.position = clampedPosition;
        
    }
    private Vector3 GetTopLeftCornerPosition(Transform planeTransform)
    {
        Vector3 localScale = planeTransform.localScale;
        Vector3 localPosition = planeTransform.localPosition;

        float halfWidth = localScale.x / 2f;
        float halfDepth = localScale.z / 2f;

        Vector3 localTopLeft = new Vector3(-halfWidth, 0f, halfDepth);

        // Transform the local position to world space
        return planeTransform.TransformPoint(localTopLeft);
    }

    private Vector3 GetTopRightCornerPosition(Transform planeTransform)
    {
        Vector3 localScale = planeTransform.localScale;
        Vector3 localPosition = planeTransform.localPosition;

        float halfWidth = localScale.x / 2f;
        float halfDepth = localScale.z / 2f;

        Vector3 localTopRight = new Vector3(halfWidth, 0f, halfDepth);

        // Transform the local position to world space
        return planeTransform.TransformPoint(localTopRight);
    }

    private Vector3 GetBottomLeftCornerPosition(Transform planeTransform)
    {
        Vector3 localScale = planeTransform.localScale;
        Vector3 localPosition = planeTransform.localPosition;

        float halfWidth = localScale.x / 2f;
        float halfDepth = localScale.z / 2f;

        Vector3 localBottomLeft = new Vector3(-halfWidth, 0f, -halfDepth);

        // Transform the local position to world space
        return planeTransform.TransformPoint(localBottomLeft);
    }

    private Vector3 GetBottomRightCornerPosition(Transform planeTransform)
    {
        Vector3 localScale = planeTransform.localScale;
        Vector3 localPosition = planeTransform.localPosition;

        float halfWidth = localScale.x / 2f;
        float halfDepth = localScale.z / 2f;

        Vector3 localBottomRight = new Vector3(halfWidth, 0f, -halfDepth);

        // Transform the local position to world space
        return planeTransform.TransformPoint(localBottomRight);
    }
}