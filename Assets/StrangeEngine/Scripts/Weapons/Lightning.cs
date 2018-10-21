using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class Lightning : MonoBehaviour {
	
	//Public inspector properties
	public Transform EndPoint;	    		//Where the lightning effect terminates
	public int NumberOfSegments;  		//Number of line segments to use. Higher numbers make a smoother line (but may impact performance)
	public float Randomness;		//Randomness (in world units) of each segment of the lightning chain
	public float Radius;				//Arc radius.
	public float Duration;				//Duration of the effect. Set this to 0 to make it stay permanently
	public float FrameRate;			//Number of frames per second at which it should animate. Set to 0 to not animate at all.
	public bool FadeOut;				//Should it fade out over time?
	public Color StartColor = Color.white;	//Color of the line at the start
	public Color EndColor = Color.white;	//Color of the line at the end
    public UnityEvent onLightningDuration;
	//private members
	private Vector2 midPoint;
	private float maxZ;
	private float timer = 0f;
	private LineRenderer lineRenderer;
	private GameObject endGameObject;
	private int vertCount = 0;

	void Start () 
	{
		//set up the line renderer
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.useWorldSpace = false;
        lineRenderer.startColor = StartColor;
        lineRenderer.endColor = EndColor;

		//If there is no EndPoint set, create one at 0,0,0
		if (EndPoint == null)
		{
			endGameObject = new GameObject("EndPoint");
			EndPoint = endGameObject.transform;
		}
		transform.LookAt(EndPoint);

		//If we have a frame rate greater than 0, we will animate the line over time.
		// Otherwise, just render it once.
		if (FrameRate > 0)
		{
			StartCoroutine(AnimateLightning());
		}
		else
		{
			RenderLightning();
		}
	}

	/// <summary>
	/// Renders the lightning.
	/// </summary>
	public void RenderLightning()
	{
		//If the number of segments has changed, update the line renderer
		if (vertCount != NumberOfSegments)
		{
			lineRenderer.positionCount =NumberOfSegments;
			vertCount = NumberOfSegments;
		}

		//grab a random point inside a circle for the midpoint our arc.
		midPoint = Random.insideUnitCircle * Radius;

        //get the total length of the line
        if (EndPoint)
        {
            maxZ = (EndPoint.position - transform.position).magnitude;
        }
        //interpolate each point of the line over an arc.
        for (int i=1; i < NumberOfSegments-1; i++)
		{
//			float z = 0;
			float z =((float)i)*(maxZ)/(float)(NumberOfSegments-1);
			
			float x = -midPoint.x*z*z/(2* maxZ) + z*midPoint.x/2f;
			
			float y = -midPoint.y*z*z/(2* maxZ) + z*midPoint.y/2f;

			//set the position, and add some randomness for a nice jaggy lightning effect.
			lineRenderer.SetPosition(i, new Vector3(x + Random.Range(Randomness,-Randomness) ,y + Random.Range(Randomness,-Randomness),z));
		}

		//set the first and last position
		lineRenderer.SetPosition(0, Vector3.zero);
		lineRenderer.SetPosition(NumberOfSegments-1, new Vector3(0,0,maxZ));
        onLightningDuration.Invoke();
	}

	public IEnumerator AnimateLightning()
	{
		while (FrameRate > 0)
		{
			RenderLightning();

			yield return new WaitForSeconds(1f/FrameRate);
		}
	}

	void Update () {

		//Keep it pointed at the EndPoint
		transform.LookAt(EndPoint);

        //Update the color in case we are fading (or you change it in the inspector)
        lineRenderer.startColor = StartColor;
        lineRenderer.endColor = EndColor;

        if (timer < Duration)
		{
			timer += Time.deltaTime;	//keep out timer ticking up

			if (FadeOut)
			{
				//smoothly lerp the colors alpha value toward 0
				StartColor.a = Mathf.Lerp(StartColor.a, 0f, timer / Duration);
				EndColor.a = Mathf.Lerp(EndColor.a, 0f, timer / Duration);
			}

			//if the duration has run out, kill the effect.
			if (timer >= Duration)
			{
				//if we had spawned our own end point, clean it up here.
				if (endGameObject != null)
				{
					Destroy(endGameObject);
				}
				Destroy(gameObject);
			}
		}
	}

}