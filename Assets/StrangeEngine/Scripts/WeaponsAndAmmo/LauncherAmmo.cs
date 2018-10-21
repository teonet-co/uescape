using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Ammo class for weapons with launcher script attached.
/// </summary>
public class LauncherAmmo : MonoBehaviour {
	Text ammoText;									// Text Component.
	[HideInInspector]
	public float ammo;								// Actual Ammo.
	[Tooltip("Start Ammo of Weapon")]
	public float startAmmo;							//Start Ammo.
//
	void Start()
	{
		ammoText = GetComponent<Text> ();			//Reference to the Text Component, attached to this gameObject.
		ammo = startAmmo;							// Set current Ammo with Start Ammo.
	}
//
	void Update()
	{
		ammoText.text = "" + ammo.ToString("f0");	// Update Text. Convert float to String and make a round number. 
	}
    /// <summary>
    /// Public function for other Scripts to use when Firing. 
    /// </summary>
    public void MinusAmmo()
	{
		ammo--;
	}
    /// <summary>
    /// Public function for other Scripts to use when you want to add Amo  with Pickups or some other stuff. 
    /// </summary>
    /// <param name="ammoToAdd"></param>
    public void addAmmo(int ammoToAdd)
	{
		ammo += ammoToAdd;
	}
    /// <summary>
    /// Public function for other Scripts to use when you want to use Flamethrower or laser.
    /// </summary>
    public void MinusLauncherAmmo()
	{
		ammo -= Time.deltaTime* 10f;
	}
}