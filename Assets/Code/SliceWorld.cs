using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliceWorld : MonoBehaviour {

    public List<GameObject> formPrefabs;

    public float xCount;
    public float zCount;
    public float gap;


    public SliceWorld()
    {
        xCount = 5;
        zCount = 5;
        gap = 2000;
    }

    private void CreateSliceForm(Vector3 pos)
    {
        GameObject gameObject = new GameObject();
        SliceForm sliceForm = gameObject.AddComponent<SliceForm>();
        gameObject.AddComponent<Hover>();
        pos.y += Random.Range(-200, 200);
        gameObject.transform.position = pos;

        sliceForm.size = new Vector3(1000, 3000, 1000);
        sliceForm.sliceCount = new Vector2(20, 20);
        sliceForm.noiseDelta = new Vector2(0.1f, 0.1f);
        sliceForm.noiseStart = new Vector2(Random.Range(0.0f, 1000.0f), Random.Range(0.0f, 1000.0f));
        sliceForm.noiseToBase = 0.4f;
        sliceForm.closed = false;
        sliceForm.horizontalColour = sliceForm.verticalColour = Pallette.Random();
        gameObject.transform.parent = transform;
    }

	// Use this for initialization
	void Start () 
    {
        float width = xCount * gap;
        float depth = zCount * gap;
        float left = transform.position.x - (width / 2);
        float front = transform.position.z - (depth / 2);
        
        for (float x = 0; x < xCount; x ++)
        {
            for (float z = 0 ; z < zCount; z ++)
            {
                Vector3 pos = new Vector3();
                pos.x = left + (x * gap);
                pos.z = front + (z * gap);
                pos.y = transform.position.y;
                if (Random.Range(0.0f, 1.0f) > 0.5f)
                {
                    CreateSliceForm(pos);
                }
                else
                {
                    //CreateSliceForm(pos);
                    CreateLifeForm(pos);
                }
            }
        }

      }

    private void CreateLifeForm(Vector3 pos)
    {
        int i = Random.Range(0, formPrefabs.Count);

        GameObject form = Instantiate(formPrefabs[i]);
        form.SetActive(true);
        form.transform.position = pos;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
