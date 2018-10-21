using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
namespace StrangeEngine
{
    /// <summary>
    /// Use this class to generate new map in Editor;
    /// </summary>
    [ExecuteInEditMode]
    public class GenerateLevel : MonoBehaviour
    {
        /// <summary>
        /// Generates map in editor;
        /// </summary>
        public void GenerateLevelInEditMode()
        {
            LevelGeneration levelGen = FindObjectOfType<LevelGeneration>();
            levelGen.canGenerateInEditMode = true;
            levelGen.StartGenerating();
        }
        /// <summary>
        /// Clears all temp map variables;
        /// </summary>
        public void ClearMap()
        {
            StopAllCoroutines();
            HealthScore[] _enemies = new HealthScore[0];
            _enemies = FindObjectsOfType<HealthScore>();
            if (_enemies.Length > 0)
            {
                foreach (var enemy in _enemies)
                {
                    DestroyImmediate(enemy.gameObject);
                }
            }
            _enemies = new HealthScore[0];
            GameObject obj1 = GameObject.Find("LevelParent");
            GameObject obj2 = GameObject.Find("Spawned Objects");
            if(obj1)
            DestroyImmediate(obj1);
            if(obj2)
            DestroyImmediate(obj2);
            LevelGeneration _levelGenerator = FindObjectOfType<LevelGeneration>();
            _levelGenerator.groundMap.ClearAllTiles();
            _levelGenerator.wallsMap.ClearAllTiles();
           // if (_levelGenerator.curentMap)
            //{
                if (_levelGenerator.MapProperties.extraSurface.Count != 0)
                {
                    _levelGenerator.ClearSurfaces();
                }
                _levelGenerator.ClearValues();
           // }

        }
    }
}