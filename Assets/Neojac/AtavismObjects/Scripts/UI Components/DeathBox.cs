using UnityEngine;
using System.Collections;

public class DeathBox : AtavismWindowTemplate {
	
	bool dead = false;

	// Use this for initialization
	void Start () {
		SetupRect();
		// Register for 
		//EventSystem.RegisterEvent("PROPERTY_deadstate", this);
		ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler ("deadstate", HandleDeadState);
		
		// The player may have changed scenes, but their stats were not sent back down, so let's take a look
		if (ClientAPI.GetPlayerObject() != null) {
			if (ClientAPI.GetPlayerObject().PropertyExists("deadstate")) {
				dead = (bool)ClientAPI.GetPlayerObject().GetProperty("deadstate");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy () {
		//EventSystem.UnregisterEvent("PROPERTY_deadstate", this);
		if (ClientAPI.GetPlayerObject() != null && ClientAPI.GetPlayerObject().GameObject != null) {
			ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler("deadstate", HandleDeadState);
		}
	}
	
	void OnGUI() {
		// Name
		if (dead) {
			GUI.depth = uiLayer;
			GUI.skin = skin;
			GUI.Box(uiRect, "");
			GUI.Label(new Rect(uiRect.x + 20, uiRect.y + 5, 110, 20), "You have died");
			if (GUI.Button(new Rect(uiRect.x + 20, uiRect.yMax - 25, 80, 20), "Release")) {
				long targetOid = ClientAPI.GetPlayerObject ().Oid;
				NetworkAPI.SendTargetedCommand (targetOid, "/release");
			}
		}
	}
	
	public void HandleDeadState (object sender, PropertyChangeEventArgs args)
	{
		dead = (bool)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty("deadstate");
	}
	
	/*public void OnEvent(EventData eData) {
		if (eData.eventType == "PROPERTY_deadstate") {
			if (eData.eventArgs[0] == "player") {
				dead = (bool)ClientAPI.GetPlayerObject().GetProperty("deadstate");
			}
		}
	}*/
}

