using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Effects Configuration
public class ServerCurrency : AtavismDatabaseFunction
{

		public Dictionary<int, CurrencyData> dataRegister;
		public CurrencyData editingDisplay;
		public CurrencyData originalDisplay;
	
		// Handles the prefab creation, editing and save
		private CurrencyPrefab prefab = null;
		public string[] statFunctionOptions = new string[] {"~ none ~"};

		// Use this for initialization
		public ServerCurrency ()
		{	
				functionName = "Currencies";
				// Database tables name
				tableName = "currencies";
				functionTitle = "Currency Configuration";
				loadButtonLabel = "Load Currencies";
				notLoadedText = "No Currencies loaded.";
				// Init
				dataRegister = new Dictionary<int, CurrencyData> ();

				editingDisplay = new CurrencyData ();			
				originalDisplay = new CurrencyData ();			
		}

		public override void Activate ()
		{
				linkedTablesLoaded = false;
		}

		// Load Database Data
		public override void Load ()
		{
				if (!dataLoaded) {
						// Clean old data
						dataRegister.Clear ();
						displayKeys.Clear ();

						// Read all entries from the table
						string query = "SELECT * FROM " + tableName + " where isSubCurrency = 0";
			
						// If there is a row, clear it.
						if (rows != null)
								rows.Clear ();
		
						// Load data
						rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
						//Debug.Log("#Rows:"+rows.Count);
						// Read all the data
						if ((rows != null) && (rows.Count > 0)) {
								foreach (Dictionary<string,string> data in rows) {
										//foreach(string key in data.Keys)
										//	Debug.Log("Name[" + key + "]:" + data[key]);
										//return;
										CurrencyData display = new CurrencyData ();
										display.id = int.Parse (data ["id"]);
										display.name = data ["name"]; 
										display.icon = data ["icon"];
										display.description = data ["description"];
										display.maximum = int.Parse (data ["maximum"]);
										display.external = bool.Parse (data ["external"]);
										display.isSubCurrency = bool.Parse (data ["isSubCurrency"]);
										display.subCurrency1ID = int.Parse (data ["subCurrency1"]);
										display.subCurrency2ID = int.Parse (data ["subCurrency2"]);

										display.isLoaded = true;
										//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
										dataRegister.Add (display.id, display);
										displayKeys.Add (display.id);
								}
								LoadSelectList ();
						}
						dataLoaded = true;
				}
				
			foreach (CurrencyData data in dataRegister.Values) {
				if (data.subCurrency1ID > 0) {
					data.subCurrency1 = LoadSubCurrency (data.subCurrency1ID);
				}
				if (data.subCurrency2ID > 0) {
					data.subCurrency2 = LoadSubCurrency (data.subCurrency2ID);
				}
			}
		}

		CurrencyData LoadSubCurrency (int subCurrencyID)
		{
				// Read all entries from the table
				string query = "SELECT * FROM " + "currencies" + " where id = " + subCurrencyID;
		
				// If there is a row, clear it.
				if (rows != null)
						rows.Clear ();
		
				// Load data
				rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
				//Debug.Log("#Rows:"+rows.Count);
				// Read all the data
				if ((rows != null) && (rows.Count > 0)) {
						foreach (Dictionary<string,string> data in rows) {
								CurrencyData display = new CurrencyData ();
								display.id = int.Parse (data ["id"]);
								display.name = data ["name"]; 
								display.icon = data ["icon"];
								display.maximum = int.Parse (data ["maximum"]);
								display.isSubCurrency = true;
								return display;
						}
				}
				return null;
		}
	
		public void LoadSelectList ()
		{
				//string[] selectList = new string[dataRegister.Count];
				displayList = new string[dataRegister.Count];
				int i = 0;
				foreach (int displayID in dataRegister.Keys) {
						//selectList [i] = displayID + ". " + dataRegister [displayID].name;
						displayList [i] = displayID + ". " + dataRegister [displayID].name;
						i++;
				}
				//displayList = new Combobox(selectList);
		}	
	
	
		// Draw the loaded list
		public override void DrawLoaded (Rect box)
		{	
				// Setup the layout
				Rect pos = box;
				pos.x += ImagePack.innerMargin;
				pos.y += ImagePack.innerMargin;
				pos.width -= ImagePack.innerMargin;
				pos.height = ImagePack.fieldHeight;
						
				if (dataRegister.Count <= 0) {
						pos.y += ImagePack.fieldHeight;
						ImagePack.DrawLabel (pos.x, pos.y, "You must create a Currency before editing it.");		
						return;
				}
		
				// Draw the content database info
				ImagePack.DrawLabel (pos.x, pos.y, "Currency Configuration");

				if (newItemCreated) {
						newItemCreated = false;
						LoadSelectList ();
						newSelectedDisplay = displayKeys.Count - 1;
				}
		

				// Draw data Editor
				if (newSelectedDisplay != selectedDisplay) {
						selectedDisplay = newSelectedDisplay;	
						int displayKey = displayKeys [selectedDisplay];
						editingDisplay = dataRegister [displayKey];		
						originalDisplay = editingDisplay.Clone ();
				} 

				//if (!displayList.showList) {
				pos.y += ImagePack.fieldHeight;
				pos.x -= ImagePack.innerMargin;
				pos.y -= ImagePack.innerMargin;
				pos.width += ImagePack.innerMargin;
				DrawEditor (pos, false);
				pos.y -= ImagePack.fieldHeight;
				//pos.x += ImagePack.innerMargin;
				pos.y += ImagePack.innerMargin;
				pos.width -= ImagePack.innerMargin;
				//}
		
				if (state != State.Loaded) {
						// Draw combobox
						pos.width /= 2;
						pos.x += pos.width;
						newSelectedDisplay = ImagePack.DrawCombobox (pos, "", selectedDisplay, displayList);
						pos.x -= pos.width;
						pos.width *= 2;
				}

		}

		public override void CreateNewData ()
		{
				editingDisplay = new CurrencyData ();		
				originalDisplay = new CurrencyData ();
				selectedDisplay = -1;
		}
	
		// Edit or Create
		public override void DrawEditor (Rect box, bool newItem)
		{
		
				// Setup the layout
				Rect pos = box;
				pos.x += ImagePack.innerMargin;
				pos.y += ImagePack.innerMargin;
				pos.width -= ImagePack.innerMargin;
				pos.height = ImagePack.fieldHeight;
		
				if (!linkedTablesLoaded) {	
						linkedTablesLoaded = true;
				}

				// Draw the content database info
				//pos.y += ImagePack.fieldHeight;
		
				if (newItem) {
						ImagePack.DrawLabel (pos.x, pos.y, "Create a new Currency");		
						pos.y += ImagePack.fieldHeight;
				}
		
				editingDisplay.name = ImagePack.DrawField (pos, "Name:", editingDisplay.name, 0.75f);
				pos.y += ImagePack.fieldHeight;
				pos.width /= 2;
				editingDisplay.maximum = ImagePack.DrawField (pos, "Max:", editingDisplay.maximum);
				pos.x += pos.width;
				editingDisplay.icon = ImagePack.DrawTextureAsset (pos, "Icon:", editingDisplay.icon);		
				pos.x -= pos.width;
				pos.y += ImagePack.fieldHeight;
				editingDisplay.external = ImagePack.DrawToggleBox (pos, "External:", editingDisplay.external);
				pos.y += ImagePack.fieldHeight;
				editingDisplay.description = ImagePack.DrawField (pos, "Description:", editingDisplay.description);
				/*if (!newItem) {
						pos.y += ImagePack.fieldHeight * 1.5f;
						if (editingDisplay.subCurrency1 != null) {
						ImagePack.DrawLabel (pos, "Sub-currency 1");
						pos.y += ImagePack.fieldHeight;
						editingDisplay.subCurrency1.name = ImagePack.DrawField (pos, "Name:", editingDisplay.subCurrency1.name);
						pos.x += pos.width;
						editingDisplay.subCurrency1.icon = ImagePack.DrawTextureAsset (pos, "Icon:", editingDisplay.subCurrency1.icon);		
						pos.x -= pos.width;
						pos.y += ImagePack.fieldHeight;
						editingDisplay.subCurrency1.maximum = ImagePack.DrawField (pos, "Max:", editingDisplay.subCurrency1.maximum);
						pos.y += ImagePack.fieldHeight * 2.5f;
						} else {
							if (ImagePack.DrawButton (pos.x, pos.y, "Add Sub Currency")) {
								editingDisplay.subCurrency1 = new CurrencyData();
								editingDisplay.subCurrency1.maximum = 99;
								editingDisplay.subCurrency1.isSubCurrency = true;
							}
							pos.y += ImagePack.fieldHeight * 2.5f;
						}
						if (editingDisplay.subCurrency2 != null) {
						ImagePack.DrawLabel (pos, "Sub-currency 2");
						pos.y += ImagePack.fieldHeight;
						editingDisplay.subCurrency2.name = ImagePack.DrawField (pos, "Name:", editingDisplay.subCurrency2.name);
						pos.x += pos.width;
						editingDisplay.subCurrency2.icon = ImagePack.DrawTextureAsset (pos, "Icon:", editingDisplay.subCurrency2.icon);		
						pos.x -= pos.width;
						pos.y += ImagePack.fieldHeight;
						editingDisplay.subCurrency2.maximum = ImagePack.DrawField (pos, "Max:", editingDisplay.subCurrency2.maximum);
						} else if (editingDisplay.subCurrency1 != null) {
							if (ImagePack.DrawButton (pos.x, pos.y, "Add Sub Currency")) {
								editingDisplay.subCurrency2 = new CurrencyData();
								editingDisplay.subCurrency2.maximum = 99;
								editingDisplay.subCurrency2.isSubCurrency = true;
							}
							pos.y += ImagePack.fieldHeight * 1.5f;
						}
				}*/

				pos.width *= 2;

				pos.y += 2.5f * ImagePack.fieldHeight; 
				// Save data
				pos.x -= ImagePack.innerMargin;
				pos.width /= 3;
				if (ImagePack.DrawButton (pos.x, pos.y, "Save Data")) {
						if (newItem)
								InsertEntry ();
						else
								UpdateEntry ();
			
						state = State.Loaded;
				}
		
				// Delete data
				if (!newItem) {
						pos.x += pos.width;
						if (ImagePack.DrawButton (pos.x, pos.y, "Delete Data")) {
								DeleteEntry ();
								newSelectedDisplay = 0;
								state = State.Loaded;
						}
				}
		
				// Cancel editing
				pos.x += pos.width;
				if (ImagePack.DrawButton (pos.x, pos.y, "Cancel")) {
						editingDisplay = originalDisplay.Clone ();
						if (newItem)
								state = State.New;
						else
								state = State.Loaded;
				}
		
				if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup) {
						pos.y += ImagePack.fieldHeight;
						ImagePack.DrawText (pos, result);
				}

		}
	
		// Insert new entries into the table
		void InsertEntry ()
		{
				NewResult ("Inserting...");
				// Setup the update query
				string query = "INSERT INTO " + tableName;		
				query += " (" + editingDisplay.FieldList ("", ", ") + ") ";
				query += "VALUES ";
				query += " (" + editingDisplay.FieldList ("?", ", ") + ") ";
		
				int itemID = -1;

				// Setup the register data		
				List<Register> update = new List<Register> ();
				foreach (string field in editingDisplay.fields.Keys) {
						update.Add (editingDisplay.fieldToRegister (field));       
				}
		
				// Update the database
				itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);

				// If the insert failed, don't insert the spawn marker
				if (itemID != -1) {          
						// Update online table to avoid access the database again			
						editingDisplay.id = itemID;
						editingDisplay.isLoaded = true;
						//Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
						dataRegister.Add (editingDisplay.id, editingDisplay);
						displayKeys.Add (editingDisplay.id);
						newItemCreated = true;
						// Configure the correponding prefab
						CreatePrefab ();
						NewResult ("New entry inserted");
				} else {
						NewResult ("Error occurred, please check the Console");
				}
		}

		// Update existing entries in the table based on the iddemo_table
		void UpdateEntry ()
		{
		NewResult ("Updating...");
			if (editingDisplay.subCurrency1 != null) {
				if (editingDisplay.subCurrency1.id > 1) {
					// Update existing entry
					UpdateSubcurrency (editingDisplay.subCurrency1);
				} else {
					// Insert new entry
					int subCurrency1ID = InsertSubCurrency (editingDisplay.subCurrency1);
					// Update the main currency entry to include the new id of the sub currency
					editingDisplay.subCurrency1ID = subCurrency1ID;
				}
			}
		if (editingDisplay.subCurrency2 != null) {
			if (editingDisplay.subCurrency2.id > 1) {
				// Update existing entry
				UpdateSubcurrency (editingDisplay.subCurrency2);
			} else {
				// Insert new entry
				int subCurrency2ID = InsertSubCurrency (editingDisplay.subCurrency2);
				// Update the main currency entry to include the new id of the sub currency
				editingDisplay.subCurrency2ID = subCurrency2ID;
			}
		}
				
				// Setup the update query
				string query = "UPDATE " + tableName;
				query += " SET ";
				query += editingDisplay.UpdateList ();
				query += " WHERE id=?id";
		
				// Setup the register data		
				List<Register> update = new List<Register> ();
				foreach (string field in editingDisplay.fields.Keys) {
						update.Add (editingDisplay.fieldToRegister (field));       
				}
				update.Add (new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int));
		
				// Update the database
				DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, update);
				
				// Update online table to avoid access the database again			
				dataRegister [displayKeys [selectedDisplay]] = editingDisplay;
		// Configure the correponding prefab
		CreatePrefab ();
				NewResult ("Entry updated");				
		}

		/// <summary>
		/// Inserts a new entry in the currencies database for the sub currency.
		/// </summary>
		/// <returns>The sub currency.</returns>
		/// <param name="subCurrency">Sub currency.</param>
		/// <param name="parent">Parent.</param>
		int InsertSubCurrency (CurrencyData subCurrency)
		{
				// Setup the update query
				string query = "INSERT INTO " + tableName;		
				query += " (" + subCurrency.FieldList ("", ", ") + ") ";
				query += "VALUES ";
				query += " (" + subCurrency.FieldList ("?", ", ") + ") ";

				int itemID = -1;
		
				// Setup the register data		
				List<Register> update = new List<Register> ();
				foreach (string field in subCurrency.fields.Keys) {
						update.Add (subCurrency.fieldToRegister (field));       
				}
		
				// Update the database
				itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, update);

				return itemID;
		}

		// Update an existing sub currency
		void UpdateSubcurrency (CurrencyData subCurrency)
		{
				NewResult ("Updating...");
				// Setup the update query
				string query = "UPDATE " + tableName;
				query += " SET ";
				query += subCurrency.UpdateList ();
				query += " WHERE id=?id";
		
				// Setup the register data		
				List<Register> update = new List<Register> ();
				foreach (string field in subCurrency.fields.Keys) {
						update.Add (subCurrency.fieldToRegister (field));       
				}
				update.Add (new Register ("id", "?id", MySqlDbType.Int32, subCurrency.id.ToString (), Register.TypesOfField.Int));
		}
	
		// Delete entries from the table
		void DeleteEntry ()
		{
		// Remove the prefab
		DeletePrefab ();
		
				Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
				DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
		
				// Update online table to avoid access the database again			
				dataRegister.Remove (displayKeys [selectedDisplay]);
				displayKeys.Remove (selectedDisplay);
				if (dataRegister.Count > 0)		
						LoadSelectList ();
				else {
						displayList = null;
						dataLoaded = false;
				}

		}

		void CreatePrefab ()
		{
			// Configure the correponding prefab
			prefab = new CurrencyPrefab (editingDisplay.id, editingDisplay.name);
			int subCurrency1ID = -1;
			string subCurrency1Name = "";
			string subCurrency1Icon = "";
			if (editingDisplay.subCurrency1 != null) {
				subCurrency1ID = editingDisplay.subCurrency1ID;
				subCurrency1Name = editingDisplay.subCurrency1.name;
				subCurrency1Icon = editingDisplay.subCurrency1.icon;
			}
			int subCurrency2ID = -1;
			string subCurrency2Name = "";
			string subCurrency2Icon = "";
			if (editingDisplay.subCurrency2 != null) {
				subCurrency2ID = editingDisplay.subCurrency2ID;
				subCurrency2Name = editingDisplay.subCurrency2.name;
				subCurrency2Icon = editingDisplay.subCurrency2.icon;
			}
		prefab.Save (editingDisplay.icon, subCurrency1ID, subCurrency1Name, subCurrency1Icon, 
		             subCurrency2ID, subCurrency2Name, subCurrency2Icon);
		}
	
		void DeletePrefab ()
		{
				prefab = new CurrencyPrefab (editingDisplay.id, editingDisplay.name);
		
				if (prefab.Load ())
						prefab.Delete ();
		}
}
