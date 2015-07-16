using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bag {
	public int slotNum;
	public int id = 0;
	public string name = "";
    public Texture2D icon = null;
	public int numSlots = 0;
	public Dictionary<int, AtavismInventoryItem> items = new Dictionary<int, AtavismInventoryItem>();
}

public class Inventory : MonoBehaviour {
	
	static Inventory instance;
	Dictionary<string, EquipmentDisplay> equipmentDisplays;
	Dictionary<int, AtavismInventoryItem> items;
	Dictionary<int, Currency> currencies;
	public Currency mainCurrency;
	
	// These are filled by messages from the server
	Dictionary<int, Bag> bags = new Dictionary<int, Bag>();
	Dictionary<int, AtavismInventoryItem> equippedItems = new Dictionary<int, AtavismInventoryItem>();
	Dictionary<int, AtavismInventoryItem> loot = new Dictionary<int, AtavismInventoryItem>();
	OID lootTarget;
	
	void Start() {
		if (instance != null) {
			return;
		}
		instance = this;
		
		// Load in equipment displays - Not currently used
		/*equipmentDisplays = new Dictionary<string, EquipmentDisplay>();
		Object[] displayPrefabs = Resources.LoadAll("Content/EquipmentDisplay");
		foreach (Object displayPrefab in displayPrefabs) {
			GameObject go = (GameObject) displayPrefab;
			EquipmentDisplay displayData = go.GetComponent<EquipmentDisplay>();
			equipmentDisplays.Add(go.name, displayData);
		}*/

		// Load in items
		items = new Dictionary<int, AtavismInventoryItem>();
		Object[] itemPrefabs = Resources.LoadAll("Content/Items");
		foreach (Object displayPrefab in itemPrefabs) {
			GameObject go = (GameObject) displayPrefab;
			AtavismInventoryItem displayData = go.GetComponent<AtavismInventoryItem>();
			if (!items.ContainsKey(displayData.TemplateId)) {
				items.Add(displayData.TemplateId, displayData);
			}
		}

		// Load in currencies
		currencies = new Dictionary<int, Currency>();
		Object[] currencyPrefabs = Resources.LoadAll("Content/Currencies");
		foreach (Object displayPrefab in currencyPrefabs) {
			GameObject go = (GameObject) displayPrefab;
			Currency displayData = go.GetComponent<Currency>();
			if (!currencies.ContainsKey(displayData.id) && displayData.id > 0) {
				currencies.Add(displayData.id, displayData);
			}
		}
		
		// Listen for messages from the server
		NetworkAPI.RegisterExtensionMessageHandler("BagInventoryUpdate", HandleBagInventoryUpdate);
		NetworkAPI.RegisterExtensionMessageHandler("EquippedInventoryUpdate", HandleEquippedInventoryUpdate);
		NetworkAPI.RegisterExtensionMessageHandler("currencies", HandleCurrencies);
		NetworkAPI.RegisterExtensionMessageHandler("LootList", HandleLootList);
		NetworkAPI.RegisterExtensionMessageHandler("inventory_event", HandleInventoryEvent);
	}
	
	/*public EquipmentDisplay GetEquipmentDisplay(string displayID) {
		// Player does not have this ability - lets use the template
		if (equipmentDisplays.ContainsKey(displayID))
			return equipmentDisplays[displayID];
		return null;
	}*/
	
	public void HandleBagInventoryUpdate(Dictionary<string, object> props) {
		bags.Clear();
		int numBags = (int)props["numBags"];
    	for (int i = 0; i < numBags; i++) {
        	Bag bag = new Bag();
			bag.id = (int)props["bag_" + i + "ID"];
        	bag.name = (string)props["bag_" + i + "Name"];
			AtavismInventoryItem invInfo = LoadItemPrefabData(bag.name);
			bag.icon = invInfo.icon;
			AtavismLogger.LogDebugMessage("Got bag with name: " + bag.name);
        	bag.numSlots = (int)props["bag_" + i + "NumSlots"];
        	bag.slotNum = i;
        	//CSVReader.loadBagData(bag);
        	bags[i] = bag;
		}
    	int numItems = (int)props["numItems"];
    	for (int i = 0; i < numItems; i++) {
        	int bagNum = (int)props["item_" + i + "BagNum"];
        	int slotNum = (int)props["item_" + i + "SlotNum"];
        	string baseName = (string)props["item_" + i + "BaseName"];
        	AtavismInventoryItem invInfo = LoadItemPrefabData(baseName);
			AtavismLogger.LogDebugMessage("Got item: " + invInfo.BaseName);
        	//invInfo.copyData(GetGenericItemData(invInfo.baseName));
        	invInfo.Count = (int)props["item_" + i + "Count"];
        	//ClientAPI.Log("ITEM: item count for item %s is %s" % (invInfo.name, invInfo.count))
        	invInfo.ItemId = (OID)props["item_" + i + "Id"];
        	invInfo.name = (string)props["item_" + i + "Name"];
        	invInfo.EnergyCost = (int)props["item_" + i + "EnergyCost"];
        	int numResists = (int)props["item_" + i + "NumResistances"];
        	for (int j = 0; j < numResists; j++) {
            	string resistName = (string)props["item_" + i + "Resist_" + j + "Name"];
            	int resistValue = (int)props["item_" + i + "Resist_" + j + "Value"];
            	invInfo.Resistances[resistName] = resistValue;
			}
        	int numStats = (int)props["item_" + i + "NumStats"];
        	for (int j = 0; j < numStats; j++) {
            	string statName = (string)props["item_" + i + "Stat_" + j + "Name"];
            	int statValue = (int)props["item_" + i + "Stat_" + j + "Value"];
            	invInfo.Stats[statName] = statValue;
			}
        	//ClientAPI.Log("InventoryUpdateEntry fields: %s, %d, %d, %s" % (invInfo.itemId, bagNum, slotNum, invInfo.name))
        	if (invInfo.itemType == "Weapon") {
            	invInfo.DamageValue = (int)props["item_" + i + "DamageValue"];
            	invInfo.DamageType = (string) props["item_" + i + "DamageType"];
            	invInfo.WeaponSpeed = (int)props["item_" + i + "Delay"];
			}
        	bags[bagNum].items[slotNum] = invInfo;
		}
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("INVENTORY_UPDATE", args);
	}
	
	public void HandleEquippedInventoryUpdate(Dictionary<string, object> props) {
		equippedItems.Clear();
		int numSlots = (int)props["numSlots"];
		for (int i = 0; i < numSlots; i++) {
			string name = (string)props["item_" + i + "Name"];
			if (name == null || name == "")
				continue;
			string baseName = (string)props["item_" + i + "BaseName"];
			AtavismInventoryItem invInfo = LoadItemPrefabData(baseName);
			invInfo.name = name;
			invInfo.Count = (int)props["item_" + i + "Count"];
        	invInfo.ItemId = (OID)props["item_" + i + "Id"];
        	invInfo.EnergyCost = (int)props["item_" + i + "EnergyCost"];
        	int numResists = (int)props["item_" + i + "NumResistances"];
        	for (int j = 0; j < numResists; j++) {
            	string resistName = (string)props["item_" + i + "Resist_" + j + "Name"];
            	int resistValue = (int)props["item_" + i + "Resist_" + j + "Value"];
            	invInfo.Resistances[resistName] = resistValue;
			}
        	int numStats = (int)props["item_" + i + "NumStats"];
        	for (int j = 0; j < numStats; j++) {
            	string statName = (string)props["item_" + i + "Stat_" + j + "Name"];
            	int statValue = (int)props["item_" + i + "Stat_" + j + "Value"];
            	invInfo.Stats[statName] = statValue;
			}
			equippedItems.Add(i, invInfo);
			AtavismLogger.LogDebugMessage("Added equipped item: " + invInfo.name + " to slot: " + i);
		}
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("EQUIPPED_UPDATE", args);
	}

	public void HandleCurrencies(Dictionary<string, object> props) {
		int numCurrencies = (int)props["numCurrencies"];
		for (int i = 0; i < numCurrencies; i++) {
			int currencyID = (int)props["currency" + i + "ID"];
			if (currencies.ContainsKey(currencyID))
				currencies[currencyID].Current = (int)props["currency" + i + "Current"];
		}
		// dispatch a ui event to tell the rest of the system
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("CURRENCY_UPDATE", args);
	}
	
	public void HandleLootList(Dictionary<string, object> props) {
		loot.Clear();
		int numItems = (int)props["numItems"];
		AtavismLogger.LogDebugMessage("Got Loot list with num items: " + numItems);
		lootTarget = (OID)props["lootTarget"];
		for (int i = 0; i < numItems; i++) {
			string name = (string)props["item_" + i + "Name"];
			if (name == null || name == "")
				continue;
			string baseName = (string)props["item_" + i + "BaseName"];
			AtavismInventoryItem invInfo = LoadItemPrefabData(baseName);
			invInfo.name = name;
			invInfo.Count = (int)props["item_" + i + "Count"];
        	invInfo.ItemId = (OID)props["item_" + i + "Id"];
        	invInfo.EnergyCost = (int)props["item_" + i + "EnergyCost"];
        	int numResists = (int)props["item_" + i + "NumResistances"];
        	for (int j = 0; j < numResists; j++) {
            	string resistName = (string)props["item_" + i + "Resist_" + j + "Name"];
            	int resistValue = (int)props["item_" + i + "Resist_" + j + "Value"];
            	invInfo.Resistances[resistName] = resistValue;
			}
        	int numStats = (int)props["item_" + i + "NumStats"];
        	for (int j = 0; j < numStats; j++) {
            	string statName = (string)props["item_" + i + "Stat_" + j + "Name"];
            	int statValue = (int)props["item_" + i + "Stat_" + j + "Value"];
            	invInfo.Stats[statName] = statValue;
			}
			loot.Add(i, invInfo);
			AtavismLogger.LogDebugMessage("Added loot item: " + invInfo.name + " to slot: " + i);
		}
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("LOOT_UPDATE", args);
	}
	
	public void HandleInventoryEvent(Dictionary<string, object> props) {
		string eventType = (string)props["event"];
		int itemID = (int)props["itemID"];
		int count = (int)props["count"];
		string data = (string)props["data"];
		
		// dispatch a ui event to tell the rest of the system
		string[] args = new string[4];
		args[0] = eventType;
		args[1] = itemID.ToString();
		args[2] = count.ToString();
		args[3] = data;
		AtavismEventSystem.DispatchEvent("INVENTORY_EVENT", args);
	}
	
	AtavismInventoryItem LoadItemPrefabData(string itemBaseName) {
		GameObject itemPrefab = (GameObject) Resources.Load("Content/Items/Item" + itemBaseName);
		if (itemPrefab == null) {
			return gameObject.AddComponent<AtavismInventoryItem>();
		} else {
			AtavismInventoryItem itemData = itemPrefab.GetComponent<AtavismInventoryItem>();
			return itemData.Clone(gameObject);
		}
	}

	public AtavismInventoryItem GetItemByTemplateID(int itemID) {
		if (items.ContainsKey(itemID))
			return items[itemID];
		return null;
	}
	
	public AtavismInventoryItem GetInventoryItem(OID itemOID) {
		foreach (Bag bag in bags.Values) {
			foreach (AtavismInventoryItem item in bag.items.Values) {
				if (item.ItemId == itemOID) {
					return item;
				}
			}
		}
		
		return null;
	}
	
	public void DeleteItemWithName(string name) {
		AtavismInventoryItem itemToDelete = null;
		foreach (Bag bag in bags.Values) {
			foreach (AtavismInventoryItem item in bag.items.Values) {
				if (item.name == name) {
					itemToDelete = item;
					break;
				}
			}
		}
		
		if (itemToDelete != null) {
			long targetOid = ClientAPI.GetPlayerObject ().Oid;
			NetworkAPI.SendTargetedCommand(targetOid, "/deleteItem " + itemToDelete.ItemId.ToString());
		}
	}

    public void DeleteItemStack(AtavismInventoryItem item)
    {
        long targetOid = ClientAPI.GetPlayerObject().Oid;
        NetworkAPI.SendTargetedCommand(targetOid, "/deleteItemStack " + item.ItemId.ToString());
    }

	public EquipmentDisplay LoadEquipmentDisplay(string equipmentDisplayName) {
		equipmentDisplayName = equipmentDisplayName.Remove(0, 17);
		equipmentDisplayName = equipmentDisplayName.Remove(equipmentDisplayName.Length-7);
		GameObject eqPrefab = (GameObject) Resources.Load(equipmentDisplayName);
		return eqPrefab.GetComponent<EquipmentDisplay>();
	}
	
	public Bag GetBagData(int slot) {
		if (bags.ContainsKey(slot)) {
			return bags[slot];
		}
		return null;
	}
	
	public Currency GetCurrency(int currencyID) {
		if (currencies.ContainsKey(currencyID)) {
			return currencies[currencyID];
		}
		return null;
	}
	
	public string GetCurrencyName(int currencyID) {
		if (currencies.ContainsKey(currencyID)) {
			return currencies[currencyID].name;
		}
		return "Unknown Currency";
	}
	
	public Dictionary<int, Bag> Bags {
		get {
			return bags;
		}
	}
	
	public Dictionary<int, AtavismInventoryItem> EquippedItems {
		get {
			return equippedItems;
		}
	}

	public Dictionary<int, Currency> Currencies {
		get {
			return currencies;
		}
	}
	
	public Dictionary<int, AtavismInventoryItem> Loot {
		get {
			return loot;
		}
	}
	
	public OID LootTarget {
		get {
			return lootTarget;
		}
	}
}
