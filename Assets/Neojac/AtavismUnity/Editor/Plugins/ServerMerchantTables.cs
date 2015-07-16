using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Skills Configuration
public class ServerMerchantTables : AtavismDatabaseFunction
{

	public Dictionary<int, MerchantTable> dataRegister;
	public MerchantTable editingDisplay;
	public MerchantTable originalDisplay;

	public int[] itemIds = new int[] {-1};
	public string[] itemsList = new string[] {"~ none ~"};

	// Simulated the auto-increment on tables without it - Not used
	private int autoID = 1;

	// Use this for initialization
	public ServerMerchantTables ()
	{	
		functionName = "Merchant Tables";
		// Database tables name
		tableName = "merchant_tables";
		functionTitle = "Merchant Table Configuration";
		loadButtonLabel = "Load Merchant Tables";
		notLoadedText = "No Tables loaded.";
		// Init
		dataRegister = new Dictionary<int, MerchantTable> ();

		editingDisplay = new MerchantTable ();
		originalDisplay = new MerchantTable ();
	}

	public override void Activate ()
	{
		linkedTablesLoaded = false;
	}	

	private void LoadItemList ()
	{
		string query = "SELECT id, name FROM item_templates ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows != null) && (rows.Count > 0)) {
			itemsList = new string[rows.Count + 1];
			itemsList [optionsId] = "~ none ~"; 
			itemIds = new int[rows.Count + 1];
			itemIds [optionsId] = 0;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				itemsList [optionsId] = data ["id"] + ":" + data ["name"]; 
				itemIds [optionsId] = int.Parse (data ["id"]);
			}
		}
	}

	// Load Database Data
	public override void Load ()
	{
		if (!dataLoaded) {
			// Clean old data
			dataRegister.Clear ();
			displayKeys.Clear ();

			// Read all entries from the table
			string query = "SELECT * FROM " + tableName;
			
			// If there is a row, clear it.
			if (rows != null)
				rows.Clear ();
		
			// Load data
			rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
			//Debug.Log("#Rows:"+rows.Count);
			// Read all the data
			if ((rows != null) && (rows.Count > 0)) {
				foreach (Dictionary<string,string> data in rows) {
					//foreach(string key in data.Keys)
					//	Debug.Log("Name[" + key + "]:" + data[key]);
					//return;
					MerchantTable display = new MerchantTable ();

					display.id = int.Parse (data ["id"]);
					display.name = data ["name"]; 

					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList ();
			}
			dataLoaded = true;
			foreach (MerchantTable merchantTable in dataRegister.Values) {
				LoadMerchantTableItems (merchantTable);
			}
		}
	}

	void LoadMerchantTableItems (MerchantTable merchantTable)
	{
		// Read all entries from the table
		string query = "SELECT * FROM " + "merchant_item" + " where tableID = " + merchantTable.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows != null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				MerchantTableItemEntry entry = new MerchantTableItemEntry ();
				
				entry.id = int.Parse (data ["id"]);
				entry.count = int.Parse (data ["count"]); 
				entry.itemID = int.Parse (data ["itemID"]);
				entry.refreshTime = int.Parse (data ["refreshTime"]);
				merchantTable.tableItems.Add (entry);
			}
		}
	}
	
	public void LoadSelectList ()
	{
		//string[] selectList = new string[dataRegister.Count];
		displayList = new string[dataRegister.Count];
		int i = 0;
		foreach (int displayID in dataRegister.Keys) {
			//selectList [i] = displayID + ". " + dataRegister [displayID].name;
			displayList [i] = displayID + ". " + dataRegister [displayID].name;
			i++;
			// Simulate autoincrement
			/*if (displayID == autoID)
					// Changes the ID if already exist
					autoID++;*/
		}
		//displayList = new Combobox(selectList);
	}
	
	
	// Draw the loaded list
	public override  void DrawLoaded (Rect box)
	{	
		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;
				
		if (dataRegister.Count <= 0) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawLabel (pos.x, pos.y, "You must create a Merchant Table before edit it.");		
			return;
		}	
		
		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Merchant Table Configuration");
		
		
		if (newItemCreated) {
			newItemCreated = false;
			LoadSelectList ();
			newSelectedDisplay = displayKeys.Count - 1;
		}
		

		// Draw data Editor
		if (newSelectedDisplay != selectedDisplay) {
			selectedDisplay = newSelectedDisplay;	
			int displayKey = displayKeys [selectedDisplay];
			editingDisplay = dataRegister [displayKey];		
			originalDisplay = editingDisplay.Clone ();
		} 

		//if (!displayList.showList) {
		pos.y += ImagePack.fieldHeight;
		pos.x -= ImagePack.innerMargin;
		pos.y -= ImagePack.innerMargin;
		pos.width += ImagePack.innerMargin;
		DrawEditor (pos, false);
		pos.y -= ImagePack.fieldHeight;
		//pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		//}
		
		if (state != State.Loaded) {
			// Draw combobox
			pos.width /= 2;
			pos.x += pos.width;
			newSelectedDisplay = ImagePack.DrawCombobox (pos, "", selectedDisplay, displayList);
			pos.x -= pos.width;
			pos.width *= 2;
		}
		
	}
	
	public override void CreateNewData ()
	{
		editingDisplay = new MerchantTable ();		
		originalDisplay = new MerchantTable ();
		selectedDisplay = -1;
	}
	
	// Edit or Create
	public override void DrawEditor (Rect box, bool newItem)
	{
		
		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;

		if (!linkedTablesLoaded) {	
			LoadItemList();
			linkedTablesLoaded = true;
		}

		// Draw the content database info
		//pos.y += ImagePack.fieldHeight;
		
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Merchant Table");		
			pos.y += ImagePack.fieldHeight;
		}

		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);		
		pos.width /= 2;
		pos.y += 1.5f * ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Table Items");
		pos.y += 1.5f * ImagePack.fieldHeight;
		if (editingDisplay.tableItems.Count == 0) {
			editingDisplay.tableItems.Add (new MerchantTableItemEntry (-1, -1));
		}
		for (int i = 0; i < editingDisplay.tableItems.Count; i++) {
			editingDisplay.tableItems [i].count = ImagePack.DrawField (pos, "Count:", editingDisplay.tableItems [i].count);
			pos.x += pos.width;
			int selectedItem = GetPositionOfItem (editingDisplay.tableItems [i].itemID);
			selectedItem = ImagePack.DrawSelector (pos, "Item " + (i + 1) + ":", selectedItem, itemsList);
			editingDisplay.tableItems [i].itemID = itemIds [selectedItem];
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.tableItems [i].refreshTime = ImagePack.DrawField (pos, "Refresh Time:", editingDisplay.tableItems [i].refreshTime);
			pos.x += pos.width;
			if (ImagePack.DrawButton (pos.x, pos.y, "Remove Item")) {
				if (editingDisplay.tableItems[i].id > 0)
					editingDisplay.itemsToBeDeleted.Add(editingDisplay.tableItems[i].id);
				editingDisplay.tableItems.RemoveAt(i);
			}
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
		}
		if (ImagePack.DrawButton (pos.x, pos.y, "Add Item")) {
			editingDisplay.tableItems.Add (new MerchantTableItemEntry (-1, -1));
		}

		pos.width *= 2;
		pos.y += 1.4f * ImagePack.fieldHeight;
		// Save data
		pos.x -= ImagePack.innerMargin;
		pos.width /= 3;
		if (ImagePack.DrawButton (pos.x, pos.y, "Save Data")) {
			if (newItem)
				InsertEntry ();
			else
				UpdateEntry ();
			
			state = State.Loaded;
		}
		
		// Delete data
		if (!newItem) {
			pos.x += pos.width;
			if (ImagePack.DrawButton (pos.x, pos.y, "Delete Data")) {
				DeleteEntry ();
				newSelectedDisplay = 0;
				state = State.Loaded;
			}
		}
		
		// Cancel editing
		pos.x += pos.width;
		if (ImagePack.DrawButton (pos.x, pos.y, "Cancel")) {
			editingDisplay = originalDisplay.Clone ();
			if (newItem)
				state = State.New;
			else
				state = State.Loaded;
		}
		
		if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawText (pos, result);
		}
		
		if (!newItem)
			EnableScrollBar (pos.y - box.y + ImagePack.fieldHeight + 28);
		else
			EnableScrollBar (pos.y - box.y + ImagePack.fieldHeight);

	}
	
	// Insert new entries into the table
	void InsertEntry ()
	{
		NewResult ("Inserting...");
		// Setup the update query
		string query = "INSERT INTO " + tableName;		
		query += " (" + editingDisplay.FieldList ("", ", ") + ") ";
		query += "VALUES ";
		query += " (" + editingDisplay.FieldList ("?", ", ") + ") ";
		
		int itemID = -1;

		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in editingDisplay.fields.Keys) {
			update.Add (editingDisplay.fieldToRegister (field));       
		}
		
		// Update the database
		itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);

		// If the insert failed, don't insert the spawn marker
		if (itemID != -1) {   
			editingDisplay.id = itemID; 
			// Insert the abilities
			foreach (MerchantTableItemEntry entry in editingDisplay.tableItems) {
				if (entry.itemID != -1) {
					entry.tableID = itemID;
					InsertItem (entry);
				}
			}

			// Update online table to avoid access the database again			
			//editingDisplay.id = itemID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;

			NewResult ("New entry inserted");
		} else {
			NewResult ("Error occurred, please check the Console");
		}
	}

	void InsertItem (MerchantTableItemEntry entry)
	{
		string query = "INSERT INTO merchant_item";		
		query += " (tableID, count, itemID, refreshTime) ";
		query += "VALUES ";
		query += " (" + entry.tableID + "," + entry.count + "," + entry.itemID + "," + entry.refreshTime + ") ";

		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in entry.fields.Keys) {
			update.Add (entry.fieldToRegister (field));       
		}

		int itemID = -1;
		itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);

		entry.id = itemID;
	}

	// Update existing entries in the table based on the iddemo_table
	void UpdateEntry ()
	{
		NewResult ("Updating...");
		// Setup the update query
		string query = "UPDATE " + tableName;
		query += " SET ";
		query += editingDisplay.UpdateList ();
		query += " WHERE id=?id";

		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in editingDisplay.fields.Keys) {
			update.Add (editingDisplay.fieldToRegister (field));       
		}
		update.Add (new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int));
	
		// Update the database
		DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);

		// Insert/Update the abilities
		foreach (MerchantTableItemEntry entry in editingDisplay.tableItems) {
			if (entry.itemID != -1) {
				if (entry.id < 1) {
					// This is a new entry, insert it
					entry.tableID = editingDisplay.id;
					InsertItem (entry);
				} else {
					// This is an existing entry, update it
					entry.tableID = editingDisplay.id;
					UpdateItem (entry);
				}
			}
		}
		
		// Delete any abilities that are tagged for deletion
		foreach (int itemID in editingDisplay.itemsToBeDeleted) {
			DeleteItem(itemID);
		}
		
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;

		NewResult ("Entry updated");				
	}

	void UpdateItem (MerchantTableItemEntry entry)
	{
		string query = "UPDATE merchant_item";		
		query += " SET ";
		query += entry.UpdateList ();
		query += " WHERE id=?id";
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in entry.fields.Keys) {
			update.Add (entry.fieldToRegister (field));       
		}
		
		DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
	}
	
	void DeleteItem(int itemID) {
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, itemID.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "merchant_item", delete);
	}
	
	// Delete entries from the table
	void DeleteEntry ()
	{
		
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
		
		// Update online table to avoid access the database again			
		dataRegister.Remove (displayKeys [selectedDisplay]);
		displayKeys.Remove (selectedDisplay);
		if (dataRegister.Count > 0)		
			LoadSelectList ();
		else {
			displayList = null;
			dataLoaded = false;
		}

		// Delete the item links
		delete = new Register ("tableID", "?tableID", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "merchant_item", delete);
	}
	
	private int GetPositionOfItem (int itemId)
	{
		for (int i = 0; i < itemIds.Length; i++) {
			if (itemIds [i] == itemId)
				return i;
		}
		return 0;
	}
}
