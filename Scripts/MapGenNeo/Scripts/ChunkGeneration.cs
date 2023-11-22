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

    [Range(0, 1000)]
    public float BasePlateFactor = 150f;
    [Range(0, 0.05f)]
    public float MountainSpread = 0.003f;
    [Range(0, 0.05f)]
    public float HillSpread = 0.007f;
    [Range(0, 1000)]
    public float MountainSpreadScale = 200f;
    [Range(0, 1000)]
    public float HillSpreadScale = 70f;
    [Range(0, 1)]
    public float BiomeFineScale = 0.0002f;
    [Range(0, 1)]
    public float BiomeMidScale = 0.0007f;
    [Range(0, 1)]
    public float BiomeCorseScale = 0.00007f;

    public int seed;

    private void Start()
    {
        waterLevel = Mathf.PerlinNoise(seed, seed) * 256;
        waterLevel = -30;
        StartCoroutine(GenerateChunks());
        GameObject current = Instantiate(water, new Vector3((128 * chunks.x) / 2, waterLevel, (128 * chunks.y) / 2), Quaternion.identity);
        current.transform.localScale = new Vector3(16 * chunks.x, 128, 16 * chunks.y);
    }

    public void EditorStart()
    {
        waterLevel = Mathf.PerlinNoise(seed, seed) * 256;
        waterLevel = -30;
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
