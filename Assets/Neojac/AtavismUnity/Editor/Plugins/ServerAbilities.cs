using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Skills Configuration
public class ServerAbilities : AtavismDatabaseFunction
{

	public Dictionary<int, AbilitiesData> dataRegister;
	public AbilitiesData editingDisplay;
	public AbilitiesData originalDisplay;
	public int[] skillIds = new int[] {-1};
	public string[] skillOptions = new string[] {"~ none ~"};
	public int[] effectIds = new int[] {-1};
	public string[] effectOptions = new string[] {"~ none ~"};
	public int[] itemIds = new int[] {-1};
	public string[] itemsList = new string[] {"~ none ~"};
	public int[] coordIds = new int[] {-1};
	public string[] coordList = new string[] {"~ none ~"};
	
	public string[] weaponTypeOptions = new string[] {"~ none ~"};
	public string[] speciesOptions = new string[] {"~ none ~"};
	public string[] targetOptions = new string[] {"~ none ~"};
	
	Vector2 tooltipScroll = new Vector2();

	// Handles the prefab creation, editing and save
	private AbilityPrefab prefab = null;

	// Use this for initialization
	public ServerAbilities ()
	{	
		functionName = "Abilities";		
		// Database tables name
		tableName = "abilities";
		functionTitle = "Abilities Configuration";
		loadButtonLabel = "Load Abilities";
		notLoadedText = "No Abilitie loaded.";
		// Init
		dataRegister = new Dictionary<int, AbilitiesData> ();

		editingDisplay = new AbilitiesData ();			
		originalDisplay = new AbilitiesData ();
	}

	public override void Activate ()
	{
		linkedTablesLoaded = false;
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
			skillIds [optionsId] = 0;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				skillOptions [optionsId] = data ["id"] + ":" + data ["name"]; 
				skillIds [optionsId] = int.Parse (data ["id"]);
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
		if ((rows != null) && (rows.Count > 0)) {
			itemsList = new string[rows.Count + 1];
			itemsList [optionsId] = "~ none ~"; 
			itemIds = new int[rows.Count + 1];
			itemIds [optionsId] = 0;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				itemsList [optionsId] = data ["id"] + ":" + data ["name"]; 
				itemIds [optionsId] = int.Parse (data ["id"]);
			}
		}
	}

	private void LoadCoordList ()
	{
		string query = "SELECT id, name FROM coordinated_effects ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows != null) && (rows.Count > 0)) {
			coordList = new string[rows.Count + 1];
			coordList [optionsId] = "~ none ~"; 
			coordIds = new int[rows.Count + 1];
			coordIds [optionsId] = 0;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				coordList [optionsId] = data ["name"]; 
				coordIds [optionsId] = int.Parse (data ["id"]);
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
					AbilitiesData display = new AbilitiesData ();

					// Cleanup 
					string [] fields = new string[data.Count];
					int i = 0;
					foreach (string field in data.Keys) {
						fields [i] = field;
						i++;
					}
					foreach (string field in fields) {
						if (field != "id") {
							if ((display.fields [field] == "int") && (data [field] == ""))
								data [field] = "0";
							if (display.fields [field] == "bool") {
								if (data [field].ToLower () == "true")
									data [field] = "True";
								else
									data [field] = "False";
							}
						}
					}

					display.id = int.Parse (data ["id"]);
					display.name = data ["name"]; 
					display.icon = data ["icon"]; 

					display.abilityType = data ["abilityType"];
					display.skill = int.Parse (data ["skill"]);
					display.passive = bool.Parse (data ["passive"]);
					display.activationCost = int.Parse (data ["activationCost"]);
					display.activationCostType = data ["activationCostType"];
					display.activationLength = float.Parse (data ["activationLength"]);
					display.activationAnimation = data ["activationAnimation"];
					display.activationParticles = data ["activationParticles"];
					display.casterEffectRequired = int.Parse (data ["casterEffectRequired"]);
					display.casterEffectConsumed = bool.Parse (data ["casterEffectConsumed"]);
					display.targetEffectRequired = int.Parse (data ["targetEffectRequired"]);
					display.targetEffectConsumed = bool.Parse (data ["targetEffectConsumed"]);
					display.weaponRequired = data ["weaponRequired"];
					display.reagentRequired = int.Parse (data ["reagentRequired"]);
					display.reagentConsumed = bool.Parse (data ["reagentConsumed"]);
					display.maxRange = int.Parse (data ["maxRange"]);
					display.minRange = int.Parse (data ["minRange"]);
					display.aoeRadius = int.Parse (data ["aoeRadius"]);
					display.targetType = data ["targetType"];
					display.targetState = int.Parse (data ["targetState"]);
					display.speciesTargetReq = data ["speciesTargetReq"];
					display.specificTargetReq = data ["specificTargetReq"];
					display.globalCooldown = bool.Parse (data ["globalCooldown"]);
					display.cooldown1Type = data ["cooldown1Type"];
					display.cooldown1Duration = float.Parse (data ["cooldown1Duration"]);
					display.weaponCooldown = bool.Parse (data ["weaponCooldown"]);
					display.activationEffect1 = int.Parse (data ["activationEffect1"]);
					display.activationTarget1 = data ["activationTarget1"];
					display.activationEffect2 = int.Parse (data ["activationEffect2"]);
					display.activationTarget2 = data ["activationTarget2"];
					display.activationEffect3 = int.Parse (data ["activationEffect3"]);
					display.activationTarget3 = data ["activationTarget3"];
					display.coordEffect1Event = data ["coordEffect1event"];
					display.coordEffect1 = data ["coordEffect1"];
					display.coordEffect2Event = data ["coordEffect2event"];
					display.coordEffect2 = data ["coordEffect2"];
					display.tooltip = data ["tooltip"];

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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create an Ability before edit it.");		
			return;
		}

		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Abilities Configuration");


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
		editingDisplay = new AbilitiesData ();		
		originalDisplay = new AbilitiesData ();	
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
			LoadEffectOptions ();
			LoadSkillOptions ();
			LoadItemList ();
			LoadCoordList ();
			weaponTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Weapon Type", true);
			speciesOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Species", true);
			targetOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Target Type", true);
			linkedTablesLoaded = true;
		}

		// Draw the content database info		
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Ability");		
			pos.y += 1.5f * ImagePack.fieldHeight;
		}
		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);
		pos.y += ImagePack.fieldHeight;
		pos.width /= 2;
		editingDisplay.abilityType = ImagePack.DrawSelector (pos, "Ability Type:", editingDisplay.abilityType, editingDisplay.abilityTypeOptions);
		pos.x += pos.width;
		editingDisplay.icon = ImagePack.DrawTextureAsset (pos, "Icon:", editingDisplay.icon);		
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int selectedSkill = GetPositionOfSkill (editingDisplay.skill);
		selectedSkill = ImagePack.DrawSelector (pos, "Skill:", selectedSkill, skillOptions);
		editingDisplay.skill = skillIds [selectedSkill];
		pos.y += ImagePack.fieldHeight;
		editingDisplay.passive = ImagePack.DrawToggleBox (pos, "Passive:", editingDisplay.passive);
		pos.y += 1.5f * ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Activation Requirements");
		pos.y += 1.5f * ImagePack.fieldHeight;
		editingDisplay.activationCost = ImagePack.DrawField (pos, "Cost:", editingDisplay.activationCost);
		pos.y += ImagePack.fieldHeight;
		editingDisplay.activationCostType = ImagePack.DrawSelector (pos, "Cost Type:", editingDisplay.activationCostType, editingDisplay.activationCostTypeOptions);
		pos.x += pos.width;
		editingDisplay.activationLength = ImagePack.DrawField (pos, "Length:", editingDisplay.activationLength);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.casterEffectRequired = ImagePack.DrawCombobox (pos, "Caster Effect:", editingDisplay.casterEffectRequired, effectOptions);
		pos.x += pos.width;
		editingDisplay.casterEffectConsumed = ImagePack.DrawToggleBox (pos, "is Consumed?", editingDisplay.casterEffectConsumed);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.targetEffectRequired = ImagePack.DrawCombobox (pos, "Target Effect:", editingDisplay.targetEffectRequired, effectOptions);
		pos.x += pos.width;
		editingDisplay.targetEffectConsumed = ImagePack.DrawToggleBox (pos, "is Consumed?", editingDisplay.targetEffectConsumed);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int selectedItem = GetPositionOfItem (editingDisplay.reagentRequired);
		selectedItem = ImagePack.DrawSelector (pos, "Reagent:", selectedItem, itemsList);
		editingDisplay.reagentRequired = itemIds [selectedItem];
		pos.x += pos.width;
		editingDisplay.reagentConsumed = ImagePack.DrawToggleBox (pos, "is Consumed?", editingDisplay.reagentConsumed);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.weaponRequired = ImagePack.DrawSelector (pos, "Weapon:", editingDisplay.weaponRequired, weaponTypeOptions);
		/*pos.x += pos.width;
		editingDisplay.activationAnimation = ImagePack.DrawField (pos, "Animation:", editingDisplay.activationAnimation);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.activationParticles = ImagePack.DrawGameObject (pos, "Particles:", editingDisplay.activationParticles, 0.75f);*/
		pos.x += pos.width;
		editingDisplay.aoeRadius = ImagePack.DrawField (pos, "Effect Area:", editingDisplay.aoeRadius);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.maxRange = ImagePack.DrawField (pos, "Max. Range:", editingDisplay.maxRange);
		pos.x += pos.width;
		editingDisplay.minRange = ImagePack.DrawField (pos, "Min. Range:", editingDisplay.minRange);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.targetType = ImagePack.DrawSelector (pos, "Target Type:", editingDisplay.targetType, targetOptions);
		pos.x += pos.width;
		editingDisplay.targetState = ImagePack.DrawSelector (pos, "Target State:", editingDisplay.targetState, editingDisplay.targetStateOptions);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.speciesTargetReq = ImagePack.DrawSelector (pos, "Species Target:", editingDisplay.speciesTargetReq, speciesOptions);
		/*pos.x += pos.width;
		editingDisplay.specificTargetReq = ImagePack.DrawField (pos, "Specific Target:", editingDisplay.specificTargetReq);
		pos.x -= pos.width;*/
		pos.y += 1.5f * ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Cooldown Attributes");		
		pos.y += 1.5f * ImagePack.fieldHeight;
		editingDisplay.globalCooldown = ImagePack.DrawToggleBox (pos, "Global Cooldown?:", editingDisplay.globalCooldown);
		pos.x += pos.width;
		editingDisplay.weaponCooldown = ImagePack.DrawToggleBox (pos, "Weapon Cooldown?:", editingDisplay.weaponCooldown);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.cooldown1Type = ImagePack.DrawField (pos, "Cooldown Type:", editingDisplay.cooldown1Type);
		pos.x += pos.width;
		editingDisplay.cooldown1Duration = ImagePack.DrawField (pos, "Duration:", editingDisplay.cooldown1Duration);
		pos.x -= pos.width;
		pos.y += 1.5f * ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Effects");		
		pos.y += 1.5f * ImagePack.fieldHeight;
		editingDisplay.activationTarget1 = ImagePack.DrawSelector (pos, "Act. Target1:", editingDisplay.activationTarget1, editingDisplay.activationTarget1Options);
		pos.x += pos.width;
		int selectedEffect = GetPositionOfEffect(editingDisplay.activationEffect1);
		selectedEffect = ImagePack.DrawSelector (pos, "Act. Effect1:", selectedEffect, effectOptions);
		editingDisplay.activationEffect1 = effectIds[selectedEffect];
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.activationTarget2 = ImagePack.DrawSelector (pos, "Act. Target2:", editingDisplay.activationTarget2, editingDisplay.activationTarget2Options);
		pos.x += pos.width;
		selectedEffect = GetPositionOfEffect(editingDisplay.activationEffect2);
		selectedEffect = ImagePack.DrawSelector (pos, "Act. Effect2:", selectedEffect, effectOptions);
		editingDisplay.activationEffect2 = effectIds[selectedEffect];
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.activationTarget3 = ImagePack.DrawSelector (pos, "Act. Target3:", editingDisplay.activationTarget3, editingDisplay.activationTarget3Options);
		pos.x += pos.width;
		selectedEffect = GetPositionOfEffect(editingDisplay.activationEffect3);
		selectedEffect = ImagePack.DrawSelector (pos, "Act. Effect3:", selectedEffect, effectOptions);
		editingDisplay.activationEffect3 = effectIds[selectedEffect];
		pos.x -= pos.width;
		pos.y += 1.5f * ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Coordinated Effects");		
		pos.y += 1.5f * ImagePack.fieldHeight;
		editingDisplay.coordEffect1Event = ImagePack.DrawSelector (pos, "Effect Event 1:", editingDisplay.coordEffect1Event, editingDisplay.coordEffect1EventOptions);
		pos.x += pos.width;
		//editingDisplay.coordEffect1 = ImagePack.DrawField (pos, "Coord. Effect1:", editingDisplay.coordEffect1);
		editingDisplay.coordEffect1 = ImagePack.DrawSelector (pos, "Coord. Effect1:", editingDisplay.coordEffect1, coordList);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.coordEffect2Event = ImagePack.DrawSelector (pos, "Effect Event 2:", editingDisplay.coordEffect2Event, editingDisplay.coordEffect2EventOptions);
		pos.x += pos.width;
		//editingDisplay.coordEffect2 = ImagePack.DrawField (pos, "Coord. Effect2:", editingDisplay.coordEffect2);
		editingDisplay.coordEffect2 = ImagePack.DrawSelector (pos, "Coord. Effect2:", editingDisplay.coordEffect2, coordList);
		pos.x -= pos.width;
		
		pos.width *= 2;
		pos.y += 1.5f * ImagePack.fieldHeight;
		GUI.Label (pos, "Description:", ImagePack.FieldStyle ());
		pos.height *= 2;
		tooltipScroll = GUI.BeginScrollView(pos, tooltipScroll, new Rect(0, 0, pos.width * 0.75f, 100));
		editingDisplay.tooltip = GUI.TextArea (new Rect (115, 0, pos.width * 0.75f, 100), editingDisplay.tooltip, ImagePack.TextAreaStyle ());
		GUI.EndScrollView();
		pos.height /= 2;
		pos.width /= 2;

		pos.y += 2.2f*ImagePack.fieldHeight;
		pos.width *= 2;
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

		// If the insert failed, don't insert the spawn marker
		if (itemID != -1) {          
			// Update online table to avoid access the database again			
			editingDisplay.id = itemID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;

			// Configure the correponding prefab
			CreatePrefab ();
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

		// Configure the correponding prefab
		CreatePrefab ();
		NewResult("Entry updated");	
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
	}

	void CreatePrefab ()
	{
		// Configure the correponding prefab
		prefab = new AbilityPrefab (editingDisplay);
		prefab.Save (editingDisplay);
	}
	
	void DeletePrefab ()
	{
		prefab = new AbilityPrefab (editingDisplay);
		
		if (prefab.Load ())
			prefab.Delete ();
	}

	private int GetPositionOfSkill (int skillID)
	{
		for (int i = 0; i < skillIds.Length; i++) {
			if (skillIds [i] == skillID)
				return i;
		}
		return 0;
	}

	private int GetPositionOfEffect (int effectID)
	{
		for (int i = 0; i < effectIds.Length; i++) {
			if (effectIds [i] == effectID)
				return i;
		}
		return 0;
	}

	private int GetPositionOfItem (int itemId)
	{
		for (int i = 0; i < itemIds.Length; i++) {
			if (itemIds [i] == itemId)
				return i;
		}
		return 0;
	}
}
