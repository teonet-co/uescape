using UnityEngine;

public class AddHealth : MonoBehaviour {
    public int healthValue;
    public string playerTag = "Player";
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            collision.gameObject.GetComponent<PlayerHealth>().GetMoreHealth(healthValue);
            Destroy(gameObject);
        }
    }
}
