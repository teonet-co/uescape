using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/// <summary>
/// Text Starts bouncing, then goes up direction, then dissapere.
/// </summary>
public class TextFadeInOut : MonoBehaviour
{
    //
    [Tooltip("Speed of Vertical movement.")]
    public float verticalSpeed;                                                    // Speed of Vertical movement.
    [Tooltip("Speed of Second Bounce.")]
    public float bounceSpeed1;                                                     //Speed of Second Bounce.
    [Tooltip("Time until destroy.")]
    public float destroyTime;
    [Tooltip("How long blinking text.")]
    public float FramesToFlash;                                                    // How long blinking text.
    [Tooltip("Speed of First Bounce.")]
    public float BounceSpeed2;                                                     // Speed of First Bounce.
    [Tooltip("only 4 fonts can be added, other will be ignored.")]
	public Font[] fonts;
    float elapsedTime;
    //
    void Start()
	{
		elapsedTime = 0f;
	}
//
	void Update()
	{
		elapsedTime += Time.deltaTime;                                              // counting elapsed time. 
		if (0.1f >= elapsedTime)
        {			
			transform.position += Vector3.up * BounceSpeed2 * Time.deltaTime;       // text goes up
		}
		if (0.2f >= elapsedTime && elapsedTime >= 0.1f)
        {
			transform.position += Vector3.down * BounceSpeed2 * Time.deltaTime;     // text goes down
        }
		if (0.3f >= elapsedTime && elapsedTime >= 0.2f)
        {
			transform.position += Vector3.up * bounceSpeed1 * Time.deltaTime;       // text goes up
        }
		if (0.4f >= elapsedTime && elapsedTime >= 0.3f)
        {
			transform.position += Vector3.down * bounceSpeed1 * Time.deltaTime;     // text goes down
        }
		if (3f >= elapsedTime && elapsedTime >= 0.4f)
        {
			transform.position += Vector3.up * verticalSpeed * Time.deltaTime;      // text goes up
        }
		StartCoroutine (FadeTextToZeroAlpha(GetComponent<Text>(),FramesToFlash));   // Starts Flashing fonts.
		Destroy (gameObject, destroyTime);                                          // Destroy
	}
    /// <summary>
    /// Changes fonts and there colors.
    /// </summary>
    /// <param name="Font"></param>
    /// <param name="Frames to Flash"></param>
    /// <returns>null</returns>
	public IEnumerator FadeTextToZeroAlpha(Text i, float d)
	{
		while (true)
		{
			i.font = fonts[0];
			i.color = new Color (255, 255, 0);
			i.fontSize = 2;			
		for(int f = 0; f < d; f++)
		{
			yield return 0;
		}		
		i.font = fonts[1];
			i.color = new Color (255, 255, 255);
			i.fontSize = 2;
		for(int f = 0; f < d; f++)
		{
			yield return 0;
		}				
			i.font = fonts[2];
			i.color = new Color (255, 255, 255);
			i.fontSize = 2;
		for(int f = 0; f < d; f++)
		{
			yield return 0;
		}				
			i.font = fonts[3];
			i.color = new Color (255,255, 255);
			i.fontSize = 2;
		for(int f = 0; f < d; f++)
		{
			yield return 0;
		}
			yield return null;
		}
	}
}