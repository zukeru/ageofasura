using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LoginController : MonoBehaviour {
	
	public GameObject soundMenu;
	public GameObject musicObject;
	public GameObject dialogWindow;
	
	enum LoginState
	{
		Login,
		Authenticating,
		CharacterSelect,
		CharacterCreate
	}
	
	#region fields
	LoginState loginState;
	
	// Login fields
	public string username = "";
	public string password = "";
	
	// Registration fields
	string password2 = "";
	string email = "";
	string email2 = "";

	string dialogMessage = "";
	string errorMessage = "";
	
	#endregion fields

	// Use this for initialization
	void Start () {
		loginState = LoginState.Login;
		AtavismEventSystem.RegisterEvent("LOGIN_RESPONSE", this);
		AtavismEventSystem.RegisterEvent("REGISTER_RESPONSE", this);
		// Play music
		SoundSystem.LoadSoundSettings();
		SoundSystem.PlayMusic(musicObject.GetComponent<AudioSource>());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetUserName(string username) {
		this.username = username;
	}
	
	public void SetPassword(string password) {
		this.password = password;
	}
	
	public void SetPassword2(string password2) {
		this.password2 = password2;
	}
	
	public void SetEmail(string email) {
		this.email = email;
	}
	
	public void SetEmail2(string email2) {
		this.email2 = email2;
	}
	
	public void Login() {
		AtavismClient.Instance.Login(username, AtavismEncryption.Md5Sum(password));
	}
	
	public void Register() {
		if (username == "") {
			ShowDialog("Please enter a username", true);
			return;
		}
		if (username.Length < 4) {
			ShowDialog("Your username must be at least 4 characters long", true);
			return;
		}
		foreach(char chr in username) {
			if ( (chr < 'a' || chr > 'z') && (chr < 'A' || chr > 'Z') && (chr < '0' || chr > '9') ) {
				ShowDialog("Your username can only contain letters and numbers", true);
				return;
			}
		}
		if (password == "") {
			ShowDialog("Please enter a password", true);
			return;
		}
		foreach(char chr in password) {
			if (chr == '*' || chr == '\'' || chr == '"' || chr == '/' || chr == '\\' || chr == ' ') {
				ShowDialog("Your password cannot contain * \' \" / \\ or spaces", true);
				return;
			}
		}
		if (password.Length < 6) {
			ShowDialog("Your password must be at least 6 characters long", true);
			return;
		}
		if (password != password2) {
			ShowDialog("Your passwords must match", true);
			return;
		}
		if (email == "") {
			ShowDialog("Please enter an email address", true);
			return;
		}
		if (!ValidateEmail(email)) {
			ShowDialog("Please enter a valid email address", true);
			return;
		}
		if (email != email2) {
			ShowDialog("Your email addresses must match", true);
			return;
		}
		AtavismClient.Instance.CreateAccount(username, AtavismEncryption.Md5Sum(password), email);
	}
	
	private bool ValidateEmail (string email)
	{
		Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
		Match match = regex.Match (email);
		if (match.Success)
			return true;
		else
			return false;
	}
	
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "LOGIN_RESPONSE") {
			dialogMessage = "";
			if (eData.eventArgs[0] == "Success") {
				Application.LoadLevel("CharacterSelection");
			} else {
				errorMessage = eData.eventArgs[0];
				
			}
		} else if (eData.eventType == "REGISTER_RESPONSE") {
			dialogMessage = "";
			if (eData.eventArgs[0] == "Success") {
				loginState = LoginState.Login;
				errorMessage = "Account created. You can now log in";
			} else {
				errorMessage = eData.eventArgs[0];
			}
		}
	}
	
	void ShowDialog(string message, bool showButton) {
		if (dialogWindow == null)
			return;
		dialogWindow.SetActive(true);
		dialogWindow.SendMessage("SetMessage", message);
		if (showButton) {
			dialogWindow.SendMessage("ShowButton");
		}
		/*string[] args = new string[2];
		args[0] = message;
		args[1] = showButton.ToString();
		EventSystem.DispatchEvent("SHOW_DIALOG", args);*/
	}
}
