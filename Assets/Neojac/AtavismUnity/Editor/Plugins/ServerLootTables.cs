using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Mob Configuration
public class ServerLootTables : AtavismDatabaseFunction 
{
	
	public Dictionary<int, LootTable> dataRegister;
	public LootTable editingDisplay;
	public LootTable originalDisplay;
	
	public int[] itemIds = new int[] {-1};
	public string[] itemsList = new string[] {"~ none ~"};

	// Use this for initialization
	public ServerLootTables ()
	{	
		functionName = "Loot Tables";
		// Database tables name
	    tableName = "loot_tables";
		functionTitle =  "Loot Table Configuration";
		loadButtonLabel =  "Load Loot Tables";
		notLoadedText =  "No Loot Table loaded.";
		// Init
		dataRegister = new Dictionary<int, LootTable> ();

		editingDisplay = new LootTable ();	
		originalDisplay = new LootTable ();	
	}

	public override void Activate()
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
		if ((rows!=null) && (rows.Count > 0)) {
			itemsList = new string[rows.Count + 1];
			itemsList [optionsId] = "~ none ~"; 
			itemIds = new int[rows.Count + 1];
			itemIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				itemsList [optionsId] = data ["id"] + ":" + data ["name"]; 
				itemIds[optionsId] = int.Parse(data ["id"]);
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
			if ((rows!=null) && (rows.Count > 0)) {
				foreach (Dictionary<string,string> data in rows) {
					//foreach(string key in data.Keys)
					//	Debug.Log("Name[" + key + "]:" + data[key]);
					//return;
					LootTable display = new LootTable ();
					display.id = int.Parse (data ["id"]);
					display.name = data ["name"];
					for (int i = 1; i <= display.maxEntries; i++) {
						int itemId = int.Parse (data ["item" + i]); 
						if (itemId != -1) {
							int chance = int.Parse (data ["item" + i + "chance"]);
							int count = int.Parse (data ["item" + i + "count"]);
							LootTableEntry entry = new LootTableEntry(itemId, chance, count);
							display.entries.Add(entry);
						}
					}
											
					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList();
			}
			dataLoaded = true;
		}
	}
	
	public void LoadSelectList() 
	{
			//string[] selectList = new string[dataRegister.Count];
			displayList =  new string[dataRegister.Count];
			int i = 0;
			foreach (int displayID in dataRegister.Keys) {
				//selectList [i] = displayID + ". " + dataRegister [displayID].name;
				displayList [i] = displayID + ". " + dataRegister [displayID].name;
				i++;
			}
			//displayList = new Combobox(selectList);
	}	
	
	// Draw the loaded list
	public override void DrawLoaded (Rect box)
	{	
		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;
								
		if (dataRegister.Count <= 0) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawLabel (pos.x, pos.y, "You must create a Loot Table before edit it.");		
			return;
		}
		

		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Loot Table Configuration");
		
		if (newItemCreated) {
			newItemCreated = false;
			LoadSelectList();
			newSelectedDisplay = displayKeys.Count - 1;
		}
		

		// Draw data Editor
		if (newSelectedDisplay != selectedDisplay) {
		selectedDisplay = newSelectedDisplay;	
		int displayKey = displayKeys [selectedDisplay];
		editingDisplay = dataRegister [displayKey];		
			originalDisplay = editingDisplay.Clone();
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
	
	public override void CreateNewData()
	{
			editingDisplay = new LootTable ();		
			originalDisplay = new LootTable ();	
		selectedDisplay = -1;
	}
	// Edit or Create
	public override void DrawEditor (Rect box, bool newItem)
	{
		if (!linkedTablesLoaded) {
			// Load items
			LoadItemList();
			linkedTablesLoaded = true;
		}
		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;

		// Draw the content database info		
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new loot table");		
			pos.y += ImagePack.fieldHeight;
		}
		
		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.5f);
		pos.y += ImagePack.fieldHeight;
		
		if (editingDisplay.entries.Count == 0) {
			editingDisplay.entries.Add(new LootTableEntry(-1, 100, 1));
		}
		for (int i = 0; i < editingDisplay.maxEntries; i++) {
			if (editingDisplay.entries.Count > i) {
				pos.width = pos.width / 2;
				int selectedItem = GetPositionOfItem(editingDisplay.entries[i].itemId);
				selectedItem = ImagePack.DrawSelector (pos, "Item " + (i+1) + ":", selectedItem, itemsList);
				editingDisplay.entries[i].itemId = itemIds[selectedItem];
				pos.x += pos.width;
				editingDisplay.entries[i].count = ImagePack.DrawField (pos, "Count:", editingDisplay.entries[i].count);
				pos.x -= pos.width;
				pos.y += ImagePack.fieldHeight;
				editingDisplay.entries[i].chance = ImagePack.DrawField (pos, "Chance:", editingDisplay.entries[i].chance);
				pos.x += pos.width;
				if (ImagePack.DrawButton (pos.x, pos.y, "Remove Item")) {
					editingDisplay.entries.RemoveAt(i);
				}
				pos.x -= pos.width;
				pos.y += ImagePack.fieldHeight;
				pos.width = pos.width * 2;
			}
		}
		if (editingDisplay.entries.Count < editingDisplay.maxEntries) {
			if (ImagePack.DrawButton (pos.x, pos.y, "Add Item")) {
				LootTableEntry lootTableEntry = new LootTableEntry(-1, 0, 1);
				editingDisplay.entries.Add(lootTableEntry);
			}
		}
		//pos.width = pos.width * 2;
		
		// Save data		
		pos.x -= ImagePack.innerMargin;
		pos.y += 1.4f * ImagePack.fieldHeight;
		pos.width /=3;
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
			editingDisplay = originalDisplay.Clone();
			if (newItem)
				state = State.New;
			else
				state = State.Loaded;
		}
		
		if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawText(pos, result);
		}
	}
	
	// Insert new entries into the table
	void InsertEntry ()
	{
		NewResult("Inserting...");
		// Setup the update query
		string query = "INSERT INTO " + tableName;		
		query += " (" + editingDisplay.FieldList ("", ", ") + ") ";
		query += "VALUES ";
		query += " (" + editingDisplay.FieldList ("?", ", ") + ") ";
		
		int mobID = -1;

		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in editingDisplay.fields.Keys) {
			update.Add (editingDisplay.fieldToRegister (field));       
		}
		
		// Update the database
		mobID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);

		// If the insert failed, don't insert the spawn marker
		if (mobID != -1) {          
			// Update online table to avoid access the database again			
			editingDisplay.id = mobID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + mobID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;
			NewResult("New entry inserted");
		} else {
			NewResult("Error occurred, please check the Console");
		}
	}

	// Update existing entries in the table based on the iddemo_table
	void UpdateEntry ()
	{
		NewResult("Updating...");
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
				
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;	
		NewResult("Entry updated");			
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
			LoadSelectList();
		else {
			displayList = null;
			dataLoaded = false;
		}
	}
	
	private int GetPositionOfItem(int itemId) {
		for (int i = 0; i < itemIds.Length; i++) {
			if (itemIds[i] == itemId)
				return i;
		}
		return 0;
	}

}
