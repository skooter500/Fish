
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

        public float radius;

        [HideInInspector]
        public List<GameObject> boids;
        public List<GameObject> enemies;       
     
        [Range(0, 2)]
        public float timeMultiplier;
     
        [Header("Debug")]
        public bool drawGizmos;        
        public float updateDither;

        public Vector3 flockCenter;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        Flock()
        {
            radius = 100;
            timeMultiplier = 1.0f;
            boids = new List<GameObject>();
            enemies = new List<GameObject>();           
            updateDither = 1.0f;
            numCells = 50;
        
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
            if (drawGizmos)
            {
                LineDrawer.DrawSphere(transform.position, radius, 20, Color.yellow);
            }

            if (drawGizmos)
            {
                // In case the flock center moves
                space.bounds.center = transform.position;
                space.Draw();
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
