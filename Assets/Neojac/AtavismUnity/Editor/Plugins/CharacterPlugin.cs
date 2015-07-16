using UnityEngine;
using System.Collections;

public class CharacterPlugin : AtavismPlugin {

	// Use this for initialization
	public CharacterPlugin()
	{	
		pluginName = "Character";
		string serverCategory = "AT_button_category_character";
		icon = (Texture)Resources.Load (serverCategory, typeof(Texture));
		iconOver = (Texture)Resources.Load (serverCategory + "_over", typeof(Texture));
		iconSelected = (Texture)Resources.Load (serverCategory + "_selected", typeof(Texture));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
