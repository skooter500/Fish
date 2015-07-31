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
        height = 2000;
        width = 500;
        numJellies = 10;
        
    }

	void Start () {
        flock = GetComponent<Flock>();
        float gap = height / numJellies;
        float halfHeight = height / 2.0f;
        for(float y = - halfHeight ; y < halfHeight ; y += gap)
        {
            GameObject boidGameObject = new GameObject();
            boidGameObject.name = "Boid Game Object";
            Vector3 pos = Random.insideUnitSphere * width;
            pos.y = y;
            boidGameObject.transform.position = transform.position + pos;
            boidGameObject.transform.parent = transform;
            BGE.Boid boid = boidGameObject.AddComponent<BGE.Boid>();
            boid.TurnOffAll();
           
            boid.randomWalkEnabled = true;
            boid.enforceNonPenetrationConstraint = true;
            boid.randomWalkRadius = width;
            boid.randomWalkKeepY = true;
            boid.applyBanking = false;
           
            boid.drawGizmos = false;
            boid.drawVectors = false;
            boid.sphereConstrainEnabled = true;
            boid.flock = flock;
            flock.boids.Add(boidGameObject);
            GameObject jelly = GameObject.Instantiate<GameObject>(jellyPrefab);
            Vector3 offset = Random.insideUnitSphere* width;
            offset.y = y;
            jelly.transform.position = boidGameObject.transform.position + offset;
             
            jelly.transform.parent = boid.transform;
            jelly.GetComponent<AnimationSpeed>().boidObject = boidGameObject;
            float scale = Random.Range(0.2f, 1.0f) * gap * 2.0f;
            jelly.transform.localScale = new Vector3(scale, scale, scale);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
