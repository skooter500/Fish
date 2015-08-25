
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class DolphinParts : MonoBehaviour
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
        public Vector3 headRotPoint;
        public Vector3 bodyRotPoint;
        public Vector3 tailRotPoint;

        private Vector3 headSize;
        private Vector3 bodySize;
        private Vector3 tailSize;

        [Range(0.0f, 0.2f)]
        public float speedMultiplier;

        [Range(0, 180)]
        public float headField;

        [Range(0, 180)]
        public float tailField;

        [Range(0, 180)]
        public float bodyField;

        [Range(0, 200)]
        public float bodyWiggleHeight;

        [Range(0, 180)]
        public float maxTurnAngle;

        [Range(0, 1000)]
        public float maxTurnSpeed;
        public bool forwardWiggle;
        public bool sideWiggle;

        public GameObject boidGameObject;

        [HideInInspector]
        public Boid boid;

        public DolphinParts()
        {
            segments = new List<GameObject>();

            theta = 0;
            speedMultiplier = 1.0f;
            headField = 30;
            bodyField = 
            tailField = 20;

            forwardWiggle = true;
            sideWiggle = true;
            bodyWiggleHeight = 20;
            maxTurnAngle = 50.0f;
            maxTurnSpeed = 400.0f;

            boidSpeedToAnimationSpeed = true;
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
            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(transform.TransformPoint(headRotPoint), 5);
            Gizmos.DrawSphere(transform.TransformPoint(bodyRotPoint), 5);
            Gizmos.DrawSphere(transform.TransformPoint(tailRotPoint), 5);

            Gizmos.color = Color.magenta;
        }


        public void Start()
        {
            if (transform.childCount != 3)
            {
                head = InstiantiateDefaultShape();
                body = InstiantiateDefaultShape();
                tail = InstiantiateDefaultShape();
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

            boid = (boidGameObject == null) ? GetComponent<Boid>() : boidGameObject.GetComponent<Boid>();

            originalTailPosition = tail.transform.localPosition;

            maxTurnAngleRadians = maxTurnAngle * Mathf.Deg2Rad;
        }

        public bool boidSpeedToAnimationSpeed;

        float oldHeadRot = 0;
        float oldBodyRot = 0; 
        float oldTailRot = 0;

        private float myBankAngle = 0.0f;
        private float bankAngle = 0.0f;
        private bool lastClamp = false;

        private float lastBodyWiggle = 0;
        private Vector3 originalTailPosition;
        private float maxTurnAngleRadians;
        // Calculate by how much the head has turned relative to body
        private float TurnAngle()
        {
            float dot = Vector3.Dot(body.transform.right, head.transform.right);
            float angle = Mathf.Acos(dot);
            if (float.IsNaN(angle)) // Not sure why this happens
            {
                angle = 0.0f;
            }
            float side = Vector3.Dot(head.transform.forward, body.transform.right);
            if (side < 0) // Angle < 90
            {
                angle = -angle;
            }
            return angle;
        }

        public void Update()
        {
            bool turning = false;

            if (sideWiggle)
            {
                float turnAngle = TurnAngle();
                bankAngle = boid.bankAngle;

                float threshold = 0.01f;
                float turnSpeed = maxTurnSpeed * Mathf.Deg2Rad * Time.deltaTime * 1.1f;
                if (boid.clamping && lastClamp)
                {
                    turning = true;
                    if (bankAngle < -threshold)
                    {
                        if (turnAngle > -maxTurnAngleRadians)
                        {
                            // Turn left
                            head.transform.RotateAround(transform.TransformPoint(headRotPoint), transform.up, -turnSpeed);
                            tail.transform.RotateAround(transform.TransformPoint(tailRotPoint), transform.up, turnSpeed);
                            myBankAngle--;
                        }
                    }
                    else if (bankAngle > threshold)
                    {
                        if (turnAngle < maxTurnAngleRadians)
                        {
                            // Turn right
                            head.transform.RotateAround(transform.TransformPoint(headRotPoint), transform.up, turnSpeed);
                            tail.transform.RotateAround(transform.TransformPoint(tailRotPoint), transform.up, -turnSpeed);
                            myBankAngle++;
                        }
                    }
                }
                else
                {
                    // One degree threshold
                    float turnThreshold = 1.0f * Mathf.Deg2Rad; 
                    // Straighten up
                    float rot = 0;
                    if (turnAngle < -turnThreshold)
                    {
                        rot = turnSpeed;
                        myBankAngle += 1;
                    }
                    if (turnAngle > turnThreshold)
                    {
                        rot = -turnSpeed;
                        myBankAngle -= 1;
                    }
                    head.transform.RotateAround(transform.TransformPoint(headRotPoint), transform.up, rot);
                    tail.transform.RotateAround(transform.TransformPoint(tailRotPoint), transform.up, -rot);                    
                }
            }

            if (forwardWiggle)
            {
                float speed = (boidSpeedToAnimationSpeed) ? boid.velocity.magnitude : 20.0f;
                theta += speed * angularVelocity * Time.deltaTime * speedMultiplier;
                if (theta >= Mathf.PI * 2.0f)
                {
                    theta -= (Mathf.PI * 2.0f);
                }
                // Animate the head            
                float headRot = Mathf.Sin(theta) * headField;
                head.transform.RotateAround(transform.TransformPoint(headRotPoint), head.transform.right, -(headRot - oldHeadRot));

                oldHeadRot = headRot;
                // Animate the tail
                float tailThetaOffset = 0;
                float tailRot = Mathf.Sin(theta + tailThetaOffset) * tailField;
                tail.transform.RotateAround(transform.TransformPoint(tailRotPoint), tail.transform.right, tailRot - oldTailRot);
                oldTailRot = tailRot;

                // Wiggle the body
                float wiggleThetaOffset = Mathf.PI / 6.0f;
                float bodyWiggle = Mathf.Sin(theta + wiggleThetaOffset) * bodyWiggleHeight;
                Vector3 bodyPos = transform.position;
                bodyPos -= (bodyWiggle - lastBodyWiggle) * transform.parent.up;
                transform.position = bodyPos;
                lastBodyWiggle = bodyWiggle;

                float bodyThetaOffset = Mathf.PI / 2.0f;
                float bodyRot = Mathf.Sin(theta + bodyThetaOffset) * bodyField;
                Vector3 rot = new Vector3(bodyRot - oldBodyRot, 0, 0);
                transform.Rotate(rot);
                //transform.Rotate(transform.right, bodyRot - oldBodyRot);
                oldBodyRot = bodyRot;                
            }
            lastClamp = boid.clamping;
            /*BoidManager.PrintVector("My Right: ", transform.right);
            BoidManager.PrintVector("Parent Rigfht: ", transform.parent.right);
            BoidManager.PrintMessage(transform.right == transform.parent.right ? "Same" : "Different");
            BoidManager.PrintVector("My up: ", transform.up);
            BoidManager.PrintVector("Parent up: ", transform.parent.up);
             */
        }
    }
}
