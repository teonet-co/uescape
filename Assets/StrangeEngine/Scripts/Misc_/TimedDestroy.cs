using UnityEngine;
/// <summary>
/// Destroy GameObject with delay.
/// </summary>
public class TimedDestroy : MonoBehaviour
{
	[Tooltip("Seconds until Destroy")]
	public float delay;
//
	void Start()
	{
		Destroy (gameObject, delay); // Destroy with Delay
	}
}