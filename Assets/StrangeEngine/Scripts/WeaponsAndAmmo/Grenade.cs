using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
/// <summary>
/// Class for Grenades settings.
/// </summary>
namespace StrangeEngine
{
    public class Grenade : MonoBehaviour
    {
        [Header("Main Settings.")]
        [Space]
        public string wallTag = "Walls";
        public string enemyTag = "Enemy";
        [Tooltip("Actual Damage you want Send to Enemy")]
        public int Damage;
        [Tooltip("Actual Damage you want Send to Walls. If 0, it will not explode on collision with walls.")]
        public int DamageWalls;
        [Tooltip("If this Grenade is Main it's going to be true, if a child it must be false.")]
        public bool MainGrenade;
        [Tooltip("Speed of GrenadeChild Prefab")]
        public float speed;
        [Tooltip("grenade particles means particles component attached to child of this gameObject.")]
        public bool enableGrenadeParticles;
        [Tooltip("If this Grenade is Main it's going to activate this amount of children , if there will be no collisions.")]
        public int amountOfChildren;
        [Tooltip("If this Grenade is Main it's going to activate this amount of children , if there will be collision with enemy.")]
        public int amountOfChildrenOnEnemies;
        [Tooltip("If this Grenade is Main it's going to activate this amount of children , if there will be collision with wall.")]
        public int amountOfChildrenOnWalls;
        [Header("Events")]
        public UnityEvent onMainGrenadeExplode;
        public UnityEvent onChildGrenadeExplode;
        public UnityEvent onChildGrenadeLaunch;
        [Space]
        public target AIMTarget;
        [Tooltip("if player is a target")]
        public pTargetSet options;
        //
        private LevelGeneration levelGen;
        [Tooltip("Wall Layer(for wall collisions)")]
        public LayerMask layers;
        private CircleCollider2D coll;
        public GrenadeLauncher grenadeLauncher;
        //    
        private void Awake()
        {
            if (MainGrenade)
            {
                Invoke("FireGrenade", 2);
                Invoke("D", 3);
            }
            if (!MainGrenade)
            {
                Invoke("D", 1);
            }
            levelGen = GameManager._gameManager.levelGeneration;
            coll = GetComponent<CircleCollider2D>();
        }
        private void OnEnable()
        {
            if (enableGrenadeParticles)
            {
                transform.GetChild(1).gameObject.SetActive(true);
            }
            if (MainGrenade)
            {
                Invoke("FireGrenade", 2);
                Invoke("D", 3);
            }
            if (!MainGrenade)
            {
                Invoke("D", 1);
            }
        }
        private void OnDisable()
        {
            CancelInvoke();
        }
        //
        void OnTriggerEnter2D(Collider2D other)
        {
            switch (AIMTarget)
            {
                case target.enemies:
                    if (other.CompareTag(enemyTag))
                    {
                        other.GetComponent<HealthScore>().laserDamage(Damage);
                        if (MainGrenade)
                        {
                            FireGrenadeWithParameter(amountOfChildrenOnEnemies);
                        }
                        Invoke("D", 0.2f);
                    }
                    break;
                case target.player:
                    if (other.CompareTag(options.playersTag))
                    {
                        other.GetComponent<PlayerHealth>().laserDamage(options.damage);
                        if (MainGrenade)
                        {
                            FireGrenadeWithParameter(amountOfChildrenOnEnemies);
                        }
                        Invoke("D", 0.2f);
                    }
                    break;
                default:
                    break;
            }

            if (other.CompareTag(wallTag) && DamageWalls != 0)
            {
                RaycastHit2D[] hitArray = Physics2D.CircleCastAll(transform.position, coll.radius, Vector2.zero, layers);
                Vector3 hitPosition = Vector3.zero;
                foreach (var hit in hitArray)
                {
                    hitPosition.x = hit.point.x - 10f * hit.normal.x;
                    hitPosition.y = hit.point.y - 10f * hit.normal.y;
                    levelGen.Destruct(levelGen.wallsMap.WorldToCell(hitPosition), DamageWalls);
                }
                if (MainGrenade)
                {
                    FireGrenadeWithParameter(amountOfChildrenOnWalls);
                }
                Invoke("D", 0.2f);
            }
        }
        /// <summary>
        /// Takes Explosion effect from pool. Disables this GameObject.
        /// </summary>
        void D()
        {
            if (MainGrenade) onMainGrenadeExplode.Invoke();
            else onChildGrenadeExplode.Invoke();
            try
            {
                GameObject expl = grenadeLauncher.GetPooledObject(grenadeLauncher.extraChildExplosions, grenadeLauncher.temp, grenadeLauncher.ChildExplosions, grenadeLauncher.ChildExplosionsPool);
                if (expl)
                {
                    expl.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    expl.SetActive(true);
                }
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No Explosion Prefab Attached!");
            }
            gameObject.SetActive(false);
        }
        /// <summary>
        //Take grenades form pool and add them force.
        /// </summary>
        public void FireGrenade()
        {
            try
            {
                for (int i = 0; i < amountOfChildren; i++)
                {

                    GameObject grena = grenadeLauncher.GetPooledObject(grenadeLauncher.ExtraChildGrenades, grenadeLauncher.temp, grenadeLauncher.GrenadeChildPrefabs, grenadeLauncher.GrenadeChildPool);
                    if (grena == null) return;
                    grena.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    grena.SetActive(true);
                    Rigidbody2D grenaRig = grena.GetComponent<Rigidbody2D>();
                    onChildGrenadeLaunch.Invoke();
                    grenaRig.AddForce(new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * speed, ForceMode2D.Impulse);
                }
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No Grenade Prefab Attached!");
            }
        }
        /// <summary>
        ///Take amount of grenades form pool and add them force.
        /// </summary>
        public void FireGrenadeWithParameter(int amount)
        {
            try
            {
                for (int i = 0; i < amount; i++)
                {
                    GameObject grena = grenadeLauncher.GetPooledObject(grenadeLauncher.ExtraChildGrenades, grenadeLauncher.temp, grenadeLauncher.GrenadeChildPrefabs, grenadeLauncher.GrenadeChildPool);
                    if (grena == null) return;
                    grena.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    grena.SetActive(true);
                    Rigidbody2D grenaRig = grena.GetComponent<Rigidbody2D>();
                    onChildGrenadeLaunch.Invoke();
                    grenaRig.AddForce(new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * speed, ForceMode2D.Impulse);
                }
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No Grenade Prefab Attached!");
            }
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
                wal.transform.parent = parentObject.transform;
                w.Add(wal);
                return wal;
            }
            return null;
        }
        private int CountActive(List<GameObject> objToCount)
        {
            int amountActiveObj = 0;
            for (int i = 0; i < objToCount.Count; i++)
            {
                if (objToCount[i].activeSelf == true)
                {
                    amountActiveObj++;
                }
            }
            return amountActiveObj;
        }
    }
}