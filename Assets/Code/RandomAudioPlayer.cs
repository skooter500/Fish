using UnityEngine;
using System.Collections;

public class RandomAudioPlayer : MonoBehaviour {

    public AudioSource audioSource;
    public float maxInterval;
    public bool cycleColoursOnPlay;
    public bool lerpColours;
    bool playing = false;

    public RandomAudioPlayer()
    {
        maxInterval = 20.0f;
        lerpColours = false;
    }

    System.Collections.IEnumerator PlayDelayedAudio()
    {
        playing = true;
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(1.0f, maxInterval));
            audioSource.Play();
            if (cycleColoursOnPlay)
            {
                if (lerpColours)
                {
                    ColorLerper lerper = GetComponent<ColorLerper>();
                    lerper.gameObjects.Clear();
                    lerper.gameObjects.Add(gameObject);
                    lerper.from.Clear();
                    lerper.to.Clear();
                    Renderer renderer = GetComponentInChildren<Renderer>();
                    lerper.from.Add(renderer.material.color);
                    lerper.to.Add(Pallette.RandomNot(renderer.material.color));
                    lerper.StartLerping();
                }                                
            }
            else
            {
                GetComponent<SectionColours>().CycleColours();
            }
        }
    }

	// Use this for initialization
	void Start () 
    {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (audioSource != null && ! playing)
        {
            StartCoroutine("PlayDelayedAudio");
        }
	}
}
