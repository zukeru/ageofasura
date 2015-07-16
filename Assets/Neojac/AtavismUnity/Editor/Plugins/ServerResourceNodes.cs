using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// Handles the Effects Configuration
public class ServerResourceNodes : AtavismDatabaseFunction
{

	public Dictionary<int, StatsData> dataRegister;
	public StatsData editingDisplay;
	public StatsData originalDisplay;
	bool showConfirmDelete = false;

	// Use this for initialization
	public ServerResourceNodes ()
	{	
		functionName = "Resource Nodes";		
		// Database tables name
		tableName = "resource_node_template";
		functionTitle = "Resource Nodes";
		loadButtonLabel = "";
		notLoadedText = "";
		// Init
		dataRegister = new Dictionary<int, StatsData> ();

		editingDisplay = new StatsData ();			
		originalDisplay = new StatsData ();	
		showConfirmDelete = false;		
	}

	public override void Activate()
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
			string query = "SELECT * FROM " + tableName;
			
			// If there is a row, clear it.
			if (rows != null)
				rows.Clear ();
		
			// Load data
			rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
			//Debug.Log("#Rows:"+rows.Count);
			// Read all the data
			int fakeId = 0;
			if ((rows!=null) && (rows.Count > 0)) {
				foreach (Dictionary<string,string> data in rows) {
					//foreach(string key in data.Keys)
					//	Debug.Log("Name[" + key + "]:" + data[key]);
					//return;
					StatsData display = new StatsData ();
					// As we don have a primary key ID field
					fakeId++;
					display.id = fakeId;
					display.name = data ["name"]; 

					display.isLoaded = true;
					//Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
					dataRegister.Add (display.id, display);
					displayKeys.Add (display.id);
				}
				LoadSelectList();
			}
			dataLoaded = true;
		}
	}
	
	public void LoadSelectList() 
	{
			//string[] selectList = new string[dataRegister.Count];
			displayList =  new string[dataRegister.Count];
			int i = 0;
			foreach (int displayID in dataRegister.Keys) {
				//selectList [i] = displayID + ". " + dataRegister [displayID].name;
				displayList [i] = displayID + ". " + dataRegister [displayID].name;
				i++;
			}
			//displayList = new Combobox(selectList);
	}	
	
	
	// Draw the loaded list
	public override  void DrawLoaded (Rect box)
	{	
		// Setup the layout
		Rect pos = box;
		pos.x += ImagePack.innerMargin;
		pos.y += ImagePack.innerMargin;
		pos.width -= ImagePack.innerMargin;
		pos.height = ImagePack.fieldHeight;
						
		if (dataRegister.Count <= 0) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawLabel (pos.x, pos.y, "");		
			return;
		}
		
		// Draw the content database info
		ImagePack.DrawLabel (pos.x, pos.y, "Resource Configuration");

		if (newItemCreated) {
			newItemCreated = false;
			LoadSelectList();
			newSelectedDisplay = displayKeys.Count - 1;
		}
		

		// Draw data Editor
		if (newSelectedDisplay != selectedDisplay) {
		selectedDisplay = newSelectedDisplay;	
		int displayKey = displayKeys [selectedDisplay];
		editingDisplay = dataRegister [displayKey];		
			originalDisplay = editingDisplay.Clone();
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
		
	public override void CreateNewData()
	{
		editingDisplay = new StatsData ();		
		originalDisplay = new StatsData ();
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
		
		ImagePack.DrawLabel (pos.x, pos.y, "Save Resource Nodes");
		pos.y += ImagePack.fieldHeight;
		ImagePack.DrawText(pos, "Save the Resource Nodes in the current scene by clicking below.");
		pos.y += ImagePack.fieldHeight;
		ImagePack.DrawText(pos, "When the Resource Nodes have been saved, save your scene again.");
		pos.y += ImagePack.fieldHeight;
		if (ImagePack.DrawButton (pos.x, pos.y, "Save Nodes")) {
			GetSceneResourceNodes();
			showConfirmDelete = false;
		}
		
		pos.y += ImagePack.fieldHeight;
		ImagePack.DrawText(pos, "To delete all node data from this Scene in the Database, click");
		pos.y += ImagePack.fieldHeight;
		ImagePack.DrawText(pos, "Delete Node Data.");
		pos.y += ImagePack.fieldHeight;
		if (ImagePack.DrawButton (pos.x, pos.y, "Delete Node Data")) {
			showConfirmDelete = true;
		}
		
		if (showConfirmDelete) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawText(pos, "Are you sure?");
			pos.y += ImagePack.fieldHeight;
			if (ImagePack.DrawButton (pos.x, pos.y, "Yes, delete")) {
				ClearSavedInstanceNodeData();
				showConfirmDelete = false;
			}
		}
		
		
		if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup) {
			pos.y += ImagePack.fieldHeight;
			ImagePack.DrawText(pos, result);
		}

	}
	
	void GetSceneResourceNodes() {
		NewResult ("Saving...");
		string instance = EditorApplication.currentScene;
		string[] split = instance.Split(char.Parse("/"));
		instance = split[split.Length -1];
		split = instance.Split(char.Parse("."));
		instance = split[0];
		
		List<ResourceNode> preexistingNodes = new List<ResourceNode>();
		List<ResourceNode> newNodes = new List<ResourceNode>();
		
		// Find all resource nodes in the scene
		int itemID = -1;
		UnityEngine.Object[] resourceNodes = FindObjectsOfType<ResourceNode>();
		string query = "";
		foreach (UnityEngine.Object resourceNodeObj in resourceNodes) {
			ResourceNode resourceNode = (ResourceNode)resourceNodeObj;
			if (resourceNode.id > 0) {
				preexistingNodes.Add(resourceNode);
			} else {
				newNodes.Add(resourceNode);
			}
		}
	
		foreach (ResourceNode resourceNode in newNodes) {
			// Insert the new resource node
			string coordEffect = "";
			if (resourceNode.harvestCoordEffect != null)
				coordEffect = resourceNode.harvestCoordEffect.name;
			query = "INSERT INTO resource_node_template (name, skill, skillLevel, skillLevelMax, weaponReq, equipped, gameObject, coordEffect" 
				+ ", instance, respawnTime, locX, locY, locZ, harvestCount, harvestTimeReq)" + " values ('" + resourceNode.name + "'," + resourceNode.skillType + "," 
				+ resourceNode.reqSkillLevel + "," + resourceNode.skillLevelMax + "," + "'" + resourceNode.harvestTool + "'," 
				+ resourceNode.ToolMustBeEquipped + ",'','" + coordEffect + "','" + instance + "',"
				+ resourceNode.refreshDuration + "," + resourceNode.transform.position.x + "," + resourceNode.transform.position.y + "," 
				+ resourceNode.transform.position.z + "," + resourceNode.resourceCount + "," + resourceNode.timeToHarvest + ")";	
			itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, new List<Register>());
			
			if (itemID == -1) {
				// Insert failed, set message
				return;
			}
			resourceNode.id = itemID;
			EditorUtility.SetDirty( resourceNode );
			
			foreach (ResourceDrop drop in resourceNode.resources) {
				// Insert the resource drops
				if (drop.item == null) {
					Debug.LogWarning("ResourceDrop skipped for resourceNode " + itemID + " as it has no item");
					continue;
				}
				query = "INSERT INTO resource_drop (resource_template, item, min, max, chance)"
					+ " values(" + itemID + "," + drop.item.TemplateId + "," + drop.min + "," + drop.max + "," + drop.chance + ")";
				DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, new List<Register>());
			}
		}
		
		foreach (ResourceNode resourceNode in preexistingNodes) {
			// Update the existing resource node
			string coordEffect = "";
			if (resourceNode.harvestCoordEffect != null)
				coordEffect = resourceNode.harvestCoordEffect.name;
			query = "UPDATE resource_node_template set name = '" + resourceNode.name + "', skill = " + resourceNode.skillType + ", skillLevel = " 
				+ resourceNode.reqSkillLevel + ", skillLevelMax = " + resourceNode.skillLevelMax + ", weaponReq = '" + resourceNode.harvestTool
				+ "', equipped = " + resourceNode.ToolMustBeEquipped + ", gameObject = '', coordEffect = '" + coordEffect + "', instance = '" 
				+ instance + "', respawnTime = " + resourceNode.refreshDuration + ", locX = " + resourceNode.transform.position.x 
				+ ", locY = " + resourceNode.transform.position.y + ", locZ = " + resourceNode.transform.position.z + ", harvestCount = "
				+ resourceNode.resourceCount + ", harvestTimeReq = " + resourceNode.timeToHarvest + " where id = " + resourceNode.id;
			DatabasePack.Update (DatabasePack.contentDatabasePrefix, query, new List<Register>());
			
			// Delete the existing resource drops for this node - not the most efficient, but easier to do
			Register delete = new Register ("resource_template", "?resource_template", MySqlDbType.Int32, resourceNode.id.ToString (), Register.TypesOfField.Int);
			DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "resource_drop", delete, false);
			
			// And re-insert them
			foreach (ResourceDrop drop in resourceNode.resources) {
				// Insert the resource drops
				if (drop.item == null) {
					Debug.LogWarning("ResourceDrop skipped for resourceNode " + itemID + " as it has no item");
					continue;
				}
				query = "INSERT INTO resource_drop (resource_template, item, min, max, chance)"
					+ " values(" + resourceNode.id + "," + drop.item.TemplateId + "," + drop.min + "," + drop.max + "," + drop.chance + ")";
				DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, new List<Register>());
			}
		}
		
		NewResult ("Nodes saved");
	}
	
	void ClearSavedInstanceNodeData() {
		NewResult ("Deleting...");
		string instance = EditorApplication.currentScene;
		string[] split = instance.Split(char.Parse("/"));
		instance = split[split.Length -1];
		split = instance.Split(char.Parse("."));
		instance = split[0];
		
		DatabasePack.con = DatabasePack.Connect(DatabasePack.contentDatabasePrefix);
		// Get all resource nodes that are to be deleted
		string query = "Select id from resource_node_template where instance = '" + instance + "'";
		
		try
		{
			List<int> dropsToDelete = new List<int>();
			// Open the connection
			if (DatabasePack.con.State.ToString() != "Open")
				DatabasePack.con.Open();
			// Use the connections to fetch data
			using (DatabasePack.con)
			{
				using (MySqlCommand cmd = new MySqlCommand(query, DatabasePack.con))
				{
					// Execute the query
					MySqlDataReader data = cmd.ExecuteReader();
					// If there are columns
					if (data.HasRows) {
						int fieldsCount = data.FieldCount;
						while (data.Read()) {
							dropsToDelete.Add(data.GetInt32("id"));
						}
					}
					data.Dispose();
				}
			}
			
			foreach (int id in dropsToDelete) {
				Register delete = new Register ("resource_template", "?resource_template", MySqlDbType.Int32, id.ToString (), Register.TypesOfField.Int);
				DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "resource_drop", delete, false);
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
			NewResult ("Error occurred deleting old entries");
			return;
		}
		finally
		{
		}
		
		// Now delete all resource nodes in this instance
		Register delete2 = new Register ("instance", "?instance", MySqlDbType.String, instance, Register.TypesOfField.String);
		DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "resource_node_template", delete2, true);
		
		// Finally, reset the id for each node in the scene
		UnityEngine.Object[] resourceNodes = FindObjectsOfType<ResourceNode>();
		foreach (UnityEngine.Object resourceNodeObj in resourceNodes) {
			ResourceNode resourceNode = (ResourceNode)resourceNodeObj;
			resourceNode.id = -1;
			EditorUtility.SetDirty(resourceNode);
		}
		NewResult ("Node data deleted");	
	}
	
}
