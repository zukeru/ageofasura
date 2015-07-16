using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StandardCommands : MonoBehaviour {

	static StandardCommands instance;

	// Use this for initialization
	void Start () {
		if (instance != null) {
			return;
		}
		instance = this;
		
		AtavismCommand.RegisterCommandHandler("loc", HandleLoc);
		AtavismCommand.RegisterCommandHandler("orient", HandleOrient);
		AtavismCommand.RegisterCommandHandler("props", HandleProperties);
		AtavismCommand.RegisterCommandHandler("say", HandleSay);
		AtavismCommand.RegisterCommandHandler("pet", HandleSpawnPet);
		AtavismCommand.RegisterCommandHandler("purchaseSkillPoint", HandlePurchaseSkillPoint);
		AtavismCommand.RegisterCommandHandler("getMobTemplates", HandleGetMobTemplates);
		AtavismCommand.RegisterCommandHandler("saveMob", HandleSaveMob);
		AtavismCommand.RegisterCommandHandler("probe", HandleBuildingGridProbe);
		AtavismCommand.RegisterCommandHandler("spawner", HandleToggleBuilder);
		AtavismCommand.RegisterCommandHandler("claim", HandleClaim);
		AtavismCommand.RegisterCommandHandler("wave", HandleWave);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void HandleLoc(string args_str) {
		AtavismLogger.LogDebugMessage("Got loc command");
		AtavismObjectNode target = ClientAPI.GetTargetObject();
    	if (target == null)
    		target = ClientAPI.GetPlayerObject();
    	ClientAPI.Write("Target Position: " + target.Position);
	}
	
	public void HandleOrient(string args_str) {
		AtavismLogger.LogDebugMessage("Got orient command");
		AtavismPlayer player = ClientAPI.GetPlayerObject();
    	ClientAPI.Write("Player Position: " + player.Orientation);
	}
	
	public void HandleProperties(string args_str) {
		AtavismObjectNode target = ClientAPI.GetTargetObject();
    	if (target == null)
    		target = ClientAPI.GetPlayerObject();
    	ClientAPI.Write("Properties for: " + target.Name + " with num Properties: " + target.PropertyNames.Count);
    	foreach (string prop in target.PropertyNames)
        	ClientAPI.Write(prop + ": " + target.GetProperty(prop));
	}
	
	public void HandleSay(string args_str) {
		AtavismLogger.LogDebugMessage("Got say command with message: " + args_str);
    	//ClientAPI.Network.SendCommMessage(args_str);
		CommMessage commMessage = new CommMessage();
        commMessage.ChannelId = 1;  // CommChannel.Say
        commMessage.Message = args_str;
        commMessage.SenderName = ClientAPI.GetPlayerObject().Name;
		AtavismNetworkHelper.Instance.SendMessage(commMessage);
		AtavismLogger.LogDebugMessage("Sent chat message: " + commMessage);
	}
	
	public void HandleSpawnPet(string args_str) {
		GameObject pet = (GameObject)Instantiate(Resources.Load(""));
	}
	
	public void HandlePurchaseSkillPoint(string args_str) {
		Dictionary<string, object> props = new Dictionary<string, object> ();
		NetworkAPI.SendExtensionMessage (ClientAPI.GetPlayerOid (), false, "combat.PURCHASE_SKILL_POINT", props);
	}
	
	public void HandleGetMobTemplates(string args_str) {
		//MobCreator.GetMobTemplates();
	}
	
	public void HandleSaveMob(string args_str) {
		ClientAPI.ScriptObject.GetComponent<MobCreator>().SaveMobTemplate();
	}
	
	public void HandleBuildingGridProbe(string args_str) {
		GameObject probe = (GameObject)Resources.Load("Content/BuildingGridProbe");
		GameObject go = (GameObject)Instantiate(probe, ClientAPI.GetPlayerObject().Position, ClientAPI.GetPlayerObject().Orientation);
		AtavismLogger.LogDebugMessage("Created probe");
	}

	public void HandleToggleBuilder(string args_str) {
		Camera.main.GetComponentInChildren<MobCreator>().ToggleBuildingModeEnabled();
	}

	public void HandleClaim(string args_str) {
		Dictionary<string, object> props = new Dictionary<string, object> ();
		props.Add("loc", ClientAPI.GetPlayerObject().Position);
		int size = int.Parse(args_str);
		props.Add("size", size);
		NetworkAPI.SendExtensionMessage (ClientAPI.GetPlayerOid (), false, "voxel.CREATE_CLAIM", props);
		ClientAPI.Write("Sent claim message");
	}
	
	public void HandleWave(string args_str) {
		Dictionary<string, object> props = new Dictionary<string, object> ();
		props.Add ("coordEffect", "PlayWaveAnimation");
		props.Add ("hasTarget", false);
		NetworkAPI.SendExtensionMessage (ClientAPI.GetPlayerOid(), false, "ao.PLAY_COORD_EFFECT", props);
	}

	public void Quit() {
		Application.Quit();
	}
	
}
