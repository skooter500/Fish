#pragma strict

@System.NonSerialized var followerID : int; 
@System.NonSerialized var currentNetworkPlayer : NetworkPlayer;
@System.NonSerialized var nextPosition : Vector3;
@System.NonSerialized var nextRotation : Quaternion;
@System.NonSerialized var currentOwnedTeam : int = -1;
@System.NonSerialized var bonded : boolean = false; // the follower is currently Bonded to a parent 
//We Keep A Local Counter : This is tracked offline to allow some time btw collision with user 
@System.NonSerialized var collideWithUserTimer : float = 0;
//
private var canMakeBonds : boolean = true; // This allows the follower to make bonds like the leader//This == ViralLeader.followerBonding
//
private var leader : ViralLeaderNetwork; // This is the leader AKA Player
//
var bondHealth : float = 100; // We use this to decay The bond Strenght over time/over collisions 
//
var bondLink : GameObject; //This is the link object that sits in between followers//BondLink does not need colliders//
var Hp : TextMesh; // We Use this to display the bond Hp

//InterPolation Uses
private var tmpPos : Vector3;
private var tmpQuat : Quaternion;
var interpolationBackTime : double = 0.1; 

//State class fro holding sync info...using a class instead of a struct 	
private class State{ 
	internal var timestamp : double;
	internal var pos : Vector3;
	internal var rot : Quaternion;
}

// We store twenty states with "playback" information
private var m_BufferedState : State[] = new State[20];
// Keep track of what slots are used
private var m_TimestampCount : int;
	

function Start (){
	transform.tag = "Follower"; // We tag Here to keep refrence 
	//leader = GameObject.FindWithTag("Player").gameObject.GetComponent(ViralLeaderNetwork);
	for(var st : State in m_BufferedState){
		st = new State();	
	}
	
	if(GetComponent.<Rigidbody>()){
		GetComponent.<Rigidbody>().freezeRotation = true;} // Free The rigidbody rotation
}

function LateUpdate(){
	if(Network.isClient){
		GetComponent.<Rigidbody>().isKinematic = true;
	}
	//Lets Enable the link object
	if(bonded && bondLink && bondLink.activeSelf != true){
		bondLink.SetActive(true);
	}else if(!bonded && bondLink && bondLink.activeSelf != false){
		bondLink.SetActive(false);
	}
	if(Hp){
		Hp.text = bondHealth.ToString();
	}
}

function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo){
	if (stream.isWriting){//Sent From User To Others
		stream.Serialize(followerID);
		stream.Serialize(currentNetworkPlayer);
		stream.Serialize(bonded);
		stream.Serialize(bondHealth);
		stream.Serialize(currentOwnedTeam);		
		
		tmpPos = transform.position;
		tmpQuat = transform.rotation; 
		
		stream.Serialize(tmpPos);	
		stream.Serialize(tmpQuat);
	}
	else{//Read on Others; 
		stream.Serialize(followerID);
		stream.Serialize(currentNetworkPlayer);
		stream.Serialize(bonded);
		stream.Serialize(bondHealth);
		stream.Serialize(currentOwnedTeam);	
		
		tmpPos = Vector3.zero;
		tmpQuat = Quaternion.identity; 
		
		stream.Serialize(tmpPos);
		stream.Serialize(tmpQuat);
		
		// Shift buffer contents, oldest data erased, 18 becomes 19, ... , 0 becomes 1
		for (var i : int = m_BufferedState.Length-1; i >= 1; i--){
			m_BufferedState[i] = m_BufferedState[i-1];
		}
		
		// Save currect received state as 0 in the buffer, safe to overwrite after shifting
		var state : State = new State();
		state.timestamp = info.timestamp;
		state.pos = tmpPos;
		state.rot = tmpQuat;
		m_BufferedState[0] = state;
		
		// Increment state count but never exceed buffer size
		m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

		// Check integrity, lowest numbered state in the buffer is newest and so on
		for (var j : int = 0; j < m_TimestampCount-1; j++){
			if (m_BufferedState[j].timestamp < m_BufferedState[j+1].timestamp)
				Debug.Log("State inconsistent");
		}		
	}
}

function Update(){
	if(Network.peerType != NetworkPeerType.Disconnected && Network.isClient){
		//transform.position = Vector3.Lerp(transform.position,tmpPos,Time.deltaTime * 10);
		//transform.rotation = Quaternion.Lerp(transform.rotation,tmpQuat,Time.deltaTime * 10);

		var currentTime : double = Network.time;
		var interpolationTime : double = currentTime - interpolationBackTime;
		// We have a window of interpolationBackTime where we basically play 
		// By having interpolationBackTime the average ping, you will usually use interpolation.
		// And only if no more data arrives we will use extrapolation
		
		// Use interpolation
		// Check if latest state exceeds interpolation time, if this is the case then
		// it is too old and extrapolation should be used
		if (m_BufferedState[0].timestamp > interpolationTime){
			for (var i : int = 0; i < m_TimestampCount; i++){
				// Find the state which matches the interpolation time (time+0.1) or use last state
				if (m_BufferedState[i].timestamp <= interpolationTime || i == m_TimestampCount-1){
					// The state one slot newer (<100ms) than the best playback state
					var rhs : State = m_BufferedState[Mathf.Max(i-1, 0)];
					// The best playback state (closest to 100 ms old (default time))
					var lhs : State = m_BufferedState[i];
					
					// Use the time between the two slots to determine if interpolation is necessary
					var length : double = rhs.timestamp - lhs.timestamp;
					var t : float = 0.0F;
					// As the time difference gets closer to 100 ms t gets closer to 1 in 
					// which case rhs is only used
					if (length > 0.0001)
						t = ((interpolationTime - lhs.timestamp) / length);
					
					// if t=0 => lhs is used directly
					transform.position = Vector3.Lerp(lhs.pos, rhs.pos, t);
					transform.rotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
					return;
				}
			}
		}
		// Use extrapolation. Here we do something really simple and just repeat the last
		// received state. You can do clever stuff with predicting what should happen.
		else{
			var latest : State = m_BufferedState[0];
			
			transform.position = latest.pos;
			transform.rotation = latest.rot;
		}
	}
}