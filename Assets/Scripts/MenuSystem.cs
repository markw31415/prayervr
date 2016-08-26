using UnityEngine;
using System.Collections.Generic;



public class MenuSystem : MonoBehaviour {
	Transform self; // cache reference to our own transform, 
		// so it doesn't constantly query for component per frame (under the hood)
	Transform host; // the object we attach to and move around with 
		// (keeping our own seperate rotation on 2 axes) 
	MenuMode prevMode; // previous menu 
	MenuMode currMode;
	Dictionary<MenuMode, GameObject> menus = new Dictionary<MenuMode, GameObject>();

	enum MenuMode {
		Compact,
		Main,
		Controls,
		Music,
		Sound,

		MAX
	}



	void Start() {
		self = transform;
		host = GameObject.Find("DevSpectate").transform;

		for (int i = 0; i < (int)MenuMode.MAX; i++) {
			var mm = (MenuMode)i;
			var name = mm + "Menu";
			var o = GameObject.Find(name);

			if (o) {
				menus.Add(mm, o);

				if (mm != MenuMode.Compact)
					o.SetActive(false); // make all other menus inactive/invisible 
			}else{
				Debug.LogError("GameObject \"" + name + "\" does not exist!!!");
			}
		}
	}
	

	void LateUpdate() {
		self.position = host.position;
		self.eulerAngles = new Vector3(-90f, host.eulerAngles.y, 0f);
	}


	public void GoToMainMenu() {
		Debug.Log("GoToMainMenu()");
		switchTo(MenuMode.Main);
	}


	private void switchTo(MenuMode mm) {
		prevMode = currMode;
		currMode = mm;

		menus[prevMode].SetActive(false);
		menus[currMode].SetActive(true);
	}
}
