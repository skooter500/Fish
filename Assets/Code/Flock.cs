
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class Flock: MonoBehaviour
    {
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

        public Vector3 centreOfMass;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(flockCenter, radius);
        }


        void Start()
        {
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
            UpdateEnemyPositions();
        }
    }
}
