using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {

    public float amplitude;
    public float frequency;

    float theta;
    Vector3 startPos;
    public Hover()
    {
        amplitude = 20;
        frequency = 0.5f;
        theta = 0.0f;
    }
	// Use this for initialization
	void Start () {
        theta = Random.Range(0, 2.0f * Mathf.PI);
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 hoverPos = new Vector3();    

        hoverPos.y = Mathf.Sin(theta) * amplitude;
        theta += Time.deltaTime * frequency;
        if (theta > (2.0f * Mathf.PI))
        {
            theta = 0.0f;
        }
        transform.position = startPos + hoverPos;
	}
}
