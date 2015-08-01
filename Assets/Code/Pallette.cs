using UnityEngine;
using BGE;
using System.Collections.Generic;

public class Pallette : MonoBehaviour {
    List<Color> pallette;

    public static Pallette Instance;
    Pallette()
    {
        Instance = this;

        pallette = new List<Color>();

        pallette.Add(Color.red);
        pallette.Add(Color.yellow);
        pallette.Add(Color.cyan);
        pallette.Add(Color.magenta);
        pallette.Add(Color.green);
        pallette.Add(Utilities.ColorFromRGB("FF9900"));

        
    }

    public static Color Random()
    {
        return new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
        //int i = UnityEngine.Random.Range(0, Instance.pallette.Count - 1);
        //return Instance.pallette[i];
    }

    public static Color RandomNot(Color c)
    {
        return new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
        /*
        int i = 0;
        do
        {
            i = UnityEngine.Random.Range(0, Instance.pallette.Count - 1);
        }
        while (Instance.pallette[i] == c);
        return Instance.pallette[i];
         */
    }

    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
