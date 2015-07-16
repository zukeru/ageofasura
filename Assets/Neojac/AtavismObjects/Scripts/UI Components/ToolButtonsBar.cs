using UnityEngine;
using System.Collections;

public class ToolButtonsBar : AtavismWindowTemplate {
	public int buttonHeight = 32;
	public int buttonWidth = 16;
	public Texture2D characterIcon;
	public Texture2D questLogIcon;
	public Texture2D skillsIcon;
	public Texture2D builderIcon;
	int numButtons = 4;

	// Use this for initialization
	void Start () {
		width = buttonWidth * numButtons + 4;
		height = buttonHeight + 4;
		SetupRect();
		ToggleOpen();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	 
	void OnGUI() {
		GUI.depth = uiLayer;
		GUI.skin = skin;
		GUI.Box(uiRect, "");
		
		if (GUI.Button(new Rect(uiRect.x + 0 * buttonWidth + 2, uiRect.y + 2, buttonWidth, buttonHeight), characterIcon))
		{
			GetComponent<Character>().ToggleOpen();
		}
		if (GUI.Button(new Rect(uiRect.x + 1 * buttonWidth + 2, uiRect.y + 2, buttonWidth, buttonHeight), questLogIcon))
		{
			GetComponent<QuestLog>().ToggleOpen();
		}
		if (GUI.Button(new Rect(uiRect.x + 2 * buttonWidth + 2, uiRect.y + 2, buttonWidth, buttonHeight), skillsIcon))
		{
			GetComponent<SkillsUI>().ToggleOpen();
		}
		if (GUI.Button(new Rect(uiRect.x + 3 * buttonWidth + 2, uiRect.y + 2, buttonWidth, buttonHeight), builderIcon))
		{
			GetComponent<WorldBuilderUI>().ToggleOpen();
		}
	}
}
