using UnityEngine;
using UnityEngine.Events;
namespace StrangeEngine
{
    /// <summary>
    /// class for Arrows
    /// </summary>
    public class ArrowScript : MonoBehaviour
    {
        //
        [Tooltip("Damage")]
        public int Damage;
        [Tooltip("time to Destroy gameObject")]
        public float timer;
        public string enemyTag;
        public string playerTag;
        public string wallTag;
        public target _target;
        [Header("Events")]
        public UnityEvent onArrowImpact;
        //
        void OnEnable()
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();//Reference to Rigidbody2D component.
            BoxCollider2D bc = GetComponent<BoxCollider2D>();// Reference to BoxCollider2D component.
            rb.isKinematic = false;
            rb.freezeRotation = false;
            bc.enabled = true;
            Invoke("destroy", timer);
        }
        //
        void OnCollisionEnter2D(Collision2D coll)
        {
            switch (_target)
            {
                case target.enemies:
                    if (coll.gameObject.CompareTag(enemyTag))
                    {
                        onArrowImpact.Invoke();
                        gameObject.transform.position = coll.transform.position;
                        coll.gameObject.GetComponent<HealthScore>().laserDamage(Damage);
                        Rigidbody2D rb = GetComponent<Rigidbody2D>();//Reference to Rigidbody2D component.
                        BoxCollider2D bc = GetComponent<BoxCollider2D>();// Reference to BoxCollider2D component.
                        Destroy(rb);// I Destroy those components, because i don'd want them to collide with other gameobjects in scene after Amount of time. Looks nice and Perfomence is much better.
                        Destroy(bc);
                        rb.velocity = Vector3.zero;
                        rb.isKinematic = true;
                        rb.freezeRotation = true;
                        bc.enabled = false;
                    }
                    break;
                case target.player:
                    if (coll.gameObject.CompareTag(playerTag))
                    {
                        onArrowImpact.Invoke();
                        gameObject.transform.position = coll.transform.position;
                        coll.gameObject.GetComponent<PlayerHealth>().laserDamage(Damage);
                        Rigidbody2D rb = GetComponent<Rigidbody2D>();//Reference to Rigidbody2D component.
                        BoxCollider2D bc = GetComponent<BoxCollider2D>();// Reference to BoxCollider2D component.
                        Destroy(rb);// I Destroy those components, because i don'd want them to collide with other gameobjects in scene after Amount of time. Looks nice and Perfomence is much better.
                        Destroy(bc);
                        rb.velocity = Vector3.zero;
                        rb.isKinematic = true;
                        rb.freezeRotation = true;
                        bc.enabled = false;
                    }
                    break;
                default:
                    break;
            }

            if (coll.gameObject.CompareTag(wallTag))
            {
                onArrowImpact.Invoke();
                gameObject.transform.parent = coll.transform;
                Rigidbody2D rb = GetComponent<Rigidbody2D>();//Reference to Rigidbody2D component.
                BoxCollider2D bc = GetComponent<BoxCollider2D>();// Reference to BoxCollider2D component.
                Destroy(rb);// I Destroy those components, because i don'd want them to collide with other gameobjects in scene after Amount of time. Looks nice and Perfomence is much better.
                Destroy(bc);
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
                rb.freezeRotation = true;
                bc.enabled = false;
            }
        }
        /// <summary>
        /// disables this gameobject.
        /// </summary>
        void destroy()
        {
            gameObject.SetActive(true);
        }
    }
}