using UnityEngine;
using System.Collections;

public class BigBoid : MonoBehaviour {

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.GetComponent<Renderer>().material.color = Color.red;

        }
    }
}
