using UnityEngine;
using System.Collections;

public class TargetPortrait : AtavismWindowTemplate {

	public GUISkin skin2;
	public GUISkin bar;	
	
	int health;
	int maxHealth;
	string name;
	AtavismMobNode target;
	
	string health_prop = "health";
	string health_max_prop = "health-max";

	// Use this for initialization
	void Start () {
		SetupRect();
		// Register for 
		AtavismEventSystem.RegisterEvent("PLAYER_TARGET_CHANGED", this);
		AtavismEventSystem.RegisterEvent("PROPERTY_health", this);
		AtavismEventSystem.RegisterEvent("PROPERTY_health-max", this);
		
		health = 25;
		maxHealth = 50;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy () {
		AtavismEventSystem.UnregisterEvent("PLAYER_TARGET_CHANGED", this);
		AtavismEventSystem.UnregisterEvent("PROPERTY_" + health_prop, this);
		AtavismEventSystem.UnregisterEvent("PROPERTY_" + health_max_prop, this);
	}
	
	void OnGUI() {

		// Return if we have no target
		if (target == null)
			return;
			
		GUI.depth = uiLayer;
		GUI.skin = skin;
		// Name
		GUI.Label(uiRect, name);
		// Health
		int barLength = 120;
		float healthBarLength = ((float)health / (float)maxHealth) * barLength;
		float adjustment = 5;

		GUI.skin = skin2;
		GUI.Box (new Rect (uiRect.x, uiRect.y + 20, barLength + 15, 15), "");
		
		GUI.skin = bar;
		GUI.color = Color.red;
		GUI.Box (new Rect (uiRect.x + 9, uiRect.y + 23, healthBarLength, 8), "");
		GUI.color = Color.white;
		GUI.Label(new Rect(uiRect.x, uiRect.y + 15, barLength, 25), health + "/" + maxHealth);
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "PLAYER_TARGET_CHANGED") {
			target = ClientAPI.GetTargetObject();
			if (target != null) {
				name = target.Name;
			} else {
				name = "";
				return;
			}
			AtavismLogger.LogDebugMessage("Target is: " + name);
			// Try get other properties
			if (ClientAPI.GetTargetObject().PropertyExists("health")) {
				health = (int)ClientAPI.GetTargetObject().GetProperty("health");
			}
			if (ClientAPI.GetTargetObject().PropertyExists("health-max")) {
				maxHealth = (int)ClientAPI.GetTargetObject().GetProperty("health-max");
			}
		} else if (eData.eventType == "PROPERTY_health") {
			if (eData.eventArgs[0] == "target") {
				health = (int)ClientAPI.GetTargetObject().GetProperty("health");
			}
			//Debug.Log("Got health property: " + eData.eventArgs.Length + " with unit: " + eData.eventArgs[0]);
		} else if (eData.eventType == "PROPERTY_health-max") {
			if (eData.eventArgs[0] == "target") {
				maxHealth = (int)ClientAPI.GetTargetObject().GetProperty("health-max");
			}
		}
	}
}
