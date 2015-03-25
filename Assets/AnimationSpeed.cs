using UnityEngine;
using System.Collections;
using BGE;

public class AnimationSpeed : MonoBehaviour {
    public float multiplier;

    public enum speedControllerType { force, acceleration, velocity };

    speedControllerType speedController;


	// Use this for initialization
    AnimationSpeed()
    {
        multiplier = 0.1f;
        speedController = speedControllerType.velocity;
    }

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        float speed = 0;
        
        switch (speedController)
        {
            case speedControllerType.force:
                 speed = GetComponent<Boid>().force.magnitude;
                break;
            case speedControllerType.acceleration:
                speed = GetComponent<Boid>().acceleration.magnitude;
                break;
            case speedControllerType.velocity:
                speed = GetComponent<Boid>().velocity.magnitude;
                break;
        }

        Animator anim = gameObject.GetComponent<Animator>();
        //BoidManager.PrintFloat("Speed: ", speed);
        if (anim != null)
        {
            anim.speed = speed * multiplier;            
        }
	}
}
