using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    [FormerlySerializedAs("Chunk")] public TerrainChunk chunkPrefab;
    FastNoise fastNoise = new FastNoise();
    public Transform player;
    public int chunkDistance = 5;
    [SerializeField] private List<Vector2> pooledChunkKeys = new List<Vector2>();

    public static Dictionary<Vector2, TerrainChunk> Chunks = new Dictionary<Vector2, TerrainChunk>();

    private List<TerrainChunk> pooledChunks = new List<TerrainChunk>();
    private List<Vector2> toGenerate = new List<Vector2>();


    void Start()
    {
        // fastNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        // fastNoise.SetFrequency(100);
        LoadChunk(true);
    }

    void Update()
    {
        LoadChunk();
    }

    private void BuildChunk(int startingX, int startingZ)
    {
        TerrainChunk chunk;
        if (pooledChunks.Count > 0)
        {
            chunk = pooledChunks[0];
            chunk.gameObject.SetActive(true);
            chunk.transform.position = new Vector3(startingX, 0, startingZ);

            pooledChunks.RemoveAt(0);
        }
        else
        {
            chunk = Instantiate(chunkPrefab, new Vector3(startingX, 0, startingZ), quaternion.identity);
        }

        for (int x = 0; x < TerrainChunk.chunk_width + 2; x++)
        {
            for (int z = 0; z < TerrainChunk.chunk_width + 2; z++)
            {
                for (int y = 0; y < TerrainChunk.chunk_height; y++)
                {
                    chunk.blocks[x, y, z] = GetBlock(startingX + x - 1, y, startingZ + z - 1);
                }
            }
        }

        BuildTrees(startingX, startingZ, chunk.blocks);
        chunk.BuildMesh();

        //water
        WaterChunk water = chunk.GetComponentInChildren<WaterChunk>();
        water.SetLocs(chunk.blocks);
        water.BuildMesh();

        if (!Chunks.ContainsKey(new Vector2(startingX, startingZ)))
        {
            Chunks.Add(new Vector2(startingX, startingZ), chunk);
        }
        // Debug.Log($"NEW CHUNK AT X: {startingX}, Z: {startingZ}");
    }

    private BlockType GetBlock(int x, int y, int z)
    {
        BlockType blockType = BlockType.Air;

        var simplex = fastNoise.GetSimplex(x*.8f, z*.8f)*10;
        var simplex2 = fastNoise.GetSimplex(x * 2f, y * 0.6f, z * 2f) * 10 *
                      (fastNoise.GetSimplex(x * .2f, y * .2f, z * .2f) + .5f);
        
        float heightMap = simplex + simplex2;

        var baseLandHeight = TerrainChunk.chunk_height * .5f + heightMap;

        //stone height
        var stoneSimplex = fastNoise.GetSimplex(x * 2f, y * 0.2f, z * 2f) * 10 *
                           (fastNoise.GetSimplex(x * .1f, y, z * .2f) + .5f);
        float stoneHeightMap = simplex + stoneSimplex;
        float stoneHeight = TerrainChunk.chunk_height * .3f + stoneHeightMap;
        if (y <= baseLandHeight)
        {
            blockType = BlockType.Dirt;

            if (y > baseLandHeight - 1 && y > WaterChunk.water_height - 2)
            {
                blockType = BlockType.Grass;
            }

            if (y <= stoneHeight)
            {
                blockType = BlockType.Stone;
            }
        }

        return blockType;
    }

    private void BuildTrees(int x, int z, BlockType[,,] blocks)
    {
        System.Random rand = new System.Random(x * 10000 + z);
        var simplex = fastNoise.GetSimplex(x * .8f, z * .8f);
        if (simplex <= 0) return;

        simplex *= 2f;
        int treeCount = Mathf.FloorToInt((float)rand.NextDouble() * simplex * 5);

        for (int i = 0; i < treeCount; i++)
        {
            //get pos
            int xPos = (int)(rand.NextDouble() * 14) + 1;
            int zPos = (int)(rand.NextDouble() * 14) + 1;

            int y = TerrainChunk.chunk_height - 1;
            //find the ground
            while (y > 0 && blocks[xPos, y, zPos] == BlockType.Air)
            {
                y--;
            }

            y++;

            if (y <= WaterChunk.water_height)
            {
                return;
            }

            var height = 3 + (int)rand.NextDouble() * 4;
            for (int j = 0; j < height; j++)
            {
                blocks[xPos, y + j, zPos] = BlockType.Trunk;
            }

            int leavesWidth = 1 + (int)(rand.NextDouble() * 6);
            int leavesHeight = 1 + (int)(rand.NextDouble() * 3);
            int iter = 0;
            for (int j = y + height - 1; j <= y + height + leavesHeight; j++)
            {
                for (int k = xPos - (int)(leavesWidth * .5f) + iter / 2;
                     k <= xPos + (int)(leavesWidth * .5f) - iter / 2;
                     k++)
                {
                    for (int l = zPos - (int)(leavesWidth * .5f) + iter / 2;
                         l <= zPos + (int)(leavesWidth * .5f) - iter / 2;
                         l++)
                    {
                        if (k >= 0 && k <= 16 && l >= 0 && l <= 16)
                        {
                            bool isEdge = (k == 0 || k == 16) || (l == 0 || l == 16);
                            if (!isEdge || rand.NextDouble() <= 0.69f)
                            {
                                blocks[k, j, l] = BlockType.Leave;
                            }
                        }
                        // Debug.Log($"Leave {k} {j} {l}");
                    }
                }

                iter++;
            }

            // Debug.Log($"Leave Width {leavesWidth}");
            // Debug.Log($"Leave Height {leavesHeight}");
        }
    }

    int currentChunkX = -1;
    int currentChunkZ = -1;

    private void LoadChunk(bool init = false)
    {
        var width = TerrainChunk.chunk_width;
        var currentChunkPosX = Mathf.FloorToInt(player.position.x / 16) * 16;
        var currentChunkPosZ = Mathf.FloorToInt(player.position.z / 16) * 16;
        // entered new chunk
        if (currentChunkX != currentChunkPosX || currentChunkZ != currentChunkPosZ)
        {
            //current chunk pos
            currentChunkX = currentChunkPosX;
            currentChunkZ = currentChunkPosZ;
            for (int x = currentChunkPosX - width * chunkDistance;
                 x <= currentChunkPosX + width * chunkDistance;
                 x += width)
            {
                for (int z = currentChunkPosZ - width * chunkDistance;
                     z <= currentChunkPosZ + width * chunkDistance;
                     z += width)
                {
                    if (init)
                    {
                        BuildChunk(x, z);
                    }
                    else
                    {
                        if (!Chunks.ContainsKey(new Vector2(x, z)))
                        {
                            toGenerate.Add(new Vector2(x, z));
                        }
                    }
                }
            }
        }

        // if too far away delete
        List<Vector2> toDestroy = new List<Vector2>();
        foreach (KeyValuePair<Vector2, TerrainChunk> c in Chunks)
        {
            Vector2 cp = c.Key;
            if (Mathf.Abs(currentChunkPosX - cp.x) > 16 * (chunkDistance + 2) ||
                Mathf.Abs(currentChunkPosZ - cp.y) > 16 * (chunkDistance + 2))
            {
                toDestroy.Add(c.Key);
            }
        }

        for (int i = 0; i < toGenerate.Count; i++)
        {
            if (Mathf.Abs(currentChunkPosX - toGenerate[i].x) > 16 * (chunkDistance + 1) ||
                Mathf.Abs(currentChunkPosZ - toGenerate[i].y) > 16 * (chunkDistance + 2))
            {
                toGenerate.Remove(toGenerate[i]);
            }
        }

        foreach (Vector2 key in toDestroy)
        {
            Chunks[key].gameObject.SetActive(false);
            pooledChunks.Add(Chunks[key]);
            Chunks.Remove(key);
        }

        StartCoroutine(DelayedBuildChunk());
    }

    private IEnumerator DelayedBuildChunk()
    {
        while (toGenerate.Count > 0)
        {
            BuildChunk((int)toGenerate[0].x, (int)toGenerate[0].y);
            toGenerate.RemoveAt(0);

            yield return new WaitForSeconds(.2f);
        }
    }
}