using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Class for Menus, Panels. Disabling and Enabling GameObjects(Panels, Butttons etc.)
/// </summary>
public class ButtonController : MonoBehaviour {
	[Tooltip("Panel you want to Open or Close")]
	public GameObject menuPanel;					// Panel that will Open and Close
/// <summary>
/// Public function that Opens Panel and pause game
/// </summary>
	public void OpenShop()
	{
        if (menuPanel)
        {
            menuPanel.SetActive(true);
            Time.timeScale = 0;
        }
        if (!menuPanel)
            Debug.Log("Please add Panel you want to Open or Close");

    }
    /// <summary>
    /// Public function that Close Panel and resume game
    /// </summary>
    public void CloseShop()
	{
        if (menuPanel)
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1;
        }
        if (!menuPanel)
            Debug.Log("Please add Panel you want to Open or Close");
    }
    /// <summary>
    /// Exit Game.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
    /// <summary>
    /// Load Scene by scene number.
    /// </summary>
    /// <param name="SceneNumber"></param>
    public void LoadScene(int SceneNumber)
    {
		Time.timeScale = 1;
		PlayerPrefs.Save();
        SceneManager.LoadScene(SceneNumber);
    }
}