using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Loot Table
/*
/* Table structure for tables
/*
CREATE TABLE `mob_loot` (
`id` int(11) NOT NULL AUTO_INCREMENT,
  `category` int(11) NOT NULL DEFAULT '1',
  `mobTemplate` int(11) NOT NULL,
  `lootTable` int(11) DEFAULT NULL,
  `dropChance` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mobTemplate` (`mobTemplate`)
)
*/

public class AtavismEditorOptionChoice : DataStructure
{
	public AtavismEditorOptionChoice() : this(-1, -1, "") {
	}
	
	public AtavismEditorOptionChoice(int entryID, int optionTypeID, string choice) {
		this.id = entryID;
		this.optionTypeID = optionTypeID;
		this.choice = choice;
		
		fields = new Dictionary<string, string> () {
			{"id", "int"},
			{"optionTypeID", "int"},
			{"choice", "string"},
		};
	}
	
	public int optionTypeID;
	public string choice = "";
	
	public AtavismEditorOptionChoice Clone()
	{
		return (AtavismEditorOptionChoice) this.MemberwiseClone();
	}
	
	public override string GetValue (string fieldKey)
	{
		if (fieldKey == "id") {
			return id.ToString();
		} else if (fieldKey == "optionTypeID") {
			return optionTypeID.ToString();
		} else if (fieldKey == "choice") {
			return choice;
		}
		return "";
	}
}

public class AtavismEditorOption : DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public string name = "";
	public bool deletable = true;
	
	public List<AtavismEditorOptionChoice> optionChoices = new List<AtavismEditorOptionChoice>();
	// Choices to be deleted
	public List<int> choicesToBeDeleted = new List<int>();
	
	public AtavismEditorOption ()
	{
		// Database fields
		fields = new Dictionary<string, string> () {
			{"optionType", "string"},
			{"deletable", "bool"},
		};
	}
	
	public AtavismEditorOption Clone()
	{
		return (AtavismEditorOption) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "optionType":
			return name;
			break;
		case "deletable":
			return deletable.ToString();
			break;
		}	
		return "";
	}
		
}