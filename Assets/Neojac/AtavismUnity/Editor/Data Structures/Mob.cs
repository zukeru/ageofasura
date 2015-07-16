using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Mob
/*
/* Table structure for table `mobtemplates`
/*
DROP TABLE IF EXISTS `mobtemplates`;
CREATE TABLE `mobtemplates` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `category` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `subTitle` varchar(64) DEFAULT NULL,
  `mobType` int(11) NOT NULL,
  `display1` varchar(30) NOT NULL DEFAULT '-1',
  `display2` varchar(30) DEFAULT NULL,
  `display3` varchar(30) DEFAULT NULL,
  `display4` varchar(30) DEFAULT NULL,
  `scale` float DEFAULT NULL,
  `hitbox` int(11) DEFAULT NULL,
  `baseAnimationState` int(11) NOT NULL DEFAULT '1',
  `faction` int(11) NOT NULL DEFAULT '0',
  `attackable` tinyint(1) NOT NULL,
  `minLevel` int(11) NOT NULL,
  `maxLevel` int(11) DEFAULT NULL,
  `species` varchar(64) NOT NULL,
  `subSpecies` varchar(64) NOT NULL,
  `questCategory` varchar(32) DEFAULT NULL,
  `specialUse` varchar(32) DEFAULT NULL,
  `speed_walk` float DEFAULT NULL,
  `speed_run` float DEFAULT NULL,
  `minDmg` int(11) DEFAULT NULL,
  `maxDmg` int(11) DEFAULT NULL,
  `attackSpeed` int(11) DEFAULT NULL,
  `dmgType` varchar(20) DEFAULT NULL,
  `primaryWeapon` int(11) DEFAULT NULL,
  `secondaryWeapon` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
*/

public class Mob: DataStructure
{
	public int id = -1;					// Database Index
	// General Parameters
	 public int category = 0; 			// Always set to 0 at the current time
	 public string name = "name";		// The mob template name
	 public string subTitle = "";		// Tag that goes under a mobs name (i.e Weapon Vendor)
	 public int mobType = 0;			// 0 = normal, -1 = untargetable/non attackable object (such as a chest), 1 = boss
	 public string species = "~ none ~";		// Humanoid, Beast, Undead, Elemental, Dragon, Unknown
	 public string subspecies = "";
	 public int faction = 0;			// The faction the mob belongs to, this will link to the faction table
	// Appearance Parameters
	//public List<GameObject> displays;		// Name of the first prefab used for display
	 public string display1 = "";		// Name of the first prefab used for display
	 public string display2 = "";		// Name of the second prefab used for display
	 public string display3 = "";		// Name of the third prefab used for display
	 public string display4 = "";		// Name of the fourth prefab used for display
	 public float scale = 1.0f;			// How big the mobs model is scaled
	 public int hitBox = 1;				// How far away the mob can be hit (generally set to 1)
	 public int baseAnimationState = 1;	// 1 = standing, 2 = swimming, 3 = flying
	 public string[] baseAnimationStateOptions = new string[] {"standing", "swimming", "flying"};
	 public float speedWalk = 3f;		// How fast the mob walks (should be about 3)
	 public float speedRun = 7f;			// How fast the mob runs (generally occurs in combat, should be around 7)
	 public int primaryWeapon = 0;		// The weapon the mob has in their main hand (this links to the inventory table)
	 public int secondaryWeapon = 0; 	// The weapon (or shield) the mob has in the off hand (this links to the inventory table) (optional)
	// Combat Parameters
	 public bool attackable = true;		// Is the mob attackable?
	 public int minLevel = 1;			// The minimum level of the mob
	 public int maxLevel = 1;			// The maximum level of the mob
	 public int minDamage = 1;			// The minimum base damage the mob deals
	 public int maxDamage = 1;			// The maximum base damage the mob deals
	 public string damageType = "~ none ~"; 		// Can be either Slash, Crush or Pierce
	 public int autoAttack = 0;

	 public float attackSpeed = 1.7f;		// How long the mob waits between attacks (in seconds)
	 public string questCategory = "";
	 public string specialUse = "";
		
	public Mob ()
	{
		// Database fields
		fields = new Dictionary<string, string> () {
		{"category", "int"},
		{"name", "string"},
		{"subTitle", "string"}, 
		{"mobType", "int"},
		{"display1", "string"},
		{"display2", "string"},
		{"display3", "string"},
		{"display4", "string"},
		{"scale", "float"},
		{"hitbox", "int"},
		{"baseAnimationState", "int"},
		{"faction", "int"},
		{"attackable", "bool"}, 
		{"minLevel", "int"},
		{"maxLevel", "int"}, 
		{"species", "string"}, 
		{"subSpecies", "string"}, 
		{"questCategory", "string"},
		{"specialUse", "string"},
		{"speed_walk", "float"},
		{"speed_run", "float"},
		{"minDmg", "int"}, 
		{"maxDmg", "int"},
		{"attackSpeed", "float"},
		{"dmgType", "string"},
		{"primaryWeapon",  "int"},
		{"secondaryWeapon",  "int"},
		{"autoAttack",  "int"},
	};

	}
	
	public Mob Clone()
	{
		return (Mob) this.MemberwiseClone();
	}
	
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "category":
			return category.ToString();
			break;
		case "name":
			return name;
			break;
		case "subTitle":
			return subTitle;
			break;
		case "mobType":
			return mobType.ToString();
			break;
		case "species": 
			return species;
			break;
		case "subSpecies": 			
			return subspecies;
			break;
		case "faction":
			return faction.ToString();
			break;
		case "display1":
			return display1;
			break;
		case "display2":
			return display2;
			break;
		case "display3": 
			return display3;
			break;
		case "display4":
			return display4;
			break;
		case "scale":
			return scale.ToString();
			break;
		case "hitbox":
			return hitBox.ToString();
			break;
		case "baseAnimationState":
			return baseAnimationState.ToString();
			break;
		case "speedWalk": 	
			return speedWalk.ToString();
			break;
		case "speedRun":
			return speedRun.ToString();
			break;
		case "primaryWeapon": 
			return primaryWeapon.ToString();
			break;
		case "secondaryWeapon": 
			return secondaryWeapon.ToString();
			break;
		case "attackable":
			return attackable.ToString();
			break;

		case "minLevel": 
			return minLevel.ToString();
			break;

		case "maxLevel": 
			return maxLevel.ToString();
			break;

		case "minDmg": 
			return minDamage.ToString();
			break;

		case "maxDmg": 
			return maxDamage.ToString();
			break;

		case "damageType": 
			return damageType;
			break;

		case "attackSpeed": 
			return attackSpeed.ToString();
			break;
		case "autoAttack": 
			return autoAttack.ToString();
			break;
		}	
		return "";
	}

			
}