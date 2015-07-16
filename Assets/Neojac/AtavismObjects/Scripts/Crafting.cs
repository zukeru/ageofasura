using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftingComponent {
	public AtavismInventoryItem item = null;
    public int count = 1;
}

public class Blueprint {
	public int recipeID;
	public int resultID;
	public int recipeItemID;
	public string station;
	public List<List<CraftingComponent>> slots;
}

public class ResourceItem {
	public AtavismInventoryItem item;
	public int count;
}

public class Crafting : MonoBehaviour {

	public int gridSize = 3;
	CraftingStationType stationType = CraftingStationType.None;
	GameObject station;
	
	List<CraftingComponent> gridItems = new List<CraftingComponent>();
	AtavismInventoryItem dye = null;
	AtavismInventoryItem essence = null;
	int recipeID = -1;
	string recipeName = "";
	int recipeItemID = -1;
	int resultItemID = -1;
	AtavismInventoryItem recipeItem = null;
	AtavismInventoryItem resultItem = null;
	List<Blueprint> blueprints = new List<Blueprint>();
	
	List<ResourceItem> resourceLoot;
	Dictionary<int, ResourceNode> resourceNodes = new Dictionary<int, ResourceNode>();
	int currentResourceNode = -1;
	
	void Start() {
		int gridCount = gridSize * gridSize;
		for (int i = 0; i < gridCount; i++) {
			gridItems.Add(new CraftingComponent());
		}
	
		// Listen for messages from the server
		NetworkAPI.RegisterExtensionMessageHandler("CraftingGridMsg", HandleCraftingGridResponse);
		NetworkAPI.RegisterExtensionMessageHandler("CraftingMsg", HandleCraftingMessage);
		NetworkAPI.RegisterExtensionMessageHandler("BlueprintMsg", HandleBlueprintMessage);
		NetworkAPI.RegisterExtensionMessageHandler("resource_drops", HandleResourceDropsMessage);
		NetworkAPI.RegisterExtensionMessageHandler("resource_state", HandleResourceStateMessage);

		// Listen for inventory changes
		AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
	}

	void Update() {
		if (station != null) {
			if (Vector3.Distance(ClientAPI.GetPlayerObject().Position, station.transform.position) > 5) {
				ClearGrid();
				station.SendMessage("CloseStation");
				station = null;
			}
		}
	}
	
	void ClientReady() {
		ClientAPI.WorldManager.RegisterObjectPropertyChangeHandler("recipes", RecipesPropertyHandler);
	}
	
	public void RegisterResourceNode(ResourceNode resourceNode) {
		resourceNodes[resourceNode.id] = resourceNode;
	}
	
	public void RecipesPropertyHandler(object sender, ObjectPropertyChangeEventArgs args) {
		if (args.Oid != ClientAPI.GetPlayerOid())
			return;
		LinkedList<object> recipes_prop = (LinkedList<object>)ClientAPI.GetPlayerObject().GetProperty("recipes");
		Debug.Log("Got player recipes property change: " + recipes_prop);
		LinkedList<string> recipeIDs = new LinkedList<string>();
		int numRecipes = 0;
		Dictionary<string, object> props = new Dictionary<string, object> ();
		foreach (string recipeString in recipes_prop) {
			// Get items
			bool haveBlueprint = false;
			int recipeID = int.Parse(recipeString);
			foreach(Blueprint bp in blueprints) {
				if (bp.recipeID == recipeID)
					haveBlueprint = true;
			}
			if (!haveBlueprint) {
				props.Add ("recipe"+ numRecipes, recipeID);
				numRecipes++;
			}
		}
		
		props.Add ("numRecipes", numRecipes);
		NetworkAPI.SendExtensionMessage (ClientAPI.GetPlayerOid(), false, "crafting.GET_BLUEPRINTS", props);
	}

	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "INVENTORY_UPDATE") {
			// The inventory has updated, we need to see if it effects the crafting UI
			int gridCount = gridSize * gridSize;
			for (int i = 0; i < gridCount; i++) {
				if (gridItems[i].item != null) {
					AtavismInventoryItem item = gridItems[i].item;
					gridItems[i].item = ClientAPI.ScriptObject.GetComponent<Inventory>().GetInventoryItem(item.ItemId);
					gridItems[i].item.ResetUseCount();
				}
			}
			for (int i = 0; i < gridCount; i++) {
				if (gridItems[i].item != null) {
					gridItems[i].item.AlterUseCount(gridItems[i].count);
				}
			}
		}
		//string[] args = new string[1];
		//EventSystem.DispatchEvent("CRAFTING_GRID_UPDATE", args);
	}

	public void SetGridItem(int gridPos, AtavismInventoryItem item) {
		if (item == null) {
			if (gridItems[gridPos].item != null)
				gridItems[gridPos].item.AlterUseCount(-gridItems[gridPos].count);
			gridItems[gridPos].item = null;
			gridItems[gridPos].count = 1;
		} else if (gridItems[gridPos].item == item) {
			gridItems[gridPos].count++;
		} else {
			gridItems[gridPos].item = item;
			gridItems[gridPos].count = 1;
		}

		if (item != null)
			item.AlterUseCount(1);
		
		SendGridUpdated();
	}
	
	public void SetRecipeItem(AtavismInventoryItem item) {
		recipeItem = item;
		SendGridUpdated();
	}
	
	public void SetDye(AtavismInventoryItem item) {
		dye = item;
		SendGridUpdated();
	}
	
	public void SetEssence(AtavismInventoryItem item) {
		essence = item;
		SendGridUpdated();
	}

	void SendGridUpdated() {
		// Send message to server to work out if we have a valid recipe
		Dictionary<string, object> props = new Dictionary<string, object> ();
		LinkedList<object> itemIds = new LinkedList<object>();
		LinkedList<object> itemCounts = new LinkedList<object>();
		for (int i = 0; i < gridItems.Count; i++) {
			if (gridItems[i].item != null) {
				itemIds.AddLast(gridItems[i].item.templateId);
			} else {
				itemIds.AddLast(-1);
			}
			itemCounts.AddLast(gridItems[i].count);
		}
		props.Add ("gridSize", gridSize);
		props.Add ("componentIDs", itemIds);
		props.Add ("componentCounts", itemCounts);
		props.Add ("stationType", stationType.ToString());
		if (recipeItem != null) {
			props.Add("recipeItemID", recipeItem.templateId);
		} else {
			props.Add("recipeItemID", -1);
		}
		NetworkAPI.SendExtensionMessage (ClientAPI.GetPlayerOid(), false, "crafting.GRID_UPDATED", props);
		
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("INVENTORY_UPDATE", args);
	}
	
	public void CraftItem() {
		Dictionary<string, object> props = new Dictionary<string, object>();
		//properties["CraftType"] = craftType;
		LinkedList<object> items = new LinkedList<object>();
		LinkedList<object> itemCounts = new LinkedList<object>();
		for (int i = 0; i < gridItems.Count; i++) {
			if (gridItems[i].item != null) {
				items.AddLast(gridItems[i].item.ItemId.ToLong());
				itemCounts.AddLast(gridItems[i].count);
			}
		}
		props.Add ("gridSize", gridSize);
		props.Add ("components", items);
		props.Add ("componentCounts", itemCounts);
		props.Add ("RecipeId", recipeID);
		props.Add ("stationType", stationType.ToString());
		if (recipeItem != null) {
			props.Add("recipeItemID", recipeItem.templateId);
		} else {
			props.Add("recipeItemID", -1);
		}
		NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "crafting.CRAFT_ITEM", props);
	}
	
	public void HandleCraftingGridResponse(Dictionary<string, object> props) {
		recipeID = (int)props["recipeID"];
		recipeName = (string)props["recipeName"];
		recipeItemID = (int)props["recipeItem"];
		resultItemID = (int)props["resultItem"];
		
		if (resultItemID != -1) {
			resultItem = GetComponent<Inventory>().GetItemByTemplateID(resultItemID);
		} else {
			resultItem = null;
		}
		if (recipeItemID != -1) {
			recipeItem = GetComponent<Inventory>().GetItemByTemplateID(recipeItemID);
		} else {
			recipeItem = null;
		}
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("CRAFTING_GRID_UPDATE", args);
	}
	
	void HandleCraftingMessage(Dictionary<string, object> props)
	{
		string msgType = (string)props["PluginMessageType"];
		
		switch (msgType)
		{
		case "CraftingStarted":
		{
			GameObject ui = GameObject.Find("UI");
			ClearGrid();
			//ui.GetComponent<CraftingUI>().StartProgressBar();
			break;
		}
		case "CraftingFailed":
		{
			Dictionary<string, object> errors = new Dictionary<string,object>();
			errors.Add("ErrorText", (string)props["ErrorMsg"]);
			GameObject ui = GameObject.Find("UI");
			ui.GetComponent<ErrorMessage>().HandleErrorMessage(errors);
			break;
		}
		}
		
		AtavismLogger.LogDebugMessage("Got A Crafting Message!");
	}
	
	public void HandleBlueprintMessage(Dictionary<string, object> props) {
		int numBlueprints = (int)props["numBlueprints"];
		
		for (int i = 0; i < numBlueprints; i++) {
			Blueprint bp = new Blueprint();
			bp.recipeID = (int)props["recipeID" + i];
			bp.resultID = (int)props["itemID" + i];
			bp.recipeItemID = (int)props["recipeItemID" + i];
			bp.station = (string)props["station" + i];
			int numRows = (int)props["numRows" + i];
			List<List<CraftingComponent>> slots = new List<List<CraftingComponent>>();
			for (int j = 0; j < numRows; j++) {
				List<CraftingComponent> columnSlots = new List<CraftingComponent>();
				int numColumns = (int)props["numColumns" + i + "_" + j];
				for (int k = 0; k < numColumns; k++) {
					int itemID = (int)props["item" + i + "_" + j + "_" + k];
					if (itemID == -1)
						continue;
					AtavismInventoryItem item = ClientAPI.ScriptObject.GetComponent<Inventory>().GetItemByTemplateID(itemID);
					CraftingComponent component = new CraftingComponent();
					component.item = item;
					columnSlots.Add(component);
				}
				slots.Add(columnSlots);
			}
			bp.slots = slots;
			blueprints.Add(bp);
		}
	}
	
	void HandleResourceDropsMessage(Dictionary<string, object> props)
	{
		resourceLoot = new List<ResourceItem>();
		currentResourceNode = (int)props["resourceNode"];
		int numDrops = (int)props["numDrops"];
		
		for (int i = 0; i < numDrops; i++) {
			int itemID = (int)props["drop" + i];
			int count = (int)props["dropCount" + i];
			ResourceItem resourceItem = new ResourceItem();
			resourceItem.item = ClientAPI.ScriptObject.GetComponent<Inventory>().GetItemByTemplateID(itemID);
			resourceItem.count = count;
			resourceLoot.Add(resourceItem);
		}
		// dispatch a ui event to tell the rest of the system
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("RESOURCE_LOOT_UPDATE", args);
	}
	
	void HandleResourceStateMessage(Dictionary<string, object> props)
	{
		int nodeID = (int)props["nodeID"];
		bool active = (bool)props["active"];
		resourceNodes[nodeID].Active = active;
	}
	
	public void ClearGrid() {
		int gridCount = gridSize * gridSize;
		for (int i = 0; i < gridCount; i++) {
			if (gridItems[i].item != null) {
				gridItems[i].item.ResetUseCount();
			}
		}
		gridItems.Clear();
		for (int i = 0; i < gridCount; i++) {
			gridItems.Add(new CraftingComponent());
		}

		// Also clear special slots
		resultItem = null;
		resultItemID = -1;
		dye = null;
		essence = null;
		recipeItem = null;
		recipeItemID = -1;
		
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("CRAFTING_GRID_UPDATE", args);
		AtavismEventSystem.DispatchEvent("INVENTORY_UPDATE", args);
	}
	
	public List<CraftingComponent> GridItems {
		get {
			return gridItems;
		}
	}
	
	public AtavismInventoryItem ResultItem {
		get {
			return resultItem;
		}
	}
	
	public AtavismInventoryItem RecipeItem {
		get {
			return recipeItem;
		}
	}

	public CraftingStationType StationType {
		get {
			return stationType;
		}
		set {
			stationType = value;
		}
	}

	public GameObject Station {
		get {
			return station;
		}
		set {
			station = value;
		}
	}
	
	public int CurrentResourceNode {
		get {
			return currentResourceNode;
		}
		set {
			currentResourceNode = value;
		}
	}
	
	public List<ResourceItem> ResourceLoot {
		get {
			return resourceLoot;
		}
		set {
			resourceLoot = value;
		}
	}
}
