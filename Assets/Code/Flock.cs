using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class Flock: MonoBehaviour
    {
        public float radius;
        public int boidCount;
        public GameObject boidPrefab;
        List<GameObject> boids;
        List<GameObject> enemies;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        Flock()
        {
            radius = 100;
            boidCount = 200;
            boids = new List<GameObject>();
            enemies = new List<GameObject>();
        }

        void Start()
        {
            for (int i = 0; i < boidCount; i++)
            {
                GameObject boid = (GameObject)GameObject.Instantiate(boidPrefab);
                boids.Add(boid);
                boid.transform.position = transform.position + UnityEngine.Random.insideUnitSphere * UnityEngine.Random.RandomRange(0, radius);
                boid.GetComponent<Boid>().sphereConstrainEnabled = true;
                boid.GetComponent<Boid>().sphereRadius = radius;
                boid.GetComponent<Boid>().sphereCentre = transform.position;
                if (i == boidCount / 2)
                {
                    //boid.GetComponent<Boid>().drawNeighbours = true;
                }
                boid.GetComponent<Boid>().sphereConstrainEnabled = true;
            }
        }
    }
}
