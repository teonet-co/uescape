using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
namespace StrangeEngine
{
    public enum M_ShootTrigger
    {
        Joystick,
        Button
    }
    public class GameManager : MonoBehaviour
    {
        // you can use thos non static variables from any place you want, just type ' GameManager._gameManager.neededVariable '
        [Header("Main Settings.")]
        [Space]
        [HideInInspector]
        public Vector3 pooledObjectTempPosition;
        [Tooltip("Mobile Controls ON/OFF")]
        public bool mobile;
        public GameObject mainCamera;
        public GameObject textCanvas;
        [Tooltip("Choose button to shoot.")]
        public ShootButtons shootOptions;
        [Tooltip("Mobile Joystics GameObject.")]
        public GameObject joystics;
        [Tooltip("Player GameObject")]
        public GameObject player;
        public LevelGeneration levelGeneration;
        public WeaponSelection weaponSelection;
        [Tooltip("Weapon damage list. Each variable in this list compares to each weapon.")]
        public List<int> WeaponsDamage;
        [Tooltip("Maximum amount of enemies in pursuit mode at one time.")]
        [Range(1, 50)]
        public int maxPersuitingEnemies;
        [Tooltip("Maximum amount of enemies in active mode at one time.")]
        [Range(1, 50)]
        public int maxActiveEnemies;
        [Header("Object Pooling for Walls.(Place here objects you want to pool, wall destroy.)")]
        public List<Pool> objectPooling;
        [HideInInspector]
        public GameObject PoolParent; //Parent GameObject for object pooling
        
        [Space]
        [Space]
        [Header("DEBUG :")]
        [Tooltip("Temporary list of enemies")]
        public List<GameObject> Enemies;
        public List<Transform> enemiesPositions;
        [Tooltip("Temporary list of enemies in pursuit mode.")]
        public List<GameObject> pursuitingEnemies;
        [Tooltip("Temporary list of enemies in active mode.")]
        public List<GameObject> activeEnemies;
        
        // you can use this static variables from any place you want, just type GameManager.neededVariable; 
        public static bool Mobile; // use this to switch mobile or pc from other scripts.
        public static GameObject _player; // use this in other scripts.
        public static RectTransform fRt;// use this in other scripts.
        public static GameManager _gameManager;// use this in other scripts.
        [HideInInspector]
        public GameObject SplashesAndBlows;
        private Vector3 vec;
        private Quaternion qot;
        //
        void Awake()
        {
            if (_gameManager != null)
                GameObject.Destroy(_gameManager);
            else
                _gameManager = this;
            Mobile = mobile;
            if (PlayerPrefs.HasKey("mobile"))
            {
                Mobile = true;
                mobile = true;
            }
            else
            {
                Mobile = mobile;
            }
            fRt = shootOptions.FireJoysticksRectTransform;
            _player = player;
            PoolParent = new GameObject("Object Pooling");
            SplashesAndBlows = new GameObject("Splashes And Blows");
            if (mobile)
            {
                Debug.Log("mobile controls are ON!");
                joystics.SetActive(true);

            }
            else
            {
                Debug.Log("mobile controls are OFF!");
                joystics.SetActive(false);
            }
            for (int i = 0; i < objectPooling.Count; i++)
            {
                ObjectPooling(objectPooling[i].maxPooledObject, objectPooling[i].parent, objectPooling[i].objectsToPool, objectPooling[i].objectsPool);
            }
           
        }
        /// <summary>
        /// public function for Object pool.
        /// </summary>
        /// <param name="maxPooledObjects"></param>
        /// <param name="parentObject"></param>
        /// <param name="ObjectsToPool"></param>
        /// <param name="ObjectPool"></param>
        public void ObjectPooling(int maxPooledObjects, GameObject parentObject, List<GameObject> ObjectsToPool, List<GameObject> ObjectPool)
        {
            // Object Pooling;
            // Instantiate ObjectsToPool and add them to ObjectPool list
            for (int i = 0; i < maxPooledObjects; i++)
            {
                GameObject pObj = Instantiate(ObjectsToPool[UnityEngine.Random.Range(0, ObjectsToPool.Count)]);
                if (parentObject)
                    pObj.transform.parent = parentObject.transform;
                else
                    pObj.transform.SetParent(PoolParent.transform);
                pObj.SetActive(true);
                pObj.SetActive(false);
                ObjectPool.Add(pObj);
            }
        }
        public void GetObjectFromPool(int index)
        {
            GameObject obj = GetPooledObject(objectPooling[index].expandPool, objectPooling[index].parent, objectPooling[index].objectsToPool, objectPooling[index].objectsPool);
            obj.transform.SetPositionAndRotation(pooledObjectTempPosition, Quaternion.identity);
            obj.SetActive(true);
        }
        /// <summary>
        /// get disabled object from pool.
        /// </summary>
        public GameObject GetPooledObject(bool extraPooledObjects, GameObject parentObject, List<GameObject> ObjectsToPool, List<GameObject> ObjectPool)
        {
            List<GameObject> w = ObjectPool;
            for (int i = 0; i < w.Count; i++)
            {
                if (!w[i].activeInHierarchy)
                {
                    return w[i];
                }
            }
            if (extraPooledObjects)
            {
                GameObject wal = Instantiate(ObjectsToPool[UnityEngine.Random.Range(0, ObjectsToPool.Count)]);
                if (parentObject)
                    wal.transform.parent = parentObject.transform;
                else
                    wal.transform.parent = GameManager._gameManager.PoolParent.transform;
                w.Add(wal);
                return wal;
            }
            return null;
        }
    }
    [System.Serializable]
    public struct Pool
    {
        public string poolName;
        public List<GameObject> objectsToPool;
        public int maxPooledObject;
        public bool expandPool;
        public GameObject parent;
        public List<GameObject> objectsPool;
    }
    public enum ShootMode { burst, single }
    [System.Serializable]
    public struct ShootButtons
    {
        [Header("PC")]
        public KeyCode key;
        [Header("Mobile")]
        public M_ShootTrigger shootTrigger;
        [Tooltip("Mobile Joystick, you want to shoot and aim.(MobileJoystick2 by default.)")]
        public RectTransform FireJoysticksRectTransform;
        public Button shootButton;
    }
}
