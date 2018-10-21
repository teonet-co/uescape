using UnityEngine;
//
namespace StrangeEngine
{

    public class EnemyEyes : MonoBehaviour
    {
        [Tooltip("how fast enemy will rotate to the player.")]
        public float rotSpeed;
        [Tooltip("Raycast Layers.(player, walls etc..)")]
        public LayerMask layers;
        [Tooltip("Time in seconds. How long enemy will pursuit player.")]
        public float seekTimer;
        [Header("Debug Indicators")]
        public bool seePlayer;
        public bool seekPlayer = false;
        public string playerTag = "Player";
        //
        private GameObject Player; // players gameObjet;
        private float timer; // temporary timer;
        private RaycastHit2D hit; //
        private AI_movement AIMove;
        //
        void Start()
        {
            Player = GameManager._player; // reference to player GameObject;
            AIMove = GetComponentInParent<AI_movement>();
        }
        //
        void FixedUpdate()
        {
            // if player GameObject exists,.
            if (Player != null)
            {
                Vector3 dir = Player.transform.position - transform.position; // find direction to the player.
                dir.Normalize(); // normalize direction;
                float zAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90; // rotation angle;
                Quaternion desiredRot = Quaternion.Euler(0, 0, zAngle); // 
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, rotSpeed * Time.deltaTime);   // face to the player.
                                                                                                                            // if player not in trigger zone
                if (!AIMove.PlayerEnterTrigger)
                {
                    // if pursuit mode ON;
                    if (seekPlayer)
                    {
                        followPlayer(); // start follow player;
                    }
                    hit = Physics2D.Raycast(transform.position, transform.up, 100, layers); // reaycast hit;
                                                                                            // if hit collider exist and its player;
                    if (hit.collider != null && hit.collider.CompareTag(playerTag))
                    {
                        // just double check; if false, enable pursuit mode;
                        if (!AIMove.PlayerEnterTrigger)
                        {
                            seekPlayer = true;
                            seePlayer = true;
                        }
                    }
                }
                // if player in trigger zone, disable pursuit mode;
                if (AIMove.PlayerEnterTrigger)
                {
                    seekPlayer = false;
                    //AIMove.persuit = seekPlayer;
                    seePlayer = false;
                }
            }
        }
        /// <summary>
        /// Starts pursuit mode;
        /// </summary>
        void followPlayer()
        {
            timer -= Time.deltaTime; // temp timer;
            AIMove.FollowPlayerPath(); // actual pursuit;
                                       // if timer reached 0 , stop pursuit;
            if (timer < 0)
            {
                seekPlayer = false;
                timer = seekTimer;
                seePlayer = false;
            }
        }
    }
}