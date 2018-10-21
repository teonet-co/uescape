using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Events;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]

/// <summary>
/// Class for Weapon Ammunition.
/// </summary>
public class Cartridge : MonoBehaviour {
//    
    [Tooltip("Amount of time until destroy.")]
    public float destroyTime;
    public bool enableFlash =true;
    [Tooltip("How many frames you want cartridges to blink.")]
    public int framesToFlash = 3;
    [Tooltip("Array of ammunition values. Every Element compares with elements in 'StartWeapons' in GameManager.")]
    public int[] ammoToAdd;
    public string playerTag = "Player";
    [Header("Events")]
    public UnityEvent onAddAmmo;
    private int ammo;                                       // Amout of Ammunition that you want to add to players weapon.
    private SpriteRenderer spriteRend;	        
	ActiveWeapons active;
	int i;                                                  // index of active Weapon in Active Weapons Array.
//
	void Awake()
	{
        try
        {
            active = FindObjectOfType<ActiveWeapons>(); // Referens to ActiveWeapons.
        }
        catch (NullReferenceException)
        {
            Debug.Log("There is no Active 'ActiveWeapons' panel in the Scene.");
        }
		Destroy (gameObject, destroyTime);                  // Destroy this gameObject after destroyTime.
        spriteRend = GetComponent<SpriteRenderer> ();       // Reference to gameObjects SpriteRenderer.
        try
        {
      // shoose Active Weapon randomly.
            i = UnityEngine.Random.Range(0, active.activeWeapons.Count);
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.Log("Could not add Ammo. You should have more than one weapon on weapon panel. Argument is out of Range.");
        }
        // Add different Ammo depending on 'Weapon' GameObject name.
        try
        {
            for (int k = 0; k < active.Weapons.Count; k++)
            {
                if(active.Weapons[k]== active.activeWeapons[i])
                {
                    onAddAmmo.Invoke();
                    ammo = ammoToAdd[k];
                }
            } 
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.Log("Could not add Ammo. You should have more than one weapon on weapon panel. Argument is out of Range.");
        }
    }
// Starts blinking after 3 seconds.
	IEnumerator Start()
	{
        if (enableFlash)
        {
            yield return new WaitForSeconds(3f);
            StartCoroutine(FlashMuzzleFlash());
        }
	}
/// <summary>
/// Blinking
/// </summary>
/// <returns></returns>
    IEnumerator FlashMuzzleFlash()
	{
		while(spriteRend.flipX)
        {
			spriteRend.flipX = false;
		for(int i = 0; i < framesToFlash; i++)
		    {
			yield return 0;
		    }
			spriteRend.flipX = false;
			for(int i = 0; i < framesToFlash; i++)
			{
				yield return 0;
			}
            break;
        }
	}
 // when trigger enters player, send some ammo to players weapon.
		void OnTriggerEnter2D(Collider2D col)
		{
			if(col.CompareTag(playerTag))
			{
            try
            {
                active.activeWeapons[i].BroadcastMessage("addAmmo", ammo);
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.Log("Could not add Ammo. You should have more than one weapon on weapon panel. Argument is out of Range.");
            }
            Destroy (gameObject);
			}
		}
	}