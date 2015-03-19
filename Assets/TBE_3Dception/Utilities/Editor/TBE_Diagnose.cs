/* Diagnose and setup 3Dception
 * Setup: creates 3Dception Global object with Initialisation and Destroy components, 
 * 		creates Global Listener on main camera, sets script execution order
 * Diagnose: Check if plugin paths are correct and if plugins exist. Also checks 
 * 			if the native plugin and the unity plugin are compatible.
 * 
 * Copyright (c) Two Big Ears Ltd., 2014
 * support@twobigears.com
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using TBE_3DCore;

public class TBE_Diagnose : EditorWindow 
{

	string windows32Status = string.Empty;
	string windows64Status = string.Empty;
	string linux32Status = string.Empty;
	string linux64Status = string.Empty;
	string osxStatus = string.Empty;
	string iosStatus = string.Empty;
	string androidStatus = string.Empty;
	string versionStatus = string.Empty;

	bool toggleInitDestroy = true;
	bool toggleCreateListener = true;
	bool toggleScriptExecOrder = true;
	bool toggleEnvironment = true;

	static Vector2 windowSize = new Vector2 (300, 600);

	public static TBE_Diagnose instance { get; private set; }
	
	[MenuItem ("3Dception/Setup And Diagnostics")]
	static void Init () 
	{
		// Get existing open window or if none, make a new one:		
		instance = (TBE_Diagnose)EditorWindow.GetWindow (typeof(TBE_Diagnose));
		instance.ShowUtility ();
		instance.title = "Setup/Diagnose";
		instance.minSize = windowSize;
		instance.maxSize = windowSize;
	}

	void OnGUI ()
	{	
		EditorGUILayout.Space ();

		GUILayout.Label ("3Dception — Project Setup", EditorStyles.boldLabel);

		EditorGUILayout.LabelField ("Chose the options below and click on 'Setup Scene' to automatically setup your scene.\n\n" +
			"The 3Dception Global object includes an 'Initialisation' and a 'Destroy' component to explicitly control when resources for 3Dception are initialised and destroyed in your project.\n\n" +
			"'Create 3Dception Listener' adds the 3Dception Listener component to the main camera in the scene.\n\n" +
		   	"'Create 3Dception Environment Object' adds the 3Dception Environment object to the scene, for controlling global environment properties.\n\n" +
		    "'Set Execution Order' ensures the various components for 3Dception are executed in the correct order.\n", EditorStyles.wordWrappedLabel);

		toggleInitDestroy = GUILayout.Toggle (toggleInitDestroy, "Create 3Dception Global Object");

		toggleEnvironment = GUILayout.Toggle (toggleEnvironment, "Create 3Dception Environment Object");

		toggleCreateListener = GUILayout.Toggle (toggleCreateListener, "Create 3Dception Listener");

		toggleScriptExecOrder = GUILayout.Toggle (toggleScriptExecOrder, "Set Execution Order");

		if(GUILayout.Button("Setup Scene"))
		{
			if (toggleInitDestroy)
			{
				createGlobalObject();
			}

			if (toggleEnvironment)
			{
				createEnvironmentlObject();
			}

			if (toggleCreateListener)
			{
				createListener();
			}

			if (toggleScriptExecOrder)
			{
				setScriptExecOrderForAll();
			}
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		GUILayout.Label ("3Dception — Diagnostics", EditorStyles.boldLabel);

		EditorGUILayout.LabelField ("Click on 'Run Diagnostics' if you have upgraded Unity or 3Dception to a new version to check if everything is okay.", EditorStyles.wordWrappedLabel);

		if(GUILayout.Button("Run Diagnostics"))
		{
			checkPluginStructure();
			checkVersionMatch();
		}

		EditorGUILayout.HelpBox (windows32Status + "\n" +
		                         windows64Status + "\n" +
		                         osxStatus + "\n" +
								 iosStatus + "\n" +
								 androidStatus + "\n" +
								 linux32Status + "\n" +
		                         linux64Status + "\n" +
		                         versionStatus
		                         , MessageType.None);
	}

	void checkPluginStructure()
	{

		string unityVersion = Application.unityVersion;
		bool isVersion5 = unityVersion.StartsWith("5.");

		if (isVersion5) 
		{	
			tryDelete("Assets/Plugins/tbe_3Dception.dll");
			tryDelete("Assets/Plugins/libtbe_3Dception.so");
			tryDelete("Assets/Plugins/tbe_3Dception.bundle");
			tryDelete("Assets/Plugins/x86/tbe_3Dception.dll");
			tryDelete("Assets/Plugins/x86/libtbe_3Dception.so");
			tryDelete("Assets/Plugins/x86_64/tbe_3Dception.dll");
			tryDelete("Assets/Plugins/x86_64/libtbe_3Dception.so");
			tryDelete("Assets/Plugins/iOS/libtbe_3Dception_ios.a");
			tryDelete("Assets/Plugins/Android/libtbe_3Dception.so");

			checkExists("Assets/TBE_3Dception/Plugins/x86/tbe_3Dception.dll", "Windows 32", out windows32Status);
			checkExists("Assets/TBE_3Dception/Plugins/x86_64/tbe_3Dception.dll", "Windows 64", out windows64Status);
			checkExists("Assets/TBE_3Dception/Plugins/x86/libtbe_3Dception.so", "Linux 32", out linux32Status);
			checkExists("Assets/TBE_3Dception/Plugins/x86_64/libtbe_3Dception.so", "Linux 64", out linux64Status);
			checkExists("Assets/TBE_3Dception/Plugins/tbe_3Dception.bundle/Contents/MacOS/tbe_3Dception", "OSX", out osxStatus);
			checkExists("Assets/TBE_3Dception/Plugins/iOS/libtbe_3Dception_ios.a", "iOS", out iosStatus);
			checkExists("Assets/TBE_3Dception/Plugins/Android/libtbe_3Dception.so", "Android", out androidStatus);
		} 
		else 
		{
			tryDelete("Assets/Plugins/tbe_3Dception.dll");
			tryDelete("Assets/Plugins/libtbe_3Dception.so");

			checkExists("Assets/Plugins/x86/tbe_3Dception.dll", "Windows 32", out windows32Status);
			checkExists("Assets/Plugins/x86_64/tbe_3Dception.dll", "Windows 64", out windows64Status);
			checkExists("Assets/Plugins/x86/libtbe_3Dception.so", "Linux 32", out linux32Status);
			checkExists("Assets/Plugins/x86_64/libtbe_3Dception.so", "Linux 64", out linux64Status);
			checkExists("Assets/Plugins/tbe_3Dception.bundle/Contents/MacOS/tbe_3Dception", "OSX", out osxStatus);
			checkExists("Assets/Plugins/iOS/libtbe_3Dception_ios.a", "iOS", out iosStatus);
			checkExists("Assets/Plugins/Android/libtbe_3Dception.so", "Android", out androidStatus);
		}
	}

	void tryDelete(string path)
	{
		bool deleted = FileUtil.DeleteFileOrDirectory(path);
		
		if (deleted) {
			Debug.Log ("3Dception Diagnostics: Deleted " + path);
		} 
		else 
		{
			//TODO: check if plugin has been deleted successfully or not
		}
	}

	void checkExists(string path, string platform, out string uiMessage)
	{
		if (File.Exists (path)) uiMessage = platform + ": Plugin Available";
		else uiMessage = platform + ": Plugin Unavailable";
	}

	void checkVersionMatch()
	{	
		string coreVersionString = TBEngine.getVersionMajor () + "." + TBEngine.getVersionMinor () + "." + TBEngine.getVersionPatch ();
		if (coreVersionString.Equals (TBE_CONSTANTS.EXPECTED_CORE_VERSION)) 
		{
			versionStatus = "3Dception version is correct, all good!";
		}
		else
		{
			versionStatus = "3Dception version is incorrect, restart Unity and re-import the package";
		}
	}

	void createGlobalObject()
	{
		GameObject tbeGlobal = GameObject.Find ("3Dception Global");

		if (tbeGlobal == null) 
		{	
			// Create 3Dception Global object with components
			tbeGlobal = new GameObject("3Dception Global");
			tbeGlobal.AddComponent<TBE_Initialise>();
			tbeGlobal.AddComponent<TBE_Destroy>();
			tbeGlobal.AddComponent<TBE_InitialiseRoomModelling>();
		}
	}

	void createEnvironmentlObject()
	{
		GameObject tbeEnv = GameObject.Find ("3Dception Environment");
		
		if (tbeEnv == null) 
		{	
			// Create 3Dception Environment object with components
			tbeEnv = new GameObject("3Dception Environment");
			tbeEnv.AddComponent<TBE_Environment>();
		}
	}

	void setScriptExecOrderForAll()
	{
		setScriptExecutionOrder(typeof(TBE_Initialise).Name, -400);
		setScriptExecutionOrder(typeof(TBE_InitialiseRoomModelling).Name, -300);
		setScriptExecutionOrder(typeof(TBE_Environment).Name, -250);
		setScriptExecutionOrder(typeof(TBE_GlobalListener).Name, -200);
		setScriptExecutionOrder(typeof(TBE_RoomProperties).Name, -100);
		setScriptExecutionOrder(typeof(TBE_Destroy).Name, 100);
	}

	void createListener()
	{
		Camera mainCam = Camera.main;
		TBE_GlobalListener listener = mainCam.GetComponent<TBE_GlobalListener> ();
		if (listener == null) 
		{
			mainCam.gameObject.AddComponent<TBE_GlobalListener>();
		}
	}

	void setScriptExecutionOrder(string className, int order)
	{
		foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
		{
			if (monoScript.name == className)
			{	
				MonoImporter.SetExecutionOrder(monoScript, order);
				break;
			}
		}
	}
}
