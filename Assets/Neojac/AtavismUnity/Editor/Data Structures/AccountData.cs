using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of an Atavism Game Account
public class AccountData : DataStructure
{
	public int id = 0;				// Database index
	// General Parameters
	public string name = "name";				// Account username
	public int status = 1;								// Type of island
	public static string[] statusOptions = new string[] {"Normal", "Banned", "GM", "Admin"};
	public static int[] statusValues = new int[] {1, 0, 3, 5};
		
	public AccountData() {
	// Database fields
	fields = new Dictionary<string, string> () {
		{"username", "string"},
		{"status", "int"},
	};
	}
	
	public AccountData Clone()
	{
		return (AccountData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "username":
			return name;
			break;
		case "status":
			return status.ToString();
			break;
		}
		return "";
	}
}
