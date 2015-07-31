using UnityEngine;
using System.Collections;

public class RandomAudioPlayer : MonoBehaviour {

    public AudioSource audioSource;
    public float maxInterval;
    public bool cycleColoursOnPlay;

    bool playing = false;

    public RandomAudioPlayer()
    {
        maxInterval = 20.0f;
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
