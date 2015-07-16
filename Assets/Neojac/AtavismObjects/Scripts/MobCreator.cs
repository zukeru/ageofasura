using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobTemplate
{
	public int ID = -1;
	public string name = "";
	public string subtitle = "";
	public string species = "";
	public string subspecies = "";
	public string displayType = "";
	public string gender = "Female";
	public List<string> displays;
	public float scale = 1.0f;
	public int level = 1;
	public bool attackable = true;
	public int mobType = 0;
	public int faction = -1;
	//public int equippedItems = [];
	//public int lootTables = [];
	
	public MobTemplate ()
	{
		displays = new List<string> ();
	}
	
	public MobTemplate (MobTemplate tmpl)
	{
		ID = tmpl.ID;
		name = tmpl.name;
		subtitle = tmpl.subtitle;
		species = tmpl.species;
		subspecies = tmpl.subspecies;
		displayType = tmpl.displayType;
		gender = tmpl.gender;
		displays = tmpl.displays;
		scale = tmpl.scale;
		level = tmpl.level;
		attackable = tmpl.attackable;
		mobType = tmpl.mobType;
		faction = tmpl.faction;
	}
}

public class QuestTemplate
{
	public int questID;
	public string title;
}

public class DialogueTemplate
{
	public int dialogueID;
	public string title;
}

public class MerchantTableTemplate
{
	public int tableID;
	public string title;
}

public class MobSpawn
{
	public int ID = -1;
	public int mobTemplateID = -1;
	public MobTemplate mobTemplate = null;
	public GameObject marker;
	public Vector3 position;
	public Quaternion orientation;
	public int spawnRadius = 0;
	public int numSpawns = 1;
	public int respawnTime = 60;
	public int despawnTime = 50;
	public float roamRadius = 0;
	public int merchantTable = -1;
	public bool isChest = false;
	public int pickupItemID = -1;
	public List<int> startsQuests = new List<int> ();
	public List<int> endsQuests = new List<int> ();
	public List<int> startsDialogues = new List<int> ();
	
	public string GetMobTemplateName ()
	{
		if (mobTemplate != null)
			return mobTemplate.name + " (" + mobTemplateID + ")";
		return "";
	}
	
	public void CreateMarkerObject (GameObject template)
	{
		marker = (GameObject)GameObject.Instantiate (template, position, orientation);
		marker.GetComponent<SpawnMarker> ().MarkerID = ID;
	}
	
	public void DeleteMarker() {
		GameObject.Destroy(marker);
	}
}

public enum MobCreationState
{
	Menu,
	SelectTemplate,
	SpawnMob,
	SelectSpawn,
	EditSpawn,
	None,
	Disabled
}

public enum MobPropertySelectState
{
	MobTemplate,
	StartQuest,
	EndQuest,
	StartDialogue,
	MerchantTable,
	SpawnPositioning,
	None
}

public class MobCreator : MonoBehaviour
{
	
	public GUISkin skin;
	public GameObject spawnMarkerTemplate;
	List<MobTemplate> mobTemplates = new List<MobTemplate> ();
	List<QuestTemplate> questTemplates = new List<QuestTemplate> ();
	List<DialogueTemplate> dialogueTemplates = new List<DialogueTemplate> ();
	List<MerchantTableTemplate> merchantTables = new List<MerchantTableTemplate> ();
	MobTemplate mobInCreation;
	Dictionary<int, MobSpawn> mobSpawns = new Dictionary<int, MobSpawn> ();
	MobSpawn spawnInCreation;
	MobCreationState mobCreationState;
	bool hasAccess = false;
	bool accessChecked = false;
	Vector2 templateListScrollPosition = Vector2.zero;
	Vector2 questListScrollPosition = Vector2.zero;
	Vector2 selectedListScrollPosition = Vector2.zero;
	int selectedTemplate = -1;
	MobPropertySelectState propertySelectState;
	public int uiLayer = 3;

	// Use this for initialization
	void Start ()
	{
		mobCreationState = MobCreationState.Disabled;
		propertySelectState = MobPropertySelectState.None;
		
		NetworkAPI.RegisterExtensionMessageHandler ("world_developer_response", WorldDeveloperHandler);
		NetworkAPI.RegisterExtensionMessageHandler ("mobTemplates", HandleMobTemplateUpdate);
		NetworkAPI.RegisterExtensionMessageHandler ("questTemplates", HandleQuestTemplateUpdate);
		NetworkAPI.RegisterExtensionMessageHandler ("dialogueTemplates", HandleDialogueTemplateUpdate);
		NetworkAPI.RegisterExtensionMessageHandler ("merchantTables", HandleMerchantTableUpdate);
		NetworkAPI.RegisterExtensionMessageHandler ("add_visible_spawn_marker", HandleSpawnList);
		NetworkAPI.RegisterExtensionMessageHandler ("spawn_data", HandleSpawnData);
		
		// Verify we have access
		CheckAccess();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnGUI ()
	{
		GUI.depth = uiLayer;
		
		if (mobCreationState == MobCreationState.None) {
			DrawEnterBuildMode ();
		} else if (mobCreationState == MobCreationState.Menu) {
			DrawBuilderMenu ();
		} else if (mobCreationState == MobCreationState.SelectSpawn) {
			DrawSelectSpawn ();
		} else if (mobCreationState == MobCreationState.SpawnMob || mobCreationState == MobCreationState.EditSpawn) {
			DrawSpawnMob ();

			if (propertySelectState == MobPropertySelectState.MobTemplate) {
				DrawMobTemplateSelection();
			} else if (propertySelectState == MobPropertySelectState.StartQuest) {
				DrawStartsQuest ();
			} else if (propertySelectState == MobPropertySelectState.EndQuest) {
				DrawEndsQuest ();
			} else if (propertySelectState == MobPropertySelectState.StartDialogue) {
				DrawStartsDialogues ();
			} else if (propertySelectState == MobPropertySelectState.MerchantTable) {
				DrawMerchantTables ();
			} else if (propertySelectState == MobPropertySelectState.SpawnPositioning) {
				DrawSpawnPositioning ();
			}
		}
	}
	
	void DrawEnterBuildMode ()
	{
		if (GUI.Button (new Rect (150, Screen.height - 30, 150, 20), "Enter Build Mode")) {
			StartBuilder (true);
		}
	}
	
	void DrawBuilderMenu ()
	{
		Rect templateRect = new Rect (10, 100, 200, 200);
		GUILayout.BeginArea (templateRect, skin.GetStyle ("Window"));
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Builder Menu");
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("X")) {
			ClearSpawns();
			mobCreationState = MobCreationState.None;
			AtavismUiSystem.RemoveFrame ("MobSpawn", new Rect (0, 0, 0, 0));
		}
		GUILayout.EndHorizontal ();
		if (GUILayout.Button ("Spawn New Mob")) {
			StartMobSpawner ();
		}
		if (GUILayout.Button ("Select Spawn")) {
			mobCreationState = MobCreationState.SelectSpawn;
		}
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Close")) {
			ClearSpawns();
			mobCreationState = MobCreationState.None;
			AtavismUiSystem.RemoveFrame ("MobSpawn", new Rect (0, 0, 0, 0));
		}
		GUILayout.EndArea ();
	}
	
	void DrawSelectSpawn ()
	{
		
		Rect templateRect = new Rect (10, 100, 200, 200);
		GUILayout.BeginArea (templateRect, skin.GetStyle ("Window"));
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Select Spawn");
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("X")) {
			mobCreationState = MobCreationState.Menu;
			AtavismUiSystem.RemoveFrame ("MobSpawn", new Rect (0, 0, 0, 0));
		}
		GUILayout.EndHorizontal ();
		GUILayout.Label ("Click on the spawn you wish to edit");
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Close")) {
			mobCreationState = MobCreationState.Menu;
			AtavismUiSystem.RemoveFrame ("MobSpawn", new Rect (0, 0, 0, 0));
		}
		GUILayout.EndArea ();
	}
	
	void DrawSpawnMob ()
	{
		
		Rect templateRect = new Rect (10, 100, 200, 430);
		GUILayout.BeginArea (templateRect, skin.GetStyle ("Window"));
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Mob Spawner");
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("X")) {
			StartBuilder(false);
		}
		GUILayout.EndHorizontal ();
		GUILayout.Label ("Template: " + spawnInCreation.GetMobTemplateName ());
		if (GUILayout.Button ("Change Template")) {
			propertySelectState = MobPropertySelectState.MobTemplate;
			selectedListScrollPosition = Vector2.zero;
		}
		GUILayout.Label ("Roam Radius: " + spawnInCreation.roamRadius.ToString ("N1"));
		spawnInCreation.roamRadius = GUILayout.HorizontalSlider (spawnInCreation.roamRadius, 0.0f, 10.0f);
		//GUILayout.BeginHorizontal();
		GUILayout.Label ("Despawn Time: " + spawnInCreation.despawnTime + " secs");
		spawnInCreation.despawnTime = (int)GUILayout.HorizontalSlider (spawnInCreation.despawnTime, 5.0f, 300.0f);
		//GUILayout.EndHorizontal();
		//GUILayout.BeginHorizontal();
		GUILayout.Label ("Respawn Time: " + spawnInCreation.respawnTime + " secs");
		spawnInCreation.respawnTime = (int)GUILayout.HorizontalSlider (spawnInCreation.respawnTime, 5.0f, 300.0f);
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Pickup Item: ");
		string pickupItem = GUILayout.TextField ("" + spawnInCreation.pickupItemID);
		spawnInCreation.pickupItemID = int.Parse (pickupItem);
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Is Chest: ");
		spawnInCreation.isChest = GUILayout.Toggle (spawnInCreation.isChest, "");
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button ("Starts Quests")) {
			propertySelectState = MobPropertySelectState.StartQuest;
			selectedListScrollPosition = Vector2.zero;
			questListScrollPosition = Vector2.zero;
		}
		if (GUILayout.Button ("Ends Quests")) {
			propertySelectState = MobPropertySelectState.EndQuest;
			selectedListScrollPosition = Vector2.zero;
			questListScrollPosition = Vector2.zero;
		}
		GUILayout.EndHorizontal ();
		if (GUILayout.Button ("Dialogues")) {
			propertySelectState = MobPropertySelectState.StartDialogue;
			selectedListScrollPosition = Vector2.zero;
			questListScrollPosition = Vector2.zero;
		}
		if (GUILayout.Button ("Merchant Table: " + spawnInCreation.merchantTable)) {
			propertySelectState = MobPropertySelectState.MerchantTable;
			selectedListScrollPosition = Vector2.zero;
			questListScrollPosition = Vector2.zero;
		}
		
		GUILayout.FlexibleSpace();
		if (spawnInCreation.mobTemplate != null && mobCreationState == MobCreationState.SpawnMob) {
			if (GUILayout.Button ("Spawn Here")) {
				SpawnMobHere ();
			}
		} else if (spawnInCreation.mobTemplate != null && mobCreationState == MobCreationState.EditSpawn) {
			if (GUILayout.Button ("Edit Position")) {
				propertySelectState = MobPropertySelectState.SpawnPositioning;
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button ("Save")) {
				UpdateSpawn();
				StartBuilder(false);
			}
			if (GUILayout.Button ("Cancel")) {
				StartBuilder(false);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space (10);
			if (GUILayout.Button ("Delete")) {
				DeleteSpawn();
				StartBuilder(false);
			}
		} else {
			GUILayout.Space (20);
		}
		GUILayout.EndArea ();
	}
	
	void DrawMobTemplateSelection() 
	{
		Rect templateRect = new Rect (210, Screen.height - 350, 200, 250);
		GUILayout.BeginArea (templateRect, skin.GetStyle ("Window"));
		GUILayout.Label ("Mob Templates:");
		templateListScrollPosition = GUILayout.BeginScrollView (templateListScrollPosition, false, true);
		// Draw Display names
		int pos = 0;
		foreach (MobTemplate tmpl in mobTemplates) {
			if (GUILayout.Button (tmpl.ID + ". " + tmpl.name)) {
				spawnInCreation.mobTemplate = tmpl;
				spawnInCreation.mobTemplateID = tmpl.ID;
				propertySelectState = MobPropertySelectState.None;
				AtavismUiSystem.RemoveFrame ("MobInteraction", new Rect (0, 0, 0, 0));
			}
			pos++;
		}
		GUILayout.EndScrollView ();
		if (GUILayout.Button ("Close")) {
			propertySelectState = MobPropertySelectState.None;
			AtavismUiSystem.RemoveFrame ("MobInteraction", new Rect (0, 0, 0, 0));
		}
		GUILayout.EndArea ();
	}

	void DrawStartsQuest ()
	{
		Rect templateRect = new Rect (210, Screen.height - 350, 200, 300);
		GUILayout.BeginArea (templateRect, skin.GetStyle ("Window"));
		GUILayout.Label ("Starts Quests:");
		selectedListScrollPosition = GUILayout.BeginScrollView (selectedListScrollPosition, false, true);
		// Draw Display names
		int pos = 0;
		foreach (int questID in spawnInCreation.startsQuests) {
			QuestTemplate tmpl = GetQuestTemplate (questID);
			if (GUILayout.Button (tmpl.questID + ". " + tmpl.title)) {
				spawnInCreation.startsQuests.Remove (tmpl.questID);
				break;
			}
			pos++;
		}
		GUILayout.EndScrollView ();

		GUILayout.Label ("Available Quests:");
		questListScrollPosition = GUILayout.BeginScrollView (questListScrollPosition, false, true);
		// Draw Display names
		pos = 0;
		foreach (QuestTemplate tmpl in questTemplates) {
			if (!spawnInCreation.startsQuests.Contains (tmpl.questID)) {
				if (GUILayout.Button (tmpl.questID + ". " + tmpl.title)) {
					spawnInCreation.startsQuests.Add (tmpl.questID);
					break;
				}
				pos++;
			}
		}
		GUILayout.EndScrollView ();

		if (GUILayout.Button ("Close")) {
			propertySelectState = MobPropertySelectState.None;
			AtavismUiSystem.RemoveFrame ("MobInteraction", new Rect (0, 0, 0, 0));
		}
		GUILayout.EndArea ();
	}

	void DrawEndsQuest ()
	{
		
		Rect templateRect = new Rect (210, Screen.height - 350, 200, 300);
		GUILayout.BeginArea (templateRect, skin.GetStyle ("Window"));
		GUILayout.Label ("Ends Quests:");
		selectedListScrollPosition = GUILayout.BeginScrollView (selectedListScrollPosition, false, true);
		// Draw Display names
		int pos = 0;
		foreach (int questID in spawnInCreation.endsQuests) {
			QuestTemplate tmpl = GetQuestTemplate (questID);
			if (GUILayout.Button (tmpl.questID + ". " + tmpl.title)) {
				spawnInCreation.endsQuests.Remove (tmpl.questID);
				break;
			}
			pos++;
		}
		GUILayout.EndScrollView ();
		
		GUILayout.Label ("Available Quests:");
		questListScrollPosition = GUILayout.BeginScrollView (questListScrollPosition, false, true);
		// Draw Display names
		pos = 0;
		foreach (QuestTemplate tmpl in questTemplates) {
			if (!spawnInCreation.endsQuests.Contains (tmpl.questID)) {
				if (GUILayout.Button (tmpl.questID + ". " + tmpl.title)) {
					spawnInCreation.endsQuests.Add (tmpl.questID);
					break;
				}
				pos++;
			}
		}
		GUILayout.EndScrollView ();
		
		if (GUILayout.Button ("Close")) {
			propertySelectState = MobPropertySelectState.None;
			AtavismUiSystem.RemoveFrame ("MobInteraction", new Rect (0, 0, 0, 0));
		}
		GUILayout.EndArea ();
	}
	
	void DrawMerchantTables ()
	{
		Rect templateRect = new Rect (210, Screen.height - 350, 200, 250);
		GUILayout.BeginArea (templateRect, skin.GetStyle ("Window"));
		
		GUILayout.Label ("Merchant Tables:");
		questListScrollPosition = GUILayout.BeginScrollView (questListScrollPosition, false, true);
		// Draw Display names
		foreach (MerchantTableTemplate tmpl in merchantTables) {
			if (GUILayout.Button (tmpl.tableID + ". " + tmpl.title)) {
				spawnInCreation.merchantTable = tmpl.tableID;
				propertySelectState = MobPropertySelectState.None;
				AtavismUiSystem.RemoveFrame ("MobInteraction", new Rect (0, 0, 0, 0));
				break;
			}
		}
		GUILayout.EndScrollView ();
		GUILayout.EndArea ();
	}
	
	void DrawSpawnPositioning()
	{
		Rect templateRect = new Rect (210, Screen.height - 350, 200, 250);
		GUILayout.BeginArea (templateRect, skin.GetStyle ("Window"));
		// Allow manual editing of the position
		GUILayout.Label("Position:");
		Vector3 position = spawnInCreation.marker.transform.position;
		GUILayout.BeginHorizontal();
		GUILayout.Label("x:");
		string posX = GUILayout.TextField(position.x.ToString("0.00"));
		position.x = float.Parse(posX);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("y:");
		string posY = GUILayout.TextField(position.y.ToString("0.00"));
		position.y = float.Parse(posY);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("z:");
		string posZ = GUILayout.TextField(position.z.ToString("0.00"));
		position.z = float.Parse(posZ);
		GUILayout.EndHorizontal();
		spawnInCreation.marker.transform.position = position;
		spawnInCreation.position = position;
		
		if (GUILayout.Button ("Move to Player")) {
			spawnInCreation.marker.transform.position = ClientAPI.GetPlayerObject().Position;
			spawnInCreation.marker.transform.rotation = ClientAPI.GetPlayerObject().Orientation;
			spawnInCreation.position = ClientAPI.GetPlayerObject().Position;
			spawnInCreation.orientation = ClientAPI.GetPlayerObject().Orientation;
		}
		if (GUILayout.Button ("Close")) {
			propertySelectState = MobPropertySelectState.None;
			AtavismUiSystem.RemoveFrame ("MobInteraction", new Rect (0, 0, 0, 0));
		}
		GUILayout.EndArea ();
	}

	void DrawStartsDialogues ()
	{
		
		Rect templateRect = new Rect (210, Screen.height - 350, 200, 300);
		GUILayout.BeginArea (templateRect, skin.GetStyle ("Window"));
		GUILayout.Label ("Starts Dialogues:");
		selectedListScrollPosition = GUILayout.BeginScrollView (selectedListScrollPosition, false, true);
		// Draw Display names
		int pos = 0;
		foreach (int questID in spawnInCreation.startsDialogues) {
			DialogueTemplate tmpl = GetDialogueTemplate (questID);
			if (GUILayout.Button (tmpl.dialogueID + ". " + tmpl.title)) {
				spawnInCreation.startsDialogues.Remove (tmpl.dialogueID);
				break;
			}
			pos++;
		}
		GUILayout.EndScrollView ();
		
		GUILayout.Label ("Available Dialogues:");
		questListScrollPosition = GUILayout.BeginScrollView (questListScrollPosition, false, true);
		// Draw Display names
		pos = 0;
		foreach (DialogueTemplate tmpl in dialogueTemplates) {
			if (!spawnInCreation.startsDialogues.Contains (tmpl.dialogueID)) {
				if (GUILayout.Button (tmpl.dialogueID + ". " + tmpl.title)) {
					spawnInCreation.startsDialogues.Add (tmpl.dialogueID);
					break;
				}
				pos++;
			}
		}
		GUILayout.EndScrollView ();
		
		if (GUILayout.Button ("Close")) {
			propertySelectState = MobPropertySelectState.None;
			AtavismUiSystem.RemoveFrame ("MobInteraction", new Rect (0, 0, 0, 0));
		}
		GUILayout.EndArea ();
	}
	
	void CheckAccess() {
		if (ClientAPI.GetPlayerObject() != null) {
			int adminLevel = (int) ClientAPI.GetObjectProperty(ClientAPI.GetPlayerOid(), "adminLevel");
			if (adminLevel == 5) {
				hasAccess = true;
			} else {
				string currentWorld = (string) ClientAPI.GetObjectProperty(ClientAPI.GetPlayerOid(), "world");
				if (currentWorld != "") {
					Dictionary<string, object> props = new Dictionary<string, object>();
					props.Add("senderOid", ClientAPI.GetPlayerOid());
					props.Add("world", currentWorld);
					NetworkAPI.SendExtensionMessage (0, false, "ao.REQUEST_DEVELOPER_ACCESS", props);
				}
			}
			accessChecked = true;
		}
	}
	
	public void StartBuilder (bool getTemplates)
	{
		if (getTemplates)
			GetMobTemplates ();

		mobCreationState = MobCreationState.Menu;
		propertySelectState = MobPropertySelectState.None;
		AtavismUiSystem.AddFrame ("MobSpawn", new Rect (10, 100, 200, 200));
	}
	
	public void StartMobSpawner ()
	{
		mobCreationState = MobCreationState.SpawnMob;
		spawnInCreation = new MobSpawn ();
		AtavismUiSystem.AddFrame ("MobSpawn", new Rect (10, Screen.height - 400, 200, 350));
	}
	
	public void SpawnSelected (int spawnID)
	{
		if (mobCreationState != MobCreationState.SelectSpawn)
			return;
		Dictionary<string, object> props = new Dictionary<string, object> ();
		props.Add ("senderOid", ClientAPI.GetPlayerOid ());
		props.Add ("markerID", spawnID);
		NetworkAPI.SendExtensionMessage (0, false, "ao.REQUEST_SPAWN_DATA", props);
	}

	QuestTemplate GetQuestTemplate (int questID)
	{
		foreach (QuestTemplate tmpl in questTemplates) {
			if (tmpl.questID == questID)
				return tmpl;
		}
		return null;
	}

	DialogueTemplate GetDialogueTemplate (int dialogueID)
	{
		foreach (DialogueTemplate tmpl in dialogueTemplates) {
			if (tmpl.dialogueID == dialogueID)
				return tmpl;
		}
		return null;
	}
	
	Dictionary<string, object> GetMobSpawnMessageProps (Vector3 position, Quaternion orientation)
	{
		Dictionary<string, object> props = new Dictionary<string, object> ();
		props.Add ("playerOid", ClientAPI.GetPlayerOid ());
		props.Add ("markerID", spawnInCreation.ID);
		props.Add ("loc", position);
		props.Add ("orient", orientation);
		props.Add ("mobTemplate", spawnInCreation.mobTemplateID);
		props.Add ("respawnTime", spawnInCreation.respawnTime);
		props.Add ("despawnTime", spawnInCreation.despawnTime);
		props.Add ("numSpawns", 1);
		props.Add ("spawnRadius", 0);
		props.Add ("roamRadius", (int)spawnInCreation.roamRadius);
		props.Add ("merchantTable", spawnInCreation.merchantTable);
		props.Add ("pickupItem", spawnInCreation.pickupItemID);
		props.Add ("isChest", spawnInCreation.isChest);
		props.Add ("domeID", -1);
		props.Add ("startsQuestsCount", spawnInCreation.startsQuests.Count);
		props.Add ("endsQuestsCount", spawnInCreation.endsQuests.Count);
		for (int i = 0; i < spawnInCreation.startsQuests.Count; i++) {
			props.Add ("startsQuest" + i + "ID", spawnInCreation.startsQuests [i]);
		}
		for (int i = 0; i < spawnInCreation.endsQuests.Count; i++) {
			props.Add ("endsQuest" + i + "ID", spawnInCreation.endsQuests [i]);
		}
		props.Add ("startsDialoguesCount", spawnInCreation.startsDialogues.Count);
		for (int i = 0; i < spawnInCreation.startsDialogues.Count; i++) {
			props.Add ("startsDialogue" + i + "ID", spawnInCreation.startsDialogues [i]);
		}
		return props;
	}
	
	public void SpawnMobHere ()
	{
		Vector3 position = ClientAPI.GetPlayerObject ().Position;
		position.y = Mathf.Ceil (position.y);
		Dictionary<string, object> props = GetMobSpawnMessageProps (position, ClientAPI.GetPlayerObject ().Orientation);
		NetworkAPI.SendExtensionMessage (0, false, "mob.CREATE_MOB_SPAWN", props);
		ClientAPI.Write ("Sending create mob spawn");
	}
	
	public void UpdateSpawn() {
		Dictionary<string, object> props = GetMobSpawnMessageProps(spawnInCreation.position, spawnInCreation.orientation);
		NetworkAPI.SendExtensionMessage(0, false, "ao.EDIT_SPAWN_MARKER", props);
	}
	
	public void DeleteSpawn() {
		Dictionary<string, object> props = new Dictionary<string, object>();
		props.Add("senderOid", ClientAPI.GetPlayerOid());
		props.Add("markerID", spawnInCreation.ID);
		NetworkAPI.SendExtensionMessage(0, false, "ao.DELETE_SPAWN_MARKER", props);
		spawnInCreation.DeleteMarker();
		spawnInCreation = null;
	}
	
	void ClearSpawns() {
		if (spawnInCreation != null)
			spawnInCreation.DeleteMarker();
		foreach (MobSpawn spawn in mobSpawns.Values) {
			spawn.DeleteMarker();
		}
		mobSpawns.Clear ();
	}
	
	public MobTemplate GetMobTemplateByID (int id)
	{
		MobTemplate template = null;
		foreach (MobTemplate tmpl in mobTemplates) {
			if (tmpl.ID == id)
				return tmpl;
		}
		return template;
	}
	
	public void SaveMobTemplate ()
	{
		Dictionary<string, object> props = new Dictionary<string, object> ();
		props.Add ("playerOid", ClientAPI.GetPlayerOid ());
		props.Add ("templateID", mobInCreation.ID);
		props.Add ("name", mobInCreation.name);
		props.Add ("subtitle", mobInCreation.subtitle);
		props.Add ("species", mobInCreation.species);
		props.Add ("subspecies", mobInCreation.subspecies);
		props.Add ("gender", mobInCreation.gender);
		props.Add ("appearanceType", mobInCreation.displayType);
		props.Add ("scale", mobInCreation.scale);
		props.Add ("level", mobInCreation.level);
		props.Add ("attackable", mobInCreation.attackable);
		props.Add ("mobType", mobInCreation.mobType);
		props.Add ("faction", mobInCreation.faction);
		props.Add ("equipCount", 0);
		props.Add ("lootTableCount", 0);
		props.Add ("displayCount", mobInCreation.displays.Count);
		for (int i = 0; i < mobInCreation.displays.Count; i++) {
			props.Add ("display" + i, mobInCreation.displays [i]);
		}
		/*for i in range(0, len(mobInCreation.equippedItems)):
        	itemID = mobInCreation.equippedItems[i].templateID
        	props["equip" + str(i) + "ID"] = itemID
    	for i in range(0, len(mobInCreation.lootTables)):
        	lootTable = mobInCreation.lootTables[i]
        	props["lootTable" + str(i) + "ID"] = lootTable.itemID
        	props["lootTable" + str(i) + "Chance"] = lootTable.dropChance*/
		NetworkAPI.SendExtensionMessage (0, false, "ao.CREATE_MOB", props);
		ClientAPI.Write ("Sent save mob template message");
	}
	
	public void GetMobTemplates ()
	{
		// get mob templates
		Dictionary<string, object> props = new Dictionary<string, object> ();
		props.Add ("senderOid", ClientAPI.GetPlayerOid ());
		props.Add ("type", "mob");
		NetworkAPI.SendExtensionMessage (0, false, "mob.GET_TEMPLATES", props);
		// Get Quest and Dialogue Templates
		props = new Dictionary<string, object> ();
		props.Add ("senderOid", ClientAPI.GetPlayerOid ());
		props.Add ("type", "quests");
		NetworkAPI.SendExtensionMessage (0, false, "mob.GET_TEMPLATES", props);
		props = new Dictionary<string, object> ();
		props.Add ("senderOid", ClientAPI.GetPlayerOid ());
		props.Add ("type", "dialogues");
		NetworkAPI.SendExtensionMessage (0, false, "mob.GET_TEMPLATES", props);
		// Get Merchant Tables
		props = new Dictionary<string, object> ();
		props.Add ("senderOid", ClientAPI.GetPlayerOid ());
		props.Add ("type", "merchantTables");
		NetworkAPI.SendExtensionMessage (0, false, "mob.GET_TEMPLATES", props);
		// Get spawn markers
		props = new Dictionary<string, object> ();
		props.Add ("senderOid", ClientAPI.GetPlayerOid ());
		NetworkAPI.SendExtensionMessage (0, false, "ao.VIEW_MARKERS", props);
	}
	
	#region Message Handlers
	
	public void WorldDeveloperHandler (Dictionary<string, object> props)
	{
		bool isAdmin = (bool) props["isAdmin"];
		bool isDeveloper = (bool) props["isDeveloper"];
		if (isAdmin || isDeveloper) {
			hasAccess = true;
		} else {
			hasAccess = false;
		}
	}
	
	public void HandleMobTemplateUpdate (Dictionary<string, object> props)
	{
		mobTemplates.Clear ();
		int numTemplates = (int)props ["numTemplates"];
		for (int i = 0; i < numTemplates; i++) {
			MobTemplate template = new MobTemplate ();
			template.name = (string)props ["mob_" + i + "Name"];
			template.ID = (int)props ["mob_" + i + "ID"];
			template.subtitle = (string)props ["mob_" + i + "SubTitle"];
			template.species = (string)props ["mob_" + i + "Species"];
			template.subspecies = (string)props ["mob_" + i + "Subspecies"];
			template.level = (int)props ["mob_" + i + "Level"];
			template.attackable = (bool)props ["mob_" + i + "Attackable"];
			template.faction = (int)props ["mob_" + i + "Faction"];
			template.mobType = (int)props ["mob_" + i + "MobType"];
			//template.gender = (string)props["mob_" + i + "Gender"];
			template.scale = (float)props ["mob_" + i + "Scale"];
			List<string> displays = new List<string> ();
			int numDisplays = (int)props ["mob_" + i + "NumDisplays"];
			for (int j = 0; j < numDisplays; j++) {
				string display = (string)props ["mob_" + i + "Display" + j];
				displays.Add (display);
			}
			template.displays = displays;
			/*for itemID in props["mob_%dEquipment" % i]:
            	template.equippedItems.append(GetItemTemplateByID(itemID))
        	numLootTables = int(props["mob_%dNumLootTables" % i])
        	for j in range(0, numLootTables):
            	lootTableID = int(props["mob_%dLootTable%d" % (i, j)])
            	lootTableChance = int(props["mob_%dLootTable%dChance" % (i, j)])
            	lootTable = LootTableDropEntry()
            	lootTable.itemID = lootTableID
            	lootTable.dropChance = lootTableChance
            	template.lootTables.append(lootTable)
            	//ClientAPI.Write("Loot table %s for mob %s has chance: %s" % (lootTableID, template.name, lootTableChance))*/
			mobTemplates.Add (template);
		}
		ClientAPI.Write ("Number of mob templates added: " + mobTemplates.Count);
		//mobCreationState = MobCreationState.SelectTemplate;
		//selectedTemplate = -1;
	}

	public void HandleQuestTemplateUpdate (Dictionary<string, object> props)
	{
		questTemplates.Clear ();
		int numTemplates = (int)props ["numTemplates"];
		for (int i = 0; i < numTemplates; i++) {
			QuestTemplate template = new QuestTemplate ();
			template.title = (string)props ["quest_" + i + "Title"];
			template.questID = (int)props ["quest_" + i + "Id"];
			questTemplates.Add (template);
		}
		ClientAPI.Write ("Number of quest templates added: " + questTemplates.Count);
	}

	public void HandleDialogueTemplateUpdate (Dictionary<string, object> props)
	{
		dialogueTemplates.Clear ();
		int numTemplates = (int)props ["numTemplates"];
		for (int i = 0; i < numTemplates; i++) {
			DialogueTemplate template = new DialogueTemplate ();
			template.title = (string)props ["dialogue_" + i + "Title"];
			template.dialogueID = (int)props ["dialogue_" + i + "Id"];
			dialogueTemplates.Add (template);
		}
		ClientAPI.Write ("Number of dialogue templates added: " + dialogueTemplates.Count);
	}
	
	public void HandleMerchantTableUpdate (Dictionary<string, object> props)
	{
		merchantTables.Clear ();
		int numTemplates = (int)props ["numTemplates"];
		for (int i = 0; i < numTemplates; i++) {
			MerchantTableTemplate template = new MerchantTableTemplate ();
			template.title = (string)props ["merchant_" + i + "Title"];
			template.tableID = (int)props ["merchant_" + i + "Id"];
			merchantTables.Add (template);
		}
		ClientAPI.Write ("Number of merchant tables added: " + merchantTables.Count);
	}
	
	public void HandleSpawnList (Dictionary<string, object> props)
	{
		AtavismLogger.LogDebugMessage ("Got spawn list");
		ClearSpawns();
		int numMarkers = (int)props ["numMarkers"];
		for (int i  = 0; i < numMarkers; i++) {
			MobSpawn spawn = new MobSpawn ();
			spawn.ID = (int)props ["markerID_" + i];
			spawn.position = (Vector3)props ["markerLoc_" + i];
			spawn.orientation = (Quaternion)props ["markerOrient_" + i];
			spawn.CreateMarkerObject (spawnMarkerTemplate);
			mobSpawns.Add (spawn.ID, spawn);
			AtavismLogger.LogDebugMessage ("Added spawn: " + spawn.ID);
		}
	}
	
	public void HandleSpawnData (Dictionary<string, object> props)
	{
		int spawnID = (int)props ["spawnID"];
		spawnInCreation = mobSpawns[spawnID];
		//spawnInCreation.ID = 
		spawnInCreation.numSpawns = (int)props ["numSpawns"];
		spawnInCreation.despawnTime = (int)props ["despawnTime"];
		spawnInCreation.respawnTime = (int)props ["respawnTime"];
		spawnInCreation.spawnRadius = (int)props ["spawnRadius"];
		spawnInCreation.mobTemplateID = (int)props ["mobTemplate"];
		spawnInCreation.mobTemplate = GetMobTemplateByID(spawnInCreation.mobTemplateID);
		spawnInCreation.roamRadius = (int)props ["roamRadius"];
		//spawnInCreation.hasCombat = (bool)props["hasCombat"];
		//spawnInCreation.startsQuests = (List<object>)props["startsQuests"];
		LinkedList<object> questList = (LinkedList<object>)props ["startsQuests"];
		foreach (object quest in questList) {
			spawnInCreation.startsQuests.Add ((int)quest);
		}
		//spawnInCreation.endsQuests = (List<object>)props["endsQuests"];
		questList = (LinkedList<object>)props ["endsQuests"];
		foreach (object quest in questList) {
			spawnInCreation.endsQuests.Add ((int)quest);
		}
		LinkedList<object> dialogueList = (LinkedList<object>)props ["startsDialogues"];
		foreach (object dialogue in dialogueList) {
			spawnInCreation.startsDialogues.Add ((int)dialogue);
		}
		spawnInCreation.pickupItemID = (int)props ["pickupItem"];
		spawnInCreation.isChest = (bool)props ["isChest"];
		mobCreationState = MobCreationState.EditSpawn;
	}
	
	#endregion Message Handlers

	public void ToggleBuildingModeEnabled ()
	{
		if (!accessChecked) {
			CheckAccess();
		}
		if (mobCreationState == MobCreationState.Disabled && hasAccess) {
			mobCreationState = MobCreationState.None;
		} else {
			mobCreationState = MobCreationState.Disabled;
			ClearSpawns();
		}
	}
}
