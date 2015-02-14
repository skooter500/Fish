using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class FishParts:MonoBehaviour
    {
        public GameObject headPrefab;
        public GameObject bodyPrefab;
        public GameObject tailPrefab;
        
        [HideInInspector]
        public GameObject head;
        [HideInInspector]
        public GameObject body;        
        [HideInInspector]
        public GameObject tail;

        List<GameObject> segments;

        float segmentExtents = 3;
        float gap = 1;
        
        // Animation stuff
        float theta;
        float headField = 5;
        float tailField = 50;
        float angularVelocity = 0.15f;

        private Vector3 headRotPoint;
        private Vector3 tailRotPoint;

        private Vector3 headSize;
        private Vector3 bodySize;
        private Vector3 tailSize;

        public FishParts()
        {
            segments = new List<GameObject>();
        }

        public GameObject InstiantiateSegmentFromPrefab(GameObject prefab)
        {
            
            GameObject segment = null;
            if (prefab == null)
            {
                segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Vector3 scale = new Vector3(1, segmentExtents, segmentExtents);
                segment.transform.localScale = scale;
            }
            else
            {
                segment = (GameObject) GameObject.Instantiate(prefab);
            }
            segment.transform.parent = transform;
            
            return segment;
        }

        public void OnDrawGizmos()
        {
            float radius = (1.5f * segmentExtents) + gap;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }


        public void Start()
        {
            head = InstiantiateSegmentFromPrefab(headPrefab);
            body = InstiantiateSegmentFromPrefab(bodyPrefab);
            tail = InstiantiateSegmentFromPrefab(tailPrefab);

            segments.Add(head);
            segments.Add(body);
            segments.Add(tail);
            if (head.collider != null)
            {
                head.collider.enabled = false;
            }
            if (body.collider != null)
            {
                body.collider.enabled = false;
            }
            if (tail.collider != null)
            {
                tail.collider.enabled = false;
            }

            LayoutSegments();
        }

        private void LayoutSegments()
        {
            bodySize = body.renderer.bounds.size;
            headSize = head.renderer.bounds.size;
            tailSize = tail.renderer.bounds.size;

            body.transform.localPosition = Vector3.zero;
            float headOffset = (bodySize.z / 2.0f) + gap + (headSize.z / 2.0f);
            head.transform.localPosition = new Vector3(0, 0, headOffset);

            float tailOffset = (bodySize.z / 2.0f) + gap + (tailSize.z / 2.0f);
            tail.transform.localPosition = new Vector3(0, 0, - tailOffset);

            headRotPoint = head.transform.position;
            headRotPoint.z -= headSize.z / 2;

            tailRotPoint = tail.transform.position;
            tailRotPoint.z += tailSize.z / 2;
        }

        float oldHeadRot = 0;
        float oldTailRot = 0;

        public void Update()
        {

            // Animate the head            
            float headRot = Mathf.Sin(theta) * headField;
            head.transform.RotateAround(transform.TransformPoint(headRotPoint), transform.up, headRot - oldHeadRot);            
            
            oldHeadRot = headRot;

            // Animate the tail
            float tailRot = Mathf.Sin(theta) * tailField;
            tail.transform.RotateAround(transform.TransformPoint(tailRotPoint), transform.up, tailRot - oldTailRot);
            oldTailRot = tailRot;

            theta += angularVelocity;
            if (theta >= Math.PI * 2.0f)
            {
                theta = 0;
            }

            /*
            Vector3 pos = transform.position;
            pos.x += Time.deltaTime * 5.0f;
            pos.z += Time.deltaTime * 5.0f;
            transform.position = pos;
            //transform.Rotate(transform.forward, 30 * Time.deltaTime);             
            */
        }
    }
}
