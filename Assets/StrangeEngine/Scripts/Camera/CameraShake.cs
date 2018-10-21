using UnityEngine;
using System.Collections;
namespace StrangeEngine
{
    /// <summary>
    /// Class for Camera Shake.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        [Tooltip("How long camera will shake")]
        public float shakeTimer;
        [Tooltip("Strength of camera shake")]
        public float shakeAmount;
        
        IEnumerator currentShakeCoroutine; // temp current shake;
        private void Awake()
        {
            shakeTimer = -0.1f; // reset timer;
        }
        void Update()
        {
            if (shakeTimer >= 0)
            {
                Vector2 ShakePos = Random.insideUnitCircle * shakeAmount;           // Setting a Shake Position
                transform.position = new Vector3(transform.position.x + ShakePos.x, transform.position.y + ShakePos.y, transform.position.z);//Camera position plus Shake position
                shakeTimer -= Time.deltaTime;                                       // duration timer count down. 
            }
        }
        /// <summary>
        /// Camera Shake function;
        /// </summary>
        /// <param name="properties"></param>
        public void StartShake(Properties properties)
        {
            //if there is shake runing, stop this shake;
            if (currentShakeCoroutine != null)
            {
                StopCoroutine(currentShakeCoroutine);
            }
            //reference to Shake properties;
            currentShakeCoroutine = Shake(properties);
            // Start Shake;
            StartCoroutine(currentShakeCoroutine);
        }
        /// <summary>
        /// Camera Shake function with sine movement;
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        IEnumerator Shake(Properties properties)
        {
            float completionPercent = 0;
            float movePercent = 0;
            float angle_radians = properties.angle * Mathf.Deg2Rad - Mathf.PI;
            Vector3 previousWaypoint = transform.localPosition;
            Vector3 currentWaypoint = transform.localPosition;
            float moveDistance = 0;
            float speed = 0;
            do
            {
                if (movePercent >= 1 || completionPercent == 0)
                {
                    float dampingFactor = DampingCurve(completionPercent, properties.dampingPercent);
                    float noiseAngle = (Random.value - .5f) * Mathf.PI;
                    angle_radians += Mathf.PI + noiseAngle * properties.noisePercent;
                    previousWaypoint = transform.localPosition;
                    currentWaypoint = (new Vector3(Mathf.Cos(angle_radians), Mathf.Sin(angle_radians)) * properties.strength * dampingFactor) + previousWaypoint;
                    moveDistance = Vector3.Distance(currentWaypoint, previousWaypoint);
                    speed = Mathf.Lerp(properties.minSpeed, properties.maxSpeed, dampingFactor);
                    movePercent = 0;
                }
                completionPercent += Time.deltaTime / properties.duration;
                movePercent += Time.deltaTime / moveDistance * speed;
                transform.localPosition = Vector3.Lerp(previousWaypoint, currentWaypoint, movePercent);
                yield return null;
            } while (moveDistance > 0);
        }
        /// <summary>
        /// Public function to use Camera Shake in other scripts
        /// </summary>
        /// <param name="Shake Power"></param>
        /// <param name="Shake Duration"></param>
        public void ShakeCamera(float shakepwr, float shakeDur)
        {
            shakeAmount = shakepwr;
            shakeTimer = shakeDur;
        }
        float DampingCurve(float x, float dampingPercent)
        {
            x = Mathf.Clamp01(x);
            float a = Mathf.Lerp(2, .25f, dampingPercent);
            float b = 1 - Mathf.Pow(x, a);
            return b * b * b;
        }
        [System.Serializable]
        public class Properties
        {
            [Tooltip("Shake Angle.")]
            public float angle;
            [Tooltip("Shake Strength.")]
            public float strength;
            [Tooltip("Shake Maximum speed.")]
            public float maxSpeed;
            [Tooltip("Shake minimum speed.")]
            public float minSpeed;
            [Tooltip("Shake Duration.")]
            public float duration;
            [Tooltip("Percent of noise.")]
            [Range(0, 1)]
            public float noisePercent;
            [Range(0, 1)]
            [Tooltip("Damping percent.")]
            public float dampingPercent;

            public Properties(float angle, float strength, float speed, float duration, float noisePercent, float dampingPercent, float rotationPercent)
            {
                this.angle = angle;
                this.strength = strength;
                this.maxSpeed = speed;
                this.duration = duration;
                this.noisePercent = Mathf.Clamp01(noisePercent);
                this.dampingPercent = Mathf.Clamp01(dampingPercent);
            }
        }
    }
}