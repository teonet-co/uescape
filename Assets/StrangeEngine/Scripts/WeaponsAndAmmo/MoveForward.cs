using UnityEngine;
using System.Collections;
using UnityEngine.Events;
/// <summary>
/// Bullets movement and muzzle flash on fire.
/// </summary>
public class MoveForward : MonoBehaviour {
//
	[Tooltip("Maximum bullet speed.")]
	public float maxSpeed = 5f;
    public bool enableFlash = true;
    [Tooltip("Muzzle Flash Sprite")]
	public Sprite muzzleFlash;
	[Tooltip("How many frames Muzzle Flash will flash.")]
	public int framesToFlash = 3;
	[Tooltip("Destroy Time.")]
	public float destroyTime = 3;
    public bool addForce = true;
    [Header("Events")]
    public UnityEvent onBulletMovement;
//
	private SpriteRenderer spriteRend;
	private Sprite defaultSprite;
//
	void Awake()
	{
		spriteRend = GetComponent<SpriteRenderer>(); // Reference to SpriteRenderer
		defaultSprite = spriteRend.sprite;
    }
    private void OnEnable()
    {
        try
        {
            if(enableFlash)
            StartCoroutine(FlashMuzzleFlash());
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("bad happend");
        }
        
        Invoke("destroy", destroyTime);
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
    /// <summary>
    /// Shows muzzleFlash sprite, then waits some frames and changes to default sprite. 
    /// </summary>
    /// <returns></returns>
    IEnumerator FlashMuzzleFlash()
	{
        try
        {
            spriteRend.sprite = muzzleFlash;
        }
        catch (System.NullReferenceException)
        {
            
            Debug.Log("no sprite or spriterenderer");
        }       
		for(int i = 0; i < framesToFlash; i++)
		{
			yield return 0;
		}
		spriteRend.sprite = defaultSprite;
	}
// Bullet Movement
	void FixedUpdate () 
	{
        if (addForce)
        {
            Vector3 pos = transform.position;// Start position = gun position		
            Vector3 velocity = new Vector3(0, maxSpeed * Time.deltaTime, 0); // velocity over lifetime.		
            pos += transform.rotation * velocity;
            transform.position = pos;
            onBulletMovement.Invoke();
        }
	}
    void destroy()
    {
        StopCoroutine(FlashMuzzleFlash());
        gameObject.SetActive(false);
    }
}