using UnityEngine;
using UnityEngine.Events;
namespace StrangeEngine
{
    //using UnityEditor;
    [RequireComponent(typeof(Rigidbody2D))]
    /// <summary>
    /// Player Movement.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        //
        [System.Serializable]
        public class PlayersMovementEvents
        {
            public UnityEvent onPlayerMove;
            public UnityEvent onPlayerFlip;
        }
        [HideInInspector] public bool facingRight = true;
        [Tooltip("Movement Speed of Player")]
        public float moveSpeed;                             // Movement Speed.	
        [Header("For PC Controls.")]
        public GameObject sight;// when using pc platform.
        public string verticalAxes = "Vertical";
        public string horizontalAxes = "Horizontal";
        public PlayersMovementEvents playerMovementEvents;
        //
        private Rigidbody2D rig;                            // Rigidbody Component.
        Vector3 movement;                                   // Vector3 for Movement.
        Animator anim;                                      // Animator Component.
        private bool mobile;
        private Vector2 moveInput; // input.
        private Vector2 moveVel; // move velocity.
        private RectTransform rt;
        [HideInInspector]
        public bool EnemyNear;
        [HideInInspector]
        public Aim AIMModes;
        [HideInInspector]
        public Vector3 pointer;
        private SpriteRenderer sprRend;
        float h;// temp float;
        float v;// temp float;
        private CameraMovement _camM;
                //
        void Awake()
        {
            // Set up references.
            anim = GetComponent<Animator>();                //Reference to Animator Component attached to this gameObject.
            rig = GetComponent<Rigidbody2D>();          //Reference to Rigidbody2D Component attached to this gameObject.
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
            sprRend = GetComponent<SpriteRenderer>();
        }
        private void Start()
        {
            _camM = Camera.main.GetComponent<CameraMovement>();
        }
        //
        void Update()
        {
            if (!mobile)
            {
                h = Input.GetAxis(horizontalAxes);
                v = Input.GetAxis(verticalAxes);
                moveInput = new Vector3(h, v);
                moveVel = moveInput.normalized * moveSpeed * 100f;
                Animating(h, v);// Animate Movement.
                                // If the player doesn't stop quickly enough using GetAxis,
                                // you can go to Edit->Project Settings->Input and change the Gravity under the Horizontal and Vertical axes. 
                                // The higher the number, the faster the axis will go back to 0 after you let go of the movement key.
            }
            if (mobile)
            {
                h = VirtualJoystick.InputDirection.x;       //Horisontal Axes.
                v = VirtualJoystick.InputDirection.y;       //Vertical Axes.
                moveInput = new Vector3(h, v);
                moveVel = moveInput.normalized * moveSpeed * 100f;
                Animating(h, v);// Animate Movement.
            }
            Vector2 clampedPos = new Vector2(Mathf.Clamp(transform.position.x, _camM.bounderies.w, _camM.bounderies.x), Mathf.Clamp(transform.position.y, _camM.bounderies.y, _camM.bounderies.z));
            transform.position = clampedPos;
        }
        //
        void FixedUpdate()
        {
            // Move the player around the scene with joystick.
            if (mobile)
            {
                rig.velocity = moveVel * Time.deltaTime;
                playerMovementEvents.onPlayerMove.Invoke();
            }
            // Move the player around the scene with keyboard.
            if (!mobile)
            {
                rig.velocity = moveVel * Time.deltaTime; // move player;
                playerMovementEvents.onPlayerMove.Invoke();
            }
            switch (AIMModes)
            {
                case Aim.none:
                    if (mobile)
                    {
                        try
                        {
                            if (rt.anchoredPosition3D.x > 1 && !facingRight) // if you Move Joystick it will Flip.
                            {
                                playerMovementEvents.onPlayerFlip.Invoke();
                                sprRend.flipX = false;
                            }
                            else if (rt.anchoredPosition3D.x < -1 && facingRight)   // if you Move Joystick it will Flip.
                            {
                                playerMovementEvents.onPlayerFlip.Invoke();
                                sprRend.flipX = true;
                            }
                        }
                        catch (UnassignedReferenceException)
                        {
                            Debug.Log("No 'MobileJoystick2 Rect Transform' Attached !");
                        }
                    }
                    else
                    {
                        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition); // get mouse position.
                        Face(difference);
                    }
                    break;
                case Aim.Manual:
                case Aim.FindClosestTarget:
                case Aim.ChooseRandomTarget:
                    if (EnemyNear)
                        Face(sight.transform.position);
                    else
                    {
                        if (h > 0) // if you Move Joystick it will Flip.
                        {
                            playerMovementEvents.onPlayerFlip.Invoke();
                            //Flip();
                            sprRend.flipX = false;
                        }
                        else if (h < 0)   // if you Move Joystick it will Flip.
                        {
                            playerMovementEvents.onPlayerFlip.Invoke();
                            //Flip();
                            sprRend.flipX = true;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// flip players sprite to left or right when he moves.
        /// </summary>
        void Flip()
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        void Face(Vector3 inputPos)
        {
            try
            {
                //
                Vector3 pointerr = new Vector3(inputPos.x, inputPos.y, 0); // set sight position.
                Vector3 dir = (pointerr - transform.position).normalized;
                float direction = Vector3.Dot(dir, transform.right);
                if (direction < 0.3f)
                {// stuff here
                    sprRend.flipX = true;
                    playerMovementEvents.onPlayerMove.Invoke();
                }
                if (direction > 0.3f)
                {
                    sprRend.flipX = false;
                    playerMovementEvents.onPlayerMove.Invoke();
                }
            }
            catch (UnassignedReferenceException)
            {
                Debug.Log("No 'Sight' Attached !");
            }
        }
        /// <summary>
        /// animate players movement.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="v"></param>
        void Animating(float h, float v)
        {
            bool walking = h != 0f || v != 0f;                              // Create a boolean that is true if either of the input axes is non-zero.
            anim.SetBool("IsWalking", walking);                         // Tell the animator whether or not the player is walking.
        }
    }
}