using UnityEngine;
using UnityEditor;
using System.Collections;

public class AbilityPrefab {

	// Prefab Parameters
	public AbilitiesData abilityData;

	// Prefab file information
	private string prefabName;
	private string prefabPath;
	// Common Prefab Prefix and Sufix
	private string itemPrefix = "Ability";
	private string itemSufix = ".prefab";
	// Base path
	private string basePath = "";
	// Example Item Prefab Information
	private string basePrefab = "Example Ability Prefab.prefab";
	private string basePrefabPath;

	public AbilityPrefab(AbilitiesData abilityData) {
		this.abilityData = abilityData;
		
		basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
		prefabName = itemPrefix + abilityData.name + itemSufix;
		prefabPath = basePath + prefabName;
		basePrefabPath = basePath + basePrefab;
	}

	public void Save(AbilitiesData abilityData)
	{
		this.abilityData = abilityData;
		
		this.Save ();
	}

	// Save data from the class to the new prefab, creating one if it doesnt exist
	public void Save() {
		GameObject item = (GameObject) AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

		// If this is a new prefab
		if (item == null) {
			AssetDatabase.CopyAsset(basePrefabPath, prefabPath);
			AssetDatabase.Refresh();
			item = (GameObject) AssetDatabase.LoadAssetAtPath(prefabPath,  typeof(GameObject));
		}

		item.GetComponent<Ability>().id = abilityData.id;
		item.GetComponent<Ability>().name = abilityData.name;
		Texture2D icon = (Texture2D) AssetDatabase.LoadAssetAtPath(abilityData.icon, typeof(Texture2D));
		if (icon != null)
			item.GetComponent<Ability>().icon = icon;
		item.GetComponent<Ability>().tooltip = abilityData.tooltip;
		item.GetComponent<Ability>().cost = abilityData.activationCost;
		item.GetComponent<Ability>().costProperty = abilityData.activationCostType;
		
		EditorUtility.SetDirty(item);
		AssetDatabase.Refresh();
	}

	public void Delete() {
		GameObject item = (GameObject) AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
		
		// If this is a new prefab
		if (item != null) {
			AssetDatabase.DeleteAsset(prefabPath);
			AssetDatabase.Refresh();
		}
	}

	// Load data from the prefab base on its name
	// return true if the prefab exist and false if there is no prefab
	public bool Load() {

		GameObject ability = (GameObject) AssetDatabase.LoadAssetAtPath(prefabPath,  typeof(GameObject));
		
		// If this is a new prefab
		if (ability == null) 
			return false;

		abilityData = new AbilitiesData();
		abilityData.id = ability.GetComponent<Ability>().id;
		abilityData.name = ability.GetComponent<Ability>().name;
		//abilityData.icon = ability.GetComponent<Ability>().icon;
		abilityData.tooltip = ability.GetComponent<Ability>().tooltip;
		abilityData.activationCost = ability.GetComponent<Ability>().cost;
		abilityData.activationCostType = ability.GetComponent<Ability>().costProperty;

		return true;
	}

}
