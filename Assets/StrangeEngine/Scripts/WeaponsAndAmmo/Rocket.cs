using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
namespace StrangeEngine
{
    [Serializable]
    public struct pooling
    {
        [Tooltip("puffs. effect when grenade blaw.")]
        public List<GameObject> objects;
        [Tooltip("maximum puffs in pool")]
        public int maxObjects;
        [Tooltip("infinite puffs in pool")]
        public bool extraObjects;
        [Header("DEBUG:")]
        public List<GameObject> ObjectsPool;
    }
    [System.Serializable]
    public struct pTargetSet
    {
        [Tooltip("amount of damage.(only if player is a target.)")]
        public float damage;
        [Tooltip("Players Tag")]
        public string playersTag;
    }
    public enum target { enemies,player}
    [RequireComponent(typeof(Rigidbody2D))]
    /// <summary>
    /// class for Rocket movement, rotation and auto aiming.
    /// </summary>
    public class Rocket : MonoBehaviour
    {
        //
        [Tooltip("Missile Speed")]
        public float MissileSpeed;
        [Tooltip("Time in seconds. destroy gameobject after amount of time.")]
        public float destroyTime;
        public string enemyTag = "Enemy";
        public string wallsTag = "Walls";
        public UnityEvent onRocketFly;
        public UnityEvent onHitEnemy;
        public UnityEvent onHitWall;
        public UnityEvent onHitPlayer;
        public target AIMTarget;
        [Tooltip("if player is a target")]
        public pTargetSet options;
        public List<pooling> ObjectPooling;
        private Rigidbody2D rocketRigidbody;
        private float turn = 2.5f;                  // How fast rocket will turn.
        private float lastTurn = 0f;
        private Transform target;
        private Quaternion newRotation;
        [HideInInspector]
        public GameObject temp;
        //
        void Awake()
        {
            temp = new GameObject("RocketObjects");
            temp.transform.parent = GameManager._gameManager.PoolParent.transform;
            rocketRigidbody = GetComponent<Rigidbody2D>(); // Reference to Rigidbody2D
            foreach (var p in ObjectPooling)
            {
                if (p.objects.Count > 0) GameManager._gameManager.ObjectPooling(p.maxObjects, temp, p.objects, p.ObjectsPool);// fill pool;
            }
        }
        //
        void OnEnable()
        {
            switch (AIMTarget)
            {
                case StrangeEngine.target.enemies:
                    try
                    {
                        target = FindClosestEnemy(0.1f, 100f).transform;
                    }
                    catch (NullReferenceException)
                    {
                        Debug.Log("No Enemies");
                    }
                    break;
                case StrangeEngine.target.player:
                    try
                    {
                        target = GameManager._gameManager.player.transform;
                    }
                    catch (NullReferenceException)
                    {
                        Debug.Log("No Player");
                    }
                    break;
                default:
                    break;
            }

            Invoke("Destroy", destroyTime); // Destroy after amount of seconds.
        }
        private void OnDisable()
        {
            CancelInvoke();
        }
        //
        void FixedUpdate()
        {
            try
            {
                if (target)
                {
                    newRotation = Quaternion.LookRotation(transform.position - target.position, Vector3.forward); // Rocket Rotation
                }
                else
                {
                    newRotation = Quaternion.LookRotation(transform.position, Vector3.up); // Rocket Rotation
                }
                newRotation.x = 0.0f;
                newRotation.y = 0.0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * turn); // new rocket Rotation.
                rocketRigidbody.velocity = transform.up * MissileSpeed;
                onRocketFly.Invoke();
            }
            catch (NullReferenceException)
            {
                Debug.Log("No Enemies");
            }
            if (turn < 40f)
            {
                lastTurn += Time.deltaTime * Time.deltaTime * 50f;
                turn += lastTurn;
            }
        }
        /// <summary>
        /// Finds closest Enemy by Distance.
        /// </summary>
        /// <param name="Minimum Distance"></param>
        /// <param name="Maximum Distance"></param>
        /// <returns>closest enemy</returns>
        public GameObject FindClosestEnemy(float min, float max)
        {
            GameObject[] gos = GameManager._gameManager.Enemies.ToArray(); // Set temporary arrray of enemies. 
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            // Calculate squared distances
            min = min * min;
            max = max * max;
            foreach (GameObject go in gos) // for every enemy in enemies.
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance && curDistance >= min && curDistance <= max)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            if (closest) return closest; else return null;

        }
        // on collision destroy Rocket
        void OnCollisionEnter2D(Collision2D other)
        {
            if (AIMTarget == StrangeEngine.target.player)
            {
                if (other.gameObject.CompareTag(options.playersTag))
                {
                    onHitPlayer.Invoke();
                    other.gameObject.GetComponent<PlayerHealth>().laserDamage(options.damage);
                }
            }

            if (other.gameObject.CompareTag(enemyTag))
            {
                onHitEnemy.Invoke();
                Destroy();
            }
            if (other.gameObject.CompareTag(wallsTag))
            {
                onHitWall.Invoke();
                Destroy();
            }
        }
        void Destroy()
        {
            gameObject.SetActive(false);
        }
        public void _GetEnabledObjectFromPool (int index)
        {
            GameObject obj = GetPooledObject(ObjectPooling[index].extraObjects, temp, ObjectPooling[index].objects, ObjectPooling[index].ObjectsPool);
            obj.transform.position = transform.position;
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
}