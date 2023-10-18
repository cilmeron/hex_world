using System.Collections;
using UnityEngine;

public class ChunkGeneration : MonoBehaviour
{
    public Vector2 chunks;
    public Vector2 chunkResolution;

    public Material terrainMaterial;

    public GameObject water;
    public float waterLevel;
    public GameObject[] trees;
    public float treeRandomness;
    public float treeThreshold;

    public Transform player;
    public int chunksChunkLoaded;

    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;

    private void Start()
    {
        waterLevel = Mathf.PerlinNoise(seed, seed) * 256;
        StartCoroutine(GenerateChunks());
        GameObject current = Instantiate(water, new Vector3((128 * chunks.x) / 2, waterLevel, (128 * chunks.y) / 2), Quaternion.identity);
        current.transform.localScale = new Vector3(16 * chunks.x, 128, 16 * chunks.y);
    }

    public IEnumerator GenerateChunks()
    {
        for (int i = 0, x = 0; x < chunks.x; x++)
        {
            for (int z = 0; z < chunks.y; z++)
            {
                GameObject current = new GameObject("Terrain" + new Vector2(x * 128, z * 128),
                    typeof(TerrainGenerator),
                    typeof(MeshRenderer),
                    typeof(MeshCollider),
                    typeof(MeshFilter));
                current.transform.parent = transform;
                current.transform.position = new Vector3(x * (chunkResolution.x) * (128 / chunkResolution.x),
                    0f,
                    z * (chunkResolution.y) * (128 / chunkResolution.y));
                i++;
                if (i == chunksChunkLoaded)
                {
                    i = 0;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }
        }
    }

    public void GenerateChunksEditor()
    {
        for (int i = 0, x = 0; x < chunks.x; x++)
        {
            for (int z = 0; z < chunks.y; z++)
            {
                GameObject current = new GameObject("Terrain" + new Vector2(x * 128, z * 128),
                    typeof(TerrainGenerator),
                    typeof(MeshRenderer),
                    typeof(MeshCollider),
                    typeof(MeshFilter));
                current.transform.parent = transform;
                current.transform.position = new Vector3(x * (chunkResolution.x) * (128 / chunkResolution.x),
                    0f,
                    z * (chunkResolution.y) * (128 / chunkResolution.y));
                i++;
            }
        }
    }
}
