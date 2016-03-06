#pragma strict
import System.Collections.Generic;
//
private var Type : bondType = bondType.delayed;
//
private var initialized : boolean = false; //Can we be used
//
var maxFollowers : int = 50; //Max number of followers that can be in the chain
private var followers : List.<ViralFollowerNetwork> = new List.<ViralFollowerNetwork>();
//User Move And  Turn
private var moveDirection : Vector3; //The Move Direction 
var moveSpeed : float = 10; // Speed Of Character 
var turnSpeed : float = 2.5; //Speed of Character turn
//The Indicator Points to last part of chain
private var lastFollower : Transform; 
private var myTransform : Transform; // Our tranform
var indicator : Transform; // This is used to show the last follower
var indicatorOffset : float = .5; // How far up is the indicator from the follower
//
var bondDistance : float = 2;
var bondDamping : float = 3;
//Controller
private var vDirection : float;
private var hDirection : float;
private var isMoving : boolean = false;
//Follower Movement 
private var wantedRotation : Quaternion;
private var currentRotation : Quaternion;
private var wantedRotationAngle : float;
private var currentRotationAngle : float;
private var wantedPosition : Vector3;

@HideInInspector var currentNetworkPlayer : NetworkPlayer;
private var gameManager : NetworkGameManager;

function Start(){
	gameManager = GameObject.FindObjectOfType(typeof(NetworkGameManager)) as NetworkGameManager;
	myTransform = transform; //Cashe us
}

function ReturnLast () : Transform{ //This determines that lastFollower follower on the chain
	if(followers.Count > 0){
		return followers[followers.Count -1].transform;
	}else{return myTransform;} //If we have more than 0 followers return the lastFollower 
	//else the player is the lastFollower;
}
//User Functions 
function GetViewID() : NetworkViewID{
	var nViews : Component[]= gameObject.GetComponentsInChildren(NetworkView);	
	return (nViews[0]  as NetworkView).viewID;
}
function Initialize (user : UserClass){
	GetComponent.<NetworkView>().RPC("InitializeNet",RPCMode.AllBuffered,user.name,user.netPlr,user.netID);
}
function DeInitialize (netID : NetworkViewID){
	GetComponent.<NetworkView>().RPC("DeInitializeNet",RPCMode.AllBuffered,netID);
}

@RPC
function InitializeNet (name : String,netPlayer : NetworkPlayer, netId : NetworkViewID){
	var nViews : Component[]= gameObject.GetComponentsInChildren(NetworkView);
    	(nViews[0]  as NetworkView  ).viewID = netId;
    	// Make the rigid body not change rotation
	if(GetComponent.<Rigidbody>()){
		GetComponent.<Rigidbody>().freezeRotation = true;}
	
	currentNetworkPlayer = netPlayer; //Asign netplayer
	gameObject.name = name; //Asign name
	
			
	if(!GetComponent.<NetworkView>().isMine){
		myTransform.tag = "Remote"; // Lest Tag remote //For other users
		initialized = false;
	}else{
		myTransform.tag = "Player"; // Lest Tag ourself //this helps the followers find "This object"
		initialized = true;
		ViralCameraNetwork.target = this.transform;
	}
	followers = new List.<ViralFollowerNetwork>();
}

@RPC
function DeInitializeNet(netID : NetworkViewID){
	initialized = false;
	var nViews : Component[]= gameObject.GetComponentsInChildren(NetworkView);
    (nViews[0]  as NetworkView  ).viewID = netID;
}

function Controller () : boolean{ //Controls the user and return an is moving 
	hDirection = Input.GetAxis("Horizontal");
	vDirection = Input.GetAxis("Vertical");
	
	//Process Last MoveDirection 
	moveDirection = Vector3.zero; // Reset Move Direction
	moveDirection = Vector3(0,0,vDirection);
	myTransform.Translate(moveDirection * Time.deltaTime * moveSpeed); // Move the leader
	
	myTransform.Rotate(Vector3.up * Time.deltaTime * hDirection * 100 * turnSpeed); // This Rotates the  leader
	//We return a boolean to check if we are moving
	return (hDirection + vDirection != Vector3.zero); 
}

function Update(){
	if((Network.player == currentNetworkPlayer) && GetComponent.<NetworkView>().isMine && initialized){
		//Gather MoveDirection and move user
		isMoving = Controller(); //This creates our Move Direction
		
		//This Keeps our indicator alwasy pointed to the lastFollower
		lastFollower = ReturnLast();
		
		if(indicator){ //if we have an indicator
			indicator.position = lastFollower.position + Vector3(0,indicatorOffset,0);
			indicator.rotation = transform.rotation;
		}
		
		var prevFollower : Transform;
		var follower : Transform;
		var nextPos : Vector3;
		var nextRot : Quaternion;
		
		//This process can all be shifted to the server for anti cheat & accuracy measures
		//Kept a an individual user process to save host from running an 0(n^2) operation
		//Calculating everything for everybody reduces the RPC calls, but might make the server run slower as
		//the server must perform more math calculations
		for(var i : int = 0; i < followers.Count; i++){ 
			//Fail Safe --- The server should keep track of follower's boded status & follower's current User
			if(!followers[i].bonded || followers[i].currentNetworkPlayer != Network.player){
				return;	
			}
			
			if(i == 0){
				prevFollower = transform;
			}else{
				prevFollower = followers[i-1].transform;
			}
			follower = followers[i].transform;

			wantedPosition = prevFollower.TransformPoint(0, 0, -bondDistance); 
			wantedRotation = Quaternion.LookRotation(prevFollower.position - follower.transform.position, prevFollower.up);	
			//Send The Information to the server
			if(Network.isClient){
				gameManager.GetComponent.<NetworkView>().RPC("UpdateFollowerPosition",RPCMode.Server,followers[i].followerID,Network.player,wantedPosition,wantedRotation);
			}else{
				gameManager.UpdateFollowerPosition(followers[i].followerID,Network.player,wantedPosition,wantedRotation);
			}
		}	
	}
}

// This Functions handle making new bonds whether the user hits the object or the object hits any part of the body
function OnCollisionEnter (obj : Collision){ // This if for when the leader hits an Object
	if(obj.collider.transform.tag == "Follower" && GetComponent.<NetworkView>().isMine){ // We have found a follower obj
		var follower : ViralFollowerNetwork = obj.collider.gameObject.GetComponent(ViralFollowerNetwork);
		//Lets try to establish a bond or break an existing one
		if(!followers.Contains(follower)){
			if(follower.bonded){
				var breakBond : boolean = false; //Our verdict to break a bond will be passed here
				if(gameManager.gameFields.isTeamGame){ //We Are in a Team Game
					var ownedbyTeammate : boolean = false;
					if(follower.currentOwnedTeam == gameManager.networkController.GetUser(follower.currentNetworkPlayer).team){ //TeamMate
						if(gameManager.gameFields.breakOwnTeamBonds){
							breakBond = true;
						}	
					}else{ //Enemy
						if(gameManager.gameFields.breakEnemyBonds){ //Can we break the enemies bond
							breakBond = true;
						}
					}
				}else{ //Not in A Team Based Game
					breakBond = true;
				}
				if(breakBond){ //our final verdict was to break the bond
					if(Network.isClient){
						gameManager.GetComponent.<NetworkView>().RPC("RequestingBreakBond",RPCMode.Server,follower.followerID,follower.currentNetworkPlayer);
					}else{
						gameManager.RequestingBreakBond(follower.followerID,follower.currentNetworkPlayer);
					}
				}
			}else{ //The follower is not in our chain
				if(followers.Count < maxFollowers){
					if(Network.isClient){
						gameManager.GetComponent.<NetworkView>().RPC("RequestingNewBond",RPCMode.Server,follower.followerID,Network.player); //Lets request a new bond
					}else{
						gameManager.RequestingNewBond(follower.followerID,Network.player);
					}
				}
			}	
		}else{ //We hit a follower already on the chain
			if(gameManager.gameFields.breakOwnBonds){ //Can we break our own bonds
				if(Network.isClient){
					gameManager.GetComponent.<NetworkView>().RPC("RequestingBreakBond",RPCMode.Server,follower.followerID,follower.currentNetworkPlayer);
				}else{
					gameManager.RequestingBreakBond(follower.followerID,follower.currentNetworkPlayer);
				}
			}
		}
	}
}

@RPC  //Sent back from server after a new bond has been approved
function BondRequestRespose(followerID : int,player : NetworkPlayer){
	Debug.Log("Bond Request Response : "+followerID);
	if(Network.player == player){
		Debug.Log(gameManager);
		var follower : ViralFollowerNetwork = gameManager.GetFollower(followerID);
		Debug.Log(follower);
		followers.Add(follower);
	}
}

@RPC //Sent from the server frocing user to break bonds
function BondRemoveCommand(followerID : int,player : NetworkPlayer){
	Debug.Log("Bond Remove Command : "+followerID);
	if(Network.player == player){
		if(!gameManager.gameFields.followersBreakWithParent){ //Only Break a single follower	
			for(var i : int = followers.Count -1; i > 0; i--){
				if(followers[i].currentNetworkPlayer == player && followers[i].followerID == followerID){
					var follower : ViralFollowerNetwork = followers[i];
					followers.RemoveAt(i);
					break;	
				}
			}
		}else{ //Break Chain of followers
		
		}
	}
}

@RPC
function UpdateScoreCommand(userID : NetworkViewID, score : int){ //Update the User Score
	(gameManager.networkController.GetUser(userID) as UserClass).score = score;
}