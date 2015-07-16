using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : AtavismWindowTemplate {
	
	public List<string> slots = new List<string>();
	public int buttonSize = 32;
	Dictionary<int, AtavismInventoryItem> equippedItems = new Dictionary<int, AtavismInventoryItem>();
	public KeyCode toggleButton;

	// Use this for initialization
	void Start () {
		int columns = 2;
		height = (((slots.Count-1) / columns) + 1) * buttonSize + 24;
		width = 100 + columns * buttonSize + 6;
		
		SetupRect();
	
		AtavismEventSystem.RegisterEvent("EQUIPPED_UPDATE", this);
		equippedItems = ClientAPI.ScriptObject.GetComponent<Inventory>().EquippedItems;
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
		GUI.Label(new Rect(uiRect.x + 2, uiRect.y + 2, width - 22, 20), ClientAPI.GetPlayerObject().Name);
		if (GUI.Button(new Rect(uiRect.xMax - 22, uiRect.y + 2, 20, 20), "x")) {
			open = false;
		}
		for (int i = 0; i < slots.Count; i++) {
			float x = uiRect.x + 3;
			float y = uiRect.y + i * buttonSize + 22;
			if (i >= slots.Count / 2) {
				x = uiRect.xMax - buttonSize - 3;
				y = uiRect.y + (i - (slots.Count/2)) * buttonSize + 22;
			}
			//int row = i / columns;
			//int column = i % columns;
			Rect buttonRect = new Rect(x, y, buttonSize, buttonSize);
			AtavismInventoryItem item = GetItemInSlot(slots[i]);
			if (item != null) {
				if (GUI.Button(buttonRect, item.icon)) {
					if (Event.current.button == AtavismCursor.Instance.PickupButton)
						AtavismCursor.Instance.PickupOrPlaceEquippedItem(i, item);
					else if (Event.current.button == AtavismCursor.Instance.ActivateButton)
						item.Activate();
				}
				GUI.Label(new Rect(buttonRect.xMax-10, buttonRect.yMax-20, 10, 20), "" + item.Count);
			} else {
				// No item, but need a button for players to place items on
				if (GUI.Button(buttonRect, ""/*, GUIStyle.none*/)) {
					AtavismCursor.Instance.PickupOrPlaceEquippedItem(i, null);
				}
			}
			// Check if mouse is over the button
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.y = Screen.height - mousePosition.y;
			if (buttonRect.Contains(mousePosition)) {
				DrawItemTooltip(slots[i], item,mousePosition.x, mousePosition.y);
			}
		}
	}
	
	void DrawItemTooltip(string slot, AtavismInventoryItem item, float x, float y) {
		if (item != null) {
			item.DrawTooltip(x, y);
		} else {
			Rect tooltipRect = new Rect(x, y-30, 90, 30);
			GUI.Box(tooltipRect, "");
			GUI.Label(new Rect(tooltipRect.x + 5, tooltipRect.y + 5, 80, 20), slot);
		}
	}
	
	AtavismInventoryItem GetItemInSlot(string slotName) {
		foreach (AtavismInventoryItem item in equippedItems.Values) {
			if (item.slot == slotName)
				return item;
		}
		return null;
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "EQUIPPED_UPDATE") {
			// Update equipped items dictionary
			equippedItems = ClientAPI.ScriptObject.GetComponent<Inventory>().EquippedItems;
		}
	}
}
