using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ErrorMessage : AtavismWindowTemplate {
	string errorMessage;
	float stopDisplay;

	// Use this for initialization
	void Start () {
		SetupRect();
		
		// Register for 
		AtavismEventSystem.RegisterEvent("ERROR_MESSAGE", this);
		
		NetworkAPI.RegisterExtensionMessageHandler("error_message", HandleErrorMessage);
		NetworkAPI.RegisterExtensionMessageHandler("ability_error", HandleAbilityErrorMessage);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		GUI.skin = skin;
		if (Time.time < stopDisplay) {
			GUI.Label(uiRect, errorMessage, skin.GetStyle("ErrorText"));
			//GUI.Label(new Rect(Screen.width / 2 - 100, 50, 200, 40), errorMessage);
		}
	}
	
	public void HandleErrorMessage(Dictionary<string, object> props) {
		errorMessage = (string) props["ErrorText"];
		stopDisplay = Time.time + 3;
	}

	public void HandleAbilityErrorMessage(Dictionary<string, object> props) {
		int messageType = (int)props["ErrorText"];
		if (messageType == 1) {
			errorMessage = "Invalid target";
		} else if (messageType == 2) {
			errorMessage = "Target is too far away";
		} else if ( messageType == 3) {
			errorMessage = "Target is too close";
		} else if ( messageType == 4) {
			errorMessage = "You cannot perform that action yet";
		} else if ( messageType == 5) {
			errorMessage = "Not enough Mana";
			/*int Etype = ClientAPI.GetObjectProperty("energy-type");
			if (Etype == 1) {
				errorMessage = "Not enough Mana";
			} else if ( Etype == 2) {
				errorMessage = "Not enough Mana";
			} else if ( Etype == 3) {
				errorMessage = "Not enough Power";
			}*/
		} else if ( messageType == 6) {
			errorMessage = "You do not have the required reagent";
		} else if ( messageType == 7) {
			errorMessage = "You do not have the required tool";
		} else if ( messageType == 8) {
			errorMessage = "You have not met the requirement for the action";
		} else if ( messageType == 9) {
			errorMessage = "You are not in the correct stance";
		} else if ( messageType == 10) {
			errorMessage = "You do not have the required weapon equipped";
		} else if ( messageType == 11) {
			errorMessage = "You do not have a shield equipped";
		} else if ( messageType == 12) {
			errorMessage = "Not Enough Vigor";
		} else if ( messageType == 13) {
			errorMessage = "You do not have the required effect";
		} else if ( messageType == 14) {
			errorMessage = "You have no target";
		} else if ( messageType == 15) {
			errorMessage = "You do not have the required weapon type equipped";
		} else if ( messageType == 16) {
			errorMessage = "You cannot activate a passive ability";
		} else if ( messageType == 17) {
			errorMessage = "Interrupted";
		} else if ( messageType == 18) {
			errorMessage = "You cannot do that while you are dead";
		}
		stopDisplay = Time.time + 3;
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "ERROR_MESSAGE") {
			errorMessage = eData.eventArgs[0];
			stopDisplay = Time.time + 3;
		}
	}
	
}
