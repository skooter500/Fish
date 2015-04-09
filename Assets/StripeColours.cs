using UnityEngine;
using System.Collections.Generic;
using BGE;

[RequireComponent(typeof(Flock))]

public class StripeColours : MonoBehaviour {

    public int numStripes;
    public List<Color> sequence;

    public StripeColours()
    {
        sequence = new List<Color>();
        numStripes = 2;
    }

	// Use this for initialization
	void Start () {

        for (int i = 0; i < numStripes; i ++)
        {
            Color color = Pallette.Random();
            sequence.Add(color);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            for (int j = 0; j < child.transform.childCount; j++)
            {
                GameObject segment = child.transform.GetChild(j).gameObject;
                segment.GetComponent<Renderer>().material.color =  sequence[j % numStripes];
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
