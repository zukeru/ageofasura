using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurrenciesUI : AtavismWindowTemplate {
	
	public int buttonSize = 32;
	public KeyCode toggleButton;

	// Use this for initialization
	void Start () {
		SetupRect();
	
		//EventSystem.RegisterEvent("CURRENCY_UPDATE", this);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(toggleButton)) {
			ToggleOpen();
		}
	}
	
	void OnGUI() {
		if (!open)
			return;
		
		GUI.depth = uiLayer;
		GUI.skin = skin;
		
		GUI.Box(uiRect, "");
		GUILayout.BeginArea(new Rect(uiRect));
		// Top Bar
		GUILayout.BeginHorizontal();
		GUILayout.Label("Currencies");
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("X")) {
			ToggleOpen();
		}
		GUILayout.EndHorizontal();
		
		foreach (Currency c in ClientAPI.ScriptObject.GetComponent<Inventory>().Currencies.Values) {
			GUILayout.BeginHorizontal();
			GUILayout.Button(c.icon, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
			GUILayout.Label(c.Current + " " + c.name);
			GUILayout.EndHorizontal();
			/*if (c.SubCurrency1ID > 0) {
				GUILayout.BeginHorizontal();
				GUILayout.Space(20);
				GUILayout.Button(c.subCurrency1Icon, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
				GUILayout.Label(c.SubCurrency1Count + " " + c.subCurrency1Name);
				GUILayout.EndHorizontal();
			}
			if (c.SubCurrency2ID > 0) {
				GUILayout.BeginHorizontal();
				GUILayout.Space(20);
				GUILayout.Button(c.subCurrency2Icon, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
				GUILayout.Label(c.SubCurrency2Count + " " + c.subCurrency2Name);
				GUILayout.EndHorizontal();
			}*/
		}
		GUILayout.EndArea();
	}
	
	/*public void OnEvent(EventData eData) {
		if (eData.eventType == "LOOT_UPDATE") {
			// Update 
			loot = ClientAPI.ScriptObject.GetComponent<Inventory>().Loot;
			lootTarget = ClientAPI.ScriptObject.GetComponent<Inventory>().LootTarget;
			if (loot.Count > 0 && !open)
				ToggleOpen();
			else if (loot.Count == 0 && open)
				ToggleOpen();
		}
	}*/
}
