using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BGE.Geom;

// Hello world

namespace BGE
{
    public class Boid : MonoBehaviour
    {
        // Variables required to implement the boid
        [Header("Boid Attributes")]
        public float mass = 1.0f;
        public float maxSpeed = 20.0f;
        public float maxForce = 10.0f;
        public float forceMultiplier = 1.0f;
        [Range(0.0f, 2.0f)]
        public float timeMultiplier = 1.0f;
        [Range(0.0f, 1.0f)]
        public float damping = 0.01f;
        public enum CalculationMethods { WeightedTruncatedSum, WeightedTruncatedRunningSumWithPrioritisation, PrioritisedDithering };
        public CalculationMethods calculationMethod = CalculationMethods.WeightedTruncatedRunningSumWithPrioritisation;
        public float radius = 5.0f;
        public float maxTurnDegrees = 180.0f;
        public bool applyBanking = true;
        public float straighteningTendancy = 0.2f;
        

        [HideInInspector]
        public Flock flock;

        public bool enforceNonPenetrationConstraint;

        [Header("Debugging")]
        public bool drawGizmos = false;
        public bool drawForces = false;
        public bool drawVectors = false;
        public bool drawNeighbours = false;

        [Header("Seek")]
        public bool seekEnabled = false;
        public Vector3 seekTargetPos = Vector3.zero;
        public float seekWeight = 1.0f;
        public bool seekPlayer = false;

        [Header("Flee")]
        public bool fleeEnabled;
        public float fleeWeight = 1.0f;
        public GameObject fleeTarget = null;
        public float fleeRange = 100.0f;
        [HideInInspector]
        public Vector3 fleeForce;

        [Header("Arrive")]
        public bool arriveEnabled = false;
        public Vector3 arriveTargetPos = Vector3.zero;
        public float arriveWeight = 1.0f;
        public float arriveSlowingDistance = 15.0f;
        [Range(0.0f, 1.0f)]
        public float arriveDeceleration = 0.9f;

        public enum WanderMethod { Jitter, Noise };
        [Header("Wander")]
        public bool wanderEnabled;
        public WanderMethod wanderMethod;
        public float wanderRadius = 10.0f;
        public float wanderJitter = 20.0f;
        public float wanderDistance = 15.0f;
        public float wanderWeight = 1.0f;
        public float wanderNoiseDeltaX = 0.5f;
        private float wanderNoiseX;
        private float wanderNoiseY;


        [Header("Separation")]
        public bool separationEnabled = false;
        public float separationWeight = 1.0f;

        [Header("Alignment")]
        public bool alignmentEnabled = false;
        public float alignmentWeight = 1.0f;

        [Header("Cohesion")]
        public bool cohesionEnabled = false;
        public float cohesionWeight = 1.0f;

        [Header("Obstacle Avoidance")]
        public bool obstacleAvoidanceEnabled = false;
        public float minBoxLength = 50.0f;
        public float obstacleAvoidanceWeight = 1.0f;

        [Header("Plane Avoidance")]
        public bool planeAvoidanceEnabled = false;
        public float planeAvoidanceWeight = 1.0f;
        public float planeY = 0;

        [Header("Follow Path")]
        public bool followPathEnabled = false;
        public float followPathWeight = 1.0f;
        public bool ignoreHeight = false;
        public Path path;

        [Header("Pursuit")]
        public bool pursuitEnabled = false;
        public GameObject pursuitTarget = null;
        public float pursuitWeight = 1.0f;
        private Vector3 pursuitTargetPos;

        [Header("Evade")]
        public bool evadeEnabled = false;
        public GameObject evadeTarget = null;
        public float evadeWeight = 1.0f;


        [Header("Offset Pursuit")]
        public bool offsetPursuitEnabled = false;
        public GameObject offsetPursuitTarget = null;
        public float offsetPursuitWeight = 1.0f;
        [Range(0.0f, 1.0f)]
        public float pitchForceScale = 1.0f;
        [HideInInspector]
        public Vector3 offset;
        private Vector3 offsetPursuitTargetPos = Vector3.zero;

        [Header("Sphere Constrain")]
        public bool sphereConstrainEnabled = false;
        public bool centreOnPosition = true;
        public Vector3 sphereCentre = Vector3.zero;
        public float sphereRadius = 1000.0f;
        public float sphereConstrainWeight = 1.0f;

        [Header("Random Walk")]
        public bool randomWalkEnabled = false;
        [HideInInspector]
        public Vector3 randomWalkCenter = Vector3.zero;
        public float randomWalkWaitMaxSeconds = 5.0f;
        private float randomWalkWait = 0.0f;
        public float randomWalkRadius = 1000.0f;
        public bool randomWalkKeepY = true;
        public float randomWalkWeight = 1.0f;
        private Vector3 randomWalkForce = Vector3.zero;

        [Header("Scene Avoidance")]
        public bool sceneAvoidanceEnabled = false;
        public float sceneAvoidanceWeight = 1.0f;
        public float sceneAvoidanceForwardFeelerDepth = 30;
        public float sceneAvoidanceSideFeelerDepth = 15;
        List<FeelerInfo> sceneAvoiodanceFeelers = new List<FeelerInfo>();

        [Header("Wiggle")]
        public float wiggleAngularSpeedDegrees = 30;
        public float wiggleAmplitude = 50;
        public bool wiggleEnabled = false;
        public float wiggleWeigth = 1.0f;
        public WiggleAxis wiggleDirection = WiggleAxis.Horizontal;
        public enum WiggleAxis { Horizontal, Vertical };
        private Vector3 wiggleWorldTarget = Vector3.zero;

        [HideInInspector]
        public Vector3 force = Vector3.zero;

        [HideInInspector]
        public Vector3 velocity = Vector3.zero;

        [HideInInspector]
        public Vector3 acceleration;

        List<GameObject> tagged = new List<GameObject>();
        List<Vector3> PlaneAvoidanceFeelers = new List<Vector3>();

        private Vector3 wanderTargetPos;
        private Vector3 randomWalkTarget;

        Color debugLineColour = Color.cyan;
        float timeDelta;

        Collider myCollider;

        public float rotateMax = Mathf.PI / 4.0f;

        public void OnDrawGizmos()
        {
            if (seekEnabled)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, seekTargetPos);
            }
            if (arriveEnabled)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, arriveTargetPos);
            }
            if (pursuitEnabled)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, pursuitTargetPos);
            }
            if (offsetPursuitEnabled)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, offsetPursuitTargetPos);
            }

            if (wanderEnabled)
            {
                Gizmos.color = Color.blue;
                Vector3 wanderCircleCenter = transform.TransformPoint(Vector3.forward * wanderDistance);
                Gizmos.DrawWireSphere(wanderCircleCenter, wanderRadius);
                Gizmos.color = Color.green;
                Vector3 worldTarget = transform.TransformPoint(wanderTargetPos + Vector3.forward * wanderDistance);
                Gizmos.DrawLine(transform.position, worldTarget);
            }

            if (wiggleEnabled)
            {
                LineDrawer.DrawTarget(wiggleWorldTarget, Color.red);
                Vector3 worldCenter = TransformPointNoScale(transform, Vector3.forward * wanderDistance);
                LineDrawer.DrawSphere(worldCenter, wanderRadius, 10, Color.yellow);
            }

            if (sceneAvoidanceEnabled)
            {
                if (drawGizmos)
                {
                    foreach (FeelerInfo feeler in sceneAvoiodanceFeelers)
                    {
                        LineDrawer.DrawLine(transform.position, transform.position + transform.TransformDirection(feeler.localDirection)
                            * feeler.depth, Color.cyan);
                    }
                }
            }

            if (sphereConstrainEnabled)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere((flock != null) ? flock.flockCenter : sphereCentre, sphereRadius);
            }
        }

        public Boid()
        {
            TurnOffAll();           
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
                offset = Quaternion.Inverse(transform.rotation) * offset;
            }

            wanderNoiseX = UnityEngine.Random.Range(0, 10000);
            wanderNoiseY = UnityEngine.Random.Range(0, 10000);

            randomWalkWait = UnityEngine.Random.Range(0, randomWalkWaitMaxSeconds);

            if (centreOnPosition)
            {
                sphereCentre = transform.position;
            }

            myCollider = GetComponentInChildren<Collider>();

            if (path == null)
            {
                path = GetComponent<Path>();
            }
        }

        #region Flags

        public void TurnOffAll()
        {
            seekEnabled = false;
            fleeEnabled = false;
            arriveEnabled = false;
            wanderEnabled = false;
            cohesionEnabled = false;
            separationEnabled = false;
            alignmentEnabled = false;
            obstacleAvoidanceEnabled = false;
            planeAvoidanceEnabled = false;
            followPathEnabled = false;
            pursuitEnabled = false;
            evadeEnabled = false;
            offsetPursuitEnabled = false;
            sphereConstrainEnabled = false;
            randomWalkEnabled = false;
            wiggleEnabled = false;
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

        Vector3 oldSeparation, oldAlignment, oldCohesion;

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
                    force = Flee(fleeTarget.transform.position, fleeRange) * fleeWeight;
                    force *= forceMultiplier;
                    fleeForce = force;
                }
                if (flock != null)
                {
                    force = Vector3.zero;
                    foreach (GameObject enemy in flock.enemies)
                    {
                        if (enemy != null)
                        {
                            force += Flee(enemy.transform.position, fleeRange) * fleeWeight;
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
                if (seekPlayer)
                {
                    force = Seek(Player.Instance.transform.position) * seekWeight;
                }
                else
                {
                    force = Seek(seekTargetPos) * seekWeight;
                }
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
                force = Pursuit() * pursuitWeight;
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
        
        Vector3 Wiggle()
        {

            float n = Mathf.Sin(wiggleTheta);
            float t = Utilities.Map(n, -1.0f, 1.0f, - wiggleAmplitude, wiggleAmplitude);
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
            wiggleWorldTarget = TransformPointNoScale(transform, localTarget);

            Vector3 worldTargetOnY = transform.position + Quaternion.Euler(yawRoll) * localTarget;            
            wiggleTheta += timeDelta * wiggleAngularSpeedDegrees * Mathf.Deg2Rad;
            if (wiggleTheta > Utilities.TWO_PI)
            {
                wiggleTheta = Utilities.TWO_PI - wiggleTheta;
            }

            return Seek(worldTargetOnY);
        }

        Vector3 CalculateIncidentForce(Vector3 point, Vector3 normal)
        {
            Vector3 desiredVelocity;
            desiredVelocity = point - transform.position;
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            Utilities.checkNaN(desiredVelocity);

            Vector3 force = Vector3.Reflect(desiredVelocity - velocity, -normal);
            return force;
        }

        struct FeelerInfo
        {
            public Vector3 localDirection;
            public float depth;
            public FeelerInfo(Vector3 localDirection, float depth)
            {
                this.localDirection = localDirection;
                this.depth = depth;
            }
        }

        Vector3 SceneAvoidance()
        {
            Vector3 force = Vector3.zero;
            RaycastHit info;
            Vector3 feelerDirection;
            bool collided = false;
            sceneAvoiodanceFeelers.Clear();
            
            float forwardFeelerDepth = sceneAvoidanceForwardFeelerDepth + ((velocity.magnitude / maxSpeed) * sceneAvoidanceForwardFeelerDepth);
            float sideFeelerDepth = sceneAvoidanceSideFeelerDepth + ((velocity.magnitude / maxSpeed) * sceneAvoidanceSideFeelerDepth);

            sceneAvoiodanceFeelers.Add(new FeelerInfo(Vector3.forward, forwardFeelerDepth));

            feelerDirection = Vector3.forward;
            feelerDirection = Quaternion.AngleAxis(-45, Vector3.up) * feelerDirection; // Left feeler
            sceneAvoiodanceFeelers.Add(new FeelerInfo(feelerDirection, sideFeelerDepth));

            feelerDirection = Vector3.forward;
            feelerDirection = Quaternion.AngleAxis(45, Vector3.up) * feelerDirection; // Right feeler
            sceneAvoiodanceFeelers.Add(new FeelerInfo(feelerDirection, sideFeelerDepth));

            feelerDirection = Vector3.forward;
            feelerDirection = Quaternion.AngleAxis(45, Vector3.right) * feelerDirection; // Up feeler
            sceneAvoiodanceFeelers.Add(new FeelerInfo(feelerDirection, sideFeelerDepth));

            feelerDirection = Vector3.forward;
            feelerDirection = Quaternion.AngleAxis(-45, Vector3.right) * feelerDirection; // Down feeler
            sceneAvoiodanceFeelers.Add(new FeelerInfo(feelerDirection, sideFeelerDepth));

            for (int i = 0; i < sceneAvoiodanceFeelers.Count; i++)
            {
                Vector3 feelerDir = transform.TransformDirection(sceneAvoiodanceFeelers[i].localDirection);
                float feelerDepth = sceneAvoiodanceFeelers[i].depth;
                collided = Physics.Raycast(transform.position, feelerDir, out info, feelerDepth);
                
                if (collided && info.collider != myCollider)
                {
                    force += CalculateIncidentForce(info.point, info.normal);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, transform.position + feelerDir * feelerDepth);
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(info.point, info.point + force);
                }
            }
            return force;
        }

        void Update()
        {
            bool calculateThisFrame = true;
            float smoothRate;

            timeDelta = Time.deltaTime * timeMultiplier;
            if (flock != null)

            {
                timeDelta *= flock.timeMultiplier;
            }

            

            if (calculateThisFrame)
            {
                force = Calculate();
            } // Otherwise use the value from the previous calculation

            if (drawForces)
            {
                Quaternion q = Quaternion.FromToRotation(Vector3.forward, force);
                LineDrawer.DrawArrowLine(transform.position, transform.position + force, Color.magenta, q);
            }
            Utilities.checkNaN(force);
            Vector3 newAcceleration = force / mass;
            
            if (timeDelta > 0.0f)
            {
                smoothRate = Utilities.Clip(9.0f * timeDelta, 0.15f, 0.4f) / 2.0f;
                Utilities.BlendIntoAccumulator(smoothRate, newAcceleration, ref acceleration);
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
            Vector3 globalUp = new Vector3(0, straighteningTendancy, 0);
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
                Vector3 forward = velocity.normalized;
                //forward.y = Mathf.Clamp(transform.forward.y, -0.5f, 0.25f);
                transform.forward = forward;
                /*transform.forward = Vector3.RotateTowards(
                    transform.forward,
                    forward,
                    rotateMax * Time.deltaTime,
                    1.0f
                );*/

                if (applyBanking)
                {
                    transform.LookAt(transform.position + transform.forward, tempUp);
                }
                // Apply damping
                velocity *= (1.0f - damping);
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

        
        Vector3 OffsetPursuit(Vector3 offset)
        {
            Vector3 target = Vector3.zero;

            target = TransformPointNoScale(offsetPursuitTarget.transform, offset);


            float dist = (target - transform.position).magnitude;

            float lookAhead = (dist / maxSpeed);

            target = target + (lookAhead * offsetPursuitTarget.GetComponent<Boid>().velocity);

            float pitchForce = target.y - transform.position.y;
            pitchForce *= (1.0f - pitchForceScale);
            target.y -= pitchForce;

            Utilities.checkNaN(target);
            return Seek(target);
        }

        Vector3 Pursuit()
        {
            Vector3 toTarget = pursuitTarget.transform.position - transform.position;
            float dist = toTarget.magnitude;
            float time = dist / maxSpeed;

            pursuitTargetPos = pursuitTarget.transform.position + (time * pursuitTarget.GetComponent<Boid>().velocity);            
            return Seek(pursuitTargetPos);
        }

        Vector3 Flee(Vector3 targetPos, float fleeRange)
        {
            Vector3 desiredVelocity;
            desiredVelocity = transform.position - targetPos;
            if (desiredVelocity.magnitude > fleeRange)
            {
                return Vector3.zero;
            }
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            Utilities.checkNaN(desiredVelocity);
            return (desiredVelocity - velocity);
        }

        Vector3 Flee(Vector3 targetPos)
        {
            Vector3 desiredVelocity;
            desiredVelocity = transform.position - targetPos;
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
                randomWalkTarget = randomWalkTarget = randomWalkCenter + r * randomWalkRadius;
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
            return transform.TransformPoint(localPoint);
            //Vector3 p = transform.rotation * localPoint;
            //p += transform.position;
            //return p;
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

            n = Mathf.PerlinNoise(wanderNoiseY, 0);
            theta = Utilities.Map(n, 0.0f, 1.0f, 0, Mathf.PI * 2.0f);

            wanderTargetPos.y = 0;
            wanderTargetPos *= wanderRadius;
            Vector3 localTarget = wanderTargetPos + (Vector3.forward * wanderDistance);
            Vector3 worldTarget = TransformPointNoScale(transform, localTarget);

            
            wanderNoiseX += wanderNoiseDeltaX * Time.deltaTime;
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
            float epsilon = 20.0f;
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
            if ((!path.looped) && path.IsLast())
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
            return steeringForce;
        }

        #endregion

        #region Flocking
        private int TagNeighboursSimple(float inRange)
        {
            if (flock != null)
            {
                float prob = UnityEngine.Random.Range(0.0f, 1.0f);
                if (prob > flock.tagDither)
                {
                    return tagged.Count;
                }
            }

            tagged.Clear();

            float inRangeSq = inRange * inRange;
            foreach (GameObject boid in flock.boids)
            {
                
                if (boid != gameObject)
                {
                    if (drawNeighbours)
                    {
                        LineDrawer.DrawLine(transform.position, boid.transform.position, Color.cyan);
                    }
                    if ((transform.position - boid.transform.position).sqrMagnitude < inRangeSq)
                    {
                        tagged.Add(boid);
                    }
                }
                if (tagged.Count > flock.maxTagged)
                {
                    break;
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