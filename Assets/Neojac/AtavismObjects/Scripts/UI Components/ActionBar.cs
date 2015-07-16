using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionBar : AtavismWindowTemplate {
	
	public int id;
	public int buttonCount;
	public List<KeyCode> actionButtonBindings;
	AtavismAction[] actions;
	
	// Use this for initialization
	void Start () {
		width = buttonCount * 32 + 4;
		SetupRect();
		ToggleOpen();
		
		actions = new AtavismAction[buttonCount];

		AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
		ClientAPI.ScriptObject.GetComponent<Actions>().AddActionBar(this);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < actions.Length; i++) {
			if (actions[i] != null && actions[i].actionObject != null) {
				if (Input.GetKeyDown(actionButtonBindings[i])) {
					actions[i].Activate();
				}
			}
		}
	}

	void OnGUI() {
		GUI.depth = uiLayer;
		GUI.Box(uiRect, "");
		
		Activatable tooltipObject = null;
		for (int i = 0; i < actions.Length; i++) {
			if (actions[i] != null && actions[i].actionObject != null) {
				Rect buttonRect = new Rect(uiRect.x + i*32 + 2, uiRect.y + 2, 32, 32);
				if (GUI.Button(buttonRect, actions[i].actionObject.icon)) {
					actions[i].Activate();
				}
				Vector3 mousePosition = Input.mousePosition;
				mousePosition.y = Screen.height - mousePosition.y;
				if (buttonRect.Contains(mousePosition)) {
					tooltipObject = actions[i].actionObject;
				}
			}
		}
		
		Tooltips.Instance.SetTooltip(tooltipObject, frameName);
	}
	
	public void ActionUpdate(AtavismAction action) {
		if (action.slot >= actions.Length)
			return;
		actions[action.slot] = action;
		//actionButtons[action.slot].SendMessage("ActionUpdate", action);
	}
	
	public void ActivateAction(int slot) {
		//playerActions[slot].actionObject.Activate();
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "INVENTORY_UPDATE") {
			
		}
	}
}
