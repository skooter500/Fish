
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

        public float radius = 100;

        [HideInInspector]
        public List<Boid> boids = new List<Boid>();
        public List<GameObject> enemies = new List<GameObject>();       
     
        [Range(0, 2)]
        public float timeMultiplier = 1.0f;
     
        [Header("Debug")]
        public bool drawGizmos;        

        [Header("Performance")]
        public float tagDither = 1.0f;
        public int maxTagged = 50;

        [HideInInspector]
        public Vector3 flockCenter;
        [HideInInspector]
        public Vector3 oldFlockCenter;        

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(flockCenter, radius);
        }


        void Start()
        {
            if (UseCellSpacePartitioning)
            {
                // Allow 3x the radius in case boids go outside of the sphere...
                numCells = 40; //(radius * 3) / neighbourDistance;
                space = new Space(transform.position, radius * 3, radius * 3, radius * 3, numCells, boids);
            }
            flockCenter = transform.position;
        }


        public void Update()
        {
            if (UseCellSpacePartitioning)
            {
                space.bounds.center = transform.position;
            }
            if (drawGizmos)
            {
                LineDrawer.DrawSphere(flockCenter, radius, 20, Color.magenta);
                if (UseCellSpacePartitioning)
                {
                    // In case the flock center moves

                    space.Draw();
                }
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
