using UnityEngine;
using System.Collections.Generic;
using BGE;

[RequireComponent(typeof(Flock))]

public class StripeColours : MonoBehaviour {

    public int numStripes;
    public List<Color> sequence;

    ColorLerper lerper;
    List<GameObject> segments; // The list of children objects in order of Z

    public StripeColours()
    {
        sequence = new List<Color>();
        segments = new List<GameObject>();
        numStripes = 2;
    }

	// Use this for initialization
	void Start () {

        for (int i = 0; i < numStripes; i ++)
        {
            Color color = Pallette.Random();
            sequence.Add(color);
        }

        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Renderer render = child.GetComponent<Renderer>();
            if (render != null)
            {
                segments.Add(child);
            }
        }
        segments.Sort(new Sorter());
     
        for (int i = 0; i < segments.Count; i++)
        {
            Color col = sequence[i % numStripes];
            Utilities.RecursiveSetColor(segments[i], col);

        }

        lerper = GetComponent<ColorLerper>();

        StartCoroutine("CycleOnTimer");
	}

    System.Collections.IEnumerator CycleOnTimer()
    {
        while (true)
        {
            CycleColours();
            yield return new WaitForSeconds(2.5f);
        }
    }

    [HideInInspector]
    public int sequenceNumber = 0;

    public void CycleColours()
    {
        if (lerper == null)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                Utilities.RecursiveSetColor(segments[i], sequence[i % numStripes]);
            }
        }
        else
        {
            lerper.to.Clear();
            for (int i = 0; i < segments.Count; i ++ )
            {
                lerper.to.Add(sequence[(i + sequenceNumber) % numStripes]);
            }

            sequenceNumber++;
            lerper.gameObjects = segments;
            lerper.speed = 0.1f;
            lerper.StartLerping();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    class Sorter : IComparer<GameObject>
    {
        public int Compare(GameObject a, GameObject b)
        {
            if (a.transform.position.z == b.transform.position.z)
            {
                return 0;
            }
            return (a.transform.position.z > b.transform.position.z) ? 1 : -1;
        }
    }
}
