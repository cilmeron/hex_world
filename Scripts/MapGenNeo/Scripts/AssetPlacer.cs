using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPlacer : MonoBehaviour
{
    public GameObject[] trees;
    public GameObject[] bushes;
    public GameObject[] rocks;

    public float treeRandomness;

    public float treeThreshold;
    public float bushThreshold;
    public float rockThreshold;

    private ChunkGeneration chunkGen;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AssetPlacement(TerrainGenerator[] terrainGenerators)
    {
        chunkGen = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkGeneration>();
        foreach (TerrainGenerator terrainGenerator in terrainGenerators)
        {
            terrainGenerator;
        }
    }

    bool DoesSpwan(int x, int z, float threshold)
    {
        float doesSpawn = Mathf.PerlinNoise(x + transform.position.x + chunkGen.seed, z + transform.position.z + chunkGen.seed);
        doesSpawn -= Mathf.PerlinNoise((x + transform.position.x) * 0.5f + chunkGen.seed, (z + transform.position.z) * 0.5f + chunkGen.seed);
        if (doesSpawn > threshold) return true;
        return false;
    }

    void SpawnAsset(int x, float y, int z, float threshold, float waterLevel, GameObject[] assetList)
    {
        bool spawns = DoesSpwan(x, z, threshold);
        if (spawns && y > waterLevel + 30)
        {
            int whatSpawns = Mathf.RoundToInt(Random.Range(0f, assetList.Length - 1));
            float offset = Random.Range(0f, 10f);
            offset = offset / 10f;

            GameObject current = Instantiate(assetList[(int)whatSpawns],
                new Vector3(x * (128 / chunkGen.chunkResolution.x) + transform.position.x + offset, y + transform.position.y, z * (128 / chunkGen.chunkResolution.y) + transform.position.z + offset),
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));

            float randomScaleFactor = Random.Range(0.8f, 1.2f);
            current.transform.localScale = new Vector3(randomScaleFactor, randomScaleFactor, randomScaleFactor);

            current.transform.parent = transform;
        }
    }
}
