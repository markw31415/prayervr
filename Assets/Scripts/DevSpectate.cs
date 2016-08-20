using UnityEngine;
using System.Collections;

public class DevSpectate : MonoBehaviour {
	Transform t;
	Vector3 eul; // cached euler angles which we manipulate and clamp before putting it into the Transform 
	GameObject fpc;
	Camera devCam;
	Camera fpcCam;
	AudioListener devAL;
	AudioListener fpcAL;



	void Start() {
		t = transform;
		//eul = t.eulerAngles;
		devCam = GetComponent<Camera>();
		devAL = GetComponent<AudioListener>();

		fpc = GameObject.Find("FirstPersonCharacter");

		if (fpc) {
			fpcCam = fpc.GetComponentInChildren<Camera>();
			fpcAL = fpc.GetComponentInChildren<AudioListener>();
			Debug.Log("found FirstPersonCharacter");
		}else{
			Debug.Log("FirstPersonCharacter NOT FOUND!");
		}
	}


	void FixedUpdate() { // 50 frames per second
		maybeMove();
		maybeLookAround();
	}


	void Update() {
		maybeToggleCameras();
	}


	void maybeLookAround() {
		float sens = 1f; // sensitivity 
		eul.x += sens * Input.GetAxis("Mouse Y");
		eul.y += sens * Input.GetAxis("Mouse X");

		// clamp extreme up/down angles to prevent upside-down view 
		if (eul.x > 88f)
			eul.x = 88f;
		if (eul.x < -88f)
			eul.x = -88f;

		t.eulerAngles = eul;
	}


	void maybeMove() {
		float dist = 0.06f; // distance to move 
		var moveDelta = new Vector3(0,0,0);

		if (devCam.enabled) {
			if (Input.GetKey(KeyCode.W)) {
				moveDelta.z += dist;
			}
			if (Input.GetKey(KeyCode.S)) {
				moveDelta.z -= dist;
			}
			if (Input.GetKey(KeyCode.A)) {
				moveDelta.x -= dist;
			}
			if (Input.GetKey(KeyCode.D)) {
				moveDelta.x += dist;
			}
			if (Input.GetKey(KeyCode.Q)) {
				moveDelta.y += dist;
			}
			if (Input.GetKey(KeyCode.Z)) {
				moveDelta.y -= dist;
			}

			t.position += t.rotation * moveDelta;
		}
	}


	void maybeToggleCameras() {
		if (Input.GetKeyDown(KeyCode.Home)) {
			if (devCam.enabled) {
				Debug.Log("\n<color=green>NORMAL CAM</color>");

				devCam.enabled = false;
				devAL.enabled = false;
				fpcCam.enabled = true;
				fpcAL.enabled = true;
			}else{
				Debug.Log("\n<color=green>DEVELOPER SPECTATE CAM</color>");

				fpcCam.enabled = false;
				fpcAL.enabled = false;
				devCam.enabled = true;
				devAL.enabled = true;
			}
		}	
	}
}
