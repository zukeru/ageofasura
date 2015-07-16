using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Mob Configuration
public class ServerDialogues : AtavismDatabaseFunction 
{
	
	public Dictionary<int, DialogueData> dataRegister;
	public DialogueData editingDisplay;
	public DialogueData originalDisplay;

	int requestedDisplay = -1;
	
	public int[] dialogueIds = new int[] {-1};
	public string[] dialogueList = new string[] {"~ none ~"};

	public int[] questIds = new int[] {-1};
	public string[] questList = new string[] {"~ none ~"};

	public int[] factionIds = new int[] {-1};
	public string[] factionList = new string[] {"~ none ~"};
	
	public string[] actionOptions = new string[] {"~ none ~"};
	
	Vector2 descriptionScroll = new Vector2();

	// Use this for initialization
	public ServerDialogues ()
	{	
		functionName = "Dialogue";
		// Database tables name
	    tableName = "dialogue";
		functionTitle =  "Dialogue Configuration";
		loadButtonLabel =  "Load Dialogues";
		notLoadedText =  "No Dialogue loaded.";
		// Init
		dataRegister = new Dictionary<int, DialogueData> ();

		editingDisplay = new DialogueData ();	
		originalDisplay = new DialogueData ();	
	}

	public override void Activate()
	{
		linkedTablesLoaded = false;
	}

	private void LoadDialogueList ()
	{
		string query = "SELECT id, name FROM dialogue ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows!=null) && (rows.Count > 0)) {
			dialogueList = new string[rows.Count + 1];
			dialogueList [optionsId] = "~ none ~"; 
			dialogueIds = new int[rows.Count + 1];
			dialogueIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				dialogueList [optionsId] = data ["id"] + ":" + data ["name"]; 
				dialogueIds[optionsId] = int.Parse(data ["id"]);
			}
		}
	}

	private void LoadQuestList ()
	{
		string query = "SELECT id, name FROM quests ";
		
		// If there is a row, clear it.
		if (rows != null)
			rows.Clear ();
		
		// Load data
		rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
		// Read data
		int optionsId = 0;
		if ((rows!=null) && (rows.Count > 0)) {
			questList = new string[rows.Count + 1];
			questList [optionsId] = "~ none ~"; 
			questIds = new int[rows.Count + 1];
			questIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				questList [optionsId] = data ["id"] + ":" + data ["name"]; 
				questIds[optionsId] = int.Parse(data ["id"]);
			}
		}
	}

	private void LoadFactionList ()
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
			factionList = new string[rows.Count + 1];
			factionList [optionsId] = "~ none ~"; 
			factionIds = new int[rows.Count + 1];
			factionIds [optionsId] = -1;
			foreach (Dictionary<string,string> data in rows) {
				optionsId++;
				factionList [optionsId] = data ["id"] + ":" + data ["name"]; 
				factionIds[optionsId] = int.Parse(data ["id"]);
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
					DialogueData display = new DialogueData ();
					display.id = int.Parse (data ["id"]);
					display.name = data ["name"];
					display.openingDialogue = bool.Parse (data ["openingDialogue"]);
					display.repeatable = bool.Parse (data ["repeatable"]);
					display.prereqDialogue = int.Parse (data ["prereqDialogue"]);
					display.prereqQuest = int.Parse (data ["prereqQuest"]);
					display.prereqFaction = int.Parse (data ["prereqFaction"]);
					display.prereqFactionStance = int.Parse (data ["prereqFactionStance"]);
					display.reactionAutoStart = bool.Parse (data ["reactionAutoStart"]);
					display.text = data ["text"];
					for (int i = 1; i <= display.maxEntries; i++) {
						string text = data ["option" + i + "text"]; 
						if (text != null && text.Length != 0) {
							string action = data ["option" + i + "action"];
							int actionID = int.Parse (data ["option" + i + "actionID"]);
							DialogueActionEntry entry = new DialogueActionEntry(text, action, actionID);
							display.entries.Add(entry);
						}
					}
											
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
			ImagePack.DrawLabel (pos.x, pos.y, "You must create a Dialogue before edit it.");		
			return;
		}
		

		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Dialogue Configuration");
		
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
			requestedDisplay = selectedDisplay;
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
			//requestedDisplay = newSelectedDisplay;
			pos.x -= pos.width;
			pos.width *= 2;
		}

		if(requestedDisplay != -1 && requestedDisplay != selectedDisplay)
			newSelectedDisplay = requestedDisplay;
	}
	
	public override void CreateNewData()
	{
		editingDisplay = new DialogueData ();		
		originalDisplay = new DialogueData ();	
		selectedDisplay = -1;
	}

	// Edit or Create
	public override void DrawEditor (Rect box, bool newItem)
	{
		if (!linkedTablesLoaded) {
			LoadDialogueList();
			LoadQuestList();
			LoadFactionList();
			actionOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Dialogue Action", false);
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
			ImagePack.DrawLabel (pos.x, pos.y, "Create a new Dialogue");		
			pos.y += ImagePack.fieldHeight;
		}

		// Draw a button to go back to the previous dialogue
		if (editingDisplay.previousDialogueID != -1) {
			DialogueData dd = (DialogueData) dataRegister[editingDisplay.previousDialogueID];
			pos.width = pos.width * 2;
			if (ImagePack.DrawButton (pos.x, pos.y, "Go back to " + dd.id + "." + dd.name)) {
				//editingDisplay = GetDialogue(editingDisplay.previousDialogueID);
				requestedDisplay = GetPositionOfPreviousDialogue(editingDisplay.previousDialogueID);
				state = State.Edit;
				return;
			}
			pos.y += ImagePack.fieldHeight;
			pos.width = pos.width / 2;
		}

		editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.5f);
		pos.y += ImagePack.fieldHeight;
		pos.width = pos.width / 2;
		editingDisplay.openingDialogue = ImagePack.DrawToggleBox(pos, "Opening Dialogue:", editingDisplay.openingDialogue);
		pos.x += pos.width;
		editingDisplay.repeatable = ImagePack.DrawToggleBox(pos, "Repeatable:", editingDisplay.repeatable);
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int dialogueID = GetPositionOfDialogue(editingDisplay.prereqDialogue);
		dialogueID = ImagePack.DrawSelector (pos, "Prereq Dialogue:", dialogueID, dialogueList);
		editingDisplay.prereqDialogue = dialogueIds[dialogueID];
		pos.x += pos.width;
		int questID = GetPositionOfFaction(editingDisplay.prereqQuest);
		questID = ImagePack.DrawSelector (pos, "Prereq Quest:", questID, questList);
		editingDisplay.prereqFaction = factionIds[questID];
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		int factionID = GetPositionOfFaction(editingDisplay.prereqFaction);
		factionID = ImagePack.DrawSelector (pos, "Prereq Faction:", factionID, factionList);
		editingDisplay.prereqFaction = factionIds[factionID];
		pos.x += pos.width;
		int factionStance = GetPositionOfStance(editingDisplay.prereqFactionStance);
		factionStance = ImagePack.DrawSelector (pos, "Prereq Stance:", factionStance, FactionData.stanceOptions);
		editingDisplay.prereqFactionStance = FactionData.stanceValues[factionStance];
		pos.x -= pos.width;
		pos.y += ImagePack.fieldHeight;
		pos.width = pos.width * 2;
		GUI.Label (pos, "Text:", ImagePack.FieldStyle ());
		pos.height *= 3;
		descriptionScroll = GUI.BeginScrollView(pos, descriptionScroll, new Rect(0, 0, pos.width * 0.75f, 150));
		editingDisplay.text = GUI.TextArea (new Rect (115, 0, pos.width * 0.75f, 150), editingDisplay.text, ImagePack.TextAreaStyle ());
		//editingDisplay.text = ImagePack.DrawField (pos, "Text:", editingDisplay.text, 0.75f, 60);
		GUI.EndScrollView();
		pos.height /= 3;
		pos.y += 3f * ImagePack.fieldHeight;

		// Only show actions if it has been saved due to complications of moving forward/back
		if (!newItem) {
			if (editingDisplay.entries.Count == 0) {
				editingDisplay.entries.Add(new DialogueActionEntry("", actionOptions[0], -1));
			}
			for (int i = 0; i < editingDisplay.maxEntries; i++) {
				if (editingDisplay.entries.Count > i) {
					ImagePack.DrawLabel (pos.x, pos.y, "Action " + (i+1));
					pos.y += ImagePack.fieldHeight;
					editingDisplay.entries[i].text = ImagePack.DrawField (pos, "Text:", editingDisplay.entries[i].text, 1.4f);
					pos.width = pos.width / 2;
					pos.y += ImagePack.fieldHeight;
					editingDisplay.entries[i].action = ImagePack.DrawSelector (pos, "Action Type:", editingDisplay.entries[i].action, actionOptions);
					pos.x += pos.width;
					if (editingDisplay.entries[i].action == "Dialogue") {
						// Treat dialogues differently as the developer will be able to click to move onto the next one
						if (editingDisplay.entries[i].actionID == -1) {
							// No dialogue yet, click button to create a new one
							if (ImagePack.DrawButton (pos.x, pos.y, "Create Dialogue")) {
								// Save dialogue first then lets go create a new one
								UpdateEntry ();
								int previousDialogueID = editingDisplay.id;
								CreateNewData();
								editingDisplay.previousDialogueID = previousDialogueID;
								editingDisplay.previousActionPosition = i;
								editingDisplay.openingDialogue = false;
								this.state = State.New;
								return;
							}
						} else {
							// Show dialogue with option to move forward
							if (ImagePack.DrawButton (pos.x, pos.y, "Edit Dialogue")) {
								// Save dialogue first then lets go create a new one
								UpdateEntry ();
								dataRegister[editingDisplay.entries[i].actionID].previousDialogueID = editingDisplay.id;
								requestedDisplay = GetPositionOfDialogue(editingDisplay.entries[i].actionID) - 1;
								state = State.Edit;
							}
						}
					} else {
						editingDisplay.entries[i].actionID = ImagePack.DrawField (pos, "ActionID:", editingDisplay.entries[i].actionID);
					}
					pos.x -= pos.width;
					pos.y += ImagePack.fieldHeight;
					pos.width = pos.width * 2;
				}
			}
			if (editingDisplay.entries.Count < editingDisplay.maxEntries) {
				if (ImagePack.DrawButton (pos.x, pos.y, "Add Action")) {
					DialogueActionEntry actionEntry = new DialogueActionEntry("", actionOptions[0], -1);
					editingDisplay.entries.Add(actionEntry);
				}
			}
		}
		//pos.width = pos.width * 2;
		
		// Save data		
		pos.x -= ImagePack.innerMargin;
		pos.y += 1.4f * ImagePack.fieldHeight;
		pos.width /=3;
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
		
		int mobID = -1;

		// Setup the register data		
		List<Register> update = new List<Register> ();
		foreach (string field in editingDisplay.fields.Keys) {
			update.Add (editingDisplay.fieldToRegister (field));       
		}
		
		// Update the database
		mobID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);

		// If the insert succeeded - update other fields
		if (mobID != -1) {          
			// Update online table to avoid access the database again			
			editingDisplay.id = mobID;
			editingDisplay.isLoaded = true;
			//Debug.Log("ID:" + mobID + "ID2:" + editingDisplay.id);
			dataRegister.Add (editingDisplay.id, editingDisplay);
			displayKeys.Add (editingDisplay.id);
			newItemCreated = true;
			// If this has a previous id - update the actionID field for that dialogue
			if (editingDisplay.previousDialogueID != -1) {
				int tempSelectedDisplay = selectedDisplay;
				selectedDisplay = GetPositionOfPreviousDialogue(editingDisplay.previousDialogueID);
				int actionPos = editingDisplay.previousActionPosition;
				editingDisplay = dataRegister [editingDisplay.previousDialogueID];	
				editingDisplay.entries[actionPos].actionID = mobID;
				UpdateEntry();
				editingDisplay = originalDisplay;
				selectedDisplay = tempSelectedDisplay;
			}
			LoadDialogueList ();
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
			LoadSelectList();
		else {
			displayList = null;
			dataLoaded = false;
		}
		LoadDialogueList ();
	}
	
	private int GetPositionOfDialogue(int dialogueID) {
		for (int i = 0; i < dialogueIds.Length; i++) {
			if (dialogueIds[i] == dialogueID)
				return i;
		}
		return 0;
	}

	private int GetPositionOfQuest(int questID) {
		for (int i = 0; i < questIds.Length; i++) {
			if (questIds[i] == questID)
				return i;
		}
		return 0;
	}

	private int GetPositionOfFaction(int factionID) {
		for (int i = 0; i < factionIds.Length; i++) {
			if (factionIds[i] == factionID)
				return i;
		}
		return 0;
	}

	private int GetPositionOfStance(int stanceValue) {
		for (int i = 0; i < FactionData.stanceValues.Length; i++) {
			if (FactionData.stanceValues[i] == stanceValue)
				return i;
		}
		return 0;
	}

	private int GetPositionOfPreviousDialogue(int dialogueID) {
		for (int i = 0; i < displayKeys.Count; i++) {
			if (displayKeys[i] == dialogueID) {
				return i;
			}
		}
		return 0;
	}
}
