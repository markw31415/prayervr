using UnityEngine;
using System.Collections;

public class DevSpectate : MonoBehaviour {
	float speedDelta = 0.002f; // how much to increment speed 
	float frac = 0.85f; // move towards zero speed by multiplying with this fraction (of 1f) 
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
	}


	void Update() {
		maybeToggleCameras();
		maybeLookAround();
	}


	void maybeLookAround() {
		if (devCam.enabled) {
			float sensX = 2f; // sensitivity 
			float sensY = 1f; // sensitivity 
			eul.y += sensX * Input.GetAxis("Mouse X");
			eul.x += sensY * Input.GetAxis("Mouse Y");

			// clamp extreme up/down angles to prevent upside-down view 
			if (eul.x > 88f)
				eul.x = 88f;
			if (eul.x < -88f)
				eul.x = -88f;

			t.eulerAngles = eul;
		}
	}


	Vector3 localSpeed = Vector3.zero; // local space relative to player 
	void maybeMove() {
		if (devCam.enabled) {
			float maxSpeed = 10f;

			// forward/back axis 
			updateAxisSpeedWithPlusAndMinusKeys(ref localSpeed.z, KeyCode.W, KeyCode.S);

			// lateral axis 
			updateAxisSpeedWithPlusAndMinusKeys(ref localSpeed.x, KeyCode.D, KeyCode.A);

			// up/down axis 
			updateAxisSpeedWithPlusAndMinusKeys(ref localSpeed.y, KeyCode.Q, KeyCode.Z);

			localSpeed.x = Mathf.Clamp(localSpeed.x, -maxSpeed, maxSpeed);
			localSpeed.y = Mathf.Clamp(localSpeed.y, -maxSpeed, maxSpeed);
			localSpeed.z = Mathf.Clamp(localSpeed.z, -maxSpeed, maxSpeed);
			t.position += t.rotation * localSpeed;
		}
	}


	void updateAxisSpeedWithPlusAndMinusKeys(ref float speed, KeyCode plus, KeyCode minus) {
		if (Input.GetKey(plus)) {
			if (speed < 0f)
				speed *= frac/2;

			speed += speedDelta;
		}else
		if (Input.GetKey(minus)) {
			if (speed > 0f)
				speed *= frac/2;

			speed -= speedDelta;
		}else{
			speed *= frac;
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
