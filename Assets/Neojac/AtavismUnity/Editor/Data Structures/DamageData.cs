using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Skills
/*
/* Table structure for tables related to effects
/*

CREATE TABLE `effects` (
 `name` varchar(45) NOT NULL,
  `resistance_stat` varchar(45) NOT NULL,
 Damage type table:

name - String - the name of the damage type
resistance_stat - String - the name of the stat that is used to reduce damage from this damage type (this should have a drop down that links to the list of stats created in the previous table).
The idea is that someone may create a stat called fire_resistance 
then create a damage type called fire, which uses the fire_resistance stat as the resistance_stat.


    
*/

public class DamageData: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	 public string name = "name";		
	 public string resistanceStat = "";

	public string abilityType; 
	
	public DamageData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"name", "string"},
		{"resistance_stat", "string"}
	};
	}	
	
	public DamageData Clone()
	{
		return (DamageData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "name":
			return name;
			break;
		case "resistance_stat":
			return resistanceStat;
			break;
		}	
		return "";
	}
		
}