using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Stats
/*
/* Table structure for tables
/*
 CREATE TABLE `stat` (
  `name` varchar(45) NOT NULL,
  `type` int(11) DEFAULT '0',
  `stat_function` varchar(45) DEFAULT NULL,
  
  Stat table:

name - String - The name of the stat.
type - Integer - Use a drop down with the following: 
(0: Base stat - this is for like strength, agility etc; and 1: Resistance stat - for armour etc)
stat_function - String - What function the stat serves, only used for base stats. 
It only wants the following options: 
health_mod, mana_mod (these two effect the character's health/mana). physical_power, magical_power (these two effect the damage done by the characters abilities). physical_accuracy, magical_accuracy (these two effect the chance of hitting with an ability). See my attached sql file to see examples.  
*/

public class StatsData: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public string name = "name";		// The stat name
    public string originalName = "";
	public int type = 0;
	public string[] typeOptions = new string[] {"Base stat", "Resistance stat", "Vitality stat"};
	public string statFunction = "~ none ~";
	public int mobBase = 0;
	public int mobLevelIncrease = 0;
	public float mobLevelPercentIncrease = 0;
	public int min = 0;
	public string maxstat = "";
	public string[] targetOptions = new string[] {"All", "Player Only", "Mob Only"};
	public int shiftTarget = 2;
	public int shiftValue = 0;
	public int shiftReverseValue = 0;
	public int shiftInterval = 2;
	public bool isShiftPercent = true;
	public string onMaxHit = "";
	public string onMinHit = "";
	public string shiftReq1 = "";
	public bool shiftReq1State = false;
	public bool shiftReq1SetReverse = false;
	public string shiftReq2 = "";
	public bool shiftReq2State = false;
	public bool shiftReq2SetReverse = false;
	public string shiftReq3 = "";
	public bool shiftReq3State = false;
	public bool shiftReq3SetReverse = false;
	
	public StatsData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"name", "string"},
		{"type", "int"},
		{"stat_function", "string"},
		{"mob_base", "int"},
		{"mob_level_increase", "int"},
		{"mob_level_percent_increase", "float"},
		{"min", "int"},
		{"maxstat", "string"},
		{"shiftTarget", "int"},
		{"shiftValue", "int"},
		{"shiftReverseValue", "int"},
		{"shiftInterval", "int"},
		{"isShiftPercent", "bool"},
		{"onMaxHit", "string"},
		{"onMinHit", "string"},
		{"shiftReq1", "string"},
		{"shiftReq1State", "bool"},
		{"shiftReq1SetReverse", "bool"},
		{"shiftReq2", "string"},
		{"shiftReq2State", "bool"},
		{"shiftReq2SetReverse", "bool"},
		{"shiftReq3", "string"},
		{"shiftReq3State", "bool"},
		{"shiftReq3SetReverse", "bool"},
	};
	}
	
	public StatsData Clone()
	{
		return (StatsData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "name":
			return name;
			break;
		case "type":
			return type.ToString();
			break;
		case "stat_function":
			return statFunction;
			break;
		case "mob_base":
			return mobBase.ToString();
			break;
		case "mob_level_increase":
			return mobLevelIncrease.ToString();
			break;
		case "mob_level_percent_increase":
			return mobLevelPercentIncrease.ToString();
			break;
		case "min":
			return min.ToString();
			break;
		case "maxstat":
			return maxstat;
			break;
		case "shiftTarget":
			return shiftTarget.ToString();
			break;
		case "shiftValue":
			return shiftValue.ToString();
			break;
		case "shiftReverseValue":
			return shiftReverseValue.ToString();
			break;
		case "shiftInterval":
			return shiftInterval.ToString();
			break;
		case "isShiftPercent":
			return isShiftPercent.ToString();
			break;
		case "onMaxHit":
			return onMaxHit;
			break;
		case "onMinHit":
			return onMinHit;
			break;
		case "shiftReq1":
			return shiftReq1;
			break;
		case "shiftReq1State":
			return shiftReq1State.ToString();
			break;
		case "shiftReq1SetReverse":
			return shiftReq1SetReverse.ToString();
			break;
		case "shiftReq2":
			return shiftReq2;
			break;
		case "shiftReq2State":
			return shiftReq2State.ToString();
			break;
		case "shiftReq2SetReverse":
			return shiftReq2SetReverse.ToString();
			break;
		case "shiftReq3":
			return shiftReq3;
			break;
		case "shiftReq3State":
			return shiftReq3State.ToString();
			break;
		case "shiftReq3SetReverse":
			return shiftReq3SetReverse.ToString();
			break;
		}
		return "";
	}
		
}