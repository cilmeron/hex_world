using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor.ShaderGraph.Internal;
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
        chunkGen.GetMinHeight();
        chunkGen.GetMaxHeight();
        foreach (TerrainGenerator terrainGenerator in terrainGenerators)
        {
            for (int i = 0, x = 0; x <= chunkGen.chunkResolution.x; x++)
            {
                for (int z = 0; z <= chunkGen.chunkResolution.y; z++)
                {
                    // Spawn Assets
                    Vector3 vertices = terrainGenerator.mesh.vertices[i];
                    float y = vertices.y;
                    float slope = terrainGenerator.heightDeltas[i];
                    SpawnAsset(x, y, z, slope,
                        chunkGen.treeThreshold, chunkGen.waterLevel, chunkGen.treeMinSlopeThreshold, chunkGen.treeMaxSlopeThreshold,
                        terrainGenerator, chunkGen.trees);
                    SpawnAsset(x, y, z, slope,
                        chunkGen.bushThreshold, chunkGen.waterLevel, chunkGen.bushMinSlopeThreshold, chunkGen.bushMaxSlopeThreshold,
                        terrainGenerator, chunkGen.bushes);
                    SpawnAsset(x, y, z, slope,
                        chunkGen.rockThreshold, chunkGen.waterLevel, chunkGen.rockMinSlopeThreshold, chunkGen.rockMaxSlopeThreshold,
                        terrainGenerator, chunkGen.rocks);

                    i++;
                }
            }
        }
    }

    bool DoesSpwan(int x, int z, float slope, float minSlope, float maxSlope, float threshold, TerrainGenerator terrainChunk)
    {
        if (slope < minSlope || slope > maxSlope) return false;
        float doesSpawn = Mathf.PerlinNoise(x + terrainChunk.transform.position.x + chunkGen.seed, z + terrainChunk.transform.position.z + chunkGen.seed);
        doesSpawn -= Mathf.PerlinNoise((x + terrainChunk.transform.position.x) * 0.5f + chunkGen.seed, (z + terrainChunk.transform.position.z) * 0.5f + chunkGen.seed);
        if (doesSpawn > threshold) return true;
        return false;
    }

    void SpawnAsset(int x, float y, int z, float slope, float threshold, float waterLevel, float minSlope, float maxSlope, TerrainGenerator terrainChunk, GameObject[] assetList)
    {
        bool spawns = DoesSpwan(x, z, slope, minSlope, maxSlope, threshold, terrainChunk);
        if (spawns && y > waterLevel + 12)
        {
            int whatSpawns = Mathf.RoundToInt(Random.Range(0f, assetList.Length - 1));
            float offset = Random.Range(0f, 10f);
            offset = offset / 10f;

            GameObject current = Instantiate(assetList[(int)whatSpawns],
                new Vector3(x * (128 / chunkGen.chunkResolution.x) + terrainChunk.transform.position.x + offset, y + terrainChunk.transform.position.y, z * (128 / chunkGen.chunkResolution.y) + terrainChunk.transform.position.z + offset),
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));

            float randomScaleFactor = Random.Range(0.8f, 1.2f);
            current.transform.localScale = new Vector3(randomScaleFactor, randomScaleFactor, randomScaleFactor);

            current.transform.parent = terrainChunk.transform;
        }
    }
}
