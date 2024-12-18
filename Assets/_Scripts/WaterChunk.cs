using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class WaterChunk : MonoBehaviour
{
    public static int water_height = 28;
    
    //0 = air, 1 = water
    public int[,] locs = new int[16, 16];

    void Start()
    {
        transform.localPosition = new Vector3(0, water_height, 0);
    }

    public void SetLocs(BlockType[,,] blocks)
    {
        var width = TerrainChunk.chunk_width;
        int y;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                y = TerrainChunk.chunk_height - 1;
                while (y > 0 && blocks[x, y, z] == BlockType.Air)
                {
                    y--;
                }

                if (y + 1 < water_height)
                {
                    locs[x, z] = 1;
                }
            }
        }
    }
    Vector2[] uvpat = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };

    public void BuildMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                if (locs[x, z] == 1)
                {
                    vertices.Add(new Vector3(x, 0, z));
                    vertices.Add(new Vector3(x, 0, z+1));
                    vertices.Add(new Vector3(x+1, 0, z+1));
                    vertices.Add(new Vector3(x+1, 0, z));

                    vertices.Add(new Vector3(x, 0, z));
                    vertices.Add(new Vector3(x, 0, z + 1));
                    vertices.Add(new Vector3(x + 1, 0, z + 1));
                    vertices.Add(new Vector3(x + 1, 0, z));


                    uvs.AddRange(uvpat);
                    uvs.AddRange(uvpat);
                    int tl = vertices.Count-8;
                    triangles.AddRange(new int[] { tl, tl + 1, tl + 2, tl, tl + 2, tl + 3,
                        tl+3+4,tl+2+4,tl+4,tl+2+4,tl+1+4,tl+4});
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        
        GetComponent<MeshFilter>().mesh = mesh;
    }
}