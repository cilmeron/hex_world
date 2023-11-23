using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    ChunkGeneration chunkGen;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider collider;
    Vector2[] uv;

    private void Start()
    {
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        chunkGen = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkGeneration>();
        if (chunkGen == null)
        {
            return;
        }
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<MeshCollider>();

        meshRenderer.material = chunkGen.terrainMaterial;

        mesh = new Mesh();
        Vector3[] vertices = new Vector3[(int)((chunkGen.chunkResolution.x + 1) * (chunkGen.chunkResolution.y + 1))];
        uv = new Vector2[vertices.Length];
        int[] triangles;

        for (int i = 0, x = 0; x <= chunkGen.chunkResolution.x; x++)
        {
            for (int z = 0; z <= chunkGen.chunkResolution.y; z++)
            {
                float y = Noise(x, z, BiomeNoise(x, z));
                vertices[i] = new Vector3(x * (128 / chunkGen.chunkResolution.x), y, z * (128 / chunkGen.chunkResolution.y));
                chunkGen.UpdateMinMaxHeight(vertices[i]);
                i++;
            }
        }

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        triangles = new int[(int)(chunkGen.chunkResolution.x * chunkGen.chunkResolution.y * 6)];

        int tris = 0;
        int vert = 0;

        for (int x = 0; x < chunkGen.chunkResolution.y; x++)
        {
            for (int z = 0; z < chunkGen.chunkResolution.x; z++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = (int)(vert + chunkGen.chunkResolution.x + 1);
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = (int)(vert + chunkGen.chunkResolution.x + 1);
                triangles[tris + 5] = (int)(vert + chunkGen.chunkResolution.x + 2);

                vert++;
                tris += 6;
            }

            vert++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateBounds();
        mesh.triangles = mesh.triangles.Reverse().ToArray();
        collider.sharedMesh = mesh;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    public float Noise(float x, float z, float biomeNoise)
    {
        //Base Plate
        float y = biomeNoise * chunkGen.BasePlateFactor;
        //Mountains
        float multiplier = 1 + Mathf.Pow(BiomeNoise(x, z), 3) * 10f;
        y = y * multiplier;
        y += (Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * chunkGen.MountainSpread, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * chunkGen.MountainSpread) * chunkGen.MountainSpreadScale) * biomeNoise;
        //Hills
        y += (Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * chunkGen.HillSpread, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * chunkGen.HillSpread) * chunkGen.HillSpreadScale) * biomeNoise;
        return y;
    }

    public float BiomeNoise(float x, float z)
    {
        float y = Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * chunkGen.BiomeFineScale, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * chunkGen.BiomeFineScale);
        y += Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * chunkGen.BiomeMidScale, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * chunkGen.BiomeMidScale);
        y -= Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * chunkGen.BiomeCorseScale, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * chunkGen.BiomeCorseScale) * 2;
        return y;
    }

    List<Vector3> GetAssetPlacementCoordinates()
    {
        List<Vector3> validCoordinates = new List<Vector3>();

        for (int x = 0; x <= chunkGen.chunkResolution.x; x++)
        {
            for (int z = 0; z <= chunkGen.chunkResolution.y; z++)
            {
                float vertexHeight = Noise(x, z, BiomeNoise(x, z));

                // Check the slope at the current vertex
                if (IsSuitableSlope(x, z))
                {
                    Vector3 coordinate = new Vector3(x * (128 / chunkGen.chunkResolution.x), vertexHeight, z * (128 / chunkGen.chunkResolution.y));
                    validCoordinates.Add(coordinate);
                }
            }
        }

        return validCoordinates;
    }

    bool IsSuitableSlope(int x, int z)
    {
        float slopeThreshold = 30f;  // Adjust as needed
        float slope = CalculateSlope(x, z);

        return slope <= slopeThreshold;
    }

    float CalculateSlope(int x, int z)
    {
        // Use central differences to approximate the gradient
        float heightCenter = Noise(x, z, BiomeNoise(x, z));
        float heightXPlus = Noise(x + 1, z, BiomeNoise(x + 1, z));
        float heightXMinus = Noise(x - 1, z, BiomeNoise(x - 1, z));
        float heightZPlus = Noise(x, z + 1, BiomeNoise(x, z + 1));
        float heightZMinus = Noise(x, z - 1, BiomeNoise(x, z - 1));

        float gradientX = (heightXPlus - heightXMinus) / 2f;
        float gradientZ = (heightZPlus - heightZMinus) / 2f;

        // Calculate the slope angle in degrees
        float slope = Mathf.Rad2Deg * Mathf.Atan(Mathf.Sqrt(gradientX * gradientX + gradientZ * gradientZ));

        return slope;
    }
}
