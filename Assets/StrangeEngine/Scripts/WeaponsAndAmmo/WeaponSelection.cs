using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Selects Weapon with its index. Set Active Weapon with selected index, other unselect.
/// </summary>
public class WeaponSelection : MonoBehaviour {
//
	[HideInInspector]
	public List<GameObject> models;					// list of "Weapon" GameObjects.
	[Tooltip("Index of Weapon you want to set active")]
	public int selectionIndex = 0;						// index of selected "Weapon".
    public UnityEvent onSelectWeapon;
	ActiveWeapons act;
//
	private void Start () {

		models = new List<GameObject> (); 				// set temporary list of weapons.
		foreach (Transform t in transform) 				// for every weapon in weapons.
		{
			models.Add(t.gameObject);					//parent weapon to this gameObject.
			t.gameObject.SetActive (false);				// set gameObject to inactive.
		}
	}
    /// <summary>
    ///  Public function that will select weapon with choosed index.
    /// </summary>
    /// <param name="WeaponIndex"></param>
    public void Select(int index)
	{
		if(index == selectionIndex)						// if index equal to selection index do nothing.
			return;
		if(index < 0 || index >= models.Count)			// if index value is bigger then number of weapons, do nothing.
			return;
		models[selectionIndex].SetActive(false);
		selectionIndex = index;
		models[selectionIndex].SetActive(true);
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i] != models[selectionIndex])
            {
                models[i].SetActive(false);
            }
        }
        onSelectWeapon.Invoke();
	}
}