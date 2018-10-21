using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Camera Smoothly follows or outruns player with set values.
/// </summary>
namespace StrangeEngine
{
    [System.Serializable]
    public struct _bounds
    {
        [Tooltip("Bounds name.")]
        public string name;
        [Tooltip("Minimum offset on X axes")]
        public float minX;
        [Tooltip("Minimum offset on Y axes")]
        public float minY;
        [Tooltip("Maximum offset on X axes")]
        public float maxX;
        [Tooltip("Maximum offset on Y axes")]
        public float maxY;
    }
    public enum updateOpt
    {
        FixedUpdate,
        LateUpdate
    }
    public class CameraMovement : MonoBehaviour
    {
        [Tooltip("Update Camera movement settings.")]
        public updateOpt updateOptions;
        [Tooltip("Camera offset")]
        public float offset;                            //camera offset
        private Vector3 playerPosition;
        [Tooltip("Value represents how smoothly camera will move.")]
        public float offsetSmoothing;                   // Smooth Camera Movement
        [Tooltip("Maximum camera size.(When player move,camera will smoothly encrease size. Depends on players velocity.)")]
        public float maxCameraSize;
        [Tooltip("Minimum camera size.")]
        public float minCameraSize;
        [Tooltip("How smooth camera will encrease size.")]
        public float smoothValue;
        [Tooltip("Cut off players velocity(How fast camera will encrease size.Depends on players velocity)")]
        public float cutOff;
        [Tooltip("Camera component.")]
        public Camera _camera;
      [Tooltip("List of Bounds offset for different type of Level Generation.(Don't add/delete bounds, only adjust values.)")]
        public List<_bounds> bounds;
        [HideInInspector]
        public int boundsOffsetIndex; // index of bounds in bounds list;
        private float h; // horizontal axes;
        private float v; // vertical axes;
        private Rigidbody2D rig; // Rigidbody;
        private float scrW; // screen width;
        private float scrH; // screen height;
        private Vector2 direction; // temp direction;
        private LevelGeneration levelMaker; // Reference to Level Generation;
        private GameObject player;         // Referens to the player.
        [HideInInspector]
        public Vector4 bounderies;
        //
        private void Start()
        {
            // Reference to LevelGeneration;
            levelMaker = GameManager._gameManager.levelGeneration;
            // Reference to player GameObject;
            player = GameManager._gameManager.player;
            // Check if Camera component not set in Inspector, try to check in this gameObject;
            if (_camera == null) _camera = GetComponent<Camera>();
            if (_camera == null) Debug.Log("No Camera!");
            //Reference to RigidBody2D;
            rig = player.GetComponent<Rigidbody2D>();
            // Changing Index, dependinng on Generation type;
            switch (levelMaker.generationType)
            {
                case genType._old:
                    boundsOffsetIndex = 0;
                    break;
                case genType._new:
                    boundsOffsetIndex = 1;
                    break;
            }
        }
        private void FixedUpdate()
        {
            if (updateOptions == updateOpt.FixedUpdate) Move();
        }
        private void LateUpdate()
        {
            if (updateOptions == updateOpt.LateUpdate) Move();
        }
        /// <summary>
        /// Camera movement function.
        /// </summary>
        private void Move()
        {
            //Check, if player null code wont run;
            if (player == null) return;
            if(offset != 0)
            {
                if (GameManager.Mobile)
                {
                    h = VirtualJoystick.InputDirection.x; //Mobile Horizontal Axes.
                    v = VirtualJoystick.InputDirection.y;   //Mobile Vertical Axes.
                }
                else
                {
                    direction = (_camera.ScreenToWorldPoint(Input.mousePosition) - player.transform.position).normalized;
                    h = direction.x; // mouse horizontal axes;
                    v = direction.y;// mouse vertical axes;
                }
            }
            // zoom camera;
            float zoom = Mathf.Lerp(minCameraSize, maxCameraSize, rig.velocity.magnitude / cutOff);
            float lerpSize = Mathf.Lerp(_camera.orthographicSize, zoom, smoothValue * Time.deltaTime);
            _camera.orthographicSize = lerpSize;
            // camera height;
            scrH = _camera.orthographicSize;
            // calculate camera width for correct bounds work;
            scrW = scrH * Screen.width / Screen.height;
            playerPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);// Setting Vector of Player position
            playerPosition.x += h * offset;
            playerPosition.y += v * offset;
            // clamp camera pos in bounds;
            float clampX = Mathf.Clamp(playerPosition.x, levelMaker.minimumX + (scrW + bounds[boundsOffsetIndex].minX), levelMaker.maximumX - (scrW + bounds[boundsOffsetIndex].maxX));
            float clampY = Mathf.Clamp(playerPosition.y, levelMaker.minimumY + (bounds[boundsOffsetIndex].minY+ scrH), levelMaker.maximumY - (bounds[boundsOffsetIndex].maxY+ scrH));
            playerPosition = new Vector3(clampX, clampY, transform.position.z);
            //Move camera;
            transform.position = Vector3.Lerp(transform.position, playerPosition, offsetSmoothing * Time.deltaTime);// Smooth Camera Movement

            bounderies.w = levelMaker.minimumX ;
            bounderies.x = levelMaker.maximumX ;
            bounderies.y = levelMaker.minimumY ;
            bounderies.z = levelMaker.maximumY ;

        }
    }
}