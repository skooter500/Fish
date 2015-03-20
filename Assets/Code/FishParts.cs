
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class FishParts : MonoBehaviour
    {
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

        public float headField;
        public float tailField;

        public bool randomPartColours;

       
        public FishParts()
        {
            segments = new List<GameObject>();

            theta = 0;
            speedMultiplier = 1.0f;
            headField = 5;
            tailField = 50;
            randomPartColours = false;
            //colour = Color.white;
        }

        public GameObject InstiantiateDefaultShape()
        {

            GameObject segment = null;
            segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Vector3 scale = new Vector3(1, segmentExtents, segmentExtents);
            segment.transform.localScale = scale;            
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

            if (transform.childCount != 3)
            {
                head = InstiantiateDefaultShape();
                body = InstiantiateDefaultShape();
                tail = InstiantiateDefaultShape();
                LayoutSegments();
            }
            else
            {
                head = transform.GetChild(0).gameObject;
                body = transform.GetChild(1).gameObject;
                tail = transform.GetChild(2).gameObject;
            }

            segments.Add(head);
            segments.Add(body);
            segments.Add(tail);

            foreach(GameObject segment in segments)
            {
                if (segment.GetComponent<Renderer>() != null)
                {
                    if (randomPartColours)
                    {
                        segment.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    }
                    else
                    {
                        segment.GetComponent<Renderer>().material.color = colour;
                    }
                }
            }
            
            if (head.GetComponent<Collider>() != null)
            {
                head.GetComponent<Collider>().enabled = false;
            }
            if (body.GetComponent<Collider>() != null)
            {
                body.GetComponent<Collider>().enabled = false;
            }
            if (tail.GetComponent<Collider>() != null)
            {
                tail.GetComponent<Collider>().enabled = false;
            }
        }

        private void LayoutSegments()
        {
            bodySize = body.GetComponent<Renderer>().bounds.size;
            headSize = head.GetComponent<Renderer>().bounds.size;
            tailSize = tail.GetComponent<Renderer>().bounds.size;

            body.transform.position = transform.position;

            float headOffset = (bodySize.z / 2.0f) + gap + (headSize.z / 2.0f);
            head.transform.position = transform.position + new Vector3(0, 0, headOffset);

            float tailOffset = (bodySize.z / 2.0f) + gap + (tailSize.z / 2.0f);
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
            while (true)
            {
                if (GetComponent<Boid>().fleeForce.magnitude == 0)
                {
                    break;
                }
                /*
                for(int i = segments.Count - 1 ; i > 0  ; i --)
                {
                    segments[i] = segments[i - 1];
                }
                segments[0].renderer.material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                yield return new WaitForSeconds(fleeColourWait);
                */

                foreach (GameObject segment in segments)
                {
                    segment.GetComponent<Renderer>().material.color = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.0f, 0.0f), Random.Range(0.0f, 0.0f));
                }
                yield return new WaitForSeconds(fleeColourWait);
                foreach (GameObject segment in segments)
                {
                    segment.GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                }
                yield return new WaitForSeconds(fleeColourWait);
            }
            fleeColourStarted = false;
        }

        public void Update()
        {
            float fleeForce = GetComponent<Boid>().fleeForce.magnitude;
            if (fleeForce > 0)
            {
                fleeColourWait = 0.1f; // 100000.0f / fleeForce;
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
