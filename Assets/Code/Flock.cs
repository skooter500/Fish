
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class Flock: MonoBehaviour
    {

        [Header("Cell Space Partitioning")]
        public bool UseCellSpacePartitioning;
        [HideInInspector]
        public Space space;
        [HideInInspector]
        public float numCells;
        
        public float neighbourDistance;
        public float neighbourTagDither;

        public float radius;
        public int boidCount;
        public GameObject boidPrefab;

        [HideInInspector]
        public List<GameObject> boids;
        public List<GameObject> enemies;
        
        public bool spawnInTopHemisphere;

        [Range(0, 2)]
        public float timeMultiplier;
        [Range(0, 1)]
        public float spread;

        [Header("Debug")]
        public bool drawGizmos;

        [HideInInspector]
        public bool doNeighbourCount;
        

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
            numCells = 50;
            neighbourDistance = 50;
            neighbourTagDither = 1.0f;

            spread = 1.0f;
            
        }

        void Start()
        {
            for (int i = 0; i < boidCount; i++)
            {
                GameObject boid = GameObject.Instantiate<GameObject>(boidPrefab);
                boids.Add(boid);
                Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                SetIfNotNull(boid, color);
                for (int j = 0; j < boid.transform.childCount; j ++)
                {
                    SetIfNotNull(boid.transform.GetChild(j).gameObject, color);
                }
                   
                boid.transform.parent = transform;
                bool inside = false;
                do
                {
                    Vector3 unit = UnityEngine.Random.insideUnitSphere;
                    if (spawnInTopHemisphere)
                    {
                        unit.y = Mathf.Abs(unit.y);
                    }
                    boid.transform.position = transform.position + unit * UnityEngine.Random.Range(0, radius * spread);
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
                AudioSource audioSource = boid.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    //if (Random.Range(0, 1) > 0.1)
                    {
                        audioSource.enabled = false;
                    }
                }               
                if (i == boidCount / 2)
                {
                    if (drawGizmos)
                    {
                        boid.GetComponent<Boid>().drawGizmos = drawGizmos;
                        boid.GetComponent<Boid>().drawNeighbours = true;
                    }
                }
            }
            //int camBoid = Random.Range(0, boidCount);
            //BoidManager.Instance.cameraBoid = boids[camBoid];
            //boids[camBoid].GetComponent<FishParts>().enabled = false;
            //boids[camBoid].GetComponent<Boid>().fleeEnabled = false;
            //boids[camBoid].GetComponent<Boid>().timeMultiplier = 1.0f;

            // Allow 3x the radius in case boids go outside of the sphere...
            numCells = 40; //(radius * 3) / neighbourDistance;
            space = new Space(transform.position, radius * 3, radius * 3, radius * 3, numCells, boids);

            doNeighbourCount = true;
        }

        private void SetIfNotNull(GameObject gameObject, Color color)
        {
            if (gameObject != null)
            {
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
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

            if (drawGizmos)
            {
                // In case the flock center moves
                space.bounds.center = transform.position;
                space.Draw();
            }

            float prob = Random.Range(0.0f, 1.0f);
            if (prob <= neighbourTagDither)
            {
                doNeighbourCount = true;
            }
            else
            {
                doNeighbourCount = false;
            }
        }

        void LateUpdate()
        {
            if (UseCellSpacePartitioning)
            {
                space.Partition();
            }
        }
    }
}
