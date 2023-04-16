using UnityEngine;
using TMPro;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private float destroyTime = 3f;
    [SerializeField] private float verticalSpeed = 0.5f;

    private void Start()
    {
        mainCamera = Camera.main;
        Destroy(gameObject, destroyTime);
    }

    private void LateUpdate()
    {
        // Make the billboard always face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);

        // Move the billboard upwards
        transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
    }

    public TextMeshProUGUI TMP
    {
        get => tmp;
        set => tmp = value;
    }
}