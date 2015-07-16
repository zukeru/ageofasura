using UnityEngine;
using UnityEditor;
using System.Collections;

public class SkillPrefab {

	// Prefab Parameters
	public int id = -1;
	public string name = "";
	public Texture2D icon = null;
	public int parentSkill = -1;
	public int parentSkillLevelReq = 1;
	public int playerLevelReq = 1;

	// Prefab file information
	private string prefabName;
	private string prefabPath;
	// Common Prefab Prefix and Sufix
	private string itemPrefix = "Skill";
	private string itemSufix = ".prefab";
	// Base path
	private string basePath = "";
	// Example Item Prefab Information
	private string basePrefab = "Example Skill Prefab.prefab";
	private string basePrefabPath;

	public SkillPrefab(int id, string itemName) {
		this.id = id;
		name = itemName;
		
		basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
		prefabName = itemPrefix+itemName+itemSufix;
		prefabPath = basePath+prefabName;
		basePrefabPath = basePath+basePrefab;
	}

	public void Save(string iconNew, int parentSkill, int parentSkillLevelReq, int playerLevelReq)
	{
		icon = (Texture2D) AssetDatabase.LoadAssetAtPath(iconNew, typeof(Texture2D));
		this.parentSkill = parentSkill;
		this.parentSkillLevelReq = parentSkillLevelReq;
		this.playerLevelReq = playerLevelReq;
		
		this.Save ();
	}

	public void Save(Texture2D iconNew, int parentSkill, int parentSkillLevelReq, int playerLevelReq)
	{
		if (icon != null)
			icon = iconNew;
		this.parentSkill = parentSkill;
		this.parentSkillLevelReq = parentSkillLevelReq;
		this.playerLevelReq = playerLevelReq;

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

		item.GetComponent<Skill>().id = id;
		item.GetComponent<Skill>().skillname = name;
		if (icon != null)
			item.GetComponent<Skill>().icon = icon;
		item.GetComponent<Skill>().parentSkill = parentSkill;
		item.GetComponent<Skill>().parentSkillLevelReq = parentSkillLevelReq;
		item.GetComponent<Skill>().playerLevelReq = playerLevelReq;
		
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

		GameObject item = (GameObject) AssetDatabase.LoadAssetAtPath(prefabPath,  typeof(GameObject));
		
		// If this is a new prefab
		if (item == null) 
			return false;

		id = item.GetComponent<Skill>().id;
		name = item.GetComponent<Skill>().skillname;
		icon = item.GetComponent<Skill>().icon;
		parentSkill = item.GetComponent<Skill>().parentSkill;
		parentSkillLevelReq = item.GetComponent<Skill>().parentSkillLevelReq;
		playerLevelReq = item.GetComponent<Skill>().playerLevelReq;

		return true;
	}

}
