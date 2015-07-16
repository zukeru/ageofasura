using UnityEngine;
using System.Collections;

public class ServerPlugin : AtavismPlugin {

	// Use this for initialization
	public ServerPlugin()
	{	
		pluginName = "Server";
		string serverCategory = "AT_button_category_server";
		icon = (Texture)Resources.Load (serverCategory, typeof(Texture));
		iconOver = (Texture)Resources.Load (serverCategory + "_over", typeof(Texture));
		iconSelected = (Texture)Resources.Load (serverCategory + "_selected", typeof(Texture));

		RegisterFunction("DataBase", "Server");
		RegisterFunction("OptionChoices", "Server");
		RegisterFunction("Instances", "Server");
		RegisterFunction("Accounts", "Server");
		RegisterFunction("Mobs", "Mob");
    	RegisterFunction("LootTables", "Mob");
		RegisterFunction("MerchantTables", "Mob");
		RegisterFunction("Factions", "Mob");
		RegisterFunction("Quests", "Mob");
		RegisterFunction("Dialogues", "Mob");
		RegisterFunction("Items", "Item");
		RegisterFunction ("Currency", "Item");
		RegisterFunction("CraftingRecipes", "Item");
		RegisterFunction("Skills", "Combat");
		RegisterFunction("Abilities", "Combat");
		RegisterFunction("Effects", "Combat");
		RegisterFunction("CoordEffects", "Combat");
		RegisterFunction("Stats", "Combat");
		RegisterFunction("Damage", "Combat");
		RegisterFunction("Character", "Character");
		RegisterFunction("LevelXp", "Character");
		RegisterFunction("ResourceNodes", "World");
		RegisterFunction("NavMesh", "World");
		//RegisterFunction("Objectives", "Combat");
		//RegisterFunction("Rewards", "Combat");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
