using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BGE;

public class SectionColours : MonoBehaviour {
    List<GameObject> segments; // The list of children objects in order of Z

    SectionColours()
    {
        segments = new List<GameObject>();
    }

	// Use this for initialization
	void Start () {
        int count = transform.childCount;
        for(int i = 0 ; i < count ; i ++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Renderer render = child.GetComponent<Renderer>();
            if (render != null)
            {
                segments.Add(child);
            }
        }
        segments.Sort(new Sorter());

        foreach (GameObject segment in segments)
        {
            Renderer render = segment.GetComponent<Renderer>();
            if (render != null)
            {
                render.material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            }
        }
	}

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
                Renderer current = segments[i].GetComponent<Renderer>();
                Renderer previous = segments[i - 1].GetComponent<Renderer>();

                if (current != null && previous != null)
                {
                    current.material.color = previous.material.color;
                }
                yield return new WaitForSeconds(fleeColourWait);
            }
            segments[0].GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
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
	
	// Update is called once per frame
	void Update () {
        float fleeForce = GetComponent<Boid>().fleeForce.magnitude;
        if (fleeForce > 0)
        {
            fleeColourWait = 0.1f; 
            if (!fleeColourStarted)
            {
                StartCoroutine("FleeColourCycle");
            }
        }
	}

    class Sorter:IComparer<GameObject>
    {
        public int Compare(GameObject a, GameObject b)
        {
            if (a.transform.position.z == b.transform.position.z)
            {
                return 0;
            }
            return (a.transform.position.z > b.transform.position.z) ? 1 : - 1;
        }
    }
}
