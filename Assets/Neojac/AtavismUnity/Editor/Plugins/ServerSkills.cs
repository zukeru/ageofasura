using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Skills Configuration
public class ServerSkills : AtavismDatabaseFunction
{

	public Dictionary<int, SkillsData> dataRegister;
	public SkillsData editingDisplay;
	public SkillsData originalDisplay;
	public int[] abilityIds = new int[] {-1};
	public string[] abilityOptions = new string[] {"~ none ~"};
	public int[] skillIds = new int[] {-1};
	public string[] skillOptions = new string[] {"~ none ~"};
	public string[] statOptions = new string[] {"~ none ~"};
	public string[] aspectOptions = new string[] {"~ none ~"};
	
	// Handles the prefab creation, editing and save
	private SkillPrefab prefab = null;

	// Simulated the auto-increment on tables without it - Not used
	private int autoID = 1;

	// Use this for initialization
	public ServerSkills ()
	{	
		functionName = "Skills";
		// Database tables name
		tableName = "skills";
		functionTitle = "Skills Configuration";
		loadButtonLabel = "Load Skills";
		notLoadedText = "No Skill loaded.";
		// Init
		dataRegister = new Dictionary<int, SkillsData> ();

		editingDisplay = new SkillsData ();
		originalDisplay = new SkillsData ();
	}

	public override void Activate ()
	{
		linkedTablesLoaded = false;
	}

	public void LoadAbilityOptions ()
	{
		string query = "SELECT id, name FROM abilities ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows != null) && (rows.Count > 0)) {
			abilityOptions = new string[rows.Count + 1];
			abilityOptions [optionsId] = "~ none ~"; 
			abilityIds = new int[rows.Count + 1];
			abilityIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				abilityOptions [optionsId] = data ["id"] + ":" + data ["name"]; 
				abilityIds [optionsId] = int.Parse (data ["id"]);
			}
		}
	}
	
	public void LoadSkillOptions ()
	{
		string query = "SELECT id, name FROM skills ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows != null) && (rows.Count > 0)) {
			skillOptions = new string[rows.Count + 1];
			skillOptions [optionsId] = "~ none ~"; 
			skillIds = new int[rows.Count + 1];
			skillIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				skillOptions [optionsId] = data ["id"] + ":" + data ["name"]; 
				skillIds [optionsId] = int.Parse (data ["id"]);
			}
		}
	}

	public void LoadStatOptions ()
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
			if ((rows != null) && (rows.Count > 0)) {
				foreach (Dictionary<string,string> data in rows) {
					//foreach(string key in data.Keys)
					//	Debug.Log("Name[" + key + "]:" + data[key]);
					//return;
					SkillsData display = new SkillsData ();

					display.id = int.Parse (data ["id"]);
					display.name = data ["name"]; 
					display.icon = data ["icon"]; 
					display.aspect = data ["aspect"];
					display.oppositeAspect = data ["oppositeAspect"];
					display.primaryStat = data ["primaryStat"];
					display.secondaryStat = data ["secondaryStat"];
					display.thirdStat = data ["thirdStat"];
					display.fourthStat = data ["fourthStat"];
					
					display.maxLevel = int.Parse (data ["maxLevel"]);
					display.automaticallyLearn = bool.Parse (data ["automaticallyLearn"]);
					display.skillPointCost = int.Parse (data ["skillPointCost"]);
					display.parentSkill = int.Parse (data ["parentSkill"]);
					display.parentSkillLevelReq = int.Parse (data ["parentSkillLevelReq"]);
					
					display.prereqSkill1 = int.Parse (data ["prereqSkill1"]);
					display.prereqSkill1Level = int.Parse (data ["prereqSkill1Level"]);
					display.prereqSkill2 = int.Parse (data ["prereqSkill2"]);
					display.prereqSkill2Level = int.Parse (data ["prereqSkill2Level"]);
					display.prereqSkill3 = int.Parse (data ["prereqSkill3"]);
					display.prereqSkill3Level = int.Parse (data ["prereqSkill3Level"]);
					display.playerLevelReq = int.Parse (data ["playerLevelReq"]);

					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList ();
			}
			dataLoaded = true;
			foreach (SkillsData skillData in dataRegister.Values) {
				LoadSkillAbilities (skillData);
			}
		}
	}

	void LoadSkillAbilities (SkillsData skillData)
	{
		// Read all entries from the table
		string query = "SELECT * FROM " + "skill_ability_gain" + " where skillID = " + skillData.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows != null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				SkillAbilityEntry entry = new SkillAbilityEntry ();
				
				entry.id = int.Parse (data ["id"]);
				entry.skillLevelReq = int.Parse (data ["skillLevelReq"]); 
				entry.abilityID = int.Parse (data ["abilityID"]);
				entry.automaticallyLearn = bool.Parse (data ["automaticallyLearn"]);
				skillData.skillAbilities.Add (entry);
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create a Skill before edit it.");		
			return;
		}	
		
		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Skills Configuration");
		
		
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
		editingDisplay = new SkillsData ();		
		originalDisplay = new SkillsData ();
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
			LoadAbilityOptions ();
			LoadSkillOptions();
			LoadStatOptions ();
			aspectOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Class", true);
			linkedTablesLoaded = true;
		}

		// Draw the content database info
		//pos.y += ImagePack.fieldHeight;
		
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Skill");		
			pos.y += ImagePack.fieldHeight;
		}

		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);		
		pos.y += ImagePack.fieldHeight;
		pos.width /= 2;
		editingDisplay.maxLevel = ImagePack.DrawField (pos, "Max Level:", editingDisplay.maxLevel);
		pos.x += pos.width;
		editingDisplay.icon = ImagePack.DrawTextureAsset (pos, "Icon:", editingDisplay.icon);		
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.skillPointCost = ImagePack.DrawField (pos, "Skill Point Cost:", editingDisplay.skillPointCost);
		pos.y += ImagePack.fieldHeight;
		editingDisplay.automaticallyLearn = ImagePack.DrawToggleBox (pos, "Automatically Learn:", editingDisplay.automaticallyLearn);
		pos.y += ImagePack.fieldHeight * 2f;
		editingDisplay.aspect = ImagePack.DrawSelector (pos, "Skill Class:", editingDisplay.aspect, aspectOptions);
		pos.x += pos.width;
		editingDisplay.oppositeAspect = ImagePack.DrawSelector (pos, "Opposite Class:", editingDisplay.oppositeAspect, aspectOptions);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.primaryStat = ImagePack.DrawSelector (pos, "Primary Stat:", editingDisplay.primaryStat, statOptions);
		pos.x += pos.width;
		editingDisplay.secondaryStat = ImagePack.DrawSelector (pos, "Secondary Stat:", editingDisplay.secondaryStat, statOptions);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.thirdStat = ImagePack.DrawSelector (pos, "Third Stat:", editingDisplay.thirdStat, statOptions);
		pos.x += pos.width;
		editingDisplay.fourthStat = ImagePack.DrawSelector (pos, "Fourth Stat:", editingDisplay.fourthStat, statOptions);
		pos.x -= pos.width;
		pos.y += 1.5f * ImagePack.fieldHeight;
		// Requirements
		ImagePack.DrawLabel (pos.x, pos.y, "Requirements");
		pos.y += ImagePack.fieldHeight;
		int selectedSkill = GetPositionOfSkill (editingDisplay.parentSkill);
		selectedSkill = ImagePack.DrawSelector (pos, "Parent Skill:", selectedSkill, skillOptions);
		editingDisplay.parentSkill = skillIds[selectedSkill];
		pos.x += pos.width;
		editingDisplay.parentSkillLevelReq = ImagePack.DrawField (pos, "Skill Level Req:", editingDisplay.parentSkillLevelReq);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		// prereq 1
		selectedSkill = GetPositionOfSkill (editingDisplay.prereqSkill1);
		selectedSkill = ImagePack.DrawSelector (pos, "Prereq Skill 1:", selectedSkill, skillOptions);
		editingDisplay.prereqSkill1 = skillIds[selectedSkill];
		pos.x += pos.width;
		editingDisplay.prereqSkill1Level = ImagePack.DrawField (pos, "Skill Level Req:", editingDisplay.prereqSkill1Level);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		// prereq 2
		selectedSkill = GetPositionOfSkill (editingDisplay.prereqSkill2);
		selectedSkill = ImagePack.DrawSelector (pos, "Prereq Skill 2:", selectedSkill, skillOptions);
		editingDisplay.prereqSkill2 = skillIds[selectedSkill];
		pos.x += pos.width;
		editingDisplay.prereqSkill2Level = ImagePack.DrawField (pos, "Skill Level Req:", editingDisplay.prereqSkill2Level);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		// prereq 3
		selectedSkill = GetPositionOfSkill (editingDisplay.prereqSkill3);
		selectedSkill = ImagePack.DrawSelector (pos, "Prereq Skill 3:", selectedSkill, skillOptions);
		editingDisplay.prereqSkill3 = skillIds[selectedSkill];
		pos.x += pos.width;
		editingDisplay.prereqSkill3Level = ImagePack.DrawField (pos, "Skill Level Req:", editingDisplay.prereqSkill3Level);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.playerLevelReq = ImagePack.DrawField (pos, "Player Level Req:", editingDisplay.playerLevelReq);
		pos.y += 1.5f * ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Skill Abilities");
		pos.y += 1.5f * ImagePack.fieldHeight;
		if (editingDisplay.skillAbilities.Count == 0) {
			editingDisplay.skillAbilities.Add (new SkillAbilityEntry (0, -1));
		}
		for (int i = 0; i < editingDisplay.skillAbilities.Count; i++) {
			editingDisplay.skillAbilities [i].skillLevelReq = ImagePack.DrawField (pos, "Level:", editingDisplay.skillAbilities [i].skillLevelReq);
			pos.x += pos.width;
			int selectedAbility = GetPositionOfAbility (editingDisplay.skillAbilities [i].abilityID);
			selectedAbility = ImagePack.DrawSelector (pos, "Ability " + (i + 1) + ":", selectedAbility, abilityOptions);
			editingDisplay.skillAbilities [i].abilityID = abilityIds [selectedAbility];
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.skillAbilities [i].automaticallyLearn = ImagePack.DrawToggleBox (pos, "Automatically Learn:", editingDisplay.skillAbilities [i].automaticallyLearn);
			pos.x += pos.width;
			if (ImagePack.DrawButton (pos.x, pos.y, "Remove Ability")) {
				if (editingDisplay.skillAbilities[i].id > 0)
					editingDisplay.abilitiesToBeDeleted.Add(editingDisplay.skillAbilities[i].id);
				editingDisplay.skillAbilities.RemoveAt(i);
			}
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
		}
		if (ImagePack.DrawButton (pos.x, pos.y, "Add Ability")) {
			editingDisplay.skillAbilities.Add (new SkillAbilityEntry (0, -1));
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
			foreach (SkillAbilityEntry entry in editingDisplay.skillAbilities) {
				if (entry.abilityID != -1) {
					entry.skillID = itemID;
					InsertAbility (entry);
				}
			}

			// Update online table to avoid access the database again			
			//editingDisplay.id = itemID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;
			
			// Configure the correponding prefab
			CreatePrefab ();
			LoadSkillOptions ();
			NewResult ("New entry inserted");
		} else {
			NewResult ("Error occurred, please check the Console");
		}
	}

	void InsertAbility (SkillAbilityEntry entry)
	{
		string query = "INSERT INTO skill_ability_gain";		
		query += " (skillID, skillLevelReq, abilityID, automaticallyLearn) ";
		query += "VALUES ";
		query += " (" + entry.skillID + "," + entry.skillLevelReq + "," + entry.abilityID + "," + entry.automaticallyLearn + ") ";

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
		foreach (SkillAbilityEntry entry in editingDisplay.skillAbilities) {
			if (entry.abilityID != -1) {
				if (entry.id < 1) {
					// This is a new entry, insert it
					entry.skillID = editingDisplay.id;
					InsertAbility (entry);
				} else {
					// This is an existing entry, update it
					entry.skillID = editingDisplay.id;
					UpdateAbility (entry);
				}
			}
		}
		
		// Delete any abilities that are tagged for deletion
		foreach (int abilityID in editingDisplay.abilitiesToBeDeleted) {
			DeleteAbility(abilityID);
		}
		
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;
		
		// Configure the correponding prefab
		CreatePrefab ();
		NewResult ("Entry updated");				
	}

	void UpdateAbility (SkillAbilityEntry entry)
	{
		string query = "UPDATE skill_ability_gain";		
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
	
	void DeleteAbility(int abilityID) {
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, abilityID.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "skill_ability_gain", delete);
	}
	
	// Delete entries from the table
	void DeleteEntry ()
	{
		// Remove the prefab
		DeletePrefab ();
		
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

		// Delete the ability links
		delete = new Register ("skillID", "?skillID", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "skill_ability_gain", delete);
		
		LoadSkillOptions ();
	}
	
	void CreatePrefab ()
	{
		// Configure the correponding prefab
		prefab = new SkillPrefab (editingDisplay.id, editingDisplay.name);
		prefab.Save (editingDisplay.icon, editingDisplay.parentSkill, editingDisplay.parentSkillLevelReq, editingDisplay.playerLevelReq);
	}
	
	void DeletePrefab ()
	{
		prefab = new SkillPrefab (editingDisplay.id, editingDisplay.name);
		
		if (prefab.Load ())
			prefab.Delete ();
	}

	private int GetPositionOfAbility (int abilityID)
	{
		for (int i = 0; i < abilityIds.Length; i++) {
			if (abilityIds [i] == abilityID)
				return i;
		}
		return 0;
	}
	
	private int GetPositionOfSkill (int skillID)
	{
		for (int i = 0; i < skillIds.Length; i++) {
			if (skillIds [i] == skillID)
				return i;
		}
		return 0;
	}
}
