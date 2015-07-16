using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Stats
/*
/* Table structure for tables
/*
CREATE TABLE `character_create_stats` (
  `id` int(11) NOT NULL,
  `character_create_id` int(11) NOT NULL,
  `stat` varchar(45) NOT NULL,
  `value` int(11) NOT NULL,
*/

public class CharStatsData: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public int charId = 0;					// Database Index
	public string stat = "none";		// The ability template name
	public int statValue = 10;
	public float levelIncrease = 0;
	public float levelPercentIncrease = 0;
	
	public CharStatsData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"character_create_id", "int"},
		{"stat", "string"},
		{"value", "int"},
		{"levelIncrease", "float"},
		{"levelPercentIncrease", "float"},
	};
	}
	
	public CharStatsData Clone()
	{
		return (CharStatsData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "stat":
			return stat;
			break;
		case "character_create_id":
			return charId.ToString();
			break;
		case "value":
			return statValue.ToString();
			break;
		case "levelIncrease":
			return levelIncrease.ToString();
			break;
		case "levelPercentIncrease":
			return levelPercentIncrease.ToString();
			break;
		}	
		
		return "";
	}
		
}