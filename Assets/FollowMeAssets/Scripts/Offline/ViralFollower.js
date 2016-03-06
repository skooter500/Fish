#pragma strict

private var canMakeBonds : boolean = true; // This allows the follower to make bonds like the leader//This == ViralLeader.followerBonding
//
private var leader : ViralLeader; // This is the leader AKA Player
private var me : Transform; // This is us
//
var breakForce : float = 50; // Force required to breakforce need to break bond
private var bondHealth : float = 0; // We use this to decay The bond Strenght over time/over collisions 
//
@HideInInspector
var bonded : boolean = false; // the follower is currently Bonded to a parent 
@HideInInspector //We do not need to show this in the Inspector
var bondId : int; // This keeps our position on the array of the leader

var bondLink : GameObject; //This is the link object that sits in between followers//BondLink does not need colliders//
// With the proper bondLink size the chain can really look like as Snake
var Hp : TextMesh; // We Use this to display the bond Hp

function Start (){
	me = transform;
	me.tag = "Follower"; // We tag Here to keep refrence 
	leader = GameObject.FindWithTag("Player").gameObject.GetComponent(ViralLeader);
	
	if(GetComponent.<Rigidbody>()){
		GetComponent.<Rigidbody>().freezeRotation = true;} // Free The rigidbody rotation
		
	if(bondLink){ //This disables the bond link object on Wake
		bondLink.SetActive(false);
	}
}

function Update(){
	// This Comes from the unity Smooth Follow Script Adpated and Changed
	// Early out if we don't have a Parent
	if(bonded){
		canMakeBonds = ViralLeader.followerBonding;//The boolean from the leader allowing followers to make bonds
		if(GetComponent.<Rigidbody>() && GetComponent.<Rigidbody>().velocity != Vector3.zero){
			//rigidbody.velocity = Vector3.zero; // Freeze Velocity
		}
		//Here we check the bond health
		if(bondHealth <= 0){ //Make sure we are not running this always by checking parent 
			leader.RemoveMe(bondId); //Tell The leader to remove this follower;
		}
	}
}
function BreakBond (externalForce : float){ //Breaks the bond if the force is great enough
	if(!bonded){return;} // Only if we are bonded
	if(externalForce >= breakForce){ //the force we recieved is big enough to break the bond
		leader.RemoveMe(bondId); //Tell The leader to remove this follower
	}else{ // The force is not big enough to break it so we then decay the bondHealth
		bondHealth -= externalForce;
	}
	
	//Update Health
	if(Hp){
		Hp.text = bondHealth.ToString();
	}
}
function RemoveMe (){ // This is called from leader if parent's bond is broken
	Debug.Log("Removed : "+bondId); 
	bondHealth = 0;
	bonded = false;
	//Update Health
	if(Hp){
		Hp.text = bondHealth.ToString(); // Update The Hp
	}
	if(bondLink && bondLink.activeSelf != false){ //This disables the bond link object
		bondLink.SetActive(false);
	}
}
//====================================================================================
function MakeBond (id : int){ // This creates the bonds we need	
	bondHealth = 100; // Make sure the new bond is at full strenght
	bondId = id; // This is our id (position in the arrayList stored by the leader)
	bonded = true;
	//Update Health
	if(Hp){
		Hp.text = bondHealth.ToString();
	}
	//Lets Enable the link object
	if(bondLink && bondLink.activeSelf != true){
		bondLink.SetActive(true);
	}
}
//This section handles Collisions
function OnCollisionEnter (obj : Collision){
	if(!bonded){return;} // Only if we are bonded
	//Perform these actions if we are bonded
	if(obj.collider.transform.tag == "Follower"){ // We have hit another follower
		if(canMakeBonds){ // Only if we can make bonds and the leader is done bonding
			leader.BondThis(obj.transform);
		} //This tells the user to bond this loose follower to the chain
	}
}
