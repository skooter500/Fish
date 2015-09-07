using UnityEngine;
using BGE;
using System.Collections.Generic;
using Gamelogic.Colors;

public class Palette : MonoBehaviour {

    PaletteGenerator generator;

    public static Palette Instance;

    Palette()
    {
        Instance = this;
        
    }

    public static Color Random()
    {
        //return Instance.generator.palette.colors[UnityEngine.Random.Range(0, Instance.generator.palette.colors.Length)];
        return new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
    }

    public static Color RandomNot(Color c)
    {
        //return Instance.generator.palette.colors[UnityEngine.Random.Range(0, Instance.generator.palette.colors.Length)];

        return new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
    }

    
	// Use this for initialization
	void Start () {
        generator = GetComponent<PaletteGenerator>();                
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
