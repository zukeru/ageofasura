using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ItemPrefab {

	// Prefab Parameters
	public ItemData itemData;

	// Prefab file information
	private string prefabName;
	private string prefabPath;
	// Common Prefab Prefix and Sufix
	private string itemPrefix = "Item";
	private string itemSufix = ".prefab";
	// Base path
	private string basePath = "";
	// Example Item Prefab Information
	private string basePrefab = "Example Item Prefab.prefab";
	private string basePrefabPath;

	public ItemPrefab(ItemData itemData) {
		this.itemData = itemData;
		
		basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
		prefabName = itemPrefix + itemData.name + itemSufix;
		prefabPath = basePath + prefabName;
		basePrefabPath = basePath + basePrefab;
	}

	public void Save(ItemData itemData)
	{
		this.itemData = itemData;
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

		item.GetComponent<AtavismInventoryItem>().templateId = itemData.id;
		item.GetComponent<AtavismInventoryItem>().name = itemData.name;
		Texture2D icon = (Texture2D) AssetDatabase.LoadAssetAtPath(itemData.icon, typeof(Texture2D));
		if (icon != null)
			item.GetComponent<AtavismInventoryItem>().icon = icon;
		item.GetComponent<AtavismInventoryItem>().tooltip = itemData.toolTip;
		item.GetComponent<AtavismInventoryItem>().itemType = itemData.itemType;
		item.GetComponent<AtavismInventoryItem>().subType = itemData.subType;
		item.GetComponent<AtavismInventoryItem>().slot = itemData.slot;
		item.GetComponent<AtavismInventoryItem>().quality = itemData.itemQuality;
		item.GetComponent<AtavismInventoryItem>().currencyType = itemData.purchaseCurrency;
		item.GetComponent<AtavismInventoryItem>().cost = itemData.purchaseCost;
		item.GetComponent<AtavismInventoryItem>().sellable = itemData.sellable;
		item.GetComponent<AtavismInventoryItem>().ClearEffects();
		foreach (ItemEffectEntry effect in itemData.effects) {
			item.GetComponent<AtavismInventoryItem>().itemEffectTypes.Add(effect.itemEffectType);
			item.GetComponent<AtavismInventoryItem>().itemEffectNames.Add(effect.itemEffectName);
			item.GetComponent<AtavismInventoryItem>().itemEffectValues.Add(effect.itemEffectValue);
		}
		
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

		itemData = new ItemData();
		itemData.id = item.GetComponent<AtavismInventoryItem>().templateId;
		itemData.name = item.GetComponent<AtavismInventoryItem>().name;
		//itemData.icon = item.GetComponent<AtavismInventoryItem>().icon;
		itemData.toolTip = item.GetComponent<AtavismInventoryItem>().tooltip;
		itemData.itemType = item.GetComponent<AtavismInventoryItem>().itemType;
		itemData.subType = item.GetComponent<AtavismInventoryItem>().subType;
		itemData.slot = item.GetComponent<AtavismInventoryItem>().slot;
		itemData.itemQuality = item.GetComponent<AtavismInventoryItem>().quality;
		itemData.purchaseCurrency = item.GetComponent<AtavismInventoryItem>().currencyType;
		itemData.purchaseCost = item.GetComponent<AtavismInventoryItem>().cost;
		itemData.sellable = item.GetComponent<AtavismInventoryItem>().sellable;

		return true;
	}

}
