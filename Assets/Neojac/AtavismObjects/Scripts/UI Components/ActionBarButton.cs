using UnityEngine;
using System.Collections;

public class ActionBarButton : MonoBehaviour {
	
	public AtavismAction action;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnClick () { Activate(); }
	
	public void ActionUpdate(AtavismAction action) {
		this.action = action;
		if (action == null || action.actionObject == null) {
			//TODO: set to blank icon
		} else {
			/*GetComponentInChildren<UISprite>().spriteName = action.actionObject.icon.name;
			GetComponent<UIImageButton>().hoverSprite = action.actionObject.icon.name;
			GetComponent<UIImageButton>().normalSprite = action.actionObject.icon.name;
			GetComponent<UIImageButton>().pressedSprite = action.actionObject.icon.name;*/
		}
	}
	
	public void Activate() {
		// Temp Hack
		ClientAPI.ScriptObject.SendMessage("SurveyLand");
		//action.Activate();
	}
}
