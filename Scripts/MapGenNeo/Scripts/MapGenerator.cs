using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChunkGeneration))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChunkGeneration chunkGen = (ChunkGeneration)target;

        if (GUILayout.Button("Generate Map"))
        {
            GenerateMap(chunkGen);
        }
    }

    private void GenerateMap(ChunkGeneration chunkGen)
    {

        // Clear existing chunks if needed
        Transform[] childChunks = chunkGen.transform.GetComponentsInChildren<Transform>();
        AssetPlacer AssetPlacer = new AssetPlacer();

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

        AssetPlacer.AssetPlacement(terrainGenerators);
    }
}
