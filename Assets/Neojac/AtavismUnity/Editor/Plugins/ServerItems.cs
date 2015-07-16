using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Item Configuration
public class ServerItems : AtavismDatabaseFunction
{

	public Dictionary<int, ItemData> dataRegister;
	public ItemData editingDisplay;
	public ItemData originalDisplay;

	public int[] abilityIds = new int[] {-1};
	public string[] abilityOptions = new string[] {"~ none ~"};
	
	public int[] currencyIds = new int[] {-1};
	public string[] currencyOptions = new string[] {"~ none ~"};
	
	public string[] itemTypeOptions = new string[] {"~ none ~"};
	public string[] weaponTypeOptions = new string[] {"~ none ~"};
	public string[] armorTypeOptions = new string[] {"~ none ~"};
	
	public string[] damageOptions = new string[] {"~ none ~"};
	
	public string[] itemEffectTypeOptions = new string[] {"~ none ~"};
	public string[] statOptions = new string[] {"~ none ~"};

	// Handles the prefab creation, editing and save
	private ItemPrefab item = null;
	
	// Use this for initialization
	public ServerItems ()
	{	
		functionName = "Items";		
		// Database tables name
		tableName = "item_templates";
		functionTitle = "Items Configuration";
		loadButtonLabel = "Load Items";
		notLoadedText = "No Item loaded.";
		// Init
		dataRegister = new Dictionary<int, ItemData> ();
					
		editingDisplay = new ItemData ();	
		originalDisplay = new ItemData ();
	}

	public override void Activate()
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
		if ((rows!=null) && (rows.Count > 0)) {
			abilityOptions = new string[rows.Count + 1];
			abilityOptions [optionsId] = "~ none ~"; 
			abilityIds = new int[rows.Count + 1];
			abilityIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				abilityOptions [optionsId] = data ["id"] + ":" + data ["name"]; 
				abilityIds[optionsId] = int.Parse(data ["id"]);
			}
		}
	}
	
	public void LoadCurrencyOptions ()
	{
		string query = "SELECT id, name FROM currencies ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows!=null) && (rows.Count > 0)) {
			currencyOptions = new string[rows.Count + 1];
			currencyOptions [optionsId] = "~ none ~"; 
			currencyIds = new int[rows.Count + 1];
			currencyIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				currencyOptions [optionsId] = data ["id"] + ":" + data ["name"]; 
				currencyIds[optionsId] = int.Parse(data ["id"]);
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
					ItemData display = new ItemData ();

					display.id = int.Parse (data ["id"]);
					display.name = data ["name"]; 
					display.icon = data ["icon"]; 
					display.category = data ["category"]; 
					display.subcategory = data ["subcategory"]; 
					display.itemType = data ["itemType"]; 
					display.subType = data ["subType"]; 
					display.slot = data ["slot"]; 
					display.display = data ["display"];
					display.itemQuality = int.Parse (data ["itemQuality"]); 
					display.binding = int.Parse (data ["binding"]); 
					display.isUnique = bool.Parse (data ["isUnique"]); 
					display.stackLimit = int.Parse (data ["stackLimit"]);
					display.purchaseCurrency = int.Parse (data ["purchaseCurrency"]); 
					display.purchaseCost = int.Parse (data ["purchaseCost"]);
					display.sellable = bool.Parse (data ["sellable"]); 
					display.levelReq = int.Parse (data ["levelReq"]);
					display.raceReq = data ["raceReq"]; 
					display.damage = int.Parse (data ["damage"]);
					display.damageType = data ["damageType"]; 
					display.delay = float.Parse (data ["delay"]);
					display.toolTip = data ["toolTip"]; 
					for (int i = 1; i <= display.maxEffectEntries; i++) {
						string effectType = data ["effect" + i + "type"]; 
						if (effectType != null && effectType != "") {
							string effectName = data ["effect" + i + "name"];
							string effectValue = data ["effect" + i + "value"];
							ItemEffectEntry entry = new ItemEffectEntry(effectType, effectName, effectValue);
							display.effects.Add(entry);
						}
					}
											
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create an Item before edit it.");		
			return;
		}
		

		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Items Configuration");
		
		//****
		if (newItemCreated) {
			newItemCreated = false;
			LoadSelectList ();
			newSelectedDisplay = displayKeys.Count - 1;
			// Debug.Log ("New Item Created:" + newSelectedDisplay);
		}
		
		// Draw data Editor
		if (newSelectedDisplay != selectedDisplay) {
			// Debug.Log ("(Load)I:" + newSelectedDisplay + "C:" + displayKeys.Count + "Dl:" + displayList.listBox.Length + "St:" + state);
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

	public override void CreateNewData ()
	{
		editingDisplay = new ItemData ();
		originalDisplay = new ItemData ();
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
			LoadAbilityOptions();
			LoadCurrencyOptions();
			LoadDamageOptions();
			LoadStatOptions();
			itemTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Item Type", false);
			weaponTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Weapon Type", false);
			armorTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Armor Type", false);
			itemEffectTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Item Effect Type", true);
			linkedTablesLoaded = true;
		}

		// Draw the content database info
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new item");
			pos.y += ImagePack.fieldHeight;
		}

		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.8f);
		pos.y += ImagePack.fieldHeight;
		pos.width /= 2;
		editingDisplay.itemType = ImagePack.DrawSelector (pos, "Item Type:", editingDisplay.itemType, itemTypeOptions);
		pos.x += pos.width;
		editingDisplay.icon = ImagePack.DrawTextureAsset (pos, "Icon:", editingDisplay.icon);		
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		if (editingDisplay.itemType == "Weapon")
			editingDisplay.subType = ImagePack.DrawSelector (pos, "Sub-Type:", editingDisplay.subType, weaponTypeOptions);
		else if (editingDisplay.itemType == "Armor")
			editingDisplay.subType = ImagePack.DrawSelector (pos, "Sub-Type:", editingDisplay.subType, armorTypeOptions);
		pos.width *= 2;
		pos.y += 1.3f*ImagePack.fieldHeight;
		if ((editingDisplay.itemType == "Weapon") || (editingDisplay.itemType == "Armor")) {
			pos.width *= 0.7f;
			editingDisplay.display = ImagePack.DrawGameObject(pos, "Equipment Display:", editingDisplay.display, 0.6f);
			pos.width /= 0.7f;
		}
		pos.width /= 2;
		pos.y += 1.5f*ImagePack.fieldHeight;
		/*editingDisplay.category = ImagePack.DrawField (pos, "Category:", editingDisplay.category);
		pos.x += pos.width;
		editingDisplay.subcategory = ImagePack.DrawField (pos, "Sub-category:", editingDisplay.subcategory);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;*/
		if ((editingDisplay.itemType == "Weapon") || (editingDisplay.itemType == "Armor")) {
			if (editingDisplay.itemType == "Weapon")
				editingDisplay.slot = ImagePack.DrawSelector (pos, "Slot:", editingDisplay.slot, editingDisplay.slotWeaponOptions);
			else
				editingDisplay.slot = ImagePack.DrawSelector (pos, "Slot:", editingDisplay.slot, editingDisplay.slotArmorOptions);

			if (editingDisplay.itemType == "Weapon") {
				pos.y += ImagePack.fieldHeight;
				editingDisplay.damageType = ImagePack.DrawSelector (pos, "Damage Type:", editingDisplay.damageType, damageOptions); 
				pos.x += pos.width;
				editingDisplay.damage = ImagePack.DrawField (pos, "Damage:", editingDisplay.damage);
				pos.x -= pos.width;
				pos.y += ImagePack.fieldHeight;				
				editingDisplay.delay = ImagePack.DrawField (pos, "Delay:", editingDisplay.delay);
			}
		pos.y += ImagePack.fieldHeight;
		}
		editingDisplay.itemQuality = ImagePack.DrawSelector (pos, "Item Quality:", editingDisplay.itemQuality - 1, editingDisplay.itemQualityOptions) + 1;
		pos.x += pos.width;
		editingDisplay.binding = ImagePack.DrawSelector (pos, "Binding:", editingDisplay.binding, editingDisplay.bindingOptions);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.purchaseCost = ImagePack.DrawField (pos, "Purchase Cost:", editingDisplay.purchaseCost);
		pos.x += pos.width;
		int selectedCurrency = GetPositionOfCurrency(editingDisplay.purchaseCurrency);
		selectedCurrency = ImagePack.DrawSelector (pos, "Currency:", selectedCurrency, currencyOptions);
		editingDisplay.purchaseCurrency = currencyIds[selectedCurrency];
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.sellable = ImagePack.DrawToggleBox (pos, "Is Item Sellable?", editingDisplay.sellable);
		pos.x += pos.width;
		editingDisplay.isUnique = ImagePack.DrawToggleBox (pos, "Is Item Unique?", editingDisplay.isUnique);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.stackLimit = ImagePack.DrawField (pos, "Stack Limit:", editingDisplay.stackLimit);
		pos.x += pos.width;
		editingDisplay.levelReq = ImagePack.DrawField (pos, "Level Required:", editingDisplay.levelReq);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		//editingDisplay.raceReq = ImagePack.DrawField (pos, "Use Rate:", editingDisplay.raceReq);
		//pos.x += pos.width;
		//pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		pos.width *= 2;
		editingDisplay.toolTip = ImagePack.DrawField (pos, "Tool Tip:", editingDisplay.toolTip, 0.75f, 60);
		
		pos.y += 2.5f*ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos. x, pos.y, "Item Effects");
		pos.y += ImagePack.fieldHeight;
		
		for (int i = 0; i < editingDisplay.maxEffectEntries; i++) {
			if (editingDisplay.effects.Count > i) {
				pos.width = pos.width / 2;
				editingDisplay.effects[i].itemEffectType = ImagePack.DrawSelector (pos, "Effect " + (i+1) 
					+ " Type:", editingDisplay.effects[i].itemEffectType, itemEffectTypeOptions);
				pos.x += pos.width;
				
				if (editingDisplay.effects[i].itemEffectType == "Stat") {
					pos.x -= pos.width;
					pos.y += ImagePack.fieldHeight;
					editingDisplay.effects[i].itemEffectName = ImagePack.DrawSelector (pos, "Stat:", editingDisplay.effects[i].itemEffectName, statOptions);
					pos.x += pos.width;
					if (editingDisplay.effects[i].itemEffectValue == "")
						editingDisplay.effects[i].itemEffectValue = "0";
					editingDisplay.effects[i].itemEffectValue = ImagePack.DrawField (pos, "Value:", editingDisplay.effects[i].itemEffectValue);
				} else if (editingDisplay.effects[i].itemEffectType == "UseAbility") {
					if (editingDisplay.effects[i].itemEffectValue == "")
						editingDisplay.effects[i].itemEffectValue = "0";
					int selectedAbility = GetPositionOfAbility(int.Parse(editingDisplay.effects[i].itemEffectValue));
					selectedAbility = ImagePack.DrawSelector (pos, "Ability:", selectedAbility, abilityOptions);
					editingDisplay.effects[i].itemEffectValue = abilityIds[selectedAbility].ToString();
				} else if (editingDisplay.effects[i].itemEffectType == "AutoAttack") {
					if (editingDisplay.effects[i].itemEffectValue == "")
						editingDisplay.effects[i].itemEffectValue = "0";
					int selectedAbility = GetPositionOfAbility(int.Parse(editingDisplay.effects[i].itemEffectValue));
					selectedAbility = ImagePack.DrawSelector (pos, "Ability:", selectedAbility, abilityOptions);
					editingDisplay.effects[i].itemEffectValue = abilityIds[selectedAbility].ToString();
				} else if (editingDisplay.effects[i].itemEffectType == "Currency") {
					if (editingDisplay.effects[i].itemEffectValue == "")
						editingDisplay.effects[i].itemEffectValue = "0";
					selectedCurrency = GetPositionOfCurrency(int.Parse(editingDisplay.effects[i].itemEffectValue));
					selectedCurrency = ImagePack.DrawSelector (pos, "Currency:", selectedCurrency, currencyOptions);
					editingDisplay.effects[i].itemEffectValue = currencyIds[selectedCurrency].ToString();
				} else if (editingDisplay.effects[i].itemEffectType == "ClaimObject") {
					pos.y += ImagePack.fieldHeight;
					pos.x -= pos.width;
					pos.width *= 2;
					editingDisplay.effects[i].itemEffectValue = ImagePack.DrawGameObject (pos, "Game Object:", editingDisplay.effects[i].itemEffectValue, 1.0f);
					pos.width /= 2;
					pos.x += pos.width;
				} else if (editingDisplay.effects[i].itemEffectType == "CreateClaim") {
					editingDisplay.effects[i].itemEffectValue = ImagePack.DrawField (pos, "Size:", editingDisplay.effects[i].itemEffectValue);
				} else {
					editingDisplay.effects[i].itemEffectValue = ImagePack.DrawField (pos, "Value:", editingDisplay.effects[i].itemEffectValue);
				}
				
				pos.y += ImagePack.fieldHeight;
				if (ImagePack.DrawButton (pos.x, pos.y, "Remove Effect")) {
					editingDisplay.effects.RemoveAt(i);
				}
				pos.x -= pos.width;
				//pos.x -= pos.width;
				pos.y += ImagePack.fieldHeight;
				pos.width = pos.width * 2;
			}
		}
		if (editingDisplay.effects.Count < editingDisplay.maxEffectEntries) {
			if (ImagePack.DrawButton (pos.x, pos.y, "Add Item Effect")) {
				ItemEffectEntry itemEffectEntry = new ItemEffectEntry("", "", "");
				editingDisplay.effects.Add(itemEffectEntry);
			}
		}
		
		// Save data		
		pos.x -= ImagePack.innerMargin;
		pos.y += 3f * ImagePack.fieldHeight;
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
				LoadSelectList ();
				newSelectedDisplay = 0;
				// Debug.Log ("(Delete)I:" + newSelectedDisplay);
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
			// Update online table to avoid access the database again			
			editingDisplay.id = itemID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;

			// Configure the correponding prefab
			CreatePrefab();
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
		
		// Remove the prefab
		DeletePrefab();
		// Configure the correponding prefab
		CreatePrefab();
		NewResult("Entry updated");	
	}
	
	// Delete entries from the table
	void DeleteEntry ()
	{
		// Remove the prefab
		DeletePrefab();
		// Delete the database entry
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

	void CreatePrefab()
	{
		// Configure the correponding prefab
		item = new ItemPrefab(editingDisplay);
		item.Save(editingDisplay);
	}

	void DeletePrefab()
	{
		item = new ItemPrefab(editingDisplay);

		if (item.Load())
			item.Delete();
	}

	private int GetPositionOfAbility(int abilityID) {
		for (int i = 0; i < abilityIds.Length; i++) {
			if (abilityIds[i] == abilityID)
				return i;
		}
		return 0;
	}
	
	private int GetPositionOfCurrency(int currencyID) {
		for (int i = 0; i < currencyIds.Length; i++) {
			if (currencyIds[i] == currencyID)
				return i;
		}
		return 0;
	}
}
