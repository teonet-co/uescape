using UnityEngine;
using System;
/// <summary>
/// Instantiate shell prefab on every shot. Good for firearm.
/// </summary>
public class ShellScript : MonoBehaviour {
	[Tooltip("Time to Destroy gameObject.")]
	public float timer = 1f;
	[Tooltip("Time to Disable Rigidbody2D and BoxCollider2D components.")]
	public float Disabletimer;
	Rigidbody2D rb;
	BoxCollider2D bc;
    private float timer1;
    private float timer2;
//
	void Awake()
	{
		rb = GetComponent<Rigidbody2D> (); //Reference to Rigidbody2D component.
		bc = GetComponent<BoxCollider2D> (); // Reference to BoxCollider2D component.
        timer1 = timer;
        timer2 = Disabletimer;
    }
//
	void Update () 
	{
		timer1 -= Time.deltaTime;
		timer2 -= Time.deltaTime;
		if(timer2 <= 0) // I Destroy those components, because i don'd want them to collide with other gameobjects in scene after Amount of time. Looks nice and Perfomance is much better.
		{
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            rb.freezeRotation = true;
            bc.enabled = false;
		}
		if(timer1 <= 0) {
            gameObject.SetActive(false);
            timer1 = timer;
            timer2 = Disabletimer;
        }
	}
}