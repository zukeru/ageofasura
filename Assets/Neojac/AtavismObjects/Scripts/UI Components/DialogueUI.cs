using UnityEngine;
using System.Collections;

public enum DialogueType {
	Chat,
	Options,
	QuestOffer,
	QuestProgress,
	QuestConclusion
}

public class DialogueUI : AtavismWindowTemplate {

	DialogueType dialogueType;
	int dialogueID;

	Vector2 scrollPos = new Vector2();

	// Use this for initialization
	void Start () {
		SetupRect();
		
		// Register for 
		AtavismEventSystem.RegisterEvent("NPC_INTERACTIONS_UPDATE", this);
		AtavismEventSystem.RegisterEvent("DIALOGUE_UPDATE", this);
		AtavismEventSystem.RegisterEvent("QUEST_OFFERED_UPDATE", this);
		AtavismEventSystem.RegisterEvent("QUEST_PROGRESS_UPDATE", this);
	}
	
	// Update is called once per frame
	void Update () {
		if (open) {
			OID npcOID = ClientAPI.ScriptObject.GetComponent<NpcInteraction>().NpcId;
			if (npcOID != null) {
				if (Vector3.Distance(ClientAPI.GetPlayerObject().Position, ClientAPI.GetObjectNode(npcOID.ToLong()).Position) > 5) {
					ToggleOpen();
				}
			}
		}
	}
	
	void OnDestroy () {
		AtavismEventSystem.UnregisterEvent("NPC_INTERACTIONS_UPDATE", this);
		AtavismEventSystem.UnregisterEvent("DIALOGUE_UPDATE", this);
		AtavismEventSystem.UnregisterEvent("QUEST_OFFERED_UPDATE", this);
		AtavismEventSystem.UnregisterEvent("QUEST_PROGRESS_UPDATE", this);
	}
	
	void OnGUI() {
		if (!open)
			return;
			
		GUI.depth = uiLayer;
		GUI.skin = skin;

		GUI.Box(uiRect, "");
		GUILayout.BeginArea(new Rect(uiRect));
		// NPC Name
		OID npcOID = ClientAPI.ScriptObject.GetComponent<NpcInteraction>().NpcId;
		GUILayout.BeginHorizontal();
		GUILayout.Label(ClientAPI.GetObjectNode(npcOID.ToLong()).Name);
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("X")) {
			ToggleOpen();
		}
		GUILayout.EndHorizontal();
		// Scroll view
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		// Call functions based on the current state
		if (dialogueType == DialogueType.Chat) {
			DrawChat();
		} else if (dialogueType == DialogueType.Options) {
			DrawOptions();
		} else if (dialogueType == DialogueType.QuestOffer) {
			DrawQuestOffer();
		} else if (dialogueType == DialogueType.QuestProgress) {
			DrawQuestProgress();
		} else if (dialogueType == DialogueType.QuestConclusion) {
			DrawQuestConclusion();
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	void DrawChat() {
		Dialogue d = ClientAPI.ScriptObject.GetComponent<NpcInteraction>().Dialogue;
		GUILayout.TextArea(d.text);
		foreach (DialogueAction action in d.actions) {
			if (GUILayout.Button(action.actionText)) {
				d.PerformDialogueAction(action);
			}
		}
	}

	void DrawOptions() {
		Dialogue d = ClientAPI.ScriptObject.GetComponent<NpcInteraction>().Dialogue;
		if (d != null) {
			GUILayout.TextArea(d.text);
		}
		
		foreach (NpcInteractionEntry entry in ClientAPI.ScriptObject.GetComponent<NpcInteraction>().InteractionOptions) {
			string buttonText = entry.interactionTitle;
			if (entry.interactionType == "offered_quest") {
				buttonText = "! " + buttonText;
			} else if (entry.interactionType == "progress_quest") {
				buttonText = "? " + buttonText;
			}
			
			if (GUILayout.Button(buttonText)) {
				entry.StartInteraction();
			}
		}
	}

	void DrawQuestOffer() {
		QuestLogEntry q = ClientAPI.ScriptObject.GetComponent<Quests>().GetQuestOfferedInfo(0);
		GUILayout.Label("New Quest:" + q.Title);
		GUILayout.TextArea(q.Description);
		GUILayout.Label("Objective");
		GUILayout.TextArea(q.Objective);
		GUILayout.Label("Rewards");
		// TODO: Rewards
		if (q.gradeInfo[0].rewardItems.Count > 0)
			GUILayout.Label("You will receive the following items:");
		foreach (QuestRewardEntry reward in q.gradeInfo[0].rewardItems) {
			GUILayout.BeginHorizontal();
			GUILayout.Button(reward.icon, GUILayout.Width(32), GUILayout.Height(32));
			GUILayout.Label(reward.name + " x" + reward.count);
			GUILayout.EndHorizontal();
		}
		if (q.gradeInfo[0].RewardItemsToChoose.Count > 0)
			GUILayout.Label("You can choose one of following items:");
		foreach (QuestRewardEntry reward in q.gradeInfo[0].RewardItemsToChoose) {
			GUILayout.BeginHorizontal();
			GUILayout.Button(reward.icon, GUILayout.Width(32), GUILayout.Height(32));
			GUILayout.Label(reward.name + " x" + reward.count);
			GUILayout.EndHorizontal();
		}
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Accept")) {
			ClientAPI.ScriptObject.GetComponent<Quests>().AcceptQuest(0);
			ToggleOpen();
		} else if (GUILayout.Button("Decline")) {
			ClientAPI.ScriptObject.GetComponent<Quests>().DeclineQuest(0);
			ToggleOpen();
		}
		GUILayout.EndHorizontal();
	}

	void DrawQuestProgress() {
		QuestLogEntry q = ClientAPI.ScriptObject.GetComponent<Quests>().GetQuestProgressInfo(0);
		GUILayout.Label(q.Title);
		GUILayout.TextArea(q.ProgressText);
		if (q.Complete) {
			if (GUILayout.Button("Continue")) {
				dialogueType = DialogueType.QuestConclusion;
			}
		}
	}

	void DrawQuestConclusion() {
		QuestLogEntry q = ClientAPI.ScriptObject.GetComponent<Quests>().GetQuestProgressInfo(0);
		GUILayout.Label("Conclude Quest: " + q.Title);
		GUILayout.TextArea(q.gradeInfo[0].completionText);
		GUILayout.Label("Rewards");
		// TODO: Rewards
		if (q.gradeInfo[0].rewardItems.Count > 0)
			GUILayout.Label("You will receive the following items:");
		foreach (QuestRewardEntry reward in q.gradeInfo[0].rewardItems) {
			GUILayout.BeginHorizontal();
			GUILayout.Button(reward.icon, GUILayout.Width(32), GUILayout.Height(32));
			GUILayout.Label(reward.name + " x" + reward.count);
			GUILayout.EndHorizontal();
		}
		if (q.gradeInfo[0].RewardItemsToChoose.Count > 0) {
			GUILayout.Label("You can choose one of following items:");
		}
		int pos = 0;
		foreach (QuestRewardEntry reward in q.gradeInfo[0].RewardItemsToChoose) {
			GUILayout.BeginHorizontal();
			if (reward.id == q.itemChosen) {
				if (GUILayout.Button(reward.icon, GUILayout.Width(32), GUILayout.Height(32))) {
				}
				GUILayout.Label(reward.name + " x" + reward.count + " (Chosen)");
			} else {
				if (GUILayout.Button(reward.icon, GUILayout.Width(32), GUILayout.Height(32))) {
					q.itemChosen = reward.id;
				}
				GUILayout.Label(reward.name + " x" + reward.count);
			}
			pos++;
			
			GUILayout.EndHorizontal();
		}
		GUILayout.BeginHorizontal();
		if (q.gradeInfo[0].RewardItemsToChoose.Count == 0 || q.itemChosen > 0) {
			if (GUILayout.Button("Complete")) {
				ClientAPI.ScriptObject.GetComponent<Quests>().CompleteQuest();
				ToggleOpen();
			}
		} else {
			GUILayout.Label("You must choose a reward to complete this quest.");
		}
		GUILayout.EndHorizontal();
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "NPC_INTERACTIONS_UPDATE") {
			dialogueType = DialogueType.Options;
			if (!open)
				ToggleOpen();
		} else if (eData.eventType == "DIALOGUE_UPDATE") {
			dialogueType = DialogueType.Chat;
			if (!open)
				ToggleOpen();
		} else if (eData.eventType == "QUEST_OFFERED_UPDATE") {
			dialogueType = DialogueType.QuestOffer;
			if (!open)
				ToggleOpen();
		} else if (eData.eventType == "QUEST_PROGRESS_UPDATE") {
			dialogueType = DialogueType.QuestProgress;
			if (!open)
				ToggleOpen();
		}
	}
}

