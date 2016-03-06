#pragma strict
//Attach this to any object that is allowed to break bonds

var force : float = 50;
private var gameManager : NetworkGameManager; //Game Manager

function Start(){
	gameManager = GameObject.FindObjectOfType(typeof(NetworkGameManager)) as NetworkGameManager;
}

function OnCollisionEnter (obj : Collision){
	if(obj.collider.transform.tag == "Follower"){ // This adds force to the chain if hit
		if(Network.isServer){ //We only do this in the server as the server is reponsible for moving the followers
			var follower : ViralFollowerNetwork = obj.gameObject.GetComponent(ViralFollowerNetwork) as ViralFollowerNetwork;
			gameManager.RequestingDamageBond(follower.followerID,follower.currentNetworkPlayer,force);
		}
	}
}
