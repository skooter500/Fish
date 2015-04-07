using UnityEngine;
using System.Collections.Generic;

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

        for (int i = 0 ; i < transform.childCount ; i ++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            for (int j = 0; j < child.transform.childCount; j++)
            {
                GameObject segment = child.transform.GetChild(j).gameObject;
                segment.GetComponent<Renderer>().material.color = color;
                sequence.Add(color);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
