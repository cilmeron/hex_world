using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager instance;
    public Vector2 worldSize;
    public int resolution = 16;

    public float islandRadius = 900f;

    public Material terrainMat;


    public Vector2 worldCentre;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        worldCentre = new Vector2((worldSize.x / 2f) * 128f, (worldSize.y / 2f) * 128f);

        StartCoroutine(GenerateChunks());
    }

    IEnumerator GenerateChunks()
    {
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.y; y++)
            {
                TerrainGeneratorNova tg = new TerrainGeneratorNova();

                GameObject current = new GameObject("Terrain" + (x * y), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                current.transform.parent = transform;
                current.transform.localPosition = new Vector3(x * 128, 0f, y * 128f);

                tg.Init(current);
                tg.Generate(terrainMat);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}

class TerrainGeneratorNova
{
    MeshFilter filter;
    MeshRenderer renderer;
    MeshCollider collider;
    Mesh mesh;

    Vector3[] verts;
    int[] triangles;
    Vector2[] uvs;

    public void Init(GameObject cur)
    {
        filter = cur.GetComponent<MeshFilter>();
        renderer = cur.GetComponent<MeshRenderer>();
        collider = cur.GetComponent<MeshCollider>();
        mesh = new Mesh();
    }

    public void Generate(Material mat)
    {
        Vector2 worldPos = new Vector2(filter.gameObject.transform.localPosition.x, filter.gameObject.transform.localPosition.z);
        int resolution = ChunkManager.instance.resolution;

        verts = new Vector3[(ChunkManager.instance.resolution + 1) * (ChunkManager.instance.resolution + 1)];
        uvs = new Vector2[verts.Length];

        Vector2 worldCenter = ChunkManager.instance.worldCentre;

        for (int i = 0, x = 0; x <= resolution; x++)
        {
            for (int z = 0; z <= resolution; z++)
            {
                Vector2 vertexWorldPos = new Vector2(worldPos.x + (x * (128f / resolution)), worldPos.y + (z * (128f / resolution)));
                float islandRadius = ChunkManager.instance.islandRadius;

                float distance = Vector2.Distance(worldCenter, vertexWorldPos);
                float sin = Mathf.Sin(Mathf.Clamp(((1 + distance) / islandRadius), 0f, 1f) + 90f);
                float islandMultiplier = sin * Mathf.PerlinNoise(vertexWorldPos.x * 0.005f, vertexWorldPos.y * 0.005f);

                islandMultiplier += Mathf.PerlinNoise(vertexWorldPos.x * 0.01f, vertexWorldPos.y * 0.01f) * 0.5f * sin;
                islandMultiplier += Mathf.PerlinNoise(vertexWorldPos.x * 0.02f, vertexWorldPos.y * 0.02f) * 0.3f * sin;
                islandMultiplier += Mathf.PerlinNoise(vertexWorldPos.x * 0.007f, vertexWorldPos.y * 0.007f) * 0.3f * sin;

                float y = islandMultiplier * 150f;

                verts[i] = new Vector3(x * (128f / resolution), y, z * (128f / resolution));

                i++;
            }
        }

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(verts[i].x + worldPos.x, verts[i].z + worldPos.y);
        }

        triangles = new int[resolution * resolution * 6];
        int tris = 0;
        int vert = 0;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + 1;
                triangles[tris + 2] = (int)(vert + resolution + 1);
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = (int)(vert + resolution + 2);
                triangles[tris + 5] = (int)(vert + resolution + 1);

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        collider.sharedMesh = mesh;
        filter.mesh = mesh;
        renderer.material = mat;
    }
}