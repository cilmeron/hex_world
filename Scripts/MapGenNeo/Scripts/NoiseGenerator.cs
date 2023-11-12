using UnityEngine;

public static class NoiseGeneration
{

    public static float GenerateNoise(int x, int z, int seed, float scale, int octaves, float persistance, float lacunarity)
    {
        float noise = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            float sampleX = (x + octaveOffsets[i].x) / scale * frequency;
            float sampleZ = (z + octaveOffsets[i].y) / scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
            noise += perlinValue * amplitude;

            maxValue += amplitude;
            amplitude *= persistance;
            frequency *= lacunarity;
        }

        noise /= maxValue;

        return noise;
    }

}