using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
public class PlayersPath : MonoBehaviour {

    public Queue<GameObject> tir; // new Queue;
    [Tooltip("Prefab you want to use in queue.")]
    public GameObject pref;
    [Tooltip("time in seconds to activate another prefab.")]
    public float time;
    [Tooltip("Activate Players path.")]
    public bool enablePath= true;
    [Tooltip("maximum elements in queue")]
    public int queueMax;
    [Tooltip("don't touch.")]
    public List<GameObject> pointers;
    private int y = 0;
    private GameObject playersPath; // Temp Parent Path;
    // Use this for initialization
    void Start ()
    {
        tir = new Queue<GameObject>(queueMax);        // new queue with queueMax size;
        pointers = new List<GameObject>(queueMax); // new List with queueMax size;
        playersPath = new GameObject("Players Path");
        // Object Pooling;
        for (int i = 0; i < queueMax; i++)
        {
            GameObject tObj = Instantiate(pref, transform.position, transform.rotation);
            tObj.transform.parent = playersPath.transform;
            tObj.SetActive(false);
            pointers.Add(tObj);
        }        
        StartCoroutine(startPath(time));// Start Loop threw all prefabs in queue and enable or disable them;
    }	
	// Update is called once per frame
	void Update ()
    {
        // disable first prefab in queue;
        if (tir.Count == queueMax)
        {
            tir.Dequeue().SetActive(false);
        }        
	}
    public IEnumerator startPath(float time)
    {
        while (enablePath)
        {
            yield return new WaitForSeconds(time); // wait amount of time
            tir.Enqueue(pointers[y]); // add to queue prefab;
            pointers[y].transform.position = transform.position; // set new position;
            pointers[y].SetActive(true); // set prefab active;
            y++; // go forward;
            // if last prefab activates, and its number equals to queueMax, restart;
            if (y == queueMax)
            {
                y = 0;
            }
        }              
    }
}
