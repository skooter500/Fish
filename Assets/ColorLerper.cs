using UnityEngine;
using System.Collections.Generic;

public class ColorLerper : MonoBehaviour {

    [HideInInspector]
    public List<GameObject> gameObjects;
    public List<Color> from;
    public List<Color> to;
    public float t;
    public float speed;
	// Use this for initialization
	void Start () {
        t = 1.0f;
        speed = 0.2f;
	}
	
	// Update is called once per frame
	void Update () {
	    if (t >= 1.0f)
        {
            return;
        }
        for (int i = 0 ; i < gameObjects.Count ; i ++)
        {
            GameObject gameObject = gameObjects[i];
            Color fromColor = from[i];
            Color toColor = to[i];
            Color myColor = new Color();
            myColor.r = Mathf.Lerp(fromColor.r, toColor.r, t);
            myColor.g = Mathf.Lerp(fromColor.g, toColor.g, t);
            myColor.b = Mathf.Lerp(fromColor.b, toColor.b, t);
            t += Time.deltaTime * speed;
            gameObject.GetComponent<Renderer>().material.color = myColor;
        }
	}

    public void StartLerping()
    {
        t = 0;
        from.Clear();
        foreach(GameObject gameObject in gameObjects)
        {
            from.Add(gameObject.GetComponent<Renderer>().material.color);
        }
    }
}
