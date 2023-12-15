using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using static EndlessTerrain;

public class TownPlacer : MonoBehaviour
{
    public GameObject[] mainBuilding;
    public GameObject[] defenseBuilding;
    public GameObject[] cosmeticBuilding;


    public float treeThreshold;
    public float bushThreshold;
    public float rockThreshold;

    private ChunkGeneration chunkGen;

    public void StructurePlacement(TerrainGenerator[] terrainGenerators)
    {
        chunkGen = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkGeneration>();
        mainBuilding = chunkGen.mainBuilding;
        defenseBuilding = chunkGen.defenseBuilding;
        cosmeticBuilding = chunkGen.cosmeticBuilding;

        TerrainGenerator flatTerrainChunk = FindPossibleBuildingSite(terrainGenerators);
        if (flatTerrainChunk == null) return;

        Vector3 centerPosition = new Vector3(
            chunkGen.chunkResolution.x / 2f * (128 / chunkGen.chunkResolution.x) + flatTerrainChunk.transform.position.x,
            flatTerrainChunk.transform.position.y + flatTerrainChunk.mesh.vertices[(int)((chunkGen.chunkResolution.x * chunkGen.chunkResolution.x) / 2f)].y,
            chunkGen.chunkResolution.y / 2f * (128 / chunkGen.chunkResolution.y) + flatTerrainChunk.transform.position.z
        );

        PlaceMainBuilding(flatTerrainChunk, centerPosition);
        PlaceDefenseBuildings(flatTerrainChunk, centerPosition);
        PlaceCosmeticBuildings(flatTerrainChunk, centerPosition);
    }

    public TerrainGenerator FindPossibleBuildingSite(TerrainGenerator[] terrainGenerators)
    {
        TerrainGenerator terrainChunk = new TerrainGenerator();
        float minSlope = float.MaxValue;

        foreach (TerrainGenerator terrainGenerator in terrainGenerators)
        {
            if ((terrainGenerator.averageHeight < minSlope) &&
                (terrainGenerator.averageHeight > (chunkGen.waterLevel * 1.1f)) &&
                (terrainGenerator.averageHeight <= (chunkGen.waterLevel * 2f)))
            {
                minSlope = terrainGenerator.averageHeight;
                terrainChunk = terrainGenerator;
            }
        }

        return terrainChunk;
    }

    private void PlaceMainBuilding(TerrainGenerator terrainChunk, Vector3 centerPosition)
    {
        GameObject selectedMainBuilding = mainBuilding[(int)chunkGen.GenerateRandomInRange(0f, (float)mainBuilding.Length)];
        GameObject mainBuildingInstance = Instantiate(selectedMainBuilding, centerPosition, Quaternion.identity);
        mainBuildingInstance.transform.parent = terrainChunk.transform;
        mainBuildingInstance.tag = "MainBuilding";
    }

    private void PlaceDefenseBuildings(TerrainGenerator terrainChunk, Vector3 centerPosition)
    {
        GameObject selectedDefenseBuilding = defenseBuilding[(int)chunkGen.GenerateRandomInRange(0f, (float)defenseBuilding.Length)];
        float terrainHeight = 0;
        // Place 5 defense buildings in a circular manner around the main building
        for (int i = 0; i < 7; i++)
        {
            float angle = i * (360f / 7); // 72 degrees apart
            float distance = 0.3f * chunkGen.chunkResolution.x;

            Vector3 offset = new Vector3(
                distance * Mathf.Cos(Mathf.Deg2Rad * angle),
                0,
                distance * Mathf.Sin(Mathf.Deg2Rad * angle)
            );

            Vector3 defensePosition = centerPosition + offset;

            terrainHeight = GetTerrainHeight(defensePosition, terrainChunk) - 1;
            defensePosition.y = terrainHeight;

            Vector3 lookAtDirection = centerPosition - defensePosition;
            Quaternion rotation = Quaternion.LookRotation(lookAtDirection, Vector3.up);
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);

            GameObject DefenseBuildingInstance = Instantiate(selectedDefenseBuilding, defensePosition, rotation);
            DefenseBuildingInstance.transform.parent = terrainChunk.transform;
            DefenseBuildingInstance.tag = "DefenseBuilding";
        }
    }

    private void PlaceCosmeticBuildings(TerrainGenerator terrainChunk, Vector3 centerPosition)
    {
        float terrainHeight = 0;
        float ringRadius = 0.3f * chunkGen.chunkResolution.x;
        for (int i = 0; i < 12; i++)
        {
            GameObject selectedCosmeticBuilding = cosmeticBuilding[(int)chunkGen.GenerateRandomInRange(0f, (float)cosmeticBuilding.Length)];
            float angle = i * (360f / 12); // 45 degrees apart
            float distance = chunkGen.GenerateRandomInRange(0.2f, 0.8f) * ringRadius; // Random distance within the ring

            Vector3 offset = new Vector3(
                distance * Mathf.Cos(Mathf.Deg2Rad * angle),
                0,
                distance * Mathf.Sin(Mathf.Deg2Rad * angle)
            );

            Vector3 cosmeticPosition = centerPosition + offset;

            terrainHeight = GetTerrainHeight(cosmeticPosition, terrainChunk);
            cosmeticPosition.y = terrainHeight;

            GameObject cosmeticBuildingInstance = Instantiate(selectedCosmeticBuilding, cosmeticPosition, Quaternion.Euler(0f, chunkGen.GenerateRandomInRange(0f, 360f), 0f));
            cosmeticBuildingInstance.transform.parent = terrainChunk.transform;
            cosmeticBuildingInstance.tag = "CosmeticBuilding";
        }

    }

    private float GetTerrainHeight(Vector3 position, TerrainGenerator terrainChunk)
    {
        position.x = position.x - terrainChunk.transform.position.x;
        position.z = position.z - terrainChunk.transform.position.z;

        Mesh terrainMesh = terrainChunk.mesh;
        Vector3[] vertices = terrainMesh.vertices;

        // Find the closest vertex in the mesh
        int closestVertexIndex = 0;
        float closestDistance = Vector3.Distance(new Vector3(position.x, 0, position.z), new Vector3(vertices[0].x, 0, vertices[0].z));

        for (int i = 1; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(new Vector3(position.x, 0, position.z), new Vector3(vertices[i].x, 0, vertices[i].z));
            if (distance < closestDistance)
            {
                closestVertexIndex = i;
                closestDistance = distance;
            }
        }

        // Get the height (y) of the closest vertex
        float terrainHeight = vertices[closestVertexIndex].y;

        return terrainHeight;
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
            int whatSpawns = Mathf.RoundToInt(chunkGen.GenerateRandomInRange(0f, assetList.Length - 1));
            float offset = chunkGen.GenerateRandomInRange(0f, 10f);
            offset = offset / 10f;

            GameObject current = Instantiate(assetList[(int)whatSpawns],
                new Vector3(x * (128 / chunkGen.chunkResolution.x) + terrainChunk.transform.position.x + offset, y + terrainChunk.transform.position.y, z * (128 / chunkGen.chunkResolution.y) + terrainChunk.transform.position.z + offset),
                Quaternion.Euler(0f, chunkGen.GenerateRandomInRange(0f, 360f), 0f));

            float randomScaleFactor = chunkGen.GenerateRandomInRange(0.8f, 1.2f);
            current.transform.localScale = new Vector3(randomScaleFactor, randomScaleFactor, randomScaleFactor);

            current.transform.parent = terrainChunk.transform;
        }
    }
}
