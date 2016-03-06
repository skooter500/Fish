#pragma strict
import System.Collections.Generic;
var leader : ViralLeader;
//MAKE SURE YOU REMOVE THIS FOR IOS ONGUI IS TOO EXPENSIVE
function OnGUI(){
	GUILayout.BeginVertical();
	GUILayout.Space(5);
	
	GUILayout.Box("Following : "+ leader.followers.Count + "/" + leader.maxFollowers,GUILayout.Width(115)); // The Number of followers we have

	GUILayout.Space(5);
	
	GUILayout.BeginHorizontal();
	if(GUILayout.Button("Bond Type :",GUILayout.Width(115))){ // this changes the bond Type
		if(leader.Type == bondType.constant){
			leader.Type = bondType.delayed;
		}else if(leader.Type == bondType.delayed){
			//Type = bondType.flock;
			leader.Type = bondType.constant;
		}
		//else if(Type == bondType.flock){
			leader.Type = bondType.constant;
		//}
	}
	GUILayout.Label("" + leader.Type);
	GUILayout.EndHorizontal();
	
	GUILayout.Space(5);
	
	if(GUILayout.Button("Clear Followers", GUILayout.Width(115))){ //This Clears the chain
		leader.followersCanBreak = true;//Make Sure followers can break first
		if(leader.followers.Count > 0)
		leader.RemoveAll();
		leader.followersCanBreak = false;	
	}
	
	GUILayout.Space(5);
	
	GUILayout.BeginHorizontal();
	if(GUILayout.Button("Followers Break :", GUILayout.Width(115))){ //Enables and disables followers breaking Off from chain
		if(leader.followersCanBreak){leader.followersCanBreak = false;}
		else{leader.followersCanBreak = true;}
	}
	GUILayout.Label("" + leader.followersCanBreak);
	GUILayout.EndHorizontal();
	
	GUILayout.Space(5);
	
	GUILayout.BeginHorizontal();
	if(GUILayout.Button("User Can Break :", GUILayout.Width(115))){ //Enables and disables can break
		if(leader.canBreakFollowers){leader.canBreakFollowers = false;}
		else{leader.canBreakFollowers = true;}
	}
	GUILayout.Label("" + leader.canBreakFollowers);
	GUILayout.EndHorizontal();
	
	GUILayout.Space(5);
	
	GUILayout.BeginHorizontal();
	if(GUILayout.Button("Followers Bond :", GUILayout.Width(115))){ //Enables and disables followers making Bonds
		if(leader.followersCanBond){leader.followersCanBond = false;}
		else{leader.followersCanBond = true;}
	}
	GUILayout.Label("" + leader.followersCanBond);
	GUILayout.EndHorizontal();
	
	GUILayout.Space(5);
	
	GUILayout.BeginHorizontal();
	if(GUILayout.Button("Mass Breaking :", GUILayout.Width(115))){ //Enables and disables followers making Bonds
		if(leader.followersBreakWithParent){leader.followersBreakWithParent = false;}
		else{leader.followersBreakWithParent = true;}
	}
	GUILayout.Label("" + leader.followersBreakWithParent);
	GUILayout.EndHorizontal();
	
	GUILayout.Space(5);
	
	GUILayout.Label("Move Speed : " + leader.moveSpeed);
	leader.moveSpeed = GUILayout.HorizontalScrollbar(leader.moveSpeed,0.0,0.0,10.0,GUILayout.Width(115)); //Contols the move speed
	leader.moveSpeed = Mathf.CeilToInt(leader.moveSpeed); //this rounds the move speed to nearest 100
	
	GUILayout.Space(5);
	
	GUILayout.Label("Turn Speed : " + leader.turnSpeed);
	leader.turnSpeed = GUILayout.HorizontalScrollbar(leader.turnSpeed,0.0,0.0,5.0,GUILayout.Width(115)); //Contols the turn speed
	leader.turnSpeed = Mathf.CeilToInt(leader.turnSpeed); //this rounds the move speed to nearest 100
	
	GUILayout.Space(5);
	
	GUILayout.Label("Bond Distace : " + leader.bondDistance);
	leader.bondDistance = GUILayout.HorizontalScrollbar(leader.bondDistance,0.0,1.5,5.0,GUILayout.Width(115)); //Contols the bond distance
	//bondDistance = Mathf.CeilToInt(bondDistance); //this rounds the move speed to nearest 100
	
	GUILayout.Space(5);
	
	GUILayout.Label("Bond Damping : " + leader.bondDamping);
	leader.bondDamping = GUILayout.HorizontalScrollbar(leader.bondDamping,0.0,1.0,5.0,GUILayout.Width(115)); //Contols the bond damping
	leader.bondDamping = Mathf.CeilToInt(leader.bondDamping); //this rounds the move speed to nearest 100
	
	GUILayout.EndVertical();
}