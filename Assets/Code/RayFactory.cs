using UnityEngine;
using System.Collections;

public class RayFactory : MonoBehaviour {
    public GameObject rayPrefab;
    public float height;
    public float width;
    public int numRays = 5;

    public RayFactory()
    {
        height = 1000;
        width = 1000;
    }
	// Use this for initialization
	void Start () {
        float gap = height / numRays;       
      
	    for (float y = height; y >= 0; y -= gap)
        {
            GameObject ray = Instantiate(rayPrefab);
            Vector3 pos = new Vector3();
            pos = Random.insideUnitSphere * width;
            pos.x += transform.position.x;
            pos.z += transform.position.z;
            pos.y = transform.position.y + y;
            ray.transform.position = pos;
            ray.transform.parent = transform;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
