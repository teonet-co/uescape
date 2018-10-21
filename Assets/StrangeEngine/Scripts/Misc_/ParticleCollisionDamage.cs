using UnityEngine;
using System.Collections;
namespace StrangeEngine
{

    /// <summary>
    /// Make damage on particle collision.
    /// </summary>
    public class ParticleCollisionDamage : MonoBehaviour
    {
        [Tooltip("Damage you want to send to Enemies")]
        public int damage;
        public string damageRecieverTag = "Enemy";
        [Tooltip("Enable electric hit effect.")]
        public bool enableElectricEffect;
        [Tooltip("When electric particles touches enemy, Instantiate this GameObject(effect of electric hit)")]
        public GameObject electricParticlesEffectForEnemy;
        [Tooltip("Time in seconds. Destroy electricParticlesEffectForEnemy after amount of time.")]
        public float destroyEffectAfter;
        public target _target;
        /// <summary>
        /// Send Damage.
        /// </summary>
        /// <param name="other"></param>
        void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag(damageRecieverTag))
            {
                if (enableElectricEffect) // if effect enabled
                {
                    if (!other.GetComponent<HealthScore>().wasHit()) // if enemy was not hited before
                    {
                        GameObject par = Instantiate(electricParticlesEffectForEnemy, other.transform.position, other.transform.rotation); // instantiate effect
                        par.transform.parent = other.transform; // parent effect to enemy transform.
                        other.GetComponent<HealthScore>().hit(true); // say to enemy that he took a hit.
                        StartCoroutine(destroy(par, destroyEffectAfter)); // destroy effect after amount of time.
                    }
                    else return;
                }
                switch (_target)
                {
                    case target.enemies:
                        other.GetComponent<HealthScore>().laserDamage(damage); // harm enemy with particles.

                        break;
                    case target.player:
                        other.GetComponent<PlayerHealth>().laserDamage(damage); // harm enemy with particles.
                        break;
                    default:
                        break;
                }
            }
        }
        IEnumerator destroy(GameObject p, float time)
        {//destroy electric effect
            yield return new WaitForSeconds(time);
            if (p)
            {
                p.transform.parent.GetComponent<HealthScore>().hit(false);
                Destroy(p, time);
            }

        }
    }
}