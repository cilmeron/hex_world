using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkGeneration))]
public class MapGeneratorEditor : Editor
{
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChunkGeneration chunkGen = (ChunkGeneration)target;

        if (GUILayout.Button("Generate Map"))
        {
            GenerateMap(chunkGen);
        }
    }
#endif
    private void GenerateMap(ChunkGeneration chunkGen)
    {

        // Clear existing chunks if needed
        Transform[] childChunks = chunkGen.transform.GetComponentsInChildren<Transform>();
        AssetPlacer AssetPlacer = new AssetPlacer();
        TownPlacer TownPlacer = new TownPlacer();
        UnitPlacer UnitPlacer = new UnitPlacer();

        foreach (Transform child in childChunks)
        {
            if (child != chunkGen.transform)
            {
                if (child != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        // Generate new map
        chunkGen.EditorStart();
        chunkGen.GenerateChunksEditor();

        // Call GenerateTerrain for each chunk
        TerrainGenerator[] terrainGenerators = chunkGen.GetComponentsInChildren<TerrainGenerator>();
        foreach (TerrainGenerator terrainGenerator in terrainGenerators)
        {
            terrainGenerator.GenerateTerrain();
        }


        // Access and build the NavMeshSurface
        NavMeshSurface navMeshSurface = chunkGen.GetComponentInChildren<NavMeshSurface>();


        if (navMeshSurface != null)
        {
            // You can perform additional configuration or trigger navmesh building
            //navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogError("NavMeshSurface component not found on the specified GameObject or its children.");
        }

        if (UnitPlacer.FindSpawnAreas(terrainGenerators))
        {
            //p1
            UnitPlacer.SpawnUnits(0, 10, UnitPlacer.attackerSpawnChunk);
            //p1
            UnitPlacer.SpawnUnits(1, 10, UnitPlacer.defenderSpawnChunk);
        }

        // place assets
        AssetPlacer.AssetPlacement(terrainGenerators);
        TownPlacer.StructurePlacement(terrainGenerators);
    }
}
