using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Light))]
/// <summary>
/// Randomly changes light Intensity and Range.
/// </summary>
public class LightFlicker : MonoBehaviour {
//
    [Tooltip("Minimum light Intensity.")]
    public float minIntensity = 0.25f;
    [Tooltip("Maximum light Intensity.")]
    public float maxIntensity = 0.5f;
    [Tooltip("Minimum light Range.")]
    public float minRange = 0.25f;
    [Tooltip("Maximum light Range.")]
    public float maxRange = 0.5f;
    [Tooltip("Point Light.")]
    public Light point;
    public bool startFlicker;
//
	float random;
//
	void Start()
	{		
		random = Random.Range(0.0f, 65535.0f); // some random value.
	}
//
	void Update()
	{
        if (startFlicker)
        {
            float noise = Mathf.PerlinNoise(random, Time.time); // Random noise.
            point.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);// changing Intensity.
            point.range = Mathf.Lerp(minRange, maxRange, noise); //changing range.
        }
	}
    public void Blink(float delay)
    {
        StopAllCoroutines();
        StartCoroutine(startBlinking(delay));
    }
    private IEnumerator startBlinking(float delay)
    {
        point.enabled = true;
        yield return new WaitForSeconds(delay);
        point.enabled = false;
    } 
}