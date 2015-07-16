using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Quests Configuration
public class ServerQuests : AtavismDatabaseFunction
{

	public Dictionary<int, QuestsData> dataRegister;
	public QuestsData editingDisplay;
	public QuestsData originalDisplay;

	public int[] factionIds = new int[] {-1};
	public string[] factionOptions = new string[] {"~ none ~"};
	public int[] itemIds = new int[] {-1};
	public string[] itemsList = new string[] {"~ none ~"};
	public int[] questIds = new int[] {-1};
	public string[] questOptions = new string[] {"~ none ~"};
	public string[] raceOptions = new string[] {"~ none ~"};
	public string[] aspectOptions = new string[] {"~ none ~"};
	public int[] skillIds = new int[] {-1};
	public string[] skillOptions = new string[] {"~ none ~"};
	public int[] mobIds = new int[] {-1};
	public string[] mobList = new string[] {"~ none ~"};
	
	public string[] objectiveTypeOptions = new string[] {"~ none ~"};

	Vector2 descriptionScroll = new Vector2();
	Vector2 objectiveScroll = new Vector2();
	Vector2 progressScroll = new Vector2();
	Vector2 completionScroll = new Vector2();
	
	// Use this for initialization
	public ServerQuests ()
	{	
		functionName = "Quests";		
		// Database tables name
		tableName = "quests";
		functionTitle = "Quests Configuration";
		loadButtonLabel = "Load Quests";
		notLoadedText = "No Quest loaded.";
		// Init
		dataRegister = new Dictionary<int, QuestsData> ();

		editingDisplay = new QuestsData ();		
		originalDisplay = new QuestsData ();
	} 

	public override void Activate()
	{
		linkedTablesLoaded = false;
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

	public void LoadQuestOptions ()
	{
		string query = "SELECT id, name FROM quests ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows != null) && (rows.Count > 0)) {
			questOptions = new string[rows.Count + 1];
			questOptions [optionsId] = "~ none ~"; 
			questIds = new int[rows.Count + 1];
			questIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				questOptions [optionsId] = data ["id"] + ":" + data ["name"]; 
				questIds [optionsId] = int.Parse (data ["id"]);
			}
		}
	}

	public void LoadAspectOptions ()
	{
		if (!dataLoaded) {
			// Read all entries from the table
			string query = "SELECT name FROM aspect";
			
			// If there is a row, clear it.
			if (rows != null)
				rows.Clear ();
			
			// Load data
			rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
			//Debug.Log("#Rows:"+rows.Count);
			// Read all the data
			int optionsId = 0;
			if ((rows!=null) && (rows.Count > 0)) {
				aspectOptions = new string[rows.Count + 1];
				aspectOptions[optionsId] = "~ none ~"; 
				foreach (Dictionary<string,string> data in rows) {
					optionsId++;
					aspectOptions[optionsId] = data ["name"]; 
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
			itemIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				itemsList [optionsId] = data ["id"] + ":" + data ["name"]; 
				itemIds [optionsId] = int.Parse (data ["id"]);
			}
		}
	}

	private void LoadMobList ()
	{
		string query = "SELECT id, name FROM mob_templates ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows != null) && (rows.Count > 0)) {
			mobList = new string[rows.Count + 1];
			mobList [optionsId] = "~ none ~"; 
			mobIds = new int[rows.Count + 1];
			mobIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				mobList [optionsId] = data ["id"] + ":" + data ["name"]; 
				mobIds [optionsId] = int.Parse (data ["id"]);
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
					QuestsData display = new QuestsData ();

					display.id = int.Parse (data ["id"]);
					display.name = data ["name"]; 
										
					display.category = int.Parse (data ["category"]);
					display.faction = int.Parse (data ["faction"]);
					display.chain = data ["chain"];
					display.level = int.Parse (data ["level"]);
					display.zone = data ["zone"];
					display.numGrades = int.Parse (data ["numGrades"]);
					display.repeatable = bool.Parse (data ["repeatable"]);
					display.description = data ["description"];
					display.objectiveText = data ["objectiveText"];
					display.progressText = data ["progressText"];
					display.deliveryItem1 = int.Parse (data ["deliveryItem1"]);
					display.deliveryItem2 = int.Parse (data ["deliveryItem2"]);
					display.deliveryItem3 = int.Parse (data ["deliveryItem3"]);
					display.questPrereq = int.Parse (data ["questPrereq"]);
					display.questStartedReq = int.Parse (data ["questStartedReq"]);
					display.levelReq = int.Parse (data ["levelReq"]);
					display.raceReq = data ["raceReq"];
					display.aspectReq = data ["aspectReq"];
					display.skillReq = int.Parse (data ["skillReq"]);
					display.skillLevelReq = int.Parse (data ["skillLevelReq"]);
					display.repReq = int.Parse (data ["repReq"]);
					display.repLevelReq = int.Parse (data ["repLevelReq"]);
					display.completionText = data ["completionText"]; 
					display.experience = int.Parse (data ["experience"]);
					display.item1 = int.Parse (data ["item1"]);
					display.item1count = int.Parse (data ["item1count"]);
					display.item2 = int.Parse (data ["item2"]);
					display.item2count = int.Parse (data ["item2count"]);
					display.item3 = int.Parse (data ["item3"]);
					display.item3count = int.Parse (data ["item3count"]);
					display.item4 = int.Parse (data ["item4"]);
					display.item4count = int.Parse (data ["item4count"]);
					display.chooseItem1 = int.Parse (data ["chooseItem1"]);
					display.chooseItem1count = int.Parse (data ["chooseItem1count"]);
					display.chooseItem2 = int.Parse (data ["chooseItem2"]);
					display.chooseItem2count = int.Parse (data ["chooseItem2count"]);
					display.chooseItem3 = int.Parse (data ["chooseItem3"]);
					display.chooseItem3count = int.Parse (data ["chooseItem3count"]);
					display.chooseItem4 = int.Parse (data ["chooseItem4"]);
					display.chooseItem4count = int.Parse (data ["chooseItem4count"]);
					display.currency = int.Parse (data ["currency1"]);
					display.currencyCount = int.Parse (data ["currency1count"]);
					display.currency2 = int.Parse (data ["currency2"]);
					display.currency2count = int.Parse (data ["currency2count"]);
					display.rep1 = int.Parse (data ["rep1"]);
					display.rep1gain = int.Parse (data ["rep1gain"]);
					display.rep2 = int.Parse (data ["rep2"]);
					display.rep2gain = int.Parse (data ["rep2gain"]);

					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList ();
			}
			dataLoaded = true;
		}
		foreach(QuestsData questData in dataRegister.Values) {
			LoadQuestObjectives(questData);
		}
	}

	void LoadQuestObjectives(QuestsData questData)
	{
		// Read all entries from the table
		string query = "SELECT * FROM " + "quest_objectives" + " where questID = " + questData.id;
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		//Debug.Log("#Rows:"+rows.Count);
		// Read all the data
		if ((rows!=null) && (rows.Count > 0)) {
			foreach (Dictionary<string,string> data in rows) {
				QuestsObjectivesData entry = new QuestsObjectivesData ();
				
				entry.id = int.Parse (data ["id"]);
				entry.objectiveType = data ["objectiveType"]; 
				entry.target = int.Parse (data ["target"]);
				entry.targetCount = int.Parse (data ["targetCount"]);
				entry.targetText = data ["targetText"]; 
				questData.questObjectives.Add(entry);
			}
		}
	}
	
	public void LoadSelectList ()
	{
		displayList = new string[dataRegister.Count];
		int i = 0;
		foreach (int displayID in dataRegister.Keys) {
			displayList [i] = displayID + ". " + dataRegister [displayID].name;
			i++;
		}
	}
	
	// Draw the loaded list
	public override  void DrawLoaded (Rect box)
	{	
		
		breadCrumb = "";

		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;
								
		if (dataRegister.Count <= 0) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawLabel (pos.x, pos.y, "You must create a Quest before edit it.");		
			return;
		}
		

		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Quests Configuration");
		

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
		editingDisplay = new QuestsData ();		
		originalDisplay = new QuestsData ();
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
			LoadFactionOptions();
			LoadQuestOptions();
			LoadAspectOptions();
			LoadSkillOptions();
			LoadItemList ();
			LoadMobList();
			objectiveTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Quest Objective Type", false);
			raceOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Race", true);
			linkedTablesLoaded = true;
		}

		// Draw the content database info
		
		if (newItem) {
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Quest");
			pos.y += 1f*ImagePack.fieldHeight;
		}
		pos.y += 0.5f*ImagePack.fieldHeight;

		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);
		pos.y += ImagePack.fieldHeight;
		GUI.Label (pos, "Description:", ImagePack.FieldStyle ());
		pos.height *= 2;
		descriptionScroll = GUI.BeginScrollView(pos, descriptionScroll, new Rect(0, 0, pos.width * 0.75f, 100));
		editingDisplay.description = GUI.TextArea (new Rect (115, 0, pos.width * 0.75f, 100), editingDisplay.description, ImagePack.TextAreaStyle ());
		//editingDisplay.description = ImagePack.DrawField (new Rect(0, 0, pos.width * 0.75f, 200), "Description:", editingDisplay.description, 0.75f, 50);
		GUI.EndScrollView();
		pos.y += 2.2f*ImagePack.fieldHeight;
		pos.height /= 2;
		GUI.Label (pos, "Objective Text:", ImagePack.FieldStyle ());
		pos.height *= 2;
		objectiveScroll = GUI.BeginScrollView(pos, objectiveScroll, new Rect(0, 0, pos.width * 0.75f, 100));
		editingDisplay.objectiveText = GUI.TextArea (new Rect (115, 0, pos.width * 0.75f, 100), editingDisplay.objectiveText, ImagePack.TextAreaStyle ());
		GUI.EndScrollView();
		pos.height /= 2;
		pos.y += 2.2f*ImagePack.fieldHeight;
		GUI.Label (pos, "Progress Text:", ImagePack.FieldStyle ());
		pos.height *= 2;
		progressScroll = GUI.BeginScrollView(pos, progressScroll, new Rect(0, 0, pos.width * 0.75f, 100));
		editingDisplay.progressText = GUI.TextArea (new Rect (115, 0, pos.width * 0.75f, 100), editingDisplay.progressText, ImagePack.TextAreaStyle ());
		GUI.EndScrollView();
		pos.height /= 2;
		pos.y += 2.2f*ImagePack.fieldHeight;
		GUI.Label (pos, "Completion Text:", ImagePack.FieldStyle ());
		pos.height *= 2;
		completionScroll = GUI.BeginScrollView(pos, completionScroll, new Rect(0, 0, pos.width * 0.75f, 100));
		editingDisplay.completionText = GUI.TextArea (new Rect (115, 0, pos.width * 0.75f, 100), editingDisplay.completionText, ImagePack.TextAreaStyle ());
		GUI.EndScrollView();
		pos.height /= 2;
		pos.y += 2.2f*ImagePack.fieldHeight;
		pos.width /= 2;
		//editingDisplay.category = ImagePack.DrawField (pos, "Category:", editingDisplay.category);
		//pos.x += pos.width;
		editingDisplay.faction = ImagePack.DrawField (pos, "Faction:", editingDisplay.faction);
		pos.x += pos.width;
		editingDisplay.level = ImagePack.DrawField (pos, "Level:", editingDisplay.level);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.chain = ImagePack.DrawField (pos, "Chain:", editingDisplay.chain);
		pos.x += pos.width;
		//editingDisplay.zone = ImagePack.DrawField (pos, "Zone:", editingDisplay.zone);
		//pos.x += pos.width;
		//editingDisplay.numGrades = ImagePack.DrawField (pos, "Num. Grades:", editingDisplay.numGrades);
		//pos.x += pos.width;
		editingDisplay.repeatable = ImagePack.DrawToggleBox (pos, "Is Repeatable?", editingDisplay.repeatable);
		pos.x -= pos.width;
		//pos.x -= 2*pos.width;	
		pos.width *= 2;
		pos.y += 1.5f*ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Items Given");		
		pos.y += 1.5f*ImagePack.fieldHeight;
		pos.width /= 2;
		int selectedItem = GetPositionOfItem(editingDisplay.deliveryItem1);
		selectedItem = ImagePack.DrawSelector (pos, "Item 1:", selectedItem, itemsList);
		editingDisplay.deliveryItem1 = itemIds[selectedItem];
		pos.x += pos.width;
		selectedItem = GetPositionOfItem(editingDisplay.deliveryItem2);
		selectedItem = ImagePack.DrawSelector (pos, "Item 2:", selectedItem, itemsList);
		editingDisplay.deliveryItem2 = itemIds[selectedItem];
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		selectedItem = GetPositionOfItem(editingDisplay.deliveryItem3);
		selectedItem = ImagePack.DrawSelector (pos, "Item 3:", selectedItem, itemsList);
		editingDisplay.deliveryItem3 = itemIds[selectedItem];
		pos.width *= 2;
		pos.y += 1.5f*ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, "Quest Prerequisites");		
		pos.y += 1.5f*ImagePack.fieldHeight;
		//pos.width *= 3;
		pos.width /= 2;
		int selectedQuest = GetPositionOfQuest (editingDisplay.questPrereq);
		selectedQuest = ImagePack.DrawSelector (pos, "Quest Completed:", selectedQuest, questOptions);
		editingDisplay.questPrereq = questIds [selectedQuest];
		pos.x += pos.width;
		selectedQuest = GetPositionOfQuest (editingDisplay.questStartedReq);
		selectedQuest = ImagePack.DrawSelector (pos, "Quest Started:", selectedQuest, questOptions);
		editingDisplay.questStartedReq = questIds [selectedQuest];
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		editingDisplay.levelReq = ImagePack.DrawField (pos, "Level:", editingDisplay.levelReq);
		pos.y += ImagePack.fieldHeight;
		editingDisplay.raceReq = ImagePack.DrawSelector (pos, "Race:", editingDisplay.raceReq, raceOptions);
		pos.x += pos.width;
		editingDisplay.aspectReq = ImagePack.DrawSelector (pos, "Class:", editingDisplay.aspectReq, aspectOptions);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int selectedSkill = GetPositionOfSkill (editingDisplay.skillReq);
		selectedSkill = ImagePack.DrawSelector (pos, "Skill:", selectedSkill, skillOptions);
		editingDisplay.skillReq = skillIds [selectedSkill];
		pos.x += pos.width;
		editingDisplay.skillLevelReq = ImagePack.DrawField (pos, "Skill Level:", editingDisplay.skillLevelReq);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int factionID = GetPositionOfFaction(editingDisplay.repReq);
		factionID = ImagePack.DrawSelector (pos, "Faction:", factionID, factionOptions);
		editingDisplay.repReq = factionIds[factionID];
		pos.x += pos.width;
		int selectedStance = GetPositionOfStance(editingDisplay.repLevelReq);
		selectedStance = ImagePack.DrawSelector (pos, "Stance:", selectedStance, FactionData.stanceOptions);
		editingDisplay.repLevelReq = FactionData.stanceValues[selectedStance];
		pos.x -= pos.width;
		pos.width *= 2;

		pos.y += 1.5f*ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos. x, pos.y, "Objectives");
		pos.y += ImagePack.fieldHeight;
		/*if (editingDisplay.questObjectives.Count == 0) {
			editingDisplay.questObjectives.Add(new QuestsObjectivesData());
		}*/
		for (int i = 0; i < editingDisplay.questObjectives.Count; i++) {
			pos.width /= 2;
			editingDisplay.questObjectives[i].objectiveType = ImagePack.DrawSelector (pos, "Type " + (i+1) + ":", 
				editingDisplay.questObjectives[i].objectiveType, objectiveTypeOptions);
			pos.x += pos.width;
			if (editingDisplay.questObjectives[i].objectiveType == "item") {
				selectedItem = GetPositionOfItem(editingDisplay.questObjectives[i].target);
				selectedItem = ImagePack.DrawSelector (pos, "Target " + (i+1) + ":", selectedItem, itemsList);
				editingDisplay.questObjectives[i].target = itemIds[selectedItem];
			} else if (editingDisplay.questObjectives[i].objectiveType == "mob") {
				int selectedMob = GetPositionOfMob(editingDisplay.questObjectives[i].target);
				selectedMob = ImagePack.DrawSelector (pos, "Target " + (i+1) + ":", selectedMob, mobList);
				editingDisplay.questObjectives[i].target = mobIds[selectedMob];
			}
			pos.x -= pos.width;
			pos.y += ImagePack.fieldHeight;
			editingDisplay.questObjectives[i].targetCount = ImagePack.DrawField (pos, "Count:", editingDisplay.questObjectives[i].targetCount);
			pos.y += ImagePack.fieldHeight;
			pos.width *= 2;
			editingDisplay.questObjectives[i].targetText = ImagePack.DrawField (pos, "Text:", editingDisplay.questObjectives[i].targetText, 1.4f);
			pos.width /= 2;
			pos.y += ImagePack.fieldHeight;
			pos.x += pos.width;
			if (ImagePack.DrawButton (pos.x, pos.y, "Delete Objective")) {
				if (editingDisplay.questObjectives[i].id > 0)
					editingDisplay.objectivesToBeDeleted.Add(editingDisplay.questObjectives[i].id);
				editingDisplay.questObjectives.RemoveAt(i);
			}
			pos.x -= pos.width;
			pos.width *= 2;
			pos.y += ImagePack.fieldHeight;
		}
		if (ImagePack.DrawButton (pos.x, pos.y, "Add Objective")) {
			editingDisplay.questObjectives.Add(new QuestsObjectivesData());
		}

		pos.y += 1.5f*ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos. x, pos.y, "Rewards");
		pos.y += ImagePack.fieldHeight;

		pos.width /= 2;
		editingDisplay.experience = ImagePack.DrawField (pos, "Experience:", editingDisplay.experience);
		//pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		selectedItem = GetPositionOfItem(editingDisplay.item1);
		selectedItem = ImagePack.DrawSelector (pos, "Item 1:", selectedItem, itemsList);
		editingDisplay.item1 = itemIds[selectedItem];
		pos.x += pos.width;
		editingDisplay.item1count = ImagePack.DrawField (pos, "Count:", editingDisplay.item1count);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		selectedItem = GetPositionOfItem(editingDisplay.item2);
		selectedItem = ImagePack.DrawSelector (pos, "Item 2:", selectedItem, itemsList);
		editingDisplay.item2 = itemIds[selectedItem];
		pos.x += pos.width;
		editingDisplay.item2count = ImagePack.DrawField (pos, "Count:", editingDisplay.item2count);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		selectedItem = GetPositionOfItem(editingDisplay.item3);
		selectedItem = ImagePack.DrawSelector (pos, "Item 3:", selectedItem, itemsList);
		editingDisplay.item3 = itemIds[selectedItem];
		pos.x += pos.width;
		editingDisplay.item3count = ImagePack.DrawField (pos, "Count:", editingDisplay.item3count);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		selectedItem = GetPositionOfItem(editingDisplay.item4);
		selectedItem = ImagePack.DrawSelector (pos, "Item 4:", selectedItem, itemsList);
		editingDisplay.item4 = itemIds[selectedItem];
		pos.x += pos.width;
		editingDisplay.item4count = ImagePack.DrawField (pos, "Count:", editingDisplay.item4count);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		selectedItem = GetPositionOfItem(editingDisplay.chooseItem1);
		selectedItem = ImagePack.DrawSelector (pos, "Item Choice 1:", selectedItem, itemsList);
		editingDisplay.chooseItem1 = itemIds[selectedItem];
		pos.x += pos.width;
		editingDisplay.chooseItem1count = ImagePack.DrawField (pos, "Count:", editingDisplay.chooseItem1count);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		selectedItem = GetPositionOfItem(editingDisplay.chooseItem2);
		selectedItem = ImagePack.DrawSelector (pos, "Item Choice 2:", selectedItem, itemsList);
		editingDisplay.chooseItem2 = itemIds[selectedItem];
		pos.x += pos.width;
		editingDisplay.chooseItem2count = ImagePack.DrawField (pos, "Count:", editingDisplay.chooseItem2count);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		selectedItem = GetPositionOfItem(editingDisplay.chooseItem3);
		selectedItem = ImagePack.DrawSelector (pos, "Item Choice 3:", selectedItem, itemsList);
		editingDisplay.chooseItem3 = itemIds[selectedItem];
		pos.x += pos.width;
		editingDisplay.chooseItem3count = ImagePack.DrawField (pos, "Count:", editingDisplay.chooseItem3count);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		selectedItem = GetPositionOfItem(editingDisplay.chooseItem4);
		selectedItem = ImagePack.DrawSelector (pos, "Item Choice 4:", selectedItem, itemsList);
		editingDisplay.chooseItem4 = itemIds[selectedItem];
		pos.x += pos.width;
		editingDisplay.chooseItem4count = ImagePack.DrawField (pos, "Count:", editingDisplay.chooseItem4count);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		//editingDisplay.currency = ImagePack.DrawField (pos, "Currency:", editingDisplay.currency);
		//pos.x += pos.width;
		//editingDisplay.currencyCount = ImagePack.DrawField (pos, "Count:", editingDisplay.currencyCount);
		//pos.x -= pos.width;
		//		pos.y += ImagePack.fieldHeight;
		//		editingDisplay.currency2 = ImagePack.DrawField (pos, "currency 2:", editingDisplay.currency2);
		//		pos.x += pos.width;
		//		editingDisplay.currency2count = ImagePack.DrawField (pos, "Count:", editingDisplay.currency2count);
		//		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		factionID = GetPositionOfFaction(editingDisplay.rep1);
		factionID = ImagePack.DrawSelector (pos, "Faction 1:", factionID, factionOptions);
		editingDisplay.rep1 = factionIds[factionID];
		pos.x += pos.width;
		editingDisplay.rep1gain = ImagePack.DrawField (pos, "Rep:", editingDisplay.rep1gain);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		factionID = GetPositionOfFaction(editingDisplay.rep2);
		factionID = ImagePack.DrawSelector (pos, "Faction 2:", factionID, factionOptions);
		editingDisplay.rep2 = factionIds[factionID];
		pos.x += pos.width;
		editingDisplay.rep2gain = ImagePack.DrawField (pos, "Rep:", editingDisplay.rep2gain);
		pos.x -= pos.width;
		pos.width *= 2;

		// Save data
		pos.x -= ImagePack.innerMargin;
		pos.y += 1.4f * ImagePack.fieldHeight;
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

		// If the insert failed, don't insert the spawn marker
		if (itemID != -1) { 
			// Insert the objectives
			foreach(QuestsObjectivesData entry in editingDisplay.questObjectives) {
				if (entry.target != -1) {
					entry.questID = itemID;
					InsertObjective(entry);
				}
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

	void InsertObjective(QuestsObjectivesData entry) 
	{
		string query = "INSERT INTO quest_objectives";		
		query += " (questID, primaryObjective, objectiveType, target, targetCount, targetText) ";
		query += "VALUES ";
		query += " (" + entry.questID + ",1,'" + entry.objectiveType + "'," + entry.target + "," + entry.targetCount + ",'" + entry.targetText + "') ";
		
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

		// Insert/Update the objectives
		foreach(QuestsObjectivesData entry in editingDisplay.questObjectives) {
			if (entry.target != -1) {
				if (entry.id == 0) {
					// This is a new entry, insert it
					entry.questID = editingDisplay.id;
					InsertObjective(entry);
				} else {
					// This is an existing entry, update it
					entry.questID = editingDisplay.id;
					UpdateObjective(entry);
				}
			}
		}
		// And now delete any Objectives that are tagged for deletion
		foreach (int objectiveID in editingDisplay.objectivesToBeDeleted) {
			DeleteObjective(objectiveID);
		}
				
		// Update online table to avoid access the database again			
		dataRegister [displayKeys [selectedDisplay]] = editingDisplay;		
		NewResult("Entry updated");		
	}

	void UpdateObjective(QuestsObjectivesData entry) 
	{
		string query = "UPDATE quest_objectives";		
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
	
	void DeleteObjective(int objectiveID) {
		Register delete = new Register ("id", "?id", MySqlDbType.Int32, objectiveID.ToString (), Register.TypesOfField.Int);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "quest_objectives", delete);
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

	private int GetPositionOfStance(int stanceValue) {
		for (int i = 0; i < FactionData.stanceValues.Length; i++) {
			if (FactionData.stanceValues[i] == stanceValue)
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

	private int GetPositionOfQuest (int questID)
	{
		for (int i = 0; i < questIds.Length; i++) {
			if (questIds [i] == questID)
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

	private int GetPositionOfItem (int itemId)
	{
		for (int i = 0; i < itemIds.Length; i++) {
			if (itemIds [i] == itemId)
				return i;
		}
		return 0;
	}

	private int GetPositionOfMob (int mobId)
	{
		for (int i = 0; i < mobIds.Length; i++) {
			if (mobIds [i] == mobId)
				return i;
		}
		return 0;
	}

}
