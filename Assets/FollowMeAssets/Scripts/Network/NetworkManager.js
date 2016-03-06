#pragma strict

//Global Vars
var gameName : String = "FollowMe";
var hostPort : int = 35000;
//Hidden Vars
@HideInInspector public var hostData : HostData[];
@HideInInspector public var nowConnecting : boolean = false;
//Private Vars
private var lastHostListRequest : float = 0;
private var isConnected : boolean = false;
private var useNat : boolean;
private var doneTesting : boolean;

private var testStatus = "Testing network connection capabilities.";
private var testMessage = "Test in progress";
private var shouldEnableNatMessage : String = "";
private var probingPublicIP = false;
private var connectionTestResult = ConnectionTesterStatus.Undetermined;
private var timer : float = 0;

var netController : NetworkController;
//Getters
function IsConnected(){
	return (Network.peerType != NetworkPeerType.Disconnected);
}

//Unity Functions
function Awake (){
	//MasterServer.ipAddress = "000.0.0.0";
	//MasterServer.port = 00000;
	//Network.natFacilitatorIP = "000.0.0.0";
	//Network.natFacilitatorPort = 00000;
		
	Network.isMessageQueueRunning = true;
	//Basic Setup
	lastHostListRequest = Time.time; 
	hostData = new HostData[0];	
	//Set Nat
	useNat = !Network.HavePublicAddress();
}

function Update(){
	if(!isConnected){
		if(!doneTesting)TestConnection(); //Test Our Connection
		FetchHostList(); //Get The List Of Games
	}
}
//Connectors
function Connect(hostInfo : HostData){ //Using The GUID Way
	hostInfo.useNat = true; 
	Network.Connect(hostInfo.guid); 
	nowConnecting = true;
}

//Hosting
function StartHost(serverName : String, players : int, map : String, gameMode : String){
	Debug.Log("Initializing Host");
	Network.InitializeServer(players,hostPort,useNat);
	MasterServer.RegisterHost(gameName,serverName,map+"~"+gameMode);
}
function FetchHostList(){	
	if(Time.time > lastHostListRequest){
		lastHostListRequest = Time.time + 1;
		
		hostData = MasterServer.PollHostList();					
		MasterServer.RequestHostList(gameName);	
	}
}

//Network Callbacks- --------------------------------------------
function OnFailedToConnectToMasterServer(info: NetworkConnectionError){
	Debug.Log("Failed To Connect(MS) info : " + info);
}

function OnFailedToConnect(info: NetworkConnectionError){
	Debug.Log("Failed To Connect(SS) info : " + info);	
}

function OnConnectedToServer(){
	Debug.Log("Connected To Server");
	//Register
	netController.Initialize();
	isConnected = true;
}

function OnServerInitialized(){
	Debug.Log("Server Started");
	//Register
	netController.Initialize();
	isConnected = true;
}

function OnPlayerDisconnected(player : NetworkPlayer){
	netController.CleanUpPlayer(player); //We Want the server to clean up player as well 
	GetComponent.<NetworkView>().RPC("DeRegisterUser",RPCMode.OthersBuffered,player);
	Network.RemoveRPCs(player);
}

function OnDisconnectedFromServer(info : NetworkDisconnection){
	isConnected = false;
	netController.DeInitialize();
	if(Network.isServer){
        Debug.Log("Local server connection disconnected");
    }
}

function TestConnection() {
	// Start/Poll the connection test, report the results in a label and 
	// react to the results accordingly
	connectionTestResult = Network.TestConnection();
	switch (connectionTestResult){
	    case ConnectionTesterStatus.Error: 
	        testMessage = "Problem determining NAT capabilities";
	        doneTesting = true;
	        break;
	        
	    case ConnectionTesterStatus.Undetermined: 
	        testMessage = "Undetermined NAT capabilities";
	        doneTesting = false;
	        break;
	                    
	    case ConnectionTesterStatus.PublicIPIsConnectable:
	        testMessage = "Directly connectable public IP address.";
	        useNat = false;
	        doneTesting = true;
	        break;
	    // This case is a bit special as we now need to check if we can 
	    // circumvent the blocking by using NAT punchthrough
	    case ConnectionTesterStatus.PublicIPPortBlocked:
	        testMessage = "Non-connectable public IP address (port " +
	            hostPort +" blocked), running a server is impossible.";
	        useNat = false;
	        // If no NAT punchthrough test has been performed on this public 
	        // IP, force a test
	        if (!probingPublicIP) {
	            connectionTestResult = Network.TestConnectionNAT();
	            probingPublicIP = true;
	            testStatus = "Testing if blocked public IP can be circumvented";
	            timer = Time.time + 10;
	        }
	        // NAT punchthrough test was performed but we still get blocked
	        else if (Time.time > timer) {
	            probingPublicIP = false;         // reset
	            useNat = true;
	            doneTesting = true;
	        }
	        break;
	    case ConnectionTesterStatus.PublicIPNoServerStarted:
	        testMessage = "Public IP address but server not initialized, "+
	            "it must be started to check server accessibility. Restart "+
	            "connection test when ready.";
	        break;
	                    
	    case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
	        testMessage = "Limited NAT punchthrough capabilities. Cannot "+
	            "connect to all types of NAT servers. Running a server "+
	            "is ill advised as not everyone can connect.";
	        useNat = true;
	        doneTesting = true;
	        break;
	        
	    case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
	        testMessage = "Limited NAT punchthrough capabilities. Cannot "+
	            "connect to all types of NAT servers. Running a server "+
	            "is ill advised as not everyone can connect.";
	        useNat = true;
	        doneTesting = true;
	        break;
	    
	    case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
	    case ConnectionTesterStatus.NATpunchthroughFullCone:
	        testMessage = "NAT punchthrough capable. Can connect to all "+
	            "servers and receive connections from all clients. Enabling "+
	            "NAT punchthrough functionality.";
	        useNat = true;
	        doneTesting = true;
	        break;
	
	    default: 
	        testMessage = "Error in test routine, got " + connectionTestResult;
	}
	if(doneTesting){
	    if(useNat){
	        shouldEnableNatMessage = "When starting a server the NAT "+
	            "punchthrough feature should be enabled (useNat parameter)";
	    }else{
	        shouldEnableNatMessage = "NAT punchthrough not needed";
	    }
	    testStatus = "Done testing"; Debug.Log(testMessage);
	}
}