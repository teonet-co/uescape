using UnityEngine;
using UnityEngine.Events;
namespace StrangeEngine
{
    /// <summary>
    /// You can use this to any objects that you want to set active, or if want to play or stop Particles. You can make many other weapons like this.
    /// </summary>
    public class Launcher : MonoBehaviour
    {
        //
        public float Ammo;
        [Tooltip("AmmoText GameObject you want to get Ammo from.")]
        public GameObject AmmoText;
        public bool enableAmmo = true;
        [Space(5)]
        [Tooltip("if true, you can use Flamethrower.")]
        public bool isFlamethrower;
        [Tooltip("if true, start flamethrower.")]
        public bool startFlamethrower;
        [Tooltip("flamethrower Particle System.")]
        public ParticleSystem Flamethrower;
        [Space(5)]
        [Tooltip("if true, you can use Light.")]
        public bool isLight;
        [Tooltip("if true, start Light.")]
        public bool startLight;
        [Tooltip("Light GameObject.")]
        public GameObject WeaponToLaunch;
        [Header("Camera Shake Settings.")]
        [Tooltip("Camera Shake parameters. Where x - is Shake power , y - Shake duration")]
        public ShakeType cameraShakeType;
        public oldShakeOptions simpleShakeSettings;
        public CameraShake.Properties sineShakeSettings;
        [Header("Events")]
        public UnityEvent onPlayerShoot;
        private bool mobile;
        private RectTransform rt;
        private LauncherAmmo ammo;
        private weaponHolder holder;
        private ShootMode shootMode;
        private CameraShake camShake;
        private ShootButton m_Button;
        private M_ShootTrigger trigger;
        //
        //Setting Start bools to false.
        void Start()
        {
            
            //startLight = false;
            //startFlamethrower = false;
            if(enableAmmo)
            ammo = AmmoText.GetComponent<LauncherAmmo>();
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
            holder = GetComponentInParent<weaponHolder>();
            trigger = GameManager._gameManager.shootOptions.shootTrigger;
            rt = GameManager.fRt;
            m_Button = GameManager._gameManager.shootOptions.shootButton.GetComponent<ShootButton>();// Instance to RectTransform selected in GameManager,
            camShake = GameManager._gameManager.mainCamera.GetComponent<CameraShake>();
        }
        //
        void Update()
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
                                    if (isLight)
                                    {
                                        startLight = true;
                                    }
                                    if (isFlamethrower)
                                    {
                                        startFlamethrower = true;
                                    }
                                }
                                else
                                {
                                    if (isLight)
                                    {
                                        startLight = false;
                                    }
                                    if (isFlamethrower)
                                    {
                                        startFlamethrower = false;
                                    }
                                }
                            }
                            break;
                        case M_ShootTrigger.Button:

                            if (m_Button != null)
                            {
                                if (m_Button.buttonDown)
                                {
                                    if (isLight)
                                    {
                                        startLight = true;
                                    }
                                    if (isFlamethrower)
                                    {
                                        startFlamethrower = true;
                                    }
                                }
                                else
                                {
                                    if (isLight)
                                    {
                                        startLight = false;
                                    }
                                    if (isFlamethrower)
                                    {
                                        startFlamethrower = false;
                                    }
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
                            break;
                        case ShootMode.single:
                            if (Input.GetKeyDown(GameManager._gameManager.shootOptions.key))
                            {
                                if (isLight)
                                {
                                    startLight = true;
                                }
                                if (isFlamethrower)
                                {
                                    startFlamethrower = true;
                                }
                            }
                            if (Input.GetKeyUp(GameManager._gameManager.shootOptions.key))
                            {
                                if (isLight)
                                {
                                    startLight = false;
                                }
                                if (isFlamethrower)
                                {
                                    startFlamethrower = false;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

            }

            if (startLight)
            {
                Launch();                                       // Activate Light
            }
            if (!startLight && isLight)
            {
                WeaponToLaunch.SetActive(false);                // Deactivate Light
            }

            if (startFlamethrower)
            {
                LaunchFlamethrower();                           // Start ParticleSystem
            }
            if (!startFlamethrower && isFlamethrower)
            {
                Flamethrower.Stop();                            // Stop ParticleSystem
            }


            if (enableAmmo)
                Ammo = ammo.ammo;   // Get actual Ammo from AmmoText GameObject.
            else Ammo = Mathf.Infinity;
        }
        /// <summary>
        /// Start shooting lights.
        /// </summary>
        public void Launch()
        {
            try
            {
                if (Ammo > 0)
                {
                    WeaponToLaunch.SetActive(true);
                    onPlayerShoot.Invoke();
                    if(enableAmmo)
                    ammo.MinusLauncherAmmo();      // Minus one Ammo
                }
                else
                    WeaponToLaunch.SetActive(false);
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No 'Weapon to launch' Attached!!");
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
        }
        /// <summary>
        /// Start shooting with flameThrower.
        /// </summary>
        public void LaunchFlamethrower()
        {
            try
            {
                if (Ammo > 0)
                {
                    onPlayerShoot.Invoke();
                    Flamethrower.Play();
                    if(enableAmmo)
                    ammo.MinusLauncherAmmo();      // Minus one Ammo
                }
                else
                    Flamethrower.Stop();
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No 'FlameThrower' Attached!!");
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
        }
        /// <summary>
        /// public function to Shoot Lights
        /// </summary>
        /// <param name="Start"></param>
        public void LightbuttonDown(bool s)
        {
            startLight = s;
        }
        /// <summary>
        /// public function to Shoot Flamethrower
        /// </summary>
        /// <param name="Start"></param>
        public void FlamethrowerbuttonDown(bool s)
        {
            startFlamethrower = s;
        }
    }
}