#pragma strict

var follower : Rigidbody;//what are we spawning 
var intervals : float = 2;//how long we wait between each spawn
var maxSpawn : int = 10;// The max number we need to spawn
private var currSpawn : int = 0; // How many have we spawned
//
function Start (){
	Spawn (); // Lest start spawning 
}
//Using instatiation is bad for iOs, i suggets using a pooling system
function Spawn (){
	//We Check if we are below the max spawn
	while(currSpawn < maxSpawn){
		//Wait and spawn as long as we are below max
		Instantiate(follower,Vector3(Random.Range(-20,20),transform.position.y,Random.Range(-20,20)),transform.rotation);
		currSpawn++;
		yield WaitForSeconds(intervals); // wait for the intervak
	}
}