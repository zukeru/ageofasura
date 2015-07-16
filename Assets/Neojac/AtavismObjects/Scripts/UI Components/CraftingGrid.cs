using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftingGrid : AtavismWindowTemplate {
	
	public int buttonSize = 32;
	public KeyCode toggleButton;
	int rowCount = 3;
	int columnCount = 3;
	
	List<CraftingStation> stationScripts = new List<CraftingStation>();

	// Use this for initialization
	void Start () {
		rowCount = ClientAPI.ScriptObject.GetComponent<Crafting>().gridSize;
		columnCount = ClientAPI.ScriptObject.GetComponent<Crafting>().gridSize;
	
		height = (rowCount+1) * buttonSize + 120;
		width = columnCount * buttonSize + 24;
		
		SetupRect();
		
		AtavismEventSystem.RegisterEvent("CRAFTING_GRID_UPDATE", this);
		AtavismEventSystem.RegisterEvent("CRAFTING_START", this);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(toggleButton)) {
			ToggleOpen();
		}
		
		/*Vector3 playerPos = ClientAPI.GetPlayerObject().Position;
		stationScripts.Clear();
		GameObject[] stations = GameObject.FindGameObjectsWithTag("Crafting");
			
		foreach (GameObject obj in stations)
		{
			if (Vector3.Distance(playerPos, obj.transform.position) < 10 && !stationScripts.Contains(obj.GetComponent<CraftingStation>()))
			{
				stationScripts.Add(obj.GetComponent<CraftingStation>());
			}
		}
			
		ToggleOpen();*/
	}
	
	void OnGUI() {
		if (!open)
			return;
		GUI.skin = skin;
		
		GUI.Box(uiRect, "");
		GUILayout.BeginArea(new Rect(uiRect));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Crafting");
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("X")) {
			ToggleOpen();
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Recipe:");
		AtavismInventoryItem recipeItem = ClientAPI.ScriptObject.GetComponent<Crafting>().RecipeItem;
		if (recipeItem != null) {
		}
		
		List<CraftingComponent> gridItems = ClientAPI.ScriptObject.GetComponent<Crafting>().GridItems;
		for (int i = 0; i < rowCount; i++) {
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			for (int j = 0; j < columnCount; j++) {
				int itemPos = i * columnCount + j;
				if (gridItems[itemPos].item != null) {
					if (GUILayout.Button(gridItems[itemPos].item.icon, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize))) {
						AtavismCursor.Instance.SetCursorItem(gridItems[itemPos].item);
						ClientAPI.ScriptObject.GetComponent<Crafting>().SetGridItem(itemPos, null);
					}
					Vector3 mousePosition = Input.mousePosition;
					mousePosition.y = Screen.height - mousePosition.y;
					/*if (buttonRect.Contains(mousePosition)) {
						bagData.items[i].DrawTooltip(mousePosition.x, mousePosition.y);
					}*/
				} else {
					if (GUILayout.Button("", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize))) {
						if (AtavismCursor.Instance.CursorHasItem()) {
							AtavismInventoryItem item = AtavismCursor.Instance.GetCursorItem();
							ClientAPI.ScriptObject.GetComponent<Crafting>().SetGridItem(itemPos, item);
							AtavismCursor.Instance.ResetCursor();
						}
					}
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		
		GUILayout.Label("Creates:");
		AtavismInventoryItem resultItem = ClientAPI.ScriptObject.GetComponent<Crafting>().ResultItem;
		if (resultItem != null) {
			GUILayout.Button(resultItem.icon, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
			if (GUILayout.Button("Craft")) {
				ClientAPI.ScriptObject.GetComponent<Crafting>().CraftItem();
			}
		}
		
		GUILayout.EndArea();
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "CRAFTING_GRID_UPDATE") {
			// Update 
		} else if (eData.eventType == "CRAFTING_START") {
			if (!open) {
				ToggleOpen();
			}
		}
	}
	
	public new void ToggleOpen() {
		open = !open;
		if (open) {
			AtavismUiSystem.AddFrame("CraftingGrid", uiRect);
		} else {
			AtavismUiSystem.RemoveFrame("CraftingGrid", new Rect(0, 0, 0, 0));
		}
	}
}
