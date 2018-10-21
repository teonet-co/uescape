using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
/// <summary>
/// Players health.
/// </summary>
public class PlayerHealth : MonoBehaviour {
    //
    [System.Serializable]
    public class PlayerEvents
    {
        public UnityEvent onPlayerDeacreaseHealth;
        public UnityEvent onPlayerEncreaseHealth;
        public UnityEvent onPlayerDies;
    }
    [Tooltip("Start health of your player ")]
	public float startHealth = 100;   						// Start Health.
	[Tooltip("Reference to filled HealthBar Image.")]
	public Image HealthImage;                               //Filled health image.
    [Tooltip("Reference to Death Text.This text will be shown when player dies.")]
    public GameObject DeathText;
    [Tooltip("Scene number to load after death.")]
    public int sceneNumber;
    public PlayerEvents playerEvents;
    [Header("Debug:")]
    [Tooltip("Actual health of your player ")]
    public float health;                                    // Actual Health.
    public string enemyBulletTag = "EnemyBullet";
    SpriteRenderer spriteRend; 								// SpriteRenderer.
//
	void Start() 
	{
		health = startHealth;								//Set health with StartHealth on Level Start.

	}
//
	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.CompareTag(enemyBulletTag)) 		// if Enemy bullet hits player.
		{
            playerEvents.onPlayerDeacreaseHealth.Invoke();
			health--; 										// Set player health minus one;
			HealthImage.fillAmount = health / startHealth;	// change fillAmount of health Image.
		}
	}
//
	void Update() 
	{
		if(health <= 0) 									// if players health less then 0 it will be Destroyed.
		{
			Die();
		}
        HealthImage.fillAmount = health / startHealth;
    }
/// <summary>
/// Die. Slows down time and show death text.
/// </summary>
	void Die() 
	{
        playerEvents.onPlayerDies.Invoke();
        try
        {
            DeathText.SetActive(true);
        }
        catch (NullReferenceException)
        {
            Debug.Log("Put 'Death Text' gameobject to 'PlayerHealth'");
        }
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;

        Invoke("LoadScene", 1f);
    }
	/// <summary>
	/// loads SelectionMenu Scene. return time to normal..
	/// </summary>
    void LoadScene()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
        SceneManager.LoadScene(sceneNumber);
    }
    /// <summary>
    ///  Public function to use it in Pickup Scripts, to get more Health.
    /// </summary>
    /// <param name="healthValue"></param>
    public void GetMoreHealth(int healthValue)
	{
		health += healthValue;
		HealthImage.fillAmount = health / startHealth;
        playerEvents.onPlayerEncreaseHealth.Invoke();
	}
    /// <summary>
    /// Public function to use it in Specials, to increase Start Health on Level Start.
    /// </summary>
    /// <param name="healthValue"></param>
    public void GetMoreStartHealth(int healthValue)
	{
		startHealth += healthValue;

	}
    public void laserDamage(float damage)
    {
        health -= damage;
        HealthImage.fillAmount = health / startHealth;
        playerEvents.onPlayerDeacreaseHealth.Invoke();
    }
}