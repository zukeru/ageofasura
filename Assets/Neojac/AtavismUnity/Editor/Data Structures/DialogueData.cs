using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Loot Table
/*
/* Table structure for tables
/*
CREATE TABLE `loot_tables` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `item1` int(11) NOT NULL DEFAULT '-1',
  `item1count` int(11) DEFAULT NULL,
  `item1chance` int(11) DEFAULT NULL,
  `item2` int(11) NOT NULL DEFAULT '-1',
  `item2count` int(11) DEFAULT NULL,
  `item2chance` int(11) DEFAULT NULL,
  `item3` int(11) NOT NULL DEFAULT '-1',
  `item3count` int(11) DEFAULT NULL,
  `item3chance` int(11) DEFAULT NULL,
  `item4` int(11) NOT NULL DEFAULT '-1',
  `item4count` int(11) DEFAULT NULL,
  `item4chance` int(11) DEFAULT NULL,
  `item5` int(11) NOT NULL DEFAULT '-1',
  `item5count` int(11) DEFAULT NULL,
  `item5chance` int(11) DEFAULT NULL,
  `item6` int(11) NOT NULL DEFAULT '-1',
  `item6count` int(11) DEFAULT NULL,
  `item6chance` int(11) DEFAULT NULL,
  `item7` int(11) NOT NULL DEFAULT '-1',
  `item7count` int(11) DEFAULT NULL,
  `item7chance` int(11) DEFAULT NULL,
  `item8` int(11) NOT NULL DEFAULT '-1',
  `item8count` int(11) DEFAULT NULL,
  `item8chance` int(11) DEFAULT NULL,
  `item9` int(11) NOT NULL DEFAULT '-1',
  `item9count` int(11) DEFAULT NULL,
  `item9chance` int(11) DEFAULT NULL,
  `item10` int(11) NOT NULL DEFAULT '-1',
  `item10count` int(11) DEFAULT NULL,
  `item10chance` int(11) DEFAULT NULL,
  `category` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`),
  KEY `item1` (`item1`)
)
*/

public class DialogueActionEntry
{
	public DialogueActionEntry(string text, string action, int actionID) {
		this.text = text;
		this.action = action;
		this.actionID = actionID;
	}
	
	public string text;
	public string action;
	public int actionID = -1;
}

public class DialogueData: DataStructure
{
	public int id = 0;					// Database Index
	// Previous dialogue 
	public int previousDialogueID = -1;
	public int previousActionPosition = 0;

	// General Parameters
	public bool openingDialogue = true;
	public bool repeatable = true;
	public int prereqDialogue;
	public int prereqQuest;
	public int prereqFaction;
	public int prereqFactionStance = 1;
	public bool reactionAutoStart;
	public string text = "";


	public int maxEntries = 3;
	public List<DialogueActionEntry> entries = new List<DialogueActionEntry>();
	
	public DialogueData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"name", "string"},
		{"openingDialogue", "bool"},
		{"repeatable", "bool"},
		{"prereqDialogue", "int"},
		{"prereqQuest", "int"},
		{"prereqFaction", "int"},
		{"prereqFactionStance", "int"},
		{"reactionAutoStart", "bool"},
		{"text", "string"},
		{"option1text", "string"},
		{"option1action", "string"},
		{"option1actionID", "int"},
		{"option2text", "string"},
		{"option2action", "string"},
		{"option2actionID", "int"},
		{"option3text", "string"},
		{"option3action", "string"},
		{"option3actionID", "int"},
	};
	}
	
	public DialogueData Clone()
	{
		return (DialogueData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "name":
			return name;
			break;
		case "openingDialogue":
			return openingDialogue.ToString();
			break;
		case "repeatable":
			return repeatable.ToString();
			break;
		case "prereqDialogue":
			return prereqDialogue.ToString();
			break;
		case "prereqQuest":
			return prereqQuest.ToString();
			break;
		case "prereqFaction":
			return prereqFaction.ToString();
			break;
		case "prereqFactionStance":
			return prereqFactionStance.ToString();
			break;
		case "reactionAutoStart":
			return reactionAutoStart.ToString();
			break;
		case "text":
			return text;
			break;
		case "option1text":
			if (entries.Count > 0)
				return entries[0].text;
			else
				return "";
			break;
		case "option1action":
			if (entries.Count > 0)
				return entries[0].action;
			else
				return "";
			break;
		case "option1actionID":
			if (entries.Count > 0)
				return entries[0].actionID.ToString();
			else
				return "0";
			break;
		case "option2text":
			if (entries.Count > 1)
				return entries[1].text;
			else
				return "";
			break;
		case "option2action":
			if (entries.Count > 1)
				return entries[1].action;
			else
				return "";
			break;
		case "option2actionID":
			if (entries.Count > 1)
				return entries[1].actionID.ToString();
			else
				return "0";
			break;
		case "option3text":
			if (entries.Count > 2)
				return entries[2].text;
			else
				return "";
			break;
		case "option3action":
			if (entries.Count > 2)
				return entries[2].action;
			else
				return "";
			break;
		case "option3actionID":
			if (entries.Count > 2)
				return entries[2].actionID.ToString();
			else
				return "0";
			break;
		}
		return "";
	}
		
}