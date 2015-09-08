using UnityEngine;
using System.Collections;
using BGE;

public class JellyFactory : MonoBehaviour {
    public float height;
    public float width;
    public float numJellies;

    BGE.Flock flock;

    public GameObject jellyPrefab;

    public JellyFactory()
    {
        height = 1000;
        width = 500;
        numJellies = 10;
        
    }

	void Start () {
        flock = GetComponent<Flock>();
        float gap = height / numJellies;
        for(float y = height ; y >= 0 ; y -= gap)
        {
            GameObject boidGameObject = new GameObject();
            boidGameObject.name = "Boid Game Object";
            Vector3 pos = Random.insideUnitSphere * width;
            pos = Random.insideUnitSphere * width;
            pos.x += transform.position.x;
            pos.z += transform.position.z;
            pos.y = transform.position.y + y;
            boidGameObject.transform.position = pos;
            boidGameObject.transform.parent = transform;
            BGE.Boid boid = boidGameObject.AddComponent<BGE.Boid>();
            boid.TurnOffAll();
            //boid.drawVectors = true;
            boid.randomWalkEnabled = true;
            boid.randomWalkRadius = width;
            boid.randomWalkKeepY = true;
            boid.applyBanking = false;
           
            boid.flock = flock;
            flock.boids.Add(boidGameObject);
            GameObject jelly = GameObject.Instantiate<GameObject>(jellyPrefab);
            Vector3 offset = Random.insideUnitSphere * width;
            offset.y = 0;
            jelly.transform.position = boidGameObject.transform.position + offset;

            jelly.transform.parent = boid.transform;
            jelly.GetComponent<AnimationSpeed>().boidObject = boidGameObject;
            float scale = Random.Range(0.2f, 1.0f) * gap;
            jelly.transform.localScale = new Vector3(scale, scale, scale);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
