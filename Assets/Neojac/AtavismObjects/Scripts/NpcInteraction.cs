using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcInteractionEntry
{
	public string interactionType;
	public string interactionTitle;
	public int interactionID;
	public OID npcId;
	
	public void StartInteraction() {
		NetworkAPI.SendTargetedCommand (npcId.ToLong(), "/startInteraction " + interactionID + " " 
			+ interactionType);
	}
}

public class Dialogue
{
	public int dialogueID = 0;
	public string title = "";
	public string text = "";
	public List<DialogueAction> actions = new List<DialogueAction>();
	public OID npcId;
	
	public void PerformDialogueAction(DialogueAction action) {
		action.DoDialogueAction(dialogueID, npcId);
	}
}


public class DialogueAction 
{
	public string actionText;
	public string actionType;
	public int actionID;

	public void DoDialogueAction(int dialogueID, OID npcId) {
		NetworkAPI.SendTargetedCommand (npcId.ToLong(), "/dialogueOption " + dialogueID + " " 
		                                + actionType + " " + actionID);
	}
}

public class MerchantItem
{
	public int itemID;
	public int count;
	public int cost;
	public int purchaseCurrency;
}

public class NpcInteraction : MonoBehaviour
{
	static NpcInteraction instance;
	public OID NpcId;

	// info about the last quests we were offered
	int interactionOptionSelected = 0;
	List<NpcInteractionEntry> interactionOptions = new List<NpcInteractionEntry> ();
	List<MerchantItem> merchantItems = new List<MerchantItem>();

	Dialogue dialogue;

	// Use this for initialization
	void Start ()
	{
		if (instance != null) {
			return;
		}
		instance = this;
		
		NetworkAPI.RegisterExtensionMessageHandler ("npc_interactions", _HandleNpcInteractionOptionsResponse);
		NetworkAPI.RegisterExtensionMessageHandler ("npc_dialogue", _HandleNpcDialogue);
		NetworkAPI.RegisterExtensionMessageHandler ("MerchantList", _HandleMerchantList);
		NetworkAPI.RegisterExtensionMessageHandler ("item_purchase_result", HandlePurchaseResponse);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void GetInteractionOptionsForNpc(long npcOid) {
		NetworkAPI.SendTargetedCommand (npcOid, "/getNpcInteractions");
	}
	
	void _HandleNpcInteractionOptionsResponse (Dictionary<string, object> props)
	{
		// update the information about the interaction options on offer from this npc
		interactionOptions.Clear ();
		int numInteractions = (int)props ["numInteractions"];
		NpcId = (OID)props["npcOid"];
		for (int i = 0; i < numInteractions; i++) {
			NpcInteractionEntry entry = new NpcInteractionEntry ();
			entry.interactionType = (string)props ["interactionType_" + i];
			entry.interactionTitle = (string)props ["interactionTitle_" + i];
			entry.interactionID = (int)props ["interactionID_" + i];
			entry.npcId = NpcId;
			interactionOptions.Add (entry);
			//ClientAPI.Write("Quest grades: %s" % logEntry.grades)
		}
		
		string dialogueText = (string)props ["dialogue_text"];
		if (dialogueText != "") {
			dialogue = new Dialogue(); 
			dialogue.text = dialogueText;
		} else {
			dialogue = null;
		}
		//
		// dispatch a ui event to tell the rest of the system
		//
		string[] args = new string[1];
		// args [0] = npcID.ToString ();
		AtavismEventSystem.DispatchEvent ("NPC_INTERACTIONS_UPDATE", args);
	}
       
	void _HandleNpcDialogue (Dictionary<string, object> props)
	{
		// update our idea of the state
		//QuestLogEntry logEntry = null;
		NpcId = (OID)props["npcOid"];
		dialogue = new Dialogue();
		dialogue.dialogueID = (int)props ["dialogueID"];
		dialogue.title = (string)props ["title"];
		dialogue.text = (string)props ["text"];
		dialogue.npcId = NpcId;
		
		int numOptions = (int)props["numOptions"];
		for (int i = 0; i < numOptions; i++) {
			DialogueAction action = new DialogueAction();
			action.actionType = (string)props ["option" + i + "action"];
			action.actionID = (int)props ["option" + i + "actionID"];
			action.actionText = (string)props ["option" + i + "text"];
			dialogue.actions.Add(action);
		}
		
		// dispatch a ui event to tell the rest of the system
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent ("DIALOGUE_UPDATE", args);
	}
	
	void _HandleMerchantList (Dictionary<string, object> props)
	{
		merchantItems.Clear();
		NpcId = (OID)props["npcOid"];
		
		int numItems = (int)props["numItems"];
		for (int i = 0; i < numItems; i++) {
			MerchantItem merchantItem = new MerchantItem();
			merchantItem.itemID = (int)props ["item_" + i + "ID"];
			merchantItem.count = (int)props ["item_" + i + "Count"];
			merchantItem.cost = (int)props ["item_" + i + "Cost"];
			merchantItem.purchaseCurrency = (int)props ["item_" + i + "Currency"];
			merchantItems.Add(merchantItem);
		}
		
		// dispatch a ui event to tell the rest of the system
		string[] args = new string[1];
		AtavismEventSystem.DispatchEvent ("MERCHANT_UPDATE", args);
	}
	
	public void HandlePurchaseResponse(Dictionary<string, object> props) {
		string result = (string)props["result"];
		if (result == "insufficient_funds") {
			// dispatch a ui event to tell the rest of the system
			string[] args = new string[1];
			string currencyName = (string)props["currency"];
			args[0] = "You do not have enough " + currencyName + " to purchase that item";
			AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
		} else if (result == "insufficient_space") {
			// dispatch a ui event to tell the rest of the system
			string[] args = new string[1];
			args[0] = "You do not have any space in your bags to purchase that item";
			AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
		} else if (result == "no_item") {
			// dispatch a ui event to tell the rest of the system
			string[] args = new string[1];
			args[0] = "The merchant does not have that item available for purchase";
			AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
		}
	}
	
	public MerchantItem GetMerchantItem(int itemPos) {
		if (merchantItems.Count > itemPos) {
			return merchantItems[itemPos];
		}
		return null;
	}
	
	public List<NpcInteractionEntry> InteractionOptions {
		get {
			return interactionOptions;
		}
	}

	public Dialogue Dialogue {
		get {
			return dialogue;
		}
	}
	
	public List<MerchantItem> MerchantItems {
		get {
			return merchantItems;
		}
	}
}
