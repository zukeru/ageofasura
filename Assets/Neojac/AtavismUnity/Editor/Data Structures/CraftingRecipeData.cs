using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Loot Table
/*
/* Table structure for tables
/*
CREATE TABLE `loot_tables` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `item1` int(11) NOT NULL DEFAULT '-1',
  `item1count` int(11) DEFAULT NULL,
  `item1chance` int(11) DEFAULT NULL,
  `item2` int(11) NOT NULL DEFAULT '-1',
  `item2count` int(11) DEFAULT NULL,
  `item2chance` int(11) DEFAULT NULL,
  `item3` int(11) NOT NULL DEFAULT '-1',
  `item3count` int(11) DEFAULT NULL,
  `item3chance` int(11) DEFAULT NULL,
  `item4` int(11) NOT NULL DEFAULT '-1',
  `item4count` int(11) DEFAULT NULL,
  `item4chance` int(11) DEFAULT NULL,
  `item5` int(11) NOT NULL DEFAULT '-1',
  `item5count` int(11) DEFAULT NULL,
  `item5chance` int(11) DEFAULT NULL,
  `item6` int(11) NOT NULL DEFAULT '-1',
  `item6count` int(11) DEFAULT NULL,
  `item6chance` int(11) DEFAULT NULL,
  `item7` int(11) NOT NULL DEFAULT '-1',
  `item7count` int(11) DEFAULT NULL,
  `item7chance` int(11) DEFAULT NULL,
  `item8` int(11) NOT NULL DEFAULT '-1',
  `item8count` int(11) DEFAULT NULL,
  `item8chance` int(11) DEFAULT NULL,
  `item9` int(11) NOT NULL DEFAULT '-1',
  `item9count` int(11) DEFAULT NULL,
  `item9chance` int(11) DEFAULT NULL,
  `item10` int(11) NOT NULL DEFAULT '-1',
  `item10count` int(11) DEFAULT NULL,
  `item10chance` int(11) DEFAULT NULL,
  `category` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`),
  KEY `item1` (`item1`)
)
*/

public class RecipeComponentEntry
{
	public RecipeComponentEntry(int itemId, int count) {
		this.itemId = itemId;
		this.count = count;
	}
	
	public int itemId;
	public  int count = 1;
}

public class CraftingRecipe: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public string icon = "";
	public int resultItemID = -1;
	public int resultItemCount = 1;
	public int skillID = -1;
	public int skillLevelReq = 1;
	public string stationReq = "";
	public int recipeItemID = -1;
	public bool layoutReq = true;
	public bool qualityChangeable = false;
	public bool allowDyes = true;
	public bool allowEssences = false;
	public int maxEntries = 16; // 4 rows of 4
	public List<RecipeComponentEntry> entries = new List<RecipeComponentEntry>();
	
	public CraftingRecipe ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"name", "string"},
		{"icon", "string"},
		{"resultItemID", "int"},
		{"resultItemCount", "int"},
		{"skillID", "int"},
		{"skillLevelReq", "int"},
		{"stationReq", "string"},
		{"recipeItemID", "int"},
		{"layoutReq", "bool"},
		{"qualityChangeable", "bool"},
		{"allowDyes", "bool"},
		{"allowEssences", "bool"},
		{"component1", "int"},
		{"component1count", "int"},
		{"component2", "int"},
		{"component2count", "int"},
		{"component3", "int"},
		{"component3count", "int"},
		{"component4", "int"},
		{"component4count", "int"},
		{"component5", "int"},
		{"component5count", "int"},
		{"component6", "int"},
		{"component6count", "int"},
		{"component7", "int"},
		{"component7count", "int"},
		{"component8", "int"},
		{"component8count", "int"},
		{"component9", "int"},
		{"component9count", "int"},
		{"component10", "int"},
		{"component10count", "int"},
		{"component11", "int"},
		{"component11count", "int"},
		{"component12", "int"},
		{"component12count", "int"},
		{"component13", "int"},
		{"component13count", "int"},
		{"component14", "int"},
		{"component14count", "int"},
		{"component15", "int"},
		{"component15count", "int"},
		{"component16", "int"},
		{"component16count", "int"}
	};
	}
	
	public CraftingRecipe Clone()
	{
		return (CraftingRecipe) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "name":
			return name;
			break;
		case "icon":
			return icon;
			break;
		case "resultItemID":
			return resultItemID.ToString();
			break;
		case "resultItemCount":
			return resultItemCount.ToString();
			break;
		case "skillID":
			return skillID.ToString();
			break;	
		case "skillLevelReq":
			return skillLevelReq.ToString();
			break;	
		case "stationReq":
			return stationReq;
			break;
		case "recipeItemID":
			return recipeItemID.ToString();
			break;
		case "layoutReq":
			return layoutReq.ToString();
			break;
		case "qualityChangeable":
			return qualityChangeable.ToString();
			break;
		case "allowDyes":
			return allowDyes.ToString();
			break;
		case "allowEssences":
			return allowEssences.ToString();
			break;
		case "component1":
				return entries[0].itemId.ToString();
			break;
		case "component1count":
				return entries[0].count.ToString();
			break;
		case "component2":
				return entries[1].itemId.ToString();
			break;
		case "component2count":
				return entries[1].count.ToString();
			break;
		case "component3":
				return entries[2].itemId.ToString();
			break;
		case "component3count":
				return entries[2].count.ToString();
			break;
		case "component4":
				return entries[3].itemId.ToString();
			break;
		case "component4count":
				return entries[3].count.ToString();
			break;
		case "component5":
				return entries[4].itemId.ToString();
			break;
		case "component5count":
				return entries[4].count.ToString();
			break;
		case "component6":
				return entries[5].itemId.ToString();
			break;
		case "component6count":
				return entries[5].count.ToString();
			break;
		case "component7":
				return entries[6].itemId.ToString();
			break;
		case "component7count":
				return entries[6].count.ToString();
			break;
		case "component8":
			return entries[7].itemId.ToString();
			break;
		case "component8count":
			return entries[7].count.ToString();
			break;
		case "component9":
			return entries[8].itemId.ToString();
			break;
		case "component9count":
			return entries[8].count.ToString();
			break;
		case "component10":
			return entries[9].itemId.ToString();
			break;
		case "component10count":
			return entries[9].count.ToString();
			break;
		case "component11":
			return entries[10].itemId.ToString();
			break;
		case "component11count":
			return entries[10].count.ToString();
			break;
		case "component12":
			return entries[11].itemId.ToString();
			break;
		case "component12count":
			return entries[11].count.ToString();
			break;
		case "component13":
			return entries[12].itemId.ToString();
			break;
		case "component13count":
			return entries[12].count.ToString();
			break;
		case "component14":
			return entries[13].itemId.ToString();
			break;
		case "component14count":
			return entries[13].count.ToString();
			break;
		case "component15":
			return entries[14].itemId.ToString();
			break;
		case "component15count":
			return entries[14].count.ToString();
			break;
		case "component16":
			return entries[15].itemId.ToString();
			break;
		case "component16count":
			return entries[15].count.ToString();
			break;
		}
		
		return "";
	}
		
}