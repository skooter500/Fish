#pragma strict

import System.Collections.Generic;

private class MSG{ //Internal class to store messages. we utilize this with the queue class
	var msg : String;
	var duration : int;
	
	function MSG(m : String,d : int){
		this.msg = m; this.duration = d;
	}
}
		
private var messageQueue : Queue = new Queue(); //Holds the msgs that need to be shown
private var nextMsgTimer : float = 0; 
private var nextMsgMessage : String = "";
private var boxPosition : Rect = new Rect(Screen.width/2 + 190,Screen.height/2 + 225, 200, 50);
	
function Start(){
	messageQueue = new Queue();	
}
function Update(){
	RunMessageQueue(); //Check & Dissplay queued nessages
}
function OnGUI(){
	if(Time.time < nextMsgTimer){
		GUI.Box(boxPosition,nextMsgMessage);
	}
}
//Add a message to the queue, give actual msg, and a duration
function AddMessage(msg : String,duration : int,sendToOthers : boolean){
	var msgClass : MSG = new MSG(msg,duration);
	messageQueue.Enqueue(msgClass);
	//Do we send this netoworkly
	if(sendToOthers){
		GetComponent.<NetworkView>().RPC("AddMessageNet",RPCMode.Others,msg,duration); //Send The msg to the network
	}	
}

@RPC //Add a Message to the queue
function AddMessageNet(msg : String,duration : int){
	var msgClass : MSG = new MSG(msg,duration);
	messageQueue.Enqueue(msgClass);
}

//Internal Functions
private function RunMessageQueue(){//Check if we still have Message to show
	if(messageQueue.Count > 0){
		if(Time.time > nextMsgTimer + 1){ //We add a sec to add space btwn msgs
			var msg : MSG = (messageQueue.Dequeue()) as MSG;
			nextMsgMessage = msg.msg;
			nextMsgTimer = Time.time + msg.duration;	
		}
	}	
}

@script RequireComponent(NetworkView)