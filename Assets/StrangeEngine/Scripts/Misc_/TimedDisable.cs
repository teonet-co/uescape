using UnityEngine;

public class TimedDisable : MonoBehaviour {
    [Tooltip("time in seconds. Disable this gameobject after amount of time")]
    public float time;

    private void OnEnable()
    {
        Invoke("disable", time);
    }
    void disable()
    {
        gameObject.SetActive(false);
    }
}
