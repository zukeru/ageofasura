using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActionType {
	Ability,
	Item,
	None
}

public class AtavismAction {
	public ActionType actionType;
	public Activatable actionObject;
	public int bar;
	public int slot;
	
	public void Activate() {
		actionObject.Activate();
	}

	public void DrawTooltip(float x, float y) {
		actionObject.DrawTooltip(x, y);
	}
}

public class Actions : MonoBehaviour {
	
	static Actions instance;
	
	List<GameObject> actionBars;

	// Use this for initialization
	void Start () {
		if (instance != null) {
			return;
		}
		instance = this;
		
		// Listen for the Abilities and Inventory updates as the action bar may need to be updated to match
		AtavismEventSystem.RegisterEvent("ABILITY_UPDATE", this);
		AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
	}
	
	void OnDestroy() {
		AtavismEventSystem.UnregisterEvent("ABILITY_UPDATE", this);
		AtavismEventSystem.UnregisterEvent("INVENTORY_UPDATE", this);
	}

	void ClientReady() {
		ClientAPI.WorldManager.RegisterObjectPropertyChangeHandler("actions", ActionsPropertyHandler);
	}
	
	void OnLevelWasLoaded (int level) {
		actionBars = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ActivateAction(int bar, int slot) {
		actionBars[bar].SendMessage("ActivateAction", slot);
	}
	
	public int GetActionBarCount() {
		return actionBars.Count;
	}
	
	public GameObject GetActionBar(int position) {
		if (actionBars.Count < position) {
			return null;
		}
		return actionBars[position];
	}
	
	void UpdateActions() {
		if (ClientAPI.GetPlayerObject() == null || !ClientAPI.GetPlayerObject().PropertyExists("actions"))
			return;
			
		LinkedList<object> actions_prop = (LinkedList<object>)ClientAPI.GetPlayerObject().GetProperty("actions");
		AtavismLogger.LogDebugMessage("Got player actions property change: " + actions_prop);
		int pos = 0;
		int bar = 0;
    	foreach (string actionString in actions_prop) {
			AtavismAction action = new AtavismAction();
			if (actionString.StartsWith("a")) {
				action.actionType = ActionType.Ability;
				int abilityID = int.Parse(actionString.Substring(1));
				action.actionObject = GetComponent<Abilities>().GetAbility(abilityID);
			} else if (actionString.StartsWith("i")) {
				action.actionType = ActionType.Item;
			} else {
				action.actionType = ActionType.None;
			}
			action.slot = pos;
			if (actionBars[bar] != null)
        		actionBars[bar].SendMessage("ActionUpdate", action);
			pos++;
		}
   		// dispatch a ui event to tell the rest of the system
		string[] event_args = new string[1];
		AtavismEventSystem.DispatchEvent("ACTION_UPDATE", event_args);
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "ACTION_UPDATE")
			return;
		UpdateActions();
	}
	
	public void ActionsPropertyHandler(object sender, ObjectPropertyChangeEventArgs args) {
    	if (args.Oid != ClientAPI.GetPlayerOid())
        	return;
    	UpdateActions();
	}
	
	public void AddActionBar(ActionBar actionBar) {
		if (actionBars.Count > actionBar.id) {
		actionBars[actionBar.id] = actionBar.gameObject;
		} else {
			while (actionBars.Count < actionBar.id) {
				actionBars.Add(null);
			}
			actionBars.Add(actionBar.gameObject);
		}
		UpdateActions();
	}
	
	public List<GameObject> ActionBars
	{
		get {
			return actionBars;
		}
	}
}
