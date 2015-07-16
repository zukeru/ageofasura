using UnityEngine;
using System.Collections;

public class QuestLog : AtavismWindowTemplate {
	
	Vector2 scrollPos = new Vector2();
	Vector2 questScrollPos = new Vector2();
	public KeyCode toggleButton;

	// Use this for initialization
	void Start () {
		SetupRect();
		// Register for 
		AtavismEventSystem.RegisterEvent("QUEST_LOG_UPDATE", this);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(toggleButton)) {
			ToggleOpen();
		}
	}
	
	void OnDestroy () {
		AtavismEventSystem.UnregisterEvent("QUEST_LOG_UPDATE", this);
	}
	
	void OnGUI() {
		if (!open)
			return;
			
		GUI.depth = uiLayer;
		GUI.skin = skin;

		GUI.Box(uiRect, "");
		GUILayout.BeginArea(new Rect(uiRect));
		// NPC Name
		GUILayout.BeginHorizontal();
		GUILayout.Label("Quest Log");
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("X")) {
			ToggleOpen();
		}
		GUILayout.EndHorizontal();
		// Quest list
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		int pos = 0;
		foreach (QuestLogEntry q in ClientAPI.ScriptObject.GetComponent<Quests>().QuestLogEntries) {
			if (GUILayout.Button(q.Title)) {
				ClientAPI.ScriptObject.GetComponent<Quests>().ViewQuestLogEntry(pos);
			}
			pos++;
		}
		GUILayout.EndScrollView();
		QuestLogEntry selectedQuest = ClientAPI.ScriptObject.GetComponent<Quests>().GetSelectedQuestLogEntry();
		if (selectedQuest != null) {
			// Scroll view
			questScrollPos = GUILayout.BeginScrollView(questScrollPos);

			GUILayout.Label("Quest: " + selectedQuest.Title);
			
			// Draw the Objectives first
			GUILayout.Label("Objective");
			GUILayout.TextArea(selectedQuest.Objective);
			foreach (string objective in selectedQuest.gradeInfo[0].objectives) {
				GUILayout.Label(objective);
			}
			
			GUILayout.TextArea(selectedQuest.Description);
			GUILayout.Label("Rewards");
			// TODO: Rewards
			if (selectedQuest.gradeInfo[0].rewardItems.Count > 0)
				GUILayout.Label("You will receive the following items:");
			foreach (QuestRewardEntry reward in selectedQuest.gradeInfo[0].rewardItems) {
				GUILayout.BeginHorizontal();
				GUILayout.Button(reward.icon, GUILayout.Width(32), GUILayout.Height(32));
				GUILayout.Label(reward.name + " x" + reward.count);
				GUILayout.EndHorizontal();
			}
			if (selectedQuest.gradeInfo[0].RewardItemsToChoose.Count > 0)
				GUILayout.Label("You can choose one of following items:");
			foreach (QuestRewardEntry reward in selectedQuest.gradeInfo[0].RewardItemsToChoose) {
				GUILayout.BeginHorizontal();
				GUILayout.Button(reward.icon, GUILayout.Width(32), GUILayout.Height(32));
				GUILayout.Label(reward.name + " x" + reward.count);
				GUILayout.EndHorizontal();
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Abandon Quest")) {
				ClientAPI.ScriptObject.GetComponent<Quests>().AbandonQuest();
				ToggleOpen();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
		}
		GUILayout.EndArea();
	}
	
	public void OnEvent(AtavismEventData eData) {
	}
}

