using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Character

/*
/* Table structure for tables
/*

CREATE TABLE `character_create_template` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `race` varchar(45) NOT NULL,
  `aspect` varchar(45) NOT NULL,
  `instanceName` varchar(45) NOT NULL,
  `pos_x` float NOT NULL,
  `pos_y` float NOT NULL,
  `pos_z` float NOT NULL,
  `orientation` float NOT NULL,
  `health` int(11) NOT NULL,
  `mana` int(11) NOT NULL,

*/

public class CharData: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public string race = "~ none ~";		// The ability template name
	public string aspect = "~ none ~";		// The ability template name
	public int faction = 1;
	public string instanceName = "name";		// The ability template name
	public float pos_x = 0;
	public float pos_y = 0;
	public float pos_z = 0;
	string spawn = null;							// Prefab Object to spawn 
	public float orientation = 0;
	public int autoAttack = 0;

	public List<CharStatsData> charStats = new List<CharStatsData>();
	public List<CharSkillsData> charSkills = new List<CharSkillsData>();
	public List<CharItemsData> charItems = new List<CharItemsData>();
	
	public List<int> skillsToBeDeleted = new List<int>();
	public List<int> itemsToBeDeleted = new List<int>();

	public Vector3 Position {
		get {
			return new Vector3(pos_x, pos_y, pos_z);
		}
		set {
			Vector3 spawnLoc = value;
					pos_x = spawnLoc.x;
					pos_y = spawnLoc.y;
					pos_z = spawnLoc.z;
			}
		}

	public string Spawn {
		get {
			return spawn;
		}
		set {
			spawn = value;
			if (spawn != null) {
				// Try to get as Scene Object
				GameObject tempObject = GameObject.Find(spawn);
				// If the object is at the Scene
				if (tempObject != null) {
					// Get object transform
					Vector3 spawnLoc = tempObject.transform.position;
				pos_x = spawnLoc.x;
				pos_y = spawnLoc.y;
				pos_z = spawnLoc.z;
				}
			}
		}
	}
	
	public CharData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"race", "string"},
		{"aspect", "string"},
		{"faction", "int"},
		{"instanceName", "string"},
		{"pos_x", "float"},
		{"pos_y", "float"},
		{"pos_z", "float"},
		{"orientation", "float"},
		{"autoAttack", "int"},
	};
	}
	
	public CharData Clone()
	{
		return (CharData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "race":
			return race;
			break;
		case "aspect":
			return aspect;
			break;
		case "faction":
			return faction.ToString();
			break;
		case "instanceName":
			return instanceName;
			break;
		case "pos_x":
			return pos_x.ToString();
			break;
		case "pos_y":
			return pos_y.ToString();
			break;
		case "pos_z":
			return pos_z.ToString();
			break;
		case "orientation":
			return orientation.ToString();
			break;
		case "autoAttack":
			return autoAttack.ToString();
			break;
		}	
		return "";
	}
		
}
