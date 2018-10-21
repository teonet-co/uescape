using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
/// <summary>
/// Enemy shooting class.
/// </summary>
namespace StrangeEngine
{
    public class EnemyShootingNew : MonoBehaviour
    {
        //
        [Header("Main")]
        [Tooltip("Bullet Start position")]
        public Vector3 bulletOffset = new Vector3(0, 0.5f, 0);
        [Tooltip("Enemy Bullet Prefab")]
        public GameObject bulletPrefab;
        [Tooltip("How fast enemy will shoot")]
        public float fireDelay = 0.50f;
        [Tooltip("Distance beetween enemy and player, if distance lower then this variable then enemy will Start Shooting.")]
        public float FireEnableDistance;
        float cooldownTimer = 0; // cooldownTimer = fireDelay.
        Transform player; // players transform.
        [Header("Events")]
        public UnityEvent onEnemyShoot;
        [Header("ObjectPooling")]
        public int maxBulletAmount;
        public bool expand;
        private GameObject EnemyBullets;
        private AI_movement AIMove;
        [Header("DEBUG:")]
        public List<GameObject> bullets;


        //
        void Start()
        {
            try
            {
                player = GameManager._player.transform; // Reference to 'Player' transform.
            }
            catch (NullReferenceException)
            {
                Debug.Log("No Player");
            }
            // Object Pooling;
            EnemyBullets = new GameObject("Enemy bullets"); // create parent GameObject;
            EnemyBullets.transform.parent = GameManager._gameManager.PoolParent.transform;
            // Instantiate bullets and add them to bullets list
            for (int i = 0; i < maxBulletAmount; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.transform.parent = EnemyBullets.transform;
                bullet.SetActive(false);
                bullets.Add(bullet);
            }
            AIMove = GetComponentInParent<AI_movement>();
        }
        //
        void Update()
        {
            cooldownTimer -= Time.deltaTime; // start timer;
            if (cooldownTimer <= 0 && player != null && Vector3.Distance(transform.position, player.position) < FireEnableDistance && AIMove._modes==modes.activeMode)
            { // Start shooting if player near Enemy and active.            
                cooldownTimer = fireDelay;
                Shoot();
            }
        }
        /// <summary>
        /// gets non active bullet from list and if there no disabled bullets adds new one."
        /// </summary>
        /// <returns>bullet</returns>
        public GameObject getBullet()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].activeInHierarchy)
                {
                    return bullets[i];
                }
            }
            if (expand)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.transform.parent = EnemyBullets.transform;
                bullets.Add(bullet);
                return bullet;
            }
            return null;
        }

        public void Shoot()
        {
            GameObject bullet = getBullet();
            if (bullet == null)
            {
                return;
            }
            Vector3 offset = transform.rotation * bulletOffset; // offset of start bullet position
            bullet.transform.SetPositionAndRotation(transform.position + offset, transform.rotation);
            bullet.SetActive(true);
            onEnemyShoot.Invoke();
        }
    }
}