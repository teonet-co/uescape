using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Choose active weapon buttons and put them into array.
/// </summary>
public class ActiveWeapons : MonoBehaviour {
	[Tooltip("Put Here all your Weapon buttons")]
	public List<GameObject> Weapons;  					// Weapon Buttons.
	[Tooltip("Temporary list of Weapon buttons")]
	public List<GameObject> activeWeapons;			// Temporary List, that fills with active "weapon buttons" in the Start of the Level.
	private WeaponSelection sel;
//
	void Start () 
	{
        try
        {
            activeWeapons = new List<GameObject>();     // Set Temporary List for Active Weapon Buttons. 
                                                        // Cycles through all of the Weapons.
            foreach (GameObject weapon in Weapons)
            {
                // Checks if the current weapon is active, if it is, then it adds it to a temporary list.
                if (weapon.activeInHierarchy)
                {
                    activeWeapons.Add(weapon);
                }
            }
        }
        catch (NullReferenceException)
        {
            Debug.Log("Not all weapons set in ActiveWeapons!");
        }
		sel = FindObjectOfType<WeaponSelection> ();
		if (activeWeapons.Count > 0) {
			for (int i = 0; i < sel.models.Count; i++) {
				if (sel.models [i].name == activeWeapons [0].name) {
					sel.models [i].SetActive (true);
				}
			}
		}
	}
}