using UnityEngine;
using System.Collections.Generic;
using BGE;
using Gamelogic.Colors;

public class DolphinSchool : MonoBehaviour {

    public GameObject dolphinPrefab;

    public float minGap;    
    public int dolphinCount;
    public float radius;
    public float wanderRadius;
    public float heightAbove = 500;

    [HideInInspector]
    public GameObject leader;

    PaletteGenerator pg;

    float fov = 45.0f;
    public float minDist = 5000;
    public float maxDist = 10000;

    Vector3 dest;

    NoiseForm noiseForm;
    public int nextLifeForm;
    [HideInInspector]
    public List<GameObject> forms;

    public List<GameObject> formPrefabs;

    public DolphinSchool()
    {
        minGap = 200;
        dolphinCount = 10;
        radius = 300;
        wanderRadius = 10000;
    }


    void MakeNewPath(GameObject leader)
    {
        Quaternion q = Quaternion.AngleAxis(Random.Range(-fov, fov), leader.transform.up);

        Vector3 oldDest = dest;
        Vector3 newForward = q * leader.transform.forward;
        dest = leader.transform.position + newForward * Random.Range(minDist, maxDist);
        dest.y = noiseForm.GetHeight(dest) + heightAbove;
        Boid boid = leader.GetComponent<Boid>();
        //boid.path.Clear();
        boid.followPathEnabled = true;
        boid.drawGizmos = true;
        Vector3 waypoint = oldDest;
        Vector3 toDest = (dest - oldDest).normalized;
        while(Vector3.Distance(waypoint, dest) > 200)
        {
            boid.path.Waypoints.Add(waypoint);
            waypoint += toDest * 100;
            waypoint.y = noiseForm.GetHeight(waypoint) + heightAbove;

            if (boid.path.Waypoints.Count % 50 == 0)
            {
                CreateLifeForm(waypoint);
            }
        }
    }

    private GameObject CreateLifeForm(Vector3 pos)
    {
        pos.y = noiseForm.GetHeight(pos) + 500;
        GameObject form = Instantiate(formPrefabs[nextLifeForm]);
        form.SetActive(true);
        form.transform.position = pos;
        nextLifeForm = (nextLifeForm + 1) % formPrefabs.Count;
        forms.Add(form);
        form.transform.parent = transform;
        return form;
    }

    void MakePath(GameObject leader)
    {
        Boid boid = leader.GetComponent<Boid>();
        boid.followPathEnabled = true;
        int numWaypoints = 50;

        float theta = 0.0f;
        float thetaInc = (Mathf.PI * 2.0f) / numWaypoints;
        for (int i = 0; i <= numWaypoints; i++)
        {
            Vector3 waypoint = new Vector3();
            waypoint.x = transform.position.x + (Mathf.Sin(theta - (Mathf.PI * 0.5f)) * wanderRadius);
            waypoint.z = transform.position.z + (Mathf.Cos(theta - (Mathf.PI * 0.5f)) * wanderRadius);
            waypoint.y = 10000;

            RaycastHit hitInfo;
            bool collided = Physics.Raycast(waypoint, Vector3.down, out hitInfo);            
            if (collided)
            {
                waypoint.y = hitInfo.point.y + heightAbove;
            }
            
            theta += thetaInc;
            boid.path.Waypoints.Add(waypoint);
            boid.path.Looped = true;
            //boid.drawGizmos = true;
        }
        leader.transform.position = leader.GetComponent<Boid>().path.Waypoints[0];
    }

    System.Collections.IEnumerator SetUpDolphins()
    {
        yield return new WaitForSeconds(5);

        pg = GetComponent<PaletteGenerator>();
        noiseForm = FindObjectOfType<NoiseForm>();
        leader = Instantiate<GameObject>(dolphinPrefab);
        leader.transform.position = transform.position;
        MakeNewPath(leader);
        SetDolphinColors(leader);

        Vector3 lastPoint = leader.transform.position;
        for (int i = 0; i < dolphinCount; i++)
        {
            Vector3 newPoint;
            do
            {
                newPoint = leader.transform.position + UnityEngine.Random.insideUnitSphere * radius;
            }
            while (Vector3.Distance(newPoint, lastPoint) < minGap);
            GameObject dolphin = Instantiate<GameObject>(dolphinPrefab);
            dolphin.transform.position = newPoint;
            Boid boid = dolphin.GetComponent<Boid>();
            boid.offsetPursuitEnabled = true;
            boid.offsetPursuitTarget = leader;
            lastPoint = newPoint;
            DolphinParts dolphinParts = dolphin.GetComponentInChildren<DolphinParts>();
            dolphinParts.speedMultiplier += Random.Range(-0.005f, 0.005f);

            SetDolphinColors(dolphin);
        }
        BoidManager.Instance.cameraBoid = leader;

    }


    // Use this for initialization
    void Start () {

        StartCoroutine("SetUpDolphins");
       
	}

    Color RandomFromPalette()
    {
        return pg.palette.colors[Random.Range(0, pg.palette.colors.Length)];
    }

    void SetDolphinColors(GameObject dolphin)
    {
        DolphinParts dolphinParts = dolphin.GetComponentInChildren<DolphinParts>();
            
        Utilities.RecursiveSetColor(dolphinParts.tail, pg.palette.colors[2]);
        Utilities.RecursiveSetColor(dolphinParts.body, pg.palette.colors[1]);
        Utilities.RecursiveSetColor(dolphinParts.head, pg.palette.colors[0]);
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (leader != null)
        {
            float dist = Vector3.Distance(leader.transform.position, dest);
            Debug.Log("Distance: " + dist);
            if (dist < 2500)
            {
                MakeNewPath(leader);
            }
        }
        // Deactivate forms that are too far from the player to be perceived
        int formsActive = 0;
        float activateDistance = 5000;
        for (int i = 0; i < forms.Count; i++)
        {
            float distToPlayer = Vector3.Distance(Player.Instance.transform.position, forms[i].transform.position);
            if (distToPlayer < activateDistance)
            {
                if (!forms[i].activeSelf)
                {
                    forms[i].SetActive(true);
                }
                BGE.BoidManager.PrintVector("FormPos: ", forms[i].transform.position);
                BGE.BoidManager.PrintFloat("Distance: ", Vector3.Distance(Player.Instance.transform.position, forms[i].transform.position));
                formsActive++;

            }
            else
            {
                //Destroy()
                if (forms[i].activeSelf)
                {
                    forms[i].SetActive(false);
                }
            }
        }
        BGE.BoidManager.PrintFloat("Forms active: ", formsActive);
	}
}
