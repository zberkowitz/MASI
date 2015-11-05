using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;

public class ObjectOSC : MonoBehaviour {
	public string UniqueName;
	public string OSCClientName;
	private bool EnableFlag = true;

	// Update is called once per frame
	void Start(){
		//if no unique name specified, unique name = GameObject name
		if (UniqueName == ""){
			UniqueName = name;
		}

		// if no OSC client specified, take from main camera
		if (OSCClientName == ""){
			Camera camera = Camera.main;
			CameraOSC cameraScript = camera.GetComponent<CameraOSC>();
			OSCClientName = cameraScript.OSCClientName;
		}
	}

	void Update () {
		if (transform.hasChanged) {
			SendOSCPosition();
			transform.hasChanged = false;
		}

		if (EnableFlag){
			SendOSCEnable(1); //just do this once
			EnableFlag = !EnableFlag;
		}
	}
	
	private void SendOSCPosition(){
		List<object> objectPosition = new List<object> ();

		objectPosition.AddRange (new object[] {
			transform.position.x,
			transform.position.y,
			transform.position.z
		});
		
		OSCHandler.Instance.SendMessageToClient (OSCClientName, "/" + UniqueName + "/position", objectPosition);
	}

	private void SendOSCEnable(int enable){
		OSCHandler.Instance.SendMessageToClient (OSCClientName, "/" + UniqueName + "/enable", enable); 
	}
}
