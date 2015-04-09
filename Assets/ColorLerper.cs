using UnityEngine;
using System.Collections.Generic;

public class ColorLerper : MonoBehaviour {

    [HideInInspector]
    public List<GameObject> gameObjects;
    public List<Color> from;
    public List<Color> to;
    public float t = 1.0f;
    public float speed;
	// Use this for initialization
	void Start () {
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
            Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = myColor;
            }
            else
            {
                Debug.Log("No renderer in lerper");
            }
            
        }
	}

    public void StartLerping()
    {
        t = 0;
        from.Clear();
        foreach(GameObject gameObject in gameObjects)
        {
            from.Add(gameObject.GetComponentInChildren<Renderer>().material.color);
        }
    }
}
