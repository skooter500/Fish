using UnityEngine;
using System.Collections.Generic;

public class Mother : MonoBehaviour {
    public List<GameObject> flocks;

    public float radius;
    public float gap;
    public float rings;
    public int petals;

    public Mother()
    {
        flocks = new List<GameObject>();
        radius = 1000;
        gap = 2000;
        rings = 5;
        petals = 1;
    }
	// Use this for initialization
	void Start () {
        for (int ring = 1; ring <= rings; ring++)
        {
            int petalsInRing = (int)petals * ring;
            float thetaInc = (Mathf.PI * 2.0f) / petalsInRing;
            float theta = Random.Range(0, 2.0f * Mathf.PI);
            for (int i = 0; i < petalsInRing; i++)
            {
                GameObject prefab = flocks[Random.Range(0, flocks.Count)];
                GameObject flock = Instantiate<GameObject>(prefab);
                Vector3 position = new Vector3();
                position.x = transform.position.x + radius + Mathf.Sin(theta) * gap * ring;
                position.z = transform.position.z + radius + Mathf.Cos(theta) * gap * ring;
                position.y = transform.position.y; // +Random.Range(-1500, 500);
                flock.transform.position = position;
                flock.transform.parent = transform;
                flock.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * theta, Vector3.up);
                theta += thetaInc;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
