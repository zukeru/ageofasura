using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Mob Configuration
public class ServerMobs : AtavismDatabaseFunction
{
	
	public Dictionary<int, Mob> dataRegister;
	public Mob editingDisplay;
	public Mob originalDisplay;
	
	public int[] itemIds = new int[] {-1};
	public string[] itemsList = new string[] {"~ none ~"};
	
	public int[] factionIds = new int[] {-1};
	public string[] factionOptions = new string[] {"~ none ~"};
	
	public string[] mobTypeOptions = new string[] {"~ none ~"};
	public string[] speciesOptions = new string[] {"~ none ~"};
	
	public int[] abilityIds = new int[] {-1};
	public string[] abilityOptions = new string[] {"~ none ~"};
	
	// Damage Types
	public string[] damageOptions = new string[] {"~ none ~"};

	// Use this for initialization
	public ServerMobs ()
	{	
		functionName = "Mobs";
		// Database tables name
		tableName = "mob_templates";
		functionTitle = "Mobs Configuration";
		loadButtonLabel = "Load Mobs";
		notLoadedText = "No Mob loaded.";
		// Init
		dataRegister = new Dictionary<int, Mob> ();

		editingDisplay = new Mob ();	
		originalDisplay = new Mob ();	
		
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
	
	public void LoadDamageOptions ()
	{
		if (!dataLoaded) {
			// Read all entries from the table
			string query = "SELECT name FROM damage_type";
			
			// If there is a row, clear it.
			if (rows != null)
				rows.Clear ();
			
			// Load data
			rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
			//Debug.Log("#Rows:"+rows.Count);
			// Read all the data
			int optionsId = 0;
			if ((rows != null) && (rows.Count > 0)) {
				damageOptions = new string[rows.Count];
				foreach (Dictionary<string,string> data in rows) {
					optionsId++;
					damageOptions [optionsId - 1] = data ["name"]; 
				}
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

	private void LoadTableList ()
	{
		string query = "SELECT id, name FROM loot_tables ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows!=null) && (rows.Count > 0)) {
			tablesList = new string[rows.Count + 1];
			tablesList [optionsId] = "~ none ~"; 
			tableIds = new int[rows.Count + 1];
			tableIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				tablesList [optionsId] = data ["id"] + ":" + data ["name"]; 
				tableIds[optionsId] = int.Parse(data ["id"]);
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
					Mob display = new Mob ();
					display.id = int.Parse (data ["id"]);
					display.category = int.Parse (data ["category"]); 
					display.name = data ["name"]; 
					display.subTitle = data ["subTitle"]; 
					display.mobType = int.Parse (data ["mobType"]);
					display.species = data ["species"]; 
					display.subspecies = data ["subSpecies"];
					display.faction = int.Parse (data ["faction"]);
					display.display1 = data ["display1"];
					display.scale = float.Parse (data ["scale"]);
					display.hitBox = int.Parse (data ["hitbox"]); 
					display.baseAnimationState = int.Parse (data ["baseAnimationState"]); 
					display.speedWalk = float.Parse (data ["speed_walk"]); 
					display.speedRun = float.Parse (data ["speed_run"]);
					display.primaryWeapon = int.Parse (data ["primaryWeapon"]); 
					display.secondaryWeapon = int.Parse (data ["secondaryWeapon"]); 
					display.attackable = bool.Parse (data ["attackable"]); 
					display.minLevel = int.Parse (data ["minLevel"]);
					display.maxLevel = int.Parse (data ["maxLevel"]); 
					display.minDamage = int.Parse (data ["minDmg"]); 
					display.maxDamage = int.Parse (data ["maxDmg"]); 
					display.damageType = data ["dmgType"]; 
					display.attackSpeed = float.Parse (data ["attackSpeed"]);
					display.questCategory = data ["questCategory"];
					display.autoAttack = int.Parse (data ["autoAttack"]); 
											
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create a Mob before edit it.");		
			return;
		}
		

		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Mobs Configuration");
		
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
		editingDisplay = new Mob ();		
		originalDisplay = new Mob ();
		selectedDisplay = -1;
	}

	// Edit or Create
	public override void DrawEditor (Rect box, bool newItem)
	{
		// Intercept the drawing of the editor if editing mob loot is true
		if (editingLoot) {
			DrawMobLootEditor(box);
			return;
		}
		
		if (!linkedTablesLoaded) {
			// Load items
			LoadItemList();
			LoadFactionOptions();
			LoadDamageOptions();
			LoadAbilityOptions();
			mobTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Mob Type", false);
			speciesOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Species", true);
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
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new mob");		
			pos.y += 1.5f*ImagePack.fieldHeight;
		}
		
		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);
		pos.y += ImagePack.fieldHeight;
		editingDisplay.subTitle = ImagePack.DrawField (pos, "Subtitle:", editingDisplay.subTitle, 0.75f);
		pos.y += ImagePack.fieldHeight;
		editingDisplay.display1 = ImagePack.DrawGameObject (pos, "Game Object:", editingDisplay.display1, 0.75f);
		pos.y += ImagePack.fieldHeight;
		pos.width /= 2;		
		editingDisplay.species = ImagePack.DrawSelector (pos, "Species:", editingDisplay.species, speciesOptions);
		pos.x += pos.width;
		editingDisplay.subspecies = ImagePack.DrawField (pos, "Subspecies:", editingDisplay.subspecies);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.mobType = ImagePack.DrawSelector (pos, "Mob Type:", editingDisplay.mobType, mobTypeOptions);
		pos.x += pos.width;
		int otherFactionID = GetPositionOfFaction(editingDisplay.faction);
		otherFactionID = ImagePack.DrawSelector (pos, "Faction:", otherFactionID, factionOptions);
		editingDisplay.faction = factionIds[otherFactionID];
		pos.x -= pos.width;
		pos.y += 1.5f*ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Mob Display");		
		pos.y += 1.5f*ImagePack.fieldHeight;
		editingDisplay.scale = ImagePack.DrawField (pos, "Scale:", editingDisplay.scale);
		pos.x += pos.width;
		editingDisplay.hitBox = ImagePack.DrawField (pos, "Hit Range:", editingDisplay.hitBox); 
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.speedWalk = ImagePack.DrawField (pos, "Walk Speed:", editingDisplay.speedWalk);
		pos.x += pos.width;
		editingDisplay.speedRun = ImagePack.DrawField (pos, "Run Speed:", editingDisplay.speedRun);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int selectedItem = GetPositionOfItem(editingDisplay.primaryWeapon);
		selectedItem = ImagePack.DrawSelector (pos, "Prim. Weapon:", selectedItem, itemsList);
		editingDisplay.primaryWeapon = itemIds[selectedItem];
		pos.x += pos.width;
		int selectedItem2 = GetPositionOfItem(editingDisplay.secondaryWeapon);
		selectedItem2 = ImagePack.DrawSelector (pos, "Sec. Weapon:", selectedItem2, itemsList);
		editingDisplay.secondaryWeapon = itemIds[selectedItem2];
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.baseAnimationState = ImagePack.DrawSelector (pos, "Base Animation:", editingDisplay.baseAnimationState - 1, editingDisplay.baseAnimationStateOptions) + 1;
		pos.x += pos.width;
		editingDisplay.category = ImagePack.DrawField (pos, "Category:", editingDisplay.category);
		pos.x -= pos.width;
		pos.y += 1.5f*ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Mob Combat and Stats");
		pos.y += 1.5f*ImagePack.fieldHeight;		
		editingDisplay.minLevel = ImagePack.DrawField (pos, "Min. Level:", editingDisplay.minLevel);
		pos.x += pos.width;
		editingDisplay.maxLevel = ImagePack.DrawField (pos, "Max. Level:", editingDisplay.maxLevel);
		pos.y += ImagePack.fieldHeight;
		pos.x -= pos.width;
		editingDisplay.minDamage = ImagePack.DrawField (pos, "Min. Damage:", editingDisplay.minDamage); 
		pos.x += pos.width;
		editingDisplay.maxDamage = ImagePack.DrawField (pos, "Max. Damage:", editingDisplay.maxDamage); 
		pos.y += ImagePack.fieldHeight;
		pos.x -= pos.width;
		editingDisplay.attackable = ImagePack.DrawToggleBox (pos, "Is Mob Attackable?", editingDisplay.attackable);
		pos.x += pos.width;
		editingDisplay.attackSpeed = ImagePack.DrawField (pos, "Attack Speed:", editingDisplay.attackSpeed);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.damageType = ImagePack.DrawSelector (pos, "Damage Type:", editingDisplay.damageType, damageOptions); 
		pos.x += pos.width;
		editingDisplay.questCategory = ImagePack.DrawField (pos, "Quest Category:", editingDisplay.questCategory);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int selectedAbility = GetPositionOfAbility (editingDisplay.autoAttack);
		selectedAbility = ImagePack.DrawSelector (pos, "Auto Attack:", selectedAbility, abilityOptions);
		editingDisplay.autoAttack = abilityIds [selectedAbility];
		pos.width *= 2;
		
		
		// Mob loot
		pos.x -= ImagePack.innerMargin;
		pos.width /= 3;
		if (!newItem) {
			pos.y += 1.5f*ImagePack.fieldHeight;
			if (ImagePack.DrawButton(pos.x, pos.y, "Edit Mob Loot")) {
				inspectorScrollPosition.y = 0;
				EditMobLoot() ;
			}
		}
		
		// Save data		
		//pos.x -= ImagePack.innerMargin;
		pos.y += 1.5f*ImagePack.fieldHeight;
		//pos.width /= 3;
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
			LoadSelectList ();
		else {
			displayList = null;
			dataLoaded = false;
		}
		
		// Now delete all spawns related to delete mob
		delete = new Register ("mobTemplate", "?mobTemplate", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "spawn_data", delete);
	}
	
	private int GetPositionOfItem(int itemId) {
		for (int i = 0; i < itemIds.Length; i++) {
			if (itemIds[i] == itemId)
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
	
	private int GetPositionOfAbility (int abilityID)
	{
		for (int i = 0; i < abilityIds.Length; i++) {
			if (abilityIds [i] == abilityID)
				return i;
		}
		return 0;
	}

	#region Mob Loot
	public List<MobLoot> mobLoot = new List<MobLoot> ();
	public List<int> mobLootToBeDeleted = new List<int>();
	int maxLootTables = 10;
	bool editingLoot = false;
	
	public int[] tableIds = new int[] {-1};
	public string[] tablesList = new string[] {"~ none ~"};
	bool tablesLoaded = false;
	
	void EditMobLoot() {
		editingLoot = true;
		LoadMobLoot();
		LoadTableList();
	}
	
	public void LoadMobLoot ()
	{
		mobLoot.Clear();
		
		string tableName = "mob_loot";
		// Read all entries from the table
		string query = "SELECT * FROM " + tableName + " where mobTemplate = " + editingDisplay.id;
			
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows!=null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				MobLoot lootEntry = new MobLoot();
				lootEntry.id = int.Parse (data ["id"]);
				lootEntry.category = int.Parse (data ["category"]);
				lootEntry.tableId = int.Parse (data ["lootTable"]);
				lootEntry.chance = int.Parse (data ["dropChance"]);
				lootEntry.mobTemplate = editingDisplay.id;
				mobLoot.Add(lootEntry);
			}
		}
	}
	
	void DrawMobLootEditor(Rect box) {
		// First check if the selected mob has been changed
		if (mobLoot.Count > 0 && editingDisplay.id != mobLoot[0].mobTemplate) {
			LoadMobLoot ();
		}
		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;
		
		ImagePack.DrawLabel (pos.x, pos.y, "Set mob loot tables for mob: " + editingDisplay.name);		
		pos.y += ImagePack.fieldHeight;
		
		pos.width = pos.width / 2;
		for (int i = 0; i < maxLootTables; i++) {
			if (mobLoot.Count > i) {
				int selectedItem = GetPositionOfTable(mobLoot[i].tableId);
				selectedItem = ImagePack.DrawSelector (pos, "Loot Table:", selectedItem, tablesList);
				mobLoot[i].tableId = tableIds[selectedItem];
				//mobLoot[i].tableId = ImagePack.DrawField (pos, "Loot Table:", mobLoot[i].tableId);
				//pos.y += ImagePack.fieldHeight;
				pos.x += pos.width;
				mobLoot[i].chance = ImagePack.DrawField (pos, "Drop Chance:", mobLoot[i].chance);
				pos.y += ImagePack.fieldHeight;
				if (ImagePack.DrawButton (pos.x, pos.y, "Delete Entry")) {
					if (mobLoot[i].id > 0)
						mobLootToBeDeleted.Add(mobLoot[i].id);
					mobLoot.RemoveAt(i);
				}
				pos.x -= pos.width;
				pos.y += ImagePack.fieldHeight;
			}
		}
		if (mobLoot.Count < maxLootTables) {
			if (ImagePack.DrawButton (pos.x, pos.y, "Add Loot Table")) {
				MobLoot lootEntry = new MobLoot();
				lootEntry.mobTemplate = editingDisplay.id;
				mobLoot.Add(lootEntry);
			}
		}
		pos.width = pos.width * 2;
		
		// Save data
		//pos.x -= ImagePack.innerMargin;
		pos.y += 1.4f * ImagePack.fieldHeight;
		pos.width /= 3;
		if (ImagePack.DrawButton (pos.x, pos.y, "Save Data")) {
			SaveChanges();
		}
		
		// Cancel editing
		pos.x += pos.width;
		if (ImagePack.DrawButton (pos.x, pos.y, "Back")) {
			editingLoot = false;
		}
		
		if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawText(pos, result);
		}
	}
	
	void SaveChanges() {
		NewResult("Saving Changes...");
		string tableName = "mob_loot";
		foreach (MobLoot mobLootEntry in mobLoot) {
			if (mobLootEntry.id < 1) {
				// Setup the update query
				string query = "INSERT INTO " + tableName;
				query += " (" + mobLootEntry.FieldList ("", ", ") + ") ";
				query += "VALUES ";
				query += " (" + mobLootEntry.FieldList ("?", ", ") + ") ";
		
				int moblootID = -1;

				// Setup the register data		
				List<Register> update = new List<Register> ();
				foreach (string field in mobLootEntry.fields.Keys) {
					update.Add (mobLootEntry.fieldToRegister (field));       
				}
		
				// Update the database
				moblootID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);
				if (moblootID != -1)
					NewResult ("Update successful");
			} else {
				// Setup the update query
				string query = "UPDATE " + tableName;
				query += " SET ";
				query += mobLootEntry.UpdateList ();
				query += " WHERE id=?id";

				// Setup the register data		
				List<Register> update = new List<Register> ();
				foreach (string field in mobLootEntry.fields.Keys) {
					update.Add (mobLootEntry.fieldToRegister (field));
				}
				update.Add (new Register ("id", "?id", MySqlDbType.Int32, mobLootEntry.id.ToString (), Register.TypesOfField.Int));
	
				// Update the database
				DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
				NewResult ("Update successful");
			}
		}
		
		// And now delete any Objectives that are tagged for deletion
		foreach (int mobLootID in mobLootToBeDeleted) {
			DeleteMobLoot(mobLootID);
		}
	}
	
	void DeleteMobLoot(int mobLootID) {
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, mobLootID.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "mob_loot", delete);
	}
	
	private int GetPositionOfTable(int tableId) {
		for (int i = 0; i < tableIds.Length; i++) {
			if (tableIds[i] == tableId)
				return i;
		}
		return 0;
	}

	#endregion Mob Loot
}
