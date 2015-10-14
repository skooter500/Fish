
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace BGE
{
    public class MinnowFactory : MonoBehaviour
    {        
        public float radius;
        public int boidCount;
        public GameObject boidPrefab;

        public bool spawnInTopHemisphere;

        [Range(0, 1)]
        public float spread;

        [HideInInspector]
        public Flock flock;

        [Header("Debug")]
        public bool drawGizmos;

        Queue<Boid> jobQueue = new Queue<Boid>();

        public int numThreads = 1;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        MinnowFactory()
        {
            radius = 100;
            boidCount = 200;

            spread = 1.0f;
        }

        void Update()
        {
            //Debug.Log(thread.IsAlive);
        }
        
        void Start()
        {
            flock = GetComponent<Flock>();
            int maxAudioBoids = 5;
            int audioBoids = 0;

            for (int i = 0; i < boidCount; i++)
            {
                Boid boid = GameObject.Instantiate<GameObject>(boidPrefab).GetComponent<Boid>();
                flock.boids.Add(boid);
                
                bool inside = false;
                do
                {
                    Vector3 unit = UnityEngine.Random.insideUnitSphere;
                    if (spawnInTopHemisphere)
                    {
                        unit.y = Mathf.Abs(unit.y);
                    }
                    boid.position = transform.position + unit * UnityEngine.Random.Range(0, radius * spread);

                    Vector3 p = boid.position;                    
                    inside = false;
                    foreach (Obstacle obstacle in BoidManager.Instance.obstacles)
                    {
                        if (Vector3.Distance(obstacle.transform.position, boid.position) < obstacle.radius + boid.minBoxLength)
                        {
                            inside = true;
                            break;
                        }
                    }
                }
                while (inside);
                boid.transform.parent = flock.transform;
                boid.flock = flock;
                boid.sphereConstrainEnabled = true;
                boid.sphereRadius = radius;
                AudioSource audioSource = boid.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    if (audioBoids < maxAudioBoids)
                    {
                        audioSource.enabled = true;
                        audioSource.loop = true;
                        audioSource.Play();
                        audioBoids++;
                    }
                    else
                    {
                        audioSource.enabled = false;
                    }
                }
                if (i == boidCount / 2)
                {
                    if (drawGizmos)
                    {
                        boid.drawGizmos = drawGizmos;
                        boid.drawNeighbours = false;
                    }
                }
            }

            StartUpdateThreads();
        }

        bool running = false;

        void UpdateThread()
        {
            foreach(Boid boid in flock.boids)
            {
                jobQueue.Enqueue(boid);
            }

            int i = 0;
            while (running)
            {
                // Take one off the queue, update it and then enque it again
                Boid boid = jobQueue.Dequeue();
                boid.UpdateOnThread();


                Thread.Sleep(10);
                jobQueue.Enqueue(boid);
                
                //Boid boid = flock.boids[0];
                Thread.Sleep(10);
                Debug.Log("Doing thread" + (++ i));
            }
        }

        void OnApplicationQuit()
        {
            running = false;
        }

        void StartUpdateThreads()
        {
            running = true;
            for (int i = 0; i < numThreads; i++)
            {
                Thread thread = new Thread(UpdateThread);
                thread.Start();
            }
        }
    }     
}
