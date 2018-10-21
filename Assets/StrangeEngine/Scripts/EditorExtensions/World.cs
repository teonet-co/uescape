using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
namespace StrangeEngine
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "World")]
    public class World : ScriptableObject
    {
        public WorldProperties properties;
    }
    [System.Serializable]
    public class WorldProperties
    {
        public bool customMap;
        public int surfaceIndexToSpawnPlayer;
        public genType worldGenerationType;
        public TileBase groundTile;
        public TileBase wallTile;
        public TileBase burnedGroundTile;
        [Range(0, 100)]
        public int wallChance;
        [Range(1, 8)]
        public int filled;
        [Range(1, 8)]
        public int empty;
        [Range(1, 100)]
        public int repeatCount;
        public Vector3Int tileMapSize;
        public float wallsHealth;
        public List<Object> Enemies;
        public int enemyAmount;
        public int tileAmount;
        public int tileSize;
        public int maxPersuitingEnemies;
        public int maxActiveEnemies;

        public float chanceUp;                              //chance generator to move up;
        public float chanceRight;                           //chance generator to move right;
        public float chanceDown;                            //chance generator to move down;
        [Range(0, 500)]
        public int ExtraWallsX;                             // Amount of Extra walls to add by x axes;
        [Range(0, 500)]
        public int ExtraWallsY;                           // Amount of Extra walls to add by y axes;
        public List<advancedSurface> extraSurface;
        public List<objToSpawn> spawner;
    }
}