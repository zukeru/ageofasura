using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillsUI : AtavismWindowTemplate {

	public int buttonSize = 32;
	Vector2 scrollPos = new Vector2();
	public KeyCode toggleButton;

	// Use this for initialization
	void Start () {
		SetupRect();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(toggleButton)) {
			ToggleOpen();
		}
	}
	
	void OnGUI() {
		if (!open)
			return;
			
		GUI.depth = uiLayer;
		GUI.skin = skin;
		
		GUI.Box(uiRect, "");
		GUILayout.BeginArea(new Rect(uiRect));
		// Top Bar
		GUILayout.BeginHorizontal();
		GUILayout.Label("Skills");
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("X")) {
			ToggleOpen();
		}
		GUILayout.EndHorizontal();
		// Skill list
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		int pos = 0;
		foreach (Skill skill in ClientAPI.ScriptObject.GetComponent<Skills>().SkillsList.Values) {
			if (skill.parentSkill < 1) {
				DrawSkill(skill, 0);
			}
			pos++;
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
	
	/// <summary>
	/// Draws the User Interface for the skill, then calls this for any parent skills
	/// </summary>
	/// <param name="skill">Skill.</param>
	/// <param name="level">Level.</param>
	void DrawSkill(Skill skill, int level) {
		GUILayout.BeginHorizontal();
		GUILayout.Button(skill.icon, GUILayout.Width(32), GUILayout.Height(32));
		GUILayout.Label(skill.skillname);
		GUILayout.Label(skill.CurrentLevel + "/" + skill.CurrentPoints);
		GUILayout.EndHorizontal();
		foreach (Skill subSkill in ClientAPI.ScriptObject.GetComponent<Skills>().SkillsList.Values) {
			if (skill.parentSkill == skill.id) {
				DrawSkill(subSkill, level + 1);
			}
		}
	}
}
