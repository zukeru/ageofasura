using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Effects Configuration
public class ServerStats : AtavismDatabaseFunction
{

	public Dictionary<int, StatsData> dataRegister;
	public StatsData editingDisplay;
	public StatsData originalDisplay;
	
	public string[] statOptions = new string[] {"~ none ~"};
	public string[] statFunctionOptions = new string[] {"~ none ~"};
	public string[] statShiftActionOptions = new string[] {"~ none ~"};
	public string[] statShiftRequirementOptions = new string[] {"~ none ~"};
	
	public int[] effectIds = new int[] {-1};
	public string[] effectOptions = new string[] {"~ none ~"};

	// Use this for initialization
	public ServerStats ()
	{	
		functionName = "Stats";		
		// Database tables name
		tableName = "stat";
		functionTitle = "Stats Configuration";
		loadButtonLabel = "Load Stats";
		notLoadedText = "No Stats loaded.";
		// Init
		dataRegister = new Dictionary<int, StatsData> ();

		editingDisplay = new StatsData ();			
		originalDisplay = new StatsData ();			
	}

	public override void Activate()
	{
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
	
	public void LoadEffectOptions ()
	{
		if (!dataLoaded) {
			// Read all entries from the table
			string query = "SELECT id, name FROM effects";
			
			// If there is a row, clear it.
			if (rows != null)
				rows.Clear ();
			
			// Load data
			rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
			//Debug.Log("#Rows:"+rows.Count);
			// Read all the data
			int optionsId = 0;
			if ((rows != null) && (rows.Count > 0)) {
				effectOptions = new string[rows.Count + 1];
				effectOptions [optionsId] = "~ none ~"; 
				effectIds = new int[rows.Count + 1];
				effectIds [optionsId] = 0;
				foreach (Dictionary<string,string> data in rows) {
					optionsId++;
					effectOptions [optionsId] = data ["name"]; 
					effectIds [optionsId] = int.Parse (data ["id"]);
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
			if ((rows!=null) && (rows.Count > 0)) {
				foreach (Dictionary<string,string> data in rows) {
					//foreach(string key in data.Keys)
					//	Debug.Log("Name[" + key + "]:" + data[key]);
					//return;
					StatsData display = new StatsData ();
					// As we don have a primary key ID field
					fakeId++;
					display.id = fakeId;
					display.name = data ["name"];
                    display.originalName = display.name;
					display.type = int.Parse(data ["type"]);
					display.statFunction = data ["stat_function"];
					display.mobBase = int.Parse(data ["mob_base"]);
					display.mobLevelIncrease = int.Parse(data ["mob_level_increase"]);
					display.mobLevelPercentIncrease = float.Parse(data ["mob_level_percent_increase"]);
					display.min = int.Parse(data ["min"]);
					display.maxstat = data ["maxstat"];
					byte shiftPlayerOnly = byte.Parse(data ["shiftTarget"]);
					display.shiftTarget = (int) shiftPlayerOnly;
					display.shiftValue = int.Parse(data ["shiftValue"]);
					display.shiftReverseValue = int.Parse(data ["shiftReverseValue"]);
					display.shiftInterval = int.Parse(data ["shiftInterval"]);
					display.isShiftPercent = bool.Parse(data ["isShiftPercent"]);
					display.onMaxHit = data ["onMaxHit"]; 
					display.onMinHit = data ["onMinHit"]; 
					display.shiftReq1 = data ["shiftReq1"]; 
					display.shiftReq1State = bool.Parse(data ["shiftReq1State"]);
					display.shiftReq1SetReverse = bool.Parse(data ["shiftReq1SetReverse"]);
					display.shiftReq2 = data ["shiftReq2"]; 
					display.shiftReq2State = bool.Parse(data ["shiftReq2State"]);
					display.shiftReq2SetReverse = bool.Parse(data ["shiftReq2SetReverse"]);
					display.shiftReq3 = data ["shiftReq3"]; 
					display.shiftReq3State = bool.Parse(data ["shiftReq3State"]);
					display.shiftReq3SetReverse = bool.Parse(data ["shiftReq3SetReverse"]);

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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create an Stat before editing it.");		
			return;
		}
		
		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Stats Configuration");

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
			editingDisplay = new StatsData ();		
			originalDisplay = new StatsData ();
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
			LoadStatOptions();
			LoadEffectOptions();
			statFunctionOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Stat Functions", true);
			statShiftActionOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Stat Shift Action", true);
			statShiftRequirementOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Stat Shift Requirement", true);
			linkedTablesLoaded = true;
		}

		// Draw the content database info
		//pos.y += ImagePack.fieldHeight;
		
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Stat");		
			pos.y += ImagePack.fieldHeight;
		}
		
		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);
		pos.y += ImagePack.fieldHeight;
		pos.width /= 2;
		editingDisplay.type = ImagePack.DrawSelector (pos, "Stat Type:", editingDisplay.type, editingDisplay.typeOptions);
		pos.x += pos.width;
		editingDisplay.statFunction = ImagePack.DrawSelector (pos, "Stat Function:", editingDisplay.statFunction, statFunctionOptions);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight * 1.5f;
		ImagePack.DrawLabel(pos, "Mob Values and Progression for this Stat:");
		pos.y += ImagePack.fieldHeight;
		editingDisplay.mobBase = ImagePack.DrawField (pos, "Base Value:", editingDisplay.mobBase);
		pos.y += ImagePack.fieldHeight;
		ImagePack.DrawText(pos, "Each level it...");
		pos.y += ImagePack.fieldHeight;
		editingDisplay.mobLevelIncrease = ImagePack.DrawField (pos, "Increases by:", editingDisplay.mobLevelIncrease);
		pos.x += pos.width;
		editingDisplay.mobLevelPercentIncrease = ImagePack.DrawField (pos, "And Percentage:", editingDisplay.mobLevelPercentIncrease);
		pos.x -= pos.width;
		pos.width *= 2;
		
		if (editingDisplay.type == 2) {
			// Draw additional vitality stat fields
			pos.y += 2.5f * ImagePack.fieldHeight; 
			ImagePack.DrawLabel(pos, "Vitality Stat Settings:");
			pos.y += ImagePack.fieldHeight;
			pos.width /= 2;
			
			editingDisplay.min = ImagePack.DrawField (pos, "Minimum:", editingDisplay.min);
			pos.x += pos.width;
			editingDisplay.maxstat = ImagePack.DrawSelector (pos, "Max Stat:", editingDisplay.maxstat, statOptions);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.shiftTarget = ImagePack.DrawSelector (pos, "Shift Target:", editingDisplay.shiftTarget, editingDisplay.targetOptions);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.shiftValue = ImagePack.DrawField (pos, "Shift Value:", editingDisplay.shiftValue);
			pos.x += pos.width;
			editingDisplay.shiftReverseValue = ImagePack.DrawField (pos, "Reverse Value:", editingDisplay.shiftReverseValue);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.shiftInterval = ImagePack.DrawField (pos, "Shift Interval:", editingDisplay.shiftInterval);
			pos.x += pos.width;
			editingDisplay.isShiftPercent = ImagePack.DrawToggleBox (pos, "Is Shift Percentage?", editingDisplay.isShiftPercent);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			
			if (editingDisplay.onMinHit.StartsWith("effect")) {
				string[] vals = editingDisplay.onMinHit.Split(':');
				editingDisplay.onMinHit = ImagePack.DrawSelector (pos, "On Min Hit:", vals[0], statShiftActionOptions);
				if (editingDisplay.onMinHit.StartsWith("effect")) {
					pos.x += pos.width;
					int effectID = -1;
					if (vals.Length > 1)
						effectID = int.Parse(vals[1]);
					int selectedEffect = GetPositionOfEffect(effectID);
					selectedEffect = ImagePack.DrawSelector (pos, "Effect:", selectedEffect, effectOptions);
					effectID = effectIds[selectedEffect];
					editingDisplay.onMinHit = "effect:" + effectID;
					pos.x -= pos.width;
				}
			} else {
				editingDisplay.onMinHit = ImagePack.DrawSelector (pos, "On Min Hit:", editingDisplay.onMinHit, statShiftActionOptions);
			}
			pos.y += ImagePack.fieldHeight;
			if (editingDisplay.onMaxHit.StartsWith("effect")) {
				string[] vals = editingDisplay.onMaxHit.Split(':');
				editingDisplay.onMaxHit = ImagePack.DrawSelector (pos, "On max Hit:", vals[0], statShiftActionOptions);
				if (editingDisplay.onMaxHit.StartsWith("effect")) {
					pos.x += pos.width;
					int effectID = -1;
					if (vals.Length > 1)
						effectID = int.Parse(vals[1]);
					int selectedEffect = GetPositionOfEffect(effectID);
					selectedEffect = ImagePack.DrawSelector (pos, "Effect:", selectedEffect, effectOptions);
					effectID = effectIds[selectedEffect];
					editingDisplay.onMaxHit = "effect:" + effectID;
					pos.x -= pos.width;
				}
			} else {
				editingDisplay.onMaxHit = ImagePack.DrawSelector (pos, "On Max Hit:", editingDisplay.onMaxHit, statShiftActionOptions);
			}
			pos.y += ImagePack.fieldHeight;
			editingDisplay.shiftReq1 = ImagePack.DrawSelector (pos, "Requirement 1:", editingDisplay.shiftReq1, statShiftRequirementOptions);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.shiftReq1State = ImagePack.DrawToggleBox (pos, "Req True?", editingDisplay.shiftReq1State);
			pos.x += pos.width;
			editingDisplay.shiftReq1SetReverse = ImagePack.DrawToggleBox (pos, "Reverse if Fail?", editingDisplay.shiftReq1SetReverse);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.shiftReq2 = ImagePack.DrawSelector (pos, "Requirement 2:", editingDisplay.shiftReq2, statShiftRequirementOptions);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.shiftReq2State = ImagePack.DrawToggleBox (pos, "Req True?", editingDisplay.shiftReq2State);
			pos.x += pos.width;
			editingDisplay.shiftReq2SetReverse = ImagePack.DrawToggleBox (pos, "Reverse if Fail?", editingDisplay.shiftReq2SetReverse);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.shiftReq3 = ImagePack.DrawSelector (pos, "Requirement 3:", editingDisplay.shiftReq3, statShiftRequirementOptions);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.shiftReq3State = ImagePack.DrawToggleBox (pos, "Req True?", editingDisplay.shiftReq3State);
			pos.x += pos.width;
			editingDisplay.shiftReq3SetReverse = ImagePack.DrawToggleBox (pos, "Reverse if Fail?", editingDisplay.shiftReq3SetReverse);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			
			pos.width *= 2;
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
		
		if (!newItem)
			EnableScrollBar (pos.y - box.y + ImagePack.fieldHeight + 28);
		else
			EnableScrollBar (pos.y - box.y + ImagePack.fieldHeight);

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

		// If the insert succeeded
		if (itemID != -1) {          
			// Update online table to avoid access the database again			
			editingDisplay.id = dataRegister.Count; // Set the highest free index ;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;
			LoadStatOptions ();
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
		query += " WHERE name='" + editingDisplay.originalName + "'";

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
		
        // Need to update all entries in the character_create_stats if the statname changed
        query = "UPDATE character_create_stats set stat = '" + editingDisplay.name + "' where stat = '" + editingDisplay.originalName + "'";
        DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

        //TODO: update all items and effects that reference this stat
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
			LoadSelectList();
		else {
			displayList = null;
			dataLoaded = false;
		}
		LoadStatOptions ();
		
		// Delete from character stats
		delete = new Register ("stat", "?stat", MySqlDbType.String, editingDisplay.name, Register.TypesOfField.String);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "character_create_stats", delete, true);
		
	}

	private int GetPositionOfEffect (int effectID)
	{
		for (int i = 0; i < effectIds.Length; i++) {
			if (effectIds [i] == effectID)
				return i;
		}
		return 0;
	}
}
