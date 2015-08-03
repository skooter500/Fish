using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliceWorld : MonoBehaviour {

    public List<GameObject> formPrefabs;
    public Vector2 noiseDelta;
    public Vector2 sliceCount;
    public int xCount;
    public int zCount;
    public float gap;

    public int nextLifeForm;

    public bool createLifeForms;

    public SliceWorld()
    {
        xCount = 5;
        zCount = 5;
        gap = 2000;
        nextLifeForm = 0;
        createLifeForms = false;
        noiseDelta = new Vector2(0.2f, 0.2f);
        sliceCount = new Vector2(5, 5);
    }

    private SliceForm CreateSliceForm(Vector3 pos, Vector2 noise)
    {
        GameObject gameObject = new GameObject();
        SliceForm sliceForm = gameObject.AddComponent<SliceForm>();
        gameObject.AddComponent<ColorLerper>();
        gameObject.AddComponent<SectionColours>().segments.Add(gameObject);
        gameObject.GetComponent<SectionColours>().lerpColors = true;
        RandomAudioPlayer rap = gameObject.AddComponent<RandomAudioPlayer>();
        rap.cycleColoursOnPlay = true;
        rap.maxInterval = 3.0f;
        gameObject.AddComponent<Hover>();
        //pos.y += Random.Range(-200, 200);
        gameObject.transform.position = pos;

        sliceForm.size = new Vector3(1000, 10000, 1000);
        sliceForm.sliceCount = sliceCount;
        sliceForm.noiseDelta = noiseDelta;
        sliceForm.noiseStart = noise;
        sliceForm.noiseToBase = 0.2f;
        sliceForm.closed = false;
        sliceForm.horizontalColour = sliceForm.verticalColour = Pallette.Random();
        gameObject.transform.parent = transform;
        sliceForm.Generate();
        return sliceForm;
    }

	// Use this for initialization
	void Start () 
    {
        float width = xCount * gap;
        float depth = zCount * gap;
        float left = transform.position.x - (width / 2);
        float front = transform.position.z - (depth / 2);

        int xMid = xCount / 2;
        int zMid = zCount / 2;
        Vector2 noiseStart = Random.insideUnitCircle * 1000;
        float lastY = 0;
        bool first = true;
        for (int x = 0; x < xCount; x ++)
        {
            Vector3 pos = new Vector3();
            pos.x = left + (x * gap);
            Vector2 thisNoiseStart = noiseStart;
            thisNoiseStart.x += (noiseDelta.x * x * sliceCount.x);
            for (int z = 0 ; z < zCount; z ++)
            {                
                pos.z = front + (z * gap);
                pos.y = transform.position.y;
                thisNoiseStart.y += (noiseDelta.y * z * sliceCount.y);
                if (!first && (Random.Range(0.0f, 1.0f) > 0.5f))
                {
                    pos.y = lastY - 1000; // 500 pixels below the height of the last sliceform
                    CreateLifeForm(pos);                    
                }
                else
                {
                    SliceForm sf = CreateSliceForm(pos, noiseStart);
                    pos.y += 100;
                    lastY = sf.maxY;
                    first = false;
                }
                noiseStart.x += noiseDelta.x * sliceCount.x;


                //if ((x == xMid || x == xMid - 1) && (z == zMid || z == zMid - 1))
                //{
                //    // Skip
                //}
                //else
                //{
                //    CreateSliceForm(pos);
                //}
            }
        }
      }

    private void CreateLifeForm(Vector3 pos)
    {
        GameObject form = Instantiate(formPrefabs[nextLifeForm]);
        form.SetActive(true);
        form.transform.position = pos;
        nextLifeForm = (nextLifeForm + 1) % formPrefabs.Count;

    }
	
	// Update is called once per frame
	void Update () {
        //float width = xCount * gap;
        //float depth = zCount * gap;
        //float left = transform.position.x - (width / 2);
        //float front = transform.position.z - (depth / 2);

        //int xMid = xCount / 2;
        //int zMid = zCount / 2;
        //float height = -500;
        //for (int x = 0; x < xCount; x++)
        //{
        //    Vector3 pos = new Vector3();
        //    pos.x = left + (x * gap);
        //    Vector3[] points = new Vector3[2];
        //    points[0] = new Vector3(pos.x, height, front);
        //    points[1] = new Vector3(pos.x, height, front + depth);
        //    BGE.LineDrawer.DrawLine(points[0], points[1], Color.cyan);
        //}

        //for (int z = 0; z < zCount; z++)
        //{
        //    Vector3 pos = new Vector3();
        //    pos.z = front + (z * gap);
        //    Vector3[] points = new Vector3[2];
        //    points[0] = new Vector3(left, height, pos.z);
        //    points[1] = new Vector3(left + width, height, pos.z);
        //    BGE.LineDrawer.DrawLine(points[0], points[1], Color.cyan);
        //}

	}
}
