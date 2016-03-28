using UnityEngine;
using System.Collections;
using BGE;

public class SnakeGenerator : MonoBehaviour {

    public int numParts = 20;

	// Use this for initialization
	void Start () {

        SnakeHead snakeHead = null;

        for (int i = 0; i < numParts; i++)
        {
            GameObject cube = (GameObject) GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = transform;
            cube.transform.position = transform.position + new Vector3(0, 0, i * 1.2f);

            if (i == 0)
            {
                snakeHead = cube.AddComponent<SnakeHead>();
                Boid boid = cube.AddComponent<Boid>();
                boid.wiggleEnabled = true;
                boid.sphereRadius = 50;

                boid.wanderEnabled = false;
                boid.wanderDistance = 1.0f;
                boid.wanderRadius = 50.0f;
                boid.wanderMethod = Boid.WanderMethod.Noise;
                boid.wanderNoiseDeltaX = 1.0f;

                boid.wiggleWeigth = 10.0f;

                boid.wiggleAngularSpeedDegrees = 100.0f;
                boid.wiggleDirection = Boid.WiggleAxis.Horizontal;
                boid.sphereConstrainEnabled = true;
                        
            }
            else
            {
                snakeHead.followers.Add(cube);
            }
            
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
