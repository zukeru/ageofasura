using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CraftingStationType {
	Anvil,
	Smelter,
	Pot,
	Cauldron,
	Oven,
	Loom,
	Sewing,
	Tannery,
	Masonry,
	Alchemy,
	Desk,
	Sawmill,
	None
}

public class CraftingStation : MonoBehaviour {
	
	public CraftingStationType stationType;
	//public GameObject target;
	public Texture2D icon;
	public GameObject coordEffect;

	// Use this for initialization
	void Start () {
		gameObject.AddComponent<AtavismNode>();
		GetComponent<AtavismNode>().AddLocalProperty("craftingStation", stationType);
		GetComponent<AtavismNode>().AddLocalProperty("targetable", false);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnMouseDown() {
		// this object was clicked - do something
		Send ();
	} 
	
	void Send ()
	{
		/*if (target == null) 
			target = gameObject;
		target.SetActive(!target.activeSelf);*/
		ClientAPI.ScriptObject.GetComponent<Crafting>().StationType = stationType;
		ClientAPI.ScriptObject.GetComponent<Crafting>().Station = gameObject;
		
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("CRAFTING_START", args);

		// Set name and icon of UI
		//target.transform.GetChild(0).FindChild("Name").GetComponent<UILabel>().text = stationType.ToString();
		//target.transform.GetChild(0).FindChild("BagIcon").GetComponent<UISprite>().spriteName = icon.name;
	}

	public void CloseStation() {
		//target.SetActive(false);
	}
}
