using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
namespace StrangeEngine
{
    [RequireComponent(typeof(SpriteRenderer))]
    /// <summary>
    /// class for instantiating Electric Field Grenade.
    /// </summary>
    public class ElectricFieldGrenade : MonoBehaviour
    {
        [Header("Object Pooling Settings.")]
        [Space]
        [Tooltip("Boom Prefabs.")]
        public List<GameObject> booms;
        [Tooltip("instantiate infinite amount of booms if needed.")]
        public bool extraBoom;
        [Tooltip("maximum amount of booms.")]
        public int maxBooms;
        [Header("Events")]
        public UnityEvent onGrenadeBlow;
        [Header("FOR DEBUG :")]
        public List<GameObject> boomPool;
        private GameObject t;
        private SpriteRenderer sr;
        //
        private void Start()
        {
            t = new GameObject("ElectricFieldGrenade");
            GameManager._gameManager.ObjectPooling(maxBooms, t, booms, boomPool);
            sr = gameObject.GetComponent<SpriteRenderer>(); // reference to SpriteRenderer.
        }
        void OnEnable()
        {
            if (sr)
                sr.enabled = true; // Enable SpriteRenderer
            transform.GetChild(2).gameObject.SetActive(true);
            Invoke("Explode", 3);               // Explode after 3 seconds.
        }
        //
        void Explode()
        {
            if (sr)
                sr.enabled = false; // Disable SpriteRenderer
            try
            {
                if (booms.Count > 0)
                {
                    GameObject b = GetPooledObject(extraBoom, t, booms, boomPool);
                    b.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    b.SetActive(true);
                    onGrenadeBlow.Invoke();
                }
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No Boom GameObject Attached!");
            }
            gameObject.SetActive(false);
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
    }
}