using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

/* Structure
id - Integer - links to id of parent table
healAmount- Integer - the amount of healing this effect does to the target
healProperty - String - either health or energy
healthTransferRate - Float - the amount of health removed from the caster based on the healing done to the target. 
A value of 1 means the caster loses as much health as was given to the target. 
Will usually be 0 for no health transferred unless its a HealthTransferEffect.
*/

public class EffectsHealData: DataStructure
{
	public int id = 0;					// Database Index

	// General Parameters
	public int healAmount; 				// the amount of healing this effect does to the target
	public string healProperty = ""; 		// either health or energy
	public string[] healPropertyOptions = new string[] {"health", "energy"};
	public float healthTransferRate = 0;		// the amount of health removed from the caster based on the healing done to the target. 

	public EffectsHealData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
			{"id", "int"},
			{"healAmount", "int"},
			{"healProperty", "string"},
			{"healthTransferRate", "float"}
	};
	}

	public void LoadEffectData(EffectsData effectData) {
		id = effectData.id;
		healAmount = effectData.healAmount;
		healProperty = effectData.healProperty;
		healthTransferRate = effectData.healthTransferRate;
	}

	public EffectsHealData Clone()
	{
		return (EffectsHealData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "healAmount":
			return healAmount.ToString();
			break;
		case "healProperty":
			return healProperty;
			break;
		case "healthTransferRate":
			return healthTransferRate.ToString();
			break;
		}	
		return "";
	}
		
}