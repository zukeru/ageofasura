using UnityEngine;
using System.Collections;

public class AtavismUIManager : MonoBehaviour {

	static AtavismUIManager instance;

    public GameObject confirmationUI;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowConfirmationBox(string message, object confirmObject, GameObject responseTarget, string responseMethod)
    {
        confirmationUI.SendMessage("SetMessage", message);
        if (confirmObject == null)
        	confirmObject = new object();
        confirmationUI.SendMessage("SetObject", confirmObject);
        confirmationUI.SendMessage("SetResponseTarget", responseTarget);
        confirmationUI.SendMessage("SetResponseMethod", responseMethod);
        confirmationUI.SendMessage("Show");
    }
	
	public static AtavismUIManager Instance {
		get {
			return instance;
		}
	}
}
