using UnityEngine;
using System.Collections.Generic;
using BGE;
using Gamelogic.Colors;

public class DolphinSchool : MonoBehaviour {

    public GameObject dolphinPrefab;

    public float minGap;    
    public int dolphinCount;
    public float radius;
    public float wanderRadius;
    public float heightAbove = 500;

    [HideInInspector]
    public GameObject leader;

    PaletteGenerator pg;

    public DolphinSchool()
    {
        minGap = 200;
        dolphinCount = 10;
        radius = 300;
        wanderRadius = 10000;
    }

    void MakePath(GameObject leader)
    {
        Boid boid = leader.GetComponent<Boid>();
        boid.followPathEnabled = true;
        int numWaypoints = 50;

        float theta = 0.0f;
        float thetaInc = (Mathf.PI * 2.0f) / numWaypoints;
        for (int i = 0; i <= numWaypoints; i++)
        {
            Vector3 waypoint = new Vector3();
            waypoint.x = transform.position.x + (Mathf.Sin(theta - (Mathf.PI * 0.5f)) * wanderRadius);
            waypoint.z = transform.position.z + (Mathf.Cos(theta - (Mathf.PI * 0.5f)) * wanderRadius);
            waypoint.y = 10000;

            RaycastHit hitInfo;
            bool collided = Physics.Raycast(waypoint, Vector3.down, out hitInfo);            
            if (collided)
            {
                waypoint.y = hitInfo.point.y + heightAbove;
            }
            
            theta += thetaInc;
            boid.path.Waypoints.Add(waypoint);
            boid.path.Looped = true;
            //boid.drawGizmos = true;
        }
        leader.transform.position = leader.GetComponent<Boid>().path.Waypoints[0];

    }

    // Use this for initialization
    void Start () {
        
        pg = GetComponent<PaletteGenerator>();
        
        leader = Instantiate<GameObject>(dolphinPrefab);
        leader.transform.position = transform.position;        
        MakePath(leader);
        SetDolphinColors(leader);

        Vector3 lastPoint = leader.transform.position;
        for (int i = 0; i < dolphinCount; i ++) 
        {
            Vector3 newPoint;
            do
            {
                newPoint = leader.transform.position + UnityEngine.Random.insideUnitSphere * radius;
            }
            while (Vector3.Distance(newPoint, lastPoint) < minGap);
            GameObject dolphin = Instantiate<GameObject>(dolphinPrefab);
            dolphin.transform.position = newPoint;
            Boid boid = dolphin.GetComponent<Boid>();
            boid.offsetPursuitEnabled = true;
            boid.offsetPursuitTarget = leader;
            lastPoint = newPoint;
            DolphinParts dolphinParts = dolphin.GetComponentInChildren<DolphinParts>();
            dolphinParts.speedMultiplier += Random.Range(-0.01f, 0.01f);

            SetDolphinColors(dolphin);
        }
        BoidManager.Instance.cameraBoid = leader;


	}

    Color RandomFromPalette()
    {
        return pg.palette.colors[Random.Range(0, pg.palette.colors.Length)];
    }

    void SetDolphinColors(GameObject dolphin)
    {
        DolphinParts dolphinParts = dolphin.GetComponentInChildren<DolphinParts>();
            
        Utilities.RecursiveSetColor(dolphinParts.tail, RandomFromPalette());
        Utilities.RecursiveSetColor(dolphinParts.body, RandomFromPalette());
        Utilities.RecursiveSetColor(dolphinParts.head, RandomFromPalette());
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
