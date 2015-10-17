
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace BGE
{
    public class MinnowFactory : MonoBehaviour
    {
        private long threadCount = 0;
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

        [HideInInspector]
        public float threadTimeDelta;

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
            BoidManager.PrintFloat("Boid FPS: ", threadFPS);
            BoidManager.PrintFloat("ThreadCount: ", (int) threadCount);
            BoidManager.PrintFloat("Thread TimeDelta", flock.threadTimeDelta);
        }

        long lastThreadCount = 0;
        float threadFPS;

        void UpdateThread()
        {
            while (running)
            {
                for (int i = 0; i < flock.boids.Count; i++)
                {
                    Boid boid = flock.boids[i];
                    boid.UpdateOnThread();
                }
                threadCount++;
                //Thread.Sleep(10);
            }
        }

        System.Collections.IEnumerator UpdateThreadTimeDelta()
        {
            while (true)
            {
                long newThreadCount = threadCount;
                threadFPS = newThreadCount - lastThreadCount;
                flock.threadTimeDelta = 1.0f / threadFPS;
                lastThreadCount = newThreadCount;
                yield return new WaitForSeconds(1.0f);
            }
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
                    boid.transform.position = transform.position + unit * UnityEngine.Random.Range(0, radius * spread);

                    Vector3 p = boid.transform.position;                    
                    inside = false;
                    foreach (Obstacle obstacle in BoidManager.Instance.obstacles)
                    {
                        if (Vector3.Distance(obstacle.transform.position, boid.transform.position) < obstacle.radius + boid.minBoxLength)
                        {
                            inside = true;
                            break;
                        }
                    }
                }
                while (inside);
                boid.transform.parent = flock.transform;
                boid.flock = flock;
                boid.multiThreadingEnabled = true;
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
            StartCoroutine("UpdateThreadTimeDelta");
        }

        bool running = false;

        

        void OnApplicationQuit()
        {
            running = false;
        }

        void StartUpdateThreads()
        {
            running = true;

            for (int i = 0; i < flock.boids.Count; i++)
            {
                Boid boid = flock.boids[i];
                boid.UpdateLocalFromTransform();
            }
            
            // Enque all the boids            
            Thread thread = new Thread(UpdateThread);
            thread.Start();            
        }
    }     
}
