using UnityEngine;
using System.Collections;

public class JellyColour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Renderer renderer = GetComponentInChildren<Renderer>();
        renderer.material.color = Pallette.Random();

        GetComponent<ColorLerper>().gameObjects.Add(gameObject);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
