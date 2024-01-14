using System.Collections.Generic;
using UnityEngine;

public class HeightMapAnalyzer : MonoBehaviour
{
    private ChunkGeneration chunkGen;
    // Example function to calculate average absolute differences
    public List<float> CalculateAverageDifferences(Vector3[] vertices)
    {
        chunkGen = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkGeneration>();
        int resolutionX = (int)chunkGen.chunkResolution.x;
        int resolutionY = (int)chunkGen.chunkResolution.y;
        //Vector3[] vertices = new Vector3[resolutionX * resolutionX];

        List<float> averageDifferences = new List<float>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == 63)
            {
                int x = 0;
            }

            if (i == 127)
            {
                int x = 0;
            }


            // Get the vertex position
            float heightValue = vertices[i].y;
            float deltaSum = 0f;
            int legitNeighbours = 0;

            int north = i - resolutionX; // lgeit if not < 0
            int east = i + 1; // legit if not > resolutionX * resolutionY && (i % resolution) != 0
            int south = i + resolutionX; // legit if not > resolutionX * resolutionY
            int west = i - 1; // legit if not < 0 && ((i - 1) % resolution) != 0

            if (north >= 0)
            {
                deltaSum += Mathf.Abs(heightValue - vertices[north].y);
                legitNeighbours++;
            }
            if ((i == 0) || ((east <= (resolutionX * resolutionY)) && ((i + 1) % resolutionX != 0)))
            {
                deltaSum += Mathf.Abs(heightValue - vertices[east].y);
                legitNeighbours++;
            }
            if (south <= (resolutionX * resolutionY))
            {
                deltaSum += Mathf.Abs(heightValue - vertices[south].y);
                legitNeighbours++;
            }
            if ((west >= 0) && (i % resolutionX != 0))
            {
                deltaSum += Mathf.Abs(heightValue - vertices[west].y);
                legitNeighbours++;
            }

            averageDifferences.Add(deltaSum / legitNeighbours);
        }

        return averageDifferences;
    }

    // Function to get von Neumann neighbors for a given vertex
    List<Vector3> GetVonNeumannNeighbors(Vector3[] vertices, Vector3 vertexPos, int currentIndex)
    {
        chunkGen = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkGeneration>();
        int resolutionX = (int)chunkGen.chunkResolution.x;
        int resolutionY = (int)chunkGen.chunkResolution.y;
        List<Vector3> neighbors = new List<Vector3>();

        // Define von Neumann neighbors offsets
        int[,] offsets = {
            { 0, 1 },  // North
            { 1, 0 },  // East
            { 0, -1 }, // South
            { -1, 0 }  // West
        };

        int north = currentIndex - resolutionX; // lgeit if not < 0
        int east = currentIndex + 1; // legit if not > resolutionX * resolutionY && (i % resolution) != 0
        int south = currentIndex + resolutionX; // legit if not > resolutionX * resolutionY
        int west = currentIndex - 1; // legit if not < 0 && ((i - 1) % resolution) != 0

        for (int j = 0; j < 4; j++)
        {
            int neighborX = Mathf.RoundToInt(vertexPos.x) + offsets[j, 0];
            int neighborZ = Mathf.RoundToInt(vertexPos.z) + offsets[j, 1];

            // Check if the neighbor is within bounds
            if (neighborX >= 0 && neighborX < resolutionX && neighborZ >= 0 && neighborZ < resolutionX)
            {
                int neighborIndex = neighborX + neighborZ * resolutionX;

                // Ignore non-existing neighbors on the edges
                if (neighborIndex != currentIndex)
                {
                    neighbors.Add(vertices[neighborIndex]);
                }
            }
        }

        return neighbors;
    }
}
