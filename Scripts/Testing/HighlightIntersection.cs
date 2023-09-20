using UnityEngine;
using System.Collections.Generic;

public class HighlightIntersection : MonoBehaviour
{
    public Material highlightMaterial; // The material to use for highlighting

    [SerializeField] private Collider sphereCollider;
    private GameObject highlightObject;
    private MeshFilter highlightMeshFilter;
    private MeshRenderer highlightRenderer;

    private void Start()
    {
        // Create the highlight object
        highlightObject = new GameObject("HighlightObject");
        highlightObject.transform.SetParent(transform);
        highlightObject.transform.localPosition = Vector3.zero;
        highlightObject.transform.localScale = Vector3.one;

        // Add a mesh renderer to the highlight object
        highlightRenderer = highlightObject.AddComponent<MeshRenderer>();

        // Set the highlight material on the renderer
        highlightRenderer.material = highlightMaterial;

        // Add a mesh filter to the highlight object
        highlightMeshFilter = highlightObject.AddComponent<MeshFilter>();

        // Disable the highlight object by default
        highlightObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the collider intersects with the environment
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Get all the triangles of the environment mesh
            Mesh environmentMesh = other.GetComponent<MeshFilter>().mesh;
            int[] environmentTriangles = environmentMesh.triangles;
            Vector3[] environmentVertices = environmentMesh.vertices;

            // Find the triangles that intersect with the collider
            List<int> intersectingTriangles = new List<int>();
            for (int i = 0; i < environmentTriangles.Length; i += 3)
            {
                int triangleIndex = i / 3;
                Vector3 v1 = environmentVertices[environmentTriangles[i]];
                Vector3 v2 = environmentVertices[environmentTriangles[i + 1]];
                Vector3 v3 = environmentVertices[environmentTriangles[i + 2]];
                if (TriangleColliderIntersection.TestTriangle(sphereCollider, v1, v2, v3))
                {
                    intersectingTriangles.Add(environmentTriangles[i]);
                    intersectingTriangles.Add(environmentTriangles[i + 1]);
                    intersectingTriangles.Add(environmentTriangles[i + 2]);
                }
            }

            // Create a new mesh with the intersecting triangles
            Mesh highlightMesh = new Mesh();
            highlightMesh.vertices = environmentMesh.vertices;
            highlightMesh.triangles = intersectingTriangles.ToArray();
            highlightMesh.RecalculateNormals();

            // Assign the mesh to the mesh filter of the highlight object
            highlightMeshFilter.mesh = highlightMesh;

            // Enable the highlight object
            highlightObject.SetActive(true);
        }
        else
        {
            // Disable the highlight object if the collider is not intersecting with the environment
            if (highlightObject != null)
                highlightObject.SetActive(false);
        }
    }
}
