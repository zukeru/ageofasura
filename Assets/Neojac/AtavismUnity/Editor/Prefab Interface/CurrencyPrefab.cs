using UnityEngine;
using UnityEditor;
using System.Collections;

public class CurrencyPrefab {

	// Prefab Parameters
	public int id = -1;
	public string name = "";
	public Texture2D icon = null;
	public int subID1 = -1;
	public string subName1 = "";
	public Texture2D subIcon1 = null;
	public int subID2 = -1;
	public string subName2 = "";
	public Texture2D subIcon2 = null;

	// Prefab file information
	private string prefabName;
	private string prefabPath;
	// Common Prefab Prefix and Sufix
	private string itemPrefix = "Currency";
	private string itemSufix = ".prefab";
	// Base path
	private string basePath = "";
	// Example Item Prefab Information
	private string basePrefab = "Example Currency Prefab.prefab";
	private string basePrefabPath;

	public CurrencyPrefab(int id, string itemName) {
		this.id = id;
		name = itemName;
		
		basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
		prefabName = itemPrefix+itemName+itemSufix;
		prefabPath = basePath+prefabName;
		basePrefabPath = basePath+basePrefab;
	}

	public void Save(string iconNew, int subID1, string subName1, string subIcon1, int subID2, string subName2, string subIcon2)
	{
		icon = (Texture2D) AssetDatabase.LoadAssetAtPath(iconNew, typeof(Texture2D));
		this.subID1 = subID1;
		this.subName1 = subName1;
		this.subIcon1 = (Texture2D) AssetDatabase.LoadAssetAtPath(subIcon1, typeof(Texture2D));
		this.subID2 = subID2;
		this.subName2 = subName2;
		this.subIcon2 = (Texture2D) AssetDatabase.LoadAssetAtPath(subIcon2, typeof(Texture2D));
		
		this.Save ();
	}

	public void Save(Texture2D iconNew, int subID1, string subName1, Texture2D subIcon1, int subID2, string subName2, Texture2D subIcon2)
	{
		if (icon != null)
			icon = iconNew;
		this.subID1 = subID1;
		this.subName1 = subName1;
		this.subIcon1 = subIcon1;
		this.subID2 = subID2;
		this.subName2 = subName2;
		this.subIcon2 = subIcon2;
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

		item.GetComponent<Currency>().id = id;
		item.GetComponent<Currency>().name = name;
		if (icon != null)
			item.GetComponent<Currency>().icon = icon;
		/*item.GetComponent<Currency>().subCurrency1ID = subID1;
		item.GetComponent<Currency>().subCurrency1Name = subName1;
		item.GetComponent<Currency>().subCurrency1Icon = subIcon1;
		item.GetComponent<Currency>().subCurrency2ID = subID2;
		item.GetComponent<Currency>().subCurrency2Name = subName2;
		item.GetComponent<Currency>().subCurrency2Icon = subIcon2;*/
		
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

		id = item.GetComponent<Currency>().id;
		name = item.GetComponent<Currency>().name;
		icon = item.GetComponent<Currency>().icon;
		/*subID1 = item.GetComponent<Currency>().subCurrency1ID;
		subName1 = item.GetComponent<Currency>().subCurrency1Name;
		subIcon1 = item.GetComponent<Currency>().subCurrency1Icon;
		subID2 = item.GetComponent<Currency>().subCurrency2ID;
		subName2 = item.GetComponent<Currency>().subCurrency2Name;
		subIcon2 = item.GetComponent<Currency>().subCurrency2Icon;*/
		return true;
	}

}
