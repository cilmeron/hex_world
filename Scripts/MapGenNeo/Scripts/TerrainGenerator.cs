using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    ChunkGeneration chunkGen;
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
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
        meshCollider = GetComponent<MeshCollider>();

        meshRenderer.material = chunkGen.terrainMaterial;

        mesh = new Mesh();
        Vector3[] vertices = new Vector3[(int)((chunkGen.chunkResolution.x + 1) * (chunkGen.chunkResolution.y + 1))];
        uv = new Vector2[vertices.Length];
        int[] triangles;

        for (int i = 0, x = 0; x <= chunkGen.chunkResolution.x; x++)
        {
            for (int z = 0; z <= chunkGen.chunkResolution.y; z++)
            {
                // float y = NoiseGeneration.GenerateNoise(x, z, chunkGen.seed, chunkGen.noiseScale, chunkGen.octaves, chunkGen.persistance, chunkGen.lacunarity);
                float y = Noise(x, z, BiomeNoise(x, z));
                vertices[i] = new Vector3(x * (128 / chunkGen.chunkResolution.x), y, z * (128 / chunkGen.chunkResolution.y));
                float doesSpawn = Mathf.PerlinNoise(x + transform.position.x + chunkGen.seed, z + transform.position.z + chunkGen.seed);
                doesSpawn -= Mathf.PerlinNoise((x + transform.position.x) * 0.1f + chunkGen.seed, (z + transform.position.z) * 0.1f + chunkGen.seed);
                if (doesSpawn > chunkGen.treeThreshold && y > chunkGen.waterLevel + 5)
                {
                    float whatSpwans = Mathf.PerlinNoise(x + transform.position.x + (chunkGen.seed * 5), z + transform.position.z + (chunkGen.seed * 3));
                    whatSpwans = whatSpwans * chunkGen.trees.Length;
                    whatSpwans = Mathf.RoundToInt(whatSpwans);
                    float offset = Random.Range(0f, 10f);
                    offset = offset / 10f;
                    GameObject current = Instantiate(chunkGen.trees[(int)whatSpwans],
                        new Vector3(x * (128 / chunkGen.chunkResolution.x) + transform.position.x + offset, y + transform.position.y, z * (128 / chunkGen.chunkResolution.y) + transform.position.z + offset),
                        Quaternion.identity);
                    current.transform.parent = transform;
                }
                i++;

            }
        }

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        triangles = new int[(int)(chunkGen.chunkResolution.x * chunkGen.chunkResolution.y * 6)];

        int tris = 0;
        int verts = 0;

        for (int x = 0; x < chunkGen.chunkResolution.y; x++)
        {
            for (int z = 0; z < chunkGen.chunkResolution.x; z++)
            {
                triangles[tris] = verts;
                triangles[tris + 1] = verts + 1;
                triangles[tris + 2] = (int)(verts + chunkGen.chunkResolution.x + 1);
                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = (int)(verts + chunkGen.chunkResolution.x + 2);
                triangles[tris + 5] = (int)(verts + chunkGen.chunkResolution.x + 1);

                verts++;
                tris += 6;
            }
            verts++;
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = mesh;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

    }
    float Noise(float x, float z, float biomeNoise)
    {
        //Base Plate
        float y = biomeNoise * 150f;
        //Mountains
        float multiplier = 1 + Mathf.Pow(BiomeNoise(x, z), 3) * 10f;
        y = y * multiplier;
        y += (Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.003f, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.003f) * 200) * biomeNoise;
        //Hills
        y += (Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.007f, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.007f) * 70) * biomeNoise;
        return y;
    }

    float BiomeNoise(float x, float z)
    {
        float y = Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.0002f, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.0002f);
        y += Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.0007f, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.0007f);
        y -= Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * 0.00007f, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * 0.00007f) * 2;
        return y;
    }
}
