using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
/// <summary>
/// count up number of checked toggles, and don't let to check more then you need.
/// </summary>
public class MyToggleGroup : MonoBehaviour
{
	[Tooltip("toggles array")]
	public List<Toggle> toggles;        
	[Tooltip("Maximum active toggles")]
	public int maxSelected = 2;     
	private int _countSelected;     	// count of selected toggles.
//
	void Start()
	{
		foreach (Toggle t in toggles) 	// add to every toggle a method whrn value is changed
		{
			t.onValueChanged.AddListener((x) => togglesChanged(t));
		}

		correctSelected(); 				// correct togles
	}
    /// <summary>
    /// function works every time you change toggle value and correct number of checked toggles.
    /// </summary>
    /// <param name="t"></param>
    private void togglesChanged(Toggle t)
	{
		if (t.isOn) _countSelected++;
		else _countSelected--;

		foreach (Toggle togg in toggles)
		{
			if (_countSelected == maxSelected) togg.interactable = togg.isOn;
			else togg.interactable = true;
		}
	}
    /// <summary>
    /// this method count up number of checked toggles, and don't let to check more then you need.
    /// </summary>
    private void correctSelected()
	{
		_countSelected = 0;
		foreach (Toggle t in toggles)
		{
			if (t.isOn) _countSelected++;
			if (_countSelected > maxSelected)
			{
				t.isOn = false;
				t.interactable = false;
			}
		}
		_countSelected = Mathf.Clamp(_countSelected, 0, maxSelected);
	}
}