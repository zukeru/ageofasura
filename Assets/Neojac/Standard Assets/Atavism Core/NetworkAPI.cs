using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void ExtensionMessageHandler(Dictionary<string, object> props);

public class NetworkAPI : MonoBehaviour {
	
	static NetworkAPI instance;
	
	static Dictionary<string, List<ExtensionMessageHandler>> extensionHandlers = new Dictionary<string, List<ExtensionMessageHandler>>();
	
	// Use this for initialization
	void Start () {
		if (instance != null) {
			return;
		}
		instance = this;
		
        MessageDispatcher.Instance.RegisterHandler(WorldMessageType.Extension, HandleExtensionMessage);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
    
    #region Methods to send messages
    public static CharacterEntry CreateCharacter(Dictionary<string, object> attrs) {
		return AtavismClient.Instance.NetworkHelper.CreateCharacter(attrs);
	}

    public static void DeleteCharacter(Dictionary<string, object> attrs) {
		AtavismClient.Instance.NetworkHelper.DeleteCharacter(attrs);
	}
        
    public static void SendMessage(BaseWorldMessage message) {
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendQuestResponseMessage(long objectId, long questId, bool accepted) {
        QuestResponseMessage message = new QuestResponseMessage();
        message.ObjectId = objectId;
        message.QuestId = questId;
        message.Accepted = accepted;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendCommMessage(string text) {
        CommMessage message = new CommMessage();
        message.ChannelId = 1; // CommChannel.Say
        message.Message = text;
        message.SenderName = ClientAPI.GetPlayerObject().Name;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendAcquireMessage(long objectId) {
        AcquireMessage message = new AcquireMessage();
        message.ObjectId = objectId;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendEquipMessage(long objectId, string slotName) {
        EquipMessage message = new EquipMessage();
        message.ObjectId = objectId;
        message.SlotName = slotName;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendAttackMessage(long objectId, string attackType, bool attackStatus) {
        AutoAttackMessage message = new AutoAttackMessage();
        message.ObjectId = objectId;
        message.AttackType = attackType;
        message.AttackStatus = attackStatus;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendLogoutMessage() {
        LogoutMessage message = new LogoutMessage();
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendTargetedCommand(long objectId, string text) {
        CommandMessage message = new CommandMessage();
		message.ObjectId = objectId;
        message.Command = text;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendQuestInfoRequestMessage(long objectId) {
        QuestInfoRequestMessage message = new QuestInfoRequestMessage();
        message.ObjectId = objectId;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendQuestConcludeRequestMessage(long objectId) {
        QuestConcludeRequestMessage message = new QuestConcludeRequestMessage();
        message.ObjectId = objectId;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendTradeOffer(long partnerId, List<long> itemIds, bool accepted, bool cancelled) {
        TradeOfferRequestMessage message = new TradeOfferRequestMessage();
        message.Oid = ClientAPI.GetPlayerObject().Oid;
        message.ObjectId = partnerId;
        message.Accepted = accepted;
        message.Cancelled = cancelled;
        message.Offer = itemIds;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendActivateItemMessage(OID itemId, long objectId) {
        ActivateItemMessage message = new ActivateItemMessage();
        message.ItemId = itemId;
        message.ObjectId = objectId;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}

    public static void SendExtensionMessage(long targetOid, bool clientTargeted, string extensionType, Dictionary<string, object> properties) {
        ExtensionMessage message = new ExtensionMessage(targetOid, clientTargeted);
		foreach (string key in properties.Keys) {
			message.Properties[key] = properties[key];
		}
        message.Properties["ext_msg_subtype"] = extensionType;
		AtavismClient.Instance.NetworkHelper.SendMessage(message);
	}
									
	#endregion Methods to send messages
        
    public static void RegisterExtensionMessageHandler(string extensionType, ExtensionMessageHandler handler) {
        List<ExtensionMessageHandler> handlers;
        if (extensionHandlers.ContainsKey(extensionType))
            handlers = extensionHandlers[extensionType];
        else
            handlers = new List<ExtensionMessageHandler>();
        handlers.Add(handler);
		extensionHandlers[extensionType] = handlers;
	}
    
    public static void RemoveExtensionMessageHandler(string extensionType, ExtensionMessageHandler handler) {
        if (extensionHandlers.ContainsKey(extensionType)) {
            List<ExtensionMessageHandler> handlers = extensionHandlers[extensionType];
            handlers.Remove(handler);
            if (handlers.Count == 0)
                extensionHandlers.Remove(extensionType);
		}
	}

    public static void HandleExtensionMessage(BaseMessage message) {
        if (message == null || !(message is ExtensionMessage))
            return;
		ExtensionMessage extMessage = (ExtensionMessage) message;
        string extensionType = null;
        if (extMessage.Properties.ContainsKey("ext_msg_subtype")) {
            extensionType = (string) extMessage.Properties["ext_msg_subtype"];
		} else if (extMessage.Properties.ContainsKey("ext_msg_type")) {
			AtavismLogger.LogWarning("Extension message with 'ext_msg_type' is deprecated. Please use 'ext_msg_subtype'");
            extensionType = (string) extMessage.Properties["ext_msg_type"];
		} else {
			AtavismLogger.LogWarning("Received extension message without a subtype");
            return;
		}
		AtavismLogger.LogDebugMessage("Got extension message with type: " + extensionType);
        if (extensionHandlers.ContainsKey(extensionType)) {
            extMessage.Properties["ext_msg_subject_oid"] = extMessage.Oid;
            extMessage.Properties["ext_msg_target_oid"] = extMessage.TargetOid;
            extMessage.Properties["ext_msg_client_targeted"] = extMessage.ClientTargeted;
            List<ExtensionMessageHandler> handlers = extensionHandlers[extensionType];
			AtavismLogger.LogDebugMessage("Got " + handlers.Count + " handlers for extension message");
            foreach (ExtensionMessageHandler handler in handlers)
                handler(extMessage.Properties);
		}
	}
}
