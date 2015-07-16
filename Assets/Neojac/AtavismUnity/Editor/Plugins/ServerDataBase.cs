using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Server connection and setup function
public class ServerDataBase : AtavismFunction
{
	
	private string databaseHost;			// Database Host Address
	private string databasePort;			// Database Host port
	private string databaseUser;			// Database User Name
	private string databasePassword;		// Database Password
	private string databaseName;			// Database Name

	private string testType = "none";		// Connection test type (Content or Admin)
	
	// Tab selection
	public int selected = 1;
			
	// Use this for initialization
	public ServerDataBase ()
	{	
		functionName = "Data Base";		// Set the function name
		string prefix = DatabasePack.contentDatabasePrefix;
		databaseHost = EditorPrefs.GetString("databaseHost" + prefix + DatabasePack.GetProjectName());
		if (databaseHost == null || databaseHost == "") {
			EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), "localhost");
		}
		databaseName = EditorPrefs.GetString("databaseName" + prefix + DatabasePack.GetProjectName());
		if (databaseName == null || databaseName == "") {
			EditorPrefs.SetString("databaseName" + prefix + DatabasePack.GetProjectName(), "world_content");
		}
		databaseUser = EditorPrefs.GetString("databaseUser" + prefix + DatabasePack.GetProjectName());
		if (databaseUser == null || databaseUser == "") {
			EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), "root");
		}
		
		prefix = DatabasePack.adminDatabasePrefix;
		databaseHost = EditorPrefs.GetString("databaseHost" + prefix + DatabasePack.GetProjectName());
		if (databaseHost == null || databaseHost == "") {
			EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), "localhost");
		}
		databaseName = EditorPrefs.GetString("databaseName" + prefix + DatabasePack.GetProjectName());
		if (databaseName == null || databaseName == "") {
			EditorPrefs.SetString("databaseName" + prefix + DatabasePack.GetProjectName(), "admin");
		}
		databaseUser = EditorPrefs.GetString("databaseUser" + prefix + DatabasePack.GetProjectName());
		if (databaseUser == null || databaseUser == "") {
			EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), "root");
		}
	}

	
	// Update is called once per frame
	void Update ()
	{
	}
	
	private int SelectTab (Rect pos, int sel)
	{
		pos.y += ImagePack.tabTop;
		pos.x += ImagePack.tabLeft;
		bool edit = false;
		bool doc = false;
		
		switch (sel) { 
		case 1:
			edit = true;
			break;
		case 2:
			doc = true;
			break;			
		}
		
		pos.x += ImagePack.tabMargin;
		if (edit)
			pos.y += ImagePack.tabSpace;
		if (ImagePack.DrawTabEdit (pos, edit))
			return 1;
		if (edit)
			pos.y -= ImagePack.tabSpace;
		pos.x += ImagePack.tabMargin;
		if (doc)
			pos.y += ImagePack.tabSpace;
		if (ImagePack.DrawTabDoc (pos, doc))
			return 2;
		if (doc)
			pos.y -= ImagePack.tabSpace;
				
		return sel;
	}
	
	// Draw the function inspector
	// box: Rect representing the inspector area
	public override void Draw (Rect box)
	{
		
		// Draw the Control Tabs
		selected = SelectTab (box, selected);
		
		if (selected == 1) {
			// Set the drawing layout
			Rect pos = box;
			pos.x += ImagePack.innerMargin;
			pos.y += ImagePack.innerMargin;
			pos.width -= ImagePack.innerMargin;
			pos.height = ImagePack.fieldHeight;

			// Draw the content database info
			ImagePack.DrawLabel (pos.x, pos.y, "Content Database Configuration");
			DrawDataBase (pos, "content");
			// Draw the admin database info
			pos.y += 230;
			ImagePack.DrawLabel (pos.x, pos.y, "Administrator Database Configuration");
			DrawDataBase (pos, "admin");
				
			ImagePack.DrawScrollBar (box.x + box.width, box.y, box.height - 14);
		} else if (selected == 2) {
			DrawHelp (box);	
		} 	
	}
	
	// Basic Database Draw
	void DrawDataBase (Rect box, string type)
	{
		// Layout
		float posX = box.x + ImagePack.innerMargin;
		float posY = box.y;
		float width = box.width - 2 * ImagePack.innerMargin;
		float height = ImagePack.fieldHeight;
		
		// Fields prefix
		string prefix = "";
		if (type == "admin")
			prefix = DatabasePack.adminDatabasePrefix;
		
		// Database Fields		
		posY += height;
		databaseHost = ImagePack.DrawSavedData (new Rect (posX, posY, width, height), "Database Host:", "databaseHost" + prefix + DatabasePack.GetProjectName(), false);
		posY += height;
		databaseHost = ImagePack.DrawSavedData(new Rect(posX, posY, width, height), "Database Port:", "databasePort" + prefix + DatabasePack.GetProjectName(), false);	
		posY += height;
		databaseName = ImagePack.DrawSavedData(new Rect(posX, posY, width, height), "Database Name:", "databaseName" + prefix + DatabasePack.GetProjectName(), false);		
		posY += height;
		databaseUser = ImagePack.DrawSavedData(new Rect(posX, posY, width, height), "Database User:", "databaseUser" + prefix + DatabasePack.GetProjectName(), false);		
		posY += height;
		databasePassword = ImagePack.DrawSavedData(new Rect(posX, posY, width, height), "Database Password:", "databasePassword" + prefix + DatabasePack.GetProjectName(), true);		
		posY += 1.4f * height;
		
		// Test the connection
		if (ImagePack.DrawButton (posX, posY, "Test Connection")) {
			testType = type;	
			DatabasePack.TestConnection (prefix);
		}
		
		if (type == testType)
			GUI.Label (new Rect (posX + width / 2, posY, width / 2, height), DatabasePack.connectionResult, ImagePack.FieldStyle ());	
	}

}

