using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Faction Configuration
public class ServerFactions : AtavismDatabaseFunction
{

	public Dictionary<int, FactionData> dataRegister;
	public FactionData editingDisplay;
	public FactionData originalDisplay;

	public int[] factionIds = new int[] {-1};
	public string[] factionOptions = new string[] {"~ none ~"};

	// Simulated the auto-increment on tables without it
	private int autoID = 1;

	// Use this for initialization
	public ServerFactions ()
	{	
		functionName = "Factions";
		// Database tables name
		tableName = "factions";
		functionTitle = "Factions Configuration";
		loadButtonLabel = "Load Factions";
		notLoadedText = "No Faction loaded.";
		// Init
		dataRegister = new Dictionary<int, FactionData> ();

		editingDisplay = new FactionData ();
		originalDisplay = new FactionData ();
	}

	public override void Activate()
	{
		linkedTablesLoaded = false;
	}

	public void LoadFactionOptions ()
	{
		string query = "SELECT id, name FROM factions ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows!=null) && (rows.Count > 0)) {
			factionOptions = new string[rows.Count + 1];
			factionOptions [optionsId] = "~ none ~"; 
			factionIds = new int[rows.Count + 1];
			factionIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				factionOptions [optionsId] = data ["id"] + ":" + data ["name"]; 
				factionIds[optionsId] = int.Parse(data ["id"]);
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
					FactionData display = new FactionData ();

					display.id = int.Parse (data ["id"]);
					display.name = data ["name"]; 
					display.group = data ["factionGroup"];
					display.modifyable = bool.Parse(data ["public"]);
					display.defaultStance = int.Parse(data ["defaultStance"]);

					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
			LoadSelectList();
			}
			dataLoaded = true;
		}
		foreach(FactionData factionData in dataRegister.Values) {
			LoadFactionStances(factionData);
		}
	}

	void LoadFactionStances(FactionData factionData)
	{
		// Read all entries from the table
		string query = "SELECT * FROM " + "faction_stances" + " where factionID = " + factionData.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows!=null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				FactionStanceEntry entry = new FactionStanceEntry ();
				
				entry.id = int.Parse (data ["id"]);
				entry.factionID = int.Parse (data ["factionID"]); 
				entry.otherFactionID = int.Parse (data ["otherFaction"]); 
				entry.defaultStance = int.Parse (data ["defaultStance"]);
				factionData.factionStances.Add(entry);
			}
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
				// Simulate autoincrement
				if (displayID == autoID)
					// Changes the ID if already exist
					autoID++;
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create a Faction before edit it.");		
			return;
		}	
		
		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Faction Configuration");
		
		
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
		editingDisplay = new FactionData ();		
		originalDisplay = new FactionData ();
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

		if (!linkedTablesLoaded)	{	
			LoadFactionOptions();
			linkedTablesLoaded = true;
		}

		// Draw the content database info
		//pos.y += ImagePack.fieldHeight;
		
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Faction");		
			pos.y += ImagePack.fieldHeight;
		}

		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);
		pos.y += ImagePack.fieldHeight;
		editingDisplay.group = ImagePack.DrawField (pos, "Group:", editingDisplay.group, 0.75f);
		pos.y += ImagePack.fieldHeight;
		pos.width /= 2;
		editingDisplay.modifyable = ImagePack.DrawToggleBox (pos, "Dynamic:", editingDisplay.modifyable);
		pos.y += ImagePack.fieldHeight;
		int selectedStance = GetPositionOfStance(editingDisplay.defaultStance);
		selectedStance = ImagePack.DrawSelector (pos, "Default Stance:", selectedStance, FactionData.stanceOptions);
		editingDisplay.defaultStance = FactionData.stanceValues[selectedStance];
		pos.y += 1.5f*ImagePack.fieldHeight;
		if (editingDisplay.factionStances.Count == 0) {
			editingDisplay.factionStances.Add(new FactionStanceEntry(0, 0));
		}
		for (int i = 0; i < editingDisplay.factionStances.Count; i++) {
			ImagePack.DrawLabel (pos.x, pos.y, "Faction Stance:");
			pos.y += ImagePack.fieldHeight;
			int otherFactionID = GetPositionOfFaction(editingDisplay.factionStances[i].otherFactionID);
			otherFactionID = ImagePack.DrawSelector (pos, "Other Faction:", otherFactionID, factionOptions);
			editingDisplay.factionStances[i].otherFactionID = factionIds[otherFactionID];
			pos.x += pos.width;
			selectedStance = GetPositionOfStance(editingDisplay.factionStances[i].defaultStance);
			selectedStance = ImagePack.DrawSelector (pos, "Default Stance:", selectedStance, FactionData.stanceOptions);
			editingDisplay.factionStances[i].defaultStance = FactionData.stanceValues[selectedStance];
			pos.y += ImagePack.fieldHeight;
			if (ImagePack.DrawButton (pos.x, pos.y, "Delete Stance")) {
				if (editingDisplay.factionStances[i].id > 0)
					editingDisplay.stancesToBeDeleted.Add(editingDisplay.factionStances[i].id);
				editingDisplay.factionStances.RemoveAt(i);
			}
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
		}
		if (ImagePack.DrawButton (pos.x, pos.y, "Add Stance")) {
			editingDisplay.factionStances.Add(new FactionStanceEntry(0, 0));
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
			// Insert the abilities
			foreach(FactionStanceEntry entry in editingDisplay.factionStances) {
				if (entry.otherFactionID != -1) {
					entry.factionID = itemID;
					InsertStance(entry);
				}
			}

			// Update online table to avoid access the database again			
			editingDisplay.id = itemID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;
			LoadFactionOptions ();
			NewResult("New entry inserted");
		} else {
			NewResult("Error occurred, please check the Console");
		}
	}

	void InsertStance(FactionStanceEntry entry) 
	{
		string query = "INSERT INTO faction_stances";		
		query += " (factionID, otherFaction, defaultStance) ";
		query += "VALUES ";
		query += " (" + entry.factionID + "," + entry.otherFactionID + "," + entry.defaultStance + ") ";

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

		// Insert/Update the abilities
		foreach(FactionStanceEntry entry in editingDisplay.factionStances) {
			if (entry.otherFactionID != -1) {
				if (entry.id == 0) {
					// This is a new entry, insert it
					entry.factionID = editingDisplay.id;
					InsertStance(entry);
				} else {
					// This is an existing entry, update it
					entry.factionID = editingDisplay.id;
					UpdateStance(entry);
				}
			}
		}
		// Delete any stances that are tagged for deletion
		foreach (int stanceID in editingDisplay.stancesToBeDeleted) {
			DeleteStance(stanceID);
		}
		
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;		
		NewResult("Entry updated");		
	}

	void UpdateStance(FactionStanceEntry entry) 
	{
		string query = "UPDATE faction_stances";		
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
	
	void DeleteStance(int stanceID) {
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, stanceID.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "faction_stances", delete);
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

		// Delete the stance links
		delete = new Register("factionID", "?factionID", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "faction_stances", delete);
		
		LoadFactionOptions ();
	}

	private int GetPositionOfStance(int stanceValue) {
		for (int i = 0; i < FactionData.stanceValues.Length; i++) {
			if (FactionData.stanceValues[i] == stanceValue)
				return i;
		}
		return 0;
	}

	private int GetPositionOfFaction (int factionID)
	{
		for (int i = 0; i < factionIds.Length; i++) {
			if (factionIds [i] == factionID)
				return i;
		}
		return 0;
	}
}
