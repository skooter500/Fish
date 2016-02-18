using UnityEngine;
using System.Collections;
using BGE;

public class BigBoidFlock : MonoBehaviour {

    public GameObject otherFlock;

	// Use this for initialization
	void Start () {
        Flock flock = GetComponent<Flock>();

        Flock other = flock.GetComponent<Flock>();

        foreach (Boid boid in flock.boids)
        {
            other.enemies.Add(boid.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
