using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;


public class CurrencyData: DataStructure
{
	public int id = 0;					// Database Index
	// General Parameters
	public string name = "name";		// The item template name
	public int category = 1;			//leave this as 1
	public string icon = "";			// The item icon
	public string description = "";		// A description of the currency (optional)
	public int maximum = 999999;		// The maximum amount of the currency a player can have
	public bool external = false;		// Can be modified outside the game (such as being purchased from an online store0
	public bool isSubCurrency = false;

	public CurrencyData subCurrency1;
	public int subCurrency1ID = -1;
	public CurrencyData subCurrency2;
	public int subCurrency2ID = -1;
		
	public CurrencyData ()
	{
		// Database fields
	fields = new Dictionary<string, string> () {
		{"name", "string"},
		{"category", "int"},
		{"icon", "string"},
		{"description", "string"},
		{"maximum", "int"},
		{"external", "bool"},
		{"isSubCurrency", "bool"}, 
		{"subCurrency1", "int"}, 
		{"subCurrency2", "int"}, 
	};
	}
	
	public CurrencyData Clone()
	{
		return (CurrencyData) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "name":
			return name;
			break;
		case "category":
			return category.ToString();
			break;
		case "icon":
			return icon;
			break;
		case "description":
			return description;
			break;
		case "maximum":
			return maximum.ToString();
			break;	
		case "external":
			return external.ToString();
			break;
		case "isSubCurrency":
			return isSubCurrency.ToString();
			break;
		case "subCurrency1":
			return subCurrency1ID.ToString();
			break;
		case "subCurrency2":
			return subCurrency2ID.ToString();
			break;
		}	
		return "";
	}
	
}