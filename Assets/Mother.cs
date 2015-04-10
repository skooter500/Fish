using UnityEngine;
using System.Collections.Generic;

public class Mother : MonoBehaviour {
    public List<GameObject> formPrefabs;
    [HideInInspector]
    public List<GameObject> forms;

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
        for (int ring = 1; ring <= rings; ring++)
        {
            int petalsInRing = (int)petals * ring;
            float thetaInc = (Mathf.PI * 2.0f) / petalsInRing;
            float theta = Random.Range(0, 2.0f * Mathf.PI);
            for (int i = 0; i < petalsInRing; i++)
            {
                GameObject prefab = formPrefabs[Random.Range(0, formPrefabs.Count)];
                GameObject form = Instantiate<GameObject>(prefab);
                Vector3 position = new Vector3();
                position.x = transform.position.x + radius + Mathf.Sin(theta) * gap * ring;
                position.z = transform.position.z + radius + Mathf.Cos(theta) * gap * ring;
                position.y = transform.position.y; // +Random.Range(-1500, 500);
                form.transform.position = position;
                form.transform.parent = transform;
                form.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * theta, Vector3.up);
                theta += thetaInc;
                forms.Add(form);
            }
        }
	}
	
	
	void Update () {
        // Deactivate forms that are too far from the player to be perceived
        int formsActive = 0;
	    for (int i = 0 ; i < forms.Count ; i ++)
        {
            float distToPlayer = Vector3.Distance(Player.Instance.transform.position, forms[i].transform.position);
            if (distToPlayer < activateDistance)
            {
                forms[i].SetActive(true);
                formsActive++;
            }
            else
            {
                forms[i].SetActive(false);
            }
        }
        BGE.BoidManager.PrintFloat("Forms active: ", formsActive);
	}
}
