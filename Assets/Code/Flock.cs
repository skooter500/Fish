
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
        [HideInInspector]
        public List<Vector3> enemyPositions = new List<Vector3>();

     
        [Range(0, 2)]
        public float timeMultiplier = 1.0f;
     
        [Header("Debug")]
        public bool drawGizmos;        

        [HideInInspector]
        public Vector3 flockCenter;
        [HideInInspector]
        public Vector3 oldFlockCenter;

        [HideInInspector]
        public float threadTimeDelta;

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

            UpdateEnemyPositions();
        }

        private void UpdateEnemyPositions()
        {            
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemyPositions.Count <= i)
                {
                    enemyPositions.Add(Vector3.zero);
                }
                enemyPositions[i] = enemies[i].transform.position;
            }
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
                    space.Draw();
                }
            }
            UpdateEnemyPositions();
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
