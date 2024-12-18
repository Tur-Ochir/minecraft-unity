using System;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;


public class TerrainChunk : MonoBehaviour
{
    public const int chunk_width = 16;
    public const int chunk_height = 64;

    public BlockType[,,] blocks = new BlockType[chunk_width + 2, chunk_height, chunk_width + 2];

    void Start()
    {
        // BuildMesh();
    }

    public void BuildMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        

        for (int x = 1; x < chunk_width + 1; x++)
        {
            for (int z = 1; z < chunk_width + 1; z++)
            {
                for (int y = 0; y < chunk_height; y++)
                {
                    if (blocks[x, y, z] != BlockType.Air)
                    {
                        Vector3 blockPos = new Vector3(x - 1, y, z - 1);
                        int numFaces = 0;

                        //on top no block
                        if (blocks[x, y + 1, z] == BlockType.Air && y < chunk_height - 1)
                        {
                            vertices.Add(blockPos + new Vector3(0, 1, 0));
                            vertices.Add(blockPos + new Vector3(0, 1, 1));
                            vertices.Add(blockPos + new Vector3(1, 1, 1));
                            vertices.Add(blockPos + new Vector3(1, 1, 0));

                            uvs.AddRange(Block.blocks[blocks[x, y, z]].topPos.GetUVs());
                            // uvs.Add(new Vector2(0, 0)); // Bottom-left
                            // uvs.Add(new Vector2(1, 0)); // Bottom-right
                            // uvs.Add(new Vector2(0, 1)); // Top-left
                            // uvs.Add(new Vector2(1, 1)); // Top-right

                            numFaces++;
                        }

                        //bottom
                        if (y > 0 && blocks[x, y - 1, z] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(0, 0, 0));
                            vertices.Add(blockPos + new Vector3(1, 0, 0));
                            vertices.Add(blockPos + new Vector3(1, 0, 1));
                            vertices.Add(blockPos + new Vector3(0, 0, 1));

                            uvs.AddRange(Block.blocks[blocks[x, y, z]].bottomPos.GetUVs());
                            

                            numFaces++;
                        }

                        //front
                        if (blocks[x, y, z + 1] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(1, 0, 1));
                            vertices.Add(blockPos + new Vector3(1, 1, 1));
                            vertices.Add(blockPos + new Vector3(0, 1, 1));
                            vertices.Add(blockPos + new Vector3(0, 0, 1));

                            uvs.AddRange(Block.blocks[blocks[x, y, z]].sidePos.GetUVs());

                            numFaces++;
                        }

                        //back
                        if (blocks[x, y, z - 1] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(0, 0, 0));
                            vertices.Add(blockPos + new Vector3(0, 1, 0));
                            vertices.Add(blockPos + new Vector3(1, 1, 0));
                            vertices.Add(blockPos + new Vector3(1, 0, 0));

                            uvs.AddRange(Block.blocks[blocks[x, y, z]].sidePos.GetUVs());

                            numFaces++;
                        }

                        //left
                        if (blocks[x - 1, y, z] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(0, 0, 1));
                            vertices.Add(blockPos + new Vector3(0, 1, 1));
                            vertices.Add(blockPos + new Vector3(0, 1, 0));
                            vertices.Add(blockPos + new Vector3(0, 0, 0));

                            uvs.AddRange(Block.blocks[blocks[x, y, z]].sidePos.GetUVs());

                            numFaces++;
                        }

                        //right
                        if (blocks[x + 1, y, z] == BlockType.Air)
                        {
                            vertices.Add(blockPos + new Vector3(1, 0, 0));
                            vertices.Add(blockPos + new Vector3(1, 1, 0));
                            vertices.Add(blockPos + new Vector3(1, 1, 1));
                            vertices.Add(blockPos + new Vector3(1, 0, 1));

                            uvs.AddRange(Block.blocks[blocks[x, y, z]].sidePos.GetUVs());

                            numFaces++;
                        }

                        int tl = vertices.Count - 4 * numFaces;
                        for (int i = 0; i < numFaces; i++)
                        {
                            triangles.AddRange(new int[]
                            {
                                tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3
                            });
                        }
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}