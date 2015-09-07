using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BGE.Geom;

namespace BGE
{
    public class Boid:MonoBehaviour
    {
        // Variables required to implement the boid
        [Header("Boid Attributes")]
        public float mass;
        public float maxSpeed;
        public float maxForce;
        public float forceMultiplier;
        [Range(0.0f, 2.0f)]
        public float timeMultiplier;
        [Range(0.0f, 1.0f)]
        public float damping;
        public enum CalculationMethods { WeightedTruncatedSum, WeightedTruncatedRunningSumWithPrioritisation, PrioritisedDithering };
        public CalculationMethods calculationMethod;
        public float radius;
        public float maxTurnDegrees;
        public bool applyBanking;

        [HideInInspector]
        public bool clamping;

        [HideInInspector]
        public Flock flock;

        public bool enforceNonPenetrationConstraint;

        [Header("Debugging")]
        public bool drawGizmos;
        public bool drawForces;
        public bool drawVectors;
        public bool drawNeighbours;
                
        [Header("Seek")]        
        public bool seekEnabled;
        public Vector3 seekTargetPos;
        public float seekWeight;
        
        [Header("Flee")]        
        public bool fleeEnabled;
        public float fleeWeight;
        public GameObject fleeTarget;
        public float fleeRange;
        [HideInInspector]
        public Vector3 fleeForce;

        [Header("Arrive")]        
        public bool arriveEnabled;
        public Vector3 arriveTargetPos;
        public float arriveWeight;
        public float arriveSlowingDistance;
        [Range(0.0f, 1.0f)]
        public float arriveDeceleration;


        public enum WanderMethod { Jitter, Noise };
        [Header("Wander")]
        public bool wanderEnabled;
        public WanderMethod wanderMethod;
        public float wanderRadius;
        public float wanderJitter;
        public float wanderDistance;
        public float wanderWeight;
        public float wanderNoiseDeltaX;
        private float wanderNoiseX;
        private float wanderNoiseY;
       

        [Header("Separation")]                
        public bool separationEnabled;
        public float separationWeight;

        [Header("Alignment")]                
        public bool alignmentEnabled;
        public float alignmentWeight;

        [Header("Cohesion")]
        public bool cohesionEnabled;
        public float cohesionWeight;

        [Header("Obstacle Avoidance")]                
        public bool obstacleAvoidanceEnabled;
        public float minBoxLength;
        public float obstacleAvoidanceWeight;

        [Header("Plane Avoidance")]                
        public bool planeAvoidanceEnabled;
        public float planeAvoidanceWeight;
        public float planeY;

        [Header("Follow Path")]                        
        public bool followPathEnabled;
        public float followPathWeight;
        public bool ignoreHeight = false;
        public Path path = new Path();

        [Header("Pursuit")]
        public bool pursuitEnabled;
        public GameObject pursuitTarget;
        public float pursuitWeight;

        [Header("Evade")]                        
        public bool evadeEnabled;
        public GameObject evadeTarget;
        public float evadeWeight;

        [Header("Interpose")]                                
        public bool interposeEnabled;
        public float interposeWeight;

        [Header("Hide")]                                
        public bool hideEnabled;
        public float hideWeight;

        [Header("Min Distance Pursuit")]
        public bool minDistancePursuitEnabled;
        public float minDistancePursuitWeight;
        public float minDistance;

        [Header("Offset Pursuit")]                                
        public bool offsetPursuitEnabled;
        public GameObject offsetPursuitTarget;        
        public float offsetPursuitWeight;

        [HideInInspector]
        public Vector3 offset;

        [Header("Sphere Constrain")]
        public bool sphereConstrainEnabled;
        public bool centreOnPosition;
        public Vector3 sphereCentre;
        public float sphereRadius;
        public float sphereConstrainWeight;

        [Header("Random Walk")]
        public bool randomWalkEnabled;
        [HideInInspector]
        public Vector3 randomWalkCenter;
        public float randomWalkWaitMaxSeconds;
        private float randomWalkWait; 
        
        public float randomWalkRadius;
        public bool randomWalkKeepY;
        public float randomWalkWeight;
        private Vector3 randomWalkForce;

        [Header("Scene Avoidance")]
        public bool sceneAvoidanceEnabled;
        public float sceneAvoidanceWeight;
        public float sceneAvoidanceFeelerDepth;

        
        [HideInInspector]        
        public Vector3 force;
        
        [HideInInspector]
        public Vector3 velocity;

        [HideInInspector]        
        public Vector3 acceleration;
                        
        List<GameObject> tagged = new List<GameObject>();
        List<Vector3> PlaneAvoidanceFeelers = new List<Vector3>();
        List<Vector3> SceneAvoidanceFeelers = new List<Vector3>();

        private Vector3 wanderTargetPos;        
        private Vector3 randomWalkTarget;
        
        Color debugLineColour = Color.cyan;
        float timeDelta;

        Collider myCollider;

        [Header("Gravity")]
        public bool applyGravity = false;
        public Vector3 gravity = new Vector3(0, -9, 0);

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(sphereCentre, sphereRadius);
        }



        public Boid()
        {
            TurnOffAll();

            drawGizmos = false;
            drawForces = false;
            drawVectors = false;
            drawNeighbours = false;
            applyBanking = true;
            flock = null;
        
            // Set default values
            force = Vector3.zero;
            velocity = Vector3.zero;
            mass = 1.0f;
            damping = 0.01f;
            radius = 5.0f;
            forceMultiplier = 1.0f;
            timeMultiplier = 1.0f;
           
            calculationMethod = CalculationMethods.WeightedTruncatedRunningSumWithPrioritisation;
            
            seekTargetPos = Vector3.zero;
            seekWeight = 1.0f;

            fleeWeight = 1.0f;
            fleeTarget = null;
            fleeRange = 100.0f;

            arriveSlowingDistance = 15.0f;
            arriveDeceleration = 0.9f;
            arriveTargetPos = Vector3.zero;
            arriveWeight = 1.0f;

            //wanderMethod = WanderMethod.Jitter;
            wanderRadius = 10.0f;
            wanderDistance = 15.0f;
            wanderJitter = 20.0f;
            wanderWeight = 1.0f;
            
            
            wanderNoiseDeltaX = 0.01f;

            cohesionWeight = 1.0f;
            separationWeight = 1.0f;
            alignmentWeight = 1.0f;

            obstacleAvoidanceWeight = 1.0f;
            minBoxLength = 50.0f;

            followPathWeight = 1.0f;

            pursuitWeight = 1.0f;
            pursuitTarget = null;

            evadeTarget = null;
            evadeWeight = 1.0f;

            interposeWeight = 1.0f;
            hideWeight = 1.0f;

            planeAvoidanceWeight = 1.0f;

            offsetPursuitTarget = null;
            offsetPursuitWeight = 1.0f;

            sphereCentre = Vector3.zero;
            sphereConstrainWeight = 1.0f;
            centreOnPosition = true;
            sphereRadius = 1000.0f;

            randomWalkWeight = 1.0f;
            randomWalkCenter = Vector3.zero;
            randomWalkRadius = 500.0f;
            randomWalkKeepY = false;

            randomWalkForce = Vector3.zero;
            randomWalkWaitMaxSeconds = 5.0f;

            maxSpeed = 20;
            maxForce = 10;
            maxTurnDegrees = 180.0f;

            sceneAvoidanceEnabled = false;
            sceneAvoidanceWeight = 1.0f;
            sceneAvoidanceFeelerDepth = 30.0f;

            minDistancePursuitEnabled = false;
            minDistancePursuitWeight = 1.0f;
            minDistance = 5.0f;

            planeY = 0.0f;
        }

        void Start()
        {
            randomWalkCenter = transform.position;
            randomWalkTarget = transform.position;
            //if (randomWalkKeepY)
            //{
            //    randomWalkTarget.y = transform.position.y;
            //}
            if (offsetPursuitTarget != null)
            {
                offset = transform.position - offsetPursuitTarget.transform.position;
            }

            wanderNoiseX = UnityEngine.Random.Range(0, 10000);
            wanderNoiseY = UnityEngine.Random.Range(0, 10000);

            randomWalkWait = UnityEngine.Random.Range(0, randomWalkWaitMaxSeconds);

            if (centreOnPosition)
            {
                sphereCentre = transform.position;
            }

            myCollider = GetComponentInChildren<Collider>();
        }

        #region Flags

        public void TurnOffAll()
        {
            seekEnabled= false;
            fleeEnabled= false;
            arriveEnabled= false;
            wanderEnabled= false;
            cohesionEnabled= false;
            separationEnabled= false;
            alignmentEnabled= false;
            obstacleAvoidanceEnabled= false;
            planeAvoidanceEnabled= false;
            followPathEnabled= false;
            pursuitEnabled= false;
            evadeEnabled= false;
            interposeEnabled= false;
            hideEnabled= false;
            offsetPursuitEnabled= false;
            sphereConstrainEnabled= false;
            randomWalkEnabled= false;
            wiggleEnabled = false;
            minDistancePursuitEnabled = false;
        }

        
#endregion

#region Utilities        
        private void makeFeelers()
        {
            PlaneAvoidanceFeelers.Clear();
            float feelerDistance = 50.0f;
            // Make the forward feeler
            Vector3 newFeeler = Vector3.forward * feelerDistance;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);

            newFeeler = Vector3.forward * feelerDistance;
            newFeeler = Quaternion.AngleAxis(45, Vector3.up) * newFeeler;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);

            newFeeler = Vector3.forward * feelerDistance;
            newFeeler = Quaternion.AngleAxis(-45, Vector3.up) * newFeeler;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);

            newFeeler = Vector3.forward * feelerDistance;
            newFeeler = Quaternion.AngleAxis(45, Vector3.right) * newFeeler;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);

            newFeeler = Vector3.forward * feelerDistance;
            newFeeler = Quaternion.AngleAxis(-45, Vector3.right) * newFeeler;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);
        }

        // These feelers are normals       

#endregion
#region Integration

        private bool accumulateForce(ref Vector3 runningTotal, Vector3 force)
        {
            float soFar = runningTotal.magnitude;

            float remaining = maxForce - soFar;
            if (remaining <= 0)
            {
                return false;
            }

            float toAdd = force.magnitude;


            if (toAdd < remaining)
            {
                runningTotal += force;
            }
            else
            {
                runningTotal += Vector3.Normalize(force) * remaining;
            }
            return true;
        }



        private void EnforceNonPenetrationConstraint()
        {
            Profiler.BeginSample("Non penetration");
            
            foreach (GameObject boid in tagged)
            {
                if (boid == gameObject)
                {
                    continue;
                }
                Vector3 toOther = boid.transform.position - gameObject.transform.position;
                float distance = toOther.magnitude;
                float overlap = radius + boid.GetComponent<Boid>().radius - distance;
                if (overlap >= 0)
                {
                    boid.transform.position = (boid.transform.position + (toOther / distance) *
                     overlap);
                }
            }
            Profiler.EndSample();
        }

        public Vector3 Calculate()
        {
            if (calculationMethod == CalculationMethods.WeightedTruncatedRunningSumWithPrioritisation)
            {
                return CalculateWeightedPrioritised();
            }

            return Vector3.zero;
        }

        private Vector3 CalculateWeightedPrioritised()
        {
            Vector3 force = Vector3.zero;
            Vector3 steeringForce = Vector3.zero;

            if (obstacleAvoidanceEnabled)
            {
                force = ObstacleAvoidance() * obstacleAvoidanceWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (sceneAvoidanceEnabled)
            {
                force = SceneAvoidance() * sceneAvoidanceWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            Utilities.checkNaN(force);
            if (planeAvoidanceEnabled)
            {
                force = PlaneAvoidance() * planeAvoidanceWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }


            if (sphereConstrainEnabled)
            {
                force = SphereConstrain(sphereRadius) * sphereConstrainWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (wiggleEnabled)
            {
                force = Wiggle() * wiggleWeigth;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (evadeEnabled)
            {
                force = Evade() * evadeWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (fleeEnabled)
            {
                if (fleeTarget != null)
                {
                    force = Flee(fleeTarget.transform.position) * fleeWeight;
                    force *= forceMultiplier;
                    fleeForce = force;
                }
                if (flock != null)
                {
                    force = Vector3.zero;
                    foreach(GameObject enemy in flock.enemies)
                    {
                        if (enemy != null)
                        {
                            force += Flee(enemy.transform.position) * fleeWeight;
                            force *= forceMultiplier;
                        }
                    }
                }
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (separationEnabled || cohesionEnabled || alignmentEnabled)
            {
                if (flock != null)
                {
                    Profiler.BeginSample("Tagging neighbours");   
                    if (flock.UseCellSpacePartitioning)
                    {
                        TagNeighboursPartitioned(flock.neighbourDistance);
                        /*
                        int testTagged = TagNeighboursSimple(neighbourDistance);
                        Debug.Log(tagged + "\t" + testTagged); // These numbers should be the same
                        if (tagged != testTagged)
                        {
                            Debug.Log("Different!!"); // These numbers should be the same                                          
                        }
                         */
                    }
                    else
                    {
                        TagNeighboursSimple(flock.neighbourDistance);
                    }
                    Profiler.EndSample();   
                    
                }
            }

            if (separationEnabled && (tagged.Count > 0))
            {
                Profiler.BeginSample("Separation");
                force = Separation() * separationWeight;
                force *= forceMultiplier;
                Profiler.EndSample();
                
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }

            }

            if (alignmentEnabled && (tagged.Count > 0))
            {
                Profiler.BeginSample("ALignment");                
                force = Alignment() * alignmentWeight;
                force *= forceMultiplier;
                Profiler.EndSample();
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (cohesionEnabled && (tagged.Count > 0))
            {
                Profiler.BeginSample("Cohesion");
                
                force = Cohesion() * cohesionWeight;
                force *= forceMultiplier;
                Profiler.EndSample();
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (seekEnabled)
            {
                force = Seek(seekTargetPos) * seekWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (arriveEnabled)
            {
                force = Arrive(arriveTargetPos) * arriveWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (wanderEnabled)
            {
                if (wanderMethod == WanderMethod.Jitter)
                {
                    force = Wander() * wanderWeight;
                }
                else
                {
                    force = NoiseWander() * wanderWeight;
                }
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            
            if (pursuitEnabled)
            {
                force = Pursue() * pursuitWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (offsetPursuitEnabled)
            {
                force = OffsetPursuit(offset) * offsetPursuitWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (minDistancePursuitEnabled)
            {
                force = MinDistancePursuit(minDistance) * minDistancePursuitWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            

            if (followPathEnabled)
            {
                force = FollowPath() * followPathWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (randomWalkEnabled)
            {
                force = RandomWalk() * randomWalkWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            return steeringForce;
        }


        float wiggleTheta = 0;

        [Header("Wiggle")]        
        public float wiggleAngularSpeedDegrees = 30;
        public float wiggleLowDegrees = -30;
        public float wiggleHighDegrees = 30;
        public bool wiggleEnabled = false;
        public float wiggleWeigth = 1.0f;
        public WiggleAxis wiggleDirection = WiggleAxis.Horizontal;
        public enum WiggleAxis { Horizontal, Vertical };

        Vector3 Wiggle()
        {

            float n = Mathf.Sin(wiggleTheta);
            float t = Utilities.Map(n, -1.0f, 1.0f, wiggleLowDegrees, wiggleHighDegrees);
            float theta = Mathf.Sin(Utilities.DegreesToRads(t));

            if (wiggleDirection == WiggleAxis.Horizontal)
            {
                wanderTargetPos.x = Mathf.Sin(theta);
                wanderTargetPos.z = Mathf.Cos(theta);
                wanderTargetPos.y = 0;
            }
            else
            {
                wanderTargetPos.y = Mathf.Sin(theta);
                wanderTargetPos.z = Mathf.Cos(theta);
                wanderTargetPos.x = 0;
            }


            wanderTargetPos *= wanderRadius;
            Vector3 yawRoll = transform.rotation.eulerAngles;
            yawRoll.x = 0;
            Vector3 localTarget = wanderTargetPos + (Vector3.forward * wanderDistance);
            Vector3 worldTarget = TransformPointNoScale(transform, localTarget);

            Vector3 worldTargetOnY = transform.position + Quaternion.Euler(yawRoll) * localTarget;

            BoidManager.PrintVector("World target: ", worldTarget);
            BoidManager.PrintVector("World target on y: ", worldTargetOnY);

            if (drawGizmos)
            {
                LineDrawer.DrawTarget(worldTarget, Color.red);
                LineDrawer.DrawTarget(worldTargetOnY, Color.yellow);

                Vector3 worldCenter = TransformPointNoScale(transform, Vector3.forward * wanderDistance);
                LineDrawer.DrawSphere(worldCenter, wanderRadius, 10, Color.yellow);
            }

            wiggleTheta += timeDelta * wiggleAngularSpeedDegrees * Mathf.Deg2Rad;
            if (wiggleTheta > Utilities.TWO_PI)
            {
                wiggleTheta = 0;
            }

            return Seek(worldTargetOnY);
        }

        Vector3 SceneAvoidance()
        {
            Vector3 force = Vector3.zero;
            RaycastHit info;

            // Check the forward feeler and generate a up force
            Vector3 feeler = Vector3.forward;
            feeler = transform.TransformDirection(feeler);
            bool collided = Physics.Raycast(transform.position, feeler, out info, sceneAvoidanceFeelerDepth);
            if (collided && info.collider != myCollider)
            {
                // Push me away in the direction of the up vector. 
                // The deeper the penetration the bigger the force
                float depth = (info.point - feeler).magnitude;
                force = Vector3.up * depth;
                if (drawGizmos)
                {
                    LineDrawer.DrawLine(transform.position, transform.position + feeler * sceneAvoidanceFeelerDepth, Color.red);
                }
            }
            
            // Check the bottom feeler and generate an upwards force
            //feeler = Vector3.forward;
            //feeler = Quaternion.AngleAxis(45, Vector3.right) * feeler;
            //feeler = transform.TransformDirection(feeler);
            //collided = Physics.Raycast(transform.position, feeler, out info, sceneAvoidanceFeelerDepth);
            feeler = Vector3.down;
            if (collided && info.collider != myCollider)
            {
                // Push me away in the direction of the up vector. 
                // The deeper the penetration the bigger the force
                float depth = (info.point - feeler).magnitude;
                force += Vector3.up * depth;
                if (drawGizmos)
                {
                    LineDrawer.DrawLine(transform.position, transform.position + feeler * sceneAvoidanceFeelerDepth, Color.red);
                }
            }

            // Check the top feeler and generate a downward force            
            feeler = Vector3.forward;
            feeler = Quaternion.AngleAxis(-45, Vector3.right) * feeler;
            feeler = transform.TransformDirection(feeler);
            collided = Physics.Raycast(transform.position, feeler, out info, sceneAvoidanceFeelerDepth);
            if (collided && info.collider != myCollider)
            {
                // Push me away in the direction of the up vector. 
                // The deeper the penetration the bigger the force
                float depth = (info.point - feeler).magnitude;
                force -= transform.up * depth;
                if (drawGizmos)
                {
                    LineDrawer.DrawLine(transform.position, transform.position + feeler * sceneAvoidanceFeelerDepth, Color.red);
                }
            }

            // Check the left feeler and generate a right force            
            feeler = Vector3.forward;
            feeler = Quaternion.AngleAxis(45, Vector3.right) * feeler;
            feeler = transform.TransformDirection(feeler);
            collided = Physics.Raycast(transform.position, feeler, out info, sceneAvoidanceFeelerDepth);
            if (collided && info.collider != myCollider)
            {
                // Push me away in the direction of the up vector. 
                // The deeper the penetration the bigger the force
                float depth = (info.point - feeler).magnitude;
                force += transform.right * depth;
                if (drawGizmos)
                {
                    LineDrawer.DrawLine(transform.position, transform.position + feeler * sceneAvoidanceFeelerDepth, Color.red);
                }
            }

            // Check the right feeler and generate a left force            
            feeler = Vector3.forward;
            feeler = Quaternion.AngleAxis(-45, Vector3.right) * feeler;
            feeler = transform.TransformDirection(feeler);
            collided = Physics.Raycast(transform.position, feeler, out info, sceneAvoidanceFeelerDepth);
            if (collided && info.collider != myCollider)
            {
                // Push me away in the direction of the up vector. 
                // The deeper the penetration the bigger the force
                float depth = (info.point - feeler).magnitude;
                force -= transform.right * depth;
                if (drawGizmos)
                {
                    LineDrawer.DrawLine(transform.position, transform.position + feeler * sceneAvoidanceFeelerDepth, Color.red);
                }
            }
            return force;
        }

        /*
        Vector3 SceneAvoidance()
        {
            Vector3 force = Vector3.zero;
            makeFeelers();
            float distance = 20;
            
            foreach(Vector3 feeler in PlaneAvoidanceFeelers)
            {
                RaycastHit info;
                Vector3 direction = feeler - transform.position;
                if (Physics.Raycast(transform.position, direction, out info, distance))
                {
                    // Push me away in the direction of the up vector. 
                    // The deeper the penetration the bigger the force
                    float depth = (info.point - feeler).magnitude;
                    force = transform.up * depth * 10.0f;
                }
                if (force.magnitude > 0)
                {
                    DrawFeelers();
                }
            }
            return force;
        }
        */

        float maxAngle = float.MinValue;
        [HideInInspector]
        public float bankAngle = 0.0f;
        void Update()
        {
            bool calculateThisFrame = true;
            float smoothRate;

            timeDelta = Time.deltaTime * timeMultiplier;
            if (flock != null)
            {
                timeDelta *= flock.timeMultiplier;
            }

            if (flock != null)
            {
                float prob = UnityEngine.Random.Range(0.0f, 1.0f);
                if (prob > flock.updateDither)
                {
                    calculateThisFrame = false;                    
                }
            }

            if (calculateThisFrame)
            {
                force = Calculate();
            } // Otherwise use the value from the previous calculation
            
            if (drawForces)
            {
                Quaternion q = Quaternion.FromToRotation(Vector3.forward, force);
                LineDrawer.DrawArrowLine(transform.position, transform.position + force * 5.0f, Color.magenta, q);
            }
            Utilities.checkNaN(force);
            Vector3 newAcceleration = force / mass;
            if (drawVectors)
            {
                LineDrawer.DrawVectors(transform);
            }

            
            if (timeDelta > 0.0f)
            {
                smoothRate = Utilities.Clip(9.0f * timeDelta, 0.15f, 0.4f) / 2.0f;
                Utilities.BlendIntoAccumulator(smoothRate, newAcceleration, ref acceleration);
            }

            if (applyGravity)
            {
                acceleration += gravity;
            }

            velocity += acceleration * timeDelta;

            float speed = velocity.magnitude;
            if (speed > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }
            Utilities.checkNaN(velocity);
            transform.position += velocity * timeDelta;


            // the length of this global-upward-pointing vector controls the vehicle's
            // tendency to right itself as it is rolled over from turning acceleration
            Vector3 globalUp = new Vector3(0, 1.2f, 0);
            // acceleration points toward the center of local path curvature, the
            // length determines how much the vehicle will roll while turning
            Vector3 accelUp = acceleration * 0.05f;
            // combined banking, sum of UP due to turning and global UP
            Vector3 bankUp = accelUp + globalUp;
            // blend bankUp into vehicle's UP basis vector
            smoothRate = timeDelta;// * 3.0f;
            Vector3 tempUp = transform.up;
            Utilities.BlendIntoAccumulator(smoothRate, bankUp, ref tempUp);
            
            if (speed > 0.01f)
            {
                float maxTurnFrame = maxTurnDegrees * Time.deltaTime;
                float maxTurn = maxTurnFrame * Mathf.Deg2Rad; // Max turn in rads
                float dot = Vector3.Dot(transform.forward, velocity.normalized);
                bankAngle = Mathf.Acos(dot);

                if (float.IsNaN(bankAngle))
                {
                    bankAngle = 0.0f;
                }

                float side = Vector3.Dot(transform.right, velocity.normalized);
                if (side < 0 ) // Angle < 90
                {
                    bankAngle = -bankAngle;
                } 

                if (Mathf.Abs(bankAngle) > maxTurn)
                {
                    clamping = true;
                    // Clamp the turn
                    Vector3 axis = Vector3.Cross(transform.forward, velocity.normalized);
                    Quaternion q = Quaternion.AngleAxis(maxTurnFrame, axis);
                    transform.forward = q * transform.forward;
                    velocity = transform.forward * velocity.magnitude;
                }
                else
                {
                    clamping = false;
                    transform.forward = velocity;
                    transform.forward.Normalize();
                }
                if (Mathf.Abs(bankAngle) > maxAngle)
                {
                    maxAngle = Mathf.Abs(bankAngle);
                }

                //BoidManager.PrintFloat("Rotation angle: ", angle * Mathf.Rad2Deg);
                //BoidManager.PrintFloat("Max angle: ", maxAngle * Mathf.Rad2Deg);
                if (applyBanking)
                {
                    transform.LookAt(transform.position + transform.forward, tempUp);               
                }
                 // Apply damping
                velocity *= (1.0f - damping);
            }

            if (path != null && drawGizmos)
            {
                path.draw = true;
                path.Draw();
            }

            if (calculateThisFrame && enforceNonPenetrationConstraint)
            {
                EnforceNonPenetrationConstraint();
            }

        }

        #endregion

        #region Behaviours

        Vector3 Seek(Vector3 targetPos)
        {
            Vector3 desiredVelocity;

            desiredVelocity = targetPos - transform.position;
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            if (drawGizmos)
            {
                //LineDrawer.DrawTarget(targetPos, Color.red);
            }
            return (desiredVelocity - velocity);
        }

        Vector3 Evade()
        {
            float dist = (evadeTarget.transform.position - transform.position).magnitude;
            float lookAhead = maxSpeed;

            Vector3 targetPos = evadeTarget.transform.position + (lookAhead * evadeTarget.GetComponent<Boid>().velocity);
            return Flee(targetPos);
        }

        Vector3 ObstacleAvoidance()
        {
            Vector3 force = Vector3.zero;
            //makeFeelers();
            List<Obstacle> tagged = new List<Obstacle>();
            float boxLength = minBoxLength + ((velocity.magnitude / maxSpeed) * minBoxLength * 2.0f);

            if (float.IsNaN(boxLength))
            {
                System.Console.WriteLine("NAN");
            }


            Obstacle[] obstacles = BoidManager.Instance.obstacles;
            // Matt Bucklands Obstacle avoidance
            // First tag obstacles in range
            if (obstacles.Length == 0)
            {
                return Vector3.zero;
            }
            foreach (Obstacle obstacle in obstacles)
            {
                //
                if (obstacle == null || obstacle.gameObject == gameObject)
                {
                    continue;
                }

                Vector3 toCentre = transform.position - obstacle.transform.position;
                float dist = toCentre.magnitude;
                if (dist < boxLength)
                {
                    tagged.Add(obstacle);
                }
            }

            float distToClosestIP = float.MaxValue;
            Obstacle closestIntersectingObstacle = null;
            Vector3 localPosOfClosestObstacle = Vector3.zero;
            Vector3 intersection = Vector3.zero;

            foreach (Obstacle o in tagged)
            {
                Vector3 localPos = transform.InverseTransformPoint(o.transform.position);

                // If the local position has a positive Z value then it must lay
                // behind the agent. (in which case it can be ignored)
                if (localPos.z >= 0)
                {
                    // If the distance from the x axis to the object's position is less
                    // than its radius + half the width of the detection box then there
                    // is a potential intersection.

                    //float obstacleRadius = o.transform.localScale.x / 2;
                    float obstacleRadius = o.radius;
                    float expandedRadius = radius + obstacleRadius;
                    if ((Math.Abs(localPos.y) < expandedRadius) && (Math.Abs(localPos.x) < expandedRadius))
                    {
                        // Now to do a ray/sphere intersection test. The center of the				
                        // Create a temp Entity to hold the sphere in local space
                        Sphere tempSphere = new Sphere(expandedRadius, localPos);

                        // Create a ray
                        BGE.Geom.Ray ray = new BGE.Geom.Ray();
                        ray.pos = new Vector3(0, 0, 0);
                        ray.look = Vector3.forward;

                        // Find the point of intersection
                        if (tempSphere.closestRayIntersects(ray, Vector3.zero, ref intersection) == false)
                        {
                            continue;
                        }

                        // Now see if its the closest, there may be other intersecting spheres
                        float dist = intersection.magnitude;
                        if (dist < distToClosestIP)
                        {
                            dist = distToClosestIP;
                            closestIntersectingObstacle = o;
                            localPosOfClosestObstacle = localPos;
                        }
                    }
                }
            }

            if (closestIntersectingObstacle != null)
            {
                // Now calculate the force
                float multiplier = 1.0f + (boxLength - localPosOfClosestObstacle.z) / boxLength;

                //calculate the lateral force
                float obstacleRadius = closestIntersectingObstacle.radius; // closestIntersectingObstacle.GetComponent<Renderer>().bounds.extents.magnitude;
                float expandedRadius = radius + obstacleRadius;
                force.x = (expandedRadius - Math.Abs(localPosOfClosestObstacle.x)) * multiplier;
                force.y = (expandedRadius - Math.Abs(localPosOfClosestObstacle.y)) * multiplier;

                // Generate positive or negative direction so we steer around!
                // Not always in the same direction as in Matt Bucklands book
                if (localPosOfClosestObstacle.x > 0)
                {
                    force.x = -force.x;
                }

                // If the obstacle is above, steer down
                if (localPosOfClosestObstacle.y > 0)
                {
                    force.y = -force.y;
                }

                if (drawGizmos)
                {
                    LineDrawer.DrawLine(transform.position, transform.position + transform.forward * boxLength, Color.grey);
                }
                //apply a braking force proportional to the obstacle's distance from
                //the vehicle.
                const float brakingWeight = 0.01f;
                force.z = (expandedRadius -
                                   localPosOfClosestObstacle.z) *
                                   brakingWeight;

                //finally, convert the steering vector from local to world space
                // Dont include position!                    
                force = transform.TransformDirection(force);
            }


            return force;
        }


        Vector3 MinDistancePursuit(float acceptableDistance)
        {
            acceptableDistance = 1;
            GameObject _quarry = offsetPursuitTarget;

            if (_quarry == null)
            {
                enabled = false;
                return Vector3.zero;
            }

            var force = Vector3.zero;
            var offset = _quarry.transform.position - transform.position;
            var distance = offset.magnitude;
            var radius = this.radius + _quarry.GetComponent<Boid>().radius + acceptableDistance;

            BoidManager.PrintFloat("Min distance: ", distance);

            if (distance < radius) return force;

            var unitOffset = offset / distance;

            // how parallel are the paths of "this" and the quarry
            // (1 means parallel, 0 is pependicular, -1 is anti-parallel)
            var parallelness = Vector3.Dot(transform.forward, _quarry.transform.forward);

            // how "forward" is the direction to the quarry#
            // (1 means dead ahead, 0 is directly to the side, -1 is straight back)
            var forwardness = Vector3.Dot(transform.forward, unitOffset);

            //var directTravelTime = distance / velocity.magnitude;
            var directTravelTime = distance / maxSpeed;
            // While we could parametrize this value, if we care about forward/backwards
            // these values are appropriate enough.
            var f = Utilities.IntervalComparison(forwardness, -0.707f, 0.707f);
            var p = Utilities.IntervalComparison(parallelness, -0.707f, 0.707f);

            float timeFactor = 0; // to be filled in below

            // Break the pursuit into nine cases, the cross product of the
            // quarry being [ahead, aside, or behind] us and heading
            // [parallel, perpendicular, or anti-parallel] to us.
            switch (f)
            {
                case +1:
                    switch (p)
                    {
                        case +1: // ahead, parallel
                            timeFactor = 4;
                            break;
                        case 0: // ahead, perpendicular
                            timeFactor = 1.8f;
                            break;
                        case -1: // ahead, anti-parallel
                            timeFactor = 0.85f;
                            break;
                    }
                    break;
                case 0:
                    switch (p)
                    {
                        case +1: // aside, parallel
                            timeFactor = 1;
                            break;
                        case 0: // aside, perpendicular
                            timeFactor = 0.8f;
                            break;
                        case -1: // aside, anti-parallel
                            timeFactor = 4;
                            break;
                    }
                    break;
                case -1:
                    switch (p)
                    {
                        case +1: // behind, parallel
                            timeFactor = 0.5f;
                            break;
                        case 0: // behind, perpendicular
                            timeFactor = 2;
                            break;
                        case -1: // behind, anti-parallel
                            timeFactor = 2;
                            break;
                    }
                    break;
            }

            // estimated time until intercept of quarry
            var et = directTravelTime * timeFactor;
            //var etl = (et > _maxPredictionTime) ? _maxPredictionTime : et;

            //var target = _quarry.PredictFuturePosition(etl);
            Vector3 target = offsetPursuitTarget.GetComponent<Boid>().transform.position + (et * offsetPursuitTarget.GetComponent<Boid>().velocity);
            
            // estimated position of quarry at intercept

            //force = Vehicle.GetSeekVector(target, _slowDownOnApproach);
            return Seek(target);
        }


        Vector3 OffsetPursuit(Vector3 offset)
        {
            Vector3 target = Vector3.zero;

            target = TransformPointNoScale(offsetPursuitTarget.transform, offset);
         
            float dist = (target - transform.position).magnitude;

            float lookAhead = (dist / maxSpeed);

            target = target + (lookAhead * offsetPursuitTarget.GetComponent<Boid>().velocity);
            Utilities.checkNaN(target);
            return Arrive(target);
        }

        Vector3 Pursue()
        {
            Vector3 toTarget = pursuitTarget.transform.position - transform.position;
            float dist = toTarget.magnitude;
            float time = dist / maxSpeed;

            Vector3 targetPos = pursuitTarget.transform.position + (time * pursuitTarget.GetComponent<Boid>().velocity);
            if (drawGizmos)
            {
                LineDrawer.DrawTarget(targetPos, Color.red);
                LineDrawer.DrawLine(transform.position, targetPos, Color.cyan);
            }

            return Seek(targetPos);
        }

        Vector3 Flee(Vector3 targetPos)
        {
            Vector3 desiredVelocity;
            desiredVelocity = transform.position - targetPos;
            if (desiredVelocity.magnitude > fleeRange)
            {
                return Vector3.zero;
            }
            if (drawGizmos)
            {
                LineDrawer.DrawSphere(transform.position, fleeRange, 20, Color.yellow);
            }
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            Utilities.checkNaN(desiredVelocity);
            return (desiredVelocity - velocity);
        }

        Vector3 RandomWalk()
        {
            float dist = (transform.position - randomWalkTarget).magnitude;
            if (dist < 30)
            {
                StartCoroutine("RandomWalkWait");
                float sphereRadius = (flock != null) ? flock.radius : randomWalkRadius;
                Vector3 r = UnityEngine.Random.insideUnitSphere;
                r.y = Mathf.Abs(r.y);
                randomWalkTarget = randomWalkTarget = randomWalkCenter +  r * randomWalkRadius;
            }
            if (randomWalkKeepY)
            {
                randomWalkTarget.y = transform.position.y;
            } 
            return Seek(randomWalkTarget);
        }

        IEnumerator RandomWalkWait()
        {
            randomWalkEnabled = false;
            yield return new WaitForSeconds(randomWalkWait);
            randomWalkEnabled = true;
            randomWalkWait = UnityEngine.Random.Range(0, randomWalkWaitMaxSeconds);
        }

        Vector3 TransformPointNoScale(Transform transform, Vector3 localPoint)
        {
            Vector3 p = transform.rotation * localPoint;
            p += transform.position;
            return p;
        }

        Vector3 Wander()
        {
            float jitterTimeSlice = wanderJitter * timeDelta;

            Vector3 toAdd = UnityEngine.Random.insideUnitSphere * jitterTimeSlice;
            wanderTargetPos += toAdd;
            wanderTargetPos.Normalize();
            wanderTargetPos *= wanderRadius;

            Vector3 localTarget = wanderTargetPos + Vector3.forward * wanderDistance;
            Vector3 worldTarget = transform.TransformPoint(localTarget);
            if (drawGizmos)
            {
                LineDrawer.DrawTarget(worldTarget, Color.blue);
            }
            return (worldTarget - transform.position);
        }
               
        Vector3 NoiseWander()
        {
            float n = Mathf.PerlinNoise(wanderNoiseX, 0);
            float theta = Utilities.Map(n, 0.0f, 1.0f, 0, Mathf.PI * 2.0f);
            wanderTargetPos.x = Mathf.Sin(theta);
            wanderTargetPos.z = -Mathf.Cos(theta);

            n = Mathf.PerlinNoise(wanderNoiseX, 0);
            theta = Utilities.Map(n, 0.0f, 1.0f, 0, Mathf.PI * 2.0f);
           
            wanderTargetPos.y = 0;
            wanderTargetPos *= wanderRadius;
            Vector3 localTarget = wanderTargetPos + (Vector3.forward * wanderDistance);
            Vector3 worldTarget = TransformPointNoScale(transform, localTarget);

            if (drawGizmos)
            {
                LineDrawer.DrawTarget(worldTarget, Color.red);
                Vector3 worldCenter = TransformPointNoScale(transform, Vector3.forward * wanderDistance);
                LineDrawer.DrawSphere(worldCenter, wanderRadius, 10, Color.yellow);
            }
            wanderNoiseX += wanderNoiseDeltaX;
            Vector3 desired = worldTarget - transform.position;
            desired.Normalize();
            desired *= maxSpeed;
            //return Vector3.zero;
            return desired - velocity;
        }
         

        // Wander with quaternions
        /*
        Vector3 Wander()
        {
            // Rotate the wandertargetpos a little each frame
            float jitterTimeSlice = wanderJitter * timeDelta;

            Quaternion q = Quaternion.AngleAxis(jitterTimeSlice, UnityEngine.Random.insideUnitSphere);
            wanderTargetPos = q * wanderTargetPos;
            wanderTargetPos = wanderTargetPos.normalized * wanderRadius;
            Vector3 localTarget = wanderTargetPos + (Vector3.forward * wanderDistance);
            Vector3 worldTarget = transform.TransformPoint(localTarget);
                        
            if (drawGizmos)
            {
                LineDrawer.DrawTarget(worldTarget, Color.red);
                Vector3 worldCenter = transform.TransformPoint(Vector3.forward * wanderDistance);
                LineDrawer.DrawSphere(worldCenter, wanderRadius, 10, Color.yellow);
            }
            //return Vector3.zero;
            return (worldTarget - transform.position);
        }
        */
        public Vector3 PlaneAvoidance()
        {
            makeFeelers();

            Plane worldPlane = new Plane(new Vector3(0, 1, 0), -planeY);           
            Vector3 force = Vector3.zero;
            foreach (Vector3 feeler in PlaneAvoidanceFeelers)
            {
                if (!worldPlane.GetSide(feeler))
                {
                    float distance = Math.Abs(worldPlane.GetDistanceToPoint(feeler));
                    force += worldPlane.normal * distance;
                }
            }

            if (force.magnitude > 0.0)
            {
                DrawFeelers();
            }
            return force;
        }

        public void DrawFeelers()
        {
            if (drawGizmos)
            {
                foreach (Vector3 feeler in PlaneAvoidanceFeelers)
                {
                    LineDrawer.DrawLine(transform.position, feeler, Color.green);
                }
            }            
        }

        public Vector3 Arrive(Vector3 target)
        {
            Vector3 desired = target - transform.position;

            float distance = desired.magnitude;
            //toTarget.Normalize();
            if (drawGizmos)
            {
                LineDrawer.DrawTarget(target, Color.red);
                LineDrawer.DrawSphere(target, arriveSlowingDistance, 20, Color.yellow);
            }

            if (distance < 1.0f)
            {
                return Vector3.zero;
            }            
            float desiredSpeed = 0;
            if (distance < arriveSlowingDistance)
            {
                desiredSpeed = (distance / arriveSlowingDistance) * maxSpeed * (1.0f - arriveDeceleration);
            }
            else
            {
                desiredSpeed = maxSpeed;
            }
            desired *= desiredSpeed;

            return desired - velocity;
        }

        private Vector3 FollowPath()
        {
            float epsilon = 200.0f;
            float dist;
            Vector3 nextWayPoint = path.NextWaypoint();

            if (ignoreHeight)
            {
                nextWayPoint.y = transform.position.y;
            }
            
            dist = (transform.position - path.NextWaypoint()).magnitude;
            
            if (dist < epsilon)
            {
                path.AdvanceToNext();
            }
            if ((!path.Looped) && path.IsLast())
            {
                return Arrive(path.NextWaypoint());
            }
            else
            {
                return Seek(path.NextWaypoint());
            }
        }

        public Vector3 SphereConstrain(float radius)
        {
            Vector3 toTarget = transform.position -
                ((flock != null) ? flock.flockCenter : sphereCentre);
            float sphereRadius = (flock != null) ? flock.radius : radius;
            Vector3 steeringForce = Vector3.zero;
            if (toTarget.magnitude > sphereRadius)
            {
                steeringForce = Vector3.Normalize(toTarget) * (sphereRadius - toTarget.magnitude);
            }
            if (drawGizmos)
            {
                LineDrawer.DrawSphere((flock != null) ? flock.flockCenter : sphereCentre, radius, 20, Color.green);
            }
            return steeringForce;
        }

        #endregion

        #region Flocking
        private int TagNeighboursSimple(float inRange)
        {
            tagged.Clear();

            foreach (GameObject boid in flock.boids)
            {
                if (boid != gameObject)
                {
                    if (drawNeighbours)
                    {
                        LineDrawer.DrawLine(transform.position, boid.transform.position, Color.cyan);
                    }
                    if ((transform.position - boid.transform.position).magnitude < inRange)
                    {
                        tagged.Add(boid);
                    }
                }
            }
            DrawNeighbours(Color.white);
            return tagged.Count;
        }

        private void DrawNeighbours(Color color)
        {
            if (drawNeighbours)
            {
                foreach (GameObject neighbour in tagged)
                {
                    LineDrawer.DrawCircle(neighbour.transform.position, 5, 10, color);
                }
                LineDrawer.DrawCircle(transform.position, 5, 10, Color.red);
            }            
        }

        private int TagNeighboursPartitioned(float inRange)
        {
            
            Bounds expanded = new Bounds();
            expanded.min = new Vector3(transform.position.x - inRange, 0, transform.position.z - inRange);
            expanded.max = new Vector3(transform.position.x + inRange, 0, transform.position.z + inRange);

            if (drawNeighbours)
            {
                LineDrawer.DrawSquare(expanded.min, expanded.max, Color.yellow);
            }

            List<Cell> cells = flock.space.cells;
            tagged.Clear();
            int myCellIndex = flock.space.FindCell(transform.position);
            if (myCellIndex == -1)
            {
                //Debug.Log("Not found in space");
                // Im outside the cells so return
                return 0;
            }
            else
            {
                if (drawGizmos)
                {
                    LineDrawer.DrawSquare(cells[myCellIndex].bounds.min, cells[myCellIndex].bounds.max, Color.green);
                }
            }
            Cell myCell = flock.space.cells[myCellIndex];

            //foreach (Cell cell in flock.space.cells)
            int border = 2;
            for (int row = myCell.row - border; row < myCell.row + border; row++)
            {
                for (int col = myCell.col - border; col < myCell.col + border; col++)
                {
                    Cell cell = flock.space.GetCell(row, col);
                    if (cell != null && cell.Intersects(expanded))
                    {
                        if (drawNeighbours)
                        {
                            LineDrawer.DrawSquare(cell.bounds.min, cell.bounds.max, Color.magenta);
                        }
                        List<GameObject> cellNeighbourBoids = cell.contained;
                        float rangeSquared = inRange * inRange;
                        foreach (GameObject neighbour in cellNeighbourBoids)
                        {
                            if (neighbour != gameObject)
                            {

                                if (drawNeighbours)
                                {
                                    LineDrawer.DrawLine(transform.position, neighbour.transform.position, Color.blue);
                                }
                                if (Vector3.SqrMagnitude(transform.position - neighbour.transform.position) < rangeSquared)
                                {
                                    tagged.Add(neighbour);
                                    
                                }
                            }
                        }
                    }
                }
            }

            DrawNeighbours(Color.white);

            return this.tagged.Count;
        }

        public Vector3 Separation()
        {            
            Vector3 steeringForce = Vector3.zero;
            foreach (GameObject entity in tagged)
            {
                if (entity != gameObject)
                {
                    Vector3 toEntity = transform.position - entity.transform.position;
                    steeringForce += (Vector3.Normalize(toEntity) / toEntity.magnitude);
                }
            }

            return steeringForce;
        }

        public Vector3 Cohesion()
        {
            Vector3 steeringForce = Vector3.zero;
            Vector3 centreOfMass = Vector3.zero;
            int taggedCount = 0;
            foreach (GameObject entity in tagged)
            {
                if (entity != gameObject)
                {
                    centreOfMass += entity.transform.position;
                    taggedCount++;
                }
            }
            if (taggedCount > 0)
            {
                centreOfMass /= (float)taggedCount;

                if (centreOfMass.sqrMagnitude == 0)
                {
                    steeringForce = Vector3.zero;
                }
                else
                {
                    steeringForce = Vector3.Normalize(Seek(centreOfMass));
                }
            }
            Utilities.checkNaN(steeringForce);
            return steeringForce;
        }

        public Vector3 Alignment()
        {
            Vector3 steeringForce = Vector3.zero;
            int taggedCount = 0;
            foreach (GameObject entity in tagged)
            {
                if (entity != gameObject)
                {
                    steeringForce += entity.transform.forward;
                    taggedCount++;
                }
            }

            if (taggedCount > 0)
            {
                steeringForce /= (float)taggedCount;
                steeringForce -= transform.forward;
            }
            return steeringForce;

        }
        #endregion Flocking        
    }
}
