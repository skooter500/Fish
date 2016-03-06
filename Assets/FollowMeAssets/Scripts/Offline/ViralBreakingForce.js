#pragma strict
//Attach this to any object that is allowed to break bonds
var Force : float = 50;
//
function OnCollisionEnter (obj : Collision){
	if(obj.collider.transform.tag == "Follower"){ // This adds force to the chain if hit
		obj.gameObject.GetComponent(ViralFollower).BreakBond(Force);
	}
}
