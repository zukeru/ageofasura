using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

/* Structure 
id - Integer - links to id of parent table
damageAmount - Integer - the amount of damage this effect does to the target
damageType - String - the type of damage this effect does - values should be from the damage_types table
damageProperty - String - either "health" or "energy"
damageMod - Float - Modifies the damage done by this effect. The damage of the effect is multiplied by this number, so a value of 2 means double damage.
bonusDamageEffect - Integer - the ID of an effect, which if present, causes this effect to do more damage
bonusDamageAmount - Integer - the amount of bonus damage applied if the bonus effect is present
healthTransferRate - Float - the amount of health gained based on the damage done to the target. A value of 1 means the attacker gains as much health as was done to the target. Will usually be 0 for no health transferred.
*/

public class EffectsDamageData: DataStructure
{
	public int id = 0;					// Database Index

	// General Parameters
	public int damageAmount; 				// the amount of damage this effect does to the target
	public string damageType = ""; 		//  the type of damage this effect does - values should be from the damage_types table
	public string damageProperty = ""; 		// either "health" or "energy"
	public string[] damagePropertyOptions = new string[] {"health", "energy"};
	public float damageMod = 0;			// Modifies the damage done by this effect. The damage of the effect is multiplied by this number, so a value of 2 means double damage.
	public int bonusDamageEffect = 0;					// the ID of an effect, which if present, causes this effect to do more damage
	public int bonusDamageAmount = 0;				// the amount of bonus damage applied if the bonus effect is present
	public float healthTransferRate = 0;			// the amount of health gained based on the damage done to the target.
	
	public EffectsDamageData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
			{"id", "int"},
			{"damageAmount", "int"},
			{"damageType", "string"},
			{"damageProperty", "string"},
			{"damageMod", "float"},
			{"bonusDamageEffect", "int"},
			{"bonusDamageAmount", "int"},
			{"healthTransferRate", "float"}
 
	};
	}

	public void LoadEffectData(EffectsData effectData) {
		id = effectData.id;
		damageAmount = effectData.damageAmount;
		damageType = effectData.damageType;
		damageProperty = effectData.damageProperty;
		damageMod = effectData.damageMod;
		bonusDamageEffect = effectData.bonusDamageEffect;
		bonusDamageAmount = effectData.bonusDamageAmount;
		healthTransferRate = effectData.healthTransferRate;
	}
		
	public EffectsDamageData Clone()
	{
		return (EffectsDamageData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "damageAmount":
			return damageAmount.ToString();
			break;
		case "damageType":
			return damageType;
			break;
		case "damageProperty":
			return damageProperty;
			break;
		case "damageMod":
			return damageMod.ToString();
			break;
		case "bonusDamageEffect":
			return bonusDamageEffect.ToString();
			break;
		case "bonusDamageAmount":
			return bonusDamageAmount.ToString();
			break;
		case "healthTransferRate":
			return healthTransferRate.ToString();
			break;

		}	
		return "";
	}
		
}