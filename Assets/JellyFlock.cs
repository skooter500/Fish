using UnityEngine;
using System.Collections;

public class JellyFlock : MonoBehaviour {
    public float height;
    public float width;
    public float numJellies;

    public GameObject jellyPrefab;

    public JellyFlock()
    {
        height = 500;
        width = 500;
        numJellies = 10;
    }

	void Start () {

        float gap = height / numJellies;
        float halfHeight = height / 2.0f;
        for(float y = - halfHeight ; y < halfHeight ; y ++)
        {
            GameObject gameObject = new GameObject();
            Vector3 pos = Random.insideUnitSphere * width;
            pos.y = y;
            gameObject.transform.parent = transform;
            BGE.Boid boid = gameObject.AddComponent<BGE.Boid>();
            boid.TurnOffAll();
            boid.keepUpright = true;
            boid.randomWalkEnabled = true;
            boid.randomWalkRadius = width;
            boid.randomWalkKeepY = true;

            GameObject jelly = GameObject.Instantiate<GameObject>(jellyPrefab);
            jelly.transform.position = Random.insideUnitSphere * width;
            jelly.transform.parent = gameObject.transform;
            jelly.GetComponent<AnimationSpeed>().boidObject = gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
