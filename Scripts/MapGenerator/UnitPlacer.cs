using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EndlessTerrain;

public class UnitPlacer : MonoBehaviour
{
    public Vector3 attackerSpawnArea = Vector3.zero;
    public Vector3 defenderSpawnArea = Vector3.zero;

    public TerrainGenerator attackerSpawnChunk;
    public TerrainGenerator defenderSpawnChunk;

    private float spawnRadius = 0.5f;
    private float minimalDistance = 150f;
    private float maximalDistance = 200f;

    private ChunkGeneration chunkGen;
    private TownPlacer TownPlacer = new TownPlacer();

    public bool FindSpawnAreas(TerrainGenerator[] terrainGenerators)
    {
        TerrainGenerator defenderChunk = TownPlacer.FindPossibleBuildingSite(terrainGenerators);
        defenderSpawnChunk = defenderChunk;
        if (defenderChunk == null) return false;

        chunkGen = GameObject.Find("MapGenerator").GetComponent<ChunkGeneration>();
        defenderSpawnArea = GetCenterPosition(defenderChunk);
        defenderSpawnArea.y = GetTerrainHeight(defenderSpawnArea, defenderChunk);

        List<TerrainGenerator> possibleAreas = new List<TerrainGenerator>();
        foreach (TerrainGenerator chunk in terrainGenerators)
        {
            if (defenderChunk != this) // Skip comparing with itself
            {
                float distance = Vector2.Distance(chunk.worldPosition, defenderChunk.worldPosition);
                if (distance >= minimalDistance && distance <= maximalDistance)
                {
                    if ((chunk.averageHeight > (chunkGen.waterLevel * 1.1f)) &&
                        (chunk.averageHeight <= (chunkGen.waterLevel * 2f)))
                    {
                        possibleAreas.Add(chunk);
                        attackerSpawnArea = GetCenterPosition(chunk);
                        attackerSpawnArea.y = GetTerrainHeight(attackerSpawnArea, chunk);
                        attackerSpawnChunk = chunk;
                    }
                }
                Debug.Log($"Distance to {defenderChunk.name}: {distance}");
            }
        }
        return true;
    }

    public List<Vector3> GetSpawnVectorList(int numOfSpawns, TerrainGenerator chunk, Vector3 baseVector)
    {
        List<Vector3> spawnVectorList = new List<Vector3>();

        for (int i = 0; i < numOfSpawns; i++)
        {
            float angle = i * (360f / numOfSpawns); // Distribute points evenly in a circle
            float spawnX = baseVector.x + spawnRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float spawnZ = baseVector.z + spawnRadius * Mathf.Sin(Mathf.Deg2Rad * angle);

            // Get the y-coordinate based on the terrain
            float spawnY = GetTerrainHeight(new Vector3(spawnX, 0f, spawnZ), chunk);

            Vector3 spawnPoint = new Vector3(spawnX, spawnY, spawnZ);
            spawnVectorList.Add(spawnPoint);
        }
        return spawnVectorList;
    }

    public void SpawnUnits(int playerID, int numOfUnits, TerrainGenerator chunk)
    {
        Vector3 baseVector = GetCenterPosition(chunk);
        baseVector.y = GetTerrainHeight(defenderSpawnArea, chunk);
        List<Vector3> spawnVectors = GetSpawnVectorList(numOfUnits, chunk, baseVector);

        GameObject placeable = new GameObject();
        if (playerID == 0) placeable = chunkGen.p1Prefab;
        if (playerID == 1) placeable = chunkGen.p2Prefab;

        if (placeable == null) return;

        foreach (Vector3 vector in spawnVectors)
        {
            GameObject placedUnit = Instantiate(placeable, vector, Quaternion.identity);
            // placedUnit.GetComponent<Unit>().SetPlayer();
        }
    }

    private void SpawnAttacker()
    {
        // ignore this
    }

    private void SpawnDefender()
    {
        // ignore this
    }

    public Vector3 GetCenterPosition(TerrainGenerator chunk)
    {
        if (chunkGen == null)
        {
            chunkGen = GameObject.Find("MapGenerator").GetComponent<ChunkGeneration>();
        }
        Vector3 centerPosition = new Vector3(
            chunkGen.chunkResolution.x / 2f * (128 / chunkGen.chunkResolution.x) + chunk.transform.position.x,
            chunk.transform.position.y + chunk.mesh.vertices[(int)((chunkGen.chunkResolution.x * chunkGen.chunkResolution.x) / 2f)].y,
            chunkGen.chunkResolution.y / 2f * (128 / chunkGen.chunkResolution.y) + chunk.transform.position.z
        );
        return centerPosition;
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

    public TerrainGenerator FindAttackerSpawnArea(TerrainGenerator[] terrainGenerators, TerrainGenerator defenderChunk)
    {
        TerrainGenerator spawnArea = new TerrainGenerator();
        return spawnArea;
    }

}
