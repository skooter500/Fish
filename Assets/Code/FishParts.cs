
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
        public float gap;
        
        // Animation stuff
        float theta;
        float angularVelocity = 5.00f;

        private Vector3 headRotPoint;
        private Vector3 tailRotPoint;

        private Vector3 headSize;
        private Vector3 bodySize;
        private Vector3 tailSize;

        public float speedMultiplier;
        public Color colour;

        public float headField = 5;
        public float tailField = 50;
        

        public FishParts()
        {
            segments = new List<GameObject>();

            theta = 0;
            speedMultiplier = 1.0f;
            headField = 5;
            tailField = 50;        
            //colour = Color.white;
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
                segments.Add(segment);
            }
            if (segment.renderer != null)
            {
                segment.renderer.material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            }
            
            return segment;
        }

        public void OnDrawGizmos()
        {
            float radius = (1.5f * segmentExtents) + gap;
            Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(transform.position, radius);
        }


        public void Start()
        {
            if (head == null)
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
        }

        private void LayoutSegments()
        {
            bodySize = body.renderer.bounds.size;
            headSize = head.renderer.bounds.size;
            tailSize = tail.renderer.bounds.size;

            body.transform.position = transform.position;

            float headOffset = (bodySize.z / 2.0f) + gap + (headSize.z / 2.0f) - 0.25f;
            head.transform.position = transform.position + new Vector3(0, 0, headOffset);

            float tailOffset = (bodySize.z / 2.0f) + gap + (tailSize.z / 2.0f) + 0.19f;
            tail.transform.position = transform.position + new Vector3(0, 0, -tailOffset);

            head.transform.parent = transform;
            tail.transform.parent = transform;
            body.transform.parent = transform;

            headRotPoint = head.transform.localPosition;
            headRotPoint.z -= headSize.z / 2;

            tailRotPoint = tail.transform.localPosition;
            tailRotPoint.z += tailSize.z / 2;

        }

        float oldHeadRot = 0;
        float oldTailRot = 0;

        private float fleeColourWait;
        private bool fleeColourStarted;

        System.Collections.IEnumerator FleeColourCycle()
        {
            fleeColourStarted = true;
            while(true)
            {
                if (GetComponent<Boid>().fleeForce.magnitude == 0)
                {
                    break;
                }
                foreach(GameObject segment in segments)
                {
                    segment.renderer.material.color = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.0f, 0.0f), Random.Range(0.0f, 0.0f));
                }
                yield return new WaitForSeconds(fleeColourWait);
                foreach (GameObject segment in segments)
                {
                    segment.renderer.material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                }
                yield return new WaitForSeconds(fleeColourWait);
            }
            fleeColourStarted = false;
        }

        public void Update()
        {            
            float fleeForce = GetComponent<Boid>().fleeForce.magnitude;            
            if (fleeForce  > 0)
            {
                BoidManager.PrintFloat("Flee force: ", fleeForce);
                fleeColourWait = 0.1f; // 100000.0f / fleeForce;
                BoidManager.PrintFloat("Flee wait: ", fleeColourWait);
                if (!fleeColourStarted)
                {
                    StartCoroutine("FleeColourCycle");
                }
            }
            // Animate the head            
            float headRot = Mathf.Sin(theta) * headField;
            head.transform.RotateAround(transform.TransformPoint(headRotPoint), transform.up, headRot - oldHeadRot);            
            
            oldHeadRot = headRot;

            // Animate the tail
            float tailRot = Mathf.Sin(theta) * tailField;
            tail.transform.RotateAround(transform.TransformPoint(tailRotPoint), transform.up, tailRot - oldTailRot);
            oldTailRot = tailRot;

            float speed = GetComponent<Boid>().acceleration.magnitude;
            theta += speed * angularVelocity * Time.deltaTime * speedMultiplier;            
            if (theta >= Mathf.PI * 2.0f)
            {
                theta -= (Mathf.PI * 2.0f);
            }
        }
    }
}
