using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;

// Handles the Character Setup Configuration
public class ServerCharacter : AtavismDatabaseFunction
{

	public Dictionary<int, CharData> dataRegister;
	public CharData editingDisplay;
	public CharData originalDisplay;
	
	public string[] classOptions = new string[] {"~ none ~"};
	public string[] raceOptions = new string[] {"~ none ~"};
	
	public int[] factionIds = new int[] {-1};
	public string[] factionOptions = new string[] {"~ none ~"};
	
	public int[] abilityIds = new int[] {-1};
	public string[] abilityOptions = new string[] {"~ none ~"};
	
	// Character Stats, Skills and Items to Display
	public string[] statsList = null;

	public int[] skillIds = new int[] {-1};
	public string[] skillsList = null;

	public int[] itemIds = new int[] {-1};
	public string[] itemsList = null;
	
	// Use this for initialization
	public ServerCharacter ()
	{	
		functionName = "Player Character Setup";		
		// Database tables name
		tableName = "character_create_template";
		functionTitle = "Character Configuration";
		loadButtonLabel = "Load Character";
		notLoadedText = "No Character loaded.";
		// Init
		dataRegister = new Dictionary<int, CharData> ();

		editingDisplay = new CharData ();			
		originalDisplay = new CharData ();
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
	
	private void LoadStatList() {
		List<CharStatsData> charStats =  new List<CharStatsData>();
		foreach (string stat in statsList) {
			CharStatsData charStat = new CharStatsData();
			charStat.stat = stat;
			charStat.statValue = 0;
			charStats.Add(charStat);
		}
		editingDisplay.charStats = charStats;
	}

	public override void Activate()
	{
		if (statsList == null) {
			LoadStatOptions();
		}
		LoadStatList();
		linkedTablesLoaded = false;
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
			if ((rows!=null) && (rows.Count > 0)) {
				statsList = new string[rows.Count];
				foreach (Dictionary<string,string> data in rows) {
					optionsId++;
					statsList[optionsId-1] = data ["name"]; 
				}
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
		skillsList = new string[rows.Count + 1];
		skillIds = new int[rows.Count + 1];
		// Read data
		int optionsId = 0;
		if ((rows!=null) && (rows.Count > 0)) {
			skillsList [optionsId] = "~ none ~"; 
			skillIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				skillsList [optionsId] = data ["id"] + ":" + data ["name"]; 
				skillIds[optionsId] = int.Parse(data ["id"]);
			}
		}
	}

	public void LoadItemOptions ()
	{
		string query = "SELECT id, name FROM item_templates ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		itemsList = new string[rows.Count + 1];
		itemIds = new int[rows.Count + 1];
		// Read data
		int optionsId = 0;
		if ((rows!=null) && (rows.Count > 0)) {
			itemsList [optionsId] = "~ none ~"; 
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
					CharData display = new CharData ();
					display.id = int.Parse (data ["id"]);
					display.race = data ["race"]; 
					display.aspect = data ["aspect"]; 
					display.faction = int.Parse(data["faction"]);
					display.instanceName = data ["instanceName"]; 
					display.pos_x = float.Parse (data ["pos_x"]);
					display.pos_y = float.Parse (data ["pos_y"]);
					display.pos_z = float.Parse (data ["pos_z"]);
					display.orientation = float.Parse (data ["orientation"]);
					display.autoAttack = int.Parse(data["autoAttack"]);

					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList ();
			}
			dataLoaded = true;
		}

		// Load character starting stats, skills and items
		foreach(CharData charData in dataRegister.Values) {
			LoadCharacterStats(charData);
			LoadCharacterSkills(charData);
			LoadCharacterItems(charData);
		}
	}

	// Load Stats
	public void LoadCharacterStats (CharData charData)
	{
		List<CharStatsData> charStats = new List<CharStatsData>();
		// Read all entries from the table
		string query = "SELECT * FROM character_create_stats where character_create_id = " + charData.id;
			
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
			
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read all the data
		if ((rows!=null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				CharStatsData display = new CharStatsData ();
				display.id = int.Parse (data ["id"]);
				display.charId = int.Parse (data ["character_create_id"]); 
				display.stat = data ["stat"]; 
				display.statValue = int.Parse (data ["value"]);
				display.levelIncrease = float.Parse (data ["levelIncrease"]);
				display.levelPercentIncrease = float.Parse (data ["levelPercentIncrease"]);
				charStats.Add(display);
			}
		}
		// Check for any stats the template may not have
		foreach (string stat in statsList) {
			bool statExists = false;
			foreach (CharStatsData charStat in charStats) {
				if (stat == charStat.stat) {
					statExists = true;
				}
			}
			
			if (!statExists) {
				CharStatsData statData = new CharStatsData();
				statData.stat = stat;
				statData.statValue = 0;
				charStats.Add(statData);
			}
		}
		charData.charStats = charStats;
	}

	// Load Stats
	public void LoadCharacterSkills (CharData charData)
	{
		List<CharSkillsData> charSkills = new List<CharSkillsData>();
		// Read all entries from the table
		string query = "SELECT * FROM character_create_skills WHERE character_create_id = " + charData.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read all the data
		if ((rows!=null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				CharSkillsData display = new CharSkillsData ();
				display.id = int.Parse (data ["id"]);
				display.charId = int.Parse (data ["character_create_id"]); 
				display.skill = int.Parse (data ["skill"]);
				charSkills.Add(display);
			}
		}
		charData.charSkills = charSkills;
	}

	// Load Stats
	public void LoadCharacterItems (CharData charData)
	{
		List<CharItemsData> charItems = new List<CharItemsData>();
		// Read all entries from the table
		string query = "SELECT * FROM character_create_items WHERE character_create_id = " + charData.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read all the data
		if ((rows!=null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				CharItemsData display = new CharItemsData ();
				display.id = int.Parse (data ["id"]);
				display.charId = int.Parse (data ["character_create_id"]); 
				display.itemId = int.Parse (data ["item_id"]);
				display.count = int.Parse (data ["count"]);
				display.equipped = bool.Parse(data["equipped"]);
				charItems.Add(display);
			}
		}
		charData.charItems = charItems;
	}
	
	public void LoadSelectList ()
	{
		//string[] selectList = new string[dataRegister.Count];
		displayList = new string[dataRegister.Count];
		int i = 0;
		foreach (int displayID in dataRegister.Keys) {
			//selectList [i] = displayID + ". " + dataRegister [displayID].name;
			displayList [i] = displayID + ". " + dataRegister [displayID].race + " " + dataRegister [displayID].aspect;
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create an Character before edit it.");		
			return;
		}
		
		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Character Configuration");

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
			//LoadOptions (displayKey);	
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
		editingDisplay = new CharData ();
		originalDisplay = new CharData ();
		selectedDisplay = -1;
		LoadStatList();
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
			LoadFactionOptions();
			LoadAbilityOptions();
			raceOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Race", false);
			classOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Class", false);
			linkedTablesLoaded = true;
		}
		
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Character");		
			pos.y += ImagePack.fieldHeight;
		}
				
		pos.width /= 2;
		editingDisplay.race = ImagePack.DrawSelector (pos, "Character Race:", editingDisplay.race, raceOptions);
		pos.x += pos.width;
		editingDisplay.aspect = ImagePack.DrawSelector (pos, "Character Class:", editingDisplay.aspect, classOptions);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int otherFactionID = GetPositionOfFaction(editingDisplay.faction);
		otherFactionID = ImagePack.DrawSelector (pos, "Faction:", otherFactionID, factionOptions);
		editingDisplay.faction = factionIds[otherFactionID];
		pos.width *= 2;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.instanceName = ImagePack.DrawField (pos, "Instance Name:", editingDisplay.instanceName, 0.75f);
		pos.y += 1.5f * ImagePack.fieldHeight;
		editingDisplay.Spawn = ImagePack.DrawGameObject (pos, "Drag a Game Object to get its Position:", editingDisplay.Spawn, 0.5f);
		pos.y += ImagePack.fieldHeight;
		editingDisplay.Position = ImagePack.Draw3DPosition (pos, "Or insert manually a Spawn Location:", editingDisplay.Position);
		pos.y += ImagePack.fieldHeight;
		pos.width /= 3;
		editingDisplay.orientation = ImagePack.DrawField (pos, "Orientation:", editingDisplay.orientation);
		//pos.x += pos.width;
		pos.width *= 3;
		pos.y += ImagePack.fieldHeight;
		int selectedAbility = GetPositionOfAbility (editingDisplay.autoAttack);
		selectedAbility = ImagePack.DrawSelector (pos, "Auto Attack:", selectedAbility, abilityOptions);
		editingDisplay.autoAttack = abilityIds [selectedAbility];

		pos.y += 1.5f*ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Starting Stats");
		pos.y += 1.5f*ImagePack.fieldHeight;

		// Stats - show a map of all stats
		pos.width /= 2;
		foreach (CharStatsData charStat in editingDisplay.charStats) {
			charStat.statValue = ImagePack.DrawField (pos, charStat.stat, charStat.statValue);
			pos.y += ImagePack.fieldHeight;	
			charStat.levelIncrease = ImagePack.DrawField (pos, "Increases by:", charStat.levelIncrease);
			pos.x += pos.width;
			charStat.levelPercentIncrease = ImagePack.DrawField (pos, "And Percent:", charStat.levelPercentIncrease);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight * 1.5f;	
		}
		pos.width *= 2;
		
		if (!newItem) {
			if (skillsList == null)
				LoadSkillOptions();
			if (itemsList == null)
				LoadItemOptions();

			pos.y += 1.5f*ImagePack.fieldHeight;
			ImagePack.DrawLabel (pos.x, pos.y, "Starting Skills");
			pos.y += 1.5f*ImagePack.fieldHeight;

			pos.width /= 2;
			/*if (editingDisplay.charSkills.Count == 0) {
				editingDisplay.charSkills.Add(new CharSkillsData());
			}*/
			for (int i = 0; i < editingDisplay.charSkills.Count; i++) {
				int selectedSkill = GetPositionOfSkill(editingDisplay.charSkills[i].skill);
				selectedSkill = ImagePack.DrawSelector (pos, "Skill " + (i+1) + ":", selectedSkill, skillsList);
				editingDisplay.charSkills[i].skill = skillIds[selectedSkill];
				pos.x += pos.width;
				if (ImagePack.DrawButton (pos.x, pos.y, "Delete Skill")) {
					if (editingDisplay.charSkills[i].id > 0)
						editingDisplay.skillsToBeDeleted.Add(editingDisplay.charSkills[i].id);
					editingDisplay.charSkills.RemoveAt(i);
				}
				pos.x -= pos.width;
				pos.y += ImagePack.fieldHeight;
				
			}
			if (ImagePack.DrawButton (pos.x, pos.y, "Add Skill")) {
				editingDisplay.charSkills.Add(new CharSkillsData());
			}	

			pos.y += 1.5f*ImagePack.fieldHeight;
			ImagePack.DrawLabel (pos.x, pos.y, "Starting Items");
			pos.y += 1.5f*ImagePack.fieldHeight;

			/*if (editingDisplay.charItems.Count == 0) {
				editingDisplay.charItems.Add(new CharItemsData());
			}*/
			for (int i = 0; i < editingDisplay.charItems.Count; i++) {
				int selectedItem = GetPositionOfItem(editingDisplay.charItems[i].itemId);
				selectedItem = ImagePack.DrawSelector (pos, "Item " + (i+1) + ":", selectedItem, itemsList);
				editingDisplay.charItems[i].itemId = itemIds[selectedItem];
				pos.x += pos.width;
				editingDisplay.charItems[i].count = ImagePack.DrawField (pos, "Count:", editingDisplay.charItems[i].count);
				pos.x -= pos.width;
				pos.y += ImagePack.fieldHeight;
				editingDisplay.charItems[i].equipped = ImagePack.DrawToggleBox(pos, "Equipped", editingDisplay.charItems[i].equipped);
				pos.x += pos.width;
				if (ImagePack.DrawButton (pos.x, pos.y, "Delete Item")) {
					if (editingDisplay.charItems[i].id > 0)
						editingDisplay.itemsToBeDeleted.Add(editingDisplay.charItems[i].id);
					editingDisplay.charItems.RemoveAt(i);
				}
				pos.x -= pos.width;
				pos.y += ImagePack.fieldHeight;
			}
			if (ImagePack.DrawButton (pos.x, pos.y, "Add Item")) {
				editingDisplay.charItems.Add(new CharItemsData());
			}

			pos.width *= 2;
			pos.x += ImagePack.innerMargin;
		}
		
		pos.y += 2.5f * ImagePack.fieldHeight;
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
			pos.x += pos.width;
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawText(pos, result);
		}

		if (!newItem)
			EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 28);
		else
			EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
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
			// Insert the stats
			foreach(CharStatsData entry in editingDisplay.charStats) {
				entry.charId = itemID;
				InsertStat(entry);
			}
			// Update online table to avoid access the database again			
			editingDisplay.id = itemID;
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

	void InsertStat(CharStatsData entry) 
	{
		string query = "INSERT INTO character_create_stats";		
		query += " (character_create_id, stat, value, levelIncrease, levelPercentIncrease) ";
		query += "VALUES ";
		query += " (" + entry.charId + ",'" + entry.stat + "'," + entry.statValue + "," + entry.levelIncrease + "," + entry.levelPercentIncrease + ") ";
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in entry.fields.Keys) {
			update.Add (entry.fieldToRegister (field));       
		}
		
		int itemID = -1;
		itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);
		entry.id = itemID;
	}

	void InsertSkill(CharSkillsData entry) 
	{
		string query = "INSERT INTO character_create_skills";		
		query += " (character_create_id, skill) ";
		query += "VALUES ";
		query += " (" + entry.charId + "," + entry.skill + ") ";
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in entry.fields.Keys) {
			update.Add (entry.fieldToRegister (field));       
		}
		
		int itemID = -1;
		itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);
		entry.id = itemID;
	}

	void InsertItem(CharItemsData entry) 
	{
		string query = "INSERT INTO character_create_items";		
		query += " (character_create_id, item_id, count, equipped) ";
		query += "VALUES ";
		query += " (" + entry.charId + "," + entry.itemId + "," + entry.count + "," + entry.equipped + ") ";
		
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

		// Update stats, skills and items
		foreach(CharStatsData entry in editingDisplay.charStats) {
			entry.charId = editingDisplay.id;
			if (entry.id < 1)
				InsertStat(entry);
			else
				UpdateStat(entry);
		}
		foreach(CharSkillsData entry in editingDisplay.charSkills) {
			if (entry.skill == -1)
				continue;
			entry.charId = editingDisplay.id;
			if (entry.id < 1)
				InsertSkill(entry);
			else
				UpdateSkill(entry);
		}
		foreach(CharItemsData entry in editingDisplay.charItems) {
			if (entry.itemId == -1)
				continue;
			entry.charId = editingDisplay.id;
			if (entry.id < 1)
				InsertItem(entry);
			else
				UpdateItem(entry);
		}
		
		// Delete any items that are tagged for deletion
		foreach (int itemID in editingDisplay.itemsToBeDeleted) {
			DeleteItem(itemID);
		}
		// Delete any skills that are tagged for deletion
		foreach (int skillID in editingDisplay.skillsToBeDeleted) {
			DeleteSkill(skillID);
		}
				
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;	
		NewResult("Entry updated");			
	}

	// Update existing entries in the table based on the iddemo_table
	void UpdateStat (CharStatsData entry)
	{
		// Setup the update query
		string query = "UPDATE character_create_stats";
		query += " SET ";
		query += entry.UpdateList ();
		query += " WHERE id=?id";
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in entry.fields.Keys) {
			update.Add (entry.fieldToRegister (field));       
		}
		update.Add (new Register ("id", "?id", MySqlDbType.Int32, entry.id.ToString (), Register.TypesOfField.Int));
		
		// Update the database
		DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
	}

	// Update existing entries in the table based on the iddemo_table
	void UpdateSkill (CharSkillsData entry)
	{
		// Setup the update query
		string query = "UPDATE character_create_skills";
		query += " SET ";
		query += entry.UpdateList ();
		query += " WHERE id=?id";
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in entry.fields.Keys) {
			update.Add (entry.fieldToRegister (field));       
		}
		update.Add (new Register ("id", "?id", MySqlDbType.Int32, entry.id.ToString (), Register.TypesOfField.Int));
		
		// Update the database
		DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
	}

	// Update existing entries in the table based on the iddemo_table
	void UpdateItem (CharItemsData entry)
	{
		// Setup the update query
		string query = "UPDATE character_create_items";
		query += " SET ";
		query += entry.UpdateList ();
		query += " WHERE id=?id";
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in entry.fields.Keys) {
			update.Add (entry.fieldToRegister (field));       
		}
		update.Add (new Register ("id", "?id", MySqlDbType.Int32, entry.id.ToString (), Register.TypesOfField.Int));
		
		// Update the database
		DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
	}
	
	void DeleteSkill(int skillID) {
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, skillID.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "character_create_skills", delete);
	}
	
	void DeleteItem(int itemID) {
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, itemID.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "character_create_items", delete);
	}
	
	// Delete entries from the table
	void DeleteEntry ()
	{
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
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
		
		delete = new Register ("character_create_id", "?character_create_id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "character_create_stats", delete, true);
	}
	
	private int GetPositionOfFaction (int factionID)
	{
		for (int i = 0; i < factionIds.Length; i++) {
			if (factionIds [i] == factionID)
				return i;
		}
		return 0;
	}
	
	private int GetPositionOfAbility (int abilityID)
	{
		for (int i = 0; i < abilityIds.Length; i++) {
			if (abilityIds [i] == abilityID)
				return i;
		}
		return 0;
	}

	private int GetPositionOfSkill(int skillID) {
		for (int i = 0; i < skillIds.Length; i++) {
			if (skillIds[i] == skillID)
				return i;
		}
		return 0;
	}

	private int GetPositionOfItem(int itemID) {
		for (int i = 0; i < itemIds.Length; i++) {
			if (itemIds[i] == itemID)
				return i;
		}
		return 0;
	}
}
