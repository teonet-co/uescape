using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
namespace StrangeEngine
{
    //
    public class LaserLauncher : MonoBehaviour
    {
        [Header("Main Settings.")]
        public target _target;
        [Tooltip("enemy and walls layers.")]
        public LayerMask layers;
        [Tooltip("how fast laser will get wider.")]
        public float widthSpeed;
        [Tooltip("how fast line will be drawn.")]
        public float lineDrawSpeed;
        [Tooltip("start width of linerender.")]
        public float startWidth; // 
        [Tooltip("Damage")]
        public int damage;
        public string enemyTag = "Enemy";
        public string playerTag = "Player";
        [Tooltip("FireRate.")]
        public float fireRate = 0.5F;
        [Header("Camera Shake Settings.")]
        [Tooltip("Camera Shake parameters. Where x - is Shake power , y - Shake duration")]
        public ShakeType cameraShakeType;
        public oldShakeOptions simpleShakeSettings;
        public CameraShake.Properties sineShakeSettings;
        [Header("Events")]
        public UnityEvent onPlayerShoot;
        [Header("Object Pooling.")]
        [Tooltip("laser prefab.")]
        public GameObject laser;
        [Tooltip("Maximum lasers you want to instantiate in start and add them to lasers pool.")]
        public int maxLasersAmount;
        [Tooltip("infinite lasers in pool.")]
        public bool expandLasers = true;
        [Space(10)]
        [Tooltip("if true, activates 'poof' prefab when shooting. ")]
        public bool Poof = true;
        [Tooltip("Puff Prefab.")]
        public GameObject Puff;
        [Tooltip("Maximum puffs you want to instantiate in start and add them to puffs pool.")]
        public int maxPuffAmount;
        [Tooltip("infinite puffs in pool.")]
        public bool expandPuff = true;
        [Space(10)]
        [Tooltip("if true, activates 'Effect' prefab when shooting. ")]
        public bool ImpactEffect = true;
        [Tooltip("Explosion Prefab.")]
        public GameObject Effect;
        [Tooltip("Maximum effects you want to instantiate in start and add them to effects pool.")]
        public int maxImpactAmount;
        [Tooltip("infinite effects in pool.")]
        public bool expandEffect = true;
        [Space(10)]
        [Header("Ammo Settings.")]
        public bool enableAmmo;
        [Tooltip("Current Ammo.")]
        public int Ammo;
        [Tooltip("Ammo text GameObject.")]
        public GameObject AmmoText;
        public bool Shot = false;
        //
        [Header("DEBUG:")]
        [Tooltip("Only for debug. to see how many lasers in pool")]
        public List<GameObject> lasersPool;
        [Tooltip("Only for debug. to see how many puffs in pool")]
        public List<GameObject> Puffs;
        [Tooltip("Only for debug. to see how many Impact Effects in pool")]
        public List<GameObject> ImpactEffects;
        //
        private GameObject PlayerLasers; // lasers parent.
        private GameObject PlayerPuffs;// puffs parent.
        private GameObject PlayerImpactEffect; //Impact effects parent.
        private float nextFire = 0.0F;
        private GameObject g; // instantiated laser prefab.
        private LineRenderer l; // prefabs linerenderer
        private float dist; // distance.
        private Vector3 endPosition;//
        private RaycastHit2D hit;//
        private bool mobile;//
        private RectTransform rt;// RectTransform.
        private Ammo _ammo;
        private weaponHolder holder;
        private ShootMode shootMode;
        private CameraShake camShake;
        private GameObject hitObj;
        private ShootButton m_Button;
        private M_ShootTrigger trigger;
        //
        private void Start()
        {
            hitObj = new GameObject("tempLaser");
            hitObj.AddComponent<BoxCollider2D>().isTrigger = true;

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
            if (enableAmmo)
                _ammo = AmmoText.GetComponent<Ammo>();
            holder = GetComponentInParent<weaponHolder>();
            trigger = GameManager._gameManager.shootOptions.shootTrigger;
            rt = GameManager.fRt;
            m_Button = GameManager._gameManager.shootOptions.shootButton.GetComponent<ShootButton>();// Instance to RectTransform selected in GameManager,
            PlayerLasers = new GameObject("Player Lasers"); // create parent GameObject;
            PlayerLasers.transform.parent = GameManager._gameManager.PoolParent.transform;
            // Instantiate Lasers and add them to lasers list
            for (int i = 0; i < maxLasersAmount; i++)
            {
                GameObject las = Instantiate(laser);
                las.transform.parent = PlayerLasers.transform;
                las.SetActive(false);
                lasersPool.Add(las);
            }
            if (Poof)
            {
                PlayerPuffs = new GameObject("Player puffs"); // create parent GameObject;
                for (int i = 0; i < maxPuffAmount; i++) // Instantiate Pufs and add them to puffs list
                {
                    GameObject puf = Instantiate(Puff);
                    puf.transform.parent = PlayerPuffs.transform;
                    puf.SetActive(true);

                    puf.SetActive(false);
                    Puffs.Add(puf);
                }
            }
            if (ImpactEffect)
            {
                PlayerImpactEffect = new GameObject("Player Impact Effect"); // create parent GameObject;
                for (int i = 0; i < maxImpactAmount; i++) // Instantiate Impact Effects and add them to effects list
                {
                    GameObject eff = Instantiate(Effect);
                    eff.transform.parent = PlayerImpactEffect.transform;
                    eff.SetActive(true);
                    eff.SetActive(false);
                    ImpactEffects.Add(eff);
                }
            }
            camShake = GameManager._gameManager.mainCamera.GetComponent<CameraShake>();
        }
        // Update is called once per frame
        void Update()
        {
            if (enableAmmo)
            {
                Ammo = _ammo.ammo;  // Get actual Ammo from AmmoText GameObject.  
            }
            else
            {
                Ammo = int.MaxValue; // set ammo to max value.
            }

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
                                    Shoot();
                                }
                            }
                            break;
                        case M_ShootTrigger.Button:

                            if (m_Button != null)
                            {
                                if (m_Button.buttonDown)
                                {
                                    Shot = true;
                                }
                                else
                                {
                                    Shot = false;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
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
            if (Shot)
                Shoot();
        }
        public void Shoot()
        {
            if (Time.time > nextFire && Ammo > 0)
            {
                nextFire = Time.time + fireRate;            // add time for next fire
                                                            //readyToShoot = true;
                if (enableAmmo)
                {
                    _ammo.MinusAmmo();          // Send Message to AmmoText GameObject that we spent one bullet
                }
                hit = Physics2D.Raycast(transform.position, transform.up, 1000, layers);

                onPlayerShoot.Invoke();

                if (hit.collider != null)
                {
                    switch (_target)
                    {
                        case target.enemies:
                            if (hit.collider.CompareTag(enemyTag))
                            {
                                hit.collider.GetComponent<HealthScore>().laserDamage(damage);
                            }
                            break;
                        case target.player:
                            if (hit.collider.CompareTag(playerTag))
                            {
                                hit.collider.GetComponent<PlayerHealth>().laserDamage(damage);
                            }
                            break;
                        default:
                            break;
                    }

                    endPosition = hit.point - hit.normal;
                    hitObj.transform.position = endPosition;
                }
                if (hit.collider == null)
                {
                    endPosition = Vector3.up;
                }
                g = getLaser(); /// instantiate prefab.
                if (g == null)
                {
                    return;
                }
                g.transform.SetPositionAndRotation(transform.position, transform.rotation);

                if (Poof)                                   // if Poof is true,Activate Puff prefab
                {
                    GameObject poff = getPuff(); // get shell from pool;
                    if (poff == null)
                    {
                        return;
                    }
                    poff.transform.SetPositionAndRotation(g.transform.position, g.transform.rotation);
                    poff.SetActive(true);
                }
                if (ImpactEffect)                                   // if Poof is true,Activate Puff prefab
                {
                    GameObject imp = getImpactEffect(); // get shell from pool;
                    if (imp == null)
                    {
                        return;
                    }
                    imp.transform.SetPositionAndRotation(endPosition, Quaternion.identity);
                    imp.SetActive(true);
                }
                Laser lase = g.GetComponent<Laser>();
                lase.startposition = g.transform;
                lase.startWidth = startWidth;
                
                lase.hitCollider = hitObj.GetComponent<Collider2D>();
                lase.lineDrawSpeed = lineDrawSpeed;
                lase.widthSpeed = widthSpeed;
                g.SetActive(true);
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
        }
        /// <summary>
        /// get disabled Puff(ImpactEffect).
        /// </summary>
        public GameObject getLaser()
        {
            for (int i = 0; i < lasersPool.Count; i++)
            {
                if (!lasersPool[i].activeInHierarchy)
                {
                    return lasersPool[i];
                }
            }
            if (expandLasers)
            {
                GameObject las = Instantiate(laser);
                las.transform.parent = PlayerLasers.transform;
                lasersPool.Add(las);
                return las;
            }
            return null;
        }
        /// <summary>
        /// get disabled Puff(ImpactEffect).
        /// </summary>
        public GameObject getPuff()
        {
            for (int i = 0; i < Puffs.Count; i++)
            {
                if (!Puffs[i].activeInHierarchy)
                {
                    return Puffs[i];
                }
            }
            if (expandPuff)
            {
                GameObject pof = Instantiate(Puff);
                pof.transform.parent = PlayerPuffs.transform;
                Puffs.Add(pof);
                return pof;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// get disabled ImpactEffect.
        /// </summary>
        public GameObject getImpactEffect()
        {
            for (int i = 0; i < ImpactEffects.Count; i++)
            {
                if (!ImpactEffects[i].activeInHierarchy)
                {
                    return ImpactEffects[i];
                }
            }
            if (expandEffect)
            {
                GameObject ef = Instantiate(Effect);
                ef.transform.parent = PlayerImpactEffect.transform;
                ImpactEffects.Add(ef);
                return ef;
            }
            else
            {
                return null;
            }
        }

    }
}