using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class TNT : MonoBehaviour {
    public GameObject AnimationPrefab;
    public List<string> Tags = new List<string>(1) { "Bullet" };
    public float health;
    public int framesToFlash;
    public Sprite flashSprite;
    [Header("Events")]
    public UnityEvent onCollision;
    public UnityEvent onDestroy;
//
    private SpriteRenderer spriteRend;
    private Sprite defaultSprite;
//
    private void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRend.sprite;
    }
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (tag.Contains(coll.gameObject.tag))
        {
            health--;
            onCollision.Invoke();
            if(flashSprite)
            StartCoroutine(FlashImpactFlash());
            if (health <= 0)
            {
                if(AnimationPrefab)
                Instantiate(AnimationPrefab, transform.position, transform.rotation);
                onDestroy.Invoke();
                Destroy(gameObject);
            }
        } 
    }
    /// <summary>
    /// This function changes sprite to Impact Flash sprite, waites and then change to default sprite.
    /// </summary>
    /// <returns></returns>
    IEnumerator FlashImpactFlash()
    {
        spriteRend.sprite = flashSprite;
        for (int i = 0; i < framesToFlash; i++)
        {
            yield return 0;
        }
        spriteRend.sprite = defaultSprite;
    }
}
