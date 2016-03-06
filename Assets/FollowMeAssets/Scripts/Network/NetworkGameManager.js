#pragma strict

import System.Collections.Generic;

private var userModels : ViralLeaderNetwork[];
var usedUserModels : boolean[];
var followerModels : ViralFollowerNetwork[];
private var followerHash : Hashtable; //Hashtables provide O(1) insert and lookup = fast for searching for followers

//Game Stats, Vars, Options Etc
private class GameFields{
	var teamRedScore : int = 0;
	var teamBlueScore : int = 0;
	var totalGameTime : float = 300; //How Long is a game
	@HideInInspector var currentGameTime : float;
	@HideInInspector var startTime : float;
	@HideInInspector var timeIndexPT : float; //Critical Points in the game time
	var followersBreakWithParent : boolean = false; //Single Or Multi Follower Breaking
	var breakOwnTeamBonds : boolean = true;
	var breakOwnBonds : boolean = true;
	var breakEnemyBonds : boolean = true;
	var gameStarted : boolean = false;
	@HideInInspector var gameStarting : boolean;
	var isTeamGame : boolean = true;
}

var gameFields : GameFields = new GameFields();
var networkController : NetworkController;

function Start(){
	//We Need the networkview attention
	GetComponent.<NetworkView>().observed = this;
	
	userModels = GameObject.FindObjectsOfType(typeof(ViralLeaderNetwork)) as ViralLeaderNetwork[];
	followerModels = GameObject.FindObjectsOfType(typeof(ViralFollowerNetwork)) as ViralFollowerNetwork[];
}

function GetFollower(followerID : int) : ViralFollowerNetwork{
	//Return a follower by id
	for(var i : int = 0; i < followerModels.length; i++){
		if(followerModels[i].followerID == followerID){
			return followerModels[i];
		}
	}
	return null;
}
function Initialize (){
	//Assign Follower IDs
	followerHash = new Hashtable();
	for(var i : int = 0; i < followerModels.length; i++){
		followerModels[i].followerID = i;
		followerModels[i].nextPosition = followerModels[i].transform.position;
		followerModels[i].nextRotation = followerModels[i].transform.rotation;
		followerHash.Add(i,followerModels[i]);
	}
	//Set Up The User Models
	for(var j : int = 0; j < userModels.length; j++){
		userModels[j].currentNetworkPlayer = Network.player; // Set the Current NetPlayer to Servers
		userModels[j].DeInitialize(NetworkViewID.unassigned);

	}
}

function StartGame(playerList : List.<UserClass>, waitTime : int){
	//Reset Timer & Scores
	gameFields.gameStarted = true;
	gameFields.startTime = Time.time;
	gameFields.timeIndexPT = 0; //We have not crossed a time index
	gameFields.teamRedScore = 0;
	gameFields.teamBlueScore = 0;	
	//Reset Certain Attributes
	networkController.ResetPlayerList();
	GetComponent.<NetworkView>().RPC("ResetGame",RPCMode.OthersBuffered);
	//(re)Start Game 
	followerHash = new Hashtable();
	for(var i : int = 0; i < followerModels.length; i++){
		followerModels[i].followerID = i;
		followerModels[i].bonded = false;
		followerModels[i].bondHealth = 0;
		followerModels[i].currentOwnedTeam = -1;
		followerModels[i].currentNetworkPlayer = Network.player; //Set To Server Player
		followerHash.Add(i,followerModels[i]);
	}
	
	yield WaitForSeconds(waitTime); //Wait before giving command to players
	
	usedUserModels = new boolean[userModels.length];
	for(var j : int = 0; j < playerList.Count; j++){
		usedUserModels[j] = true;
		userModels[j].currentNetworkPlayer = playerList[j].netPlr; // Set the Current NetPlayer to User
		userModels[j].Initialize(playerList[j]);	
	}
}

function EndGame(){
	//decleare game as not started
	gameFields.gameStarted = false;
	//Send End Game msg
	networkController.utilities.AddMessage("GAME OVER",5,true);
	//Display The Winner 
	var winText : String = "";
	if(gameFields.isTeamGame){
		if(gameFields.teamBlueScore > gameFields.teamRedScore){
			winText = "Team Blue Won";
		}else if(gameFields.teamBlueScore == gameFields.teamRedScore){
			winText = "Game was a Tie";
		}else{
			winText = "Team Red Won";
		}
	}else{
		//Which Player Won...
		//Possibly award attributes or xp to plyers
	}
	networkController.utilities.AddMessage(winText,15,true);
	//DeInitialize all of our objects(followers)
	for(var i : int = 0; i < followerModels.length; i++){
		followerModels[i].followerID = i;
		followerModels[i].nextPosition = followerModels[i].transform.position;
		followerModels[i].nextRotation = followerModels[i].transform.rotation;
		followerModels[i].bonded = false;
		followerModels[i].currentNetworkPlayer = Network.player;
		followerModels[i].currentOwnedTeam = -1;
	}
	//Turn off the users
	for(var j : int = 0; j < userModels.length; j++){
		userModels[j].currentNetworkPlayer = Network.player; // Set the Current NetPlayer to Servers
		userModels[j].DeInitialize(NetworkViewID.unassigned);
	}
}

@RPC
function ResetGame(){
	//Reset Certain Attributes
	networkController.ResetPlayerList();	
}

function SetUpPlayer(player : UserClass){
	for(var j : int = 0; j < userModels.length; j++){
		if(usedUserModels[j] == false){
			usedUserModels[j] = true;
			userModels[j].currentNetworkPlayer = player.netPlr; // Set the Current NetPlayer to User
			userModels[j].Initialize(player);
			break;
		}	
	}	
}

function CleanUpPlayer(player : UserClass){
	for(var j : int = 0; j < userModels.length; j++){
		if(usedUserModels[j] == true && userModels[j].GetViewID() == player.netID){
			userModels[j].currentNetworkPlayer = Network.player; // Set the Current NetPlayer to Servers
			userModels[j].DeInitialize(NetworkViewID.unassigned);
			usedUserModels[j] = false;
			break;
		}	
	}
}

function Update (){
	if(Network.isServer){
		if(gameFields.gameStarted){
			//Adjust Time
			var gTime = Time.time - gameFields.startTime;
			gameFields.currentGameTime = gameFields.totalGameTime - gTime;
			
			//TimeIndexing is just done to make cerain msgs are fired before game proceeds
			//We can create as many index points as need using this simple if staement block
			//Indexing starts at the 
			if(gameFields.currentGameTime <= 10 && gameFields.timeIndexPT < 1){ //Ten Seconds Left //Index still at 0
				networkController.utilities.AddMessage("10 Seconds Left",2,true);
				gameFields.timeIndexPT++; //Go to next Index
			}
			else if(gameFields.currentGameTime <= 5 && gameFields.timeIndexPT < 2){ //Ten Seconds Left //We kno is index 1
				networkController.utilities.AddMessage("5 Seconds Left",2,true);
				gameFields.timeIndexPT++; //Go to next Index
			}
			else if(gameFields.currentGameTime <= 0){ //Game Over //No need to check index this will run without
				EndGame(); //Game Over !
			} 
		}
		//Manage The Follower sync posisiton and etc
		ManageFollowers();
		//Calculate score of most recescent or current game
		if(gameFields.isTeamGame){ //Check for a team game
			gameFields.teamRedScore = 0;
			gameFields.teamBlueScore = 0;
			//Check Score By Checking The Users Scores
			for(var user : UserClass in networkController.playerList){
				if(user.team == 0){
					gameFields.teamBlueScore+= user.score;	
				}else if(user.team == 1){
					gameFields.teamRedScore+= user.score;
				}
			}
		}
	}
	
}
//perform game specific utilities that require the player list
function PerformUtilities(plList : List.<UserClass>){
	

}
/*Server Moves The Followers Based on Received Positions From User Registered
 *The Followers are Moved, position is then Serialized & synced 
 */
private function ManageFollowers(){
	for(var follower : ViralFollowerNetwork in followerModels){
		if(follower.bonded){
			follower.transform.position = Vector3.Lerp(follower.transform.position,follower.nextPosition,Time.deltaTime * 5);
			follower.transform.rotation = Quaternion.Slerp(follower.transform.rotation,follower.nextRotation,Time.deltaTime * 5);
			if(follower.bondHealth <= 0){ //Bond Health = 0;
				RequestingBreakBond(follower.followerID, follower.currentNetworkPlayer);
			}
		}
		//Map The Follower to the approiate Team
		if(!follower.bonded){
			follower.currentOwnedTeam = -1;
		}else{
			try{
				var user : UserClass = networkController.GetUser(follower.currentNetworkPlayer);
				follower.currentOwnedTeam = user.team;
			}catch(e : System.Exception){
				follower.currentOwnedTeam = -1;
				//TODO ? Unbond The FOllower
			}
		}		
	}
}

@RPC
function RequestingBreakBond(followerID : int , player : NetworkPlayer){
	if(followerHash.Contains(followerID)){
		Debug.Log("Bond Break Request : "+followerID);
		var follower : ViralFollowerNetwork;
		follower = followerHash[followerID] as ViralFollowerNetwork;
		if(follower.currentNetworkPlayer == player && follower.bonded && Time.time > follower.collideWithUserTimer){
			//Remove Player Bond
			follower.bonded = false;
			follower.bondHealth = 0;
			var netPlayer : NetworkPlayer = follower.currentNetworkPlayer;
			follower.collideWithUserTimer = Time.time + 3; //Dissalow interaction for a second
			//Count User Score Here
			var user : UserClass = networkController.GetUser(player);
			user.score--;
			GetComponent.<NetworkView>().RPC("UpdateScore",RPCMode.OthersBuffered,player,user.score); //Tell others to update score
			//Set To Server Player
			follower.currentNetworkPlayer = Network.player; 
			follower.currentOwnedTeam = -1;
			for(var i : int = 0; i < userModels.length; i++){
				if(player == Network.player){
					userModels[i].BondRemoveCommand(followerID,player);
					break;
				}else{
					if(userModels[i].currentNetworkPlayer == player){
						userModels[i].GetComponent.<NetworkView>().RPC("BondRemoveCommand",player,followerID,netPlayer);
						break;
					}
				}
			}
		}
	}
}
 

@RPC
function RequestingDamageBond(followerID : int , player : NetworkPlayer, dmg : int){
	if(followerHash.Contains(followerID)){
		Debug.Log("Bond Damage Request : "+followerID);
		var follower : ViralFollowerNetwork;
		follower = followerHash[followerID] as ViralFollowerNetwork;
		if(follower.currentNetworkPlayer == player && follower.bonded && Time.time > follower.collideWithUserTimer){
			//Degrade HP
			follower.bondHealth -= dmg;
			follower.collideWithUserTimer = Time.time + 1; //Dissalow interaction for a second
		}
	}
}

@RPC
function RequestingNewBond(followerID : int , player : NetworkPlayer){
	if(followerHash.Contains(followerID)){
		var follower : ViralFollowerNetwork;
		follower = followerHash[followerID] as ViralFollowerNetwork;
		if(follower.currentNetworkPlayer == Network.player && !follower.bonded && Time.time > follower.collideWithUserTimer){ //If Server is controlling
			//Allow The Player To Bond
			follower.bonded = true;
			follower.bondHealth = 100;
			follower.currentNetworkPlayer = player;
			follower.collideWithUserTimer = Time.time + 3; //Dissalow interaction for a second
			//Change Team
			follower.currentOwnedTeam = networkController.GetUser(player).team;
			for(var i : int = 0; i < userModels.length; i++){
				if(player == Network.player){
					userModels[i].BondRequestRespose(followerID,player); break;
				}else{
					if(userModels[i].currentNetworkPlayer == player){
						userModels[i].GetComponent.<NetworkView>().RPC("BondRequestRespose",player,followerID,player); break;
					}
				}
			}
			//Count User Score Here
			var user : UserClass = networkController.GetUser(player);
			user.score++;
			GetComponent.<NetworkView>().RPC("UpdateScore",RPCMode.OthersBuffered,player,user.score); //Tell others to update score
		}
	}
}

@RPC //Sync up score with host
function UpdateScore(netPlr : NetworkPlayer, score : int){
	var user : UserClass = networkController.GetUser(netPlr);
	user.score = score;		
}


@RPC //We Set The next position
function UpdateFollowerPosition(followerID : int , player : NetworkPlayer, pos : Vector3, rot : Quaternion){
	if(followerHash.Contains(followerID)){
		var follower : ViralFollowerNetwork;
		follower = followerHash[followerID] as ViralFollowerNetwork;
		if(follower.currentNetworkPlayer == player){
			follower.nextPosition = pos;
			follower.nextRotation = rot;
		}
	}
}
//Lets Sync Game Info
function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo){
	if (stream.isWriting){//Write To Others
		stream.Serialize(gameFields.teamRedScore);
		stream.Serialize(gameFields.teamBlueScore);
		stream.Serialize(gameFields.totalGameTime);
		stream.Serialize(gameFields.currentGameTime);
		stream.Serialize(gameFields.gameStarted);
		stream.Serialize(gameFields.breakOwnTeamBonds);
		stream.Serialize(gameFields.breakOwnBonds);
		stream.Serialize(gameFields.breakEnemyBonds);
		stream.Serialize(gameFields.followersBreakWithParent);
		stream.Serialize(gameFields.isTeamGame);
	}
	else{//Executed on the others; 
		stream.Serialize(gameFields.teamRedScore);
		stream.Serialize(gameFields.teamBlueScore);
		stream.Serialize(gameFields.totalGameTime);
		stream.Serialize(gameFields.currentGameTime);
		stream.Serialize(gameFields.gameStarted);
		stream.Serialize(gameFields.breakOwnTeamBonds);
		stream.Serialize(gameFields.breakOwnBonds);
		stream.Serialize(gameFields.breakEnemyBonds);
		stream.Serialize(gameFields.followersBreakWithParent);
		stream.Serialize(gameFields.isTeamGame);	
	}
}

function OnGUI (){
	if(Network.peerType != NetworkPeerType.Disconnected){
		var mins : int = Mathf.CeilToInt(gameFields.currentGameTime) / 60;
		var secs : int = Mathf.CeilToInt(gameFields.currentGameTime) % 60;
		var text = String.Format ("{0:00}:{1:00}", mins, secs); 

		GUI.Box(new Rect(Screen.width/2 - 125,0,100,35),"Time [ " + mins + " : " + secs + " ]");
		if(gameFields.isTeamGame){
			GUI.Box(new Rect(Screen.width/2 - 25,0,150,35),"[ Red " + gameFields.teamRedScore +" : "+ gameFields.teamBlueScore +  " Blue ]"); 
		}else{
			GUI.Box(new Rect(Screen.width/2 - 25,0,150,35),"");
		}
	}
}

@script RequireComponent(NetworkView)