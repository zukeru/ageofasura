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
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `displayName` varchar(64) DEFAULT NULL,
  `icon` varchar(64) DEFAULT NULL,
  `effectType` varchar(64) DEFAULT NULL,
  `effectFamily` int(11) DEFAULT NULL,
  `isBuff` tinyint(1) NOT NULL DEFAULT '0',
  `skillType` int(11) DEFAULT NULL,
  `passive` tinyint(1) DEFAULT NULL,
  `stackLimit` int(11) DEFAULT NULL,
  `allowMultiple` tinyint(1) DEFAULT NULL,
  `duration` int(11) DEFAULT NULL,
  `numPulses` int(11) DEFAULT NULL,
  `tooltip` varchar(255) DEFAULT NULL,
  `bonusEffectReq` int(11) DEFAULT NULL,
  `bonusEffectReqConsumed` tinyint(1) DEFAULT NULL,
  `bonusEffect` int(11) NOT NULL DEFAULT '-1',
  `pulseParticle` varchar(32) DEFAULT NULL,
    
*/

public class EffectsData: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public string name = "name";		// The ability template name
	
	//public string displayName = "";
	public string icon = "";
	public string effectType = "";
	public string effectMainType = "";
	public string[] effectMainTypeOptions = new string[] {"Damage", "Restore", "Stat"};
	//public string effectDamageType = "";
	public string[] effectDamageTypeOptions = new string[] {"MeleeStrikeEffect", "MagicalStrikeEffect", "PhysicalDotEffect", "MagicalDotEffect", "FlatDamageEffect"};
	//public string effectHealType = "";
	public string[] effectHealTypeOptions = new string[] {"HealInstantEffect", "HealOverTimeEffect", "HealthTransferEffect"};
	//public string effectStatType = "";
	public string[] effectStatTypeOptions = new string[] {"StatEffect"};
	// public int effectFamily = 0;
	public bool isBuff = false;
	public int skillType = 0;
	public float skillLevelMod = 0;
	public bool passive = false;
	public int stackLimit = 0;
	public bool allowMultiple = false;
	public float duration = 0;
	public int pulseCount = 1;
	public string tooltip = "";
	public int bonusEffectReq = 0;
	public bool bonusEffectReqConsumed = false;
	public int bonusEffect = 0;
	public string pulseParticle = "";

	// To transfer to heal table
	public int healAmount; 				// the amount of healing this effect does to the target
	public string healProperty = ""; 		// either health or energy
	public float healthTransferRate = 0;		// the amount of health removed from the caster based on the healing done to the target. 

	// To transfer to the damage table
	public int damageAmount; 				// the amount of damage this effect does to the target
	public string damageType = ""; 		//  the type of damage this effect does - values should be from the damage_types table
	public string damageProperty = ""; 		// either "health" or "mana"
	public float damageMod = 1.0f;			// Modifies the damage done by this effect. The damage of the effect is multiplied by this number, so a value of 2 means double damage.
	public int bonusDamageEffect = 0;					// the ID of an effect, which if present, causes this effect to do more damage
	public int bonusDamageAmount = 0;				// the amount of bonus damage applied if the bonus effect is present
	//public float healthTransferRate = 0;			// the amount of health gained based on the damage done to the target.
	
	// To transfer to the stat table
	public bool modifyStatsByPercent; 	// should the stats modified by this effect be modified by a percent rather than a flat value
	public string stat1Name = ""; 		// the name of the first stat
	public float stat1Modification = 0;		// the amount the first stat is modified by
	public string stat2Name = ""; 		// the name of the second stat
	public float stat2Modification = 0;		// the amount the second stat is modified by
	public string stat3Name = ""; 		// the name of the third stat
	public float stat3Modification = 0;		// the amount the third stat is modified by
	public string stat4Name = ""; 		// the name of the fourth stat
	public float stat4Modification = 0;		// the amount the fourth stat is modified by
	public string stat5Name = ""; 		// the name of the fifth stat
	public float stat5Modification = 0;		// the amount the fifth stat is modified by

	public string abilityType; 
	
	public EffectsData ()
	{
		// Database fields
		fields = new Dictionary<string, string> () {
		{"name", "string"},
		//{"displayName", "string"},
		{"icon", "string"},
		{"effectMainType", "string"},
		{"effectType", "string"},
		//{"effectFamily", "int"},
		{"isBuff", "bool"},
		{"skillType", "int"},
		{"skillLevelMod", "float"},
		{"passive", "bool"},
		{"stackLimit", "int"},
		{"allowMultiple", "bool"},
		{"duration", "float"},
		{"pulseCount", "int"},
		{"tooltip", "string"},
		{"bonusEffectReq", "int"},
		{"bonusEffectReqConsumed", "bool"},
		{"bonusEffect", "int"},
		{"pulseParticle", "string"},
	};
	}	
	
	public EffectsData Clone()
	{
		return (EffectsData) this.MemberwiseClone();
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
		//case "displayName":
		//	return displayName;
		//	break;
		case "icon":
			return icon;
			break;
		case "effectMainType":
			return effectMainType;
			break;
		case "effectType":
			return effectType;
			break;
		//case "effectFamily":
			//	return effectFamily.ToString();
			//break;
		case "isBuff":
			return isBuff.ToString();
			break;
		case "skillType":
			return skillType.ToString();
			break;
		case "skillLevelMod":
			return skillLevelMod.ToString();
			break;
		case "passive":
			return passive.ToString();
			break;
		case "stackLimit":
			return stackLimit.ToString();
			break;
		case "allowMultiple":
			return allowMultiple.ToString();
			break;
		case "duration":
			return duration.ToString();
			break;
		case "pulseCount":
			return pulseCount.ToString();
			break;
		case "tooltip":
			return tooltip.ToString();
			break;
		case "bonusEffectReq":
			return bonusEffectReq.ToString();
			break;
		case "bonusEffectReqConsumed":
			return bonusEffectReqConsumed.ToString();
			break;
		case "bonusEffect":
			return bonusEffect.ToString();
			break;
		case "pulseParticle":
			return pulseParticle;
			break;
		}	
		return "";
	}
		
}