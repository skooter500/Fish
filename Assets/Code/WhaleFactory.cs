using UnityEngine;
using System.Collections;

public class WhaleFactory : MonoBehaviour {

    public GameObject whalePrefab;
    
    [HideInInspector]
    public BGE.Boid boid;
    public BGE.Flock flock;

	// Use this for initialization
	void Start () {
        boid = GetComponent<BGE.Boid>();
        flock = GetComponent<BGE.Flock>();

        boid.offsetPursuitEnabled = true;
        //boid.offsetPursuitTarget = flock.
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
