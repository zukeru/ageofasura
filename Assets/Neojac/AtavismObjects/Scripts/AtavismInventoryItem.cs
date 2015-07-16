using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtavismInventoryItem : Activatable
{

	string baseName = "";
	OID itemId = null;
	public int templateId = -1;
	string category = "";
	string subcategory = "";
	int count = 0;
	public string itemType = "";
	public string subType = "";
	public string slot = "";
	public int quality = 0;
	int binding = 0;
	bool unique = false;
	int stackLimit = 1;
	public int currencyType = 0;
	public int cost = 0;
	public bool sellable = true;
	int displayID = 0;
	int energyCost = 0;
	int encumberance = 0;
	Dictionary<string, int> resistances = new Dictionary<string, int>();
	Dictionary<string, int> stats = new Dictionary<string, int>();
	int damageValue = 0;
	string damageType = "";
	int weaponSpeed = 2000;
	bool randomisedStats = false;
	int globalcd = 0;
	int weaponcd = 0;
	string cdtype2 = "";
	string raceReq = "";
	string aspectReq = "";
	int levelReq = 0;
	int useAbility = -1;
	string clickEffect = "null";
	public List<string> itemEffectTypes = new List<string>();
	public List<string> itemEffectNames = new List<string>();
	public List<string> itemEffectValues = new List<string>();
	
	// Dynamic settings for crafting and other systems that allow temporary placement of items
	int usedCount = 0;
	
	public AtavismInventoryItem Clone(GameObject go) {
		AtavismInventoryItem clone = go.AddComponent<AtavismInventoryItem>();
		clone.templateId = templateId;
		clone.name = name;
		clone.icon = icon;
		clone.baseName = baseName;
		clone.category = category;
		clone.subcategory = subcategory;
		clone.count = count;
		clone.itemType = itemType;
		clone.subType = subType;
		clone.slot = slot; 
		clone.quality = quality;
		clone.binding = binding;
		clone.unique = unique;
		clone.stackLimit = stackLimit;
		clone.currencyType = currencyType;
		clone.cost = cost;
		clone.displayID = displayID;
		clone.energyCost = energyCost;
		clone.encumberance = encumberance;
		clone.resistances = resistances;
		clone.stats = stats;
		clone.damageValue = damageValue;
		clone.weaponSpeed = weaponSpeed;
		clone.randomisedStats = randomisedStats;
		clone.globalcd = globalcd;
		clone.weaponcd = weaponcd;
		clone.cdtype2 = cdtype2;
		clone.raceReq = raceReq;
		clone.aspectReq = aspectReq;
		clone.levelReq = levelReq;
		clone.clickEffect = clickEffect;
		clone.tooltip = tooltip;
		clone.itemEffectTypes = itemEffectTypes;
		clone.itemEffectNames = itemEffectNames;
		clone.itemEffectValues = itemEffectValues;
		return clone;
	}
	
	public override bool Activate() {
		//TODO: provide proper target setup
		NetworkAPI.SendActivateItemMessage(itemId, ClientAPI.GetPlayerOid());
		return true;
	}
	
	public override void DrawTooltip(float x, float y) {
		List<int> statPositions = GetEffectPositionsOfTypes("Stat");
		int width = 150;
		int height = 50 + statPositions.Count * 20;
		Rect tooltipRect = new Rect(x, y - height, width, height);
		GUI.Box(tooltipRect, "");
		GUI.Label(new Rect(tooltipRect.x + 5, tooltipRect.y + 5, 140, 20), name);
		GUI.Label(new Rect(tooltipRect.x + 5, tooltipRect.y + 25, 140, 20), itemType);
		for (int i = 0; i < statPositions.Count; i++) {
			string prefix = "";
			if (!itemEffectValues[statPositions[i]].Contains("-"))
				prefix = "+";
			GUI.Label(new Rect(tooltipRect.x + 5, tooltipRect.y + 25 + ((i+1) * 20), 140, 20), 
				      prefix + itemEffectValues[statPositions[i]] + " " + itemEffectNames[statPositions[i]]);
		}
	}
	
	public void AlterUseCount(int delta) {
		usedCount += delta;
	}
	
	public void ResetUseCount() {
		usedCount = 0;
	}
	
	public void AddItemEffect(string itemEffectType, string itemEffectName, string itemEffectValue) {
		itemEffectTypes.Add(itemEffectType);
		itemEffectNames.Add(itemEffectName);
		itemEffectValues.Add(itemEffectValue);
	}
	
	public void ClearEffects() {
		itemEffectTypes.Clear();
		itemEffectNames.Clear();
		itemEffectValues.Clear();
	}
	
	public List<int> GetEffectPositionsOfTypes(string effectType) {
		List<int> effectPositions = new List<int>();
		for (int i = 0; i < itemEffectTypes.Count; i++) {
			if (itemEffectTypes[i] == effectType)
				effectPositions.Add(i);
		}
		return effectPositions;
	}
	
	#region Properties
	public string BaseName {
		get {
			return name;
		}
		set {
			name = value;
		}
	}
	
	public OID ItemId {
		get {
			return itemId;
		}
		set {
			itemId = value;
		}
	}
	
	public int TemplateId {
		get {
			return templateId;
		}
		set {
			templateId = value;
		}
	}
	
	public string Category {
		get {
			return category;
		}
		set {
			category = value;
		}
	}
	
	public string Subcategory {
		get {
			return subcategory;
		}
		set {
			subcategory = value;
		}
	}
	
	public int Count {
		get {
			return count - usedCount;
		}
		set {
			count = value;
		}
	}
	
	public int Quality {
		get {
			return quality;
		}
		set {
			quality = value;
		}
	}
	
	public int Binding {
		get {
			return binding;
		}
		set {
			binding = value;
		}
	}
	
	public bool Unique {
		get {
			return unique;
		}
		set {
			unique = value;
		}
	}
	
	public int StackLimit {
		get {
			return stackLimit;
		}
		set {
			stackLimit = value;
		}
	}
	
	public int CurrencyType {
		get {
			return currencyType;
		}
		set {
			currencyType = value;
		}
	}
	
	public int Cost {
		get {
			return cost;
		}
		set {
			cost = value;
		}
	}
	
	public bool Sellable {
		get {
			return sellable;
		}
		set {
			sellable = value;
		}
	}
	
	public int EnergyCost {
		get {
			return energyCost;
		}
		set {
			energyCost = value;
		}
	}
	
	public int Encumberance {
		get {
			return encumberance;
		}
		set {
			encumberance = value;
		}
	}
	
	public Dictionary<string, int> Resistances {
		get {
			return resistances;
		}
		set {
			resistances = value;
		}
	}
	
	public Dictionary<string, int> Stats {
		get {
			return stats;
		}
		set {
			stats = value;
		}
	}
	
	public int DamageValue {
		get {
			return damageValue;
		}
		set {
			damageValue = value;
		}
	}
	
	public string DamageType {
		get {
			return damageType;
		}
		set {
			damageType = value;
		}
	}
	
	public int WeaponSpeed {
		get {
			return weaponSpeed;
		}
		set {
			weaponSpeed = value;
		}
	}
	#endregion Properties
}
