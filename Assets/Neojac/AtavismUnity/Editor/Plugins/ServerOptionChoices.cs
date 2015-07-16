using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Mob Configuration
public class ServerOptionChoices : AtavismDatabaseFunction 
{
	
	public Dictionary<int, AtavismEditorOption> dataRegister;
	public AtavismEditorOption editingDisplay;
	public AtavismEditorOption originalDisplay;

	// Use this for initialization
	public ServerOptionChoices ()
	{	
		functionName = "Option Choices";
		// Database tables name
	    tableName = "editor_option";
		functionTitle =  "Option Type Configuration";
		loadButtonLabel =  "Load Option Choices";
		notLoadedText =  "No Option Type loaded.";
		// Init
		dataRegister = new Dictionary<int, AtavismEditorOption> ();

		editingDisplay = new AtavismEditorOption ();	
		originalDisplay = new AtavismEditorOption ();	
	}

	public override void Activate()
	{
		linkedTablesLoaded = false;
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
					AtavismEditorOption display = new AtavismEditorOption ();
					display.id = int.Parse (data ["id"]);
					display.name = data ["optionType"];
					display.deletable = bool.Parse (data ["deletable"]);
											
					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList();
			}
			dataLoaded = true;
			foreach (AtavismEditorOption optionData in dataRegister.Values) {
				LoadOptionChoices (optionData);
			}
		}
	}
	
	void LoadOptionChoices (AtavismEditorOption optionData)
	{
		// Read all entries from the table
		string query = "SELECT * FROM " + "editor_option_choice" + " where optionTypeID = " + optionData.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows != null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				AtavismEditorOptionChoice entry = new AtavismEditorOptionChoice ();
				
				entry.id = int.Parse (data ["id"]);
				entry.choice = data ["choice"]; 
				optionData.optionChoices.Add (entry);
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create an Option Type before edit it.");		
			return;
		}
		

		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Option Type Configuration");
		
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
		editingDisplay = new AtavismEditorOption ();		
		originalDisplay = new AtavismEditorOption ();	
		selectedDisplay = -1;
	}
	// Edit or Create
	public override void DrawEditor (Rect box, bool newItem)
	{
		if (!linkedTablesLoaded) {
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
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Option Type");		
			pos.y += ImagePack.fieldHeight;
		}
		
		editingDisplay.name = ImagePack.DrawField (pos, "Option Type:", editingDisplay.name, 0.5f);
		pos.y += 1.5f * ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Option Choices");
		pos.y += ImagePack.fieldHeight;
		
		if (editingDisplay.optionChoices.Count == 0) {
			editingDisplay.optionChoices.Add (new AtavismEditorOptionChoice (0, -1, ""));
		}
		for (int i = 0; i < editingDisplay.optionChoices.Count; i++) {
			pos.width /= 1.5f;
			editingDisplay.optionChoices [i].choice = ImagePack.DrawField (pos, "Choice:", editingDisplay.optionChoices [i].choice);
			pos.x += pos.width;
			if (ImagePack.DrawButton (pos.x, pos.y, "Delete Choice")) {
				if (editingDisplay.optionChoices[i].id > 0)
					editingDisplay.choicesToBeDeleted.Add(editingDisplay.optionChoices[i].id);
				editingDisplay.optionChoices.RemoveAt(i);
			}
			pos.x -= pos.width;
			pos.width *= 1.5f;
			pos.y += ImagePack.fieldHeight;
		}
		if (ImagePack.DrawButton (pos.x, pos.y, "Add Choice")) {
			editingDisplay.optionChoices.Add (new AtavismEditorOptionChoice (0, -1, ""));
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
		if (!newItem && editingDisplay.deletable) {
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
		
		EnableScrollBar(pos.y - box.y + 2*ImagePack.fieldHeight);
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

		// If the insert failed, don't insert the choices
		if (itemID != -1) {          
			// Update online table to avoid access the database again			
			editingDisplay.id = itemID;
			// Insert the choices
			foreach (AtavismEditorOptionChoice entry in editingDisplay.optionChoices) {
				if (entry.choice != "") {
					entry.optionTypeID = itemID;
					InsertChoice (entry);
				}
			}
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
	
	void InsertChoice (AtavismEditorOptionChoice entry)
	{
		string query = "INSERT INTO editor_option_choice";		
		query += " (optionTypeID, choice) ";
		query += "VALUES ";
		query += " (" + entry.optionTypeID + ",'" + entry.choice + "') ";
		
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
		
		// Insert/Update the choices
		foreach (AtavismEditorOptionChoice entry in editingDisplay.optionChoices) {
			if (entry.choice != "") {
				if (entry.id < 1) {
					// This is a new entry, insert it
					entry.optionTypeID = editingDisplay.id;
					InsertChoice (entry);
				} else {
					// This is an existing entry, update it
					entry.optionTypeID = editingDisplay.id;
					UpdateChoice (entry);
				}
			}
		}
		// And now delete any choices that are tagged for deletion
		foreach (int choiceID in editingDisplay.choicesToBeDeleted) {
			DeleteChoice(choiceID);
		}
				
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;	
		NewResult("Entry updated");			
	}
	
	void UpdateChoice (AtavismEditorOptionChoice entry)
	{
		string query = "UPDATE editor_option_choice";		
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
	
	void DeleteChoice(int objectiveID) {
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, objectiveID.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "editor_option_choice", delete);
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

	/// <summary>
	/// Loads the atavism choice options for the specified OptionType
	/// </summary>
	/// <returns>The atavism choice options.</returns>
	/// <param name="optionType">Option type.</param>
	public static string[] LoadAtavismChoiceOptions (string optionType, bool allowNone)
	{
		string[] options = new string[] {};
		
		// First need to get the ID for the optionType
		int optionTypeID = -1;
		
		string query = "SELECT id FROM editor_option where optionType = '" + optionType + "'";
		List<Dictionary<string, string>> rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		if ((rows != null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				optionTypeID = int.Parse(data ["id"]); 
			}
		}
		
		// If we have an ID, load in the options
		if (optionTypeID != -1) {
			// Read all entries from the table
			query = "SELECT * FROM editor_option_choice where optionTypeID = " + optionTypeID;
			
			rows.Clear();
			// Load data
			rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
			//Debug.Log("#Rows:"+rows.Count);
			// Read all the data
			int optionsId = 0;
			if ((rows != null) && (rows.Count > 0)) {
				if (allowNone) {
					options = new string[rows.Count + 1];
					options[0] = "~ none ~";
					optionsId++;
				} else {
					options = new string[rows.Count];
				}
				foreach (Dictionary<string,string> data in rows) {
					optionsId++;
					options [optionsId - 1] = data ["choice"]; 
				}
			}
		}
		return options;
	}
}
