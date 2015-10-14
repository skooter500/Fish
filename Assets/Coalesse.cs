using UnityEngine;
using System.Collections;
using BGE;

public class Coalesse : MonoBehaviour {
    Flock flock;
	// Use this for initialization
	void Start () {
        flock = GetComponent<Flock>();
        StartCoroutine("CoalesseBoids");
	}

    System.Collections.IEnumerator CoalesseBoids()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(20.0f, 20.0f));
            Debug.Log("Coalessing");
            foreach (Boid boid in flock.boids)
            {
                boid.separationEnabled = false;                
                boid.alignmentEnabled = false;                
                boid.wanderEnabled = false;                
                boid.cohesionEnabled = false;
                boid.seekPlayer = false;
                boid.seekEnabled = true;
                boid.seekTargetPos = flock.flockCenter;
            }
            yield return new WaitForSeconds(20.0f);
            Debug.Log("Flocking");
            foreach (Boid boid in flock.boids)
            {
                boid.separationEnabled = true;
                boid.cohesionEnabled = true;
                boid.alignmentEnabled = true;
                boid.wanderEnabled = true;
                boid.seekEnabled = false;
            }
            yield return new WaitForSeconds(20.0f);
            Debug.Log("Seek player");
            foreach (Boid boid in flock.boids)
            {
                boid.separationEnabled = true;
                boid.cohesionEnabled = true;
                boid.alignmentEnabled = true;
                boid.wanderEnabled = false;
                boid.seekEnabled = true;
                boid.seekPlayer = true;
            }            
            yield return new WaitForSeconds(20.0f);
            Debug.Log("Flocking");
            foreach (Boid boid in flock.boids)
            {
                boid.separationEnabled = true;
                boid.cohesionEnabled = true;
                boid.alignmentEnabled = true;
                boid.wanderEnabled = true;
                boid.seekEnabled = false;
            }            
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
