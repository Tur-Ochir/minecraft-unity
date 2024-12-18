using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts
{
    public class Block
    {
        public Tile top, side, bottom;
        public TilePos topPos, sidePos, bottomPos;

        public Block(Tile tile)
        {
            top = side = bottom = tile;
            
            GetPositions();
        }

        public Block(Tile t, Tile s, Tile b)
        {
            top = t;
            side = s;
            bottom = b;
            
            GetPositions();
        }
        void GetPositions()
        {
            topPos = TilePos.tiles[top];
            sidePos = TilePos.tiles[side];
            bottomPos = TilePos.tiles[bottom];
        }
        public static Dictionary<BlockType, Block> blocks = new Dictionary<BlockType, Block>(){
            {BlockType.Ground, new Block(Tile.Ground)},
            {BlockType.Grass, new Block(Tile.Grass, Tile.GrassSide, Tile.Dirt)},
            {BlockType.Dirt, new Block(Tile.Dirt)},
            {BlockType.Stone, new Block(Tile.Stone)},
            {BlockType.Trunk, new Block(Tile.Trunk, Tile.TrunkSide, Tile.Trunk)},
            {BlockType.Leave, new Block(Tile.Leave)},
        };
    }
    [Serializable]
    public enum BlockType{
        Air,
        Ground,
        Dirt,
        Grass,
        Stone,
        Trunk,
        Leave
    }
}