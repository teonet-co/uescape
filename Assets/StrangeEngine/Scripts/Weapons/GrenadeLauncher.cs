using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
namespace StrangeEngine
{
    /// <summary>
    /// Class for shooting Grenades.
    /// </summary>
    public class GrenadeLauncher : MonoBehaviour
    {
        [Tooltip("Player GameObject")]
        private GameObject player;
        [Header("Main Settings.")]
        [Space]
        [Tooltip("You can easely change Grenade offset here.")]
        public Vector3 GrenadeOffset = new Vector3(0, 0.5f, 0);
        [Tooltip("FireRate.")]
        public float fireRate = 0.5F;
        [Tooltip("if true, on every shot will be recoil")]
        public bool Recoil;
        [Tooltip("recoil on x and y axes")]
        public Vector2 recoilVector;
        [Space(10)]
        private float nextFire = 0.0F;
        [Tooltip("Force value aplied to grenade.")]
        public float force;
        [Space(10)]
        [Tooltip("if true, activates 'poof' prefab when shooting.")]
        public bool Poof = true;
        [Header("Camera Shake Settings.")]
        [Tooltip("Camera Shake parameters. Where x - is Shake power , y - Shake duration")]
        public ShakeType cameraShakeType;
        public oldShakeOptions simpleShakeSettings;
        public CameraShake.Properties sineShakeSettings;
        [Header("Events")]
        public UnityEvent onPlayerShoot;
        [Header("Object Pooling Settings.")]
        [Tooltip("Grenade Prefabs")]
        public List<GameObject> GrenadePrefabs;
        [Tooltip("maximum Grenade Prefabs in pool")]
        public int maxGrenades;
        [Tooltip("infinite grenades in pool")]
        public bool ExtraGrenades;
        [Space(10)]
        [Tooltip("Grenade Prefabs")]
        public List<GameObject> GrenadeChildPrefabs;
        [Tooltip("maximum Grenade Prefabs in pool")]
        public int maxChildGrenades;
        [Tooltip("infinite grenades in pool")]
        public bool ExtraChildGrenades;
        [Space(10)]
        [Tooltip("puffs. effect when grenade blaw.")]
        public List<GameObject> puffs;
        [Tooltip("maximum puffs in pool")]
        public int maxpuffs;
        [Tooltip("infinite puffs in pool")]
        public bool extraPuff;
        [Space(10)]
        [Tooltip("explosion prefabs")]
        public List<GameObject> ChildExplosions;
        [Tooltip("maximum explosion prefabs on the screen")]
        public int maxChildExpllosions;
        [Tooltip("infinite explosion prefabs on the screen")]
        public bool extraChildExplosions;
        [Space(10)]
        [Header("Ammo Settings.")]
        public bool enableAmmo = true;
        [Tooltip("Just for debug.")]
        public int Ammo;
        [Tooltip("Ammo text GameObject.")]
        public GameObject AmmoText;
        [Space(10)]
        //    
        [Header("FOR DEBUG :")]
        [Tooltip("if true, weapon will start shooting.")]
        public bool shot = false;
        public List<GameObject> GrenadePool;
        public List<GameObject> GrenadeChildPool;
        public List<GameObject> PuffPool;
        public List<GameObject> ChildExplosionsPool;
        [HideInInspector]
        public GameObject temp;
        private bool mobile;
        private RectTransform rt;
        private Ammo ammo;
        private weaponHolder holder;
        private ShootMode shootMode;
        private CameraShake camShake;
        private ShootButton m_Button;
        private M_ShootTrigger trigger;
        //
        private void Awake()
        {
            temp = new GameObject("Grenades");
            if(enableAmmo)
            ammo = AmmoText.GetComponent<Ammo>();
            holder = GetComponentInParent<weaponHolder>();
            camShake = GameManager._gameManager.mainCamera.GetComponent<CameraShake>();
        }
        //
        private void Start()
        {// check runtime platform
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                mobile = true;
            }
            else if (GameManager.Mobile)
            {
                mobile = true;
            }
            else
            {
                mobile = false;
            }
            trigger = GameManager._gameManager.shootOptions.shootTrigger;
            rt = GameManager.fRt;
            m_Button = GameManager._gameManager.shootOptions.shootButton.GetComponent<ShootButton>();
            player = GameManager._player;
            temp.transform.parent = GameManager._gameManager.PoolParent.transform;
            if (GrenadeChildPrefabs.Count > 0) GameManager._gameManager.ObjectPooling(maxChildGrenades, temp, GrenadeChildPrefabs, GrenadeChildPool); // fill grenades pool;
            foreach (var g in GrenadeChildPool)
            {
                Grenade gren = g.GetComponent<Grenade>();
                if (gren) gren.grenadeLauncher = this;
            }
            if (GrenadePrefabs.Count > 0) GameManager._gameManager.ObjectPooling(maxGrenades, temp, GrenadePrefabs, GrenadePool); // fill grenades pool;
            foreach (var g in GrenadePool)
            {
                Grenade gren = g.GetComponent<Grenade>();
                if (gren) gren.grenadeLauncher = this;
            }
            if (puffs.Count > 0) GameManager._gameManager.ObjectPooling(maxpuffs, temp, puffs, PuffPool);// fill puffs pool;
            if (ChildExplosions.Count > 0) GameManager._gameManager.ObjectPooling(maxChildExpllosions, temp, ChildExplosions, ChildExplosionsPool);// fill puffs pool;
        }
        //
        void Update()
        {
            if (!mobile)
            {
                if (holder)
                {
                    shootMode = holder.shootMode;
                    switch (shootMode)
                    {
                        case ShootMode.burst:
                            if (Input.GetKey(GameManager._gameManager.shootOptions.key))
                            {
                                Shoot();
                            }
                            break;
                        case ShootMode.single:
                            if (Input.GetKeyDown(GameManager._gameManager.shootOptions.key))
                            {
                                Shoot();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            if (shot)
            {
                Shoot();                                    // if bool shot is true, start shooting
            }
            if (enableAmmo)
                Ammo = ammo.ammo;   // Get actual Ammo from AmmoText GameObject.
            else Ammo = int.MaxValue;
        }
        //
        void FixedUpdate()
        {
            if (mobile)
            {
                if (holder)
                {
                    switch (trigger)
                    {
                        case M_ShootTrigger.Joystick:
                            if (rt)
                            {
                                if (rt.anchoredPosition.x != 0 && rt.anchoredPosition.y != 0)
                                {
                                    shot = true;
                                }
                                else
                                {
                                    shot = false;
                                }
                            }
                            break;
                        case M_ShootTrigger.Button:

                            if (m_Button != null)
                            {
                                if (m_Button.buttonDown)
                                {
                                    shot = true;
                                }
                                else
                                {
                                    shot = false;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Start Shooting!!
        /// </summary>
        public void Shoot()
        {
            if (Time.time > nextFire && Ammo > 0)
            {
                onPlayerShoot.Invoke();
                nextFire = Time.time + fireRate;            // add time for next fire
                if(enableAmmo)
                ammo.MinusAmmo();          // Send Message to AmmoText GameObject that we spent one bullet
                try
                {
                    switch (cameraShakeType)
                    {
                        case ShakeType.none: break;
                        case ShakeType.simpleShake:
                            camShake.ShakeCamera(simpleShakeSettings.shakePower, simpleShakeSettings.shakeDuration); break;
                        case ShakeType.sineShake:
                            camShake.StartShake(sineShakeSettings); break;
                        default: break;
                    }
                }
                catch (NullReferenceException)
                {
                    Debug.Log("There is no Active Camera in the Scene.");
                }
                Vector3 offset1 = transform.rotation * GrenadeOffset;// Vector3 Bullet Offset
                Vector2 tt = transform.up;                  // up direction.
                if (Recoil)
                {
                    // a little bit of recoil.
                    Vector3 recoilVect = new Vector3(GetComponentInParent<Transform>().rotation.x + recoilVector.x, GetComponentInParent<Transform>().rotation.y + recoilVector.y);
                    player.transform.position = new Vector3(player.transform.position.x - recoilVect.x, player.transform.position.y - recoilVect.y, player.transform.position.z);
                }
                //Shoot!!
                try
                {// get grenade from pool and activate it and add force to it;
                    GameObject grenade = GetPooledObject(ExtraGrenades, temp, GrenadePrefabs, GrenadePool);
                    grenade.transform.SetPositionAndRotation(transform.position + offset1, transform.rotation);
                    grenade.SetActive(true);
                    grenade.GetComponent<Rigidbody2D>().AddForce(tt * force, ForceMode2D.Impulse); // Add Force to Grenade.
                }
                catch (UnassignedReferenceException)
                {
                    Debug.Log("No Grenade Prefab Attached!");
                }
                if (Poof)
                {// get poof and activate it;
                    GameObject puf = GetPooledObject(extraPuff, temp, puffs, PuffPool);
                    puf.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    puf.SetActive(true);
                }
            }
        }
        /// <summary>
        /// Get disabled object from pool.
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
                Grenade gren = wal.GetComponent<Grenade>();
                if (gren) gren.grenadeLauncher = this;
                wal.transform.parent = parentObject.transform;
                w.Add(wal);
                return wal;
            }
            return null;
        }
        /// <summary>
        ///  public function to shoot with delay.
        /// </summary>
        /// <param name="delay"></param>
        public void ButtonDownWithDelay(float delay)
        {
            Invoke("buttondown", delay);
        }
        /// <summary>
        /// public function to start shooting.
        /// </summary>
        public void buttondown()
        {
            shot = true;
        }
        /// <summary>
        /// public function to stop shooting.
        /// </summary>
        public void buttonup()
        {
            shot = false;
            CancelInvoke();
        }
    }
}