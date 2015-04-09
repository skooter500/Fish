using System.Collections;
using UnityEngine;

public class ColorBomb : MonoBehaviour {

    public float bombDelayMin;
    public float bombDelayMax;
    public float bombSequenceDelay;
    public float expansionRate;

    [HideInInspector]
    BGE.Flock flock; 

    public ColorBomb()
    {
        bombDelayMin = 5.0f;
        bombDelayMax = 10.0f;
        bombSequenceDelay = 0.1f;
        expansionRate = 2;
    }

    private Vector3 CenterOfMass()
    {
        Vector3 center = Vector3.zero;

        foreach(GameObject boid in flock.boids)
        {
            center += boid.transform.position;
        }
        center /= flock.boids.Count;
        return center;
    }

    System.Collections.IEnumerator ColourCycle()
    {        
        while (true)
        {
            Vector3 center = CenterOfMass();
            Color color = Pallette.Random();
            Color color1 = Pallette.RandomNot(color);
            Color color2 = Pallette.RandomNot(color1);
            float radius = 20;
            int boidsTagged = 0;
            while (boidsTagged < flock.boids.Count)
            {
                boidsTagged = 0;
                foreach (GameObject boid in flock.boids)
                {
                    if (Vector3.Distance(center, boid.transform.position) < radius)
                    {
                        ColorLerper lerper = boid.GetComponent<ColorLerper>();
                        lerper.to.Clear();
                        lerper.to.Add(color);
                        lerper.to.Add(color1);
                        lerper.to.Add(color2);
                        lerper.gameObjects.Clear();
                        lerper.gameObjects.Add(boid.transform.GetChild(0).gameObject);
                        lerper.gameObjects.Add(boid.transform.GetChild(1).gameObject);
                        lerper.gameObjects.Add(boid.transform.GetChild(2).gameObject);
                        lerper.StartLerping();
                        //BGE.Utilities.RecursiveSetColor(boid.transform.GetChild(0).gameObject, color);
                        //BGE.Utilities.RecursiveSetColor(boid.transform.GetChild(1).gameObject, color1);
                        //BGE.Utilities.RecursiveSetColor(boid.transform.GetChild(2).gameObject, color2);
                        boidsTagged++;
                    }
                }
                radius += 5;
                yield return new WaitForSeconds(bombSequenceDelay);
            }
            yield return new WaitForSeconds(Random.Range(bombDelayMin, bombDelayMax));
        }
    }

	// Use this for initialization
	void Start () {
        flock = GetComponent<BGE.Flock>();

        StartCoroutine("ColourCycle");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
