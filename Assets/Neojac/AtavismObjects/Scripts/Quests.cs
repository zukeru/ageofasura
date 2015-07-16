using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestLogEntry
{
	public OID QuestId = null;
	public OID NpcId;
	public string Title = "";
	public string Description = "";
	public string Objective = "";
	public int gradeCount = 0;
	public string ProgressText = "";
	public List<QuestGradeEntry> gradeInfo;
	public bool Complete = false;
	public int GradeReached = 0;
	public int itemChosen = -1;
}

public class QuestGradeEntry
{
	public string completionText;
	public List<string> objectives;
	public List<QuestRewardEntry> rewardItems;
	public List<QuestRewardEntry> RewardItemsToChoose;
	public int expReward;
	public int currencyReward;
}

public class QuestRewardEntry
{
	public string name = "";
	public int id = -1;
	public Texture2D icon;
	public int count = 0;
}

public class Quests : MonoBehaviour
{
	
	static Quests instance;
	
	// info about the last quests we were offered
	int questsOfferedSelectedIndex = 0;
	List<QuestLogEntry> questsOffered = new List<QuestLogEntry> ();

	// quest log info
	int questLogSelectedIndex = 0;
	List<QuestLogEntry> questLogEntries = new List<QuestLogEntry> ();
	
	// quest progress info
	int questProgressSelectedIndex = 0;
	List<QuestLogEntry> questsInProgress = new List<QuestLogEntry> ();
	OID npcID;

	// Use this for initialization
	void Start ()
	{
		if (instance != null) {
			return;
		}
		instance = this;
		
		NetworkAPI.RegisterExtensionMessageHandler ("ao.QUEST_OFFER", _HandleQuestOfferResponse);
		NetworkAPI.RegisterExtensionMessageHandler ("ao.QUEST_LOG_INFO", _HandleQuestLogInfo);
		NetworkAPI.RegisterExtensionMessageHandler ("ao.QUEST_STATE_INFO", _HandleQuestStateInfo);
		NetworkAPI.RegisterExtensionMessageHandler ("ao.QUEST_PROGRESS", _HandleQuestProgressInfo);
		NetworkAPI.RegisterExtensionMessageHandler ("ao.REMOVE_QUEST_RESP", _HandleRemoveQuestResponse);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public QuestLogEntry GetQuestOfferedInfo (int pos)
	{
		return questsOffered [pos];
	}

	public void AcceptQuest (int questPos)
	{
		NetworkAPI.SendQuestResponseMessage (npcID.ToLong (), questsOffered [questPos].QuestId.ToLong (), true);
	}

	public void DeclineQuest (int questPos)
	{
		NetworkAPI.SendQuestResponseMessage (npcID.ToLong (), questsOffered [questPos].QuestId.ToLong (), false);
	}

	public void ViewQuestLogEntry (int pos)
	{
		questLogSelectedIndex = pos;
	}

	public QuestLogEntry GetSelectedQuestLogEntry ()
	{
		if (questLogEntries.Count - 1 < questLogSelectedIndex)
			return null;
		return questLogEntries [questLogSelectedIndex];
	}

	public void AbandonQuest ()
	{
		if (questLogSelectedIndex == -1 || questLogSelectedIndex > questLogEntries.Count)
			return;
		NetworkAPI.SendTargetedCommand (ClientAPI.GetPlayerOid (), "/abandonQuest " + questLogEntries [questLogSelectedIndex].QuestId);
	}

	public QuestLogEntry GetQuestProgressInfo (int pos)
	{
		return questsInProgress [pos];
	}

	public void CompleteQuest ()
	{
		QuestLogEntry quest = questsInProgress [0];
		/*Dictionary<string, object> props = new Dictionary<string, object>();
		props.Add("senderOid", ClientAPI.GetPlayerOid());
		props.Add("questNPC", quest.NpcId);
		props.Add("questOID", quest.QuestId);
		props.Add("reward", rewardID);
		NetworkAPI.SendExtensionMessage(quest.NpcId.ToLong(), false, "ao.COMPLETE_QUEST", props);*/

		NetworkAPI.SendTargetedCommand (quest.NpcId.ToLong (), "/completeQuest " + quest.QuestId + " " + quest.itemChosen);
	}
	
	void _HandleQuestOfferResponse (Dictionary<string, object> props)
	{
		// update the information about the quests on offer from this npc
		questsOffered.Clear ();
		int numQuests = (int)props ["numQuests"];
		npcID = (OID)props ["npcID"];
		for (int i = 0; i < numQuests; i++) {
			QuestLogEntry logEntry = new QuestLogEntry ();
			questsOffered.Add (logEntry);
			logEntry.Title = (string)props ["title" + i];
			logEntry.QuestId = (OID)props ["questID" + i];
			logEntry.NpcId = npcID;
			logEntry.Description = (string)props ["description" + i];
			logEntry.Objective = (string)props ["objective" + i];
			//logEntry.Objectives.Clear ();
			//LinkedList<string> objectives = (LinkedList<string>)props ["objectives"];
			//foreach (string objective in objectives)
			//	logEntry.Objectives.Add (objective);
			logEntry.gradeCount = (int)props ["grades" + i];
			logEntry.gradeInfo = new List<QuestGradeEntry> ();
			//ClientAPI.Write("Quest grades: %s" % logEntry.grades)
			for (int j = 0; j < (logEntry.gradeCount + 1); j++) {
				QuestGradeEntry gradeEntry = new QuestGradeEntry ();
				List<QuestRewardEntry> gradeRewards = new List<QuestRewardEntry> ();
				int numRewards = (int)props ["rewards" + i + " " + j];
				for (int k = 0; k < numRewards; k++) {
					//id, name, icon, count = item;
					QuestRewardEntry entry = new QuestRewardEntry ();
					entry.id = (int)props ["rewards" + i + "_" + j + "_" + k];
					AtavismInventoryItem item = gameObject.GetComponent<Inventory> ().GetItemByTemplateID (entry.id);
					if (item != null) {
						entry.name = item.name;
						entry.icon = item.icon;
					}
					entry.count = (int)props ["rewards" + i + "_" + j + "_" + k + "Count"];
					gradeRewards.Add (entry);
					//ClientAPI.Write("Reward: %s" % entry)
				}
				gradeEntry.rewardItems = gradeRewards;
				// Items to choose from
				List<QuestRewardEntry> gradeRewardsToChoose = new List<QuestRewardEntry> ();
				numRewards = (int)props ["rewardsToChoose" + i + " " + j];
				for (int k = 0; k < numRewards; k++) {
					//id, name, icon, count = item;
					QuestRewardEntry entry = new QuestRewardEntry ();
					entry.id = (int)props ["rewardsToChoose" + i + "_" + j + "_" + k];
					AtavismInventoryItem item = gameObject.GetComponent<Inventory> ().GetItemByTemplateID (entry.id);
					if (item != null) {
						entry.name = item.name;
						entry.icon = item.icon;
					}
					entry.count = (int)props ["rewardsToChoose" + i + "_" + j + "_" + k + "Count"];
					gradeRewardsToChoose.Add (entry);
					//ClientAPI.Write("Reward to choose: %s" % entry)
				}
				gradeEntry.RewardItemsToChoose = gradeRewardsToChoose;
				logEntry.gradeInfo.Add (gradeEntry);
			}
		}
		// dispatch a ui event to tell the rest of the system
		gameObject.GetComponent<NpcInteraction> ().NpcId = npcID;
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent ("QUEST_OFFERED_UPDATE", args);
	}
       
	/// <summary>
	/// Handles the Quest Log Update message, which has information about the current status 
	/// of Quests that the player is on.
	/// </summary>
	/// <param name="props">Properties.</param>
	void _HandleQuestLogInfo (Dictionary<string, object> props)
	{
		// update our idea of the state
		QuestLogEntry logEntry = null;
		long quest_id = (long)props ["ext_msg_subject_oid"];
		OID questID = OID.fromLong (quest_id);
		foreach (QuestLogEntry entry in questLogEntries) {
			if (entry.QuestId.Equals (questID)) {
				logEntry = entry;
				break;
			}
		}
		if (logEntry == null) {
			logEntry = new QuestLogEntry ();
			questLogEntries.Add (logEntry);
		}
		logEntry.QuestId = questID;
		logEntry.Title = (string)props ["title"];
		logEntry.Description = (string)props ["description"];
		logEntry.Objective = (string)props ["objective"];
		logEntry.gradeCount = (int)props ["grades"];
		logEntry.gradeInfo = new List<QuestGradeEntry> ();
		for (int j = 0; j < (logEntry.gradeCount + 1); j++) {
			QuestGradeEntry gradeEntry = new QuestGradeEntry ();
			// Objectives
			List<string> objectives = new List<string> ();
			int numObjectives = (int)props ["numObjectives" + j];
			for (int k = 0; k < numObjectives; k++) {
				string objective = (string)props ["objective" + j + "_" + k];
				objectives.Add (objective);
			}
			gradeEntry.objectives = objectives;
			
			// Rewards
			List<QuestRewardEntry> gradeRewards = new List<QuestRewardEntry> ();
			int numRewards = (int)props ["rewards" + j];
			for (int k = 0; k < numRewards; k++) {
				//id, name, icon, count = item;
				QuestRewardEntry entry = new QuestRewardEntry ();
				entry.id = (int)props ["rewards" + j + "_" + k];
				AtavismInventoryItem item = gameObject.GetComponent<Inventory> ().GetItemByTemplateID (entry.id);
				if (item != null) {
					entry.name = item.name;
					entry.icon = item.icon;
				}
				entry.count = (int)props ["rewards" + j + "_" + k + "Count"];
				gradeRewards.Add (entry);
				//ClientAPI.Write("Reward: %s" % entry)
			}
			gradeEntry.rewardItems = gradeRewards;
			// Items to choose from
			List<QuestRewardEntry> gradeRewardsToChoose = new List<QuestRewardEntry> ();
			numRewards = (int)props ["rewardsToChoose" + j];
			for (int k = 0; k < numRewards; k++) {
				//id, name, icon, count = item;
				QuestRewardEntry entry = new QuestRewardEntry ();
				entry.id = (int)props ["rewardsToChoose" + j + "_" + k];
				AtavismInventoryItem item = gameObject.GetComponent<Inventory> ().GetItemByTemplateID (entry.id);
				if (item != null) {
					entry.name = item.name;
					entry.icon = item.icon;
				}
				entry.count = (int)props ["rewardsToChoose" + j + "_" + k + "Count"];
				gradeRewardsToChoose.Add (entry);
				//ClientAPI.Write("Reward: %s" % entry)
			}
			gradeEntry.RewardItemsToChoose = gradeRewardsToChoose;
			gradeEntry.expReward = (int)props ["xpReward" + j];
			logEntry.gradeInfo.Add (gradeEntry);
		}
		// dispatch a ui event to tell the rest of the system
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent ("QUEST_LOG_UPDATE", args);
	}

	/// <summary>
	/// Handles the updates of the Quest State and updates the objectives in the players Quest Log
	/// to match.
	/// </summary>
	/// <param name="props">Properties.</param>
	void _HandleQuestStateInfo (Dictionary<string, object> props)
	{
		long quest_id = (long)props ["ext_msg_subject_oid"];
		OID questID = OID.fromLong (quest_id);
		// update our idea of the state
		foreach (QuestLogEntry entry in questLogEntries) {
			if (!entry.QuestId.Equals (questID))
				continue;
			for (int j = 0; j < (entry.gradeCount + 1); j++) {	
				// Objectives
				List<string> objectives = new List<string> ();
				int numObjectives = (int)props ["numObjectives" + j];
				for (int k = 0; k < numObjectives; k++) {
					string objective = (string)props ["objective" + j + "_" + k];
					objectives.Add (objective);
				}
				entry.gradeInfo[j].objectives = objectives;
			}
		}
    
		// dispatch a ui event to tell the rest of the system
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent ("QUEST_LOG_UPDATE", args);
	}
	
	void _HandleQuestProgressInfo (Dictionary<string, object> props)
	{
		/// update the information about the quests in progress from this npc
		questsInProgress.Clear ();
		int numQuests = (int)props ["numQuests"];
		npcID = (OID)props ["npcID"];
		for (int i = 0; i < numQuests; i++) {
			QuestLogEntry logEntry = new QuestLogEntry ();
			questsInProgress.Add (logEntry);
			logEntry.Title = (string)props ["title" + i];
			logEntry.QuestId = (OID)props ["questID" + i];
			logEntry.NpcId = npcID;
			//logEntry.Description = (string)props ["description" + i];
			logEntry.ProgressText = (string)props ["progress" + i];
			logEntry.Complete = (bool)props ["complete" + i];
			logEntry.Objective = (string)props ["objective" + i];
			logEntry.gradeCount = (int)props ["grades" + i];
			logEntry.gradeInfo = new List<QuestGradeEntry> ();
			//ClientAPI.Write("Quest grades: %s" % logEntry.grades)
			for (int j = 0; j < (logEntry.gradeCount + 1); j++) {
				QuestGradeEntry gradeEntry = new QuestGradeEntry ();
				List<QuestRewardEntry> gradeRewards = new List<QuestRewardEntry> ();
				int numRewards = (int)props ["rewards" + i + " " + j];
				for (int k = 0; k < numRewards; k++) {
					//id, name, icon, count = item;
					QuestRewardEntry entry = new QuestRewardEntry ();
					entry.id = (int)props ["rewards" + i + "_" + j + "_" + k];
					AtavismInventoryItem item = gameObject.GetComponent<Inventory> ().GetItemByTemplateID (entry.id);
					entry.name = item.name;
					entry.icon = item.icon;
					entry.count = (int)props ["rewards" + i + "_" + j + "_" + k + "Count"];
					gradeRewards.Add (entry);
					//ClientAPI.Write("Reward: %s" % entry)
				}
				gradeEntry.rewardItems = gradeRewards;
				// Items to choose from
				List<QuestRewardEntry> gradeRewardsToChoose = new List<QuestRewardEntry> ();
				numRewards = (int)props ["rewardsToChoose" + i + " " + j];
				for (int k = 0; k < numRewards; k++) {
					//id, name, icon, count = item;
					QuestRewardEntry entry = new QuestRewardEntry ();
					entry.id = (int)props ["rewardsToChoose" + i + "_" + j + "_" + k];
					AtavismInventoryItem item = gameObject.GetComponent<Inventory> ().GetItemByTemplateID (entry.id);
					entry.name = item.name;
					entry.icon = item.icon;
					entry.count = (int)props ["rewardsToChoose" + i + "_" + j + "_" + k + "Count"];
					gradeRewardsToChoose.Add (entry);
					//ClientAPI.Write("Reward to choose: %s" % entry)
				}
				gradeEntry.RewardItemsToChoose = gradeRewardsToChoose;
				gradeEntry.completionText = (string)props ["completion" + i + "_" + j];
				logEntry.gradeInfo.Add (gradeEntry);
			}
		}
		//
		// dispatch a ui event to tell the rest of the system
		//
		gameObject.GetComponent<NpcInteraction> ().NpcId = npcID;
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent ("QUEST_PROGRESS_UPDATE", args);
	}

	void _HandleRemoveQuestResponse (Dictionary<string, object> props)
	{
		int index = 1; // questLogSelectedIndex is 1 based.
		long quest_id = (long)props ["ext_msg_subject_oid"];
		OID questID = OID.fromLong (quest_id);
		foreach (QuestLogEntry entry in questLogEntries) {
			if (entry.QuestId.Equals (questID)) {
				questLogEntries.Remove (entry);
				break;
			}
			index++;
		}
		if (index == questLogSelectedIndex) {
			// we removed the selected entry. reset selection
			questLogSelectedIndex = 0;
		} else if (index < questLogSelectedIndex) {
			// removed an entry before our selection - decrement our selection
			questLogSelectedIndex--;
		}
    
		// dispatch a ui event to tell the rest of the system
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent ("QUEST_LOG_UPDATE", args);
	}

	public List<QuestLogEntry> QuestLogEntries {
		get {
			return questLogEntries;
		}
	}
}
