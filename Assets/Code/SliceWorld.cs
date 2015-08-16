using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliceWorld : MonoBehaviour {

    [HideInInspector]
    public List<GameObject> forms;
    

    public List<GameObject> formPrefabs;
    public Vector2 noiseDelta;
    public Vector2 sliceCount;
    public int xCount;
    public int zCount;
    public float gap;

    public int nextLifeForm;

    public bool createLifeForms;

    public float probabilityOfEmpty;
    public float probabilityOfSlice;
    public float probabilityOfCreature;


    public SliceWorld()
    {
        xCount = 5;
        zCount = 5;
        gap = 2000;

        probabilityOfEmpty = 0.5f;
        probabilityOfSlice = 0.25f;
        probabilityOfCreature = 0.25f;

        nextLifeForm = 0;
        createLifeForms = false;
        noiseDelta = new Vector2(0.2f, 0.2f);
        sliceCount = new Vector2(5, 5);
        forms = new List<GameObject>();

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

        sliceForm.size = new Vector3(2000, 10000, 2000);
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
        float lastY = transform.position.y;
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
                float prob = Random.Range(0.0f, 1.0f);
                if (prob > probabilityOfEmpty && prob < probabilityOfEmpty + probabilityOfCreature)
                {
                    GameObject lifeForm = CreateLifeForm(pos);
                    lifeForm.transform.parent = transform;
                }
                else if (prob > probabilityOfEmpty + probabilityOfCreature)
                {
                    SliceForm sf = CreateSliceForm(pos, noiseStart);
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

    private GameObject CreateLifeForm(Vector3 pos)
    {
        GameObject form = Instantiate(formPrefabs[nextLifeForm]);
        form.SetActive(true);
        form.transform.position = pos;
        nextLifeForm = (nextLifeForm + 1) % formPrefabs.Count;
        forms.Add(form);
        return form;
    }
	
    void Update()
    {
        // Deactivate forms that are too far from the player to be perceived
        int formsActive = 0;
        float activateDistance = gap * 2f;
        for (int i = 0; i < forms.Count; i++)
        {
            float distToPlayer = Vector3.Distance(Player.Instance.transform.position, forms[i].transform.position);
            if (distToPlayer < activateDistance)
            {
                if (!forms[i].activeSelf)
                {
                    forms[i].SetActive(true);                    
                }
                BGE.BoidManager.PrintVector("FormPos: ", forms[i].transform.position);
                BGE.BoidManager.PrintFloat("Distance: ", Vector3.Distance(Player.Instance.transform.position, forms[i].transform.position));
                formsActive++;

            }
            else
            {
                if (forms[i].activeSelf)
                {
                    forms[i].SetActive(false);
                }
            }
        }
        BGE.BoidManager.PrintFloat("Forms active: ", formsActive);
    }
}
