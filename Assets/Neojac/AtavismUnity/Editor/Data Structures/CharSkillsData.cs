using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Stats
/*
/* Table structure for tables
/*

CREATE TABLE `character_create_skills` (
  `id` int(11) NOT NULL,
  `character_create_id` int(11) NOT NULL,
  `skill` int(11) NOT NULL,
*/

public class CharSkillsData: DataStructure
{
	public int id = -1;					// Database Index
	// General Parameters
	public int charId = -1;					// Database Index
	public int skill = -1;
	
	public CharSkillsData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"character_create_id", "int"},
		{"skill", "int"},
	};
	}
	
	public CharSkillsData Clone()
	{
		return (CharSkillsData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "character_create_id":
			return charId.ToString();
			break;
		case "skill":
			return skill.ToString();
			break;
		}	
		return "";
	}
		
}