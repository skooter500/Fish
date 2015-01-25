using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class Obstacle : MonoBehaviour
    {    
        
        public float radius = 20;
        private Color color = Color.yellow;

        public bool drawGizmos;

        public Obstacle()
        {
            drawGizmos = false;
            color = Color.gray;
        }
        void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        void Update()
        {
            if (drawGizmos)
            {
                LineDrawer.DrawSphere(transform.position, radius, 20, color);
            }
        }
    }
}
