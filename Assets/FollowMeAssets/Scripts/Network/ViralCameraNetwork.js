#pragma strict

var cameraType : FollowMeCamera = FollowMeCamera.Orbit;

static var target : Transform;
//
var maxRotationX : float = 30;
var maxRotationY : float = 30;
var sensativity : float = 100;
var cameraSpeed : float = 5;
var maxCameraSlide : float = 20;
//
var height : float = 5;
var distance : float = 15;
private var myTransform : Transform;
private var dDir : Vector2; 
private var originalRot : Vector3;
private var currentRot : Vector3;
private var slide : float = 0;

function Start (){
	originalRot = transform.eulerAngles;
	myTransform = transform;
	Reset();
}
function Reset (){
	currentRot = originalRot;
}
//Operations
function Update(){
	if(Input.GetKeyDown(KeyCode.C)){
		switch (cameraType){
			case FollowMeCamera.Fixed:
				cameraType = FollowMeCamera.Orbit;
				height = 5; distance = 15;
				break;
			case FollowMeCamera.Orbit:
				cameraType = FollowMeCamera.Chase;
				height = 10; distance = 15;
				break;
			case FollowMeCamera.Chase:
				cameraType = FollowMeCamera.Fixed;
				height = 10; distance = 10;
				break;
		}
	}
}

function LateUpdate (){
	if(target == null)return;
	
	var position : Vector3; 
	var rotation : Quaternion;
	
	dDir.x = Input.GetAxis("Mouse X");
	dDir.y = Input.GetAxis("Mouse Y");
	
	if(cameraType == FollowMeCamera.Fixed){ //Fixed Camera Type
		if(dDir.x > 0){
			slide = Mathf.Lerp(slide,-maxCameraSlide,Time.deltaTime);
		}else if(dDir.x < 0){
			slide = Mathf.Lerp(slide,maxCameraSlide,Time.deltaTime);
		}
		position = target.position + new Vector3(slide,height,-distance);
		myTransform.position = Vector3.Slerp(myTransform.position,position,Time.deltaTime * cameraSpeed);
		myTransform.LookAt(target.position);
	}
	else if(cameraType == FollowMeCamera.Orbit){ //Orbit Type
		
	
		currentRot.y += dDir.y * Time.deltaTime * sensativity;	
		currentRot.x += -dDir.x * Time.deltaTime * sensativity;
		//
		currentRot.y = ClampAngle(currentRot.y,-maxRotationY * .5,maxRotationY);
		//
		rotation = Quaternion.Euler(currentRot.y,currentRot.x,0);
	    position = rotation * Vector3(0.0, height, -distance) + target.position;
	    //   
	    myTransform.rotation = rotation;
    	myTransform.position = position;
    }
    else if(cameraType == FollowMeCamera.Chase){ //Cahse Cam
    	position = target.TransformPoint(0, height, -distance); 
		myTransform.position = Vector3.Lerp (myTransform.position, position, Time.deltaTime * cameraSpeed); 
	
		rotation = Quaternion.LookRotation(target.position - myTransform.position, target.up);
		myTransform.rotation = Quaternion.Slerp (myTransform.rotation, rotation, Time.deltaTime * cameraSpeed);
    }
}

function ClampAngle (angle : float, min : float, max : float) : float{
	if(angle < -360){angle += 360;}
	if(angle > 360){angle -= 360;}
	return Mathf.Clamp (angle, min, max);
}