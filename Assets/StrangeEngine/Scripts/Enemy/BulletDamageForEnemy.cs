using UnityEngine;
/// <summary>
/// Enemy takes Damage.
/// </summary>
public class BulletDamageForEnemy : MonoBehaviour {
    // On Collision with player or walls deactivate bullet bullet
    public string playerTag = "Player";
    public string wallsTag = "Walls";
    public string enemyBulletTag = "EnemyBullet";
    void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.CompareTag(playerTag))
        {
            gameObject.SetActive(false);		
		}
        if (coll.gameObject.CompareTag(wallsTag))
        {
            gameObject.SetActive(false);
        }
        if (coll.gameObject.CompareTag(enemyBulletTag))
        {
			Physics2D.IgnoreLayerCollision (14, 14, ignore: true);
		}       
    }
}