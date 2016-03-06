#pragma strict
import System.Collections.Generic;
//enum bondType {constant , delayed, flock}
enum bondType {constant , delayed} // This is an enum for the two different follow types
var Type : bondType = bondType.delayed;
//
var canBreakFollowers : boolean = true; // Set this true to allow leader to breakbonds ala "Snake"
var followersCanBreak : boolean = true; //This is allows the chain to break
var followersCanBond : boolean = false; // this allows followers to make bonds also (Can Be unstable if "On", and alot of collisions are going On)
var followersBreakWithParent : boolean = false;
static var followerBonding : boolean = true; //this is what the followers refrence to make sure they can make bonds
//
var maxFollowers : int = 10; //Max number of followers that can be in the chain
//var followers : ArrayList = new ArrayList(); //Our Follower List
var followers : List.<GameObject> = new List.<GameObject>();
static var editingList : boolean = false; // We use this to check that we are not editingList before we add or remove
//User Move And  Turn
private var moveDirection : Vector3; //The Move Direction 
var moveSpeed : float = 10; // Speed Of Character 
var turnSpeed : float = 5; //Speed of Character turn
var followersLinearSpeed : float = 5;
//The Indicator Points to last part of chain
static var last : Transform; //This always points to the last follower//We make it static so it is universal
private var me : Transform; // Our tranform
var indicator : Transform; // This is used to show the last follower
var indicatorOffset : float = .5; // How far up is the indicator from the follower
//
var bondDistance : float = 2;
var bondDamping : float = 3;
//Controller
var vDirection : float;
var hDirection : float;
//Follower Move
private var wantedRotation : Quaternion;
private var currentRotation : Quaternion;
private var wantedRotationAngle : float;
private var currentRotationAngle : float;
private var wantedPosition : Vector3;
/*/flock
var flockCenter : Vector3;
var flockVelocity : Vector3;
private var minVelocity : float = 5;
private var maxVelocity : float = 10;
private var randomness : float = 1;
*/

private var isMoving : boolean = false; // Check if the user is moving

function ReturnLast () : Transform{ //This determines that last follower on the chain
	if(followers.Count > 0){
		return followers[followers.Count -1].transform;
	}else{return me;} //If we have more than 0 followers return the last 
	//else the player is the last;
}
//User Functions 
function Start (){
	
	me = transform; //Cashe us
	me.tag = "Player"; // Lest Tag ourself //this helps the followers find "This object"
	// Make the rigid body not change rotation
	if(GetComponent.<Rigidbody>()){
		GetComponent.<Rigidbody>().freezeRotation = true;}
}

function Controller () : boolean {
    hDirection = 0; //Input.GetAxis("Horizontal");
    vDirection = 0; // Input.GetAxis("Vertical");
	
	//Process Last MoveDirection 
	moveDirection = Vector3.zero; // Reset Move Direction
	moveDirection = Vector3(0,0,vDirection);
	me.Translate(moveDirection * Time.deltaTime * moveSpeed); // Move the leader
	
	me.Rotate(Vector3.up * Time.deltaTime * hDirection * 100 * turnSpeed); // This Rotates the  leader
	
	if(!Input.GetKey("escape")){ // Press Escape to free the mouse
		//Screen.lockCursor = true;
	}else{ Screen.lockCursor = false;}
	
	return (Mathf.Abs(hDirection+vDirection) > 0);
}

function Update(){
	//Gather MoveDirection and move user
	isMoving = Controller(); //This creates our Move Direction
	
	//This Keeps our indicator alwasy pointed to the last
	last = ReturnLast();
	
	if(indicator){ //if we have an indicator
		indicator.position = last.position + Vector3(0,indicatorOffset,0);
		indicator.rotation = transform.rotation;
	}
	
	followerBonding = followersCanBond; //This tells the followers wheater they can bond

	var prevFollower : Transform;
	var follower : Transform;
	
	var theCenter = transform.position;
   	var theVelocity = Vector3.zero;
 	
   	//for(var boid : GameObject in followers) {
		//theCenter   = theCenter + boid.transform.position;
		//theVelocity = theVelocity + boid.rigidbody.velocity;
   	//}
	//flockCenter = theCenter/(followers.Count);	
	//flockVelocity = theVelocity/(followers.Count);
			
	for(var i : int = 0; i < followers.Count; i++){
		if(i == 0){
			prevFollower = this.transform;
		}else{
			prevFollower = followers[i-1].transform;
		}
		
		follower = followers[i].transform;
		
		if(Type == bondType.constant){ // Constant follow type
			ConstantMovement(prevFollower,follower);	
		}else if(Type == bondType.delayed){ // Delayed Follow Type. // This separates the position and rotation
			DelayedMovement(prevFollower,follower);
		}
		//else if(Type == bondType.flock){
			//FlockMovement(prevFollower,follower);
		//}
	}
}

private function ConstantMovement(prevFollower : Transform,follower : Transform){
	// Calculate the current rotation angles
	wantedRotationAngle = prevFollower.eulerAngles.y;
	currentRotationAngle = follower.transform.eulerAngles.y;
	
	// Damp the rotation around the y-axis
	currentRotationAngle = Mathf.LerpAngle (currentRotationAngle,wantedRotationAngle,bondDamping * Time.deltaTime);
	// Convert the angle into a rotation
	currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	// Set the position of the camera on the x-z plane to:
	// bondDistance meters behind the prevFollower

	follower.transform.position = prevFollower.position;
	//follower.rigidbody.MovePosition((prevFollower.position - currentRotation * Vector3.forward * bondDistance) * Time.deltaTime * followersLinearSpeed);
	follower.transform.position -= currentRotation * Vector3.forward * bondDistance;
	//Always look at the prevFollower
	follower.transform.LookAt (prevFollower);
}

private function DelayedMovement(prevFollower : Transform,follower : Transform){
	wantedPosition = prevFollower.TransformPoint(0, 0, -bondDistance); 
	follower.transform.position = Vector3.Lerp (follower.transform.position, wantedPosition, Time.deltaTime * bondDamping); 

	wantedRotation = Quaternion.LookRotation(prevFollower.position - follower.transform.position, prevFollower.up);
	follower.transform.rotation = Quaternion.Slerp (follower.transform.rotation, wantedRotation, Time.deltaTime * bondDamping);
}
/*
private function FlockMovement(prevFollower : Transform,follower : Transform){
	var randomize 	= Vector3((Random.value *2) -1, (Random.value * 2) -1, (Random.value * 2) -1);
 
	randomize.Normalize();
	randomize *= randomness;
 
	var follow : Vector3 = transform.position;
 
	flockCenter = transform.position - follower.position;
	flockVelocity = flockVelocity - follower.rigidbody.velocity;
	follow = follow - follower.position;
 
	var result : Vector3 = flockCenter + flockVelocity + follow + randomize;
	
	
	follower.rigidbody.velocity = Vector3.Lerp(follower.rigidbody.velocity,follower.rigidbody.velocity + result,Time.deltaTime);

	// enforce minimum and maximum speeds for the boids
	var speed = follower.rigidbody.velocity.magnitude;
	if (speed > maxVelocity) {
		follower.rigidbody.velocity = follower.rigidbody.velocity.normalized * maxVelocity;
	} else if (speed < minVelocity) {
		follower.rigidbody.velocity = follower.rigidbody.velocity.normalized * minVelocity;
	}
	
	wantedRotation = Quaternion.LookRotation(prevFollower.position - follower.transform.position, transform.up);
	follower.transform.rotation = Quaternion.Slerp (follower.transform.rotation, wantedRotation, Time.deltaTime * bondDamping);

}
*/
// This Functions handle making new bonds whether the user hits the object or the object hits any part of the body
function OnCollisionEnter (obj : Collision){ // This if for when the leader hits an Object
	if(obj.collider.transform.tag == "Follower"){ // We have found a follower obj
		//Debug.Log("We found" + obj.transform.name);
		if(!followers.Contains(obj.gameObject)){
			Debug.Log("Collided With : " + obj.gameObject.name + " : " + Time.time);
			if(followers.Count < maxFollowers)BondThis(obj.gameObject.transform); //We call the bond Functions	
		}else{ //We hit a follower already on the chain
			if(canBreakFollowers){
				RemoveMe(obj.gameObject.GetComponent(ViralFollower).bondId);} //Lets the Bond Id then remove
		}
	}
}
function BondThis (obj : Transform) : void{ //This function is called from the follower who hits a random Object
	if(followers.Contains(obj.gameObject) || followers.Count >= maxFollowers){
		return;
	}
	if(!editingList){
		editingList = true; // Set this to return true so we cant add while editingList
		obj.gameObject.GetComponent(ViralFollower).MakeBond(followers.Count);
		followers.Add(obj.gameObject); //Now add the object
		editingList = false; //Done Editing
	}
}
//This section deals with editingList followers from chain
function RemoveMe (id : int ){ // This function is the "MASTER BREAKER" breaking begins here
	if(!followersCanBreak){ return;} // Only Run if followers can Break Off
	
	if(editingList){return;} // We are already removing something
	//we use a for loop to remove every follower starting from the follower that called this
	editingList = true; // Set this to return true so we cant add while editingList
	
	if(followersBreakWithParent){
		for(var i : int = followers.Count -1; i > 0; i--){
			var follower : ViralFollower = followers[i].GetComponent(ViralFollower);
			follower.RemoveMe();
			followers.RemoveAt(i);
			if(follower.bondId == id){
				break;
			}
		}
	}else{
		for(var j : int = followers.Count -1; j > 0; j--){
			follower = followers[j].GetComponent(ViralFollower);
			if(follower.bondId == id){
				follower.RemoveMe();
				followers.RemoveAt(j);
				break;
			}
		}	
	}
	editingList = false;
}
function RemoveAll(){
	editingList = true;
	for(var i : int = followers.Count -1; i > 0; i--){
		followers[i].GetComponent(ViralFollower).RemoveMe();
		followers.RemoveAt(i);
	}
	editingList = false;
}

function MustBreak (){ //This is called by a follower that is constantly colliding with another one causing chain to jitter
	followersCanBreak = true;
}
