using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Effects Configuration
public class ServerEffects : AtavismDatabaseFunction
{

	public Dictionary<int, EffectsData> dataRegister;
	public EffectsData editingDisplay;
	public EffectsData originalDisplay;
	public int[] skillIds = new int[] {-1};
	public string[] skillOptions = new string[] {"~ none ~"};
	public int[] effectIds = new int[] {-1};
	public string[] effectOptions = new string[] {"~ none ~"};
	public string[] damageOptions = new string[] {"~ none ~"};
	public string[] statOptions = new string[] {"~ none ~"};
	public string[] vitalityStatOptions = new string[] {"~ none ~"};

	// Use this for initialization
	public ServerEffects ()
	{	
		functionName = "Effects";
		// Database tables name
		tableName = "effects";
		functionTitle = "Effects Configuration";
		loadButtonLabel = "Load Effects";
		notLoadedText = "No Effect loaded.";
		// Init
		dataRegister = new Dictionary<int, EffectsData> ();

		editingDisplay = new EffectsData ();
		originalDisplay = new EffectsData ();
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
			skillIds [optionsId] = -1;
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
				effectIds [optionsId] = -1;
				foreach (Dictionary<string,string> data in rows) {
					optionsId++;
					effectOptions [optionsId] = data ["name"]; 
					effectIds [optionsId] = int.Parse (data ["id"]);
				}
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
	
	public void LoadVitalityStatOptions ()
	{
		if (!dataLoaded) {
			// Read all entries from the table
			string query = "SELECT name FROM stat where type = 2";
			
			// If there is a row, clear it.
			if (rows != null)
				rows.Clear ();
			
			// Load data
			rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
			//Debug.Log("#Rows:"+rows.Count);
			// Read all the data
			int optionsId = 0;
			if ((rows != null) && (rows.Count > 0)) {
				vitalityStatOptions = new string[rows.Count];
				//vitalityStatOptions [optionsId] = "~ none ~"; 
				foreach (Dictionary<string,string> data in rows) {
					vitalityStatOptions [optionsId] = data ["name"]; 
					optionsId++;
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
					EffectsData display = new EffectsData ();

					display.id = int.Parse (data ["id"]);
					display.name = data ["name"]; 

					//display.displayName = data ["displayName"];
					display.icon = data ["icon"];
					display.effectMainType = data ["effectMainType"];
					display.effectType = data ["effectType"];
					//display.effectFamily = int.Parse (data ["effectFamily"]);
					display.isBuff = bool.Parse (data ["isBuff"]);
					display.skillType = int.Parse (data ["skillType"]);
					display.passive = bool.Parse (data ["passive"]);
					display.stackLimit = int.Parse (data ["stackLimit"]);
					display.allowMultiple = bool.Parse (data ["allowMultiple"]);
					display.duration = float.Parse (data ["duration"]);
					display.pulseCount = int.Parse (data ["pulseCount"]);
					display.tooltip = data ["tooltip"];
					display.bonusEffectReq = int.Parse (data ["bonusEffectReq"]);
					display.bonusEffectReqConsumed = bool.Parse (data ["bonusEffectReqConsumed"]);
					display.bonusEffect = int.Parse (data ["bonusEffect"]);
					display.pulseParticle = data ["pulseParticle"];

					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList ();
			}
			dataLoaded = true;
		}

		// Load additional data based on the main effect type
		foreach (EffectsData effectData in dataRegister.Values) {
			if (effectData.effectMainType == "Damage") {
				LoadDamageEffectData (effectData);
			} else if (effectData.effectMainType == "Heal" || effectData.effectMainType == "Restore") {
				LoadHealEffectData (effectData);
			} else if (effectData.effectMainType == "Stat") {
				LoadStatEffectData (effectData);
			}
		}
	}
	
	// Load Database Data
	public void LoadDamageEffectData (EffectsData effectData)
	{
		// Read all entries from the table
		string query = "SELECT * FROM damage_effects where id = " + effectData.id;
			
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
			
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows != null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) { 
				effectData.damageAmount = int.Parse (data ["damageAmount"]);
				effectData.damageType = data ["damageType"];
				effectData.damageProperty = data ["damageProperty"];
				effectData.damageMod = float.Parse (data ["damageMod"]);
				effectData.bonusDamageEffect = int.Parse (data ["bonusDamageEffect"]);
				effectData.bonusDamageAmount = int.Parse (data ["bonusDamageAmount"]);
				effectData.healthTransferRate = int.Parse (data ["healthTransferRate"]);
			}
		}
	}

	// Load Database Data
	public void LoadHealEffectData (EffectsData effectData)
	{
		// Read all entries from the table
		string query = "SELECT * FROM heal_effects where id = " + effectData.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows != null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) { 
				effectData.healAmount = int.Parse (data ["healAmount"]);
				effectData.healProperty = data ["healProperty"];
				effectData.healthTransferRate = int.Parse (data ["healthTransferRate"]);
			}
		}
	}
	
	public void LoadStatEffectData (EffectsData effectData)
	{
		// Read all entries from the table
		string query = "SELECT * FROM stat_effects where id = " + effectData.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows != null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) { 
				effectData.modifyStatsByPercent = bool.Parse (data ["modifyStatsByPercent"]);
				effectData.stat1Name = data ["stat1Name"];
				effectData.stat1Modification = float.Parse (data ["stat1Modification"]);
				effectData.stat2Name = data ["stat2Name"];
				effectData.stat2Modification = float.Parse (data ["stat2Modification"]);
				effectData.stat3Name = data ["stat3Name"];
				effectData.stat3Modification = float.Parse (data ["stat3Modification"]);
				effectData.stat4Name = data ["stat4Name"];
				effectData.stat4Modification = float.Parse (data ["stat4Modification"]);
				effectData.stat5Name = data ["stat5Name"];
				effectData.stat5Modification = float.Parse (data ["stat5Modification"]);
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create an Effect before edit it.");		
			return;
		}
		
		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Effects Configuration");

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
		editingDisplay = new EffectsData ();		
		originalDisplay = new EffectsData ();
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
		if (!linkedTablesLoaded) {	
			LoadSkillOptions ();
			LoadEffectOptions ();
			LoadDamageOptions ();
			LoadStatOptions ();
			LoadVitalityStatOptions();
			linkedTablesLoaded = true;
		}

		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Effect");		
			pos.y += ImagePack.fieldHeight;
		}

		int selectedEffect = -1;
		
		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);
		pos.y += ImagePack.fieldHeight;
		//editingDisplay.displayName = ImagePack.DrawField (pos, "Display Name:", editingDisplay.displayName, 0.75f);
		//pos.y += ImagePack.fieldHeight;
		//editingDisplay.icon = ImagePack.DrawField (pos, "Icon Name:", editingDisplay.icon, 0.75f);
		pos.width /= 2;
		// We only allowing changing of the effect main type when it is a new item
		if (newItem)
			editingDisplay.effectMainType = ImagePack.DrawSelector (pos, "Effect Type:", editingDisplay.effectMainType, editingDisplay.effectMainTypeOptions);
		else
			ImagePack.DrawText (pos, "Effect Type: " + editingDisplay.effectMainType);
		pos.x += pos.width;
		editingDisplay.icon = ImagePack.DrawTextureAsset (pos, "Icon:", editingDisplay.icon);		
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight * 1.5f;
		bool showTimeFields = true;
		switch (editingDisplay.effectMainType) {
		case "Damage":
			editingDisplay.effectType = ImagePack.DrawSelector (pos, "Effect Subtype:", editingDisplay.effectType, editingDisplay.effectDamageTypeOptions);
			//editingDisplay.effectType = editingDisplay.effectDamageType + "Effect";
			if (!editingDisplay.effectType.Contains("Dot"))
				showTimeFields = false;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.damageProperty = ImagePack.DrawSelector (pos, "Damage Property:", editingDisplay.damageProperty, vitalityStatOptions);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.damageType = ImagePack.DrawSelector (pos, "Damage Type:", editingDisplay.damageType, damageOptions);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.damageAmount = ImagePack.DrawField (pos, "Damage Amount:", editingDisplay.damageAmount);
			pos.x += pos.width;
			editingDisplay.damageMod = ImagePack.DrawField (pos, "Damage Modifier:", editingDisplay.damageMod);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.healthTransferRate = ImagePack.DrawField (pos, "Transfer Rate:", editingDisplay.healthTransferRate);
			pos.x += pos.width;
			selectedEffect = GetPositionOfEffect (editingDisplay.bonusDamageEffect);
			selectedEffect = ImagePack.DrawSelector (pos, "Bonus Dmg Effect Req:", selectedEffect, effectOptions);
			editingDisplay.bonusDamageEffect = effectIds [selectedEffect];
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.bonusDamageAmount = ImagePack.DrawField (pos, "Bonus Damage Amount:", editingDisplay.bonusDamageAmount);
			pos.y += ImagePack.fieldHeight;
			break;
		case "Restore":	
		case "Heal":
			editingDisplay.effectType = ImagePack.DrawSelector (pos, "Restore Type:", editingDisplay.effectType, editingDisplay.effectHealTypeOptions);
			//editingDisplay.effectType = editingDisplay.effectHealType + "Effect";
			if (editingDisplay.effectType.Contains("Instant"))
				showTimeFields = false;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.healAmount = ImagePack.DrawField (pos, "Restore Amount:", editingDisplay.healAmount);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.healProperty = ImagePack.DrawSelector (pos, "Restore Property:", editingDisplay.healProperty, vitalityStatOptions);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.healthTransferRate = ImagePack.DrawField (pos, "Transfer Rate:", editingDisplay.healthTransferRate);
			pos.y += ImagePack.fieldHeight;
			break;
		case "Stat":
			showTimeFields = true;
			editingDisplay.effectType = ImagePack.DrawSelector (pos, "Stat Type:", editingDisplay.effectType, editingDisplay.effectStatTypeOptions);
			//editingDisplay.effectType = editingDisplay.effectStatType + "Effect";
			pos.y += ImagePack.fieldHeight;
			editingDisplay.modifyStatsByPercent = ImagePack.DrawToggleBox (pos, "Modify By Percent:", editingDisplay.modifyStatsByPercent);
			pos.y += ImagePack.fieldHeight;
			editingDisplay.stat1Name = ImagePack.DrawSelector (pos, "Stat 1:", editingDisplay.stat1Name, statOptions);
			pos.x += pos.width;
			editingDisplay.stat1Modification = ImagePack.DrawField (pos, "Modification:", editingDisplay.stat1Modification);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.stat2Name = ImagePack.DrawSelector (pos, "Stat 2:", editingDisplay.stat2Name, statOptions);
			pos.x += pos.width;
			editingDisplay.stat2Modification = ImagePack.DrawField (pos, "Modification:", editingDisplay.stat2Modification);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.stat3Name = ImagePack.DrawSelector (pos, "Stat 3:", editingDisplay.stat3Name, statOptions);
			pos.x += pos.width;
			editingDisplay.stat3Modification = ImagePack.DrawField (pos, "Modification:", editingDisplay.stat3Modification);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.stat4Name = ImagePack.DrawSelector (pos, "Stat 4:", editingDisplay.stat4Name, statOptions);
			pos.x += pos.width;
			editingDisplay.stat4Modification = ImagePack.DrawField (pos, "Modification:", editingDisplay.stat4Modification);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.stat5Name = ImagePack.DrawSelector (pos, "Stat 5:", editingDisplay.stat5Name, statOptions);
			pos.x += pos.width;
			editingDisplay.stat5Modification = ImagePack.DrawField (pos, "Modification:", editingDisplay.stat5Modification);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			break;
		default:
			editingDisplay.effectType = "";
			break;
		}

		//editingDisplay.effectFamily = ImagePack.DrawField (pos, "Family:", editingDisplay.effectFamily);
		//
		editingDisplay.isBuff = ImagePack.DrawToggleBox (pos, "Is Buff?", editingDisplay.isBuff);
		pos.x += pos.width;
		editingDisplay.passive = ImagePack.DrawToggleBox (pos, "Is Passive?", editingDisplay.passive);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int selectedSkill = GetPositionOfSkill (editingDisplay.skillType);
		selectedSkill = ImagePack.DrawSelector (pos, "Skill Type:", selectedSkill, skillOptions);
		editingDisplay.skillType = skillIds [selectedSkill];
		pos.x += pos.width;
		editingDisplay.skillLevelMod = ImagePack.DrawField (pos, "Skill Mod:", editingDisplay.skillLevelMod);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		
		if (showTimeFields) {
			editingDisplay.stackLimit = ImagePack.DrawField (pos, "Stack Limit:", editingDisplay.stackLimit);
			pos.x += pos.width;
			editingDisplay.allowMultiple = ImagePack.DrawToggleBox (pos, "Allow Multiple?", editingDisplay.allowMultiple);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
		
			editingDisplay.duration = ImagePack.DrawField (pos, "Duration:", editingDisplay.duration);
			pos.x += pos.width;
			editingDisplay.pulseCount = ImagePack.DrawField (pos, "Num. Pulses", editingDisplay.pulseCount);
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			pos.width *= 2;
			pos.width /= 3;
			pos.width *= 2;
			editingDisplay.pulseParticle = ImagePack.DrawGameObject (pos, "Pulse Particle:", editingDisplay.pulseParticle, 0.7f);
			pos.width /= 2;
			//pos.x -= pos.width;
			pos.width *= 3;
			pos.y += ImagePack.fieldHeight;
			
		} else {
			pos.width *= 2;
		}
		
		editingDisplay.tooltip = ImagePack.DrawField (pos, "Tooltip:", editingDisplay.tooltip, 0.75f, 60);
		pos.y += 3 * ImagePack.fieldHeight;

		ImagePack.DrawLabel (pos.x, pos.y, "Bonus Effects");		
		pos.y += ImagePack.fieldHeight;
		pos.width /= 2;
		selectedEffect = GetPositionOfEffect (editingDisplay.bonusEffectReq);
		selectedEffect = ImagePack.DrawSelector (pos, "Required Effect:", selectedEffect, effectOptions);
		editingDisplay.bonusEffectReq = effectIds [selectedEffect];
		pos.x += pos.width;
		selectedEffect = GetPositionOfEffect (editingDisplay.bonusEffect);
		selectedEffect = ImagePack.DrawSelector (pos, "Bonus Effect:", selectedEffect, effectOptions);
		editingDisplay.bonusEffect = effectIds [selectedEffect];
		pos.x -= pos.width;		
		pos.y += ImagePack.fieldHeight;
		editingDisplay.bonusEffectReqConsumed = ImagePack.DrawToggleBox (pos, "Is Consumed?", editingDisplay.bonusEffectReqConsumed);
		pos.width *= 2;

		pos.y += ImagePack.fieldHeight;
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
			// Have to update the relevant table
			if (editingDisplay.effectMainType == "Damage") {
				InsertDamageEffect (editingDisplay);
			} else if (editingDisplay.effectMainType == "Heal" || editingDisplay.effectMainType == "Restore") {
				InsertHealEffect (editingDisplay);
			} else if (editingDisplay.effectMainType == "Stat") {
				InsertStatEffect (editingDisplay);
			}
			// Update online table to avoid access the database again			
			editingDisplay.id = itemID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;
			LoadEffectOptions ();
			NewResult ("New entry inserted");
		} else {
			NewResult ("Error occurred, please check the Console");
		}
	}

	void InsertDamageEffect (EffectsData effectData)
	{
		EffectsDamageData damageData = new EffectsDamageData ();
		damageData.LoadEffectData (effectData);
		// Setup the update query
		string query = "INSERT INTO damage_effects";		
		query += " (" + damageData.FieldList ("", ", ") + ") ";
		query += "VALUES ";
		query += " (" + damageData.FieldList ("?", ", ") + ") ";

		int itemID = -1;
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in damageData.fields.Keys) {
			update.Add (damageData.fieldToRegister (field));
		}
		
		// Update the database
		itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);
	}

	void InsertHealEffect (EffectsData effectData)
	{
		EffectsHealData healData = new EffectsHealData ();
		healData.LoadEffectData (effectData);
		// Setup the update query
		string query = "INSERT INTO heal_effects";		
		query += " (" + healData.FieldList ("", ", ") + ") ";
		query += "VALUES ";
		query += " (" + healData.FieldList ("?", ", ") + ") ";
		
		int itemID = -1;
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in healData.fields.Keys) {
			update.Add (healData.fieldToRegister (field));
		}
		
		// Update the database
		itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);
	}
	
	void InsertStatEffect (EffectsData effectData)
	{
		EffectsStatData statData = new EffectsStatData ();
		statData.LoadEffectData (effectData);
		// Setup the update query
		string query = "INSERT INTO stat_effects";		
		query += " (" + statData.FieldList ("", ", ") + ") ";
		query += "VALUES ";
		query += " (" + statData.FieldList ("?", ", ") + ") ";
		
		int itemID = -1;
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in statData.fields.Keys) {
			update.Add (statData.fieldToRegister (field));
		}
		
		// Update the database
		itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);
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

		// Update the additional table based on effect main type
		if (editingDisplay.effectMainType == "Damage") {
			UpdateDamageEntry (editingDisplay);
		} else if (editingDisplay.effectMainType == "Heal" || editingDisplay.effectMainType == "Restore") {
			UpdateHealEntry (editingDisplay);
		} else if (editingDisplay.effectMainType == "Stat") {
			UpdateStatEntry (editingDisplay);
		}
		
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;	
		NewResult ("Entry updated");			
	}

	// Update existing entries in the table based on the iddemo_table
	void UpdateDamageEntry (EffectsData effectData)
	{
		EffectsDamageData damageData = new EffectsDamageData ();
		damageData.LoadEffectData (effectData);
		// Setup the update query
		string query = "UPDATE damage_effects";
		query += " SET ";
		query += damageData.UpdateList ();
		query += " WHERE id=?id";
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in damageData.fields.Keys) {
			if (field != "id")
				update.Add (damageData.fieldToRegister (field));       
		}
		update.Add (new Register ("id", "?id", MySqlDbType.Int32, damageData.id.ToString (), Register.TypesOfField.Int));
		
		// Update the database
		DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
	}

	// Update existing entries in the table based on the iddemo_table
	void UpdateHealEntry (EffectsData effectData)
	{
		EffectsHealData healData = new EffectsHealData ();
		healData.LoadEffectData (effectData);
		// Setup the update query
		string query = "UPDATE heal_effects";
		query += " SET ";
		query += healData.UpdateList ();
		query += " WHERE id=?id";
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in healData.fields.Keys) {
			if (field != "id")
				update.Add (healData.fieldToRegister (field));       
		}
		update.Add (new Register ("id", "?id", MySqlDbType.Int32, healData.id.ToString (), Register.TypesOfField.Int));
		
		// Update the database
		DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
	}
	
	// Update existing entries in the table based on the iddemo_table
	void UpdateStatEntry (EffectsData effectData)
	{
		EffectsStatData statData = new EffectsStatData ();
		statData.LoadEffectData (effectData);
		// Setup the update query
		string query = "UPDATE stat_effects";
		query += " SET ";
		query += statData.UpdateList ();
		query += " WHERE id=?id";
		
		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in statData.fields.Keys) {
			if (field != "id")
				update.Add (statData.fieldToRegister (field));       
		}
		update.Add (new Register ("id", "?id", MySqlDbType.Int32, statData.id.ToString (), Register.TypesOfField.Int));
		
		// Update the database
		DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
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

		// Delete entry in the extension table based on effect main type
		if (editingDisplay.effectMainType == "Damage") {
			delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
			DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "damage_effects", delete);
		} else if (editingDisplay.effectMainType == "Heal" || editingDisplay.effectMainType == "Restore") {
			delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
			DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "heal_effects", delete);
		} else if (editingDisplay.effectMainType == "Stat") {
			delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
			DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "stat_effects", delete);
		}
		
		LoadEffectOptions ();
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
}
