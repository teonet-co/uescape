using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
namespace StrangeEngine
{
    public enum direction
    {
        left,
        right
    }
    [System.Serializable]
    public struct oldShakeOptions
    {
        public float shakePower;
        public float shakeDuration;
    }
    public enum ShakeType {none, simpleShake, sineShake}
    /// <summary>
    /// Class for Player Shooting settings.
    /// </summary>
    public class MachineGun : MonoBehaviour
    {
        //
        [Header("Bullet Settings")]
        [Space]
        [Tooltip("How many guns will shoot.")]
        public int amountOfGuns = 1;
        [Space]
        [Tooltip("You can easely change bullet offset here.")]
        public Vector3 bulletOffset = new Vector3(0, 0.5f, 0);
        [Tooltip("bullet accuracy, where x is minimum, y is maximum.")]
        public Vector2 bulletSprey;
        [Space(10)]
        [Tooltip("if true, on every shot will be instantiated Shell Prefab.")]
        public bool ActivateShell;
        public direction shellDirection;
        [Tooltip("You can easely change shell offset here.")]
        public Vector3 ShellOffset = new Vector3(0, 0.5f, 0);
        [Space]
        [Tooltip("FireRate.")]
        public float fireRate = 0.5F;
        private float nextFire = 0.0F;
        [Tooltip("force applied to bullet")]
        public float force;
        public ForceMode2D forceMode;
        [Space(10)]
        [Header("Ammo Settings.")]
        public bool enableAmmo;
        [Tooltip("Ammo text GameObject.(IF YOU USE WEAPON MAKER, AMMO TEXT WILL BE SET AUTOMATICLY)")]
        public GameObject AmmoText;
        [Space(10)]
        [Header("Other Settings.")]
        [Tooltip("if true, activates 'poof' prefab when shooting. ")]
        public bool Poof = true;

        [Tooltip("Activates recoil on every shot.")]
        public bool Recoil;
        [Tooltip("recoil on x and y axes(must be very low values.(0.01 or 0.001))")]
        public Vector2 recoilVector;
        [Header("Camera Shake Settings.")]
        [Tooltip("Camera Shake parameters. Where x - is Shake power , y - Shake duration")]
        public ShakeType cameraShakeType;
        public oldShakeOptions simpleShakeSettings;
        public CameraShake.Properties sineShakeSettings;

        //public Properties ShakeSettings;
        [Header("Events")]
        public UnityEvent onPlayerStartShooting;
        public UnityEvent onPlayerStopShooting;
        public UnityEvent onPlayerOutOfAmmo;
        [Header("Object Pooling.")]
        [Space(10)]
        [Tooltip("Bullet Prefab.")]
        public GameObject bulletPrefab;
        [Tooltip("Maximum bullets you want to instantiate in start and add them to bullets pool.")]
        public int maxBulletAmount;
        [Tooltip("infinite bullets in pool.")]
        public bool expand;
        [Space]
        [Tooltip("Explosion Prefab.")]
        public GameObject Puff;
        [Tooltip("Maximum puffs you want to instantiate in start and add them to puffs pool.")]
        public int maxPuffAmount;
        [Tooltip("infinite puffs in pool.")]
        public bool expandPuff = true;
        private GameObject PlayerBullets;
        private GameObject PlayerPuffs;
        [Space(10)]
        [Tooltip("Shell Prefab")]
        public GameObject ShellPrefab;
        [Tooltip("Maximum shells you want to instantiate in start and add them to shells pool.")]
        public int maxShellsAmount;
        [Tooltip("infinite shells in pool.")]
        public bool expandShells;
        private GameObject PlayerShells; // parent of all shells.
        [Space(10)]
        [Header("FOR DEBUG :")]
        [Tooltip("Current Ammo.")]
        public int Ammo;
        [Tooltip("if true, weapon will start shooting.")]
        public bool shot = false;
        [Tooltip("Only for debug. to see how many bullets in pool")]
        public List<GameObject> bullets;
        [Tooltip("Only for debug. to see how many shells in pool")]
        public List<GameObject> Shells;
        [Tooltip("Only for debug. to see how many puffs in pool")]
        public List<GameObject> Puffs;
        private bool mobile;
        private RectTransform rt;
        private GameObject player;
        private Vector2 shellVector;
        private Ammo ammo;
        private ShootMode shootMode;
        private weaponHolder holder;
        private CameraShake camShake;
        private ShootButton m_Button;
        private M_ShootTrigger trigger;
        private bool singleShot = false;
        //
        private void Start()
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!");
            holder = GetComponentInParent<weaponHolder>();
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
                ammo = AmmoText.GetComponent<Ammo>();
            trigger = GameManager._gameManager.shootOptions.shootTrigger;
            rt = GameManager.fRt;
            m_Button = GameManager._gameManager.shootOptions.shootButton.GetComponent<ShootButton>();
            
            player = GameManager._player;
            // Object Pooling for bullets;
            PlayerBullets = new GameObject("Player bullets"); // create parent GameObject;
            PlayerPuffs = new GameObject("Player puffs"); // create parent GameObject;
            PlayerBullets.transform.parent = GameManager._gameManager.PoolParent.transform;
            PlayerPuffs.transform.parent = GameManager._gameManager.PoolParent.transform;
            // Instantiate bullets and add them to bullets list
            for (int i = 0; i < maxBulletAmount; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.transform.parent = PlayerBullets.transform;
                bullet.SetActive(true);
                bullet.SetActive(false);
                bullets.Add(bullet);
            }
            // Object Pooling for Shells;
            PlayerShells = new GameObject("Player shells"); // create parent GameObject;
            PlayerShells.transform.parent = GameManager._gameManager.PoolParent.transform;
            // Instantiate Shells and add them to Shells list
            for (int i = 0; i < maxShellsAmount; i++)
            {
                GameObject shell = Instantiate(ShellPrefab);
                shell.transform.parent = PlayerShells.transform;
                shell.SetActive(true);
                shell.SetActive(false);
                Shells.Add(shell);
            }
            for (int i = 0; i < maxPuffAmount; i++)
            {
                GameObject puf = Instantiate(Puff);
                puf.transform.parent = PlayerPuffs.transform;
                puf.SetActive(true);
                puf.SetActive(false);
                Puffs.Add(puf);
            }
            camShake = GameManager._gameManager.mainCamera.GetComponent<CameraShake>();
        }
        //
        void FixedUpdate()
        {
            Debug.Log("222222222222222222");
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
                                shootMode = holder.shootMode;
                                switch (shootMode)
                                {
                                    case ShootMode.burst:
                                        if (m_Button.buttonDown)
                                        {
                                            shot = true;
                                        }
                                        else
                                        {
                                            shot = false;
                                        }
                                        break;
                                    case ShootMode.single:
                                        if (m_Button.buttonDown && singleShot == false)
                                        {
                                            shot = true;
                                            singleShot = true;
                                        }
                                        else
                                        {
                                            shot = false;
                                        }
                                        if (!m_Button.buttonDown && shootMode == ShootMode.single)
                                        {
                                            singleShot = false;
                                        }
                                        break;
                                    default:
                                        break;
                                }


                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        //
        void Update()
        {
            Debug.Log("HUI");
            if (shot)
            {                                       // if bool shot is true, start shooting
                if (holder)
                {
                    shootMode = holder.shootMode;
                    switch (shootMode)
                    {
                        case ShootMode.burst:
                            {
                                Shoot();
                            }
                            break;
                        case ShootMode.single:
                            {
                                Shoot();
                                shot = false;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            if (!mobile)
            {
                Debug.Log("PIZDA0");
                if (holder)
                {
                    Debug.Log("PIZDA1");
                    shootMode = holder.shootMode;
                    switch (shootMode)
                    {
                        case ShootMode.burst:
                            if (Input.GetKey(GameManager._gameManager.shootOptions.key))
                            {
                                Debug.Log("PIZDA");
                                Shoot();
                            }
                            break;
                        case ShootMode.single:
                            if (Input.GetKeyDown(GameManager._gameManager.shootOptions.key))
                            {
                                Debug.Log("PIZDA33");
                                Shoot();
                            }
                            break;
                        default:
                            Debug.Log("PIZDA44");
                            break;
                    }
                }

            }
            if (enableAmmo)
                Ammo = ammo.ammo;   // Get actual Ammo from AmmoText GameObject.
            else Ammo = int.MaxValue;
        }
        /// <summary>
        /// shoot function.
        /// </summary>
        public void Shoot(int bullCount = 0)                                     // Start Shooting!!
        {
            if (Time.time > nextFire )
            {
                if (Ammo > 0)
                {
                    onPlayerStartShooting.Invoke();
                    nextFire = Time.time + fireRate;            // add time for next fire
                    if (enableAmmo)
                        ammo.MinusAmmo();           // Send Message to AmmoText GameObject that we spent one bullet
                    Vector3 Shelloffset1 = transform.rotation * ShellOffset; //Vector3 shell offset
                    try
                    {
                        for (int i = 0; i < amountOfGuns; i++)
                        {
                            float spread = UnityEngine.Random.Range(bulletSprey.x, bulletSprey.y);
                            Quaternion bulletRotation = Quaternion.Euler(new Vector3(0, 0, spread));
                            Vector3 offset3 = (transform.rotation * bulletRotation) * DegreeToVector2(90); ;
                            Vector3 offset1 = transform.rotation * bulletOffset;// Vector3 Bullet Offset
                            Vector2 tt = offset3;
                            GameObject bullet = getBullet(); // get bullet from pool;
                            if (bullet == null)
                            {
                                return;
                            }
                            bullet.transform.SetPositionAndRotation(transform.position + offset1, transform.rotation);
                            bullet.SetActive(true);
                            if (bullet.GetComponent<Rigidbody2D>()) bullet.GetComponent<Rigidbody2D>().AddForce(tt * force, forceMode); // Add Force to Grenade.
                        }


                        if (Recoil)
                        {
                            // a little bit of recoil.
                            Vector3 recoilVect = new Vector3(GetComponentInParent<Transform>().rotation.x + recoilVector.x, GetComponentInParent<Transform>().rotation.y + recoilVector.y);
                            player.transform.position = new Vector3(player.transform.position.x - recoilVect.x, player.transform.position.y - recoilVect.y, player.transform.position.z);
                        }

                        if (ActivateShell)                          // if bool ActivateShell is true, Instantiate Shell Prefab
                        {
                            GameObject shell = getShell(); // get shell from pool;
                            if (shell == null)
                            {
                                return;
                            }
                            shell.transform.SetPositionAndRotation(transform.position + Shelloffset1, transform.rotation);
                            shell.GetComponent<BoxCollider2D>().enabled = true;
                            shell.GetComponent<Rigidbody2D>().isKinematic = false;
                            shell.GetComponent<Rigidbody2D>().freezeRotation = false;
                            shell.SetActive(true);
                            switch (shellDirection)
                            {
                                case direction.left:
                                    shellVector = transform.right - new Vector3(UnityEngine.Random.Range(0f, 2), 0); // Random Shell Direction
                                    break;
                                case direction.right:
                                    shellVector = (transform.right - new Vector3(UnityEngine.Random.Range(0f, 2), 0)) * -1; // Random Shell Direction
                                    break;
                                default:
                                    break;
                            }
                            Rigidbody2D ShellClone = shell.GetComponent<Rigidbody2D>();
                            ShellClone.AddForce(shellVector * UnityEngine.Random.Range(.2f, 4f), ForceMode2D.Force); // Add Force to Shell RigidBody2D
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
                    }
                    catch (UnassignedReferenceException)
                    {
                        Debug.Log("Prefab Needed!");
                    }
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
                        Debug.Log("No Active Camera or 'Camera Shake' Script doesn't attached to Camera! ");
                    }
                    onPlayerStopShooting.Invoke();
                }
                else
                {
                    onPlayerOutOfAmmo.Invoke();
                }

            }
        }
        /// <summary>
        /// get disabled bullet.
        /// </summary>
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
                bullet.transform.parent = PlayerBullets.transform;
                bullets.Add(bullet);
                return bullet;
            }
            return null;
        }
        /// <summary>
        /// get disabled Shell.
        /// </summary>
        public GameObject getShell()
        {
            for (int i = 0; i < Shells.Count; i++)
            {
                if (!Shells[i].activeInHierarchy)
                {
                    return Shells[i];
                }
            }
            if (expandShells)
            {
                GameObject shell = Instantiate(ShellPrefab);
                shell.transform.parent = PlayerBullets.transform;
                Shells.Add(shell);
                return shell;
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
            return null;
        }
        public void buttondown()
        {
            shot = true;
        }
        /// <summary>
        /// public function to Stop Shooting
        /// </summary>
        public void buttonup()
        {
            shot = false;
        }
        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
    }
}