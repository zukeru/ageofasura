using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

public class InstancePortalEntry : DataStructure
{
	public InstancePortalEntry() : this("", Vector3.zero, Quaternion.identity, "") {
	}
	
	public InstancePortalEntry(string name, Vector3 loc) : this(name, loc, Quaternion.identity, "") {
	}

	public InstancePortalEntry(string name, Vector3 loc, Quaternion orient, string gameObject) {
		this.name = name;
		this.loc = loc;
		this.orient = orient;
		this.gameObject = gameObject;
		
		fields = new Dictionary<string, string> () {
			{"id", "int"},
			{"island", "int"},
			{"name", "string"},
			{"portalType", "int"},
			{"faction", "int"},
			{"displayID", "int"},
			{"locX", "float"},
			{"locY", "float"},
			{"locZ", "float"},
			{"orientX", "float"},
			{"orientY", "float"},
			{"orientZ", "float"},
			{"orientW", "float"},
		};
	}
	
	public int instanceID;
	public string name = "";
	public int portalType = 0;
	public int faction = 0;
	public int displayID = 0;
	public Vector3 loc;
	public Quaternion orient;
	public string gameObject;
	
	public InstancePortalEntry Clone()
	{
		return (InstancePortalEntry) this.MemberwiseClone();
	}
	
	public string MarkerObject {
		get {
			return gameObject;
		}
		set {
			gameObject = value;
			if (gameObject != null) {
				// Try to get as Scene Object
				GameObject tempObject = GameObject.Find(gameObject);
				// If the object is at the Scene
				if (tempObject != null) {
					// Get object transform
					loc = tempObject.transform.position;
					orient = tempObject.transform.rotation;
				}
			}
		}
	}
	
	public override string GetValue (string fieldKey)
	{
		if (fieldKey == "id") {
			return id.ToString();
		} else if (fieldKey == "island") {
			return instanceID.ToString();
		} else if (fieldKey == "name") {
			return name;
		} else if (fieldKey == "portalType") {
			return portalType.ToString();
		} else if (fieldKey == "faction") {
			return faction.ToString();
		} else if (fieldKey == "displayID") {
			return displayID.ToString();
		} else if (fieldKey == "locX") {
			return loc.x.ToString();
		} else if (fieldKey == "locY") {
			return loc.y.ToString();
		} else if (fieldKey == "locZ") {
			return loc.z.ToString();
		} else if (fieldKey == "orientX") {
			return orient.x.ToString();
		} else if (fieldKey == "orientY") {
			return orient.y.ToString();
		} else if (fieldKey == "orientZ") {
			return orient.z.ToString();
		} else if (fieldKey == "orientW") {
			return orient.w.ToString();
		}
		return "";
	}
}

// Structure of a Atavism Instance
public class Instance: DataStructure
{
	public int id = 0;				// Database index
	// General Parameters
	public string name = "name";					// Instance name
	public bool createOnStartup = false;			// True, if wants to create at startup
	public int islandType;							// Type of island
	public int administrator;						// The account that has administration privileges of the island
	public int populationLimit = -1;	
	
	public List<InstancePortalEntry> instancePortals = new List<InstancePortalEntry>();
	
	public List<int> itemsToBeDeleted = new List<int>();
		
	public Instance() {
	// Database fields
	fields = new Dictionary<string, string> () {
		{"island_name", "string"},
		{"createOnStartup", "bool"},
		{"administrator", "int"},
		{"populationLimit", "int"},
	};
	}
	
	public Instance Clone()
	{
		return (Instance) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "island_name":
			return name;
			break;
		case "administrator":
			return administrator.ToString();
			break;
		case "populationLimit":
			return populationLimit.ToString();
			break;
		}	
		return "";
	}
}
