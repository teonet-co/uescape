using UnityEngine;
using System;
using System.Collections.Generic;
/// <summary>
/// Activate Weapon button if it has Key in PlayerPrefs, and delete that key to avoid errors.
/// </summary>
public class StartWeapons : MonoBehaviour {
//
	[Tooltip("Put all weapon buttons here")]
	public List<GameObject> Weapons;
//
	void Start ()
    {
        //PlayerPrefs.DeleteAll();
        try
        {
            // Deactivate all GameObjects in array.
			for (int i = 0; i < Weapons.Count-1; i++)
            {
                Weapons[i].SetActive(false);
            }
            // Activate Weapon if it has Key in PlayerPrefs, and delete that key to avoid errors.
            for (int i = 1; i < Weapons.Count+2; i++)
            {
                if (PlayerPrefs.HasKey("weaponNumber" + (i)))
                {
                    Weapons[i-1].SetActive(true);
                        PlayerPrefs.DeleteKey("weaponNumber"+(i));
                        Debug.Log("Weapon "+i+ " Loaded!");
                }
            }
        }
        catch (NullReferenceException)
        {
            Debug.Log("Not all weapons set in StartWeapons!");
        }
    }
}