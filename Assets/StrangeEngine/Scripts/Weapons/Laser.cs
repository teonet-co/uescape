using UnityEngine;
//
public class Laser : MonoBehaviour {
    [Header("ONLY FOR DEBUG.(All references comes from LaserLauncher.)")]
    [Space(10)]
    [Tooltip("enemy and walls layers.")]
    public LayerMask layers;
    [Tooltip("laser prefab.")]
    public GameObject laser;
    [Tooltip("how fast laser will get wider.")]
    public float widthSpeed;
    [Tooltip("how fast line will be drawn.")]
    public float lineDrawSpeed;
    //
    public Transform startposition; // this position
    public float startWidth; // start width of linerender
    public bool shoot; // if true start shooting;
    public bool en;// if true Start change laser width.
    public bool le; // if true smoothly encrease laser length.
    public float nextFire = 0.0F;
    public bool readyToShoot; // if ready to shoot ,shoot.
    public LineRenderer l; // prefabs linerenderer
    public float dist; // distance.
    public Vector3 endPosition;//
    public Collider2D hitCollider;//
    private float counter;
//
    void Start ()
    {
        l = GetComponent<LineRenderer>(); // get prefabs linerenderer.
    }
//
    private void OnEnable()
    {
        l = GetComponent<LineRenderer>(); // get prefabs linerenderer.
        l.SetPosition(0, startposition.position); // set start position.
        l.SetPosition(1, startposition.position); // set start position.
        l.widthMultiplier = startWidth; // set start width of linerenderer.
        le = true;
        if(hitCollider != null)
        {
            endPosition = hitCollider.transform.position;
        }
        else
        {
            endPosition = Vector3.up;
        }        
    }
//
    private void OnDisable()
    {
        le = false;
        en = false;
    }
//
    void Update ()
    {
        if (le)
        {
           changeLength();
        }
        if (en)
        {
            changeWidth();
        }
    }
    /// <summary>
    /// Smoothly Changes Linerenderers Length.
    /// </summary>
    private void changeLength()
    {
        //print("1!");
        if (hitCollider == null)
        {
            changeWidth();
        }
        if (hitCollider != null)
        {
            dist = Vector3.Distance(startposition.position, endPosition);
        }
        if (hitCollider == null)
        {
            dist = 200;
        }
        if (counter < dist)
        {
            //print("2!");
            counter += 0.1f / lineDrawSpeed;
            float x = Mathf.Lerp(0, dist, counter);
            Vector3 pointA = startposition.position;
            Vector3 pointB = endPosition;
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            l.SetPosition(1, pointAlongLine);
            if (Vector3.Distance(pointAlongLine, endPosition) < 5)
            {
                counter = 0;
                en = true;
                le = false;
                //print("3!");
            }
        }
    }
    /// <summary>
    /// Smoothly changes linerenderers width.
    /// </summary>
    void changeWidth()
    {
        if (l.widthMultiplier >= 0)
        {
            l.widthMultiplier -= Time.deltaTime * widthSpeed;
            if (l.widthMultiplier <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
