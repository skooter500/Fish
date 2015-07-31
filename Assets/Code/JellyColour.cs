using UnityEngine;
using System.Collections;

public class JellyColour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Renderer renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = Pallette.Random();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
