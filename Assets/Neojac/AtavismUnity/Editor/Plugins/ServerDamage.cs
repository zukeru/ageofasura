using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Effects Configuration
public class ServerDamage : AtavismDatabaseFunction
{

	public Dictionary<int, DamageData> dataRegister;
	public DamageData editingDisplay;
	public DamageData originalDisplay;
	public string[] statOptions = new string[] {"~ none ~"};

	// Use this for initialization
	public ServerDamage ()
	{	
		functionName = "Damage";		
		// Database tables name
		tableName = "damage_type";
		functionTitle = "Damage Configuration";
		loadButtonLabel = "Load Damage";
		notLoadedText = "No Damage loaded.";
		// Init
		dataRegister = new Dictionary<int, DamageData> ();

		editingDisplay = new DamageData ();	
		originalDisplay = new DamageData ();
	}

	public override void Activate ()
	{
		linkedTablesLoaded = false;
	}
	
	public void LoadStatsOptions ()
	{
		if (!dataLoaded) {
			// Read all entries from the table
			string query = "SELECT name FROM stat";
			
			// If there is a row, clear it.
			if (rows != null)
				rows.Clear ();
		
			// Load data
			rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
			//Debug.Log("#Rows:"+rows.Count);
			// Read all the data
			int optionsId = 0;
			if ((rows != null) && (rows.Count > 0)) {
				statOptions = new string[rows.Count + 1];
				statOptions [optionsId] = "~ none ~"; 
				foreach (Dictionary<string,string> data in rows) {
					optionsId++;
					statOptions [optionsId] = data ["name"]; 
				}
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
			int fakeId = 0;
			if ((rows != null) && (rows.Count > 0)) {
				foreach (Dictionary<string,string> data in rows) {
					//foreach(string key in data.Keys)
					//	Debug.Log("Name[" + key + "]:" + data[key]);
					//return;
					DamageData display = new DamageData ();
					// As we don have a primary key ID field
					fakeId++;
					display.id = fakeId;

					display.name = data ["name"]; 
					display.resistanceStat = data ["resistance_stat"];

					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList ();
			}
			dataLoaded = true;
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create an Damage before edit it.");		
			return;
		}
		
		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Damage Configuration");

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
		editingDisplay = new DamageData ();		
		originalDisplay = new DamageData ();	
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

		// Draw the content database info
		//pos.y += ImagePack.fieldHeight;
		if (!linkedTablesLoaded) {	
			LoadStatsOptions ();
			linkedTablesLoaded = true;
		}
		
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Damage");		
			pos.y += ImagePack.fieldHeight;
		}
		
		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);
		pos.y += ImagePack.fieldHeight;
		editingDisplay.resistanceStat = ImagePack.DrawCombobox (pos, "Resistance Stat:", editingDisplay.resistanceStat, statOptions);
		pos.y += ImagePack.fieldHeight;
		
		pos.y += 1.5f * ImagePack.fieldHeight;
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
			// Update online table to avoid access the database again			
			editingDisplay.id = dataRegister.Count; // Set the highest free index ;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
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
		query += " WHERE name=?name";

		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in editingDisplay.fields.Keys) {
			update.Add (editingDisplay.fieldToRegister (field));       
		}
		//update.Add (new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int));
	
		// Update the database
		DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
				
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;	
		NewResult("Entry updated");			
	}
	
	// Delete entries from the table
	void DeleteEntry ()
	{
		Register delete = new Register ("name", "?name", MySqlDbType.String, editingDisplay.name, Register.TypesOfField.String);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete, true);
		
		// Update online table to avoid access the database again			
		dataRegister.Remove (displayKeys [selectedDisplay]);
		displayKeys.Remove (selectedDisplay);
		if (dataRegister.Count > 0)		
			LoadSelectList ();
		else {
			displayList = null;
			dataLoaded = false;
		}

	}


}
