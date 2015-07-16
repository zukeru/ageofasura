using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// This is the base class for Atavism Database Access Functions
public class AtavismDatabaseFunction : AtavismFunction
{
	// Database Configuration States
	public enum State
	{
		Loading,
		Loaded,
		Edit,
		New,
		Doc
	}
	
	// State of editing
	public State state = State.New;
	
	// command object
	public MySqlCommand cmd = null;
	
	// Store database row fetchs
	public List<Dictionary<string, string>> rows = new List<Dictionary<string, string>> ();

	// Database tables name
	public string tableName = "";
	public string functionTitle = "Configuration";
	public string loadButtonLabel = "Load";
	public string notLoadedText = "Not loaded.";
	
	// Dictionary to handle data fields
	public Dictionary<int, DataStructure> dataRegister;
	public List<int> displayKeys = new List<int> ();
	
	// Stores the selected register
	public int selectedDisplay = -1;
	public int newSelectedDisplay = 0;
	
	public DataStructure editingDisplay;
	public DataStructure originalDisplay;
	
	public bool dataLoaded = false;
	public bool newItemCreated = false;
	public bool dataSaved = false;

	// Used to load in data from other tables if set to false
	public bool linkedTablesLoaded = false;

	public Vector2 inspectorScrollPosition = Vector2.zero;
	public float inspectorHeight = 0;

	//public Combobox displayList;
	public string[] displayList;
	
	// Tab selection
	public int selected = 0;
	
	// Result text
	public string result = "";
	public float resultTimeout = -1;
	public float resultDuration = 5;

	/// <summary>
	/// Enables the scroll bar and sets total window height
	/// </summary>
	/// <param name="windowHeight">Window height.</param>
	public void EnableScrollBar(float windowHeight)
	{
		inspectorHeight = windowHeight;
	}
	
	private int SelectTab (Rect pos, int sel)
	{
		pos.y += ImagePack.tabTop;
		pos.x += ImagePack.tabLeft;
		bool create = false;
		bool edit = false;
		bool doc = false;
		
		switch (sel) { 
		case 0:
			create = true;
			break;
		case 1:
			edit = true;
			break;
		case 2:
			doc = true;
			break;			
		}
		
		if (create)
			pos.y += ImagePack.tabSpace;
		if (ImagePack.DrawTabCreate (pos, create))
			return 0;
		if (create)
			pos.y -= ImagePack.tabSpace;
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
	
	// Draw the user interface
	public override void Draw (Rect box)
	{			
		//ImagePack.DrawScrollBar (box.x + box.width, box.y, box.height - 14);

		// Draw the Control Tabs
		int newSelected = SelectTab (box, selected);
		
		if (newSelected != selected) {
			selected = newSelected;
			switch (selected) { 
			case 0: // Create New
				CreateNewData();
				state = State.New;
				break;
			case 1:	// Edit
				state = State.Loading;
				break;
			case 2:	// Documentation
				state = State.Doc;
				break;			
			}
		}

		Rect inspectorScrollWindow = box;
		Rect inspectorWindow = box;	
		inspectorWindow.width -= 2;
		inspectorScrollWindow.width += 14;
		inspectorWindow.height = Mathf.Max(box.height, inspectorHeight);

		// Switch between editing states
		switch (state) {
		case State.Loading:
			// Load database information
			selected = 1;
			state = State.Loaded;
			Load ();
			dataSaved = false;
			break;
		case State.Loaded:
			// After loading, shows
			if (dataLoaded) {
				state = State.Edit;
			} else {
				state = State.Loading;
			}
			break;
		case State.Edit:
			// Editing register
			selected = 1;
			inspectorScrollPosition = GUI.BeginScrollView(inspectorScrollWindow, inspectorScrollPosition, inspectorWindow);
			DrawLoaded (box);
			GUI.EndScrollView();
			break;
		case State.New:
			// Create a new register
			selected = 0;
			inspectorScrollPosition = GUI.BeginScrollView(inspectorScrollWindow, inspectorScrollPosition, inspectorWindow);
			DrawEditor (box, true);
			GUI.EndScrollView();
			break;
		case State.Doc:
			// Create a new register
			selected = 2;
			inspectorScrollPosition = GUI.BeginScrollView(inspectorScrollWindow, inspectorScrollPosition, inspectorWindow);
			inspectorHeight = DrawHelp (box);
			GUI.EndScrollView();
			break;
		}
		
	}
	
	// Draw the Instance list
	public virtual bool DrawWaitingLoading (Rect box)
	{	
		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;

		// Draw the content database info
		pos.y += ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, functionTitle);
			
		// Load Instances Button
		pos.y += 2 * ImagePack.fieldHeight;
		if (ImagePack.DrawButton (pos.x, pos.y, loadButtonLabel))
			return true;

		// Show current instances
		pos.y += 2 * ImagePack.fieldHeight;
		ImagePack.DrawLabel (pos.x, pos.y, notLoadedText);
		
		return false;
	}
	
	public virtual void NewResult(string resultMessage) {
		result = resultMessage;
		resultTimeout = Time.realtimeSinceStartup + resultDuration;
	}
	
	public virtual void Load ()
	{
		
	}
	
	public virtual void DrawLoaded (Rect box)
	{
		
	}
		
	public virtual void DrawEditor (Rect box, bool newItem)
	{
		
	}
	
	public virtual void CreateNewData()
	{
	}
	
}
