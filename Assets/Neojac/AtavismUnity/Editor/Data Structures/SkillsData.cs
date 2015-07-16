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

public class SkillAbilityEntry : DataStructure
{
	public SkillAbilityEntry() : this(1, -1, -1, true) {
	}

	public SkillAbilityEntry(int skillLevelReq, int abilityID) : this(skillLevelReq, abilityID, -1, true) {
	}

	public SkillAbilityEntry(int skillLevelReq, int abilityID, int entryID, bool autoLearn) {
		this.id = entryID;
		this.skillLevelReq = skillLevelReq;
		this.abilityID = abilityID;
		this.automaticallyLearn = autoLearn;

		fields = new Dictionary<string, string> () {
			{"id", "int"},
			{"skillID", "int"},
			{"skillLevelReq", "int"},
			{"abilityID", "int"},
			{"automaticallyLearn", "bool"},
		};
	}
	
	public int skillID;
	public int skillLevelReq = 1;
	public int abilityID;
	public bool automaticallyLearn = true;

	public SkillAbilityEntry Clone()
	{
		return (SkillAbilityEntry) this.MemberwiseClone();
	}
	
	public override string GetValue (string fieldKey)
	{
		if (fieldKey == "id") {
			return id.ToString();
		} else if (fieldKey == "skillID") {
			return skillID.ToString();
		} else if (fieldKey == "skillLevelReq") {
			return skillLevelReq.ToString();
		} else if (fieldKey == "abilityID") {
			return abilityID.ToString();
		} else if (fieldKey == "automaticallyLearn") {
			return automaticallyLearn.ToString();
		}
		return "";
	}
}

public class SkillsData: DataStructure
{
	public int id = 1;					// Database Index
	// General Parameters
	public string name = "name";		// The skill template name
	public string icon = "";			// The ability icon
	
	public string aspect;  				// Id of the aspect this skill belongs to
	public string oppositeAspect;			// Id of the opposite aspect for this skill
	public string primaryStat;  		// Stat that gets the most gains from this skill
	public string secondaryStat;		// Stat that gets a high amount of gain from this skill
	public string thirdStat;			// Stat that gets a medium amount of gain from this skill
	public string fourthStat; 			// Stat that gets a low amount of gain from this skill
	
	public int maxLevel = 1;
	public bool automaticallyLearn = true;
	public int skillPointCost = 0;
	public int parentSkill = -1;
	public int parentSkillLevelReq = 0;
	public int prereqSkill1 = -1;
	public int prereqSkill1Level = 0;
	public int prereqSkill2 = -1;
	public int prereqSkill2Level = 0;
	public int prereqSkill3 = -1;
	public int prereqSkill3Level = 0;
	public int playerLevelReq = 0;

	public List<SkillAbilityEntry> skillAbilities = new List<SkillAbilityEntry>();
	
	public List<int> abilitiesToBeDeleted = new List<int>();

	public SkillsData () 
	{
		// Database fields
		fields = new Dictionary<string, string> () {
			{"name", "string"},
			{"icon", "string"},
			{"aspect", "string"},
			{"oppositeAspect", "string"},
			{"primaryStat", "string"},
			{"secondaryStat", "string"}, 
			{"thirdStat", "string"}, 
			{"fourthStat", "string"},
			{"maxLevel", "int"},
			{"automaticallyLearn", "bool"},
			{"skillPointCost", "int"},
			{"parentSkill", "int"},
			{"parentSkillLevelReq", "int"},
			{"prereqSkill1", "int"},
			{"prereqSkill1Level", "int"},
			{"prereqSkill2", "int"},
			{"prereqSkill2Level", "int"},
			{"prereqSkill3", "int"},
			{"prereqSkill3Level", "int"},
			{"playerLevelReq", "int"},
		};

	}
	
	public SkillsData Clone()
	{
		return (SkillsData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "name":
			return name;
			break;
		case "icon":
			return icon;
			break;
		case "aspect":
			return aspect;
			break;
		case "oppositeAspect":
			return oppositeAspect;
			break;
		case "primaryStat":
			return primaryStat;
			break;
		case "secondaryStat":
			return secondaryStat;
			break;
		case "thirdStat":
			return thirdStat;
			break;
		case "fourthStat":
			return fourthStat;
			break;
		case "maxLevel":
			return maxLevel.ToString();
			break;
		case "automaticallyLearn":
			return automaticallyLearn.ToString();
			break;
		case "skillPointCost":
			return skillPointCost.ToString();
			break;
		case "parentSkill":
			return parentSkill.ToString();
			break;
		case "parentSkillLevelReq":
			return parentSkillLevelReq.ToString();
			break;
		case "prereqSkill1":
			return prereqSkill1.ToString();
			break;
		case "prereqSkill1Level":
			return prereqSkill1Level.ToString();
			break;
		case "prereqSkill2":
			return prereqSkill2.ToString();
			break;
		case "prereqSkill2Level":
			return prereqSkill2Level.ToString();
			break;
		case "prereqSkill3":
			return prereqSkill3.ToString();
			break;
		case "prereqSkill3Level":
			return prereqSkill3Level.ToString();
			break;
		case "playerLevelReq":
			return playerLevelReq.ToString();
			break;
		}
		return "";
	}
		
}