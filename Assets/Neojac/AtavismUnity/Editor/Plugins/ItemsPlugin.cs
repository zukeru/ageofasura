using UnityEngine;
using System.Collections;

public class ItemsPlugin : AtavismPlugin {

	// Use this for initialization
	public ItemsPlugin()
	{	
		pluginName = "Item";
		string serverCategory = "AT_button_category_items";
		icon = (Texture)Resources.Load (serverCategory, typeof(Texture));
		iconOver = (Texture)Resources.Load (serverCategory + "_over", typeof(Texture));
		iconSelected = (Texture)Resources.Load (serverCategory + "_selected", typeof(Texture));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
