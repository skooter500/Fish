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
        public List<GameObject> enemies;
        public bool spawnInTopHemisphere;

        [Range(0, 2)]
        public float timeMultiplier;

        [Header("Debug")]
        public bool drawGizmos;


        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        Flock()
        {
            radius = 100;
            boidCount = 200;
            timeMultiplier = 1.0f;
            boids = new List<GameObject>();
            enemies = new List<GameObject>();
        }

        void Start()
        {
            for (int i = 0; i < boidCount; i++)
            {
                GameObject boid = (GameObject)GameObject.Instantiate(boidPrefab);
                boids.Add(boid);
                bool inside = false;
                do
                {
                    Vector3 unit = UnityEngine.Random.insideUnitSphere;
                    if (spawnInTopHemisphere)
                    {
                        unit.y = Mathf.Abs(unit.y);
                    }
                    boid.transform.position = transform.position + unit * UnityEngine.Random.RandomRange(0, radius);
                    inside = false;
                    foreach (Obstacle obstacle in BoidManager.Instance.obstacles)
                    {
                        if (Vector3.Distance(obstacle.transform.position, boid.transform.position) < obstacle.radius + boid.GetComponent<Boid>().minBoxLength)
                        {
                            inside = true;
                            break;
                        }
                    }                    
                }
                while (inside);
                boid.GetComponent<Boid>().flock = this;
                boid.GetComponent<Boid>().sphereConstrainEnabled = true;
                boid.GetComponent<Boid>().sphereRadius = radius;
                boid.GetComponent<Boid>().sphereCentre = transform.position;
                boid.GetComponent<Boid>().fleeTarget = GameObject.FindGameObjectWithTag("Player");
                if (i == boidCount / 2)
                {
                    if (drawGizmos)
                    {
                        boid.GetComponent<Boid>().drawNeighbours = true;
                    }
                }
            }
        }

        public void Update()
        {
            if (drawGizmos)
            {
                LineDrawer.DrawSphere(transform.position, radius, 20, Color.yellow);
            }

            if (GetComponent<Boid>() != null)
            {
                foreach (GameObject o in boids)
                {
                    o.GetComponent<Boid>().sphereCentre = transform.position;
                }
            }
        }
    }
}
