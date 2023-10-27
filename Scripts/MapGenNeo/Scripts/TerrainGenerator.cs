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
        Vector3[] detailVerts = new Vector3[(128 + 1) * (128 + 1)];
        uv = new Vector2[vertices.Length];
        int[] triangles;

        for (int i = 0, x = 0; x <= chunkGen.chunkResolution.x; x++)
        {
            for (int z = 0; z <= chunkGen.chunkResolution.y; z++)
            {
                float y = Noise(x, z, BiomeNoise(x, z));
                vertices[i] = new Vector3(x * (128 / chunkGen.chunkResolution.x), y, z * (128 / chunkGen.chunkResolution.y));
                float doesSpawn = Mathf.PerlinNoise(x + transform.position.x + chunkGen.seed, z + transform.position.z + chunkGen.seed);
                doesSpawn -= Mathf.PerlinNoise((x + transform.position.x) * 0.5f + chunkGen.seed, (z + transform.position.z) * 0.5f + chunkGen.seed);
                if (doesSpawn > chunkGen.treeThreshold && y > chunkGen.waterLevel + 1)
                {
                    float whatSpawns = Mathf.PerlinNoise(x + transform.position.x + (chunkGen.seed * 5), z + transform.position.z + (chunkGen.seed * 3));
                    whatSpawns = whatSpawns * chunkGen.trees.Length;
                    whatSpawns = Mathf.RoundToInt(whatSpawns);
                    float offset = Random.Range(0f, 10f);
                    offset = offset / 10f;
                    GameObject current = Instantiate(chunkGen.trees[(int)whatSpawns], new Vector3(x * (128 / chunkGen.chunkResolution.x) + transform.position.x + offset, y + transform.position.y, z * (128 / chunkGen.chunkResolution.y) + transform.position.z + offset), Quaternion.identity);
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

    float Noise(float x, float z, float biomeNoise)
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

    float BiomeNoise(float x, float z)
    {
        float y = Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * chunkGen.BiomeFineScale, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * chunkGen.BiomeFineScale);
        y += Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * chunkGen.BiomeMidScale, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * chunkGen.BiomeMidScale);
        y -= Mathf.PerlinNoise(((x * (128 / chunkGen.chunkResolution.x)) + transform.position.x + chunkGen.seed) * chunkGen.BiomeCorseScale, ((z * (128 / chunkGen.chunkResolution.y)) + transform.position.z + chunkGen.seed) * chunkGen.BiomeCorseScale) * 2;
        return y;
    }
}