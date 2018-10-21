using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
namespace StrangeEngine
{
    public class LightningBolt : MonoBehaviour
    {
        //  
        [Header("Main Settings.")]
        [Tooltip("Enemy and walls layers.")]
        public LayerMask layers;
        [Tooltip("Lightning bolt prefab.")]
        public GameObject lightningPrefab;
        [Tooltip("Damage")]
        public int damage;
        [Tooltip("Amount of lightnings to spawn at one shot.")]
        public int NumToSpawn = 5;
        [Tooltip("FireRate.")]
        public float fireRate = 0.5F;
        public string enemyTag = "Enemy";
        public string playerTag = "Player";
        public target _target;
        [Header("Camera Shake Settings.")]
        [Tooltip("Camera Shake parameters. Where x - is Shake power , y - Shake duration")]
        public ShakeType cameraShakeType;
        public oldShakeOptions simpleShakeSettings;
        public CameraShake.Properties sineShakeSettings;
        [Header("Object Pooling.")]
        [Tooltip("if true, activates 'poof' prefab when shooting. ")]
        public bool Poof = true;
        [Tooltip("Explosion Prefab.")]
        public GameObject Puff;
        [Tooltip("Maximum puffs you want to instantiate in start and add them to puffs pool.")]
        public int maxPuffAmount;
        [Tooltip("infinite puffs in pool.")]
        public bool expandPuff = true;
        private GameObject PlayerPuffs;
        [Space(10)]
        [Tooltip("if true, activates 'poof' prefab when shooting. ")]
        public bool ImpactEffect = true;
        [Tooltip("Explosion Prefab.")]
        public GameObject Effect;
        [Tooltip("Maximum puffs you want to instantiate in start and add them to puffs pool.")]
        public int maxImpactAmount;
        [Tooltip("infinite puffs in pool.")]
        public bool expandEffect = true;
        private GameObject PlayerImpactEffect;
        [Space(10)]
        [Header("Ammo Settings.")]
        public bool enableAmmo = true;
        [Tooltip("Current Ammo.")]
        public int Ammo;
        [Tooltip("Ammo text GameObject.")]
        public GameObject AmmoText;
        [Header("Events")]
        public UnityEvent onPlayerShoot;
        [Header("Object Pooling DEBUG")]
        [Tooltip("Only for debug.")]
        [HideInInspector]
        public bool shoot; // if true start shooting;
        [Tooltip("Only for debug. to see how many puffs in pool")]
        public List<GameObject> Puffs;
        [Tooltip("Only for debug. to see how many puffs in pool")]
        public List<GameObject> ImpactEffects;
        //
        private float nextFire = 0.0F;
        public bool readyToShoot;
        RaycastHit2D hit;
        private bool mobile;
        private RectTransform rt;
        private Ammo _ammo;
        private weaponHolder holder;
        private ShootMode shootMode;
        private CameraShake camShake;
        private GameObject tr;
        private Vector2 hitPosition;
        private ShootButton m_Button;
        private M_ShootTrigger trigger;

        //
        void Start()
        {
            tr = new GameObject("temp");
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
            trigger = GameManager._gameManager.shootOptions.shootTrigger;
            rt = GameManager.fRt;
            m_Button = GameManager._gameManager.shootOptions.shootButton.GetComponent<ShootButton>();// Instance to RectTransform selected in GameManager,
            if (Poof)
            {
                PlayerPuffs = new GameObject("Player puffs"); // create parent GameObject;
                PlayerPuffs.transform.parent = GameManager._gameManager.PoolParent.transform;
                for (int i = 0; i < maxPuffAmount; i++)
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
                PlayerImpactEffect.transform.parent = GameManager._gameManager.PoolParent.transform;
                for (int i = 0; i < maxImpactAmount; i++)
                {
                    GameObject eff = Instantiate(Effect);
                    eff.transform.parent = PlayerImpactEffect.transform;
                    eff.SetActive(true);
                    eff.SetActive(false);
                    ImpactEffects.Add(eff);
                }
            }
            holder = GetComponentInParent<weaponHolder>();
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
                Ammo = int.MaxValue;
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
                                    readyToShoot = true;
                                }
                                else
                                {
                                    readyToShoot = false;
                                }
                            }
                            break;
                        case M_ShootTrigger.Button:

                            if (m_Button != null)
                            {
                                if (m_Button.buttonDown)
                                {
                                    readyToShoot = true;
                                }
                                else
                                {
                                    readyToShoot = false;
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
                                shot();
                            }
                            break;
                        case ShootMode.single:
                            if (Input.GetKeyDown(GameManager._gameManager.shootOptions.key))
                            {
                                shot();
                            }
                            break;
                        default:
                            break;
                    }
                }

            }

            if (readyToShoot)
            {
                shoot = true;
            }
            if (!readyToShoot)
            {
                shoot = false;

            }
            if (shoot)
            {
                shot();
            }
        }
        //
       public void shot()
        {
            if (Time.time > nextFire && Ammo > 0)
            {
                nextFire = Time.time + fireRate;            // add time for next fire
                                                            //readyToShoot = true;
                if (enableAmmo)
                    _ammo.MinusAmmo();          // Send Message to AmmoText GameObject that we spent one bullet

                onPlayerShoot.Invoke();

                hit = Physics2D.Raycast(transform.position, transform.up, 200, layers);
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
                            if (hit.collider.CompareTag(enemyTag))
                            {
                                hit.collider.GetComponent<PlayerHealth>().laserDamage(damage);
                            }
                            break;
                        default:
                            break;
                    }

                }
                if (hit.collider != null)
                {
                    for (int i = 0; i < NumToSpawn; i++)
                    { // instantiate lightning prefabs.
                        GameObject litGO = Instantiate(lightningPrefab, transform.position, Quaternion.identity);
                        if (hit.collider != null)
                        {
                            
                            hitPosition.y = hit.point.y - hit.normal.y;
                            hitPosition.x = hit.point.x - hit.normal.x;
                            tr.transform.position = hitPosition;
                            litGO.GetComponent<Lightning>().EndPoint = tr.transform;
                        }
                    }
                }
                switch (cameraShakeType)
                {
                    case ShakeType.none: break;
                    case ShakeType.simpleShake:
                        camShake.ShakeCamera(simpleShakeSettings.shakePower, simpleShakeSettings.shakeDuration); break;
                    case ShakeType.sineShake:
                        camShake.StartShake(sineShakeSettings); break;
                    default: break;
                }
                if (Poof)                                   // if Poof is true,Activate Puff prefab
                {
                    GameObject poff = getPuff(); // get shell from pool;
                    if (poff == null)
                    {
                        return;
                    }
                    poff.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    poff.SetActive(true);
                }
                if (ImpactEffect)                                   // if Poof is true,Activate Puff prefab
                {
                    if (hit)
                    {
                        GameObject imp = getImpactEffect(); // get shell from pool;
                        if (imp == null)
                        {
                            return;
                        }
                        imp.transform.SetPositionAndRotation(hitPosition, hit.transform.rotation);
                        imp.SetActive(true);
                    }
                }
                if(_target == target.player)
                shoot = false;
            }
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