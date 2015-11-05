using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;

public class CameraOSC : MonoBehaviour {
	public string OSCClientName = "Max";
	
	void Awake () {
		OSCHandler.Instance.Init ();
	
	}

	void Update () {
		SendOSC ();
	}


	private void SendOSC(){
		List<object> cameraRotation = new List<object> ();
		List<object> cameraPosition = new List<object> ();
		
		cameraRotation.AddRange (new object[] {
			((UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye).eulerAngles.x) + transform.eulerAngles.x)%360.0f,
			((UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye).eulerAngles.y) + transform.eulerAngles.y)%360.0f,
			((UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye).eulerAngles.z) + transform.eulerAngles.z)%360.0f		
		});
		
		cameraPosition.AddRange (new object[] {
			transform.position.x,
			transform.position.y,
			transform.position.z
		});
		
		OSCHandler.Instance.SendMessageToClient (OSCClientName, "/rotation", cameraRotation);
		OSCHandler.Instance.SendMessageToClient (OSCClientName, "/position", cameraPosition);
	}
}
