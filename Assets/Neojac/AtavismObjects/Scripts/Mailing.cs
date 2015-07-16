using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MailItem {
	public AtavismInventoryItem item = null;
	public int count = 1;
}

public class MailEntry
{
    public int mailId = -1;
    public OID senderOid;
    public string senderName = "";
    public string subject = "";
    public string message = "";
    public List<MailItem> items;
    public int currencyType;
    public int currencyAmount = 0;
    public bool cashOnDelivery = false;
    public bool read = false;
}

public class Mailing : MonoBehaviour {

    public int itemLimit = 10;
    List<MailEntry> mailList = new List<MailEntry>();
    MailEntry mailBeingComposed;
    Vector3 mailboxLocation = Vector3.zero;

	// Use this for initialization
	void Start () {
        NetworkAPI.RegisterExtensionMessageHandler("MailList", HandleMailList);
        NetworkAPI.RegisterExtensionMessageHandler("MailSent", HandleMailSent);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    #region Mail Reading Functions

    public void RequestMailList(Vector3 location) {
        RequestMailList();
        this.mailboxLocation = location;
    }

    public void RequestMailList()
    {
        Dictionary<string, object> props = new Dictionary<string, object>();
        NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "inventory.GET_MAIL", props);
    }

    public MailEntry GetMailEntry(int pos)
    {
        if (mailList.Count > pos)
        {
            return mailList[pos];
        }
        else
        {
            return null;
        }
    }
    
    public MailEntry GetMailEntryById(int id) {
    	foreach(MailEntry entry in mailList) {
    		if (entry.mailId == id) {
    			return entry;
    		}
    	}
    	return null;
    }

    public void SetMailRead(MailEntry mail)
    {
        mail.read = true;
        Dictionary<string, object> props = new Dictionary<string, object>();
		props.Add("mailID", mail.mailId);
        NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "inventory.MAIL_READ", props);
    }
    
	public void ReturnMail(MailEntry mail)
	{
		Dictionary<string, object> props = new Dictionary<string, object>();
		props.Add("mailID", mail.mailId);
		NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "inventory.RETURN_MAIL", props);
	}

    public void DeleteMail(MailEntry mail)
    {
        Dictionary<string, object> props = new Dictionary<string, object>();
		props.Add("mailID", mail.mailId);
        NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "inventory.DELETE_MAIL", props);
    }

    public void TakeMailItem(MailEntry mail, int itemNum)
    {
        Dictionary<string, object> props = new Dictionary<string, object>();
		props.Add("mailID", mail.mailId);
        props.Add("itemPos", itemNum);
        NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "inventory.MAIL_TAKE_ITEM", props);
        //ClientAPI.Write("Send take mail command with mail ID: " + mail.mailId);
    }

    #endregion Mail Reading Functions

    #region Mail Writing Functions

    public void StartComposingMail()
    {
        StartComposingMail("");
    }

    public void StartComposingMail(string recipient)
    {
        mailBeingComposed = new MailEntry();
        mailBeingComposed.senderName = recipient;
        List<MailItem> items = new List<MailItem>();
        for (int i = 0; i < itemLimit; i++)
        {
            items.Add(new MailItem());
        }
        mailBeingComposed.items = items;
    }

	public void SetMailItem(int gridPos, AtavismInventoryItem item) {

		if (item == null) {
            if (mailBeingComposed.items[gridPos].item != null)
                mailBeingComposed.items[gridPos].item.AlterUseCount(-mailBeingComposed.items[gridPos].count);
            mailBeingComposed.items[gridPos].item = null;
            mailBeingComposed.items[gridPos].count = 1;
        }
        else if (mailBeingComposed.items[gridPos].item == item)
        {
            mailBeingComposed.items[gridPos].count++;
		} else {
            mailBeingComposed.items[gridPos].item = item;
            mailBeingComposed.items[gridPos].count = 1;
		}
		
		if (item != null)
			item.AlterUseCount(1);
	}

    public void SendMail()
    {
        Dictionary<string, object> props = new Dictionary<string, object>();
        props.Add("recipient", mailBeingComposed.senderName);
        props.Add("subject", mailBeingComposed.subject);
        props.Add("message", mailBeingComposed.message);
        int itemPos = 0;
        foreach (MailItem mailItem in mailBeingComposed.items)
        {
            if (mailItem.item != null)
            {
                props.Add("item" + itemPos, mailItem.item.ItemId);
            }
            else
            {
                props.Add("item" + itemPos, null);
            }
			itemPos++;
        }

        props.Add("numItems", mailBeingComposed.items.Count);
        props.Add("currencyType", mailBeingComposed.currencyType);
        props.Add("currencyAmount", mailBeingComposed.currencyAmount);
        props.Add("CoD", mailBeingComposed.cashOnDelivery);
        NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "inventory.SEND_MAIL", props);
    }

    #endregion Mail Writing Functions

    public void HandleMailList(Dictionary<string, object> props) {
        mailList = new List<MailEntry>();
        int numMail = (int)props["numMail"];
        for (int i = 0; i < numMail; i++) {
            MailEntry entry = new MailEntry();
            entry.mailId = (int)props["mail_" + i + "ID"];
            entry.senderOid = (OID)props["mail_" + i + "SenderOid"];
            entry.senderName = (string)props["mail_" + i + "SenderName"];
            entry.subject = (string)props["mail_" + i + "Subject"];
            entry.message = (string)props["mail_" + i + "Message"];
            List<MailItem> items = new List<MailItem>();
            //TODO: put item reading code here
            int numItems = (int)props["mail_" + i + "NumItems"];
            for (int j = 0; j < numItems; j++)
            {
				MailItem mailItem = new MailItem();
				int itemTemplate = (int)props["mail_" + i + "ItemTemplate" + j];
				if (itemTemplate > 0) {
					mailItem.item = ClientAPI.ScriptObject.GetComponent<Inventory>().GetItemByTemplateID(itemTemplate);
					mailItem.count = (int)props["mail_" + i + "ItemCount" + j];
				}
				items.Add(mailItem);
            }
            entry.items = items;
            entry.currencyType = (int)props["mail_" + i + "CurrencyType"];
            entry.currencyAmount = (int)props["mail_" + i + "CurrencyAmount"];
            entry.cashOnDelivery = (bool)props["mail_" + i + "CoD"];
            entry.read = (bool)props["mail_" + i + "Read"];
            mailList.Add(entry);
        }

        //dispatch an event to tell the rest of the system
        string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("MAIL_UPDATE", args);
    }

    public void HandleMailSent(Dictionary<string, object> props) {
        //dispatch an event to tell the rest of the system
        string[] args = new string[1];
		AtavismEventSystem.DispatchEvent("MAIL_SENT", args);
		
		// Send announcement message
		args = new string[1];
		args[0] = "Mail Sent";
		AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
    }

    #region Properties
    public MailEntry MailBeingComposed
    {
        get
        {
            return mailBeingComposed;
        }
    }

    public List<MailEntry> MailList
    {
        get
        {
            return mailList;
        }
    }

    public Vector3 MailboxLocation
    {
        get
        {
            return mailboxLocation;
        }
    }
    #endregion Properties
}
