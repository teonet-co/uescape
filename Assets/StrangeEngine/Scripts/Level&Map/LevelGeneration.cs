using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
/// <summary>
/// Randomly Generates Level with tiles.
/// </summary>
namespace StrangeEngine
{
    [System.Serializable]
    public struct surfaceMainData
    {
        public List<Vector3Int> _positions;
        public Tilemap _tileMap;
        public TileBase _tile;
    }
    public enum surface {walls, ground}
    [System.Serializable]
    public struct oldGenProperties
    {
        [Tooltip("Amount of floor tiles.")]
        [Range(0, 1000)]
        public int TileAmount;
        [Header("Direction of Generator")]
        [Range(0, 1)]
        public float chanceUp;                              //chance generator to move up;
        [Range(0, 1)]
        public float chanceRight;                           //chance generator to move right;
        [Range(0, 1)]
        public float chanceDown;                            //chance generator to move down;
        [Tooltip("Extra walls to add by x axes. IMPORTANT: value must be divided by 2.")]
        [Range(0, 500)]
        public int ExtraWallsX;                             // Amount of Extra walls to add by x axes;
        [Tooltip("Extra walls to add by y axes. IMPORTANT: value must be divided by 2.")]
        [Range(0, 500)]
        public int ExtraWallsY;                           // Amount of Extra walls to add by y axes;
    }
    [System.Serializable]
    public struct newGenProperties
    {
        [Tooltip("Chance to wall appear on map.")]
        [Range(0, 100)]
        public int wallChance;
        [Tooltip("Value represents, how many filled tiles can be around tile.")]
        [Range(1, 8)]
        public int filled;
        [Tooltip("Value represents, how many empty tiles can be around tile.")]
        [Range(1, 8)]
        public int empty;
        [Tooltip("Value represents, how many times repeat walls generating(nore times = more solid walls).")]
        [Range(1, 100)]
        public int repeatCount;
        [Tooltip("Wall size on axes.")]
        public Vector3Int tileMapSize;
    }
    [System.Serializable]
    public class LevelGeneration : MonoBehaviour
    {
#region variables
        [Tooltip("If null, map will generate from local values.")]
        //public World curentMap;
        // [HideInInspector]
        public WorldProperties MapProperties;
        public bool canGenerateInEditMode = false;
        [Space]
        [HideInInspector]
        [Header("Main Settings.")]
        public float wallsHealth;
        [HideInInspector]
        public Dictionary<Vector3Int, float> healthValues;
        [Tooltip("Size of one tile")]
        [Range(0, 100)]
        [HideInInspector]
        public int TileSize;
        public Grid grid;
        public Tilemap wallsMap;
        public Tilemap groundMap;
        [HideInInspector]
        public TileBase walTile;
        [HideInInspector]
        public TileBase groundTile;
        [HideInInspector]
        public TileBase burnedGroundTile;
        [HideInInspector]
        [Space]
        [Header("Generation Settings.")]
        public genType generationType;
        [HideInInspector]
        [Tooltip("Properties for old generation method.")]
        public oldGenProperties _oldGenProperties;
        [Space]
        [HideInInspector]
        [Tooltip("Properties for new generation method.")]
        public newGenProperties _newGenProperties;
        [Space]
        public UnityEvent onWallDestroy;
        [Space]
        [HideInInspector]
        public Transform parent;                            // all created tiles will be parented to this transform;
        [HideInInspector]
        [Tooltip("Array of Enemy Prefabs.")]
        public List<Object> Enemies;                        // Reference to Enemies;
        [HideInInspector]
        [Tooltip("Amount of Enemy Prefabs you want to Instantiate in Scene.")]
        [Range(0, 500)]
        public int enemyAmount = 10;                        // Number of Enemies;
        [HideInInspector]
        [SerializeField]
        private List<Vector3Int> CreatedTiles;                  // Temporary list of created tiles;
        [HideInInspector]
        [SerializeField]
        private List<Vector3Int> wallTiles;
        private TileBase[] wTiles;
        private TileBase[] gTiles;
        private int mapWidth;
        private int mapHeight;
        private int[,] map;
        [HideInInspector]
        public float minimumY;
        [HideInInspector]
        public float maximumY;
        [HideInInspector]
        public float minimumX;
        [HideInInspector]
        public float maximumX;
        private int minY = 999999;                         // no need to edit this line, it's just min or max value;
        private int maxY = 0;                              // no need to edit this line, it's just min or max value;
        private int minX = 999999;                         // no need to edit this line, it's just min or max value;
        private int maxX = 0;                              // no need to edit this line, it's just min or max value;
        private int xAmount = 0;                              // amount of wall on th x axes;
        private int yAmount = 0;                              // amount of wall on th y axes;
        [Tooltip("Camera GameObject.")]
        private GameObject Camera;                           // Reference to Camera;
        [Space(10)]
        [Tooltip("Palyer GameObject.")]
        private GameObject Player;                           // Reference to Player;
                                                             //

        [HideInInspector]
        public bool enableObjectsSpawner;
        private List<objToSpawn> ObjectsSpawner;
        private GameObject spawnedObjects; // Temp Object to parent all spawned objects;
        private Vector3Int heighestPos;
        private Vector3Int lowestPos;
        private List<advancedSurface> advanceSurfacesSettings;
        [HideInInspector]
        public List<Vector3Int> spawnedTilesPositions;
        // [HideInInspector]
        //public List<List<Vector3Int>> _surfacesIntVectors;
        public List<surfaceMainData> surfacesData;
        //public int _surfacesIntVectorsIndex;
        //
        #endregion
       private void Start()
        {
            if(grid == null)
            {
                print("Level Generation : Please insert Grid!");
            }else if(wallsMap == null)
            {
                print("Level Generation : Please insert wall map!");
            }else if(groundMap == null)
            {
                print("Level Generation : Please insert ground map!");
            }
            else
            {
                LoadParameters();
                if (!canGenerateInEditMode|| !MapProperties.customMap)
                    StartGenerating();
                spawnedObjects = new GameObject("Spawned Objects");
                Camera = GameManager._gameManager.mainCamera;
                Player = GameManager._gameManager.player;
                SpawnObjects();
                if (enableObjectsSpawner)
                {
                    spawnedTilesPositions = new List<Vector3Int>();
                    for (int i = 0; i < ObjectsSpawner.Count; i++)
                    {
                        StartCoroutine(objSpawnerRepeat(ObjectsSpawner[i])); // Start Spawn Objects;
                    }
                }
                if (canGenerateInEditMode)
                    FillWallHealthValues();
            }
        }
        public void StartGenerating()
        {
            wallsMap.ClearAllTiles();
            groundMap.ClearAllTiles();
            CreatedTiles = new List<Vector3Int>();
            wallTiles = new List<Vector3Int>();
            gTiles = new TileBase[0];
            wTiles = new TileBase[0];
            minY = 999999;
            maxY = 0;
            minX = 999999;
            maxX = 0;
            transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
            parent = new GameObject("LevelParent").transform;           // creating parent;
            heighestPos = new Vector3Int();
            lowestPos = new Vector3Int();
            //Set all references to currentMap.
            //if (curentMap)
            //{
                LoadParameters();
            //}
            //
            switch (generationType)
            {
                case genType._old:
                    GenerateLevel();              // Start Generating level;
                    break;
                case genType._new:
                    map = null;
                    StartSimulation(_newGenProperties.repeatCount);
                    break;
                default:
                    break;
            }
            FillIntList();
            StartExtraSurfaceSimulation();

            FillWallHealthValues();
        }

        /// <summary>
        /// Starts generating level.
        /// </summary>
        /// <returns></returns>
        void GenerateLevel()
        {
            for (int i = 0; i < _oldGenProperties.TileAmount; i++)            //create tiles loop;
            {
                float dir = Random.Range(0f, 1f);           // choosing random number beetween 0 and 1 to choose direction;
                CreateTile();                           // start creating tile;
                CallMoveGen(dir);                           // randomly choose direction;
                if (i == _oldGenProperties.TileAmount - 1)
                {
                    Finish();                               // if last tile was created, start finish function;
                }
            }
        }
        /// <summary>
        /// Function to randomly choose Direction of Generator movement
        /// </summary>
        /// <param name="ranDir"></param>
        void CallMoveGen(float ranDir)
        {
            if (ranDir < _oldGenProperties.chanceUp)
            {
                MoveGen(0);
            }
            else if (ranDir < _oldGenProperties.chanceRight)
            {
                MoveGen(1);
            }
            else if (ranDir < _oldGenProperties.chanceDown)
            {
                MoveGen(2);
            }
            else
            {
                MoveGen(3);
            }
        }
        /// <summary>
        /// Function to move generator in choosed direction
        /// </summary>
        /// <param name="direction of level 'generetion' GameObject"></param>
        void MoveGen(int dir)
        {
            switch (dir)
            {
                case 0:
                    //if generator direction is up, we must add tileSize value to y axes;
                    transform.position = new Vector3(transform.position.x, transform.position.y + TileSize, 0);
                    break;
                case 1:
                    //if generator direction is right, we must add tileSize value to x axes;
                    transform.position = new Vector3(transform.position.x + TileSize, transform.position.y, 0);
                    break;
                case 2:
                    //if generator direction is down, we must take away tileSize value from y axes;
                    transform.position = new Vector3(transform.position.x, transform.position.y - TileSize, 0);
                    break;
                case 3:
                    //if generator direction is left, we must take away tileSize value from x axes;
                    transform.position = new Vector3(transform.position.x - TileSize, transform.position.y, 0);
                    break;
            }
        }
        /// <summary>
        /// Function to create floor
        /// </summary>
        void CreateTile()
        {
            if (!CreatedTiles.Contains(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z))) //if new tile position not equals to created tiles position;
            {
                CreatedTiles.Add(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z)); // add floor gameobject to created tiles;
            }
            else
            { //if new tile position equals to created tiles position;
                _oldGenProperties.TileAmount++; // go to the next tile;
            }
        }
        /// <summary>
        /// Finishing function
        /// </summary>
        void Finish()
        {
            CreateWallValues();
            CreateWalls();
            FillTileMaps();
            FindMinMaxValues();
        }
        void FillTileMaps()
        {
            wTiles = new TileBase[wallTiles.Count];
            for (int i = 0; i < wTiles.Length; i++)
            {
                wTiles[i] = walTile;
            }
            gTiles = new TileBase[CreatedTiles.Count];
            for (int i = 0; i < gTiles.Length; i++)
            {
                gTiles[i] = groundTile;
            }
            wallsMap.SetTiles(wallTiles.ToArray(), wTiles);
            groundMap.SetTiles(CreatedTiles.ToArray(), gTiles);
        }
        /// <summary>
        /// Function to spawn Player, Camera, Enemies
        /// </summary>
        void SpawnObjects()
        {
            Player.SetActive(true);
            Player.transform.position = FindRandomPointOnGround(surfacesData[MapProperties.surfaceIndexToSpawnPlayer]._positions);
            Camera.SetActive(true);
        }
        private Vector3 FindRandomPointOnGround(List<Vector3Int> posMap)
        {
            Vector3Int tempPoint = new Vector3Int(0,0,0);
            if (posMap.Count > 0)
            {
                tempPoint = posMap[Random.Range(0, posMap.Count - 1)];
            }
            else
            {
                tempPoint = Vector3Int.zero;
            }
            Vector3 realPoint = grid.CellToWorld(tempPoint) + grid.cellSize / 2;
            return realPoint;

        }
        /// <summary>
        /// function that go throw all created tiles and then find lowest and highest x and y positions
        /// </summary>
        void CreateWallValues()
        {
            for (int i = 0; i < CreatedTiles.Count; i++)
            {
                if (CreatedTiles[i].y < minY)
                {
                    minY = CreatedTiles[i].y;           // the lowest y position
                    lowestPos.y = CreatedTiles[i].y;
                    //minimumY = minY;
                }
                if (CreatedTiles[i].y > maxY)
                {
                    maxY = CreatedTiles[i].y;           // the highest y position
                    heighestPos.y = CreatedTiles[i].y;
                    //maximumY = maxY;
                }
                if (CreatedTiles[i].x < minX)
                {
                    minX = CreatedTiles[i].x;           // the lowest x position
                    lowestPos.x = CreatedTiles[i].x;
                    //minimumX = minX;
                }
                if (CreatedTiles[i].x > maxX)
                {
                    maxX = CreatedTiles[i].x;           // the highest x position
                    heighestPos.x = CreatedTiles[i].x;
                    //maximumX = maxX;
                }
                xAmount = ((maxX - minX) / TileSize) + _oldGenProperties.ExtraWallsX; // adding extra walls on x axes
                yAmount = ((maxY - minY) / TileSize) + _oldGenProperties.ExtraWallsY; // adding extra walls on y axes
            }
        }
        public void FindCustomMapMinMaxValues(){
            for (int i = 0; i < surfacesData[1]._positions.Count; i++)
            {
                if (surfacesData[1]._positions[i].y < minY)
                {
                    minY = surfacesData[1]._positions[i].y;           // the lowest y position
                    lowestPos.y = surfacesData[1]._positions[i].y;
                    //minimumY = minY;
                }
                if (surfacesData[1]._positions[i].y > maxY)
                {
                    maxY = surfacesData[1]._positions[i].y;           // the highest y position
                    heighestPos.y = surfacesData[1]._positions[i].y;
                    //maximumY = maxY;
                }
                if (surfacesData[1]._positions[i].x < minX)
                {
                    minX = surfacesData[1]._positions[i].x;           // the lowest x position
                    lowestPos.x = surfacesData[1]._positions[i].x;
                    //minimumX = minX;
                }
                if (surfacesData[1]._positions[i].x > maxX)
                {
                    maxX = surfacesData[1]._positions[i].x;           // the highest x position
                    heighestPos.x = surfacesData[1]._positions[i].x;
                    //maximumX = maxX;
                }
            }

            minimumX = groundMap.CellToWorld(lowestPos).x;
            maximumX = groundMap.CellToWorld(heighestPos).x;
            minimumY = groundMap.CellToWorld(lowestPos).y;
            maximumY = groundMap.CellToWorld(heighestPos).y;
        }
        void FindMinMaxValues()
        {
            minimumX = groundMap.CellToWorld(lowestPos).x;
            maximumX = groundMap.CellToWorld(heighestPos).x;
            minimumY = groundMap.CellToWorld(lowestPos).y;
            maximumY = groundMap.CellToWorld(heighestPos).y;
        }
        /// <summary>
        /// Function to create walls
        /// </summary>
        void CreateWalls()
        {
            for (int x = 0; x < xAmount; x++)
            {
                for (int y = 0; y < yAmount; y++)
                {
                    if (!CreatedTiles.Contains(new Vector3Int((minX - (_oldGenProperties.ExtraWallsX * TileSize) / 2) + (x * TileSize), (minY - (_oldGenProperties.ExtraWallsY * TileSize) / 2) + (y * TileSize), 0)))// make shure that there are no floor where we create a wall
                    {
                        wallTiles.Add(new Vector3Int((minX - (_oldGenProperties.ExtraWallsX * TileSize) / 2) + (x * TileSize), (minY - (_oldGenProperties.ExtraWallsY * TileSize) / 2) + (y * TileSize), 0));
                    }
                }
            }
            parent.transform.position = new Vector3(0, 0, 10); // changes parent position on y = 10. Not necessary at all, you can delete it if you want
        }
        /// <summary>
        /// Starts Spawn with options;
        /// </summary>
        /// <param name="o">Object To Spawn Struct</param>
        /// <returns></returns>
        private IEnumerator objSpawnerRepeat(objToSpawn o)
        {
            if (o.enableSpawner)
            {
                switch (o.RepeateOptions) // switch between repeat options;
                {
                    case repeatOptions.noRepeate:
                        StartCoroutine(objSpawner(o.enableRandomSpawn, o.randomTimeToSpawn, o.timeToSpawn, o.amountOfObjectsToSpawn, o.spawnerRadius, o.layerToAvoid, o.objectToSpawn, o.forceMode, o.force, o.addForce, o)); // simply starts main spawn without repeat;
                        yield return 0;
                        break;
                    case repeatOptions.repeatAfterMainSpawnTime:
                        StartCoroutine(objSpawner(o.enableRandomSpawn, o.randomTimeToSpawn, o.timeToSpawn, o.amountOfObjectsToSpawn, o.spawnerRadius, o.layerToAvoid, o.objectToSpawn, o.forceMode, o.force, o.addForce, o));// starts main spawn
                        if (o.randomRepeat)
                        {
                            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(o.randomRepeatTimer.x, o.randomRepeatTimer.y));
                        }
                        else
                        {
                            yield return new WaitForSecondsRealtime(o.repeatTimer);
                        }
                        for (int i = 0; i < o.timesToRepeat; i++)
                        {
                            if (o.randomRepeat)
                            {
                                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(o.randomRepeatTimer.x, o.randomRepeatTimer.y)); // waiting for random delay;
                            }
                            else
                            {
                                yield return new WaitForSecondsRealtime(o.repeatTimer); // waiting for delay;
                            }
                            StartCoroutine(objSpawner(o.randomRepeat, Vector2.zero, 0, o.amountOfObjectsToRespawn, o.spawnerRadius, o.layerToAvoid, o.objectToSpawn, o.forceMode, o.force, o.addForce, o));// starts main repeated spawn
                        }
                        break;

                    case repeatOptions.repeateImmediately:
                        for (int i = 0; i < o.timesToRepeat; i++)
                        {
                            if (o.randomRepeat)
                            {
                                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(o.randomRepeatTimer.x, o.randomRepeatTimer.y));// waiting for random delay;
                            }
                            else
                            {
                                yield return new WaitForSecondsRealtime(o.repeatTimer);// waiting for delay;
                            }
                            StartCoroutine(objSpawner(o.randomRepeat, o.randomRepeatTimer, o.repeatTimer, o.amountOfObjectsToRespawn, o.spawnerRadius, o.layerToAvoid, o.objectToSpawn, o.forceMode, o.force, o.addForce, o));// starts main repeated spawn
                        }
                        break;
                    default:
                        print("Bad things Happend (( with object spawner.");
                        break;
                }
            }
        }
        /// <summary>
        /// Universal Object spawner.
        /// </summary>
        /// <param name="r">enable random spawn.</param>
        /// <param name="v">random time to spawn.</param>
        /// <param name="t">time to spawn.</param>
        /// <param name="a">amount of objects to spawn.</param>
        /// <param name="sr">spawner radius.</param>
        /// <param name="l">layer to avoid.</param>
        /// <param name="g">gameobject to spawn.</param>
        private IEnumerator objSpawner(bool r, Vector2 v, float t, int a, float sr, LayerMask l, GameObject g, ForceMode2D forceMode, float force, bool addForce, objToSpawn ots)
        {
            if (r)
            {
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(v.x, v.y));// waiting for random delay;
            }
            else
            {
                yield return new WaitForSecondsRealtime(t);// waiting for delay;
            }

            List<Vector3Int> tempPositions = new List<Vector3Int>();
            if(ots.targetSurfaceIndex< surfacesData.Count)
            tempPositions = surfacesData[ots.targetSurfaceIndex]._positions;
            if (tempPositions.Count > 0)
            {
                for (int i = 0; i < a; i++) // loop for amount of objects to spawn;
                {
                    switch (ots._spawnObject)
                    {
                        case SpawnObject.prefab:
                            Vector3 tempPoint;
                            for (int u = 0; u < tempPositions.Count; u++)
                            {
                                int _randomIndex1 = Random.Range(0, tempPositions.Count - 1);
                                tempPoint = surfacesData[ots.targetSurfaceIndex]._tileMap.CellToWorld(tempPositions[_randomIndex1])+grid.cellSize/2;
                                if (tempPositions.Contains(tempPositions[_randomIndex1]))
                                {
                                    GameObject obj = Instantiate(g, tempPoint, Quaternion.identity); // instantiate Object to Spawn;
                                    obj.transform.parent = spawnedObjects.transform;
                                    if (ots.addForce) // if add force is true;
                                    {
                                        // Vector2 yourChoosedDirection = new Vector2(...,...); - insert your own vector here and in AddForce;
                                        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * force, forceMode); // Add some force in random direction;
                                    }
                                    break;
                                }
                            }
                            break;
                        case SpawnObject.tile:
                            if (ots.tileSize != Vector2Int.zero)
                            {
                                List<Vector3Int> _p = NeighboursList(ots.tileSize.x, ots.tileSize.y);
                                for (int z = 0; z < tempPositions.Count; z++)
                                {
                                    int _randomIndex = Random.Range(0, tempPositions.Count - 1);
                                    Vector3Int tempPos = tempPositions[_randomIndex];
                                    if (IsContains(ots, tempPositions, _p, tempPos))
                                    {
                                        if (!spawnedTilesPositions.Contains(tempPos))
                                        {
                                            ots._Map.SetTile(tempPos, ots.tileToSpawn);
                                            spawnedTilesPositions.Add(tempPos);
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                int _randomIndex = Random.Range(0, tempPositions.Count - 1);
                                Vector3Int tempPos = tempPositions[_randomIndex];
                                ots._Map.SetTile(tempPos, ots.tileToSpawn);
                                spawnedTilesPositions.Add(tempPos);
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private bool IsContains(objToSpawn ots, List<Vector3Int> positions, List<Vector3Int> tPointsList, Vector3Int tempPos)
        {
            bool value = false;
            List<Vector3Int> temppointsList = new List<Vector3Int>();
            for (int u = 0; u < tPointsList.Count; u++)
            {
                temppointsList.Add(tempPos + tPointsList[u]);
            }
            if (spawnedTilesPositions.Count != 0)
            {
                for (int i = 0; i < temppointsList.Count; i++)
                {
                    if (spawnedTilesPositions.Contains(temppointsList[i]))
                    {
                        return false;
                    }
                }
            }

            for (int h = 0; h < temppointsList.Count; h++)
            {
                if (positions.Contains(temppointsList[h]))
                {
                    value = true;
                }
                else
                {
                    return false;
                }
            }
            return value;
        }
        private List<Vector3Int> NeighboursList(int sizeX, int sizeY)
        {
            List<Vector3Int> tempList = new List<Vector3Int>();
            for (int x = -sizeX; x <= sizeX; x++)
            {
                for (int y = -sizeY; y <= sizeY; y++)
                {
                      tempList.Add(new Vector3Int(x, y, 0));
                }
            }
            return tempList;
        }

        public void Destruct(Vector3Int wallPos, float damage)
        {
            if (healthValues.ContainsKey(wallPos))
            {
                healthValues[wallPos] -= damage;
                if (healthValues[wallPos] <= 0)
                {
                    healthValues.Remove(wallPos);
                    wallsMap.SetTile(wallPos, null);
                    if (burnedGroundTile) groundMap.SetTile(wallPos, burnedGroundTile);
                    GameManager._gameManager.pooledObjectTempPosition = groundMap.CellToWorld(wallPos);
                    onWallDestroy.Invoke();
                }
            }
        }
        
        private void FillIntList()
        {
            surfacesData = new List<surfaceMainData>();

            surfaceMainData data1 = new surfaceMainData();
            data1._positions = new List<Vector3Int>();
            data1._positions = wallTiles;
            data1._tile = walTile;
            data1._tileMap = wallsMap;
            surfacesData.Add(data1);

            surfaceMainData data2 = new surfaceMainData();
            data2._positions = new List<Vector3Int>();
            data2._positions = CreatedTiles;
            data2._tile = groundTile;
            data2._tileMap = groundMap;
            surfacesData.Add(data2);
            if (advanceSurfacesSettings.Count != 0)
            {
                foreach (var surface in advanceSurfacesSettings)
                {
                    if (surface.enableSurface)
                    {
                        surfaceMainData data = new surfaceMainData();
                        data._positions = new List<Vector3Int>();
                        data._positions = surface._tempIntMap;
                        data._tileMap = surface._Map;
                        data._tile = surface._Tile;
                        surfacesData.Add(data);
                    }
                }
            }
        }


        public void StartExampleSimulation(int repeatNumber, Tilemap exMap, TileBase exTile, List<Vector3Int> exIntMap, advancedSurface _surface)
        {
            if(exMap)
            exMap.ClearAllTiles();

            Dictionary<Vector3Int, int> values = new Dictionary<Vector3Int, int>(surfacesData[_surface.TargetSurfaceIndex]._positions.Count);

            InitializeCustomPositions(values, _surface);
            for (int i = 0; i < repeatNumber; i++)
            {
                values = GenerateCustomTilePosition(values, _surface);
            }
            foreach (Vector3Int item in surfacesData[_surface.TargetSurfaceIndex]._positions)
            {
                if (values[item] == 1)
                    exIntMap.Add(new Vector3Int(item.x, item.y, 0));
            }
        }


        public void StartSimulation(int repeatNumber)
        {
            wallsMap.ClearAllTiles();
            groundMap.ClearAllTiles();
            mapWidth = _newGenProperties.tileMapSize.x;
            mapHeight = _newGenProperties.tileMapSize.y;
            
            if (map == null)
            {
                map = new int[mapWidth, mapHeight];
                advancedSurface surf = new advancedSurface();
                InitializePositions(map, true, surf);
            }
            for (int i = 0; i < repeatNumber; i++)
            {
                advancedSurface surf = new advancedSurface();
                map = GenerateTilePosition(map, true, surf);
            }
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (map[x, y] == 1)
                    {
                        wallsMap.SetTile(new Vector3Int(-x + mapWidth / 2, -y + mapHeight / 2, 0), walTile);
                        wallTiles.Add(new Vector3Int(-x + mapWidth / 2, -y + mapHeight / 2, 0));
                    }
                    CreatedTiles.Add(new Vector3Int(-x + mapWidth / 2, -y + mapHeight / 2, 0));
                }
            }
            SetGroundTiles();
            CreateWallValues();
            FindMinMaxValues();
        }
        public void StartExtraSurfaceSimulation()
        {
            //if (curentMap)
            //{
                if (advanceSurfacesSettings.Count != 0)
                {
                    for (int k = 0; k < advanceSurfacesSettings.Count; k++)
                    {
                        if (!string.IsNullOrEmpty(advanceSurfacesSettings[k]._tileMapName))
                        {
                            Tilemap kra = GameObject.Find(advanceSurfacesSettings[k]._tileMapName).GetComponent<Tilemap>();
                            if (kra)
                            {
                                advancedSurface tempsurf = advanceSurfacesSettings[k];
                                tempsurf._Map = kra;
                                advanceSurfacesSettings[k] = tempsurf;
                            }
                        }

                    }
                }

            //}
            if (advanceSurfacesSettings.Count != 0)
            {
                //LoadParameters();
                if(!MapProperties.customMap)
                ClearSurfaces();

                foreach (var surface in advanceSurfacesSettings)
                {
                    if (surface.enableSurface)
                    {
                        //don't forget to manage this problem.
                        if(!MapProperties.customMap)
                        FillIntList();
                        StartExampleSimulation(surface.repeatCount, surface._Map, surface._Tile, surface._tempIntMap, surface);
                    }
                }

                SetSurfacesTiles();

            }
        }
        public void ClearSurfaces()
        {
            LoadParameters();
            foreach (var _surface in advanceSurfacesSettings)
            {
                if (_surface.enableSurface)
                {
                    if (_surface._Map)
                        _surface._Map.ClearAllTiles();
                }
            }
            for (int i = 0; i < advanceSurfacesSettings.Count; i++)
            {
                advancedSurface s = advanceSurfacesSettings[i];
                s._tempIntMap = new List<Vector3Int>();
                advanceSurfacesSettings[i] = s;
            }
        }
        private void SetSurfacesTiles()
        {
            for (int i = 0; i < advanceSurfacesSettings.Count; i++)
            {
                if (advanceSurfacesSettings[i].enableSurface && !advanceSurfacesSettings[i].useCustomSurface)
                {
                    for (int n = 0; n < advanceSurfacesSettings[i]._tempIntMap.Count; n++)
                    {
                        if (surfacesData[advanceSurfacesSettings[i].TargetSurfaceIndex]._positions.Contains(advanceSurfacesSettings[i]._tempIntMap[n]))
                        {
                            advanceSurfacesSettings[i]._Map.SetTile(advanceSurfacesSettings[i]._tempIntMap[n], advanceSurfacesSettings[i]._Tile);
                            //advanceSurfacesSettings[i]._tempIntMap.Remove(advanceSurfacesSettings[i]._tempIntMap[n]);
                            surfacesData[advanceSurfacesSettings[i].TargetSurfaceIndex]._positions.Remove(advanceSurfacesSettings[i]._tempIntMap[n]);
                        }
                        else
                        {
                            advanceSurfacesSettings[i]._tempIntMap.Remove(advanceSurfacesSettings[i]._tempIntMap[n]);
                        }
                    }
                }
            }
        }
        private void SetGroundTiles()
        {
            for (int i = 0; i < wallTiles.Count-1; i++)
            {
                if (CreatedTiles.Contains(wallTiles[i]))
                {
                    CreatedTiles.Remove(wallTiles[i]);
                }
            }
            foreach (Vector3Int tile in CreatedTiles)
            {
                groundMap.SetTile(tile, groundTile);
            }
        }
        public void InitializePositions(int[,] _map, bool isMain, advancedSurface _surface)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (isMain)
                        _map[x, y] = Random.Range(1, 101) < _newGenProperties.wallChance ? 1 : 0;
                    else
                        _map[x, y] = Random.Range(1, 101) < _surface.wallChance ? 1 : 0;
                }
            }
        }
        public void InitializeCustomPositions(Dictionary<Vector3Int,int> _map, advancedSurface _surface)
        {
            foreach (Vector3Int p in surfacesData[_surface.TargetSurfaceIndex]._positions)
            {
                    _map[new Vector3Int(p.x, p.y,0)] = Random.Range(1, 101) < _surface.wallChance ? 1 : 0;
            }
        }
        public int[,] GenerateTilePosition(int[,] oldMap, bool isMainSimulation , advancedSurface _surface)
        {
            int[,] newMap = new int[mapWidth, mapHeight];
            int neighbours;
            BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    neighbours = 0;

                    foreach (Vector3Int position in bounds.allPositionsWithin)
                    {
                        if (position.x == 0 && position.y == 0) continue;
                        if (isMainSimulation)
                        {
                            if (x + position.x >= 0 && x + position.x < mapWidth && y + position.y >= 0 && y + position.y < mapHeight)
                            {
                                neighbours += oldMap[x + position.x, y + position.y];
                            }
                            else
                            {
                                neighbours++;
                            }
                        }
                        else
                        {
                            if (x + position.x >= 0 && x + position.x < mapWidth && y + position.y >= 0 && y + position.y < mapHeight)
                            {
                                neighbours += oldMap[x + position.x, y + position.y];
                            }
                        }
                    }
                    if (isMainSimulation)
                    {
                        if (oldMap[x, y] == 1)
                        {
                            if (neighbours < _newGenProperties.empty) newMap[x, y] = 0;
                            else newMap[x, y] = 1;
                        }
                        if (oldMap[x, y] == 0)
                        {
                            if (neighbours > _newGenProperties.filled) newMap[x, y] = 1;
                            else newMap[x, y] = 0;
                        }
                    }
                    else
                    {
                        if (oldMap[x, y] == 1)
                        {
                            if (neighbours < _surface.empty) newMap[x, y] = 0;
                            else newMap[x, y] = 1;
                        }
                        if (oldMap[x, y] == 0)
                        {
                            if (neighbours > _surface.filled) newMap[x, y] = 1;
                            else newMap[x, y] = 0;
                        }
                    }

                }
            }
            return newMap;
        }

        public Dictionary<Vector3Int, int> GenerateCustomTilePosition(Dictionary<Vector3Int, int> oldMap, advancedSurface _surface)
        {
            Dictionary<Vector3Int, int> newMap = new Dictionary<Vector3Int, int>(surfacesData[_surface.TargetSurfaceIndex]._positions.Count);
            int neighbours;
            BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

            foreach (Vector3Int XYPos in surfacesData[_surface.TargetSurfaceIndex]._positions)
            {
                neighbours = 0;
                foreach (Vector3Int position in bounds.allPositionsWithin)
                {
                    if (position.x == 0 && position.y == 0) continue;

                    if (oldMap.ContainsKey(new Vector3Int(XYPos.x + position.x, XYPos.y + position.y, 0)))
                        neighbours += oldMap[new Vector3Int(XYPos.x + position.x, XYPos.y + position.y, 0)];
                }
                if (oldMap[new Vector3Int(XYPos.x, XYPos.y, 0)] == 1)
                {
                    if (neighbours < _surface.empty) newMap[new Vector3Int(XYPos.x, XYPos.y, 0)] = 0;
                    else newMap[new Vector3Int(XYPos.x, XYPos.y, 0)] = 1;
                }
                if (oldMap[new Vector3Int(XYPos.x, XYPos.y, 0)] == 0)
                {
                    if (neighbours > _surface.filled) newMap[new Vector3Int(XYPos.x, XYPos.y, 0)] = 1;
                    else newMap[new Vector3Int(XYPos.x, XYPos.y, 0)] = 0;
                }
            }
            return newMap;
        }

        public void LoadParameters()
        {
                walTile = MapProperties.wallTile;
                groundTile = MapProperties.groundTile;
                burnedGroundTile = MapProperties.burnedGroundTile;
                generationType = MapProperties.worldGenerationType;
                wallsHealth = MapProperties.wallsHealth;
                _newGenProperties.wallChance = MapProperties.wallChance;
                _newGenProperties.filled = MapProperties.filled;
                _newGenProperties.empty = MapProperties.empty;
                _newGenProperties.repeatCount = MapProperties.repeatCount;
                _newGenProperties.tileMapSize = MapProperties.tileMapSize;
                Enemies = MapProperties.Enemies;
                enemyAmount = MapProperties.enemyAmount;
                _oldGenProperties.TileAmount = MapProperties.tileAmount;
                TileSize = 1;
                _oldGenProperties.chanceUp = MapProperties.chanceUp;
                _oldGenProperties.chanceRight = MapProperties.chanceRight;
                _oldGenProperties.chanceDown = MapProperties.chanceDown;
                _oldGenProperties.ExtraWallsX = MapProperties.ExtraWallsX;
                _oldGenProperties.ExtraWallsY = MapProperties.ExtraWallsY;

                ObjectsSpawner = MapProperties.spawner;
            if (ObjectsSpawner.Count != 0)
            {
                for (int i = 0; i < MapProperties.spawner.Count; i++)
                {
                    if (ObjectsSpawner[i].enableSpawner)
                    {
                        objToSpawn spawner = ObjectsSpawner[i];
                        if (!string.IsNullOrEmpty(spawner._tileMapName))
                        {
                            spawner._Map = GameObject.Find(spawner._tileMapName).GetComponent<Tilemap>();
                            if (spawner._Map)
                                ObjectsSpawner[i] = spawner;
                        }
                    }
                }
            }
            advanceSurfacesSettings = new List<advancedSurface>();
            advanceSurfacesSettings = MapProperties.extraSurface;
            for (int k = 0; k < advanceSurfacesSettings.Count; k++)
            {
                if (advanceSurfacesSettings[k].enableSurface)
                {
                    advancedSurface s = new advancedSurface();
                    s = advanceSurfacesSettings[k];
                    if (!s._Map)
                    {
                        s._Map = GameObject.Find(s._tileMapName).GetComponent<Tilemap>();
                        advanceSurfacesSettings[k] = s;
                    }
                }
            }
        }
        public void ClearValues()
        {
            wallTiles.Clear();
            CreatedTiles.Clear();
        }
        private void FillWallHealthValues()
        {
            healthValues = new Dictionary<Vector3Int, float>();
            for (int i = 0; i < wallTiles.Count; i++)
            {
                healthValues.Add(wallTiles[i], wallsHealth);
            }
        }
        public void FillCustomWallHealthValues()
        {
            healthValues = new Dictionary<Vector3Int, float>();
            for (int i = 0; i < surfacesData[1]._positions.Count; i++)
            {
                healthValues.Add(surfacesData[1]._positions[i], wallsHealth);
            }
        }
    }
    [System.Serializable]
    public struct advancedSurface
    {
        public string SurfaceName;
        public bool enableSurface;
        public bool useCustomSurface;
        [Tooltip("")]
        public int TargetSurfaceIndex;
        public string _tileMapName;
        public Tilemap _Map;
        public TileBase _Tile;
        [HideInInspector]
        public List<Vector3Int> _tempIntMap;
        [Tooltip("Chance to wall appear on map.")]
        [Range(0, 100)]
        public int wallChance;
        [Tooltip("Value represents, how many filled tiles can be around tile.")]
        [Range(1, 8)]
        public int filled;
        [Tooltip("Value represents, how many empty tiles can be around tile.")]
        [Range(1, 8)]
        public int empty;
        [Tooltip("Value represents, how many times repeat walls generating(nore times = more solid walls).")]
        [Range(1, 100)]
        public int repeatCount;
    }
    public enum SpawnObject { prefab, tile };
    [System.Serializable]
    public struct objToSpawn
    {
        [Space]
        [Tooltip("Name of object you want to spawn.")]
        public string spawnObjectName;
        public bool enableSpawner;
        public Tilemap _Map;
        public string _tileMapName;
        public SpawnObject _spawnObject;
        [Header("Main Settings.")]
        [Tooltip("On what map you want to place tile.")]
        public int targetSurfaceIndex;
        [Tooltip("Object will NOT spawn on GameObjects on this Layers.")]
        public LayerMask layerToAvoid;
        public Vector2Int tileSize;
        [Tooltip("object you want to spawn.")]
        public GameObject objectToSpawn;
        [Tooltip("tile you want to place.")]
        public TileBase tileToSpawn;
        [Tooltip("Enable Random Time spawn.")]
        public bool enableRandomSpawn;
        [Tooltip("1.No Repeate - Spawn objects normally, without repeat.  2.Repeat After Main SpawnTime -Start spawn repeat, after main spawn.   3. Repeate Immediately - starts repeat spawn , without main spawn.")]
        public repeatOptions RepeateOptions;
        [Tooltip("Adds force to spawned objects in random direction.(Objects needed to have RigidBody2D)")]
        public bool addForce;
        [Tooltip("Force Mode.")]
        public ForceMode2D forceMode;
        [Tooltip("Force.")]
        [Range(0, 10000)]
        public float force;
        [Header("Single Spawn Options")]
        [Tooltip("Amount of objects you want to spawn.")]
        [Range(0, 100)]
        public int amountOfObjectsToSpawn;
        [Tooltip("Delay in seconds to start spawning objects.")]
        [Range(0, 100)]
        public float timeToSpawn;
        [Tooltip("Radius of spawn area.")]
        [Range(0, 1000)]
        public float spawnerRadius;
        [Header("Random Spawn Settings.")]
        [Space]
        [Tooltip("Random delay in seconds. X - MIN. Y - MAX.")]
        public Vector2 randomTimeToSpawn;
        [Header("Repeat Spawn Settings.")]
        [Space]
        [Tooltip("Amount of times to repeat.")]
        [Range(0, 100)]
        public int timesToRepeat;
        [Tooltip("Amount of objects you want to spawn.")]
        [Range(0, 100)]
        public int amountOfObjectsToRespawn;
        [Tooltip("Repeating Delay in seconds.")]
        [Range(0, 100)]
        public float repeatTimer;
        [Tooltip("Enables repeating with random delay.")]
        public bool randomRepeat;
        [Tooltip("Random repeating delay in seconds. X - MIN. Y - MAX.")]
        public Vector2 randomRepeatTimer;
    }
    public enum genType
    {
        _old,
        _new
    }
    public enum repeatOptions
    {
        noRepeate,
        repeatAfterMainSpawnTime,
        repeateImmediately
    }
}