using UnityEngine;
using System.Collections;

/// <summary>
/// Forces Unity to change the sample rate on Android/iOS
/// This script must be placed on an object in a dummy scene/level
/// It changes the sample rate of the application and then loads a new scene
/// </summary>
public class TBE_SRChange : MonoBehaviour {
	
	public int SampleRate = 44100;
	public int LevelNumber = 1;
	
	void Start () 
	{
		
		// The below code will have no effect on desktops/editor
		#if UNITY_ANDROID || UNITY_IOS
		AudioSettings.outputSampleRate = SampleRate;
		#endif
		Debug.Log ( "Audio Sample Rate Set To: " + AudioSettings.outputSampleRate );
		Application.LoadLevel (LevelNumber);
		
	}
	
}