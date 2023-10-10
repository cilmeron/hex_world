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
        chunkGen = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkGeneration>();
        if (chunkGen == null)
        {
            return;
        }
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        meshRenderer.material = chunkGen.terrainMaterial;

        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        mesh = new Mesh();
        Vector3[] vertices = new Vector3[(int)((chunkGen.chunkResolution.x + 1) * (chunkGen.chunkResolution.y + 1))];
        uv = new Vector2[vertices.Length];
        int[] triangles;

        for (int i = 0, x = 0; x <= chunkGen.chunkResolution.x; x++)
        {
            for (int z = 0; z <= chunkGen.chunkResolution.y; z++)
            {
                float y = Noise(x, z);
                vertices[i] = new Vector3(x * (128 / chunkGen.chunkResolution.x), y, z * (128 / chunkGen.chunkResolution.y));
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
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = mesh;
        meshFilter.mesh = mesh;

    }
    float Noise(float x, float z)
    {
        return 0f;
    }
}
