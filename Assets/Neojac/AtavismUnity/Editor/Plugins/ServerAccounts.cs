using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Class that implements the Account configuration
public class ServerAccounts : AtavismDatabaseFunction
{
	public Dictionary<int, AccountData> dataRegister;
	public AccountData editingDisplay;
	public AccountData originalDisplay;

	// Database auxiliar table name
	private string portalTableName = "account_character";

	// Use this for initialization
	public ServerAccounts ()
	{	
		functionName = "Accounts";
		// Database tables name
		tableName = "account";
		functionTitle = "Account Management";
		loadButtonLabel = "Load Accounts";
		notLoadedText = "No Account loaded.";
		// Init
		dataRegister = new Dictionary<int, AccountData> ();

		editingDisplay = new AccountData ();	
		originalDisplay = new AccountData ();			
	}

	public override void Activate ()
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
			rows = DatabasePack.LoadData (DatabasePack.adminDatabasePrefix, query);
		
			// Read all the data
			if ((rows != null) && (rows.Count > 0)) {
				foreach (Dictionary<string,string> data in rows) {
					AccountData display = new AccountData ();
					display.id = int.Parse (data ["id"]);
					display.name = data ["username"];
					display.status = int.Parse (data ["status"]);
					display.isLoaded = false;
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
	
	void GetCharacters (int instanceID)
	{
		/*if (!dataRegister [instanceID].isLoaded) {
			// Load in spawn data
			rows.Clear ();
			string query = "SELECT island, name, locX, locY, locZ, gameObject FROM " + portalTableName;
			// We consider name as the GameObject field
			query += " where island = " + dataRegister [instanceID].id; // + " and name='spawn'";
			
			rows = DatabasePack.LoadData (DatabasePack.adminDatabasePrefix, query);
			// Read all the data
			if ((rows != null) && (rows.Count > 0)) {
				foreach (Dictionary<string,string> data in rows) {
					float locX = float.Parse (data ["locX"]);
					float locY = float.Parse (data ["locY"]);
					float locZ = float.Parse (data ["locZ"]);
					dataRegister [instanceID].spawnLoc = new Vector3 (locX, locY, locZ);
					dataRegister [instanceID].Spawn = data ["gameObject"];
					dataRegister [instanceID].isLoaded = true;
				}
			}
		}*/
	}
	
	// Draw the Instance list
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
			ImagePack.DrawLabel (pos.x, pos.y, "An account must exist before editing it.");		
			return;
		}
		

		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Account Management");
				
		if (newItemCreated) {
			newItemCreated = false;
			LoadSelectList ();
			newSelectedDisplay = displayKeys.Count - 1;
		}
		

		// Draw data Editor
		if (newSelectedDisplay != selectedDisplay) {
			selectedDisplay = newSelectedDisplay;	
			int displayKey = displayKeys [selectedDisplay];
			GetCharacters (dataRegister [displayKey].id);			
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
		editingDisplay = new AccountData ();
		originalDisplay = new AccountData ();
		selectedDisplay = -1;
	}
	
	// Edit or Create a new instance
	public override void DrawEditor (Rect box, bool newInstance)
	{
		
		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;

		// Draw the content database info
		if (newInstance) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new account");		
			pos.y += ImagePack.fieldHeight;
		}
		ImagePack.DrawText(pos, "Username: " + editingDisplay.name); 
		pos.y += ImagePack.fieldHeight;
		int status = GetPositionOfStatus(editingDisplay.status);
		status = ImagePack.DrawSelector (pos, "Status:", status, AccountData.statusOptions);
		editingDisplay.status = AccountData.statusValues[status];
		
		// Save Instance data
		pos.x -= ImagePack.innerMargin;
		pos.y += 1.4f * ImagePack.fieldHeight;
		pos.width /= 3;
		if (!newInstance) {
			if (ImagePack.DrawButton (pos.x, pos.y, "Save Data")) {
			if (newInstance)
				InsertEntry ();
			else
				UpdateEntry ();
			
			state = State.Loaded;
			}
			// Cancel editing
			pos.x += pos.width;
			if (ImagePack.DrawButton (pos.x, pos.y, "Cancel")) {
				editingDisplay = originalDisplay.Clone ();
				if (newInstance)
					state = State.New;
				else
					state = State.Loaded;
			}
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
		itemID = DatabasePack.Insert (DatabasePack.adminDatabasePrefix, query, update);

		// If the insert failed, don't insert the spawn marker
		if (itemID != -1) {
			// Update online table to avoid access the database again			
			editingDisplay.id = itemID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + instanceID + "ID2:" + editingDisplay.id);
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
		DatabasePack.Update (DatabasePack.adminDatabasePrefix, query, update);	
		
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;	
		NewResult("Entry updated");			
	}
	
	// Delete entries from the table
	void DeleteEntry ()
	{
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.adminDatabasePrefix, tableName, delete);
		delete = new Register ("island", "?island", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.adminDatabasePrefix, portalTableName, delete);
		
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
	
	private int GetPositionOfStatus(int statusValue) {
		for (int i = 0; i < AccountData.statusValues.Length; i++) {
			if (AccountData.statusValues[i] == statusValue)
				return i;
		}
		return 0;
	}

}