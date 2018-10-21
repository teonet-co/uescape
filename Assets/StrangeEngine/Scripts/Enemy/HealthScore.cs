using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
/// <summary>
/// Class for Enemy Health.
/// </summary>
namespace StrangeEngine
{
    public class HealthScore : MonoBehaviour
    {
        //
        [Tooltip("How much score will take player if enemy will be killed.")]
        public int scoreValue;
        [Tooltip("Enemy health.")]
        public int health;
        [Tooltip("Your bullets Tag.")]
        public string bulletsTag = "Bullet";
        [Tooltip("Explosion prefab.")]
        public GameObject boom;
        public bool enableFlash = true;
        [Tooltip("on Impact Flash sprite.")]
        public Sprite muzzleFlash;
        [Tooltip("How many frames Impact Flash will be shown.")]
        public int framesToFlash = 3;
        [Tooltip("Put here all Ammo that enemy will drop when he dies.")]
        public GameObject[] cartridges;
        [Tooltip("Chance that when enemy dies Ammo will be droped")]
        public float CartridgeChance;
        [Header("Events")]
        public UnityEvent onEnemyDie;
        public UnityEvent onEnemyTakeDamage;
        //
        //private int Index;                           //index of weapon that will harm Enemy
        private SpriteRenderer spriteRend;
        private Sprite defaultSprite;
        /*    private WeaponSelection w;   */                        // Weapon Selection. 
        private bool hited = false; // true if enemy was hitted by players weapon.
 /// <summary>
/// This function changes sprite to Impact Flash sprite, waites and then change to default sprite.
/// </summary>
/// <returns></returns>
        IEnumerator FlashImpactFlash()
        {
            spriteRend.sprite = muzzleFlash;
            for (int i = 0; i < framesToFlash; i++)
            {
                yield return 0;
            }
            spriteRend.sprite = defaultSprite;
        }
        void Start()
        {
            spriteRend = GetComponent<SpriteRenderer>();
            defaultSprite = spriteRend.sprite;
        }

        //On collision with bullet enemy will take damage according to selected weapon and will be shawn ImpactFlash.
        void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.CompareTag(bulletsTag))
            {
                try
                {
                    if (GameManager._gameManager.weaponSelection.selectionIndex <= GameManager._gameManager.WeaponsDamage.Count)
                    {
                        health -= GameManager._gameManager.WeaponsDamage[GameManager._gameManager.weaponSelection.selectionIndex];
                        onEnemyTakeDamage.Invoke();
                    }
                    else
                    {
                        print("no Damage for selected Weapon!");
                    }
                }
                catch (NullReferenceException)
                {
                    Debug.Log("No Weapon Selection attached in GameManger.");
                }
                if (enableFlash)
                    StartCoroutine(FlashImpactFlash());
            }
            if (coll.gameObject.CompareTag("Missile"))
            {
                health -= 2000;
                onEnemyTakeDamage.Invoke();
            }
        }
        //
        void Update()
        {
            if (health <= 0)
            {
                Expl();
                Die();
            }
        }
        /// <summary>
        /// Instantiate Explosion Prefab
        /// </summary>
        void Expl()
        {
            GameObject Expl = Instantiate(boom);
            Expl.transform.position = transform.position;
        }
        /// <summary>
        /// When Enemy Health < 0, enemy will give some score to the player, may be give player some Ammo and then destroy himself.
        /// </summary>
        void Die()
        {
            ScoreManager.score += scoreValue;
            float CartridgePercentage = UnityEngine.Random.Range(0f, 100f);
            onEnemyDie.Invoke();
            if (CartridgeChance > CartridgePercentage)
            {
                for (int i = 0; i < cartridges.Length; i++)
                {
                    Instantiate(cartridges[i], transform.position + new Vector3(UnityEngine.Random.Range(1f, 4f), (UnityEngine.Random.Range(1f, 3f))), transform.rotation);
                }
            }
            GameManager._gameManager.Enemies.Remove(this.gameObject);
            GameManager._gameManager.enemiesPositions.Remove(transform);
            // remove enemy from mode lists;
            if (GameManager._gameManager.activeEnemies.Contains(this.gameObject))
            {
                GameManager._gameManager.activeEnemies.Remove(this.gameObject);     // remove this enemy from active enemies list;
            }
            if (GameManager._gameManager.pursuitingEnemies.Contains(this.gameObject))
            {
                GameManager._gameManager.pursuitingEnemies.Remove(this.gameObject);     // remove this enemy from active enemies list;
            }
            Destroy(gameObject);
        }
        /// <summary>
        /// Public function to make damage to enemy from other Scripts.
        /// </summary>
        /// <param damage="damage"></param>
        public void laserDamage(int damage)
        {
            health -= damage;
            onEnemyTakeDamage.Invoke();
        }
        /// <summary>
        /// two functions to know if enemy was hited by particles.
        /// </summary>
        /// <returns></returns>
        public bool wasHit()
        {
            onEnemyTakeDamage.Invoke();
            if (hited) return true;
            else return false;
        }
        public void hit(bool was)
        {
            onEnemyTakeDamage.Invoke();
            hited = was;
        }
    }
}