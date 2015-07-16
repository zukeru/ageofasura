using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Item
/*
/* Table structure for table `itemtemplates`
/*

CREATE TABLE `itemtemplates` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `icon` varchar(64) DEFAULT NULL,
  `category` varchar(64) DEFAULT NULL,
  `subcategory` varchar(64) DEFAULT NULL,
  `itemType` varchar(64) DEFAULT NULL,
  `subType` varchar(64) DEFAULT NULL,
  `slot` varchar(64) DEFAULT NULL,
  `display` int(11) DEFAULT NULL,
  `itemQuality` tinyint(11) DEFAULT NULL,
  `binding` tinyint(11) DEFAULT NULL,
  `isUnique` tinyint(1) DEFAULT NULL,
  `stackLimit` int(11) DEFAULT NULL,
  `duration` int(11) DEFAULT NULL,
  `purchaseCurrency` tinyint(11) DEFAULT NULL,
  `purchaseCost` int(11) DEFAULT NULL,
  `sellable` tinyint(1) DEFAULT '1',
  `levelReq` int(11) DEFAULT NULL,
  `aspectReq` varchar(64) DEFAULT NULL,
  `raceReq` varchar(64) DEFAULT NULL,
  `damage` int(11) NOT NULL DEFAULT '0',
  `damageType` varchar(32) DEFAULT NULL,
  `delay` int(11) NOT NULL DEFAULT '0',
  `useAbility` int(11) NOT NULL DEFAULT '-1',
  `clickEffect` varchar(64) DEFAULT NULL,
  `toolTip` varchar(255) DEFAULT NULL,
  `triggerEvent` varchar(32) DEFAULT NULL,
  `triggerAction1Type` varchar(32) DEFAULT NULL,
  `triggerAction1Data` varchar(32) DEFAULT NULL,
  `stat1type` varchar(32) DEFAULT NULL,
  `stat1value` int(11) DEFAULT '0',
  `stat2type` varchar(32) DEFAULT NULL,
  `stat2value` int(11) DEFAULT '0',
  `stat3type` varchar(32) DEFAULT NULL,
  `stat3value` int(11) DEFAULT '0',
  `stat4type` varchar(32) DEFAULT NULL,
  `stat4value` int(11) DEFAULT '0',
  `stat5type` varchar(32) DEFAULT NULL,
  `stat5value` int(11) DEFAULT '0',
  `stat6type` varchar(32) DEFAULT NULL,
  `stat6value` int(11) DEFAULT '0',
  `res1type` varchar(32) DEFAULT NULL,
  `res1value` int(11) DEFAULT '0',
  `res2type` varchar(32) DEFAULT NULL,
  `res2value` int(11) DEFAULT '0',
  `res3type` varchar(32) DEFAULT NULL,
  `res3value` int(11) DEFAULT '0',
  `res4type` varchar(32) DEFAULT NULL,
  `res4value` int(11) DEFAULT '0',
  `res5type` varchar(32) DEFAULT NULL,
  `res5value` int(11) DEFAULT '0',
  `res6type` varchar(32) DEFAULT NULL,
  `res6value` int(11) DEFAULT '0',

*/

public class ItemEffectEntry
{
	public ItemEffectEntry(string itemEffectType, string itemEffectName, string itemEffectValue) {
		this.itemEffectType = itemEffectType;
		this.itemEffectName = itemEffectName;
		this.itemEffectValue = itemEffectValue;
	}
	
	public string itemEffectType;
	public string itemEffectName;
	public string itemEffectValue;
}

public class ItemData: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public string name = "name";		// The item template name
	public string icon = "";			// The item icon
	public string category = "0"; 		// Always set to 0 at the current time
	public string subcategory = "0";	// Always set to 0 at the current time
	public string itemType = "test";	// Can be either Weapon, Armor, Consumable, Material, Junk
	public int itemQuality = 1;  		// Ranges from 1-6 with the names: Poor, Common, Uncommon, Rare, Epic, Legendary
	public string[] itemQualityOptions = new string[] {"Poor", "Common", "Uncommon", "Rare", "Epic", "Legendary"};
	public int binding = 0; 			// 0=No binding, 1=Binds on Equip, 2=Binds on Pickup
	public string[] bindingOptions = new string[] {"No binding", "Equip", "Pickup"};
	public bool isUnique = false;		// If true, the user can only have 1 of the item
	public int stackLimit = 1;			// How many of the item can be put into 1 stack which only takes up 1 inventory slot
	public int purchaseCurrency = 0;	// What currency is required to buy this item, use 0 at the moment
	public int purchaseCost = 0;		// How much of the currency it costs to buy the item
	public bool sellable  = true;		// Can the item be sold to a vendor
	public int levelReq = 0;			// What level the player has to be to equip/use the item
	public string raceReq = "0";		// What rate the player has to be to use the item
	public string toolTip = "";		// Some text about the item (usually for fun)

	// Fields common to weapons and armor
	public string subType = "";		// Sword, Axe, Mace, Staff, Bow, Gun
	public string slot = "";			// Weapon: Main Hand, Off Hand, Two Hand - Armor: Head, Shoulder, Chest, Legs, Hands, Feet, Waist, Back 
	public string[] slotWeaponOptions = new string[] {"Main Hand", "Off Hand", " Two Hand"};
	public string[] slotArmorOptions = new string[] {"Head", "Shoulder", "Chest", "Off Hand", "Legs", "Hands", "Feet", "Waist", "Back"};
	public string display = "";			// The id of the equipment display to use (this will need further explaining)

	// Fields only for weapons
	public int damage = 0;			// How much damage the weapon does
	public string damageType = "";		// The type of damage done.
	public float delay = 1.5f;				// How long between attacks when using this weapon in seconds
	 
	public int maxEffectEntries = 12;
	public List<ItemEffectEntry> effects = new List<ItemEffectEntry>();
		
	public ItemData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"name", "string"},
		{"icon", "string"},
		{"category", "string"},
		{"subcategory", "string"},
		{"itemType", "string"}, 
		{"subType", "string"}, 
		{"slot", "string"}, 
		{"display", "string"},
		{"itemQuality", "int"}, 
		{"binding", "int"}, 
		{"isUnique", "bool"}, 
		{"stackLimit", "int"},
		{"purchaseCurrency", "int"},
		{"purchaseCost", "int"},
		{"sellable", "bool"},
		{"levelReq", "int"},
		{"raceReq", "string"}, 
		{"damage", "int"},
		{"damageType", "string"}, 
		{"delay", "float"},
		{"toolTip", "string"},
		{"effect1type", "string"},
		{"effect1name", "string"},
		{"effect1value", "string"},
		{"effect2type", "string"},
		{"effect2name", "string"},
		{"effect2value", "string"},
		{"effect3type", "string"},
		{"effect3name", "string"},
		{"effect3value", "string"},
		{"effect4type", "string"},
		{"effect4name", "string"},
		{"effect4value", "string"},
		{"effect5type", "string"},
		{"effect5name", "string"},
		{"effect5value", "string"},
		{"effect6type", "string"},
		{"effect6name", "string"},
		{"effect6value", "string"},
		{"effect7type", "string"},
		{"effect7name", "string"},
		{"effect7value", "string"},
		{"effect8type", "string"},
		{"effect8name", "string"},
		{"effect8value", "string"},
		{"effect9type", "string"},
		{"effect9name", "string"},
		{"effect9value", "string"},
		{"effect10type", "string"},
		{"effect10name", "string"},
		{"effect10value", "string"},
		{"effect11type", "string"},
		{"effect11name", "string"},
		{"effect11value", "string"},
		{"effect12type", "string"},
		{"effect12name", "string"},
		{"effect12value", "string"},
	};
	}
	
	public ItemData Clone()
	{
		return (ItemData) this.MemberwiseClone();
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
		case "category":
			return category;
			break;
		case "subcategory":
			return subcategory;
			break;	
		case "itemType":
			return itemType;
			break;
		case "subType":
			return subType;
			break;
		case "slot":
			return slot;
			break;
		case "display":
			return display;
			break;
		case "itemQuality":
			return itemQuality.ToString();
			break;
		case "binding":
			return binding.ToString();
			break;
		case "isUnique":
			return isUnique.ToString();
			break;
		case "stackLimit":
			return stackLimit.ToString();
			break;			
		case "purchaseCurrency":
			return purchaseCurrency.ToString();
			break;
		case "purchaseCost":
			return purchaseCost.ToString();
			break;
		case "sellable":
			return sellable.ToString();
			break;
		case "levelReq":
			return levelReq.ToString();
			break;
		case "raceReq":
			return raceReq;
			break;
		case "damage":
			return damage.ToString();
			break;
		case "damageType":
			return damageType;
			break;
		case "delay":
			return delay.ToString();
			break;
		case "toolTip":
			return toolTip;
			break;
		case "effect1type":
			return getEffectData(0, "type");
			break;
		case "effect1name":
			return getEffectData(0, "name");
			break;
		case "effect1value":
			return getEffectData(0, "value");
			break;
		case "effect2type":
			return getEffectData(1, "type");
			break;
		case "effect2name":
			return getEffectData(1, "name");
			break;
		case "effect2value":
			return getEffectData(1, "value");
			break;
		case "effect3type":
			return getEffectData(2, "type");
			break;
		case "effect3name":
			return getEffectData(2, "name");
			break;
		case "effect3value":
			return getEffectData(2, "value");
			break;
		case "effect4type":
			return getEffectData(3, "type");
			break;
		case "effect4name":
			return getEffectData(3, "name");
			break;
		case "effect4value":
			return getEffectData(3, "value");
			break;
		case "effect5type":
			return getEffectData(4, "type");
			break;
		case "effect5name":
			return getEffectData(4, "name");
			break;
		case "effect5value":
			return getEffectData(4, "value");
			break;
		case "effect6type":
			return getEffectData(5, "type");
			break;
		case "effect6name":
			return getEffectData(5, "name");
			break;
		case "effect6value":
			return getEffectData(5, "value");
			break;
		case "effect7type":
			return getEffectData(6, "type");
			break;
		case "effect7name":
			return getEffectData(6, "name");
			break;
		case "effect7value":
			return getEffectData(6, "value");
			break;
		case "effect8type":
			return getEffectData(7, "type");
			break;
		case "effect8name":
			return getEffectData(7, "name");
			break;
		case "effect8value":
			return getEffectData(7, "value");
			break;
		case "effect9type":
			return getEffectData(8, "type");
			break;
		case "effect9name":
			return getEffectData(8, "name");
			break;
		case "effect9value":
			return getEffectData(8, "value");
			break;
		case "effect10type":
			return getEffectData(9, "type");
			break;
		case "effect10name":
			return getEffectData(9, "name");
			break;
		case "effect10value":
			return getEffectData(9, "value");
			break;
		case "effect11type":
			return getEffectData(10, "type");
			break;
		case "effect11name":
			return getEffectData(10, "name");
			break;
		case "effect11value":
			return getEffectData(10, "value");
			break;
		case "effect12type":
			return getEffectData(11, "type");
			break;
		case "effect12name":
			return getEffectData(11, "name");
			break;
		case "effect12value":
			return getEffectData(11, "value");
			break;
		}	
		return "";
	}
	
	string getEffectData(int effectNum, string field) {
		if (effects.Count > effectNum) {
			if (field == "type") {
				return effects[effectNum].itemEffectType;
			} else if (field == "name") {
				return effects[effectNum].itemEffectName;
			} else if (field == "value") {
				return effects[effectNum].itemEffectValue;
			} else {
				return "";
			}
		} else {
			return "";
		}
	}
		
}