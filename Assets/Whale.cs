using UnityEngine;
using System.Collections;

public class Whale : MonoBehaviour {
    SectionColours sectioncolors;
    ColorLerper lerper;

    // Use this for initialization
	void Start () {
        sectioncolors = GetComponent<SectionColours>();
        lerper = GetComponent<ColorLerper>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
