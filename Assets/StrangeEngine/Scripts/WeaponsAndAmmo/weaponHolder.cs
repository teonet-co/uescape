using UnityEngine;
using System.Collections.Generic;
namespace StrangeEngine
{
    public enum Aim{none,Manual,FindClosestTarget,ChooseRandomTarget}
    [RequireComponent(typeof(SpriteRenderer))]
    /// <summary>
    /// Rotates weapon with joystick or mouse. Changes weapon layer to 'gun' or 'grass'.
    /// </summary>
    [HasSortingLayerName("foregroundLayer", "backgroundLayer")]
    public class weaponHolder : MonoBehaviour
    {
        //	
        [Tooltip("Single/Burst fire mode.")]
        public ShootMode shootMode;
        [Header("Auto AIM Settings.")]
        [Space]
        [Tooltip("AIM mode.(none means NO Auto AIM.)")]
        public Aim AIMMode;
        [Tooltip("Reference to Sight.")]
        public GameObject sight;
        [Tooltip("Value represents, how fast aim sight will move between targets.")]
        public float smoothValue;
        [Tooltip("Minimum AIM distance.")]
        public float minDistance;
        [Tooltip("Maximum AIM distance.")]
        public float maxDistance;
        [Tooltip("Time in seconds to choose next target.")]
        public float delay;
        [Tooltip("Key to choose next target.")]
        public KeyCode nextTarget;
        [Tooltip("Key to choose previous target.")]
        public KeyCode previousTarget;
//
        private SpriteRenderer spr;                         // SpriteRenderer.
        private bool mobile;                                //
        private RectTransform rt;
        private GameObject player;
        private float r;
        [HideInInspector]
        public string foregroundLayer;
        [HideInInspector]
        public string backgroundLayer;
        [HideInInspector]
        public int index;
        private List<Transform> enemies;
        private float nextFire;
        private Transform tempTarget;
        private List<Transform> nearestEnemies;
        private Vector3 pointer;
        private PlayerMovement weap;
        private CameraMovement _cameraMovement;
        //
        private void Start()
        {
            spr = GetComponent<SpriteRenderer>();           // Reference to SpriteRenderer of this gameObject.
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
            rt = GameManager.fRt;
            player = GameManager._player;
            enemies = new List<Transform>();
            enemies = GameManager._gameManager.enemiesPositions;
            _cameraMovement = GameManager._gameManager.mainCamera.GetComponentInChildren<CameraMovement>();
            if (_cameraMovement == null) _cameraMovement = GameManager._gameManager.mainCamera.GetComponent<CameraMovement>();
            if (_cameraMovement == null) Debug.Log("Please insert camera holder  in Gamemanager component!");




            weap = GetComponentInParent<PlayerMovement>();
            weap.sight = sight;
            weap.AIMModes = AIMMode;


        }
        private void OnEnable()
        {
            weap = GetComponentInParent<PlayerMovement>();
            weap.sight = sight;
            weap.AIMModes = AIMMode;

        }
        //
        void Update()
        {
            if (mobile)
            {
                if (rt)
                {
                    if (rt.position.y >= 80)        // if joystick is up, change layer.
                        spr.sortingLayerName = backgroundLayer;
                    else
                        spr.sortingLayerName = foregroundLayer;
                }
                switch (AIMMode)
                {
                    case Aim.none:

                        float h = VirtualJoystick2.InputDirection2.x;   // Horisontal Axes.
                        float v = VirtualJoystick2.InputDirection2.y;   // Vertical Axes.
                                                                        // Rotate weapon with Joystick.
                        Vector3 lookVector = new Vector3(h, v, 4096);
                        if (lookVector.x != 0 && lookVector.y != 0)
                        {
                            transform.rotation = Quaternion.LookRotation(lookVector, Vector3.back);
                        }
                        break;
                    case Aim.Manual:
                        if (Time.time > nextFire)
                        {
                            nextFire = Time.time + delay;
                            FindEnemiesInRange(minDistance, maxDistance);
                        }
                        if (nearestEnemies.Count != 0)
                        {
                            if (index > nearestEnemies.Count - 1 || index < 0)
                                ChooseNextTarget();
                            if (Input.GetKeyDown(nextTarget))
                                ChooseNextTarget();
                            if (Input.GetKeyDown(previousTarget))
                                ChoosePreviousTarget();
                            if (nearestEnemies[index])
                                tempTarget = nearestEnemies[index];
                            else
                                FindEnemiesInRange(minDistance, maxDistance);
                        }
                        break;
                    case Aim.FindClosestTarget:
                        spr.flipX = player.GetComponent<SpriteRenderer>().flipX;
                        if (Time.time > nextFire)
                        {
                            nextFire = Time.time + delay;
                            tempTarget = FindClosestEnemy(minDistance, maxDistance);
                        }
                        if (tempTarget)
                            FaceTarget();
                        else
                        {
                            if (nearestEnemies.Count != 0)
                                nextFire = 0;
                        }
                        weap.pointer = pointer;
                        break;
                    case Aim.ChooseRandomTarget:
                        spr.flipX = player.GetComponent<SpriteRenderer>().flipX;
                        if (Time.time > nextFire)
                        {
                            nextFire = Time.time + delay;
                            tempTarget = FindRandomEnemy(minDistance, maxDistance);
                        }
                        if (tempTarget)
                            FaceTarget();
                        else
                        {
                            if (nearestEnemies.Count != 0)
                                nextFire = 0;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {// Rotate weapon with mouse
                spr.flipX = player.GetComponent<SpriteRenderer>().flipX;
                switch (AIMMode)
                {
                    case Aim.none:

                        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                        difference.Normalize();
                        float zAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90;
                        Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, 99999 * Time.deltaTime);   // face to the player.
                        break;
                    case Aim.Manual:
                        if (Time.time > nextFire)
                        {
                            nextFire = Time.time + delay;
                            FindEnemiesInRange(minDistance, maxDistance);
                        }
                        if (nearestEnemies.Count != 0)
                        {
                            if (index > nearestEnemies.Count - 1 || index < 0)
                                ChooseNextTarget();
                            if (Input.GetKeyDown(nextTarget))
                                ChooseNextTarget();
                            if (Input.GetKeyDown(previousTarget))
                                ChoosePreviousTarget();
                            if (nearestEnemies[index])
                                tempTarget = nearestEnemies[index];
                            else
                                FindEnemiesInRange(minDistance, maxDistance);
                            FaceTarget();
                        }
                        break;
                    case Aim.FindClosestTarget:
                        if (Time.time > nextFire)
                        {
                            nextFire = Time.time + delay;
                            tempTarget = FindClosestEnemy(minDistance, maxDistance);
                        }
                        if (tempTarget)
                            FaceTarget();
                        else
                        {
                            if (nearestEnemies.Count != 0)
                                nextFire = 0;
                        }
                        break;
                    case Aim.ChooseRandomTarget:
                        if (Time.time > nextFire)
                        {
                            nextFire = Time.time + delay;
                            tempTarget = FindRandomEnemy(minDistance, maxDistance);
                        }
                        if (tempTarget)
                            FaceTarget();
                        else
                        {
                            if (nearestEnemies.Count != 0)
                                nextFire = 0;
                        }
                        break;
                    default:
                        break;
                }
                float r = transform.localEulerAngles.z;
                if (r < 30 && r > 0 || r > 300)
                {
                    spr.sortingLayerName = backgroundLayer;
                }
                else
                {
                    spr.sortingLayerName = foregroundLayer;
                }
            }

            switch (AIMMode)
            {
                case Aim.none:
                    Vector3 difference1 = Camera.main.ScreenToWorldPoint(Input.mousePosition); // get mouse position.
                    sight.transform.position = new Vector3(difference1.x, difference1.y, 0); // set sight position.
                    break;
                case Aim.Manual:
                case Aim.FindClosestTarget:
                case Aim.ChooseRandomTarget:
                    if (tempTarget)
                    {
                        Vector3 oldPos = sight.transform.position;
                        sight.transform.position = Vector3.Lerp(oldPos, tempTarget.position, Time.deltaTime * smoothValue);
                        pointer = sight.transform.position;
                        weap.EnemyNear = true;
                    }
                    else
                    {
                        weap.EnemyNear = false;
                        sight.transform.position = transform.position + new Vector3(0, 300, 0);
                    }

                    break;
                default:
                    break;
            }
            switch (AIMMode)
            {
                case Aim.none:
                    break;
                case Aim.Manual:
                case Aim.FindClosestTarget:
                case Aim.ChooseRandomTarget:
                    _cameraMovement.offset = 0;
                    break;
            }
        }
        public void FaceTarget()
        {
            Vector3 lp = pointer - transform.position;
            Vector3 LookPos = new Vector3(lp.x, lp.y, 4096);

            transform.rotation = Quaternion.LookRotation(LookPos, Vector3.back);
        }
        public void ChooseNextTarget()
        {
            if (index >= nearestEnemies.Count - 1)
                index = 0;
            else
                index++;
        }
        public void ChoosePreviousTarget()
        {
            if (index <= 0)
                index = nearestEnemies.Count - 1;
            else
                index--;
        }
        public void FindEnemiesInRange(float min, float max)
        {
            Vector3 position = transform.position;
            nearestEnemies = new List<Transform>();
            // Calculate squared distances
            min = min * min;
            max = max * max;
            if (enemies.Count != 0)
            {
                foreach (Transform transform in enemies) // for every enemy in enemies.
                {
                    Vector3 diff = transform.position - position;
                    float curDistance = diff.sqrMagnitude;
                    if (curDistance >= min && curDistance <= max)
                    {
                        nearestEnemies.Add(transform);
                    }
                }
            }
        }
        public Transform FindRandomEnemy(float min, float max)
        {
            Vector3 position = transform.position;
            nearestEnemies = new List<Transform>();
            // Calculate squared distances
            min = min * min;
            max = max * max;
            if (enemies.Count != 0)
            {
                foreach (Transform transform in enemies) // for every enemy in enemies.
                {
                    Vector3 diff = transform.position - position;
                    float curDistance = diff.sqrMagnitude;
                    if (curDistance >= min && curDistance <= max)
                    {
                        nearestEnemies.Add(transform);
                    }
                }
                if (nearestEnemies.Count != 0)
                {
                    int randomIndex = Random.Range(0, nearestEnemies.Count);
                    return nearestEnemies[randomIndex];
                }
                else return null;

            }
            else return null;
        }
        /// <summary>
        /// Finds closest Enemy by Distance.
        /// </summary>
        /// <param name="min">Minimum Distance</param>
        /// <param name="max">Maximum Distance</param>
        /// <returns>closest enemy</returns>
        public Transform FindClosestEnemy(float min, float max)
        {
            nearestEnemies = new List<Transform>();
            Transform closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            // Calculate squared distances
            min = min * min;
            max = max * max;
            foreach (Transform transform in enemies) // for every enemy in enemies.
            {
                Vector3 diff = transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance >= min && curDistance <= max)
                {
                    nearestEnemies.Add(transform);
                }
                if (curDistance < distance && curDistance >= min && curDistance <= max)
                {
                    closest = transform;
                    distance = curDistance;
                }
            }
            if (closest) return closest; else return null;
        }
    }
}