using UnityEngine;
using System.Collections;
using BGE;

public class RandomPath : MonoBehaviour {
    public float heightAbove = 500;
    Vector3 dest;

    NoiseForm noiseForm;
    Boid boid;
    Flock flock;
    
    float fov = 45.0f;

    float minDist = 5000;
    float maxDist = 10000;

    void MakeNewPath()
    {

        Path path = GetComponent<Path>();

        Quaternion q = Quaternion.AngleAxis(Random.Range(-fov, fov), transform.up);

        Vector3 oldDest = dest;
        Vector3 newForward = q * transform.forward;
        dest = transform.position + newForward * Random.Range(minDist, maxDist);
        dest.y = noiseForm.GetHeight(dest) + heightAbove;        
        //boid.path.Clear();
        boid.followPathEnabled = true;
        Vector3 waypoint = oldDest;
        Vector3 toDest = (dest - oldDest).normalized;
        while (Vector3.Distance(waypoint, dest) > 200)
        {
            path.Waypoints.Add(waypoint);
            waypoint += toDest * 100;
            waypoint.y = noiseForm.GetHeight(waypoint) + heightAbove;
        }
    }

    // Use this for initialization
    void Start () {
        noiseForm = FindObjectOfType<NoiseForm>();
        boid = GetComponent<Boid>();
        flock = GetComponent<Flock>();
        MakeNewPath();
    }

    // Update is called once per frame
    void Update () {
        flock.flockCenter = transform.position;
        float dist = Vector3.Distance(transform.position, dest);
        BoidManager.PrintMessage("Distance: " + dist);
        if (dist < 2500)
        {
            MakeNewPath();
        }
    }
}
