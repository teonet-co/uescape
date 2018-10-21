using UnityEngine;
using System;
/// <summary>
/// Enemy Aim class.
/// </summary>
namespace StrangeEngine
{
    public class FacesPlayerNew : MonoBehaviour
    {
        //
        [Tooltip("How fast enemy will rotate to aim player.")]
        public float rotSpeed = 90f;
        [Tooltip("Player and Walls layers.")]
        public LayerMask layers;
        [Tooltip("weapons you want to shoot.")]
        public GameObject[] weapons;
        [Tooltip("how often enemy will shoot.")]
        public float shootChance = 90;
        public string playerTag = "Player";
        Transform player; // transform component.
        [Tooltip("How fast enemy will rotate to aim player.")]
        public bool PlayerEnterTrigger;
        private float ShootPercentage;
        private RaycastHit2D hit;
        public bool CanSeePlayer;
        //public string
        private float rTimr;
        private AI_movement AIMove;
        //
        void Awake()
        {
            try
            {
                player = GameManager._player.transform; // Reference to 'Player' GameObject.
            }
            catch (NullReferenceException)
            {
                Debug.Log("No Player");
            }
            AIMove = GetComponentInParent<AI_movement>();
            foreach (var stvol in weapons)
            {
                stvol.SetActive(false);
            }
        }
        //
        void FixedUpdate()
        {
            PlayerEnterTrigger = AIMove.PlayerEnterTrigger; // reference to AI_movement1 component and PlayerEntertrigger bool.
            hit = Physics2D.Raycast(transform.position, transform.up, 100, layers); // raycast hit
                                                                                    //Debug.DrawRay(transform.position, transform.up * 1000, Color.green);
            if (PlayerEnterTrigger) // if bool 'PlayerEnterTrigger' true Enemy will start aiming player.
            {
                CanSeePlayer = false;
                rTimr -= Time.deltaTime;
                // Debug.DrawRay (transform.position, transform.up* 1000, Color.green);
                if (hit.collider != null && hit.collider.CompareTag(playerTag))
                {
                    if (rTimr <= 0)
                    {
                        ShootPercentage = UnityEngine.Random.Range(0f, 100f); // change chance to shoot every amount of time.
                        rTimr = 1;
                    }
                    if (shootChance > ShootPercentage)
                    {
                        
                        try
                        {
                            foreach (GameObject stvol in weapons)
                            {
                                stvol.SetActive(true);
                            }
                        }
                        catch (NullReferenceException)
                        {
                            Debug.Log("Not all Enemy weapons has been set");
                        }
                    }
                    else
                    {
                        foreach (GameObject stvol in weapons)
                        {
                            stvol.SetActive(false);
                        }
                    }
                }
                else
                {
                    foreach (GameObject stvol in weapons)
                    {
                        stvol.SetActive(false);
                    }
                }
                if (hit.collider != null && !hit.collider.CompareTag(playerTag))
                {

                    foreach (GameObject stvol in weapons)
                    {
                        if (stvol)
                        {
                            stvol.SetActive(false);
                        }
                    }

                }
                Turn();
            }
            else if (!PlayerEnterTrigger && hit.collider != null && hit.collider.CompareTag(playerTag))
            {
                CanSeePlayer = true;
            }
            else
            {
                CanSeePlayer = false;
            }
        }
        /// <summary>
        /// Aiming Players GameObject
        /// </summary>
        void Turn()
        {
            if (player != null)
            { // if player GameObject exists, start Aiming.
                Vector3 dir = player.position - transform.position; // find direction to the player.
                dir.Normalize();
                float zAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
                Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, rotSpeed * Time.deltaTime);   // face to the player.
            }
        }

    }
}