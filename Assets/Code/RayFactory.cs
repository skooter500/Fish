using UnityEngine;
using System.Collections;

public class RayFactory : MonoBehaviour {
    public GameObject rayPrefab;
	// Use this for initialization
	void Start () {
	    for (int y = -500 ; y <= 500 ; y += 200)
        {
            GameObject ray = Instantiate(rayPrefab);
            Vector3 pos = new Vector3();
            pos.x = Random.Range(-500.0f, 500.0f);
            pos.z = Random.Range(-500.0f, 500.0f);
            pos.y = y;
            ray.transform.position = transform.position + pos;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
