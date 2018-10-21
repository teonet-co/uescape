	using UnityEngine;
	using UnityEngine.UI;
/// <summary>
/// Save and Load Score value.
/// </summary>
	public class ScoreManager : MonoBehaviour
	{
	[Tooltip("Player's Score")]	
	public static int score;        // The player's score.
    Text text;                      // Reference to the Text component.
//
		void Awake ()
		{
			// Set up the reference.
			text = GetComponent <Text> ();
			// Reset the score.
		score = PlayerPrefs.GetInt("Score", 0);;
		}
//
		void Update ()
		{
			// Set the displayed text to be the word "Score" followed by the score value.
			text.text = "Score: " + score;
		}
/// <summary>
/// Save Score
/// </summary>
	public void SaveScore ()
	    {
		PlayerPrefs.SetInt("Score", score); // Save Players Score to PlayerPrefs.
	    }
/// <summary>
/// Load Score
/// </summary>
	public void LoadScore ()
	    {
		PlayerPrefs.GetInt("Score", 0); // Load Players Score to PlayerPrefs.
	    }
    }