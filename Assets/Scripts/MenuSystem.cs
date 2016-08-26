using UnityEngine;
using System.Collections;



public class MenuSystem : MonoBehaviour {
	Transform self; // cache reference to our own transform, 
		// so it doesn't constantly query for component per frame (under the hood)
	Transform host; // the object we attach to and move around with 
		// (keeping our own seperate rotation on 2 axes) 
	MenuMode mode;

	enum MenuMode {
		Compact,
		Main,
		Controls,
		Music,
		Sound
	}



	void Start() {
		self = transform;
		host = GameObject.Find("DevSpectate").transform;
	}
	

	void LateUpdate() {
		self.position = host.position;
		self.eulerAngles = new Vector3(-90f, host.eulerAngles.y, 0f);
	}


	public void GoToMainMenu() {
		mode = MenuMode.Main;
		// TODO: disable start/mainmenu button
		// TODO: build and enable mainmenu buttons
	}
}
