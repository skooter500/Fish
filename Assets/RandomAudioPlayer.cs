using UnityEngine;
using System.Collections;

public class RandomAudioPlayer : MonoBehaviour {

    AudioSource audioSource;
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
                ColorLerper lerper = GetComponent<ColorLerper>();
                lerper.gameObjects.Clear();
                lerper.gameObjects.Add(gameObject);
                lerper.to.Clear();
                lerper.to.Add(Pallette.Random());
                lerper.StartLerping();
                
            }
        }
    }

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () {
        if (audioSource != null && ! playing)
        {
            StartCoroutine("PlayDelayedAudio");
        }
	}
}
