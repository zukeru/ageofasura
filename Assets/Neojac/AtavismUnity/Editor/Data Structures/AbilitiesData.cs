using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Abilities
/*
/* Table structure for tables related to abilities
/*

CREATE TABLE `abilities` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `abilityType` varchar(64) DEFAULT NULL,
  `skill` int(11) DEFAULT NULL,
  `passive` tinyint(1) DEFAULT NULL,
  `activationCost` int(11) DEFAULT NULL,
  `activationCostType` varchar(32) DEFAULT NULL,
  `activationLength` int(11) DEFAULT NULL,
  `activationAnimation` varchar(32) DEFAULT NULL,
  `activationParticles` varchar(32) DEFAULT NULL,
  `casterEffectRequired` int(11) DEFAULT NULL,
  `casterEffectConsumed` tinyint(1) DEFAULT NULL,
  `targetEffectRequired` int(11) DEFAULT NULL,
  `targetEffectConsumed` tinyint(1) DEFAULT NULL,
  `weaponRequired` varchar(32) DEFAULT NULL,
  `reagentRequired` int(11) NOT NULL DEFAULT '-1',
  `reagentConsumed` tinyint(1) DEFAULT NULL,
  `maxRange` int(11) DEFAULT NULL,
  `minRange` int(11) DEFAULT NULL,
  `aoeRadius` int(11) NOT NULL DEFAULT '0',
  `targetType` varchar(32) DEFAULT NULL,
  `targetState` int(11) DEFAULT NULL,
  `speciesTargetReq` varchar(32) DEFAULT NULL,
  `specificTargetReq` varchar(64) DEFAULT NULL,
  `globalCooldown` tinyint(1) DEFAULT NULL,
  `cooldown1Type` varchar(32) DEFAULT NULL,
  `cooldown1Duration` int(11) DEFAULT NULL,
  `weaponCooldown` tinyint(1) DEFAULT NULL,
  `activationEffect1` int(1) DEFAULT NULL,
  `activationTarget1` varchar(32) DEFAULT NULL,
  `activationEffect2` int(11) DEFAULT NULL,
  `activationTarget2` varchar(32) DEFAULT NULL,
  `activationEffect3` int(11) DEFAULT NULL,
  `activationTarget3` varchar(32) DEFAULT NULL,
  `coordEffect1event` varchar(32) DEFAULT NULL,
  `coordEffect1` varchar(64) DEFAULT NULL,
  `coordEffect2event` varchar(32) DEFAULT NULL,
  `coordEffect2` varchar(64) DEFAULT NULL,
    
The ability system is complicated because each ability can have up to 3 effects (which are a separate table)
 and many of the fields for the ability link back to effects. 
 
 The system allows for adding effect requirements to activate an ability or add bonuses and the like.

The other part is coordinated effects. These are special effect tools that allow things like animations,
 particles and sounds to be played when using an ability. These are defined solely on the client (as they have no effect on the game, it's all cosmetic).

*/

public class AbilitiesData: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public string name = "name";		// The ability template name
	public string icon = "";			// The ability icon
	public string abilityType = ""; 
	public string[] abilityTypeOptions = new string[] {"CombatMeleeAbility", "MagicalAttackAbility", "EffectAbility", "FriendlyEffectAbility"};
	public int skill = -1; 					// what skill this ability belongs to
	public bool passive = false; 				// Does this ability auto apply it's effects as soon as it is learned, or is it an ability that requires activation
	public int activationCost = 0; 			// The value of the specified resource required to activate the ability
	public string activationCostType = ""; 	// What resource this ability requires to activate (either mana or health)
	public string[] activationCostTypeOptions = new string[] {"mana", "health", "other"};
	public float activationLength = 0; 		// How long it takes to activate the ability (cast time). The player cannot move while casting
	public string activationAnimation = ""; 	// The animation to play while casting
	public string activationParticles = ""; 	// The particle to apply while casting
	public int casterEffectRequired = 0; 	// Does the caster require an effect on them to activate the ability (generally set to 0: no, otherwise the id of the effect required)
	public bool casterEffectConsumed = false; 	// Is the effect removed from the caster
	public int targetEffectRequired = 0; 	// Does the targetrequire an effect on them for the ability to activate (generally set to 0: no, otherwise the id of the effect required)
	public bool targetEffectConsumed; 	// Is the effect removed from the target
	public string weaponRequired = ""; 		// What weapon type is required to activate the ability (None, Sword, Axe, Mace etc);
	public int reagentRequired = 0; 		// Is an item required to activate this ability, if so, which one (0: no, otherwise the id of the item)
	public bool reagentConsumed = false; 		// Is the item deleted from the players inventory upon activation
	public int maxRange = 4; 				// How far away the target can be (in metres)
	public int minRange = 0; 				// How close the target can be (in metres)
	public int aoeRadius = 1; 				// how wide of an area the ability hits (in metres)
	public string targetType = ""; 			// Any: can hit anyone; Enemy: can only be used on hostile targets; Self: can only be used on the caster; Friendly: can be used on a friendly unit; FriendNotSelf: a friendly unit, but not the caster; Group: can only target someone in the casters group; AoE Enemy: multiple enemy targets; AoE Friendly: multiple friendly targets
	public int targetState = 1; 			// 0: Dead; 1; // Alive
	public string[] targetStateOptions = new string[] {"Dead", "Alive"};
	public string speciesTargetReq = ""; 	// One of the mob species (e.g. Humanoid, Beast, Dragon, Elemental, Undead, None)
	public string specificTargetReq = ""; 	// Is there a specific target (such as a certain mob) that the ability can be used on; // probably best to hide this for now.
	public bool globalCooldown = true; 		// Does this ability trigger the global cooldown
	public string cooldown1Type = ""; 		// The name of the individual cooldown this ability triggers
	public float cooldown1Duration = 0; 		// How long the cooldown lasts
	public bool weaponCooldown = false; 		// Does this ability trigger the weapon cooldown
	public int activationEffect1 = 0; 		// The id of the effect this ability applies
	public string activationTarget1 = ""; 	// Is this effect applied to the caster (true) or the target (false)
	public string[] activationTarget1Options = new string[] {"target", "caster"};
	public int activationEffect2 = 0; 		// The id of the second effect this ability applies (optional)
	public string activationTarget2 = ""; 	// Is this effect applied to the caster (true) or the target (false)
	public string[] activationTarget2Options = new string[] {"target", "caster"};
	public int activationEffect3 = 0; 		// The id of the third effect this ability applies (optional)
	public string activationTarget3 = ""; 	// Is this effect applied to the caster (true) or the target (false)
	public string[] activationTarget3Options = new string[] {"target", "caster"};
	public string coordEffect1 = ""; 		// The name of the coordinated effect this ability activated (optional)
	public string coordEffect1Event = ""; 	// When the coordinated effect is activated, can be: activating, activated, initializing, channelling, interrupted, failed
	public string[] coordEffect1EventOptions = new string[] {"completed", "activating", "activated", "initializing", "channelling", "interrupted", "failed"};
	public string coordEffect2 = ""; 		// The name of the second coordinated effect this ability activated (optional)
	public string coordEffect2Event = ""; 	// When the second coordinated effect is activated, can be: activating, activated, initializing, channelling, interrupted, failed
	public string[] coordEffect2EventOptions = new string[] {"completed", "activating", "activated", "initializing", "channelling", "interrupted", "failed"};
	public string tooltip = "";

	public AbilitiesData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"name", "string"},
		{"icon", "string"},
		{"abilityType", "string"},
		{"skill", "int"},
		{"passive", "bool"},
		{"activationCost", "int"},
		{"activationCostType", "string"},
		{"activationLength", "float"},
		{"activationAnimation", "string"},
		{"activationParticles", "string"},
		{"casterEffectRequired", "int"},
		{"casterEffectConsumed", "bool"},
		{"targetEffectRequired", "int"},
		{"targetEffectConsumed", "bool"},
		{"weaponRequired", "string"},
		{"reagentRequired", "int"},
		{"reagentConsumed", "bool"},
		{"maxRange", "int"},
		{"minRange", "int"},
		{"aoeRadius", "int"},
		{"targetType", "string"},
		{"targetState", "int"},
		{"speciesTargetReq", "string"},
		{"specificTargetReq", "string"},
		{"globalCooldown", "bool"},
		{"cooldown1Type", "string"},
		{"cooldown1Duration", "float"},
		{"weaponCooldown", "bool"},
		{"activationEffect1", "int"},
		{"activationTarget1", "string"},
		{"activationEffect2", "int"},
		{"activationTarget2", "string"},
		{"activationEffect3", "int"},
		{"activationTarget3", "string"},
		{"coordEffect1event", "string"},
		{"coordEffect1", "string"},
		{"coordEffect2event", "string"},
		{"coordEffect2", "string"},
		{"tooltip", "string"},
	};
	}
	
	public AbilitiesData Clone()
	{
		return (AbilitiesData) this.MemberwiseClone();
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
		case "abilityType":
			return abilityType;
			break;
		case "skill":
			return skill.ToString();
			break;
		case "passive":
			return passive.ToString();
			break;
		case "activationCost":
			return activationCost.ToString();
			break;
		case "activationCostType":
			return activationCostType;
			break;
		case "activationLength":
			return activationLength.ToString();
			break;
		case "activationAnimation":
			return activationAnimation;
			break;
		case "activationParticles":
			return activationParticles;
			break;
		case "casterEffectRequired":
			return casterEffectRequired.ToString();
			break;
		case "casterEffectConsumed":
			return casterEffectConsumed.ToString();
			break;
		case "targetEffectRequired":
			return targetEffectRequired.ToString();
			break;
		case "targetEffectConsumed":
			return targetEffectConsumed.ToString();
			break;
		case "weaponRequired":
			return weaponRequired;
			break;
		case "reagentRequired":
			return reagentRequired.ToString();
			break;
		case "reagentConsumed":
			return reagentConsumed.ToString();
			break;
		case "maxRange":
			return maxRange.ToString();
			break;
		case "minRange":
			return minRange.ToString();
			break;
		case "aoeRadius":
			return aoeRadius.ToString();
			break;
		case "targetType":
			return targetType;
			break;
		case "targetState":
			return targetState.ToString();
			break;
		case "speciesTargetReq":
			return speciesTargetReq;
			break;
		case "specificTargetReq":
			return specificTargetReq;
			break;
		case "globalCooldown":
			return globalCooldown.ToString();
			break;
		case "cooldown1Type":
			return cooldown1Type;
			break;
		case "cooldown1Duration":
			return cooldown1Duration.ToString();
			break;
		case "weaponCooldown":
			return weaponCooldown.ToString();
			break;
		case "activationEffect1":
			return activationEffect1.ToString();
			break;
		case "activationTarget1":
			return activationTarget1;
			break;
		case "activationEffect2":
			return activationEffect2.ToString();
			break;
		case "activationTarget2":
			return activationTarget2;
			break;
		case "activationEffect3":
			return activationEffect3.ToString();
			break;
		case "activationTarget3":
			return activationTarget3.ToString();
			break;
		case "coordEffect1event":
			return coordEffect1Event;
			break;
		case "coordEffect1":
			return coordEffect1;
			break;
		case "coordEffect2event":
			return coordEffect2Event;
			break;
		case "coordEffect2":
			return coordEffect2;
			break;
		case "tooltip":
			return tooltip;
			break;
		}	
		return "";
	}
		
}