using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Class that implements the Instances configuration
public class ServerInstances : AtavismDatabaseFunction
{
	public Dictionary<int, Instance> dataRegister;
	public Instance editingDisplay;
	public Instance originalDisplay;
	
	public int[] accountIds = new int[] {1};
	public string[] accountList = new string[] {"~ First Account ~"};

	// Database auxiliar table name
	private string portalTableName = "island_portals";

	// Use this for initialization
	public ServerInstances ()
	{	
		functionName = "Instances";
		// Database tables name
		tableName = "instance_template";
		functionTitle = "Instance Configuration";
		loadButtonLabel = "Load Instances";
		notLoadedText = "No Instance loaded.";
		// Init
		dataRegister = new Dictionary<int, Instance> ();

		editingDisplay = new Instance ();	
		originalDisplay = new Instance ();			
	}

	public override void Activate ()
	{
		linkedTablesLoaded = false;
	}
	
	private void LoadAccountList ()
	{
		string query = "SELECT id, username FROM account";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.adminDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows != null) && (rows.Count > 0)) {
			accountList = new string[rows.Count];
			accountIds = new int[rows.Count];
			foreach (Dictionary<string,string> data in rows) {
				accountList [optionsId] = data ["id"] + ":" + data ["username"]; 
				accountIds [optionsId] = int.Parse (data ["id"]);
				optionsId++;
			}
		} else {
			accountList = new string[1];
			accountList [optionsId] = "~ First Account ~"; 
			accountIds = new int[1];
			accountIds [optionsId] = 1;
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
			string query = "SELECT id, island_name, createOnStartup, administrator, populationLimit FROM " + tableName;
			
			// If there is a row, clear it.
			if (rows != null)
				rows.Clear ();
		
			// Load data
			rows = DatabasePack.LoadData (DatabasePack.adminDatabasePrefix, query);
		
			// Read all the data
			if ((rows != null) && (rows.Count > 0)) {
				foreach (Dictionary<string,string> data in rows) {
					Instance display = new Instance ();
					display.id = int.Parse (data ["id"]);
					display.name = data ["island_name"];
					display.createOnStartup = bool.Parse (data ["createOnStartup"]);
					display.administrator = int.Parse(data["administrator"]);
					display.populationLimit = int.Parse(data["populationLimit"]);
					display.isLoaded = false;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList ();
			}
			dataLoaded = true;
			foreach (Instance instanceEntry in dataRegister.Values) {
				LoadInstancePortals (instanceEntry);
			}
		}
	}
	
	void LoadInstancePortals (Instance instanceEntry)
	{
		// Read all entries from the table
		string query = "SELECT * FROM " + "island_portals" + " where island = " + instanceEntry.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.adminDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows != null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				InstancePortalEntry entry = new InstancePortalEntry ();
				
				entry.id = int.Parse (data ["id"]);
				entry.name = data ["name"]; 
				entry.loc = new Vector3 (int.Parse (data ["locX"]), int.Parse (data ["locY"]), int.Parse (data ["locZ"]));
				entry.orient = new Quaternion(int.Parse (data ["orientX"]), int.Parse (data ["orientY"]), int.Parse (data ["orientZ"]), int.Parse (data ["orientW"]));
				instanceEntry.instancePortals.Add (entry);
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
		}
		//displayList = new Combobox(selectList);
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create an Instance before edit it.");		
			return;
		}
		

		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Instance Configuration");
				
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
		editingDisplay = new Instance ();
		originalDisplay = new Instance ();
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
		
		if (!linkedTablesLoaded) {	
			LoadAccountList();
			linkedTablesLoaded = true;
		}

		// Draw the content database info
		if (newInstance) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new instance");		
			pos.y += ImagePack.fieldHeight;
		}
		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);

		pos.y += ImagePack.fieldHeight;
		editingDisplay.createOnStartup = ImagePack.DrawToggleBox (pos, "Create On Startup:", editingDisplay.createOnStartup);
		
		pos.y += ImagePack.fieldHeight;
		int selectedAccount = GetPositionOfAccount (editingDisplay.administrator);
		selectedAccount = ImagePack.DrawSelector (pos, "Admin Account:", selectedAccount, accountList);
		editingDisplay.administrator = accountIds [selectedAccount];
		pos.y += ImagePack.fieldHeight;
		editingDisplay.populationLimit = ImagePack.DrawField (pos, "Population Limit:", editingDisplay.populationLimit);
		
		pos.y += 1.5f * ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Markers");
		pos.y += 1.5f * ImagePack.fieldHeight;
		if (editingDisplay.instancePortals.Count == 0) {
			editingDisplay.instancePortals.Add (new InstancePortalEntry("spawn", Vector3.zero));
		}
		for (int i = 0; i < editingDisplay.instancePortals.Count; i++) {
			editingDisplay.instancePortals [i].name = ImagePack.DrawField (pos, "Name:", editingDisplay.instancePortals [i].name);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.instancePortals[i].loc = ImagePack.Draw3DPosition (pos, "Location:", editingDisplay.instancePortals[i].loc);
			pos.y += ImagePack.fieldHeight * 1.5f;
			pos.width /= 2;
			float yaw = editingDisplay.instancePortals[i].orient.eulerAngles.y;
			yaw = ImagePack.DrawField (pos, "Rotation:", yaw);
			Vector3 pitchYawRoll = editingDisplay.instancePortals[i].orient.eulerAngles;
			pitchYawRoll.y = yaw;
			editingDisplay.instancePortals[i].orient.eulerAngles = pitchYawRoll;
			pos.y += ImagePack.fieldHeight;
			pos.width *= 2;
			editingDisplay.instancePortals[i].MarkerObject = ImagePack.DrawGameObject (pos, "Game Object:", editingDisplay.instancePortals[i].gameObject, 0.75f);
			pos.y += ImagePack.fieldHeight;
			if (i > 0) {
				pos.width /= 2;
				pos.x += pos.width;
				if (ImagePack.DrawButton (pos.x, pos.y, "Remove Marker")) {
					if (editingDisplay.instancePortals[i].id > 0)
						editingDisplay.itemsToBeDeleted.Add(editingDisplay.instancePortals[i].id);
					editingDisplay.instancePortals.RemoveAt(i);
				}
				pos.x -= pos.width;
				pos.width *= 2;
			}
			pos.y += ImagePack.fieldHeight;
		}
		
		if (ImagePack.DrawButton (pos.x, pos.y, "Add Marker")) {
			editingDisplay.instancePortals.Add (new InstancePortalEntry());
		}
		
		// Save Instance data
		pos.x -= ImagePack.innerMargin;
		pos.y += 1.4f * ImagePack.fieldHeight;
		pos.width /= 3;
		if (ImagePack.DrawButton (pos.x, pos.y, "Save Data")) {
			if (newInstance)
				InsertEntry ();
			else
				UpdateEntry ();
			
			state = State.Loaded;
		}
		
		// Delete Instance data
		if (!newInstance) {
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
			if (newInstance)
				state = State.New;
			else
				state = State.Loaded;
		}
		
		if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawText (pos, result);
		}
		
		if (!newInstance)
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
		query += " (island_name, template, administrator, category, status, createOnStartup, islandType, public, password, style, recommendedLevel, description, size, populationLimit) ";
		query += "VALUES ";
		query += "(?island_name, ?template, ?administrator, ?category, ?status, ?createOnStartup, ?islandType, ?public, ?password, ?style, ?recommendedLevel, ?description, ?size, ?populationLimit)";
		
		int instanceID = -1;

		// Setup the register data		
		List<Register> update = new List<Register> ();
		update.Add (new Register ("island_name", "?island_name", MySqlDbType.VarChar, editingDisplay.name.ToString (), Register.TypesOfField.String));       
		update.Add (new Register ("template", "?template", MySqlDbType.VarChar, "", Register.TypesOfField.String));
		update.Add (new Register ("administrator", "?administrator", MySqlDbType.Int32, editingDisplay.administrator.ToString(), Register.TypesOfField.Int));
		update.Add (new Register ("category", "?category", MySqlDbType.Int32, "1", Register.TypesOfField.Int));
		update.Add (new Register ("status", "?status", MySqlDbType.VarChar, "Active", Register.TypesOfField.String));
		update.Add (new Register ("createOnStartup", "?createOnStartup", MySqlDbType.Byte, editingDisplay.createOnStartup.ToString (), Register.TypesOfField.Bool));
		update.Add (new Register ("islandType", "?islandType", MySqlDbType.Int32, "0", Register.TypesOfField.Int));
		update.Add (new Register ("public", "?public", MySqlDbType.Byte, "true", Register.TypesOfField.Bool));
		update.Add (new Register ("password", "?password", MySqlDbType.VarChar, "", Register.TypesOfField.String));
		update.Add (new Register ("style", "?style", MySqlDbType.VarChar, "", Register.TypesOfField.String));
		update.Add (new Register ("recommendedLevel", "?recommendedLevel", MySqlDbType.Int32, "0", Register.TypesOfField.Int));
		update.Add (new Register ("description", "?description", MySqlDbType.VarChar, "", Register.TypesOfField.String));	
		update.Add (new Register ("size", "?size", MySqlDbType.Int32, "-1", Register.TypesOfField.Int));
		update.Add (new Register ("populationLimit", "?populationLimit", MySqlDbType.Int32, editingDisplay.populationLimit.ToString(), Register.TypesOfField.Int));

		// Update the database
		instanceID = DatabasePack.Insert (DatabasePack.adminDatabasePrefix, query, update);

		// If the insert failed, don't insert the spawn marker
		if (instanceID != -1) {
			int islandID = instanceID;
			
			foreach (InstancePortalEntry entry in editingDisplay.instancePortals) {
				if (entry.name != "") {
					entry.instanceID = instanceID;
					InsertItem (entry);
				}
			}
          
			// Update online table to avoid access the database again			
			editingDisplay.id = instanceID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + instanceID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;
			NewResult ("New entry inserted");
		} else {
			NewResult ("Error occurred, please check the Console");
		}
	}
	
	void InsertItem (InstancePortalEntry entry)
	{
		string query = "INSERT INTO " + portalTableName;		
		query += " (island, portalType, faction, displayID, locX, locY, locZ, orientX, orientY, orientZ, orientW, name) ";
		query += "VALUES ";
		query += " (" + entry.instanceID + "," + entry.portalType + "," + entry.faction + "," + entry.displayID + "," + entry.loc.x + "," 
			+ entry.loc.y + "," + entry.loc.z + "," + entry.orient.x + "," + entry.orient.y 
			+ "," + entry.orient.z + "," + entry.orient.w + ",'" + entry.name + "') ";
		
		// Setup the register data
		List<Register> update = new List<Register> ();
		foreach (string field in entry.fields.Keys) {
			update.Add (entry.fieldToRegister (field));       
		}
		
		int itemID = -1;
		itemID = DatabasePack.Insert (DatabasePack.adminDatabasePrefix, query, update);
		
		entry.id = itemID;
	}

	// Update existing entries in the table based on the iddemo_table
	void UpdateEntry ()
	{
		NewResult ("Updating...");
		// Setup the update query
		string query = "UPDATE " + tableName;
		query += " SET island_name=?island_name,";
		query += " createOnStartup=?createOnStartup,";
		query += " islandType=?islandType,";
		query += " administrator=?administrator,";
		query += " populationLimit=?populationLimit WHERE id=?id";
		// Setup the register data		
		List<Register> update = new List<Register> ();
		update.Add (new Register ("island_name", "?island_name", MySqlDbType.VarChar, editingDisplay.name.ToString (), Register.TypesOfField.String));
		update.Add (new Register ("createOnStartup", "?createOnStartup", MySqlDbType.Byte, editingDisplay.createOnStartup.ToString (), Register.TypesOfField.Bool));
		update.Add (new Register ("islandType", "?islandType", MySqlDbType.Int32, "0", Register.TypesOfField.Int));
		update.Add (new Register ("administrator", "?administrator", MySqlDbType.Int32, editingDisplay.administrator.ToString(), Register.TypesOfField.Int));
		update.Add (new Register ("populationLimit", "?populationLimit", MySqlDbType.Int32, editingDisplay.populationLimit.ToString(), Register.TypesOfField.Int));
		update.Add (new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int));
	
		// Update the database
		DatabasePack.Update (DatabasePack.adminDatabasePrefix, query, update);
		
		// Insert/Update the abilities
		foreach (InstancePortalEntry entry in editingDisplay.instancePortals) {
			if (entry.name != "") {
				if (entry.id < 1) {
					// This is a new entry, insert it
					entry.instanceID = editingDisplay.id;
					InsertItem (entry);
				} else {
					// This is an existing entry, update it
					entry.instanceID = editingDisplay.id;
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
	
	void UpdateItem (InstancePortalEntry entry)
	{
		string query = "UPDATE " + portalTableName;
		query += " SET ";
		query += entry.UpdateList ();
		query += " WHERE id=?id";
		
		// Setup the register data
		List<Register> update = new List<Register> ();
		foreach (string field in entry.fields.Keys) {
			update.Add (entry.fieldToRegister (field));       
		}
		
		DatabasePack.Update (DatabasePack.adminDatabasePrefix, query, update);
	}
	
	void DeleteItem(int portalID) {
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, portalID.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, portalTableName, delete);
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
	
	private int GetPositionOfAccount (int accountId)
	{
		for (int i = 0; i < accountIds.Length; i++) {
			if (accountIds [i] == accountId)
				return i;
		}
		return 0;
	}

}