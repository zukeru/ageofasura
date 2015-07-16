using UnityEngine;
using System.Collections;

public class CombatPlugin : AtavismPlugin {

	// Use this for initialization
	public CombatPlugin()
	{	
		pluginName = "Combat";
		string serverCategory = "AT_button_category_combat";
		icon = (Texture)Resources.Load (serverCategory, typeof(Texture));
		iconOver = (Texture)Resources.Load (serverCategory + "_over", typeof(Texture));
		iconSelected = (Texture)Resources.Load (serverCategory + "_selected", typeof(Texture));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
