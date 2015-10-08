using UnityEngine;
using System.Collections.Generic;
using BGE;

public class Mother : MonoBehaviour {
    public List<GameObject> formPrefabs;
    [HideInInspector]
    public List<GameObject> forms;
    public List<GameObject> formControlers; // These control the forms...

    public float radius;
    public float gap;
    public float rings;
    public int petals;

    public float activateDistance;

    public Mother()
    {
        formPrefabs = new List<GameObject>();
        forms = new List<GameObject>();
        radius = 1000;
        gap = 2000;
        rings = 5;
        petals = 1;
        activateDistance = 2000;
    }
	// Use this for initialization
	void Start () {
        
        GameObject lastBigFlock = null;
        int lastPrefabIndex = -1;
        for (int ring = 1; ring <= rings; ring++)
        {
            int petalsInRing = (int)petals * ring;
            float thetaInc = (Mathf.PI * 2.0f) / petalsInRing;
            float theta = Random.Range(0, 2.0f * Mathf.PI);
            for (int i = 0; i < petalsInRing; i++)
            {

                GameObject prefab = null;
                
                Vector3 position = new Vector3();
                float formRadius = gap * ring + radius;
                position.x = transform.position.x + (Mathf.Sin(theta) * formRadius);
                position.z = transform.position.z + (Mathf.Cos(theta) * formRadius);
                position.y = 0;

                int prefabIndex;
                do
                {
                    prefabIndex = Random.Range(0, formPrefabs.Count);
                    prefab = formPrefabs[prefabIndex];
                }
                while (lastPrefabIndex == prefabIndex && prefabIndex == 0 && lastBigFlock != null && Vector3.Distance(position, lastBigFlock.transform.position) < activateDistance);
                
                GameObject form = Instantiate<GameObject>(prefab);
                 // +Random.Range(-1500, 500);
                form.transform.position = position;                
                form.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * theta, Vector3.up);
                form.transform.parent = transform;

                theta += thetaInc;
                forms.Add(form);

                GameObject formController = AddFormBoid(form, formRadius);
                formController.transform.parent = transform;
                formControlers.Add(formController);
                lastPrefabIndex = prefabIndex;
            }
        }
	}
	
    GameObject AddFormBoid(GameObject form, float radius)
    {
        GameObject flockBoidControllerObject = new GameObject();
        flockBoidControllerObject.transform.position = form.transform.position;
        flockBoidControllerObject.transform.rotation = form.transform.rotation;
        flockBoidControllerObject.transform.parent = transform;
        BGE.Boid boid = flockBoidControllerObject.AddComponent<BGE.Boid>();
        FlockBoidController boidFlockController = flockBoidControllerObject.AddComponent<FlockBoidController>();
        boidFlockController.flock = form.GetComponent<BGE.Flock>();
        Vector3 transPoint = form.transform.position - transform.position;
        
        int numPoints = (int) (10.0f * 2.0f * Mathf.PI);

        float thetaInc = (Mathf.PI * 2.0f) / numPoints;
        Vector3 lastPoint = transPoint;
        for (int i = 0 ; i <= numPoints ; i ++)
        {

            Vector3 point = (Quaternion.AngleAxis(thetaInc * Mathf.Rad2Deg,  Vector3.up) * lastPoint);
            boid.path.Waypoints.Add(point + transform.position); 
            lastPoint = point;            
        }
        boid.followPathEnabled = true;
        boid.drawVectors = false;
        boid.path.looped = true;
        boid.drawGizmos = false;
        boid.maxSpeed = 20.0f;
        boid.maxForce = 20.0f;

        return flockBoidControllerObject;
    }
	
	void Update () {
        // Deactivate forms that are too far from the player to be perceived
        int formsActive = 0;
	    for (int i = 0 ; i < forms.Count ; i ++)
        {
            float distToPlayer = Vector3.Distance(Player.Instance.transform.position, formControlers[i].transform.position);
            if (distToPlayer < activateDistance)
            {
                if (!forms[i].activeSelf)
                {                    
                    forms[i].SetActive(true);
                    Vector3 oldFlockCenter = forms[i].GetComponent<Flock>().oldFlockCenter;
                    Vector3 translateBy = formControlers[i].transform.position - oldFlockCenter;
                    forms[i].transform.position += translateBy;
                    forms[i].GetComponent<Flock>().flockCenter = formControlers[i].transform.position;
                }
                BGE.BoidManager.PrintVector("FormPos: ", forms[i].transform.position);
                BGE.BoidManager.PrintFloat("Distance: ", Vector3.Distance(Player.Instance.transform.position, forms[i].transform.position));
                formsActive++;

            }
            else
            {
                if (forms[i].activeSelf)
                {
                    // Remember the old position
                    forms[i].GetComponent<Flock>().oldFlockCenter = forms[i].GetComponent<Flock>().flockCenter;
                    forms[i].SetActive(false);
                }
            }
        }
        BGE.BoidManager.PrintFloat("Forms active: ", formsActive);
	}
}
