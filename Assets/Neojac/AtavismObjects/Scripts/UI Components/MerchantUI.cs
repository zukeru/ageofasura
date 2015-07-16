using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MerchantUI : AtavismWindowTemplate {

	enum MerchantState {
		Standard,
		Purchase,
		Sell
	}
	
	public int columnsPerPage = 2;
	public int rowsPerPage = 6;
	public int buttonSize = 32;
	public int columnWidth = 100;
	public KeyCode toggleButton;
	public int sellFactor = 4;
	int currentPage = 0;
	
	MerchantState mState = MerchantState.Standard;
	MerchantItem selectedItem;
	AtavismInventoryItem itemBeingSold;
	Rect confirmBoxRect;

	// Use this for initialization
	void Start () {
		height = rowsPerPage * buttonSize + 44;
		width = columnsPerPage * columnWidth + 6;
		
		SetupRect();
		
		// Register for 
		AtavismEventSystem.RegisterEvent("MERCHANT_UPDATE", this);
		AtavismEventSystem.RegisterEvent("SELL_ITEM", this);
	}
	
	void OnDestroy () {
		AtavismEventSystem.UnregisterEvent("MERCHANT_UPDATE", this);
		AtavismEventSystem.UnregisterEvent("SELL_ITEM", this);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(toggleButton)) {
			ToggleOpen();
		}
		if (open) {
			OID npcOID = ClientAPI.ScriptObject.GetComponent<NpcInteraction>().NpcId;
			if (npcOID != null) {
				if (Vector3.Distance(ClientAPI.GetPlayerObject().Position, ClientAPI.GetObjectNode(npcOID.ToLong()).Position) > 5) {
					ToggleOpen();
				}
			}
		}
	}
	
	void OnGUI() {
		if (!open)
			return; 
		
		GUI.depth = uiLayer;
		GUI.skin = skin;
		
		GUI.Box(uiRect, "");
		OID npcOID = ClientAPI.ScriptObject.GetComponent<NpcInteraction>().NpcId;
		GUI.Label(new Rect(uiRect.x + 2, uiRect.y + 2, width - 22, 20), ClientAPI.GetObjectNode(npcOID.ToLong()).Name);
		if (GUI.Button(new Rect(uiRect.xMax - 22, uiRect.y + 2, 20, 20), "x")) {
			ToggleOpen();
		}
		bool mouseOverItem = false;
		int startingItem = currentPage * rowsPerPage * columnsPerPage;
		for (int i = 0; i < columnsPerPage; i++) {
			float x = uiRect.x + 3 + (i * columnWidth);
			int columnStartItem = startingItem + (rowsPerPage * i);
			for (int j = 0; j < rowsPerPage; j++) {
				float y = uiRect.y + 20 + j * buttonSize;
				//int row = i / columns;
				//int column = i % columns;
				Rect buttonRect = new Rect(x, y, buttonSize, buttonSize);
				MerchantItem mItem = ClientAPI.ScriptObject.GetComponent<NpcInteraction>().GetMerchantItem(columnStartItem + j);
				if (mItem == null)
					continue;
				AtavismInventoryItem item = ClientAPI.ScriptObject.GetComponent<Inventory>().GetItemByTemplateID(mItem.itemID);
				if (item != null) {
					if (GUI.Button(buttonRect, item.icon)) {
						ShowPurchaseWindow(mItem);
					}
					if (mItem.count != -1)
						GUI.Label(new Rect(buttonRect.xMax-10, buttonRect.yMax-20, 10, 20), "" + mItem.count);
					Currency c = ClientAPI.ScriptObject.GetComponent<Inventory>().GetCurrency(mItem.purchaseCurrency);
					if (c != null)
						GUI.Label(new Rect(buttonRect.xMax+5, buttonRect.yMax - 30, columnWidth-buttonSize, buttonSize), mItem.cost + " " + c.name);
				}
				// Check if mouse is over the button
				Vector3 mousePosition = Input.mousePosition;
				mousePosition.y = Screen.height - mousePosition.y;
				if (buttonRect.Contains(mousePosition)) {
					mouseOverItem = true;
					AtavismCursor.Instance.MerchantItemEntered();
					//DrawItemTooltip(slots[i], item,mousePosition.x, mousePosition.y);
					item.DrawTooltip(mousePosition.x, mousePosition.y);
				}
			}
		}
		
		// Buttons to move between pages
		if (currentPage != 0) {
			if (GUI.Button(new Rect(uiRect.x + 3, uiRect.yMax - 23, 20, 20),">")) {
				currentPage--;
			}
		}
		int maxItemCount = rowsPerPage * columnsPerPage * (currentPage+1);
		if (ClientAPI.ScriptObject.GetComponent<NpcInteraction>().MerchantItems.Count > maxItemCount) {
			if (GUI.Button(new Rect(uiRect.xMax - 23, uiRect.yMax - 23, 20, 20),">")) {
				currentPage++;
			}
		}
		
		if (!mouseOverItem) {	
			AtavismCursor.Instance.MerchantItemLeft();
		}
		
		// Show main currency
		if (ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency != null) {
			GUI.Button(new Rect(uiRect.x + 30, uiRect.yMax - 20, 16, 16), ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.icon);
			GUI.Label(new Rect(uiRect.x + 50, uiRect.yMax - 22, 100, 20), "" + ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.Current 
			          + " " + ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.name);
		}
		
		if (mState == MerchantState.Purchase) {
			GUI.Box(confirmBoxRect, "");
			AtavismInventoryItem item = ClientAPI.ScriptObject.GetComponent<Inventory>().GetItemByTemplateID(selectedItem.itemID);
			string currencyName = ClientAPI.ScriptObject.GetComponent<Inventory>().GetCurrencyName(selectedItem.purchaseCurrency);
			GUI.Label(new Rect(confirmBoxRect.x + 5, confirmBoxRect.y + 5, confirmBoxRect.width-10, 60), 
			          "Do you want to purchase " + item.name + " for " + selectedItem.cost + " " + currencyName);
			if (GUI.Button(new Rect(confirmBoxRect.x + 5, confirmBoxRect.yMax - 25, 35, 20), "Yes")) {
				NetworkAPI.SendTargetedCommand(npcOID.ToLong(), "/purchaseItem " + selectedItem.itemID + " " + 1);
				HideMerchantDialog();
			} else if (GUI.Button(new Rect(confirmBoxRect.xMax - 35, confirmBoxRect.yMax - 25, 30, 20), "No")) {
				HideMerchantDialog();
			}
		} else if (mState == MerchantState.Sell) {
			GUI.Box(confirmBoxRect, "");
			string currencyName = ClientAPI.ScriptObject.GetComponent<Inventory>().GetCurrencyName(itemBeingSold.CurrencyType);
			GUI.Label(new Rect(confirmBoxRect.x + 5, confirmBoxRect.y + 5, confirmBoxRect.width-10, 60), 
			          "Do you want to sell " + itemBeingSold.name + " for " + (itemBeingSold.Cost * itemBeingSold.Count / sellFactor) + " " + currencyName);
			if (GUI.Button(new Rect(confirmBoxRect.x + 5, confirmBoxRect.yMax - 25, 35, 20), "Yes")) {
				Dictionary<string, object> props = new Dictionary<string, object> ();
				props.Add ("sellType", "");
				props.Add ("itemOid", itemBeingSold.ItemId);
				props.Add ("count", 1);
				NetworkAPI.SendExtensionMessage (ClientAPI.GetPlayerOid(), false, "inventory.SELL_ITEM", props);
				HideMerchantDialog();
			} else if (GUI.Button(new Rect(confirmBoxRect.xMax - 35, confirmBoxRect.yMax - 25, 30, 20), "No")) {
				HideMerchantDialog();
			}
		}
	}
	
	void ShowPurchaseWindow(MerchantItem mItem) {
		confirmBoxRect = new Rect((Screen.width - 200) / 2, (Screen.height - 75) / 2, 150, 100);
		mState = MerchantState.Purchase;
		selectedItem = mItem;
		AtavismUiSystem.AddFrame("MerchantDialog", uiRect);
	}
	
	void ShowSellWindow(AtavismInventoryItem item) {
		confirmBoxRect = new Rect((Screen.width - 200) / 2, (Screen.height - 75) / 2, 150, 100);
		mState = MerchantState.Sell;
		itemBeingSold = item;
		AtavismUiSystem.AddFrame("MerchantDialog", uiRect);
	}
	
	void HideMerchantDialog() {
		mState = MerchantState.Standard;
		AtavismUiSystem.RemoveFrame("MerchantDialog", new Rect(0, 0, 0, 0));
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "MERCHANT_UPDATE") {
			if (!open)
				ToggleOpen();
		} else if (eData.eventType == "SELL_ITEM") {
			if (!open)
				ToggleOpen();
			OID itemOID = OID.fromString(eData.eventArgs[0]);
			AtavismInventoryItem item = ClientAPI.ScriptObject.GetComponent<Inventory>().GetInventoryItem(itemOID);
			ShowSellWindow(item);
		}
	}
	
	public void ToggleOpen() {
		open = !open;
		if (open) {
			AtavismUiSystem.AddFrame(frameName, uiRect);
			// Also open the main bag
			gameObject.GetComponent<BagBar>().OpenBag(0);
			AtavismCursor.Instance.ChangeMerchantState(true);
		} else {
			AtavismUiSystem.RemoveFrame(frameName, new Rect(0, 0, 0, 0));
			AtavismUiSystem.RemoveFrame("MerchantDialog", new Rect(0, 0, 0, 0));
			AtavismCursor.Instance.ChangeMerchantState(false);
		}
	}
}
