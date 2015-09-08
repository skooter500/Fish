using UnityEngine;
using System.Collections;

public class WhaleStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3 pos = transform.position;
        pos.y += 2000;
        transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
