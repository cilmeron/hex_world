using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Projector))]
public class ProjectorController : MonoBehaviour
{
    public Material baseMaterial; // Assign the base material in the Inspector.
    private Material _materialInstance; // This will be a unique instance of the base material.
    public bool shouldDisplayBorderOnly; // Set this in the Inspector.
    public float borderThickness = 0.05f; // Adjust the default value as needed.
    public Color circleColor = UnityEngine.Color.blue; // Set the initial color of the circle in the Inspector.
    public LayerMask displayLayers; // Specify the layers on which to display the circle.
    public float circleRadius = 0.5f; // Set the initial radius of the circle.

    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private static readonly int WorldPosProperty = Shader.PropertyToID("_WorldPos");
    private static readonly int ShouldDisplayBorderOnlyProperty = Shader.PropertyToID("_ShouldDisplayBorderOnly");
    private static readonly int RadiusProperty = Shader.PropertyToID("_Radius");
    private static readonly int BorderThicknessProperty = Shader.PropertyToID("_BorderSize");

    void Start()
    {
        // Create a unique instance of the material.
        _materialInstance = Instantiate(baseMaterial);

        // Get the projector and set its material to the unique instance.
        Projector projector = GetComponent<Projector>();
        projector.material = _materialInstance;

        // Set the layer mask for the projector.
        projector.ignoreLayers = ~displayLayers;

        // Set the initial properties of the material.
        UpdateMaterialProperties();
    }

    void OnDestroy()
    {
        // It's important to clean up the material instance when this object is destroyed.
        // Otherwise, you could have a memory leak in your game.
        if (_materialInstance != null)
        {
            Destroy(_materialInstance);
        }
    }

    public void UpdateMaterialProperties()
    {
        if (_materialInstance != null)
        {
            // Set the color of the circle.
            _materialInstance.SetColor(ColorProperty, new Color(circleColor.r, circleColor.g, circleColor.b, 80 / 255f));

            // Set the world position.
            Vector3 worldPosition = transform.position;
            _materialInstance.SetVector(WorldPosProperty, worldPosition);

            // Set whether to display only the border.
            float shouldDisplayBorderOnlyFloat = shouldDisplayBorderOnly ? 1 : 0;
            _materialInstance.SetFloat(ShouldDisplayBorderOnlyProperty, shouldDisplayBorderOnlyFloat);

            // Set the radius and border thickness.
            _materialInstance.SetFloat(RadiusProperty, circleRadius);
            _materialInstance.SetFloat(BorderThicknessProperty, borderThickness);
        }
    }

}
