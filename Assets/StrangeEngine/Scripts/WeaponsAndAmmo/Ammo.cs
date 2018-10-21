using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Ammunition Settings.
/// </summary>
public class Ammo : MonoBehaviour {
//
	Text ammoText;									// Text Component.
	[HideInInspector]			
	public int ammo;								// Actual Ammo.
	[Tooltip("Start Ammo of Weapon")]
	public int startAmmo;							//Start Ammo
//
	void Start()
	{
		ammoText = GetComponent<Text> ();   		//Reference to the Text Component, attached to this gameObject.
		ammo = startAmmo;							// Set current Ammo with Start Ammo.
	}
//
	void Update()
	{
		ammoText.text = "" + ammo;					// Update Text.
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
}