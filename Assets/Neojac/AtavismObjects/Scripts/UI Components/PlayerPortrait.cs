using UnityEngine;
using System.Collections;

public class PlayerPortrait : AtavismWindowTemplate {

	public GUISkin skin2;
	public GUISkin bar;
	
	int health;
	int healthmax;
	int mana;
	int manamax;
	int xp;
	int xpmax;
	int level;

	string health_prop = "health";
	string health_max_prop = "health-max";
	string mana_prop = "mana";
	string mana_max_prop = "mana-max";
	string xp_prop = "experience";
	string xp_max_prop = "experience-max";
	string level_prop = "level";

	// Use this for initialization
	void Start () {
		SetupRect();
		ToggleOpen();
		
		// Register for 
		ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(health_prop, HealthHandler);
		ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(health_max_prop, HealthMaxHandler);
		ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(mana_prop, ManaHandler);
		ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(mana_max_prop, ManaMaxHandler);
		ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(xp_prop, XpHandler);
		ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(xp_max_prop, XpMaxHandler);
		ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(level_prop, LevelHandler);
		
		// The player may have changed scenes, but their stats were not sent back down, so let's take a look
		if (ClientAPI.GetPlayerObject() != null) {
			if (ClientAPI.GetPlayerObject().PropertyExists(health_prop)) {
				health = (int)ClientAPI.GetPlayerObject().GetProperty(health_prop);
			}
			if (ClientAPI.GetPlayerObject().PropertyExists(health_max_prop)) {
				healthmax = (int)ClientAPI.GetPlayerObject().GetProperty(health_max_prop);
			}
			if (ClientAPI.GetPlayerObject().PropertyExists(mana_prop)) {
				mana = (int)ClientAPI.GetPlayerObject().GetProperty(mana_prop);
			}
			if (ClientAPI.GetPlayerObject().PropertyExists(mana_max_prop)) {
				manamax = (int)ClientAPI.GetPlayerObject().GetProperty(mana_max_prop);
			}
			if (ClientAPI.GetPlayerObject().PropertyExists(xp_prop)) {
				xp = (int)ClientAPI.GetPlayerObject().GetProperty(xp_prop);
			}
			if (ClientAPI.GetPlayerObject().PropertyExists(xp_max_prop)) {
				xpmax = (int)ClientAPI.GetPlayerObject().GetProperty(xp_max_prop);
			}
			if (ClientAPI.GetPlayerObject().PropertyExists(level_prop)) {
				level = (int)ClientAPI.GetPlayerObject().GetProperty(level_prop);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy () {
	}
	
	void OnGUI() {
		if (!open)
			return;
	
		GUI.depth = uiLayer;
		GUI.skin = skin;
		// Name
		GUI.Label(uiRect, ClientAPI.GetPlayerObject().Name);
		// Health
		int barLength = 120;
		float healthBarLength = ((float)health / (float)healthmax) * barLength;
		float adjustment = 5;

		GUI.skin = skin2;
		GUI.Box (new Rect (uiRect.x, uiRect.y + 20, barLength + 15, 15), "");
		
		GUI.skin = bar;
		GUI.color = Color.red;
		GUI.Box (new Rect (uiRect.x + 9, uiRect.y + 23, healthBarLength, 8), "");
		GUI.color = Color.white;
		GUI.Label(new Rect(uiRect.x, uiRect.y + 15, barLength, 25), "Health: " + health + "/" + healthmax);

		// Mana
		float manaBarLength = ((float)mana / (float)manamax) * barLength;
		
		GUI.skin = skin2;
		GUI.Box (new Rect (uiRect.x, uiRect.y + 37, barLength + 15, 15), "");
		
		GUI.skin = bar;
		GUI.color = Color.red;
		GUI.Box (new Rect (uiRect.x + 9, uiRect.y + 40, manaBarLength, 8), "");
		GUI.color = Color.white;
		GUI.Label(new Rect(uiRect.x, uiRect.y + 32, barLength, 25), "Mana: " + mana + "/" + manamax);

		// Experience
		float xpBarLength = ((float)xp / (float)xpmax) * barLength;
		
		GUI.skin = skin2;
		GUI.Box (new Rect (uiRect.x, uiRect.y + 54, barLength + 15, 15), "");
		
		GUI.skin = bar;
		GUI.color = Color.red;
		GUI.Box (new Rect (uiRect.x + 9, uiRect.y + 57, xpBarLength, 8), "");
		GUI.color = Color.white;
		GUI.Label(new Rect(uiRect.x, uiRect.y + 49, barLength, 25), "XP: " + xp + "/" + xpmax);

		// Level
		GUI.Label(new Rect(uiRect.x, uiRect.y + 66, barLength, 25), "Level: " + level);
	}

	public void HealthHandler(object sender, PropertyChangeEventArgs args) {
		health = (int)ClientAPI.GetPlayerObject().GetProperty(health_prop);
	}

	public void HealthMaxHandler(object sender, PropertyChangeEventArgs args) {
		healthmax = (int)ClientAPI.GetPlayerObject().GetProperty(health_max_prop);
	}
	
	public void ManaHandler(object sender, PropertyChangeEventArgs args) {
		mana = (int)ClientAPI.GetPlayerObject().GetProperty(mana_prop);
	}
	
	public void ManaMaxHandler(object sender, PropertyChangeEventArgs args) {
		manamax = (int)ClientAPI.GetPlayerObject().GetProperty(mana_max_prop);
	}

	public void XpHandler(object sender, PropertyChangeEventArgs args) {
		xp = (int)ClientAPI.GetPlayerObject().GetProperty(xp_prop);
	}
	
	public void XpMaxHandler(object sender, PropertyChangeEventArgs args) {
		xpmax = (int)ClientAPI.GetPlayerObject().GetProperty(xp_max_prop);
	}

	public void LevelHandler(object sender, PropertyChangeEventArgs args) {
		level = (int)ClientAPI.GetPlayerObject().GetProperty(level_prop);
	}

}
