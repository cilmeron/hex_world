using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrainGenerator : MonoBehaviour
{
    ChunkGeneration chunkGen;

    public Mesh mesh;
    public List<float> heightDeltas = new List<float>();
    public float averageHeight;
    public Vector2 worldPosition = Vector2.zero;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider collider;
    Vector2[] uv;

    private void Start()
    {
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
        HeightMapAnalyzer heightMapAnalyzer = new HeightMapAnalyzer();

        meshRenderer.material = chunkGen.terrainMaterial;

        mesh = new Mesh();
        Vector3[] vertices = new Vector3[(int)((chunkGen.chunkResolution.x + 1) * (chunkGen.chunkResolution.y + 1))];
        uv = new Vector2[vertices.Length];
        int[] triangles;

        Vector2 worldPos = new Vector2(meshFilter.gameObject.transform.localPosition.x, meshFilter.gameObject.transform.localPosition.z);
        Vector2 worldCenter = chunkGen.worldCentre;
        worldPosition = worldPos;
        averageHeight = 0;
        for (int i = 0, x = 0; x <= chunkGen.chunkResolution.x; x++)
        {
            for (int z = 0; z <= chunkGen.chunkResolution.y; z++)
            {
                Vector2 vertexWorldPos = new Vector2(worldPos.x + (x * (128f / chunkGen.chunkResolution.x)),
                    worldPos.y + (z * (128f / chunkGen.chunkResolution.y)));
                float islandRadius = chunkGen.islandRadius;
                float distance = Vector2.Distance(worldCenter, vertexWorldPos);
                float sin = Mathf.Sin(Mathf.Clamp(((1 + distance) / islandRadius), 0f, 1f) + 90f);
                float islandMultiplier = sin * Mathf.PerlinNoise(vertexWorldPos.x * 0.005f, vertexWorldPos.y * 0.005f);

                islandMultiplier += Mathf.PerlinNoise(vertexWorldPos.x * 0.01f, vertexWorldPos.y * 0.01f) * 0.5f * sin;
                islandMultiplier += Mathf.PerlinNoise(vertexWorldPos.x * 0.02f, vertexWorldPos.y * 0.02f) * 0.3f * sin;
                islandMultiplier += Mathf.PerlinNoise(vertexWorldPos.x * 0.007f, vertexWorldPos.y * 0.007f) * 0.3f * sin;

                float y = islandMultiplier * 150f;
                averageHeight = (i * averageHeight + y) / (i + 1);

                //float y = Noise(x, z, BiomeNoise(x, z));
                vertices[i] = new Vector3(x * (128 / chunkGen.chunkResolution.x), y, z * (128 / chunkGen.chunkResolution.y));
                chunkGen.UpdateMinMaxHeight(y);
                i++;
            }
        }
        heightDeltas = heightMapAnalyzer.CalculateAverageDifferences(vertices);
        float average = heightDeltas.Average();
        for (int i = 0; i < uv.Length; i++)
        {
            vertices[i].y = ModifyHeight(vertices[i].y);
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

    public float ModifyHeight(float input)
    {
        float scaledValue = input + chunkGen.heightOffSet;
        return scaledValue;
    }

    public void GenerateTerrainNova()
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

        int seed = 100;
        int octaves = 5;
        float persistance = 0.5f;
        float scale = 55f;
        float lacunarity = 1.8f;


        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        Vector2 offset = Vector2.zero;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x; //offset is the current chunk position
            //float offsetX = prng.Next(-100000, 100000); //offset is the current chunk position
            float offsetY = prng.Next(-100000, 100000) - offset.y; //offset is the current chunk position
            //float offsetY = prng.Next(-100000, 100000); //offset is the current chunk position
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }



        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float halfWidth = 128 / 2f;
        float halfHeight = 128 / 2f;

        for (int i = 0, x = 0; x <= chunkGen.chunkResolution.x; x++)
        {
            for (int z = 0; z <= chunkGen.chunkResolution.y; z++)
            {
                float y = Noise(x, z, BiomeNoise(x, z));

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int j = 0; j < octaves; j++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[j].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[j].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                /*
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                */

                y = noiseHeight * 20;

                vertices[i] = new Vector3(x * (128 / chunkGen.chunkResolution.x), y, z * (128 / chunkGen.chunkResolution.y));
                chunkGen.UpdateMinMaxHeight(y);
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
