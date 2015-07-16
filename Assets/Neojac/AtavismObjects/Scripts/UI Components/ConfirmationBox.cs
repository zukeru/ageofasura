using UnityEngine;
using System.Collections;

public class ConfirmationBox : AtavismWindowTemplate {
	
	string confirmationType;
	string confirmationMessage;
	object confirmationObject;
    bool show = false;

    GameObject target;
    string targetMethod;

	// Use this for initialization
	void Start () {
		SetupRect();
	
		// Register for 
		AtavismEventSystem.RegisterEvent("DELETE_ITEM_REQ", this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy () {
		AtavismEventSystem.UnregisterEvent("DELETE_ITEM_REQ", this);
	}
	
	void OnGUI() {
        if (show && confirmationMessage != null && confirmationMessage != "")
        {
			GUI.depth = uiLayer;
			GUI.skin = skin;
			GUI.Box(uiRect, "");
			GUI.Label(new Rect(uiRect.x + 5, uiRect.y + 5, uiRect.width-10, 60), confirmationMessage);
			if (GUI.Button(new Rect(uiRect.x + 5, uiRect.yMax - 25, 35, 20), "Yes")) {
                target.SendMessage(targetMethod, confirmationObject);
				confirmationMessage = "";
				confirmationType = "";
				confirmationObject = null;
                show = false;
			} else if (GUI.Button(new Rect(uiRect.xMax - 35, uiRect.yMax - 25, 30, 20), "No")) {
				confirmationMessage = "";
				confirmationType = "";
				confirmationObject = null;
                show = false;
			}
		}
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "DELETE_ITEM_REQ") {
			confirmationObject = eData.eventArgs[0];
			confirmationMessage = "Delete " + eData.eventArgs[1] + "?";
			confirmationType = "delete_item";
		}
	}

    public void ShowConfirmationBox(string message, object confirmObject, GameObject responseTarget, string responseMethod)
    {
        confirmationMessage = message;
        confirmationObject = confirmObject;
        target = responseTarget;
        targetMethod = responseMethod;
        show = true;
    }

    public void SetMessage(string message)
    {
        confirmationMessage = message;
    }

    public void SetObject(object confirmObject)
    {
        confirmationObject = confirmObject;
    }

    public void SetResponseTarget(GameObject responseTarget)
    {
        target = responseTarget;
    }

    public void SetResponseMethod(string responseMethod)
    {
        targetMethod = responseMethod;
    }

    public void Show()
    {
        show = true;
    }
}

