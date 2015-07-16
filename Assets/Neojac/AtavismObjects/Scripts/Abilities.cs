using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Abilities : MonoBehaviour {

	static Abilities instance;
	
	List<Ability> playerAbilities;
	Dictionary<int, Ability> abilities;

	void Start() {
		if (instance != null) {
			return;
		}
		instance = this;
		
		playerAbilities = new List<Ability>();
		abilities = new Dictionary<int, Ability>();
		Object[] abilityPrefabs = Resources.LoadAll("Content/Abilities");
		foreach (Object abilityPrefab in abilityPrefabs) {
			GameObject go = (GameObject) abilityPrefab;
			Ability abilityData = go.GetComponent<Ability>();
			if (!abilities.ContainsKey(abilityData.id)) {
				abilities.Add(abilityData.id, abilityData);
			}
		}
	}
	
	void ClientReady() {
		// Register for abilities property
		ClientAPI.WorldManager.RegisterObjectPropertyChangeHandler("abilities", AbilitiesPropertyHandler);
	}
	
	public Ability GetAbility(int abilityID) {
		// First check if the player has a copy of this ability
		Ability ability = GetPlayerAbility(abilityID);
		if (ability == null) {
			// Player does not have this ability - lets use the template
			if (abilities.ContainsKey(abilityID))
				return abilities[abilityID];
		}
		return ability;
	}
	
	public Ability GetPlayerAbility(int abilityID) {
		Ability ability = null;
		foreach (Ability ab in playerAbilities) {
			if (ab.id == abilityID) {
				return ab;
			}
		}
		return ability;
	}
	
	public void AbilitiesPropertyHandler(object sender, ObjectPropertyChangeEventArgs args) {
    	if (args.Oid != ClientAPI.GetPlayerOid())
        	return;
		LinkedList<object> abilities_prop = (LinkedList<object>)ClientAPI.GetPlayerObject().GetProperty("abilities");
		AtavismLogger.LogDebugMessage("Got player abilities property change: " + abilities_prop);
		playerAbilities.Clear();
		int pos = 0;
    	foreach (int abilityNum in abilities_prop) 
    	{
			if (!abilities.ContainsKey(abilityNum)) {
				UnityEngine.Debug.LogWarning("Ability " + abilityNum + " does not exist");
				continue;
			}
			Ability ability = abilities[abilityNum].Clone(gameObject);
			playerAbilities.Add(ability);
		}
   		// dispatch a ui event to tell the rest of the system
		string[] event_args = new string[1];
		AtavismEventSystem.DispatchEvent("ABILITY_UPDATE", event_args);
	}
	
	public List<Ability> PlayerAbilities 
	{
		get {
			return playerAbilities;
		}
	}
}
