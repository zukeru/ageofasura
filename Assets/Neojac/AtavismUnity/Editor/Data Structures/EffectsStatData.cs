using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

public class EffectsStatData: DataStructure
{
	public int id = 0;					// Database Index

	// General Parameters
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

	public EffectsStatData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
			{"id", "int"},
			{"modifyStatsByPercent", "bool"},
			{"stat1Name", "string"},
			{"stat1Modification", "float"},
			{"stat2Name", "string"},
			{"stat2Modification", "float"},
			{"stat3Name", "string"},
			{"stat3Modification", "float"},
			{"stat4Name", "string"},
			{"stat4Modification", "float"},
			{"stat5Name", "string"},
			{"stat5Modification", "float"}
	};
	}

	public void LoadEffectData(EffectsData effectData) {
		id = effectData.id;
		modifyStatsByPercent = effectData.modifyStatsByPercent;
		stat1Name = effectData.stat1Name;
		stat1Modification = effectData.stat1Modification;
		stat2Name = effectData.stat2Name;
		stat2Modification = effectData.stat2Modification;
		stat3Name = effectData.stat3Name;
		stat3Modification = effectData.stat3Modification;
		stat4Name = effectData.stat4Name;
		stat4Modification = effectData.stat4Modification;
		stat5Name = effectData.stat5Name;
		stat5Modification = effectData.stat5Modification;
	}

	public EffectsStatData Clone()
	{
		return (EffectsStatData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "modifyStatsByPercent":
			return modifyStatsByPercent.ToString();
			break;
		case "stat1Name":
			return stat1Name;
			break;
		case "stat1Modification":
			return stat1Modification.ToString();
			break;
		case "stat2Name":
			return stat2Name;
			break;
		case "stat2Modification":
			return stat2Modification.ToString();
			break;
		case "stat3Name":
			return stat3Name;
			break;
		case "stat3Modification":
			return stat3Modification.ToString();
			break;
		case "stat4Name":
			return stat4Name;
			break;
		case "stat4Modification":
			return stat4Modification.ToString();
			break;
		case "stat5Name":
			return stat5Name;
			break;
		case "stat5Modification":
			return stat5Modification.ToString();
			break;
		}	
		return "";
	}
		
}