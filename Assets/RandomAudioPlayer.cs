using UnityEngine;
using System.Collections;

public class RandomAudioPlayer : MonoBehaviour {

    AudioSource audioSource;
    public float maxInterval;
    public bool cycleColoursOnPlay;

    public RandomAudioPlayer()
    {
        maxInterval = 20.0f;
    }

    System.Collections.IEnumerator PlayDelayedAudio()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(1.0f, maxInterval));
            audioSource.Play();
            if (cycleColoursOnPlay)
            {
                SectionColours sectionColours = GetComponent<SectionColours>();
                if (sectionColours == null)
                {
                    Renderer renderer = GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Pallette.Random();
                    }
                }
                else
                {
                    sectionColours.CycleColours();
                }
            }
        }
    }

	// Use this for initialization
	void Start () 
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            StartCoroutine("PlayDelayedAudio");
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
