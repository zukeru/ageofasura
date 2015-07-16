using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Loot : MonoBehaviour {
	
	public GUISkin skin;
	public int buttonSize = 32;
	Dictionary<int, AtavismInventoryItem> loot = new Dictionary<int, AtavismInventoryItem>();
	OID lootTarget;
	float locX;
	float locY;
	
	bool open = false;

	// Use this for initialization
	void Start () {
		AtavismEventSystem.RegisterEvent("LOOT_UPDATE", this);
		loot = ClientAPI.ScriptObject.GetComponent<Inventory>().Loot;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		if (!open)
			return;
		
		GUI.skin = skin;
		int columns = 4;
		int height = (((loot.Count-1) / columns) + 1) * buttonSize + 24;
		int width = columns * buttonSize + 6;
		Rect bagRect = new Rect(locX, locY, width, height);
		GUI.Box(bagRect, "");
		GUI.Label(new Rect(bagRect.x + 2, bagRect.y + 2, width - 22, 20), "Loot");
		if (GUI.Button(new Rect(bagRect.xMax - 22, bagRect.y + 2, 20, 20), "x")) {
			ToggleOpen();
		}
		for (int i = 0; i < loot.Count; i++) {
			int row = i / columns;
			int column = i % columns;
			Rect buttonRect = new Rect(bagRect.x + column * buttonSize + 3, bagRect.y + row * buttonSize + 22, buttonSize, buttonSize);
			AtavismInventoryItem item = loot[i];
			if (item != null) {
				if (GUI.Button(buttonRect, item.icon)) {
       				NetworkAPI.SendTargetedCommand(lootTarget.ToLong(), "/lootItem " + item.ItemId);
				}
				GUI.Label(new Rect(buttonRect.xMax-10, buttonRect.yMax-20, 10, 20), "" + item.Count);
				// Check if mouse is over the button
				Vector3 mousePosition = Input.mousePosition;
				mousePosition.y = Screen.height - mousePosition.y;
				if (buttonRect.Contains(mousePosition)) {
					item.DrawTooltip(mousePosition.x, mousePosition.y);
				}
			}
		}
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "LOOT_UPDATE") {
			// Update 
			loot = ClientAPI.ScriptObject.GetComponent<Inventory>().Loot;
			lootTarget = ClientAPI.ScriptObject.GetComponent<Inventory>().LootTarget;
			if (loot.Count > 0 && !open)
				ToggleOpen();
			else if (loot.Count == 0 && open)
				ToggleOpen();
		}
	}
	
	public void ToggleOpen() {
		open = !open;
		if (open) {
			locX = Input.mousePosition.x;
			locY = Screen.height - Input.mousePosition.y;
			int columns = 4;
			int height = (((loot.Count-1) / columns) + 1) * buttonSize + 20;
			int width = columns * buttonSize + 2;
			Rect bagRect = new Rect(locX, locY, width, height);
			AtavismUiSystem.AddFrame("Loot", bagRect);
		} else {
			AtavismUiSystem.RemoveFrame("Loot", new Rect(0, 0, 0, 0));
		}
	}
}
