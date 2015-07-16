using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtavismScriptEvent : MonoBehaviour
{

	public static AtavismScriptEvent instance;
	
	// Use this for initialization
	void Start ()
	{
		if (instance == null) {
			instance = this;
			MessageDispatcher.Instance.RegisterHandler (WorldMessageType.Comm, _HandleComm);
			MessageDispatcher.Instance.RegisterHandler(WorldMessageType.ObjectProperty, _HandleObjectProperty);
			NetworkAPI.RegisterExtensionMessageHandler("combat_text2", HandleCombatText2);
			NetworkAPI.RegisterExtensionMessageHandler("combat_text", HandleCombatText);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void _HandleComm (BaseWorldMessage message)
	{
		CommMessage commMessage = (CommMessage)message;
		AtavismLogger.LogDebugMessage("Got comm message with channel: " + commMessage.ChannelId);
		if (commMessage.ChannelId == 0) {
			// Server channel (0)
			//ClientAPI.Interface.DispatchEvent("CHAT_MSG_SAY", [message.Message, nodeName, ""]);
			string[] args = new string[1];
			args[0] = commMessage.Message;
			AtavismEventSystem.DispatchEvent("CHAT_MSG_SERVER", args);
		} else if (commMessage.ChannelId == 1) {
			// Say channel (1)
			//ClientAPI.Interface.DispatchEvent("CHAT_MSG_SAY", [message.Message, nodeName, ""]);
			string[] args = new string[3];
			args[0] = commMessage.Message;
			args[1] = commMessage.SenderName;
			args[2] = "";
			AtavismEventSystem.DispatchEvent("CHAT_MSG_SAY", args);
		} else if (commMessage.ChannelId == 2) {
			// ServerInfo channel (2)
			//ClientAPI.Interface.DispatchEvent("CHAT_MSG_SYSTEM", [message.Message, ""]);
			string[] args = new string[3];
			args[0] = commMessage.Message;
			args[1] = "System";
			args[2] = "";
			AtavismEventSystem.DispatchEvent("CHAT_MSG_SYSTEM", args);
		}
	}
	
	public void _HandleObjectProperty(BaseWorldMessage message) {
		ObjectPropertyMessage propMessage = (ObjectPropertyMessage)message;
		if (propMessage.Properties.Count <= 0)
        	return;
		AtavismObjectNode target = ClientAPI.GetTargetObject();
    	AtavismPlayer player = ClientAPI.GetPlayerObject();
    	//pet = None;
    	//activePet = MarsUnit._GetUnitProperty2("player", "activePet", None);
    	//if (activePet != null)
        //	pet = ClientAPI.World.GetObjectByOID(activePet);
		string[] args;
    	foreach (string prop in propMessage.Properties.Keys) {
        	string eventName = "PROPERTY_" + prop;
        	if (target != null && message.Oid == target.Oid) {
				args = new string[1];
				args[0] = "target";
				AtavismEventSystem.DispatchEvent(eventName, args);
			}
        	if (player != null && propMessage.Oid == player.Oid) {
				args = new string[1];
				args[0] = "player";
				AtavismEventSystem.DispatchEvent(eventName, args);
			}
        	//if (pet != null && message.Oid == pet.OID)
            //	ClientAPI.Interface.DispatchEvent(eventName, ["pet"]);
        	// Always post an "any" unit event.
			args = new string[2];
			args[0] = "any";
			args[1] = propMessage.Oid.ToString();
			AtavismEventSystem.DispatchEvent(eventName, args);
		}
	}
	
	public void HandleCombatText2(Dictionary<string, object> props) {
		int messageType = (int)props["DmgType"];
		string dmgAmount = (string)props["DmgAmount"];
		OID targetOID = (OID)props["target"];
		AtavismObjectNode target = ClientAPI.WorldManager.GetObjectNode(targetOID);
		target.GameObject.GetComponent<AtavismMobController>().GotDamageMessage(messageType, dmgAmount);
	}
	
	public void HandleCombatText(Dictionary<string, object> props) {
		int messageType = (int)props["DmgType"];
		string dmgAmount = (string)props["DmgAmount"];
		AtavismPlayer player = ClientAPI.GetPlayerObject();
		player.GameObject.GetComponent<AtavismMobController>().GotDamageMessage(messageType, dmgAmount);
	}
}
