using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BGE;

public class SectionColours : MonoBehaviour {
    List<GameObject> segments; // The list of children objects in order of Z

    public bool lerpColors;

    Boid boid;

    SectionColours()
    {
        segments = new List<GameObject>();
    }

	// Use this for initialization
	void Start () {

        boid = GetComponent<Boid>();

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
                render.material.color = Pallette.Random();
            }
        }
	}

    private float fleeColourWait;
    private bool fleeColourStarted;

    public void CycleColours()
    {
        ColorLerper lerper = null;

        if (lerpColors)
        {
            lerper = GetComponent<ColorLerper>();
            lerper.Clear();
            lerper.StartLerping();
        }

        for (int i = segments.Count - 1; i > 0; i--)
        {
            Renderer current = segments[i].GetComponent<Renderer>();
            Renderer previous = segments[i - 1].GetComponent<Renderer>();

            if (current != null && previous != null)
            {
                if (lerpColors)
                {
                    lerper.gameObjects.Add(segments[i]);
                    lerper.from.Add(current.material.color);
                    lerper.to.Add(previous.material.color);
                }
                else
                {
                    current.material.color = previous.material.color;
                }
            }
        }
        if (lerpColors)
        {
            lerper.gameObjects.Add(segments[0]);
            lerper.from.Add(segments[0].GetComponent<Renderer>().material.color);
            lerper.to.Add(Pallette.Random());            
            lerper.StartLerping();
        }
        else
        {
            segments[0].GetComponent<Renderer>().material.color = Pallette.RandomNot(segments[0].GetComponent<Renderer>().material.color);
        }
    }

    System.Collections.IEnumerator FleeColourCycle()
    {
        fleeColourStarted = true;
        while (true)
        {
            if (boid == null || boid.fleeForce.magnitude == 0)
            {
                break;
            }
            
            foreach (GameObject segment in segments)
            {
                segment.GetComponent<Renderer>().material.color = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.0f, 0.0f), Random.Range(0.0f, 0.0f));
            }
            yield return new WaitForSeconds(fleeColourWait);
            foreach (GameObject segment in segments)
            {
                segment.GetComponent<Renderer>().material.color = Pallette.Random();
            }
            yield return new WaitForSeconds(fleeColourWait);
             
        }
        fleeColourStarted = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (boid != null)
        {
            float fleeForce = boid.fleeForce.magnitude;
            if (fleeForce > 0)
            {
                fleeColourWait = 0.3f;
                if (!fleeColourStarted)
                {
                    StartCoroutine("FleeColourCycle");
                }
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
