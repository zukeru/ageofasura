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

public class MobLoot: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public int category = 0;
	public int mobTemplate;
	public int tableId = -1;
	public int chance = 0;
	
	public MobLoot ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"category", "int"},
		{"mobTemplate", "int"},
		{"lootTable", "int"},
		{"dropChance", "int"}
	};
	}
	
	public MobLoot Clone()
	{
		return (MobLoot) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "category":
			return category.ToString();
			break;
		case "mobTemplate":
			return mobTemplate.ToString();
			break;
		case "lootTable":
			return tableId.ToString();
			break;
		case "dropChance":
			return chance.ToString();
			break;
		}	
		return "";
	}
		
}