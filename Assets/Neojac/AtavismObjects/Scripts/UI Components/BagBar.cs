using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BagBar : AtavismWindowTemplate {
	
	public int numSlots = 4;
	public int buttonSize = 32;
	public int bagColumnCount = 4;
	public int maxHeight = 200;
	public Texture2D backpackIcon;
	public KeyCode toggleButton;
	//Dictionary<int, GameObject> bagObjects = new Dictionary<int, GameObject>();
	Dictionary<int, Rect> bagRects = new Dictionary<int, Rect>();
	bool[] bagOpen;
	bool justifyLeft = true;

	// Use this for initialization
	void Start () {
		bagOpen = new bool[numSlots];
		width = numSlots * buttonSize + 4;
		SetupRect();
		ToggleOpen();
		if (anchor == AnchorPoint.Right || anchor == AnchorPoint.BottomRight || anchor == AnchorPoint.TopRight) {
			justifyLeft = false;
		}
		
		AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
		Dictionary<int, Bag> bags = ClientAPI.ScriptObject.GetComponent<Inventory>().Bags;
		ProcessBagInventoryChange(bags);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(toggleButton)) {
			ToggleBag(0);
		}
	}
	
	void OnGUI() {
		GUI.depth = uiLayer;
		GUI.skin = skin;
		int width = buttonSize * numSlots + 4;
		int height = buttonSize + 4;
		GUI.Box(uiRect, "");
		for (int i = 0; i < numSlots; i++) {
			Rect bagButtonRect = new Rect(uiRect.x + i * buttonSize + 2, uiRect.y + 2, buttonSize, buttonSize);
			if (!justifyLeft) {
				bagButtonRect = new Rect(uiRect.x + uiRect.width - (i+1) * buttonSize - 2, uiRect.y + 2, buttonSize, buttonSize);
			}
			Bag bagData = ClientAPI.ScriptObject.GetComponent<Inventory>().GetBagData(i);
			if (i == 0) {
				if (GUI.Button(bagButtonRect, backpackIcon)) {
					if (Event.current.button == 0) {
						ToggleBag(i);
					}
				}
			} else if (bagData != null && bagData.numSlots > 0) {
				if (GUI.Button(bagButtonRect, bagData.icon)) {
					//bagObjects[i].GetComponent<BagUI>().ToggleOpen();
					if (Event.current.button == 0) {
						ToggleBag(i);
					} else if (Event.current.button == 1) {
						AtavismCursor.Instance.SetBag(bagData);
					}
				}
			} else {
				if (GUI.Button(bagButtonRect, "", GUIStyle.none)) {
					if (AtavismCursor.Instance.CursorHasItem()) {
						AtavismCursor.Instance.PlaceCursorItemAsBag(i);
					} else if (AtavismCursor.Instance.CursorHasBag()) {
						AtavismCursor.Instance.PlaceCursorBag(i);
					}
				}
			}
			if (bagOpen[i]) {
				DrawBag(i);
			}
		}
		
	}
	
	void DrawBag(int slot) {
		Activatable tooltipObject = null;
		Bag bagData = ClientAPI.ScriptObject.GetComponent<Inventory>().GetBagData(slot);
		Rect bagRect = bagRects[slot];
		GUI.Box(bagRect, "");
		GUI.Label(new Rect(bagRect.x + 2, bagRect.y + 2, width - 22, 20), bagData.name);
		if (GUI.Button(new Rect(bagRect.xMax - 22, bagRect.y + 2, 20, 20), "x")) {
			CloseBag(slot);
		}
		for (int i = 0; i < bagData.numSlots; i++) {
			int row = i / bagColumnCount;
			int column = i % bagColumnCount;
			Rect buttonRect = new Rect(bagRect.x + column * buttonSize + 3, bagRect.y + row * buttonSize + 22, buttonSize, buttonSize);
			if (bagData.items.ContainsKey(i)) {
				if (GUI.Button(buttonRect, bagData.items[i].icon)) {
					if (Event.current.button == AtavismCursor.Instance.PickupButton) {
						if (bagData.items[i].Count > 0)
							AtavismCursor.Instance.PickupOrPlaceBagItem(slot, i, bagData.items[i]);
					} else if (Event.current.button == AtavismCursor.Instance.ActivateButton) {
						// Check for interface overrides
						if (AtavismCursor.Instance.InterfaceStateOverriden()) {
							AtavismCursor.Instance.HandleItemUseOverride(bagData.items[i]);
						} else {
							bagData.items[i].Activate();
						}
					}
				}
				if (bagData.items[i].Count > 9) {
					GUI.Label(new Rect(buttonRect.xMax-20, buttonRect.yMax-20, 20, 20), "" + bagData.items[i].Count);
				} else {
					GUI.Label(new Rect(buttonRect.xMax-10, buttonRect.yMax-20, 10, 20), "" + bagData.items[i].Count);
				}
				// Check if mouse is over the button
				Vector3 mousePosition = Input.mousePosition;
				mousePosition.y = Screen.height - mousePosition.y;
				if (buttonRect.Contains(mousePosition)) {
					GUI.depth = 1;
					tooltipObject = bagData.items[i];
					GUI.depth = uiLayer;
				}
			} else {
				// No item, but need a button for players to place items on
				if (GUI.Button(buttonRect, ""/*, GUIStyle.none*/)) {
					AtavismCursor.Instance.PickupOrPlaceBagItem(slot, i, null);
				}
			}
		}
		
		if (slot == 0 && ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency != null) {
			GUI.Button(new Rect(bagRect.x + 10, bagRect.yMax - 20, 16, 16), ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.icon);
			GUI.Label(new Rect(bagRect.x + 30, bagRect.yMax - 22, 100, 20), "" + ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.Current
			          + " " + ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.name);
		}
		
		Tooltips.Instance.SetTooltip(tooltipObject, "Bag" + slot);
	}
	
	void ToggleBag(int slot) {
		if (bagOpen[slot] == false) {
			OpenBag(slot);
		} else {
			CloseBag(slot);
		}
	}
	
	public void OpenBag(int slot) {
		if (bagOpen[slot] == true)
			return;
		Bag bagData = ClientAPI.ScriptObject.GetComponent<Inventory>().GetBagData(slot);
		int numRows = Mathf.CeilToInt((float)bagData.numSlots / (float)bagColumnCount);
		int bagHeight = (numRows) * buttonSize + 24;
		int bagWidth = bagColumnCount * buttonSize + 6;
		if (slot == 0) {
			// Add space for the currency at the bottom
			bagHeight += 20;
		}
		
		float xOffset = 0;
		float yOffset = height + 10;
		// Work out where the bag should be based on it's number
		for (int i = 0; i < slot; i++) {
			if (!bagRects.ContainsKey(i)) {
				continue;
			}
			yOffset += bagRects[i].height + 10;
			if (yOffset + height > maxHeight) {
				// Go to next column
				yOffset = height + 10;
				xOffset += bagRects[i].width + 10;
			}
		}
		Rect bagRect = SetupRect(anchor, this.anchorOffset.x + xOffset, this.anchorOffset.y + yOffset, bagWidth, bagHeight);
		bagRects.Add(slot, bagRect);
		AtavismUiSystem.AddFrame("Bag" + slot, bagRect);
		bagOpen[slot] = true;
		
		// Any bags with a higher slot number than this one will need closed and re-opened
		if (bagRects.ContainsKey(slot+1)) {
			CloseBag (slot+1);
			OpenBag(slot+1);
		}
	}
	
	void CloseBag(int slot) {
		bagRects.Remove(slot);
		AtavismUiSystem.RemoveFrame("Bag" + slot, new Rect(0, 0, 0, 0));
		bagOpen[slot] = false;
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "INVENTORY_UPDATE") {
			// Update 
			Dictionary<int, Bag> bags = ClientAPI.ScriptObject.GetComponent<Inventory>().Bags;
			ProcessBagInventoryChange(bags);
		}
	}
	
	void ProcessBagInventoryChange(Dictionary<int, Bag> bags) {
		for (int i = 0; i < numSlots; i++) {
			if (bags.ContainsKey(i)) {
				if (bagOpen[i] == true && bags[i].numSlots == 0) {
					CloseBag(i);
				}
				/*if (bagObjects.ContainsKey(i) && bags[i].name != bagObjects[i].name) {
					// Clear out the old bag, we have a new bag in this slot
					//Destroy(bagObjects[i]);
					//bagObjects.Remove(i);
					CloseBag(i);
				}*/
				/*if (!bagObjects.ContainsKey(i)) {
					Debug.Log("Going to create an instance of bag: " + bags[i].name);
					GameObject bagTemplate = (GameObject) Resources.Load("Content/Bags/" + bags[i].name);
					GameObject bagObject = (GameObject) Instantiate(bagTemplate);
					bagObject.name = bags[i].name;
					bagObject.GetComponent<BagUI>().UiObject = gameObject;
					bagObject.transform.parent = gameObject.transform;
					bagObjects.Add(i, bagObject);
				}
				bagObjects[i].GetComponent<BagUI>().BagData = bags[i];*/
			}
		}
	}
}
