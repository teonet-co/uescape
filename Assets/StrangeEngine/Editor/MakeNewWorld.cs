using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using UnityEngine.Tilemaps;
namespace StrangeEngine
{
    [System.Serializable]
    public class MakeNewWorld : EditorWindow
    {
        #region variables
        //
        private Texture texture;
        private Vector2 scrollPos = Vector2.zero;
        private int worldIndex;
        private List<World> _worlds;
        private string[] _worldsNames;
        private string worldName;
        // worlds variables.
        // procedural gen variables;
        public int enemyAmount;
        public int tileAmount;
        public int tileSize = 1;
        public float chanceUp;                              //chance generator to move up;
        public float chanceRight;                           //chance generator to move right;
        public float chanceDown;                            //chance generator to move down;
        public int ExtraWallsX;                             // Amount of Extra walls to add by x axes;
        public int ExtraWallsY;                           // Amount of Extra walls to add by y axes;
        // random gen variables;
        public int wallChance;
        public int filled;
        public int empty;
        public int repeatCount;
        public Vector3Int tileMapSize;
        //

        // other variables;
        public genType worldGenerationType;
        public float wallsHealth;
        public TileBase groundTile;
        public TileBase wallTile;
        public TileBase burnedGroundTile;
        //
        public List<Object> Enemies;
        public int maxPersuitingEnemies;
        public int maxActiveEnemies;
        //
        World selectedWorld;
        World UseWorld;

        //World worldProp;
        AnimBool showPreview;
        int appliedWorldIndex;
        private int tempWorldIndex;
        private int tempExtraSurfaceIndex;
        private int genTypeIndex;
        private string[] generetionType = new string[] { "Old", "New" };


        public advancedSurface extraSurface;
        private List<string> surfacesNames;
        private List<string> allSurfacesNames;
        private int surfaceIndex;
        private int allSurfacesIndex;
        private LevelGeneration _levelGeneration;
        private float _fade = 0;
        private string _tempSurfaceName;

        public objToSpawn spawner;
        private List<string> spawnerNames;
        private int spawnerIndex;
        private int tempSpawnerIndex;
        private AnimBool spawnerFaded;
        private float _fadeSpawerAddButton;
        private string _tempSpawnerName;
        private bool _fadeAddWorldButton;
        private bool _fadeExtraSurfaces;
        private bool _fadeObjectSpawner;
        private int surfaceIndexToSpawnPlayer;
        Worlds wo;
        WorldProperties worldProp;
       //
       #endregion
       [MenuItem("Strange Engine/World Maker", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow<MakeNewWorld>("Worlds");
        }
       private void OnEnable()
        {
            if(EditorApplication.isPlaying|| EditorApplication.isPaused)
            {
                EditorGUILayout.HelpBox("World Maker work in Play mode is not supported ", MessageType.Warning);
            }else if (EditorApplication.isCompiling)
            {
                EditorGUILayout.HelpBox("World Maker can't work, while compiling ", MessageType.Warning);
            }
            else
            {
                scrollPos = Vector2.zero;
                generetionType = new string[] { "Old", "New" };
                string[] results = AssetDatabase.FindAssets("WorldsMaker");
                string textutrePath = AssetDatabase.GUIDToAssetPath(results[0]);
                texture = (Texture)AssetDatabase.LoadAssetAtPath(textutrePath, typeof(Texture));
                showPreview = new AnimBool(false);
                showPreview.valueChanged.AddListener(Repaint);
                tempExtraSurfaceIndex = 0;
                surfaceIndex = 0;
                surfacesNames = new List<string>();
                _levelGeneration = FindObjectOfType<LevelGeneration>();
                if (_levelGeneration == null)
                    return;
                _fade = 0;

                string[] results1 = AssetDatabase.FindAssets("scrObject_Worlds");
                string scrObjectPath = AssetDatabase.GUIDToAssetPath(results1[0]);
                wo = (Worlds)AssetDatabase.LoadAssetAtPath(scrObjectPath, typeof(Worlds));
                worldProp = wo.worlds[worldIndex].properties;
                if (worldProp.spawner.Count != 0)
                    spawner = worldProp.spawner[0];
                if (worldProp.extraSurface.Count != 0)
                    extraSurface = worldProp.extraSurface[0];
                spawnerIndex = 0;
                tempSpawnerIndex = 0;
                allSurfacesNames = new List<string>();
                if (!allSurfacesNames.Contains(_levelGeneration.wallsMap.name))
                    allSurfacesNames.Add(_levelGeneration.wallsMap.name);
                if (!allSurfacesNames.Contains(_levelGeneration.groundMap.name))
                    allSurfacesNames.Add(_levelGeneration.groundMap.name);
                if (worldProp.extraSurface.Count != 0)
                {
                    for (int i = 0; i < worldProp.extraSurface.Count; i++)
                    {
                        if (!allSurfacesNames.Contains(worldProp.extraSurface[i].SurfaceName))
                        {
                            allSurfacesNames.Add(worldProp.extraSurface[i].SurfaceName);
                        }
                    }
                }
                worldIndex = tempWorldIndex;
                if (worldIndex > wo.worlds.Count)
                    worldIndex = 0;
                switch (worldProp.worldGenerationType)
                {
                    case genType._old:
                        genTypeIndex = 0;
                        break;
                    case genType._new:
                        genTypeIndex = 1;
                        break;
                    default:
                        break;
                }
                surfaceIndexToSpawnPlayer = worldProp.surfaceIndexToSpawnPlayer;
            }

        }
        private void OnDisable()
        {
            tempWorldIndex = worldIndex;
        }
        void OnGUI()
        {
            if (EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                EditorGUILayout.HelpBox("World Maker work in Play mode is not supported ", MessageType.Warning);
            }
            else if (EditorApplication.isCompiling)
            {
                EditorGUILayout.HelpBox("World Maker can't work, while compiling ", MessageType.Warning);
            }
            else if (_levelGeneration == null)
            {
                EditorGUILayout.HelpBox("No Level Generation component in Scene !", MessageType.Warning);
                OnEnable();
            }
            else
            {
                //reference to _worlds.asset
                string[] results1 = AssetDatabase.FindAssets("scrObject_Worlds");
                string scrObjectPath = AssetDatabase.GUIDToAssetPath(results1[0]);
                wo = (Worlds)AssetDatabase.LoadAssetAtPath(scrObjectPath, typeof(Worlds));
                _worlds = wo.worlds;
                //setting length of _worldsNames array.
                _worldsNames = new string[_worlds.Count];
                // matching _worlds and _worldsNames.
                for (int i = 0; i < _worlds.Count; i++)
                {
                    _worldsNames[i] = _worlds[i].name;
                }
                // Scroll Area Begin.
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(10), GUILayout.MaxHeight(800));
                GUILayout.Label(texture, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(80));

                //Buttons to manipulate with worlds;
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(new GUIContent("Select World", "Save and select world to use it in current scene."), EditorStyles.miniButtonLeft))
                {
                    SynchronizeWorlds(wo, worldIndex);
                    SaveChanges();
                    selectedWorld.properties.tileSize = 1;
                    worldProp = selectedWorld.properties;
                    if (!_levelGeneration)
                    {
                        _levelGeneration = FindObjectOfType<LevelGeneration>();
                    }
                    if(worldProp.customMap){
                        FillIntList(_levelGeneration,worldProp.extraSurface);
                        _levelGeneration.FindCustomMapMinMaxValues();
                    }
                    _levelGeneration.MapProperties = worldProp;
                    if (!worldProp.customMap)
                    {
                        FindObjectOfType<GenerateLevel>().ClearMap();
                        _levelGeneration.canGenerateInEditMode = false;
                    }

                    GameManager g = FindObjectOfType<GameManager>();
                    g.maxActiveEnemies = selectedWorld.properties.maxActiveEnemies;
                    g.maxPersuitingEnemies = selectedWorld.properties.maxPersuitingEnemies;
                    //string path = 
                    //EditorUtility.SetDirty(_levelGeneration);
                    var obj = new SerializedObject(_levelGeneration);
                    obj.ApplyModifiedProperties();

                    EditorUtility.DisplayDialog("Done!", "World Saved and Selected!", "OK");
                    
                    
                }
                if (GUILayout.Button(new GUIContent("Generate", "Generate new world."), EditorStyles.miniButtonMid))
                {
                    SaveChanges();
                    worldProp = selectedWorld.properties;
                    _levelGeneration.MapProperties = worldProp;
                    _levelGeneration.canGenerateInEditMode = true;
                    GameManager g = FindObjectOfType<GameManager>();
                    g.maxActiveEnemies = selectedWorld.properties.maxActiveEnemies;
                    g.maxPersuitingEnemies = selectedWorld.properties.maxPersuitingEnemies;
                    FindObjectOfType<GenerateLevel>().ClearMap();

                    FindObjectOfType<GenerateLevel>().GenerateLevelInEditMode();
                    EditorUtility.DisplayDialog("Done!", "World Saved and Generated Succesfully", "OK");
                }
                if (GUILayout.Button(new GUIContent("Save Map To Prefab", "Generates world in Editor Mode and saves it to prefab."), EditorStyles.miniButtonRight))
                {
                    LevelGeneration _levelGenerator = FindObjectOfType<LevelGeneration>();
                    string path = EditorUtility.SaveFilePanelInProject("Save Map", "Map", "prefab",
                    "Please enter a file name to save the Map to");
                    //string savePath = "Assets/" + _levelGenerator.grid.name + ".prefab";
                    if (PrefabUtility.CreatePrefab(path, _levelGenerator.grid.gameObject))
                    {
                        EditorUtility.DisplayDialog("Tilemap saved", "Your Tilemap was saved under " + path, "Continue");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Tilemap NOT saved", "An ERROR occured while trying to saveTilemap under " + path, "Continue");
                    }
                }
                
                // make shure, that selected world is one we needed.

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Save Changes", "Save Changes"), EditorStyles.miniButtonLeft))
                {
                    SaveChanges();
                    EditorUtility.DisplayDialog("Done", "Saved!", "OK");
                }
                if (GUILayout.Button(new GUIContent("Delete current World", "Delete current World."), EditorStyles.miniButtonRight))
                {
                    if (EditorUtility.DisplayDialog("Delete current map", "Remove current map?", "Yes", "No"))
                    {
                        for (int i = 0; i < wo.worlds.Count; i++)
                        {
                            if (wo.worlds[i].name == _worldsNames[worldIndex])
                            {
                                wo.worlds.Remove(wo.worlds[i]);
                            }
                        }
                        AssetDatabase.DeleteAsset("Assets/StrangeEngine/Prefabs/Maps/" + _worldsNames[worldIndex]);
                        //setting length of _worldsNames array.
                        _worldsNames = new string[_worlds.Count];
                        // matching _worlds and _worldsNames.
                        for (int i = 0; i < _worlds.Count; i++)
                        {
                            _worldsNames[i] = _worlds[i].name;
                        }
                        if (worldIndex - 1 < _worlds.Count)
                            worldIndex -= 1;
                        else
                        {
                            worldIndex = 0;
                        }
                        SynchronizeWorlds(wo, worldIndex);
                        tempWorldIndex = worldIndex;
                    }
                }
                EditorGUILayout.EndHorizontal();
                //
                EditorGUILayout.Space();
                GUILayout.Label("Choose world to edit or create new one.", EditorStyles.centeredGreyMiniLabel);
                EditorGUILayout.Space();
                //Main settings;
                EditorGUILayout.BeginHorizontal();
                worldIndex = EditorGUILayout.Popup(worldIndex, _worldsNames);
                if (!_fadeAddWorldButton)
                {
                    if (GUILayout.Button(new GUIContent("Add new world", "Add new World."), EditorStyles.miniButton))
                    {
                        _fadeAddWorldButton = true;
                    }
                }
                else
                {
                    worldName = EditorGUILayout.TextField("New World name:", worldName);
                    //button to add new world
                    if (GUILayout.Button(new GUIContent("Add", "Add new World.")))
                    {
                        string newFolderName = AssetDatabase.CreateFolder("Assets/StrangeEngine/Prefabs/Maps", worldName);
                        string newFolderPath = AssetDatabase.GUIDToAssetPath(newFolderName);
                        World newWorld = ScriptableObject.CreateInstance<World>();
                        AssetDatabase.CreateAsset(newWorld, "Assets/StrangeEngine/Prefabs/Maps/sofushka.asset");
                        AssetDatabase.MoveAsset("Assets/StrangeEngine/Prefabs/Maps/sofushka.asset", newFolderPath + "/sofushka.asset");
                        AssetDatabase.RenameAsset(newFolderPath + "/sofushka.asset", worldName);
                        AssetDatabase.SaveAssets();
                        wo.worlds.Add(newWorld);
                        newWorld.properties.extraSurface = new List<advancedSurface>();
                        newWorld.properties.spawner = new List<objToSpawn>();
                        worldIndex = wo.worlds.Count - 1;
                        //setting length of _worldsNames array.
                        _worldsNames = new string[_worlds.Count];
                        // matching _worlds and _worldsNames.
                        for (int i = 0; i < _worlds.Count; i++)
                        {
                            _worldsNames[i] = _worlds[i].name;
                        }
                        AssetDatabase.ImportAsset("Assets/Editor/MakeNewWorld.cs");
                        AssetDatabase.SaveAssets();
                        EditorUtility.SetDirty(this);
                        AssetDatabase.Refresh();
                        string[] a = AssetDatabase.FindAssets(worldName);
                        string b = AssetDatabase.GUIDToAssetPath(a[0]);
                        AssetDatabase.ImportAsset(b);
                        EditorUtility.SetDirty(this);
                        EditorUtility.SetDirty(newWorld);
                        spawnerIndex = 0;
                        surfaceIndex = 0;
                        worldName = "";
                        _fadeAddWorldButton = false;
                        //		
                    }
                    if (GUILayout.Button(new GUIContent("Cancel", "Cancel.")))
                    {
                        worldName = "";
                        _fadeAddWorldButton = false;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                SynchronizeWorlds(wo, worldIndex);
                if (intHasChanged(tempWorldIndex, worldIndex))
                {

                    SynchronizeWorlds(wo, tempWorldIndex);
                    worldProp = selectedWorld.properties;
                    bool changed = false;

                    if (worldProp.spawner.Count != 0)
                    {
                        tempSpawnerIndex = spawnerIndex;
                        objToSpawn tempSpawner = worldProp.spawner[tempSpawnerIndex];
                        spawner._Map = null;
                        tempSpawner._Map = null;

                        if (!spawner.Equals(tempSpawner))
                        {
                            changed = true;
                        }
                    }
                    if (worldProp.extraSurface.Count != 0)
                    {
                        tempExtraSurfaceIndex = 0;
                        tempExtraSurfaceIndex = surfaceIndex;
                        advancedSurface tempSurface = worldProp.extraSurface[tempExtraSurfaceIndex];
                        extraSurface._Map = null;
                        tempSurface._Map = null;

                        if (!extraSurface.Equals(tempSurface))
                        {
                            changed = true;
                        }
                    }

                    if (!worldProp.Equals(selectedWorld.properties))
                    {
                        changed = true;
                    }
                    if (changed)
                    {

                        int option = EditorUtility.DisplayDialogComplex("Seems like there are some unsaved changes", "Please choose one of the following options.", "Save and Continue", "Continue without saving", "Cancel");
                        switch (option)
                        {
                            case 0:
                                SynchronizeWorlds(wo, tempWorldIndex);
                                worldProp = selectedWorld.properties;
                                SaveChanges();

                                SynchronizeWorlds(wo, worldIndex);
                                worldProp = selectedWorld.properties;

                                if (worldProp.surfaceIndexToSpawnPlayer > allSurfacesNames.Count)
                                {
                                    surfaceIndexToSpawnPlayer = 0;
                                }
                                else
                                    surfaceIndexToSpawnPlayer = worldProp.surfaceIndexToSpawnPlayer;
                                if (worldProp.extraSurface.Count != 0)
                                    extraSurface = worldProp.extraSurface[surfaceIndex];
                                if (worldProp.spawner.Count != 0)
                                    spawner = worldProp.spawner[spawnerIndex];
                                break;
                            case 1:
                                SynchronizeWorlds(wo, worldIndex);
                                worldProp = selectedWorld.properties;
                                if (worldProp.surfaceIndexToSpawnPlayer > allSurfacesNames.Count)
                                {
                                    surfaceIndexToSpawnPlayer = 0;
                                }
                                else
                                    surfaceIndexToSpawnPlayer = worldProp.surfaceIndexToSpawnPlayer;
                                if (worldProp.extraSurface.Count != 0)
                                    extraSurface = worldProp.extraSurface[surfaceIndex];
                                if (worldProp.spawner.Count != 0)
                                    spawner = worldProp.spawner[spawnerIndex];

                                switch (worldProp.worldGenerationType)
                                {
                                    case genType._old:
                                        genTypeIndex = 0;
                                        break;
                                    case genType._new:
                                        genTypeIndex = 1;
                                        break;
                                    default:
                                        break;
                                }
                                tempWorldIndex = worldIndex;
                                break;
                            case 2:
                                SynchronizeWorlds(wo, tempWorldIndex);
                                worldIndex = tempWorldIndex;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        SynchronizeWorlds(wo, worldIndex);
                        worldProp = selectedWorld.properties;
                        worldProp.tileSize = selectedWorld.properties.tileSize;
                        worldProp.wallsHealth = selectedWorld.properties.wallsHealth;
                        worldProp.groundTile = selectedWorld.properties.groundTile;
                        worldProp.wallTile = selectedWorld.properties.wallTile;
                        worldProp.burnedGroundTile = selectedWorld.properties.burnedGroundTile;
                        worldProp.enemyAmount = selectedWorld.properties.enemyAmount;

                        worldProp.tileAmount = selectedWorld.properties.tileAmount;
                        worldProp.chanceUp = selectedWorld.properties.chanceUp;
                        worldProp.chanceRight = selectedWorld.properties.chanceRight;
                        worldProp.chanceDown = selectedWorld.properties.chanceDown;
                        worldProp.ExtraWallsX = selectedWorld.properties.ExtraWallsX;
                        worldProp.ExtraWallsY = selectedWorld.properties.ExtraWallsY;

                        worldProp.wallChance = selectedWorld.properties.wallChance;
                        worldProp.filled = selectedWorld.properties.filled;
                        worldProp.empty = selectedWorld.properties.empty;
                        worldProp.repeatCount = selectedWorld.properties.repeatCount;
                        worldProp.tileMapSize = selectedWorld.properties.tileMapSize;
                        worldProp.surfaceIndexToSpawnPlayer = selectedWorld.properties.surfaceIndexToSpawnPlayer;
                        if (worldProp.surfaceIndexToSpawnPlayer > allSurfacesNames.Count)
                        {
                            surfaceIndexToSpawnPlayer = 0;
                        }
                        else
                            surfaceIndexToSpawnPlayer = worldProp.surfaceIndexToSpawnPlayer;
                        if (worldProp.extraSurface.Count != 0)
                        {
                            surfaceIndex = 0;
                            tempExtraSurfaceIndex = surfaceIndex;

                            extraSurface = worldProp.extraSurface[surfaceIndex];
                        }

                        if (worldProp.spawner.Count != 0)
                        {
                            spawnerIndex = 0;
                            tempSpawnerIndex = spawnerIndex;
                            spawner = worldProp.spawner[spawnerIndex];
                        }

                        switch (worldProp.worldGenerationType)
                        {
                            case genType._old:
                                genTypeIndex = 0;
                                break;
                            case genType._new:
                                genTypeIndex = 1;
                                break;
                            default:
                                break;
                        }
                    }
                    //


                    if (worldProp.extraSurface.Count != 0)
                    {

                        if (worldProp.extraSurface[surfaceIndex].enableSurface)
                        {
                            if (!string.IsNullOrEmpty(worldProp.extraSurface[surfaceIndex]._tileMapName))
                            {
                                if (GameObject.Find(worldProp.extraSurface[surfaceIndex]._tileMapName))
                                    extraSurface._Map = GameObject.Find(worldProp.extraSurface[surfaceIndex]._tileMapName).GetComponent<Tilemap>();
                                else
                                    extraSurface._Map = null;
                            }
                            else
                                extraSurface._Map = null;
                        }
                    }

                    if (worldProp.spawner.Count != 0)
                    {
                        if (worldProp.spawner[spawnerIndex].enableSpawner)
                        {
                            if (!string.IsNullOrEmpty(worldProp.spawner[spawnerIndex]._tileMapName))
                            {
                                if (GameObject.Find(worldProp.spawner[spawnerIndex]._tileMapName))
                                    spawner._Map = GameObject.Find(worldProp.spawner[spawnerIndex]._tileMapName).GetComponent<Tilemap>();
                                else
                                    spawner._Map = null;
                            }
                            else
                                spawner._Map = null;
                        }
                    }
                    tempWorldIndex = worldIndex;
                }

                EditorGUILayout.LabelField("Main settings", EditorStyles.boldLabel);
                worldProp.customMap = EditorGUILayout.Toggle("Use custom map", worldProp.customMap);

                if (worldProp.surfaceIndexToSpawnPlayer > allSurfacesNames.Count)
                {
                    surfaceIndexToSpawnPlayer = 0;
                }

                surfaceIndexToSpawnPlayer = EditorGUILayout.Popup("Select surface to spawn Player", surfaceIndexToSpawnPlayer, allSurfacesNames.ToArray());
                if(!worldProp.customMap){
                
                genTypeIndex = EditorGUILayout.Popup("Level Generation type", genTypeIndex, generetionType);

                switch (genTypeIndex)
                {
                    case 0:
                        worldProp.worldGenerationType = genType._old;

                        worldProp.tileAmount = EditorGUILayout.IntField("Amount of tiles in scene", worldProp.tileAmount);
                        worldProp.chanceUp = EditorGUILayout.Slider("Generator chance to move up", worldProp.chanceUp, 0, 1);
                        worldProp.chanceRight = EditorGUILayout.Slider("Generator chance to move right", worldProp.chanceRight, 0, 1);
                        worldProp.chanceDown = EditorGUILayout.Slider("Generator chance to move down", worldProp.chanceDown, 0, 1);
                        worldProp.ExtraWallsX = EditorGUILayout.IntField("Extra walls on X axes", worldProp.ExtraWallsX);
                        worldProp.ExtraWallsY = EditorGUILayout.IntField("Extra walls on Y axes", worldProp.ExtraWallsY);
                        break;
                    case 1:
                        worldProp.worldGenerationType = genType._new;
                        worldProp.wallChance = EditorGUILayout.IntSlider(new GUIContent("Walls chance", "Wall chance."), worldProp.wallChance, 0, 100);
                        worldProp.filled = EditorGUILayout.IntSlider(new GUIContent("Filled(existing) neihbours", "How many existing neighbours around should be, to place tile."), worldProp.filled, 1, 8);
                        worldProp.empty = EditorGUILayout.IntSlider(new GUIContent("Empty neihbours", "How many empty neighbours around should be, to place tile."), worldProp.empty, 1, 8);
                        worldProp.repeatCount = EditorGUILayout.IntSlider(new GUIContent("Repeate count", "How many times should generator repeat checkin for neighbours."), worldProp.repeatCount, 1, 100);
                        worldProp.tileMapSize = EditorGUILayout.Vector3IntField("TileMap size", worldProp.tileMapSize);
                        break;
                    default:
                        break;
                }
                EditorGUILayout.Space();
                }
                worldProp.wallsHealth = EditorGUILayout.FloatField("Walls health value", worldProp.wallsHealth);
                EditorGUILayout.Space();
                if(!worldProp.customMap){
                EditorGUILayout.LabelField("Tiles", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                worldProp.groundTile = (TileBase)EditorGUILayout.ObjectField("Ground tile", worldProp.groundTile, typeof(TileBase), true);
                worldProp.wallTile = (TileBase)EditorGUILayout.ObjectField("Walls tile", worldProp.wallTile, typeof(TileBase), true);
            }
            worldProp.burnedGroundTile = (TileBase)EditorGUILayout.ObjectField("Burned ground tile", worldProp.burnedGroundTile, typeof(TileBase), true);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Enemies settings", EditorStyles.boldLabel);
                worldProp.maxActiveEnemies = EditorGUILayout.IntField("Maximum active enemies", worldProp.maxActiveEnemies);
                worldProp.maxPersuitingEnemies = EditorGUILayout.IntField("Maximum pursuiting enemies", worldProp.maxPersuitingEnemies);
                //
                // Extra surfaces settings;
                EditorGUILayout.Space();

                _fadeExtraSurfaces = EditorGUILayout.ToggleLeft("Extra Surfaces Settings", _fadeExtraSurfaces, EditorStyles.boldLabel);
                if (_fadeExtraSurfaces)
                {
                    if (worldProp.extraSurface.Count != 0)
                    {
                        EditorGUILayout.LabelField("Choose Extra Surface to edit.", EditorStyles.centeredGreyMiniLabel);

                        surfacesNames = new List<string>();
                        for (int i = 0; i <= worldProp.extraSurface.Count - 1; i++)
                        {
                            if (!surfacesNames.Contains(worldProp.extraSurface[i].SurfaceName))
                            {
                                surfacesNames.Add(worldProp.extraSurface[i].SurfaceName);
                            }
                        }

                        allSurfacesNames = new List<string>();
                        if (!allSurfacesNames.Contains(_levelGeneration.wallsMap.name))
                            allSurfacesNames.Add(_levelGeneration.wallsMap.name);
                        if (!allSurfacesNames.Contains(_levelGeneration.groundMap.name))
                            allSurfacesNames.Add(_levelGeneration.groundMap.name);
                        for (int i = 0; i <= worldProp.extraSurface.Count - 1; i++)
                        {
                            if (!allSurfacesNames.Contains(worldProp.extraSurface[i].SurfaceName))
                            {
                                allSurfacesNames.Add(worldProp.extraSurface[i].SurfaceName);
                            }
                        }
                    }

                    if (surfaceIndex >= surfacesNames.Count)
                        surfaceIndex = surfacesNames.Count - 1;
                    if (worldProp.extraSurface.Count != 0)
                    {

                        surfaceIndex = EditorGUILayout.Popup(surfaceIndex, surfacesNames.ToArray());

                        if (intHasChanged(tempExtraSurfaceIndex, surfaceIndex))
                        {
                            advancedSurface tempSurface;
                            if (tempExtraSurfaceIndex <= worldProp.extraSurface.Count)
                            {
                                tempSurface = worldProp.extraSurface[tempExtraSurfaceIndex];
                                extraSurface._Map = null;
                                tempSurface._Map = null;
                            }
                            else
                            {
                                tempSurface = worldProp.extraSurface[surfaceIndex];
                                extraSurface._Map = null;
                                tempSurface._Map = null;
                            }

                            if (!extraSurface.Equals(tempSurface))
                            {

                                int option = EditorUtility.DisplayDialogComplex("Seems like there are some unsaved changes in Surface Settings.", "Please choose one of the following options.", "Save and Continue", "Continue without saving", "Cancel");
                                switch (option)
                                {
                                    case 0:
                                        extraSurface.SurfaceName = worldProp.extraSurface[tempExtraSurfaceIndex].SurfaceName;
                                        worldProp.extraSurface[tempExtraSurfaceIndex] = extraSurface;
                                        tempExtraSurfaceIndex = surfaceIndex;
                                        extraSurface = worldProp.extraSurface[surfaceIndex];
                                        break;
                                    case 1:
                                        extraSurface = worldProp.extraSurface[surfaceIndex];
                                        tempExtraSurfaceIndex = surfaceIndex;
                                        break;
                                    case 2:
                                        surfaceIndex = tempExtraSurfaceIndex;
                                        break;
                                    default:
                                        break;
                                }

                            }
                            else
                            {
                                extraSurface = worldProp.extraSurface[surfaceIndex];
                                tempExtraSurfaceIndex = surfaceIndex;
                            }
                            if (!string.IsNullOrEmpty(worldProp.extraSurface[surfaceIndex]._tileMapName))
                            {
                                extraSurface._Map = GameObject.Find(worldProp.extraSurface[surfaceIndex]._tileMapName).GetComponent<Tilemap>();
                                extraSurface._tileMapName = worldProp.extraSurface[surfaceIndex]._tileMapName;
                            }
                            else
                                extraSurface._Map = null;
                        }
                        EditorGUILayout.Space();
                        extraSurface.enableSurface = EditorGUILayout.Toggle("Enable surface", extraSurface.enableSurface);

                        if (extraSurface.enableSurface)
                        {
                            if (intHasChanged(tempExtraSurfaceIndex, surfaceIndex))
                            {
                                if (worldProp.extraSurface.Count != 0)
                                {
                                    extraSurface = worldProp.extraSurface[surfaceIndex];
                                    if (!string.IsNullOrEmpty(worldProp.extraSurface[surfaceIndex]._tileMapName))
                                        extraSurface._Map = GameObject.Find(worldProp.extraSurface[surfaceIndex]._tileMapName).GetComponent<Tilemap>();
                                    else
                                        extraSurface._Map = null;
                                }
                            }
                            extraSurface.useCustomSurface = EditorGUILayout.Toggle("Use custom extra surface", extraSurface.useCustomSurface);

                            if (!extraSurface.useCustomSurface)
                            {
                                extraSurface.TargetSurfaceIndex = EditorGUILayout.Popup("Choose target surface", extraSurface.TargetSurfaceIndex, allSurfacesNames.ToArray(), GUILayout.ExpandWidth(false));
                            }
                            extraSurface._Map = (Tilemap)EditorGUILayout.ObjectField("TileMap", extraSurface._Map, typeof(Tilemap), true);
                            if (extraSurface._Map)
                                extraSurface._tileMapName = extraSurface._Map.name;
                            else
                                extraSurface._tileMapName = null;
                            if (!extraSurface.useCustomSurface)
                            {
                                extraSurface._Tile = (TileBase)EditorGUILayout.ObjectField("Tile", extraSurface._Tile, typeof(TileBase), true);
                                extraSurface.wallChance = EditorGUILayout.IntSlider("Walls chance", extraSurface.wallChance, 0, 100);
                                extraSurface.filled = EditorGUILayout.IntSlider(new GUIContent("Filled(existing) neihbours", "How many existing neighbours around should be, to place tile."), extraSurface.filled, 1, 8);
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PrefixLabel(new GUIContent("Empty neihbours", "How many empty neighbours around should be, to place tile."));
                                extraSurface.empty = EditorGUILayout.IntSlider(extraSurface.empty, 1, 8);
                                EditorGUILayout.EndHorizontal();
                                extraSurface.repeatCount = EditorGUILayout.IntSlider("Repeate count", extraSurface.repeatCount, 1, 100);
                            }
                        }
                    }
                    if (_fade == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(new GUIContent("Add new surface", "Add new surface."), EditorStyles.miniButtonLeft))
                        {
                            if (_fade == 1)
                                _fade = 0;
                            else
                                _fade = 1;
                        }
                        if (GUILayout.Button(new GUIContent("Remove this surface", "Remove current surface."), EditorStyles.miniButtonRight))
                        {
                            if (worldProp.extraSurface.Count != 0)
                            {
                                worldProp.extraSurface.Remove(worldProp.extraSurface[surfaceIndex]);
                                if (worldProp.extraSurface.Count != 0)
                                {
                                    surfaceIndex = 0;
                                    extraSurface = worldProp.extraSurface[surfaceIndex];
                                    tempExtraSurfaceIndex = surfaceIndex;
                                }
                                EditorUtility.DisplayDialog("Removed", "Surface succesfully removed", "OK");
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("No surfaces!", "No surfaces to remove", "OK");
                            }

                        }
                        EditorGUILayout.EndHorizontal();
                    }


                    if (EditorGUILayout.BeginFadeGroup(_fade))
                    {
                        _fade = 1;
                        EditorGUILayout.BeginHorizontal();
                        _tempSurfaceName = EditorGUILayout.TextField("Surface Name", _tempSurfaceName);
                        if (GUILayout.Button(new GUIContent("Add", "Add")))
                        {
                            bool exists = false;
                            foreach (string _name in surfacesNames)
                            {
                                if (_tempSurfaceName == _name)
                                {
                                    exists = true;
                                    EditorUtility.DisplayDialog("Surface name already exists!", "Please type new Suface name!", "OK");
                                }
                            }
                            if (!exists)
                            {
                                advancedSurface surface = new advancedSurface();
                                surface.SurfaceName = _tempSurfaceName;
                                worldProp.extraSurface.Add(surface);
                                _tempSurfaceName = "";
                                _fade = 0;
                                tempExtraSurfaceIndex = 0;
                                surfaceIndex = 0;
                                EditorUtility.DisplayDialog("Done!", "Surface Added", "OK");
                            }
                        }
                        if (GUILayout.Button(new GUIContent("Cancel", "Cancel")))
                        {
                            _fade = 0;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.EndFadeGroup();
                    //
                }

                // Object spawner Settings;
                EditorGUILayout.Space();
                _fadeObjectSpawner = EditorGUILayout.ToggleLeft("Object Spawner Settings", _fadeObjectSpawner, EditorStyles.boldLabel);
                if (_fadeObjectSpawner)
                {
                    if (worldProp.spawner.Count != 0)
                    {
                        if (worldProp.spawner.Count != 0)
                        {
                            spawnerNames = new List<string>();
                            for (int i = 0; i <= worldProp.spawner.Count - 1; i++)
                            {
                                if (!spawnerNames.Contains(worldProp.spawner[i].spawnObjectName))
                                {
                                    spawnerNames.Add(worldProp.spawner[i].spawnObjectName);
                                }
                            }
                        }

                        EditorGUILayout.LabelField("Choose Object Spawner to edit", EditorStyles.centeredGreyMiniLabel);
                        spawnerIndex = EditorGUILayout.Popup(spawnerIndex, spawnerNames.ToArray());

                        if (intHasChanged(tempSpawnerIndex, spawnerIndex))
                        {

                            objToSpawn tempSpawner = worldProp.spawner[tempSpawnerIndex];
                            spawner._Map = null;
                            tempSpawner._Map = null;
                            if (!spawner.Equals(tempSpawner))
                            {
                                int option = EditorUtility.DisplayDialogComplex("Seems like there are some unsaved changes", "Please choose one of the following options.", "Save and Continue", "Continue without saving", "Cancel");
                                switch (option)
                                {
                                    case 0:
                                        spawner.spawnObjectName = worldProp.spawner[tempSpawnerIndex].spawnObjectName;
                                        worldProp.spawner[tempSpawnerIndex] = spawner;
                                        tempSpawnerIndex = spawnerIndex;
                                        spawner = worldProp.spawner[spawnerIndex];
                                        break;
                                    case 1:
                                        spawner = worldProp.spawner[spawnerIndex];
                                        tempSpawnerIndex = spawnerIndex;
                                        break;
                                    case 2:
                                        spawnerIndex = tempSpawnerIndex;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                spawner = worldProp.spawner[spawnerIndex];
                                tempSpawnerIndex = spawnerIndex;
                            }
                            if (!string.IsNullOrEmpty(worldProp.spawner[spawnerIndex]._tileMapName))
                            {
                                if (GameObject.Find(worldProp.spawner[spawnerIndex]._tileMapName))
                                    spawner._Map = GameObject.Find(worldProp.spawner[spawnerIndex]._tileMapName).GetComponent<Tilemap>();
                                else
                                    spawner._Map = null;
                            }
                            else
                                spawner._Map = null;
                        }

                        EditorGUILayout.Space();
                        spawner.enableSpawner = EditorGUILayout.Toggle("Enable Spawner", spawner.enableSpawner);

                        if (spawner.enableSpawner)
                        {

                            if (worldProp.extraSurface.Count != 0)
                            {
                                allSurfacesNames = new List<string>();
                                if (!allSurfacesNames.Contains(_levelGeneration.wallsMap.name))
                                    allSurfacesNames.Add(_levelGeneration.wallsMap.name);
                                if (!allSurfacesNames.Contains(_levelGeneration.groundMap.name))
                                    allSurfacesNames.Add(_levelGeneration.groundMap.name);
                                for (int i = 0; i <= worldProp.extraSurface.Count - 1; i++)
                                {
                                    if (!allSurfacesNames.Contains(worldProp.extraSurface[i].SurfaceName))
                                    {
                                        allSurfacesNames.Add(worldProp.extraSurface[i].SurfaceName);
                                    }
                                }
                            }




                            spawner.targetSurfaceIndex = EditorGUILayout.Popup("Target surface", spawner.targetSurfaceIndex, allSurfacesNames.ToArray(), GUILayout.ExpandWidth(true));
                            spawner._spawnObject = (SpawnObject)EditorGUILayout.EnumPopup("Object type", spawner._spawnObject);
                            switch (spawner._spawnObject)
                            {
                                case SpawnObject.prefab:
                                    spawner.objectToSpawn = (GameObject)EditorGUILayout.ObjectField("GameObject to Spawn", spawner.objectToSpawn, typeof(GameObject), true);

                                    break;
                                case SpawnObject.tile:
                                    spawner.tileSize = EditorGUILayout.Vector2IntField("Tile size", spawner.tileSize);

                                    spawner.tileToSpawn = (TileBase)EditorGUILayout.ObjectField("Tile to Spawn", spawner.tileToSpawn, typeof(TileBase), true);
                                    spawner._Map = (Tilemap)EditorGUILayout.ObjectField("Tilemap", spawner._Map, typeof(Tilemap), true);
                                    if (spawner._Map)
                                        spawner._tileMapName = spawner._Map.name;
                                    else
                                        spawner._tileMapName = null;
                                    break;
                                default:
                                    break;
                            }


                            spawner.addForce = EditorGUILayout.Toggle("Add force", spawner.addForce, EditorStyles.radioButton);
                            if (spawner.addForce)
                            {
                                spawner.forceMode = (ForceMode2D)EditorGUILayout.EnumPopup("Force mode", spawner.forceMode);
                                spawner.force = EditorGUILayout.Slider("Force", spawner.force, 0, 10000);
                            }

                            spawner.amountOfObjectsToSpawn = EditorGUILayout.IntSlider(new GUIContent("Objects amount", "Amount of objects to spawn"), spawner.amountOfObjectsToSpawn, 0, 1000);
                            spawner.enableRandomSpawn = EditorGUILayout.Toggle("Enable random timer", spawner.enableRandomSpawn, EditorStyles.radioButton);
                            if (!spawner.enableRandomSpawn)
                                spawner.timeToSpawn = EditorGUILayout.Slider("Spawn timer", spawner.timeToSpawn, 0, 10000);
                            else
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField("Min value", spawner.randomTimeToSpawn.x.ToString() + " " + "seconds", EditorStyles.whiteLabel, GUILayout.ExpandWidth(true));
                                EditorGUILayout.LabelField("Max value", spawner.randomTimeToSpawn.y.ToString() + " " + "seconds", EditorStyles.whiteLabel, GUILayout.ExpandWidth(true));
                                EditorGUILayout.MinMaxSlider("Random spawn timer", ref spawner.randomTimeToSpawn.x, ref spawner.randomTimeToSpawn.y, 0, 10000);
                                EditorGUILayout.Space();
                            }

                            EditorGUILayout.Space();
                            spawner.RepeateOptions = (repeatOptions)EditorGUILayout.EnumPopup("Repeat options", spawner.RepeateOptions);
                            switch (spawner.RepeateOptions)
                            {
                                case repeatOptions.noRepeate:
                                    break;
                                case repeatOptions.repeatAfterMainSpawnTime:
                                case repeatOptions.repeateImmediately:
                                    spawner.timesToRepeat = EditorGUILayout.IntSlider("Times to repeat", spawner.timesToRepeat, 0, 1000);
                                    spawner.amountOfObjectsToRespawn = EditorGUILayout.IntSlider("Amount of objects to respawn", spawner.amountOfObjectsToRespawn, 0, 1000);
                                    spawner.randomRepeat = EditorGUILayout.Toggle("Random repeat", spawner.randomRepeat);
                                    if (!spawner.randomRepeat)
                                        spawner.repeatTimer = EditorGUILayout.Slider("Repeat timer", spawner.repeatTimer, 0, 1000);
                                    else
                                    {
                                        EditorGUILayout.Space();
                                        EditorGUILayout.LabelField("Min value", spawner.randomRepeatTimer.x.ToString() + " " + "seconds", EditorStyles.whiteLabel, GUILayout.ExpandWidth(true));
                                        EditorGUILayout.LabelField("Max value", spawner.randomRepeatTimer.y.ToString() + " " + "seconds", EditorStyles.whiteLabel, GUILayout.ExpandWidth(true));
                                        EditorGUILayout.MinMaxSlider("Random repeat timer", ref spawner.randomRepeatTimer.x, ref spawner.randomRepeatTimer.y, 0, 10000);
                                        EditorGUILayout.Space();
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }


                    if (_fadeSpawerAddButton == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(new GUIContent("Add new Spawner", "Add Object Spawner."), EditorStyles.miniButtonLeft))
                        {
                            if (_fadeSpawerAddButton == 1)
                                _fadeSpawerAddButton = 0;
                            else
                                _fadeSpawerAddButton = 1;
                        }
                        if (GUILayout.Button(new GUIContent("Remove this Spawner", "Remove Object Spawner."), EditorStyles.miniButtonRight))
                        {
                            if (worldProp.spawner.Count != 0)
                            {
                                worldProp.spawner.Remove(selectedWorld.properties.spawner[spawnerIndex]);
                                if (worldProp.spawner.Count != 0)
                                {
                                    spawnerIndex = 0;
                                    spawner = worldProp.spawner[spawnerIndex];
                                    tempSpawnerIndex = spawnerIndex;
                                }
                                EditorUtility.DisplayDialog("Removed", "Object Spawner succesfully removed", "OK");
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("No Object Spawners!", "No Spawners to remove", "OK");
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (EditorGUILayout.BeginFadeGroup(_fadeSpawerAddButton))
                    {
                        _fadeSpawerAddButton = 1;
                        EditorGUILayout.BeginHorizontal();
                        _tempSpawnerName = EditorGUILayout.TextField("Spawner Name", _tempSpawnerName);
                        if (GUILayout.Button(new GUIContent("Add", "Add")))
                        {
                            bool exists = false;
                            if (worldProp.spawner.Count != 0)
                            {
                                foreach (string _name in spawnerNames)
                                {
                                    if (_tempSpawnerName == _name)
                                    {
                                        exists = true;
                                        EditorUtility.DisplayDialog("Spawner name already exists!", "Please type new Spawner name!", "OK");
                                    }
                                }
                            }

                            if (!exists)
                            {
                                objToSpawn _spawner = new objToSpawn();
                                _spawner.spawnObjectName = _tempSpawnerName;
                                worldProp.spawner.Add(_spawner);
                                _tempSpawnerName = "";
                                _fadeSpawerAddButton = 0;
                                EditorUtility.DisplayDialog("Done!", "Spawner Added", "OK");
                            }
                        }
                        if (GUILayout.Button(new GUIContent("Cancel", "Cancel")))
                        {
                            _fadeSpawerAddButton = 0;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.EndFadeGroup();
                }
                //
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                //End of scroll area.
                GUILayout.EndScrollView();
                //
            }
        }
        /// <summary>
        /// If integer A is changed, return true.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public bool intHasChanged(int A, int B)
        {
            bool result = false;
            if (B != A)
            {
                A = B;
                result = true;
            }
            return result;
        }
        private void SaveChanges()
        {
            worldProp.surfaceIndexToSpawnPlayer = surfaceIndexToSpawnPlayer;
            selectedWorld.properties.surfaceIndexToSpawnPlayer = worldProp.surfaceIndexToSpawnPlayer;
            if (worldProp.extraSurface.Count != 0)
            {
                string _name = worldProp.extraSurface[surfaceIndex].SurfaceName;
                extraSurface.SurfaceName = _name;

                if (extraSurface._Map)
                    extraSurface._tileMapName = extraSurface._Map.name;
                else
                    extraSurface._tileMapName = null;

                selectedWorld.properties.extraSurface[surfaceIndex] = extraSurface;
            }
            if (worldProp.spawner.Count != 0)
            {

                if (spawner._Map)
                    spawner._tileMapName = spawner._Map.name;
                else
                    spawner._tileMapName = null;

                selectedWorld.properties.spawner[spawnerIndex] = spawner;

            }
            selectedWorld.properties.tileAmount = worldProp.tileAmount;
            selectedWorld.properties.tileSize = worldProp.tileSize;
            selectedWorld.properties.enemyAmount = worldProp.enemyAmount;
            selectedWorld.properties.worldGenerationType = worldProp.worldGenerationType;
            selectedWorld.properties.wallChance = worldProp.wallChance;
            selectedWorld.properties.filled = worldProp.filled;
            selectedWorld.properties.empty = worldProp.empty;
            selectedWorld.properties.repeatCount = worldProp.repeatCount;
            selectedWorld.properties.tileMapSize = worldProp.tileMapSize;
            selectedWorld.properties.wallsHealth = worldProp.wallsHealth;
            selectedWorld.properties.maxActiveEnemies = worldProp.maxActiveEnemies;
            selectedWorld.properties.maxPersuitingEnemies = worldProp.maxPersuitingEnemies;
            selectedWorld.properties.chanceUp = worldProp.chanceUp;                              //chance generator to move up;
            selectedWorld.properties.chanceRight = worldProp.chanceRight;                           //chance generator to move right;
            selectedWorld.properties.chanceDown = worldProp.chanceDown;                            //chance generator to move down;
            selectedWorld.properties.ExtraWallsX = worldProp.ExtraWallsX;                             // Amount of Extra walls to add by x axes;
            selectedWorld.properties.ExtraWallsY = worldProp.ExtraWallsY;                           // Amount of Extra walls to add by y axes;
            selectedWorld.properties.wallTile = worldProp.wallTile;
            selectedWorld.properties.groundTile = worldProp.groundTile;
            selectedWorld.properties.burnedGroundTile = worldProp.burnedGroundTile;
            selectedWorld.properties.surfaceIndexToSpawnPlayer = worldProp.surfaceIndexToSpawnPlayer;
            switch (selectedWorld.properties.worldGenerationType)
            {
                case genType._old:
                    genTypeIndex = 0;
                    break;
                case genType._new:
                    genTypeIndex = 1;
                    break;
                default:
                    break;
            }
            string[] ac = AssetDatabase.FindAssets("scrObject_Worlds");
            string bc = AssetDatabase.GUIDToAssetPath(ac[0]);
            AssetDatabase.ImportAsset(bc);
            ScriptableObject scr = (ScriptableObject)AssetDatabase.LoadAssetAtPath(bc, typeof(ScriptableObject));
            EditorUtility.SetDirty(scr);
            EditorUtility.SetDirty(selectedWorld);
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        private void SynchronizeWorlds(Worlds wo, int index)
        {
            for (int i = 0; i < wo.worlds.Count; i++)
            {
                if (wo.worlds.Count == _worldsNames.Length)
                {
                    if (wo.worlds[i].name == _worldsNames[index])
                    {
                        selectedWorld = wo.worlds[i];
                    }
                }
            }
        }
        private void FillIntList(LevelGeneration levlgen, List<advancedSurface> surfaces)
        {
            levlgen.surfacesData = new List<surfaceMainData>();

            surfaceMainData data1 = new surfaceMainData();
            data1._positions = new List<Vector3Int>();
            data1._positions = customTilePositions(levlgen.wallsMap);
            data1._tileMap = levlgen.wallsMap;
            levlgen.surfacesData.Add(data1);

            surfaceMainData data2 = new surfaceMainData();
            data2._positions = new List<Vector3Int>();
            data2._positions = customTilePositions(levlgen.groundMap);
            data2._tileMap = levlgen.groundMap;
            levlgen.surfacesData.Add(data2);
            if (surfaces.Count != 0)
            {
                foreach (var surface in surfaces)
                {
                    if (surface.enableSurface)
                    {


                    advancedSurface s = new advancedSurface();
                    s = surface;
                    if (!s._Map)
                    {
                        s._Map = GameObject.Find(s._tileMapName).GetComponent<Tilemap>();
                    }
                        surfaceMainData data = new surfaceMainData();
                        data._positions = new List<Vector3Int>();
                        data._positions = customTilePositions(s._Map);
                        data._tileMap = s._Map;
                        //data._tile = surface._Tile;
                        levlgen.surfacesData.Add(data);
                    }
                }
            }
        }
        private List<Vector3Int> customTilePositions(Tilemap map)
        {
            List<Vector3Int> tilePoss = new List<Vector3Int>();
            foreach (var pos in map.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (map.HasTile(localPlace))
                {
                    tilePoss.Add(localPlace);
                }
            }
            return tilePoss;
        }
    }
}