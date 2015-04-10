using UnityEngine;
using System.Collections;

public class SliceWorld : MonoBehaviour {

    public float radius;
    public float petals;
    public int rings;

    public SliceWorld()
    {
        radius = 1000;        
        petals = 10;
        rings = 10;
    }

	// Use this for initialization
	void Start () {

        for (int ring = 1; ring <= rings; ring++)
        {
            int petalsInRing = (int) petals * ring;
            float thetaInc = (Mathf.PI * 2.0f) / petalsInRing;
            float theta = 0;  Random.Range(0, 2.0f * Mathf.PI);
            for (int i = 0; i < petalsInRing; i++)
            {
                GameObject gameObject = new GameObject();
                SliceForm sliceForm = gameObject.AddComponent<SliceForm>();
                gameObject.AddComponent<Hover>();
                Vector3 position = new Vector3();
                position.x = transform.position.x + Mathf.Sin(theta) * radius * ring;
                position.z = transform.position.z + Mathf.Cos(theta) * radius * ring;
                position.y = position.y + Random.Range(-1000, 500);
                gameObject.transform.position = position;
                gameObject.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * theta, Vector3.up);
                sliceForm.size = new Vector3(1000, 2000, 1000);
                sliceForm.sliceCount = new Vector2(20, 20);
                sliceForm.noiseDelta = new Vector2(0.2f, 0.2f);
                sliceForm.noiseStart = new Vector2(Random.Range(0.0f, 1000.0f), Random.Range(0.0f, 1000.0f));
                sliceForm.noiseToBase = 0.8f;
                sliceForm.closed = false;
                sliceForm.horizontalColour = sliceForm.verticalColour = Pallette.Random();
                theta += thetaInc;
                gameObject.transform.parent = transform;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
