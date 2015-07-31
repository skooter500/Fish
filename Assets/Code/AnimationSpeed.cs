using UnityEngine;
using System.Collections;
using BGE;

public class AnimationSpeed : MonoBehaviour {
    public float multiplier;

    public enum speedControllerType { force, acceleration, velocity };

    public speedControllerType speedController;
    public GameObject boidObject;

	// Use this for initialization
    AnimationSpeed()
    {
        multiplier = 0.1f;
        speedController = speedControllerType.velocity;
    }

	void Start () {
        if (boidObject == null)
        {
            boidObject = gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {

        float speed = 0;

        Boid boid = boidObject.GetComponent<Boid>();

        if (boid != null)
        {
            switch (speedController)
            {
                case speedControllerType.force:
                    speed = boidObject.GetComponent<Boid>().force.magnitude;
                    break;
                case speedControllerType.acceleration:
                    speed = boidObject.GetComponent<Boid>().acceleration.magnitude;
                    break;
                case speedControllerType.velocity:
                    speed = boidObject.GetComponent<Boid>().velocity.magnitude;
                    break;
            }
        }
        else
        {
            speed = 1.0f;
        }
        Animator anim = gameObject.GetComponent<Animator>();
        //BoidManager.PrintFloat("Speed: ", speed);
        if (anim != null)
        {
            anim.speed = speed * multiplier;            
        }
	}
}
