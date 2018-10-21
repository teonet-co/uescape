using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Class for menu. Adds selected weapon name to playerPrefs.
/// </summary>
public class SelectionMenu : MonoBehaviour {
//
    private void Start()
    {
        PlayerPrefs.DeleteAll();
    }
    /// <summary>
    /// Writes selected weapon name to playerPrefs.
    /// </summary>
    /// <param name="weaponNumber"></param>
    public void SelectWeapon(string weaponNumber)
	{
		if (PlayerPrefs.HasKey (("weaponNumber"+ weaponNumber) ))
			PlayerPrefs.DeleteKey (("weaponNumber"+ weaponNumber));
		else
			PlayerPrefs.SetInt (("weaponNumber"+ weaponNumber), 1);
	}
    /// <summary>
    /// Writes selected controls to playerPrefs.
    /// </summary>
    public void SelectControls()
    {
        if (PlayerPrefs.HasKey(("mobile")))
            PlayerPrefs.DeleteKey(("mobile"));
        else
            PlayerPrefs.SetInt(("mobile"), 1);
    }
    /// <summary>
    /// Load Scene by scene number.
    /// </summary>
    /// <param name="SceneNumber"></param>
    public void LoadScene(int SceneNumber){
		PlayerPrefs.Save ();
		SceneManager.LoadScene (SceneNumber);
	}
    /// <summary>
    /// Exit Game.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}