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

public class CoordEffectData: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public string name = "";
	public string prefab = "";
	
	public CoordEffectData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
			{"name", "string"},
			{"prefab", "string"},
	};
	}
	
	public CoordEffectData Clone()
	{
		return (CoordEffectData) this.MemberwiseClone();
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
		case "prefab":
			return prefab;
			break;
		}	
		return "";
	}
		
}