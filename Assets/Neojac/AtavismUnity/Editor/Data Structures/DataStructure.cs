using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Base Data Structure Class for Atavism 
public class DataStructure
{
	public bool isLoaded = false;	// True, if is loaded with database data

	// Database fields
	public Dictionary<string, string> fields;
	
	public int id = 0;				// Database index
	public string name = "";

	public DataStructure ()
	{
	}	
	
	// Build the Insert string for query
	public string FieldList (string startSpace, string endSpace)
	{
		string result = "";
		foreach (string field in fields.Keys) {
			result += startSpace;
			result += field;
			result += endSpace;
		}
		return result.Remove (result.Length - endSpace.Length);				
	}
	
	// Build the Update string for query
	public string UpdateList ()
	{	
		string result = "";
		foreach (string field in fields.Keys) {
			result += field + "=?" + field + ", ";
		}
		return result.Remove (result.Length - 2);
	}
	
	// Build the Update string for query
	public Register fieldToRegister (string fieldKey)
	{
		MySqlDbType sqlType = MySqlDbType.VarChar;
		Register.TypesOfField type = Register.TypesOfField.String;
		
		switch (fields [fieldKey]) {
		case "int":
			sqlType = MySqlDbType.Int32;
			type = Register.TypesOfField.Int;
			break;
		case "string":
			sqlType = MySqlDbType.VarChar;
			type = Register.TypesOfField.String;
			break;
		case "float":
			sqlType = MySqlDbType.Float;
			type = Register.TypesOfField.Float;
			break;
		case "bool":
			sqlType = MySqlDbType.Byte;
			type = Register.TypesOfField.Bool;
			break;
		default:
			sqlType = MySqlDbType.VarChar;
			type = Register.TypesOfField.String;
			break;			
		}
		
		//Debug.Log ("[" + fieldKey + "[" + sqlType + "[" + GetValue(fieldKey) + "[" + type + "]");
		return new Register (fieldKey, "?" + fieldKey, sqlType, GetValue(fieldKey), type);						
	}
	
	public virtual string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "":
			return "";
			break;
		}	
		return "";
	}
			
}