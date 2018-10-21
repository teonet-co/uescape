using UnityEngine;
using System.Collections.Generic;
namespace StrangeEngine
{
    /// <summary>
    /// universal class for Destroying and Activating GameObjects.You will use all those functions for Animation events
    /// </summary>
    public class Destroy : MonoBehaviour
    {
        [Tooltip("Particles GameObject")]
        public GameObject Particles;
        [Tooltip("Trigger GameObject")]
        public GameObject Trigger;
        [Tooltip("Money Text GameObject")]
        public GameObject MoneyText;
        [Tooltip("Money to add, if needed")]
        public int Money;
        //
        void CameraShake()
        {
            GameManager._gameManager.mainCamera.GetComponent<CameraShake>().ShakeCamera(0.8f, 0.4f); // Shake Camera (Shake Power, Shake time)
        }
        /// <summary>
        /// Activates Text Prefab.
        /// </summary>
        void ActivateText()
        {
            try
            {
                GameObject T = Instantiate(MoneyText);      // Instantiate MoneyText.
                T.transform.position = transform.position;
                if (GameManager._gameManager.textCanvas)
                    T.transform.SetParent(GameManager._gameManager.textCanvas.transform);                // Parent MoneyText to Canvas.
                else Debug.Log("Please insert text Canvas in Gamemanager textCanvas field.");
                UnityEngine.UI.Text text = T.GetComponent<UnityEngine.UI.Text>(); // Referenc to Text Component.
                text.text = "" + Money;
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No Text Attached!");
            }

        }
        /// <summary>
        /// Destroyes GameObject.
        /// </summary>
        void Dest()
        {
            Destroy(gameObject);
        }
        /// <summary>
        /// Destroyes GameObject Without ScreenShake.
        /// </summary>
        void DestroyWithoutScreenShake()
        {
            Destroy(gameObject);
        }
        public void DisableGameObject()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Instantiates GameObject with Particles.
        /// </summary>
        void activate()
        {
            try
            {
                Instantiate(Particles, transform.position, transform.rotation);
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No Particles Attached!");
            }
        }
        /// <summary>
        /// Instantiates Trigger Prefab.
        /// </summary>
        void activateTrigger()
        {
            try
            {
                Instantiate(Trigger, transform.position, transform.rotation);
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No Trigger Attached!");
            }
        }
        /// <summary>
        /// Get particles parab from pool and sets it active.
        /// </summary>
        public void activateParticles()
        {
        }
        /// <summary>
        /// get disabled bullet.
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
        /// <summary>
        /// counts active objects in list.
        /// </summary>
        /// <param name="objToCount"></param>
        /// <returns></returns>
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