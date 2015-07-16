using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MailBoxStateEnum
{
	Menu,
	ReadMail,
	NewMail,
	ReplyMail,
	DeleteMail,
	None,
	Disabled
}

public class MailBoxUI : AtavismWindowTemplate{

	public KeyCode toggleButton; //use for debug
	public int buttonSize = 32;
    public int mailPerPage = 5;
    private int currentPage = 0;
	private int rowCount = 2; // For Mailing Items
	private int columnCount = 5;// For Mailing Items
	MailBoxStateEnum mailboxState;
    MailEntry viewingMail = null;
    int wantedItem = -1;

	// Use this for initialization
	void Start () {
		mailboxState = MailBoxStateEnum.Menu;
		SetupRect();
		
		height = (rowCount+1) * buttonSize + 120;
		width = columnCount * buttonSize + 24;

        // Register for 
		AtavismEventSystem.RegisterEvent("MAIL_UPDATE", this);
		AtavismEventSystem.RegisterEvent("MAIL_SENT", this);
	}

    void OnDestroy()
    {
		AtavismEventSystem.UnregisterEvent("MAIL_UPDATE", this);
		AtavismEventSystem.UnregisterEvent("MAIL_SENT", this);
    }
	
	void Update () {
		// Will be used until we have a MailBox Station
		if (Input.GetKeyDown(toggleButton)) {
			ToggleOpen();
            if (open)
            {
                ClientAPI.ScriptObject.GetComponent<Mailing>().RequestMailList();
            }
		}
        if (open)
        {
            Vector3 mailboxLoc = ClientAPI.ScriptObject.GetComponent<Mailing>().MailboxLocation;
            if (mailboxLoc != Vector3.zero)
            {
                if (Vector3.Distance(ClientAPI.GetPlayerObject().Position, mailboxLoc) > 5)
                {
                    ToggleOpen();
                }
            }
        }
		//Debug.Log ("mailboxState: " + mailboxState);
	}

	public void OnEvent(AtavismEventData eData)
    {
        if (eData.eventType == "MAIL_UPDATE")
        {
            // Update 
            if (!open) {
				ToggleOpen();
            }
            if (viewingMail != null) {
            	// Update the viewingMail
            	viewingMail = ClientAPI.ScriptObject.GetComponent<Mailing>().GetMailEntryById(viewingMail.mailId);
            }
        }
        else if (eData.eventType == "MAIL_SENT")
        {
            mailboxState = MailBoxStateEnum.Menu;
            ClearItems();
        }
    }
	
	void OnGUI (){
		if (!open)
			return;

		GUI.depth = uiLayer;
		GUI.Box(uiRect, "");
		GUILayout.BeginArea(new Rect(uiRect));

		// Top Bar
		GUILayout.BeginHorizontal();
		GUILayout.Label("Mailbox");
		GUILayout.FlexibleSpace();

		if (GUILayout.Button("X")) {
            ClearItems();
			ToggleOpen();
            mailboxState = MailBoxStateEnum.Menu;
		}
		GUILayout.EndHorizontal();

        if (ClientAPI.ScriptObject.GetComponent<Mailing>() == null)
        {
            Debug.Log("Wmail is null");
            return;
        }

		if (mailboxState == MailBoxStateEnum.Menu) {
			DrawMailMenu ();
		} else if (mailboxState == MailBoxStateEnum.ReadMail) {
			DrawReadMail ();
		} else if (mailboxState == MailBoxStateEnum.NewMail) {
			DrawMailNew ();
		} else {
			Debug.Log ("invalid state");
		}

		GUILayout.EndArea();
	}

	void DrawMailMenu() {

        GUILayout.Label("Inbox:");
		//GUILayout.Box ("Email Received goes here", GUILayout.Height(220));
        int startingItem = currentPage * mailPerPage;
        for (int i = 0; i < mailPerPage; i++)
        {
            GUILayout.BeginHorizontal();
            MailEntry entry = ClientAPI.ScriptObject.GetComponent<Mailing>().GetMailEntry(startingItem + i);
            if (entry != null) {
                if (GUILayout.Button(entry.senderName + " - " + entry.subject))
                {
                    viewingMail = entry;
                    if (!viewingMail.read)
                        ClientAPI.ScriptObject.GetComponent<Mailing>().SetMailRead(viewingMail);
                    mailboxState = MailBoxStateEnum.ReadMail;
                }
            }
            GUILayout.EndHorizontal();
		}

        GUILayout.BeginHorizontal();
        // Buttons to move between pages
        if (currentPage != 0)
        {
            if (GUI.Button(new Rect(uiRect.x + 3, uiRect.yMax - 23, 20, 20), ">"))
            {
                currentPage--;
            }
        }
        int maxMailCount = mailPerPage * (currentPage + 1);
        if (ClientAPI.ScriptObject.GetComponent<Mailing>().MailList.Count > maxMailCount)
        {
            if (GUI.Button(new Rect(uiRect.xMax - 23, uiRect.yMax - 23, 20, 20), ">"))
            {
                currentPage++;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();

        // Action Buttons
		GUILayout.BeginHorizontal();
		/*GUILayout.BeginVertical ();
		if (GUILayout.Button ("Read")) {
			mailboxState = MailBoxStateEnum.ReadMail;
		}
		GUILayout.EndVertical ();*/
		GUILayout.BeginVertical ();
		if (GUILayout.Button ("Write New Mail")) {
            ClientAPI.ScriptObject.GetComponent<Mailing>().StartComposingMail();
			mailboxState = MailBoxStateEnum.NewMail;
		}
		GUILayout.EndVertical ();
        GUILayout.FlexibleSpace();
		/*GUILayout.BeginVertical ();
		if (GUILayout.Button ("Delete")) {
			mailboxState = MailBoxStateEnum.DeleteMail;
		}
		GUILayout.EndVertical ();*/
		GUILayout.EndHorizontal();
		GUILayout.Space(10);
	}

    void DrawReadMail()
    {
        GUILayout.Label("From: " + viewingMail.senderName);
        GUILayout.Label("Subject: " + viewingMail.subject);
        GUILayout.Label("Message:");
        GUILayout.Label(viewingMail.message);

        List<MailItem> items = viewingMail.items;
        if (items == null)
        {
            Debug.Log("Mail Items is null");
            return;
        }

        GUILayout.Label("Attachments:");
        for (int i = 0; i < rowCount; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            for (int j = 0; j < columnCount; j++)
            {
                int itemPos = i * columnCount + j;
                if (items[itemPos].item != null)
                {
                    if (GUILayout.Button(items[itemPos].item.icon, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                    {
                        if (!AtavismCursor.Instance.CursorHasItem())
                        {
                            if (viewingMail.cashOnDelivery)
                            {
                                // Show confirmation box
								string currencyName = ClientAPI.ScriptObject.GetComponent<Inventory>().GetCurrencyName(viewingMail.currencyType);
								string message = "This mail has a Cash on Delivery amount of " + viewingMail.currencyAmount + " " + currencyName + ". Pay the CoD amount?";
								AtavismUIManager.Instance.ShowConfirmationBox(message, null, gameObject, "ConfirmedPayCoD");
                                wantedItem = itemPos;
                            }
                            else
                            {
                                // Send take item message
                                ClientAPI.ScriptObject.GetComponent<Mailing>().TakeMailItem(viewingMail, itemPos);
                            }
                        }
                    }
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.y = Screen.height - mousePosition.y;
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        if (viewingMail.currencyAmount > 0) {
			GUILayout.BeginHorizontal();
			if (viewingMail.cashOnDelivery) {
				GUILayout.Label("Cash on Delivery: " + viewingMail.currencyAmount + " " 
					+ ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.name);
			} else {
				GUILayout.Label(viewingMail.currencyAmount + " " + ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.name);
				if (GUILayout.Button("Take " + ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.name)) {
					ClientAPI.ScriptObject.GetComponent<Mailing>().TakeMailItem(viewingMail, -1);
				}
			}
			GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reply", GUILayout.Width(75)))
        {
            ClientAPI.ScriptObject.GetComponent<Mailing>().StartComposingMail(viewingMail.senderName);
            mailboxState = MailBoxStateEnum.NewMail;
        }
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Return", GUILayout.Width(75)))
		{
			ClientAPI.ScriptObject.GetComponent<Mailing>().ReturnMail(viewingMail);
			mailboxState = MailBoxStateEnum.Menu;
		}
        GUILayout.FlexibleSpace();
        if (!viewingMail.cashOnDelivery) {
        	if (GUILayout.Button("Delete", GUILayout.Width(75)))
        	{
            	ClientAPI.ScriptObject.GetComponent<Mailing>().DeleteMail(viewingMail);
            	mailboxState = MailBoxStateEnum.Menu;
        	}
        }
        GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(20);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
        if (GUILayout.Button("Back", GUILayout.Width(80)))
        {
            mailboxState = MailBoxStateEnum.Menu;
        }
		GUILayout.EndHorizontal();
    }

	void DrawMailNew() {
        MailEntry mailBeingComposed = ClientAPI.ScriptObject.GetComponent<Mailing>().MailBeingComposed;
		GUILayout.BeginHorizontal();
		GUILayout.Label ("To:", GUILayout.Width(60));
        mailBeingComposed.senderName = GUILayout.TextField(mailBeingComposed.senderName, 25, GUILayout.Width(120));
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("Subject:", GUILayout.Width(60));
        mailBeingComposed.subject = GUILayout.TextField(mailBeingComposed.subject, 50, GUILayout.Width(120));
		GUILayout.EndHorizontal ();

		GUILayout.BeginVertical ();
		GUILayout.Label ("Message:", GUILayout.Width(100));
        mailBeingComposed.message = GUILayout.TextArea(mailBeingComposed.message, 500, GUILayout.Height(120));
		GUILayout.EndVertical ();

        List<MailItem> items = mailBeingComposed.items;
        if (items == null)
        {
			Debug.Log("GridItem is null");
			return;
		}

        GUILayout.Label("Attachments:");
		for (int i = 0; i < rowCount; i++) {
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			for (int j = 0; j < columnCount; j++) {
				int itemPos = i * columnCount + j;
                if (items[itemPos].item != null)
                {
                    if (GUILayout.Button(items[itemPos].item.icon, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
                    {
                        if (AtavismCursor.Instance.CursorHasItem())
                        {
                            items[itemPos].item = AtavismCursor.Instance.GetCursorItem();
                        }
                        else
                        {
                            AtavismCursor.Instance.SetCursorItem(items[itemPos].item);
                            ClientAPI.ScriptObject.GetComponent<Mailing>().SetMailItem(itemPos, null);
                        }
			        }
					Vector3 mousePosition = Input.mousePosition;
					mousePosition.y = Screen.height - mousePosition.y;
				} else {
					if (GUILayout.Button("", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize))) {
						if (AtavismCursor.Instance.CursorHasItem()) {
							AtavismInventoryItem item = AtavismCursor.Instance.GetCursorItem();
                            ClientAPI.ScriptObject.GetComponent<Mailing>().SetMailItem(itemPos, item);
							AtavismCursor.Instance.ResetCursor();
						}
					}
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

        GUILayout.BeginHorizontal();
        GUILayout.Label("Amount To Send:");
        mailBeingComposed.currencyAmount = int.Parse(GUILayout.TextArea("" + mailBeingComposed.currencyAmount));
		mailBeingComposed.currencyType = ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.id;
        GUILayout.Box(ClientAPI.ScriptObject.GetComponent<Inventory>().mainCurrency.icon);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        mailBeingComposed.cashOnDelivery = !GUILayout.Toggle(!mailBeingComposed.cashOnDelivery, "Send Money");
        mailBeingComposed.cashOnDelivery = GUILayout.Toggle(mailBeingComposed.cashOnDelivery, "Cash on Delivery");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Send", GUILayout.Width(80)))
        {
			// Send the mail here
            ClientAPI.ScriptObject.GetComponent<Mailing>().SendMail();
		}
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Cancel", GUILayout.Width(80)))
        {
			mailboxState = MailBoxStateEnum.Menu;
            ClearItems();
		}
        GUILayout.EndHorizontal();
	}

    public void ConfirmedPayCoD()
    {
        // Send take item message
        ClientAPI.ScriptObject.GetComponent<Mailing>().TakeMailItem(viewingMail, wantedItem);
    }

    /// <summary>
    /// Clears the items currently set in the New Mail items list restoring
    /// the counts of the items in the inventory back to what they should be.
    /// </summary>
    void ClearItems()
    {
        if (ClientAPI.ScriptObject.GetComponent<Mailing>().MailBeingComposed != null)
        {
            foreach (MailItem mailItem in ClientAPI.ScriptObject.GetComponent<Mailing>().MailBeingComposed.items)
            {
                if (mailItem.item != null)
                    mailItem.item.ResetUseCount();
            }
        }
    }
}
	

