using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    [SerializeField] private float modDist = 5;
    private Transform cam;

    Vector2Int[] directions =
    {
        new Vector2Int(-16, 0), // Left chunk (blockX == 1)
        new Vector2Int(16, 0), // Right chunk (blockX == 16)
        new Vector2Int(0, -16), // Backward chunk (blockZ == 1)
        new Vector2Int(0, 16) // Forward chunk (blockZ == 16)
    };

    void Start()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if (leftClick || rightClick)
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, modDist))
            {
                // Debug.Log($"HIT BLOCK: {hit.point}");
                var c = hit.transform.GetComponent<TerrainChunk>();
                int chunkX = (int)c.transform.position.x;
                int chunkZ = (int)c.transform.position.z;

                Vector3 hitPoint;

                if (leftClick)
                {
                    hitPoint = hit.point + transform.forward * 0.01f;
                }
                else
                {
                    hitPoint = hit.point - transform.forward * 0.01f;
                }


                //foot block y
                if (hitPoint.y <= transform.position.y && leftClick)
                {
                    hitPoint.y -= 0.01f;
                }

                if (hitPoint.y >= transform.position.y && rightClick)
                {
                    hitPoint.y -= 0.01f;
                }

                int blockX = Mathf.FloorToInt(hitPoint.x) - chunkX + 1;
                int blockY = Mathf.FloorToInt(hitPoint.y);
                int blockZ = Mathf.FloorToInt(hitPoint.z) - chunkZ + 1;

                if (leftClick) //destroy block
                {
                    c.blocks[blockX, blockY, blockZ] = BlockType.Air;
                    c.BuildMesh();

                    if (blockX == 1 || blockX == 16 || blockZ == 1 || blockZ == 16)
                    {
                        // Calculate which neighbor chunk to access
                        Vector2 neighborPos = new Vector2(
                            chunkX + (blockX == 1 ? directions[0].x : blockX == 16 ? directions[1].x : 0),
                            chunkZ + (blockZ == 1 ? directions[2].y : blockZ == 16 ? directions[3].y : 0)
                        );

                        TerrainChunk neighborChunk = TerrainGenerator.Chunks[neighborPos];

                        int neighborBlockX = (blockX == 1) ? 17 : (blockX == 16) ? 0 : blockX;
                        int neighborBlockZ = (blockZ == 1) ? 17 : (blockZ == 16) ? 0 : blockZ;

                        neighborChunk.blocks[neighborBlockX, blockY, neighborBlockZ] = BlockType.Air;
                        neighborChunk.BuildMesh();
                    }
                }
                else //place block
                {
                    c.blocks[blockX, blockY, blockZ] = BlockType.Dirt;
                    c.BuildMesh();
                }

                Debug.Log($"BLOCK POS: {blockX} {blockY} {blockZ}");
                Debug.Log($"HIT POINT: {hitPoint}");
            }

            Debug.DrawRay(cam.position, cam.forward * 5, Color.red, 1f);
        }
    }
}