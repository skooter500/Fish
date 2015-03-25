using UnityEngine;
using System.Collections;

public class JellyColour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Renderer renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
