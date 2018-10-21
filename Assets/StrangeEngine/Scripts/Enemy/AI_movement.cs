using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
/// <summary>
/// Enemy Movement.
/// </summary>
namespace StrangeEngine
{
    public enum modes
    {
        pursuitMode,
        activeMode,
        passiveMode,
        flyMode
    }
    
    [System.Serializable]
    public class flyOpt
    {
        [Tooltip("Activates fly mode.")]
        public bool canFly;
        [Tooltip("Speed when enemy flyes.")]
        [Range(1, 100)]
        public float flySpeed;
        [Tooltip("Minimum distance from player, that enemy can start flying.")]
        [Range(1, 500)]
        public float minDistToFly;
        [Tooltip("Time in seconds. Enemy will fly after amount of time.")]
        public Vector2 timeBetweenFly;
        public float scaleDuration;
        public Vector3 _targetScale; // change this value if you want enemy to be bigger whan it flyes.
    }
    public class AI_movement : MonoBehaviour
    {
        [System.Serializable]
        public class EnemyEvents
        {
            public UnityEvent onEnemyMoveInActiveMode;
            public UnityEvent onEnemyMoveInPassiveMode;
            public UnityEvent onEnemyMoveInPursuitMode;
            public UnityEvent onEnemyStay;
            public UnityEvent onEnemyMoveInFlyMode;
            public UnityEvent onEnemyStartFly;
            public UnityEvent onEnemyEndFly;
        }
        //	
        [Header("Speed settings.")]
        [Tooltip("Max Speed that Enemy can move in active mode")]
        [Range(1, 100)]
        public float SpeedMax;
        [Tooltip("Min Speed that Enemy can move in active mode")]
        [Range(1, 100)]
        public float SpeedMin;
        [Space]
        [Tooltip("Movement speed in passive mode.")]
        [Range(1, 100)]
        public float passiveMovementSpeed;
        [Space]
        [Tooltip("Movement speed in pursuit mode(if you dont want pursuit mode dont modify this value. PURSUIT MODE ACTIVATES WITH BOOL FOLLOW PLAYER!!)")]
        [Range(1, 100)]
        public float persuitSpeed;
        [Space]
        //
        [Header("Main settings")]
        public LayerMask avoidLayers;
        [Tooltip("Radius of zone , where enemy can move in active mode.")]
        [Range(1, 500)]
        public float radius;
        [Tooltip("Radius of zone, where enemy can move in passive mode.")]
        [Range(1, 500)]
        public float passiveRadius;
        [Tooltip("Min Distance from player and Enemy(how close enemy can be to the player).")]
        [Range(1, 500)]
        public float DistRangeMin;
        [Tooltip("After this distance passive mode will be active.")]
        [Range(1, 500)]
        public float maxDistanceFromPlayer;
        [Tooltip("time in seconds. enemy will change his position after this time (Higher value means less movement in active mode).")]
        [Range(1, 100)]
        public float activityValue;
        [Tooltip("time in seconds. enemy will change his position after this time (Higher value means less movement in passive mode)")]
        [Range(0, 100)]
        public float passivityValue;
        [Tooltip("damage on collision with player")]
        public float damage;
        [Tooltip("Pursuit mode On/Of.(follow player, when enemy see player, but player not in PlayerEnterTrigger zone.)")]
        public bool followPlayer;
        [Tooltip("Follow player with random path.(Works only if followplayer = true)")]
        public bool randomlyFollowPlayer;
        //
        [Header("Fly options")]
        public flyOpt flyOptions;
        [Header("Events")]
        public EnemyEvents enemyEvents;
        public string playerTag = "Player";
        public string enemyTag = "Enemy";
        public string wallsTag = "Walls";
        [Header("Debug indicators!!! No need to touch.")]
        public modes _modes;
        public bool PlayerEnterTrigger = false; // true if player enteres trigger zone(triggered collider is on enemy weapon).
        //
        private GameObject Player; // Reference to player GameObject.
        private float Speed;  // Temp enemy speed(Random value between SpeedMin and SpeedMax.)
        private Rigidbody2D rb; // Reference to enemies Rigidbody.
        private float timeBetweenRandomize; // time in seconds to randomise enemies movement in pursuit mode.
        private float timeBetweenRandomize2;// time in seconds to randomize enemies speed.
        private float timeBetweenChoosingnextPosition = 0;// time Between Choosing next Position in active and passive modes.
        private float timeBetweenChoosingNextPassivePosition = 0;// time Between Choosing next Position in active and passive modes.
        private Vector3 RandomPointAroundPlayer; // random point around player in active mode.
        private Vector3 RandomPointAround; // random point around enemy in passive mode.
        private float r = 0; // random time beetween fly in fly mode.
        private bool fly = false; // true if enemy actually fly.
        private Vector3 tempPlayerPosition; // previous players position.
        //private float timer = 5; // time in seconds. This timer helps to avoid situation where Enemy flying to long. That means that enemy cant fly more than 5 seconds.
        private float doubleCheckTimer = 2; // timer in case if timer didnt worked.
        //private int wallCollisions; // amount of wall collisions.
        private static Vector3 _one = new Vector3(1, 1, 1); // just Vector.one for great perfomance.
        private Vector2 enemyPos; // enemy position.
        private Vector2 playerPos; // player position.
        private Vector2 randomFollowVect; // enemy will follow this vector2 in pursuit mode with randomlyFollowPlayer= true.
        private PlayersPath path; //PlayersPath Component.
        private Vector2 tempPath; // temp Vector2 for calculation points where enemy can move in active mode.
        private float timr = 5; // time in seconds to avoid endles while loop in FindRandomPoint().
        [HideInInspector] public bool facingRight = true; // is enemy facing in right direction.
        private Collider2D coll;
        private EnemyEyes eyes;
        private float d;
        //private float _time;
        private bool startScale;
        private bool startUnScale;
        //
        void Awake()
        {
            _modes = modes.activeMode;
            coll = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>(); // reference to RigidBody Component.
            if (flyOptions.canFly) // If canFly true, set references.
            {
                //wallCollisions = 0; // deafult state.
            }
            if (DistRangeMin > radius) // Checks distrangMin and radius values to avoid errors.
            {
                DistRangeMin = radius - 10; // if DistRangeMin higher than radius. Set -10.
            }
            eyes = GetComponentInChildren<EnemyEyes>();
            if (flyOptions.canFly)
                r = Time.time + flyOptions.timeBetweenFly.y;
        }
        void Start()
        {
            GameManager._gameManager.Enemies.Add(gameObject);                          // Adding this enemy to List of enemies(you can find it in Gamemanager GameObject).
            GameManager._gameManager.enemiesPositions.Add(transform);
            try
            {
                Player = GameManager._gameManager.player;                                   // reference to player GameObject;
            }
            catch (NullReferenceException)
            {
                Debug.Log("No Player");
            }
            if (Player)
            {
                path = Player.GetComponent<PlayersPath>();                       //reference to PlayersPath Component. 
            }
        }
        //
       private void FixedUpdate()
        {
            playerPos = new Vector2(Player.transform.position.x, Player.transform.position.y); // Short writing for players Vector2;
            enemyPos = new Vector2(transform.position.x, transform.position.y); // Short writing for enemies Vector2;
            if ((enemyPos- playerPos).magnitude < radius)
            {
                PlayerEnterTrigger = true;
            }
            if ((enemyPos - playerPos).magnitude > radius)
            {
                PlayerEnterTrigger = false;
            }
            d = (Player.transform.position - transform.position).magnitude;                  // temp variable to calculate distnce between player and enemy;
            switch (_modes)
            {

                case modes.pursuitMode:
                    Flip();
                    if (randomlyFollowPlayer)
                    {
                        if (Time.time > timeBetweenRandomize)
                        {
                            timeBetweenRandomize = Time.time + 0.1f;
                            randomFollowVect = new Vector2(UnityEngine.Random.Range(0, 30), UnityEngine.Random.Range(0, 30));
                        }
                    }
                    if (path.tir.Count > 0)                         // check if Queue count bigger than zero to avoid Errors on start;
                    {
                        //if player not in triggerd zone and distance from player is bigger than maxDistanceFromPlayer, activate passsive mode;
                        if ((path.tir.Peek().transform.position - transform.position).magnitude > maxDistanceFromPlayer)
                        {
                            if (GameManager._gameManager.pursuitingEnemies.Contains(this.gameObject))
                            {
                                GameManager._gameManager.pursuitingEnemies.Remove(this.gameObject); // remove this enemy from pursuiting enemies list;
                            }
                            _modes = modes.passiveMode;
                        }
                        else
                        {
                            // if player not in triggerd zone and distance from player is less than maxDistanceFromPlayer, 
                            // and amount of pursuiting enemies is less then Maximum Enemies that can pursuit player, and enemy really see player , activate pursuit mode;
                            if (GameManager._gameManager.pursuitingEnemies.Count < GameManager._gameManager.maxPersuitingEnemies && eyes.seePlayer)
                            {
                                eyes.seekPlayer = true; //reference to Enemy Eyes component;       
                                if (!GameManager._gameManager.pursuitingEnemies.Contains(this.gameObject))
                                {
                                    GameManager._gameManager.pursuitingEnemies.Add(this.gameObject); // remove this enemy from pursuiting enemies list;
                                }
                            }
                            else
                            { // disable pursuit mode if count of pursuiting enemies is bigger then maxPersuitingEnemies;
                                if (GameManager._gameManager.pursuitingEnemies.Contains(this.gameObject))
                                {
                                    GameManager._gameManager.pursuitingEnemies.Remove(this.gameObject); // remove this enemy from pursuiting enemies list;
                                }
                                eyes.seePlayer = false; // reference to enemy eyes component;
                                _modes = modes.passiveMode;
                            }
                        }
                    }
                    else
                        Debug.Log("No players Path!");

                    break;
                case modes.activeMode:

                    if (d > maxDistanceFromPlayer || GameManager._gameManager.activeEnemies.Count >= GameManager._gameManager.maxActiveEnemies)
                    {
                        if (GameManager._gameManager.activeEnemies.Contains(this.gameObject))     // remove this enemy from active enemies list;)
                            GameManager._gameManager.activeEnemies.Remove(this.gameObject);     // remove this enemy from active enemies list;
                        _modes = modes.passiveMode;
                    }
                    else if (!PlayerEnterTrigger && (Player.transform.position - transform.position).magnitude < maxDistanceFromPlayer && eyes.seePlayer)
                    {
                        if (GameManager._gameManager.activeEnemies.Contains(this.gameObject))     // remove this enemy from active enemies list;)
                            GameManager._gameManager.activeEnemies.Remove(this.gameObject);     // remove this enemy from active enemies list;
                        _modes = modes.pursuitMode;
                    }
                    else
                    {
                        Flip();
                        Move(); // start moving;
                        if (GameManager._gameManager.activeEnemies.Contains(this.gameObject))     // remove this enemy from active enemies list;)
                            return;
                        else
                            GameManager._gameManager.activeEnemies.Add(this.gameObject);     // remove this enemy from active enemies list;
                    }

                    if (flyOptions.canFly)
                    {

                        float randomTimeBetweenFly = UnityEngine.Random.Range(flyOptions.timeBetweenFly.x, flyOptions.timeBetweenFly.y); // temp value to calculate randomly choosed time to enemy 
                        if (Time.time > r)
                        {
                            r = Time.time + randomTimeBetweenFly;
                            tempPlayerPosition = Player.transform.position;
                            //if distance from player is bigger than Minimum distance from player, that enemy can start flying;
                            if (d > flyOptions.minDistToFly)
                            {
                                startScale = true;
                                startUnScale = true;
                                fly = false;
                                _modes = modes.flyMode;
                            }
                        }
                    }

                    break;
                case modes.passiveMode:
                    if (!PlayerEnterTrigger ||d>maxDistanceFromPlayer || GameManager._gameManager.activeEnemies.Count >= GameManager._gameManager.maxActiveEnemies)
                    {
                        StayAround();                           // movement in passive mode ;
                    }
                    else
                    {
                        _modes = modes.activeMode;
                    }
                    break;
                case modes.flyMode:
                    FLy();
                    break;
                default:
                    break;
            }
            if(_modes != modes.flyMode)
            {

                if (coll.enabled == false)
                    coll.enabled = true; // enable enemy collider, because when enemy in fly mode ,collider is off, because we dont want to stack in walls;

            }
            if (rb.velocity.x < 1 || rb.velocity.y < 1)
            {
                enemyEvents.onEnemyStay.Invoke();
            }
            //_time = Time.time;
        }

        /// <summary>
        /// function for passive mode.
        /// </summary>
        public void StayAround()
        {

            if (Time.time > timeBetweenChoosingNextPassivePosition)
            {
                timeBetweenChoosingNextPassivePosition = Time.time + passivityValue;
                FindPassiveMovementPoint();
            }
            if(RandomPointAround == Vector3.zero)
                FindPassiveMovementPoint();
            if ((RandomPointAround- transform.position).magnitude> 2)
            {
                if (RandomPointAround != Vector3.zero)
                    rb.velocity = (RandomPointAround - transform.position) * passiveMovementSpeed * Time.deltaTime;
                else
                    rb.velocity = Vector2.zero;

                if (rb.velocity.x < 1 || rb.velocity.y < 1)
                {
                    enemyEvents.onEnemyMoveInPassiveMode.Invoke();
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

        }
        /// <summary>
        ///  Moves enemy with random speed to the random point around player.
        /// </summary>
        public void Move()
        {
            if (rb.velocity.x > 1|| rb.velocity.y > 1)
            {
                enemyEvents.onEnemyMoveInActiveMode.Invoke();
            }
            // if timeBetweenChoosingnextPosition elapsed, find another point to move;
            if (Time.time > timeBetweenChoosingnextPosition)
            {
                timeBetweenChoosingnextPosition = Time.time + activityValue;
                Speed = UnityEngine.Random.Range(SpeedMin, SpeedMax);
                FindRandomPoint();
            }
            if (RandomPointAroundPlayer == Vector3.zero)
            {
                FindRandomPoint();
            }
            // if enemy dont reached another point, enemy will move to it;
            if ((RandomPointAroundPlayer - transform.position).magnitude > 2)
            {
                //transform.position = Vector2.MoveTowards(transform.position, RandomPointAroundPlayer, Speed * Time.deltaTime);
                rb.velocity = (RandomPointAroundPlayer - transform.position) * Speed * Time.deltaTime;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        /// <summary>
        /// function to pursuit player;
        /// </summary>
        public void FollowPlayerPath()
        {

            if (rb.velocity.x > 1 || rb.velocity.y > 1)
                enemyEvents.onEnemyMoveInPursuitMode.Invoke();
            
            // check Queue count to avoid errors
            if (path.tir.Count > 0)
            {
                tempPath = path.tir.Peek().transform.position; // reference to last point in Queue;
            }
            // if only followplayer= true; Simply follow player
            if (followPlayer && !randomlyFollowPlayer)
            {
                rb.velocity = (tempPath - new Vector2(transform.position.x, transform.position.y)) * persuitSpeed * Time.deltaTime;
                //transform.position = Vector2.MoveTowards(transform.position, tempPath, persuitSpeed * Time.deltaTime);
            }
            // if followplayer and randomlyFollowPlayer = true; Randomly follow player;
            else
            {
                rb.velocity = ((tempPath + randomFollowVect) - new Vector2(transform.position.x,transform.position.y)) * persuitSpeed * Time.deltaTime;
                //transform.position = Vector2.MoveTowards(transform.position, tempPath + randomFollowVect, persuitSpeed * Time.deltaTime);
            }

        }
        /// <summary>
        ///  Moves enemy with fly speed to previous player's position.
        /// </summary>
        public void FLy()
        {
            rb.velocity = (tempPlayerPosition - transform.position) * flyOptions.flySpeed * Time.deltaTime;
            float ds = Vector3.Distance(transform.position, tempPlayerPosition); // distance between temporary player position and enemy position;

            // if distance bigger then minDistToFly
            if (ds > flyOptions.minDistToFly)
            {
                coll.enabled = false; // disable collider to not crash in walls;
                if (startScale)
                {
                    enemyEvents.onEnemyStartFly.Invoke();
                    StartCoroutine(ScaleObject(flyOptions.scaleDuration, flyOptions._targetScale)); // start scale enemy;
                    startScale = false;
                }
            }
            // if timer is passed or distance is less then minDistToFly or enemy is in passive mode ;
            else 
            {
                if (startUnScale)
                {
                    enemyEvents.onEnemyEndFly.Invoke();
                    StopAllCoroutines(); // stop scaling enemy;
                    StartCoroutine(UnScaleObject(flyOptions.scaleDuration, _one));          // unscale enemy to normal size;
                    startUnScale = false;
                }
            }
            if (fly)
            {
                enemyEvents.onEnemyMoveInFlyMode.Invoke();
                fly = false;
            }
        }
        /// <summary>
        /// Smoothly scale object
        /// </summary>
        /// <param name="scaleDuration"></param>
        /// <param name="targetScale"></param>
        /// <returns></returns>
        private IEnumerator ScaleObject(float scaleDuration, Vector3 targetScale)
        {
            GetComponent<SpriteRenderer>().sortingOrder ++;
            Vector3 actualScale = transform.localScale;             // scale of the object at the begining of the animation
                                                                    // extra duper puper check;
            // smooth loop, for smooth scale; 
            for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
            {
                transform.localScale = Vector3.Lerp(actualScale, targetScale, t);
                yield return null;
            }
            if (transform.localScale.x > (targetScale.x- 0.05f))
            {
                fly = true;
                StopCoroutine("ScaleObject");

            }
        }
        /// <summary>
        /// smoothly UnScale object;
        /// </summary>
        /// <param name="scaleDuration"></param>
        /// <param name="targetScale"></param>
        /// <returns></returns>
        private IEnumerator UnScaleObject(float scaleDuration, Vector3 targetScale)
        {
            doubleCheckTimer -= Time.deltaTime;
            Vector3 actualScale = transform.localScale;
            for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
            {
                transform.localScale = Vector3.Lerp(actualScale, targetScale, t);
                yield return null;
            }
            if (transform.localScale.x <= 1.1f || doubleCheckTimer <= 0)
            {
                fly = false;
                coll.enabled = true; // enable collider;
                //timer = 0;                              //resset timers; 
                //timer = 5;
                doubleCheckTimer = 2;
                GetComponent<SpriteRenderer>().sortingOrder --;
                transform.localScale = _one;
                _modes = modes.activeMode;
                StopAllCoroutines();
            }
        }
        /// <summary>
        /// Finds random point around player;
        /// </summary>
        private void FindRandomPoint()
        {
            Collider2D[] results = new Collider2D[100]; // assign temp Collider2D array(there will be Walls);
            RaycastHit2D[] results2 = new RaycastHit2D[100]; // assign temp RaycastHit2D array(there will be Walls);
            int result = 1;
            int result2 = 1;
            int tempResult = result; // assign temporary results;
            int tempResult2 = result2; // assign another temp results;
            Vector2 tempPoint; // declare temp point;
                               // if there were walls around temp point or on path to point.
            while (result > 0)
            {

                tempPoint = UnityEngine.Random.insideUnitCircle * radius + playerPos; // finding another temp point around player to move;
                tempResult = Physics2D.OverlapCircleNonAlloc(tempPoint, 4, results, avoidLayers); // find walls around temp point;
                tempResult2 = Physics2D.LinecastNonAlloc(tempPoint, enemyPos, results2, avoidLayers); // cast a line from previous position to next position and see how many walls
                // another case go out the loop;(be carefull with while loops!!)
                if (Vector2.Distance(Player.transform.position, tempPoint) > radius)
                {
                    RandomPointAroundPlayer = transform.position;
                    result = 0;
                    break;
                }
                // if we found temporary point with no walls around;
                if (tempResult == 0 && tempResult2 == 1)
                {
                    if (Vector2.Distance(Player.transform.position, tempPoint) > DistRangeMin)
                    {
                        timr = 2; // reset timer;
                        RandomPointAroundPlayer = tempPoint; // assign Random point ;
                        result = tempResult; // get out of the loop;
                        break;
                    }
                }
                timr -= Time.deltaTime; // start timer to be able to stop the loop, or it will be endless;
                if (timr < 0)
                {
                    timr = 5;
                    break;
                }
            }
            //}
        }

        private void FindPassiveMovementPoint()
        {
            Collider2D[] results = new Collider2D[2]; // assign temp Collider2D array(there will be Walls);
            //RaycastHit2D[] results2 = new RaycastHit2D[100]; // assign temp RaycastHit2D array(there will be Walls);
            int result = 1;
            //int result2 = 1;
            int tempResult = result; // assign temporary results;
            //int tempResult2 = result2; // assign another temp results;
            Vector2 tempPoint; // declare temp point;
                               // if there were walls around temp point or on path to point.
            while (result > 0 && !PlayerEnterTrigger)
            {

                tempPoint = UnityEngine.Random.insideUnitCircle * passiveRadius + enemyPos; // finding another temp point around player to move;
                tempResult = Physics2D.OverlapCircleNonAlloc(tempPoint, 2, results, avoidLayers); // find walls around temp point;
                //tempResult2 = Physics2D.LinecastNonAlloc(transform.position, tempPoint, results2, avoidLayers); // cast a line from previous position to next position and see how many walls
                // if we found temporary point with no walls around;
                if (tempResult == 0 )
                {
                        timr = 10; // reset timer;
                        RandomPointAround = tempPoint; // assign Random point ;
                        result = tempResult; // get out of the loop;

                    break;
                }
                timr -= Time.deltaTime; // start timer to be able to stop the loop, or it will be endless;
                if (timr < 0)
                {
                    timr = 10; // reset timer;
                    break;
                }
            }
            
        }
        private void Flip()
        {
            Vector3 dir = (Player.transform.position - transform.position).normalized;
            float direction = Vector3.Dot(dir, transform.right);

            if (direction < 0.3f)
            {// stuff here
                GetComponent<SpriteRenderer>().flipX = true;
            }
            if (direction > 0.3f)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        //void OnCollisionEnter2D(Collision2D col)
        //{
        //    // On collision with walls;
        //    if (col.gameObject.CompareTag(wallsTag))
        //    {
        //        GetComponentInChildren<EnemyEyes>().seekPlayer = false; // stop pursuit player;
        //        //Vector2 e = new Vector2(transform.position.x - col.transform.position.x, transform.position.y - col.transform.position.y); // opposite to wall Vector3;
        //        //rb.AddForce(e * 40); // go away from wall!; 
        //        if (PlayerEnterTrigger)
        //        {
        //            FindRandomPoint(); // if in trigger zone, find another point to move around player;
        //        }
        //        // if enemy can fly, on collision with walls it will start flying;
        //        else if (canFly && PlayerEnterTrigger)
        //        {
        //            wallCollisions++; // count collisions;
        //            if (wallCollisions > 10) // if there more then 10 , then it kind of weard and we must do something;
        //            {
        //                fly = true; // start fly;
        //                FLy();
        //                wallCollisions = 0; // reset collisions;
        //            }
        //        }
        //        else
        //        {
        //            _modes = modes.passiveMode;
        //            FindPassiveMovementPoint(); // if not in trigger zone and passive mode is on, go find another point to move;
        //        }

        //    }
        //    // if collision with player happens;
        //    if (col.gameObject.CompareTag(playerTag))
        //    {
        //        col.gameObject.GetComponent<PlayerHealth>().laserDamage(damage);// harm player;
        //        if (PlayerEnterTrigger)
        //        {
        //            FindRandomPoint();
        //            Move();// start moving around;
        //        }
        //    }
        //    if (col.gameObject.CompareTag(enemyTag))
        //    {
        //        if (PlayerEnterTrigger)
        //        {
        //            FindRandomPoint(); // if in trigger zone, find another point to move around player;
        //        }
        //        else
        //        {
        //            _modes = modes.passiveMode;
        //            FindPassiveMovementPoint(); // if not in trigger zone and passive mode is on, go find another point to move;
        //        }
        //    }

        //}
    }
}