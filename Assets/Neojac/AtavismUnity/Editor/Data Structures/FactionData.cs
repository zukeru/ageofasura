using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Skills
/*
/* Table structure for table `skills`
/*

CREATE TABLE `skills` (
  `id` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `aspect` int(11) DEFAULT NULL,
  `oppositeAspect` int(11) DEFAULT NULL,
  `primaryStat` int(11) NOT NULL,
  `secondaryStat` int(11) NOT NULL,
  `thirdStat` int(11) NOT NULL,
  `fourthStat` int(11) NOT NULL,
  `ability1` int(11) DEFAULT NULL,
  `ability1Level` int(11) DEFAULT NULL,
  `ability2` int(11) DEFAULT NULL,
  `ability2Level` int(11) DEFAULT NULL,
  `ability3` int(11) DEFAULT NULL,
  `ability3Level` int(11) DEFAULT NULL,
  `ability4` int(11) DEFAULT NULL,
  `ability4Level` int(11) DEFAULT NULL,
  `ability5` int(11) DEFAULT NULL,
  `ability5Level` int(11) DEFAULT NULL,
  `ability6` int(11) DEFAULT NULL,
  `ability6Level` int(11) DEFAULT NULL,
  `ability7` int(11) DEFAULT NULL,
  `ability7Level` int(11) DEFAULT NULL,
  `ability8` int(11) DEFAULT NULL,
  `ability8Level` int(11) DEFAULT NULL,
  `ability9` int(11) DEFAULT NULL,
  `ability9Level` int(11) DEFAULT NULL,
  `ability10` int(11) DEFAULT NULL,
  `ability10Level` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

*/

public class FactionStanceEntry : DataStructure
{
	public FactionStanceEntry() : this(-1, 0, -1) {
	}

	public FactionStanceEntry(int otherFactionID, int defaultStance) : this(otherFactionID, defaultStance, -1) {
	}

	public FactionStanceEntry(int otherFactionID, int defaultStance, int entryID) {
		this.factionID = entryID;
		this.otherFactionID = otherFactionID;
		this.defaultStance = defaultStance;

		fields = new Dictionary<string, string> () {
			{"id", "int"},
			{"factionID", "int"},
			{"otherFaction", "int"},
			{"defaultStance", "int"},
		};
	}
	
	public int factionID;
	public int otherFactionID;
	public int defaultStance;

	public FactionStanceEntry Clone()
	{
		return (FactionStanceEntry) this.MemberwiseClone();
	}
	
	public override string GetValue (string fieldKey)
	{
		if (fieldKey == "id") {
			return id.ToString();
		} else if (fieldKey == "factionID") {
			return factionID.ToString();
		} else if (fieldKey == "otherFaction") {
			return otherFactionID.ToString();
		} else if (fieldKey == "defaultStance") {
			return defaultStance.ToString();
		}
		return "";
	}
}

public class FactionData: DataStructure
{
	public int id = 1;					// Database Index
	// General Parameters
	public string name = "name";		// The skill template name

	public int category = 1;
	public string group = "";  				// Id of the aspect this skill belongs to
	public bool modifyable = false;			// Id of the opposite aspect for this skill
	public int defaultStance = 0;  		// Stat that gets the most gains from this skill
	public static string[] stanceOptions = new string[] {"Hated", "Disliked", "Neutral", "Friendly", "Honoured", "Exalted"};
	public static int[] stanceValues = new int[] {-2, -1, 0, 1, 2, 3};
	public List<FactionStanceEntry> factionStances = new List<FactionStanceEntry>();
	
	public List<int> stancesToBeDeleted = new List<int>();

	public FactionData ()
	{
		// Database fields
		fields = new Dictionary<string, string> () {
			{"name", "string"},
			{"category", "int"},
			{"factionGroup", "string"},
			{"public", "bool"},
			{"defaultStance", "int"},
		};
	}
	
	public FactionData Clone()
	{
		return (FactionData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		if (fieldKey == "name") {
			return name;
		} else if (fieldKey == "category") {
			return category.ToString();
		} else if (fieldKey == "factionGroup") {
			return group;
		} else if (fieldKey == "public") {
			return modifyable.ToString();
		} else if (fieldKey == "defaultStance") {
			return defaultStance.ToString();
		}
		return "";
	}
		
}