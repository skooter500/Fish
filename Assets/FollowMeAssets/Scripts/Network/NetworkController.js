#pragma strict

import System.Collections.Generic;
//Server Statistics Class
class ServerStatistics{
	var msgsSent : int; //TODO Next Update...(1.8)
	var msgsReceived : int;
}
public static var serverStatistics : ServerStatistics; // Keeps track of stats on server
//Chat Class
private class ChatHolder{
	var msgs : List.<String>;
	var msg : String = "";
	
	function ChatHolder(){
		msgs = new List.<String>();
	}
	
	function AddMsg(mss : String){ //Add a message to the chat list
		if(msgs.Count >= 25){ //we never wnat more than 25 msgs 
			msgs.RemoveAt(0);
		}
		msgs.Add(mss);
	}
}
private var chatHolder : ChatHolder;
//User Class & Comparers
private class CompareByName implements IComparer{
	function Compare(userA : Object , userB : Object) : int{ //Ok to DownCast we know it will be a user Class
		var usera : UserClass  = (userA as UserClass);
		var userb : UserClass = (userB as UserClass);
		return usera.name.CompareTo(userb.name);
	}
}
private class CompareByScore implements IComparer{
	function Compare(userA : Object , userB : Object) : int{ //Ok to DownCast we know it will be a user Class
		var usera : UserClass  = (userA as UserClass);
		var userb : UserClass = (userB as UserClass);
		return usera.score.CompareTo(userb.score);
	}
}
private class UserClass{ //Our user class holds information about all players needeed by many oprerations
	var name : String = "Player";
	var team : int = 0; //0 = Team Blue / 1 = Team Red
	var score : int = 0;
	var netPlr : NetworkPlayer;
	//The id that the user self allocated - 
	var netID : NetworkViewID; //this id gives us an id for the user + a networkID to use to give the user control of game objects
	var isServer : boolean = false; //is this user the server
	var isReady : int; //is player ready to play
}
@HideInInspector var playerList : List.<UserClass>;
private var playersInTeamBlue : int;
private var playersInTeamRed : int;
//Storage Fields
private var userComplaintTable : Hashtable; //The hashtable will be and netID & boolean key val pair 
//Local------keep local identifies for fas manipulation
private var localName : String = "";
private var localNetID : NetworkViewID;
private var localReadyStatus : int = 0;

private var gameStarting : float; //Just a small timer to use to run operations before games start 
private var menuUp : boolean = true; //is out menu up
private var showServerOptions : boolean = false;
private var showServerManager : boolean = false;

var gameManager : NetworkGameManager; //Our game Manager
var utilities : NetworkUtilities; //A utilities Refrence
var minUsersForGameStart : int = 0; //Number of players needed to start a game

//Rects
private var scrollPosition : Vector2 = new Vector2();
private var mainWindowRect : Rect = Rect(Screen.width/2 - 250,Screen.height/2 - 200,500,400);
private var chatWindowRect : Rect = Rect(0,Screen.height - 150,300,150);

function Initialize (){
	//Remove all Msgs in Group 1
	Network.RemoveRPCsInGroup(1);
	
	if(Network.isServer){
		serverStatistics = new ServerStatistics(); //Make Sure We can Collect server data
		gameManager.Initialize(); //Server Initialize the Game Manager
		//Lets Figure out some things..
		//are we in a team game
		//gameManager.gameFields.isTeamGame = 
	}
	
	chatHolder = new ChatHolder();
	playerList = new List.<UserClass>();
	localName = "Guest"+Random.Range(0,100);
	//Lets Create a table where we can store who we have voted agianst//to prevent vote spanning
	//The server or host will create an id/List.<> structure
	//The users will create an id/null structure
	userComplaintTable = new Hashtable(); //This Is Great because we can define what we store based on use
	//Generate id
	localNetID = Network.AllocateViewID();
	GetComponent.<NetworkView>().RPC("RegisterUser",RPCMode.AllBuffered,localName,Network.player,localNetID,Network.isServer);	
}

function DeInitialize(){
	
}

function Hide(){
	menuUp = false;
}

function StartGame(){ //Lets Start The Game
	if(AllUsersReady()){ //If all users are ready
		if(!gameManager.gameFields.gameStarted){
			GetComponent.<NetworkView>().RPC("StartingGame",RPCMode.Others);
			serverStatistics.msgsSent++;
			gameManager.StartGame(playerList,1);
			gameStarting = Time.time + 3;
		}
	}else{
		utilities.AddMessage("Players Not Ready",2,false); //Send A Messege to Host
	}
}
@RPC
function StartingGame(){
	gameStarting = Time.time + 3;	
}
function EndGame(){ //End the Game
	if(gameManager.gameFields.gameStarted){
		gameManager.EndGame();
	}
}
function ResetPlayerList (){ //Reset Certain user statistics
	for(var user : UserClass in playerList){
		user.score = 0;
		user.isReady = 0;
	}
}
function Update (){
	if(Input.GetKeyDown(KeyCode.Space)){
		if(Screen.lockCursor == true)Screen.lockCursor = false;
		else Screen.lockCursor = true;
	}
	if(gameManager.gameFields.gameStarted){
		if(Input.GetKeyDown(KeyCode.Tab)){
			if(menuUp == false){
				menuUp = true;
			}else{
		 	 	menuUp = false;
		 	 }
		}
		if(Network.isServer){
			//Game Management --> passing playerlist just in case 
			gameManager.PerformUtilities(playerList); //We allow the gameManager to perform any cyclic duties
		}
	}else{
		menuUp = true;	
	}
	//Figure Out How Many Players in each Team
	GetTeamSizes();
}
function GetTeamSizes(){
	var blueTeam : int = 0; var redTeam : int = 0;
	for(var user : UserClass in playerList){
		if(user.team == 0){
			blueTeam++;
		}else if(user.team == 1){
			redTeam++;
		}
	}
	if(blueTeam != playersInTeamBlue){playersInTeamBlue = blueTeam;}
	if(redTeam != playersInTeamRed){playersInTeamRed = redTeam;}
}
//Check if all users are ready to play
private function AllUsersReady() : boolean{
	for(var user : UserClass in playerList){
		if(!user.isReady && user.netPlr != Network.player){ //We dont want to check servers ready, just users
			return false;
		}
	}
	return true;
}

function OnGUI (){
	if(Network.peerType != NetworkPeerType.Disconnected){
		GUI.Box(new Rect(Screen.width/2 - 50,Screen.height - 30,100,30),"#Players : " + playerList.Count);
		
		//Show Window
		if(menuUp){
			GUI.Window(0,mainWindowRect,FollowMeNetLobby,"FollowMe Net Lobby");
		}else{
			GUI.Window(0,chatWindowRect,FollowMeNetChat,"Chat Window");
		}
	}
}
//Chat GUI Window
private function FollowMeNetChat(id:int){
	//Show Chat
	scrollPosition = GUILayout.BeginScrollView (scrollPosition);
	for(var str : String in chatHolder.msgs){
		GUILayout.Label(str);
	}
	GUILayout.EndScrollView ();	
	//Enter Text
	GUILayout.BeginHorizontal();
	chatHolder.msg = GUILayout.TextField(chatHolder.msg,GUILayout.Width(250));
	GUILayout.FlexibleSpace();
	if(GUILayout.Button(">",GUILayout.Width(25))){ //Send Chat MSG
		if(chatHolder.msg.Length > 0)
	 		GetComponent.<NetworkView>().RPC("InsertChatMSG",RPCMode.All,localName,chatHolder.msg);//Only Current Users Can chat	
		GUIUtility.keyboardControl = 0; //Remove Focus From text Fields
		chatHolder.msg = ""; //Clear Text
	}
	GUILayout.EndHorizontal();
}
//Lobby GUI Window
private function FollowMeNetLobby(id:int){
	GUILayout.BeginHorizontal();
	
	if(showServerOptions == false && showServerManager == false){ //Show only when displaying player list
		if(Network.isClient){
			GUILayout.Box(localName,GUILayout.Width(80));
		}else{
		//Start & End Game as Server
			if(gameManager.gameFields.gameStarted){
				if(GUILayout.Button("End Game",GUILayout.Width(80))){
					EndGame();
				}
			}else{
				if(GUILayout.Button("Start",GUILayout.Width(80))){
					if(playerList.Count > minUsersForGameStart){ //Minimum # of player before game can start
						StartGame();
					}else{
						utilities.AddMessage(minUsersForGameStart+" Players Needed",2,false); //Send A Messege to Host
					}
				}
			}
		}
		GUILayout.Space(15);
		//Team swap request! only if its a team game & game has not started (only in lobby)
		if(gameManager.gameFields.isTeamGame && !gameManager.gameFields.gameStarted){
			if(GUILayout.Button("Swap Team")){
				if(Network.isClient){
					GetComponent.<NetworkView>().RPC("TeamSwapRequest",RPCMode.Server,Network.player);
				}else if(Network.isServer){
					TeamSwapRequest(Network.player);
				}
			}	
		}
	}
	
	GUILayout.FlexibleSpace();
	
	//Show Options
	if(GUILayout.Button("Options",GUILayout.Width(80))){
		showServerManager = false; //We are done looking at stats
		showServerOptions = (showServerOptions == true)? false:true; //Show The Server Options/Details
	}
	//Show Stats
	if(Network.isServer){ //We only really need this as a server 
		if(GUILayout.Button("Manager",GUILayout.Width(80))){
			showServerOptions = false; //No longer intrested in options
			showServerManager = (showServerManager == true)? false:true; //Show The Server Options/Details
		}
	}else{
		if(GUILayout.Button("Ready",GUILayout.Width(80))){
			localReadyStatus = (localReadyStatus == 0)? 1:0;
			GetComponent.<NetworkView>().RPC("ReadyStatus",RPCMode.AllBuffered,localNetID,localReadyStatus);
		}
	}
	
	GUILayout.EndHorizontal();
	
	if(showServerManager){
		ShowManager();
	}else if(showServerOptions){
		ShowOptions();
	}else{
		if(gameManager.gameFields.isTeamGame){ //we are in a team game so split tables
			SlipUserList();
		}else{
			if(playerList != null){
				scrollPosition = GUILayout.BeginScrollView (scrollPosition);
				for(var user : UserClass in playerList){
					GUILayout.BeginHorizontal();
					ProcessUser(user);
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView ();
			}
		}
	}	
}

//Gui Functions
//Split The user List
private function ProcessUser(user : UserClass){
	if(user.isReady == 1){
		GUILayout.Box("*",GUILayout.Width(15),GUILayout.Height(20)); //User Ready
	}else{
		GUILayout.Box(" ",GUILayout.Width(15),GUILayout.Height(20)); //Not ready	
	}
	GUILayout.Label("User : " + user.name,GUILayout.Width(100));
	GUILayout.Space(50);
	GUILayout.Label("Score : " + user.score);
	GUILayout.Space(50);
	GUILayout.Label("");
	if(Network.isServer){ //We are server, cant close our own connection
		if(Network.player == user.netPlr){
			GUILayout.Box("",GUILayout.Width(25)); //This is Me On The List
		}else{
			GUILayout.Box("**",GUILayout.Width(25)); //Other Users
		}
	}else{
		if(Network.player == user.netPlr){
			GUILayout.Box("",GUILayout.Width(25)); //This is Me On The List
		}else if(user.isServer){
			GUILayout.Box("H",GUILayout.Width(25)); //Host
		}else{
			if(GUILayout.Button("X",GUILayout.Width(25))){ //send to server a command of kick
				if(userComplaintTable[user.netID] == null){
					GetComponent.<NetworkView>().RPC("UserComplaint",RPCMode.Server,user.netID,localNetID);
					userComplaintTable.Add(user.netID,1); //Add to table
				}		
			}
		}
	}
}

private function SlipUserList(){
	GUILayout.Box("----Team Blue ----");
	scrollPosition = GUILayout.BeginScrollView (scrollPosition);
	for(var user : UserClass in playerList){
		GUILayout.BeginHorizontal();
		if(user.team == 0)ProcessUser(user);
		GUILayout.EndHorizontal();
	}
	GUILayout.EndScrollView ();	
	GUILayout.Box("----Team Red ----");
	scrollPosition = GUILayout.BeginScrollView (scrollPosition);
	for(var user : UserClass in playerList){
		GUILayout.BeginHorizontal();
		if(user.team == 1)ProcessUser(user);
		GUILayout.EndHorizontal();
	}
	GUILayout.EndScrollView ();	
}
//Show the Server Manager
private function ShowManager(){
	if(playerList != null){
		GUILayout.Box("Statistics ");
		//GUILayout.Label("Messages Sent : " + serverStatistics.msgsSent);
		//GUILayout.Label("Messages Received : " + serverStatistics.msgsReceived);
		GUILayout.Box("Total Users " + Network.connections.Length); //We are an user as a server so 
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);
		var count : int = 0;
		for(var user : UserClass in playerList){
			if(!user.isServer && user.netPlr != Network.player){
				GUILayout.BeginHorizontal();
				GUILayout.Label("User : " + user.name,GUILayout.Width(100));
				GUILayout.Space(5);
				GUILayout.Label(""+user.netID);
				GUILayout.Space(5);
				GUILayout.Label("IP : " + user.netPlr.externalIP);
				GUILayout.Space(5);
				
				if(userComplaintTable[user.netID])count = (userComplaintTable[user.netID] as List.<NetworkViewID>).Count;
				else count = 0;
				
				GUILayout.Label("Votes : " + count);
				GUILayout.Space(5);
				if(GUILayout.Button("X",GUILayout.Width(25))){ //KICK A USER HERE
					Network.CloseConnection(user.netPlr,true);
				}
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndScrollView ();
	}
		
}
//Show Options
private function ShowOptions(){
	//Team Game---------
	GUILayout.BeginHorizontal();
	GUILayout.Box("[Is Team Game]",GUILayout.Width(200));
	GUILayout.FlexibleSpace();
	if(Network.isClient){
		GUILayout.Toggle(gameManager.gameFields.isTeamGame, "",GUILayout.Width(50));
	}else if(Network.isServer){
		if(gameManager.gameFields.gameStarted){
			GUILayout.Toggle(gameManager.gameFields.isTeamGame, "",GUILayout.Width(50));
		}else{
			gameManager.gameFields.isTeamGame = GUILayout.Toggle(gameManager.gameFields.isTeamGame, "",GUILayout.Width(50));
			//GUILayout.Toggle(gameManager.gameFields.isTeamGame, "",GUILayout.Width(50));
		}
	}
	GUILayout.EndHorizontal();
	//Friendly Fire-----------
	GUILayout.BeginHorizontal();
	GUILayout.Box("[Friendly Fire]",GUILayout.Width(200));
	GUILayout.FlexibleSpace();
	if(Network.isClient){
		GUILayout.Toggle(gameManager.gameFields.breakOwnTeamBonds, "",GUILayout.Width(50));
	}else if(Network.isServer){
		if(gameManager.gameFields.gameStarted){
			GUILayout.Toggle(gameManager.gameFields.breakOwnTeamBonds, "",GUILayout.Width(50));
		}else{
			gameManager.gameFields.breakOwnTeamBonds = GUILayout.Toggle(gameManager.gameFields.breakOwnTeamBonds, "",GUILayout.Width(50));
		}
	}
	GUILayout.EndHorizontal();
	//Break Own Bonds-----------
	GUILayout.BeginHorizontal();
	GUILayout.Box("[Break Own Bonds]",GUILayout.Width(200));
	GUILayout.FlexibleSpace();
	if(Network.isClient){
		GUILayout.Toggle(gameManager.gameFields.breakOwnBonds, "",GUILayout.Width(50));
	}else if(Network.isServer){
		if(gameManager.gameFields.gameStarted){
			GUILayout.Toggle(gameManager.gameFields.breakOwnBonds, "",GUILayout.Width(50));
		}else{
			gameManager.gameFields.breakOwnBonds = GUILayout.Toggle(gameManager.gameFields.breakOwnBonds, "",GUILayout.Width(50));
		}
	}
	GUILayout.EndHorizontal();
	//Break Enemy Bonds-----------
	GUILayout.BeginHorizontal();
	GUILayout.Box("[Break Enemy Bonds]",GUILayout.Width(200));
	GUILayout.FlexibleSpace();
	if(Network.isClient){
		GUILayout.Toggle(gameManager.gameFields.breakEnemyBonds, "",GUILayout.Width(50));
	}else if(Network.isServer){
		if(gameManager.gameFields.gameStarted){
			GUILayout.Toggle(gameManager.gameFields.breakEnemyBonds, "",GUILayout.Width(50));
		}else{
			gameManager.gameFields.breakEnemyBonds = GUILayout.Toggle(gameManager.gameFields.breakEnemyBonds, "",GUILayout.Width(50));
		}
	}
	GUILayout.EndHorizontal();
	//Follower Break With Parent-----------
	GUILayout.BeginHorizontal();
	GUILayout.Box("[Follower break with Parent]",GUILayout.Width(200));
	GUILayout.FlexibleSpace();
	if(Network.isClient){
		GUILayout.Toggle(gameManager.gameFields.followersBreakWithParent, "",GUILayout.Width(50));
	}else if(Network.isServer){
		if(gameManager.gameFields.gameStarted){
			GUILayout.Toggle(gameManager.gameFields.followersBreakWithParent, "",GUILayout.Width(50));
		}else{
			gameManager.gameFields.followersBreakWithParent = GUILayout.Toggle(gameManager.gameFields.followersBreakWithParent, "",GUILayout.Width(50));
		}
	}
	GUILayout.EndHorizontal();
}
//Return a user based on network player
function GetUser(netPlr : NetworkPlayer) : UserClass{ 
	for(var i : int = 0; i < playerList.Count; i++){ 
    	if(netPlr == playerList[i].netPlr){
    		return playerList[i];
    	}
    }
    return null;
}
//Return a user based on name
function GetUser(name : String) : UserClass{ 
	for(var i : int = 0; i < playerList.Count; i++){ 
    	if(name == playerList[i].name){
    		return playerList[i];
    	}
    }
    return null;
}
//Return a user based on netView ID
function GetUser(netID : NetworkViewID) : UserClass{ 
	for(var i : int = 0; i < playerList.Count; i++){ 
    	if(netID == playerList[i].netID){
    		return playerList[i];
    	}
    }
    return null;
}

@RPC
function InsertChatMSG(name : String, msg : String){
	chatHolder.AddMsg(name+" : "+msg);
}  

@RPC
function RegisterUser(name : String,netPlr : NetworkPlayer,netID : NetworkViewID, isServer : boolean){
    var user : UserClass = new UserClass();
    user.name = name; 
    user.team = 0;
    user.netPlr = netPlr;
    user.netID = netID;
    user.isServer = isServer;
    playerList.Add(user);
    
    Debug.Log(name + "Registered");
    
    if(Network.isServer){ //Place New Player
    	//Auto Place Players in Teams
    	if(gameManager.gameFields.isTeamGame){
    		var team : int = 0;
    		if(playersInTeamBlue > playersInTeamRed){
    			team = 1;	
    		}
    		user.team = team;	
    		GetComponent.<NetworkView>().RPC("TeamSwapResponse",RPCMode.OthersBuffered,netPlr,team); //Tell Other About User Team Status
    	}else{ //Solo Game Play 
    	
    	}
    	if(gameManager.gameFields.gameStarted)gameManager.SetUpPlayer(user);
    }
    //Lets sort the list here//Everyone has same list
    var list : System.Array = playerList.ToArray();
   	System.Array.Sort(list, new CompareByName());
   	playerList.Clear();
   	for(var x : int = 0; x < list.Length; x++){
   		playerList.Add(list[x] as UserClass);
   	}
}

@RPC
function ReadyStatus(userID : NetworkViewID,status : int){ //user is setting his ready status
	for(var i : UserClass in playerList){
   		 if(i.netID == userID){
   		 	i.isReady = status; break;
   		 }
   	}
}

@RPC //A User Filed a complaint against a certain user
function UserComplaint(violatorID : NetworkViewID, reporter : NetworkViewID){
	var list : List.<NetworkViewID>;
	if(!userComplaintTable.ContainsKey(violatorID)){ //Check if the compliant board has this user already
		list = new List.<NetworkViewID>();
		list.Add(reporter);
		userComplaintTable.Add(violatorID,list); //Add to table
	}else{
		(userComplaintTable[violatorID] as List.<NetworkViewID>).Add(reporter);
	}			
}

@RPC
function TeamSwapRequest(netPlr : NetworkPlayer){ //Sent To Server to ask for a team swap
	for(var i : int = 0; i < playerList.Count; i++){ 
    	if(netPlr == playerList[i].netPlr){
    		if(playerList[i].team == 0){
    			if(Mathf.Abs(playersInTeamBlue - (playersInTeamRed+1)) < 2){
    				playerList[i].team = 1;
    				GetComponent.<NetworkView>().RPC("TeamSwapResponse",RPCMode.OthersBuffered,netPlr,1);
    			}
    		}else{
    			if(Mathf.Abs((playersInTeamBlue+1) - playersInTeamRed) < 2){
    				playerList[i].team = 0;
    				GetComponent.<NetworkView>().RPC("TeamSwapResponse",RPCMode.OthersBuffered,netPlr,0);
    			}
    		}
    		break;
    	}
    }	
}

@RPC
function TeamSwapResponse(netPlr : NetworkPlayer,teamInt : int){ //Sent To Users to swap a user's team status
	for(var i : int = 0; i < playerList.Count; i++){ 
    	if(netPlr == playerList[i].netPlr){
    		if(netPlr == Network.player){ //I asked For a Swap Response
    			Debug.Log("Team Swap Approved");
    		}
    		playerList[i].team = teamInt;
    		break;
    	}
    }	
}

@RPC
function DeRegisterUser(netPl : NetworkPlayer){
	for(var i : int = 0; i < playerList.Count; i++){ 
    	if(netPl == playerList[i].netPlr){
    		playerList.RemoveAt(i); 
    		break;
    	}
    }
}
//Clean Up Player Items & Registration
function CleanUpPlayer(netPl : NetworkPlayer){ 
	for(var i : int = 0; i < playerList.Count; i++){ 
    	if(netPl == playerList[i].netPlr){
    		if(gameManager.gameFields.gameStarted)gameManager.CleanUpPlayer(playerList[i]);
    		playerList.RemoveAt(i); 
    		break;
    	}
    }		
}

@script RequireComponent(NetworkView)
@script RequireComponent(NetworkUtilities)