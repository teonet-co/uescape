using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SpriteChanger : MonoBehaviour
{
    [Tooltip("When player Enters this trigger, spriterenderer will change sprite to this sprite.")]
    public Sprite change;
    [Header("Spawn Options.")]
    [Tooltip("Radius of spawn area.")]
    public float radius;
    [Tooltip("Prefabs to spawn.")]
    public List<GameObject> prefabs;
    [Tooltip("Layers,you want to avoid, when span prefabs.")]
    public LayerMask layers;
    public string playerTag = "Player";
    [Header("Events")]
    public UnityEvent onObjectSpawn;
    private Vector2 pos; // temp position.
    private Vector2 posOffset;
    private void Start()
    {
        var grid = FindObjectOfType<Grid>();
        if(grid)
            posOffset =  grid.cellSize / 2;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(playerTag))
        {
            gameObject.GetComponent<Collider2D>().enabled = false; // disables collider;
            gameObject.GetComponent<SpriteRenderer>().sprite = change; // changes sprite;
            Vector3 tempPos = Vector3.zero; // temp position;
            pos = transform.position;
            int breakLoop = 0; // int to exit loop, if something went wrong;
            for (int i = 0; i < prefabs.Count; i++) // loop for amount of objects to spawn;
            {
                int result = 1; // temp result;
                while (result > 0 || breakLoop < 40) // if there will be no objects(on choosed Layers) near temp positions loop will break || breakeloop int will be higher than 40;
                {
                    tempPos = UnityEngine.Random.insideUnitCircle * radius + pos; // start searching for random point around choosed position;
                    result = Physics2D.OverlapCircleNonAlloc(tempPos, 10, new Collider2D[2], layers); // searching for objects around;
                    breakLoop++; // 
                }
                if (breakLoop >= 40)
                {
                    //print("no positions found for objectSpawner! Check your preferences!");
                }
                pos = tempPos; // 
                onObjectSpawn.Invoke();
                Instantiate(prefabs[i], pos+posOffset, Quaternion.identity); // instantiate Object to Spawn;
            }
        }
    }
}
