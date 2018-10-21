using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// When Player Enters triggered area, openes Weapon Selection Scene.
/// </summary>
public class FirePortal : MonoBehaviour {
//
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player")) {
			SceneManager.LoadScene (0);
		}
	}
}