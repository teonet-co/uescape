using UnityEngine;
/// <summary>
/// Class for Real Grenades settings.
/// </summary>
public class GrenadeReal : MonoBehaviour {
	[Tooltip("Actual Damage you want Send to Enemy")]
	public int Damage;
	[Tooltip("Actual Damage you want Send to Walls")]
	public int DamageWalls;
	[Tooltip("GrenadeChild Prefab")]
	public GameObject GrenadePrefab;
	[Tooltip("If this Grenade is Main it's going to be true, if a child it must be false.")]
	public bool MainGrenade;
	[Tooltip("Explosion Prefab")]
	public GameObject ExplosionPrefab;
	[Tooltip("Speed of GrenadeChild Prefab")]
	public float speed;
	private bool Triggered;
//
	void Start()
	{
		if (MainGrenade) {
			Triggered = false;
			if (Triggered == false) {
				Invoke ("FireGrenade", 2);
				Invoke ("D", 3);
			}
		} 
		if(MainGrenade == false) {
			Invoke ("D", 0.5f);
		}
	}
//
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag ("Enemy")) 
		{
			Triggered = true;
			other.SendMessage ("laserDamage", Damage);
			if(MainGrenade)
			FireGrenade ();
			Invoke ("D",0.2f);
		}
	}
    /// <summary>
    /// Instantiate Explosion. Destroy GameObject.
    /// </summary>
	void D()
	{
        try
        {
            Instantiate(ExplosionPrefab, transform.position, transform.rotation);
        }
        catch (UnassignedReferenceException)
        {
            Debug.Log("No Explosion Prefab Attached!");
        }
        Destroy (gameObject);
	}
    /// <summary>
    /// Instatiating Real Grenades and add them force.
    /// </summary>
    void FireGrenade()
	{
        try
        {
            Rigidbody2D GrenadeClone = ((GameObject)Instantiate(GrenadePrefab, transform.position, transform.rotation)).GetComponent<Rigidbody2D>();
            Rigidbody2D GrenadeClone1 = ((GameObject)Instantiate(GrenadePrefab, transform.position, transform.rotation)).GetComponent<Rigidbody2D>();
            Rigidbody2D GrenadeClone2 = ((GameObject)Instantiate(GrenadePrefab, transform.position, transform.rotation)).GetComponent<Rigidbody2D>();
            Rigidbody2D GrenadeClone3 = ((GameObject)Instantiate(GrenadePrefab, transform.position, transform.rotation)).GetComponent<Rigidbody2D>();
            GrenadeClone.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
            GrenadeClone1.AddForce(Vector2.down * speed, ForceMode2D.Impulse);
            GrenadeClone2.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
            GrenadeClone3.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
        }
        catch (UnassignedReferenceException)
        {
            Debug.Log("No Grenade Prefab Attached!");
        }
    }
}