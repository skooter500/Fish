using UnityEngine;
using System.Collections;

public class FlockBoidController : MonoBehaviour {
    public BGE.Flock flock;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        flock.flockCenter = transform.position;
	}
}
