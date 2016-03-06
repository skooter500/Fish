#pragma strict

var netManager : NetworkManager;
var netController : NetworkController;

//TMP STORAGE
private var serverName : String = "Follow Me";

//RECTS
private var mainWindowRect : Rect = Rect(Screen.width/2 - 250,Screen.height/2 - 200,500,400);

//EXTRA
private var scrollPosition : Vector2 = new Vector2();

function Awake() {
	Application.runInBackground = true; //Run In The BackGround

	if(!netManager)Debug.LogWarning("Attach A NetworkManager");
	if(!netController)Debug.LogWarning("Attach A NetworkController");
}

function OnGUI(){
	if(!netManager.IsConnected()){
		GUI.Window(0,mainWindowRect,FollowMeNetConnect,"FollowMe Net Example");
	}else{
		if(GUILayout.Button("Quit")){ //Quit the game
			if(Network.isServer){
				MasterServer.UnregisterHost();
			}
			Network.Disconnect();
		}
	}
}

private function FollowMeNetConnect(id:int){
	GUILayout.BeginVertical();
	GUILayout.Space(10);
	SetServer();
	GetServer();
	GUILayout.EndVertical();		
}


private function GetServer(){ //Display the list of games
	GUILayout.BeginHorizontal();
	// Refresh hosts
	if(GUILayout.Button ("Re-List")){netManager.FetchHostList();}
	
	GUILayout.EndHorizontal();
	scrollPosition = GUILayout.BeginScrollView (scrollPosition);

	var aHost : int = 0;
	var splitComment : String[];
	if(netManager.hostData && netManager.hostData.length > 0){ //Print information from each game
		for(var server : HostData in netManager.hostData){
			GUILayout.BeginHorizontal();				
			aHost = 1;
			var name = server.gameName + " ";
			GUILayout.Box(" ",GUILayout.Width (15),GUILayout.Height(20));
			GUILayout.Label(""+name,GUILayout.Width (125));
			GUILayout.Space(1);
			GUILayout.Label("" + server.connectedPlayers + " : " + server.playerLimit,GUILayout.Width (60));
			GUILayout.Space(1);
			splitComment = server.comment.Split('~'[0]);
			GUILayout.Label(" "+ splitComment[0],GUILayout.Width (80)); //Map
			GUILayout.Space(1);
			GUILayout.Label(" "+ splitComment[1],GUILayout.Width (80));	//GameMode
			
			GUILayout.FlexibleSpace(); 
									
			if(!netManager.nowConnecting){
				if (GUILayout.Button("Connect",GUILayout.Width(80))){netManager.Connect(server);}
			}
			GUILayout.EndHorizontal();	
		}
	}		
	GUILayout.EndScrollView ();
	
	if(aHost == 0){GUILayout.Label("No Games Found..");} //Zero Games
}

private function SetServer(){
	GUILayout.BeginHorizontal();
	GUILayout.Label("Game Name : ",GUILayout.Width(100));
	GUILayout.FlexibleSpace();
	serverName = GUILayout.TextField(serverName,GUILayout.Width(180));
	if(GUILayout.Button("Host",GUILayout.Width(80))){
		netManager.StartHost(serverName,3,"Map","Mode"); //We only allow 3 connections/ max 2*2 games
	}
	GUILayout.EndHorizontal();
}