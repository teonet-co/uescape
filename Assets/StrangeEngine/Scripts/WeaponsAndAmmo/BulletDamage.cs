using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
/// <summary>
/// class for Damage Enemies with bullets.
/// </summary>
namespace StrangeEngine
{
    public class BulletDamage : MonoBehaviour
    {
        //
        [Tooltip("Prefab Impact splash.")]
        public GameObject splash;
        public string enemyTag = "Enemy";
        public string wallTag = "Walls";
        public float damage;
        public UnityEvent onBulletImpactWall;
        public UnityEvent onBulletImpactEnemy;
        private GameObject _splash;
        private LevelGeneration levl;
        //
        private void Start()
        {
            if (splash)
            {
                _splash = Instantiate(splash);
                _splash.transform.parent = GameManager._gameManager.SplashesAndBlows.transform;
                _splash.SetActive(false);
            }
            levl = FindObjectOfType<LevelGeneration>();
        }
        /// <summary>
        /// on collision with walls or enemy enable splash GameObject.
        /// </summary>
        /// <param name="coll"></param>
        void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.CompareTag(wallTag))
            {
                Vector3 hitPosition = Vector3.zero;
                onBulletImpactWall.Invoke();
                foreach (ContactPoint2D hit in coll.contacts)
                {
                    hitPosition.x = hit.point.x - 5f * hit.normal.x;
                    hitPosition.y = hit.point.y - 5f * hit.normal.y;
                    levl.Destruct(levl.wallsMap.WorldToCell(hitPosition), damage);
                }
                if (splash)
                {
                    _splash.transform.position = transform.position;
                    _splash.transform.rotation = transform.rotation;
                    _splash.SetActive(true);
                }

                gameObject.SetActive(false);
            }
            if (coll.gameObject.CompareTag(enemyTag))
            {
                onBulletImpactEnemy.Invoke();
                if (splash)
                {
                    _splash.transform.position = transform.position;
                    _splash.transform.rotation = transform.rotation;
                    _splash.SetActive(true);
                }
                if (!splash)
                    Debug.Log("Please add Splash Prefab!");
                gameObject.SetActive(false);

                //calls for my desired "pause"
                //     ProjectileSleep ();
                //ACCURATELY waits .004ms to undo pause. Works like a charm! I multiply the .004ms by timeScale 
                //or else it will take longer than you want it to. The repeat here doesn't matter because the bullet 
                //destroys itself before it can occur.
                //     InvokeRepeating ("WakeUp", .004f * Time.timeScale, 1F);
            }
        }
        /// <summary>
        /// Starts little pause
        /// </summary>
        void ProjectileSleep()
        {
            //starts the "pause"
            Time.timeScale = .001f;
        }
        /// <summary>
        /// return timeScale to normal
        /// </summary>
        void WakeUp()
        {
            Time.timeScale = 1f;
            Destroy(gameObject);
        }
    }
}