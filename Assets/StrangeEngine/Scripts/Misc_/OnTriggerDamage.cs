using UnityEngine;
namespace StrangeEngine
{

    /// <summary>
    /// Send damage to others when other enters triggered zone.
    /// </summary>
    public class OnTriggerDamage : MonoBehaviour
    {
        //
        [Tooltip("Time until destroy gameObject")]
        public float Time;
        [Tooltip("Damage you want send to other object.()")]
        public int damage;
        [Tooltip("tag of object, who you want send damage.")]
        public string enemyTag = "Enemy";
        //
        void Update()
        {
            Destroy(gameObject, Time);
        }
        // Send Damage to Enemies.
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(enemyTag))
            {
                other.GetComponent<HealthScore>().laserDamage(damage);
            }
        }
    }
}