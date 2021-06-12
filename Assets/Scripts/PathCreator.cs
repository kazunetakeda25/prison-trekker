using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using SWS;

public class PathCreator : MonoBehaviour
{
    public static PathCreator instance;
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate { };
    GameObject walker;
    public bool candraw;
    string ParkedCar;
    LineRenderer line;
    PathManager EscapePath;
    public bool Ready;

    int CarParked;

   
    bool PathCreated = false;
    [Header("Paint")]
    public GameObject Paint;

    

    string getName,getCarName;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        walker = GameObject.FindGameObjectWithTag("Player");
         candraw = false;
        Ready = false;


    }

    // Update is called once per frame
    void Update()
    {
        if (!PathCreated) {
            if (Input.GetMouseButtonDown(0))
            {
                points.Clear();
                Ray rayCheck = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit HitInfoCar;
                if (Physics.Raycast(rayCheck, out HitInfoCar)) {

                    if (HitInfoCar.collider.gameObject.CompareTag("Stay"))
                    {
                      getName = (HitInfoCar.collider.gameObject.name);

                        Paint.SetActive(true);
                            candraw = true;
                           
                        

                    }
                  
                }
            }


            if (candraw && (Input.GetMouseButton(0)))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit HitInfo;
                if (Physics.Raycast(ray, out HitInfo))
                {
                    if (DistanceToLastPoint(HitInfo.point) > 0.5f)
                    {

                        points.Add(HitInfo.point);
               

                    }


                }


            }
            else if (candraw && (Input.GetMouseButtonUp(0)))
            {
                DrawPathForRed();
                candraw = false;
                Paint.SetActive(false);

            }

    }
}
    private float DistanceToLastPoint(Vector3 point) {

        if (!points.Any())
            return Mathf.Infinity;
        return Vector3.Distance(points.Last(), point);

    }

    void DrawPathForRed()
    {

       
            //create path manager game object
            GameObject newPath = new GameObject(getName+"Path");
            PathManager EscapePath = newPath.AddComponent<PathManager>();

            //declare waypoint positions
            Vector3[] positions = points.ToArray();
            Transform[] waypoints = new Transform[positions.Length];

            //instantiate waypoints
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject newPoint = new GameObject("Waypoint " + i);
                waypoints[i] = newPoint.transform;
                waypoints[i].position = positions[i];
            }

        //assign waypoints to path
                EscapePath.Create(waypoints, true);
                Ready = true;

    }


    public void CarStopped(bool inside) {
       // Debug.Log("CarStopped "+inside);
        if (!inside)
        {
            GameManager.instance.GameOver();
        }
    }

    public void Escape(GameObject Prisoner) {
        PathManager path = GameObject.Find(getName + "Path").GetComponent<PathManager>();
        Prisoner.GetComponent<splineMove>().SetPath(WaypointManager.Paths[path.name]);
        Prisoner.GetComponent<Animator>().SetFloat("MoveSpeed", 0.8f);
        
    }

  

  



}
