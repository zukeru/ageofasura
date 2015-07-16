using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LoginUI : MonoBehaviour
{
	public List<GameObject> characterPrefabs;
	public List<string> races;
	public List<string> classes;
	int selectedPrefab = 0;
	public GUISkin skin;
	public GameObject soundMenu;
	public GameObject musicObject;
	public Transform spawnMarker;
	public bool useSaltedPasswords;
	GameObject character;
	List<CharacterEntry> characterEntries;
	
	enum LoginState
	{
		Login,
		Register,
		Authenticating,
		CharacterSelect,
		CharacterCreate
	}
	
	enum CreationState
	{
		Body,
		Head
	}
	
	#region fields
	LoginState loginState;
	CreationState creationState;
	
	// Login fields
	string username = "";
	string password = "";
	
	// Registration fields
	string password2 = "";
	string email = "";
	string email2 = "";
	
	// Character select fields
	CharacterEntry characterSelected = null;
	string characterName = "";
	string race = "Human";
	string gender = "Male";
	string aspect;
	
	string dialogMessage = "";
	string errorMessage = "";
	
	#endregion fields

	// Use this for initialization
	void Start ()
	{
		loginState = LoginState.Login;
		AtavismEventSystem.RegisterEvent("LOGIN_RESPONSE", this);
		AtavismEventSystem.RegisterEvent("REGISTER_RESPONSE", this);

		aspect = classes[0];
		if (races.Count > 0) {
			race = races[0];
		} else {
			races.Add("Human");
		}

		// Play music
		SoundSystem.LoadSoundSettings();
		if (musicObject != null)
			SoundSystem.PlayMusic(musicObject.GetComponent<AudioSource>());
	}
	
	void OnDestroy() {
		AtavismEventSystem.UnregisterEvent("LOGIN_RESPONSE", this);
		AtavismEventSystem.UnregisterEvent("REGISTER_RESPONSE", this);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnGUI ()
	{
		GUI.depth = 3;
		GUI.skin = skin;
		
		if (errorMessage != "" || dialogMessage != "") {
			GUI.enabled = false;
		}
		
		if (loginState == LoginState.Login || loginState == LoginState.Authenticating) {
			DrawLoginUI();
			//DrawSoundMenuButton();
		} else if (loginState == LoginState.Register) {
			DrawRegisterUI();
		} else if (loginState == LoginState.CharacterSelect) {
			DrawCharacterSelectUI();
			if (characterSelected != null) {
				int width = 200;
				int left = (Screen.width - width) / 2;
				int height = 40;
				int top = Screen.height - height - 10;
				float stringWidth = GUI.skin.GetStyle("label").CalcSize(new GUIContent((string)characterSelected["characterName"])).x;
				GUI.Label(new Rect((Screen.width / 2) - (stringWidth / 2), top - 20, stringWidth, height), (string)characterSelected["characterName"]);
				if (GUI.Button(new Rect(left, top, width, height), "Enter")) {
					dialogMessage = "Entering World...";
					AtavismClient.Instance.EnterGameWorld(characterSelected.CharacterId);
				}
			}
		} else if (loginState == LoginState.CharacterCreate) {
			//GUI.Window(1, new Rect(10, 100, 200, 300), DrawCharacterCreateUI, "");
			DrawCharacterCreateUI();
		}
		
		GUI.enabled = true;
		
		if (errorMessage != "") {
			DrawErrorUI();
		} else if (dialogMessage != "") {
			DrawDialogUI();
		}
	}
	
	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "LOGIN_RESPONSE") {
			dialogMessage = "";
			if (eData.eventArgs[0] == "Success") {
				StartCharacterSelection();
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
	
	Rect CreateCenteredRect (int width, int height)
	{
		int left = (Screen.width - width) / 2;
		int top = (Screen.height - height) / 2;
		Rect centeredRect = new Rect (left, top, width, height);
		return centeredRect;
	}
	
	void DrawLoginUI ()
	{
		GUILayout.BeginArea(CreateCenteredRect (220, 150), skin.GetStyle("Window"));
		GUILayout.Label ("Username:");
		username = GUILayout.TextField (username);
		GUILayout.Label("Password:");
		password = GUILayout.PasswordField(password, '*');
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Create Account")) {
			loginState = LoginState.Register;
		}
		GUILayout.Space(20);
		if (GUILayout.Button("Login")) {
			dialogMessage = "Logging in...";
			// Verify username and password are entered, then pass them to the client
			// Also convert password to md5
			if (useSaltedPasswords) {
				AtavismClient.Instance.Login(username, password);
			} else {
				AtavismClient.Instance.Login(username, AtavismEncryption.Md5Sum(password));
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	void DrawRegisterUI ()
	{
		GUILayout.BeginArea(CreateCenteredRect (220, 360), skin.GetStyle("Window"));
		GUILayout.Label ("Create Account");
		GUILayout.Label ("Username:");
		username = GUILayout.TextField (username);
		GUILayout.Label("Password:");
		password = GUILayout.PasswordField(password, '*');
		GUILayout.Label("Confirm Password:");
		password2 = GUILayout.PasswordField(password2, '*');
		GUILayout.Label("Email:");
		email = GUILayout.TextField (email);
		GUILayout.Label("Confirm Email:");
		email2 = GUILayout.TextField (email2);
		if (GUILayout.Button("Create")) {
			Register();
		}
		if (GUILayout.Button("Cancel")) {
			loginState = LoginState.Login;
		}
		GUILayout.EndArea();
	}
	
	void DrawCharacterSelectUI ()
	{
		GUILayout.BeginArea(new Rect(10, 100, 200, 300), skin.GetStyle("Window"));
		GUILayout.Label("Characters:");
		foreach (CharacterEntry charEntry in characterEntries) {
			if (GUILayout.Button((string)charEntry["characterName"])) {
				CharacterSelected(charEntry);
			}
		}
		GUI.skin = skin;
		if (GUILayout.Button("Create")) {
			StartCharacterCreation();
		}
		if (GUILayout.Button("Delete")) {
			Dictionary<string, object> attrs = new Dictionary<string, object>();
			attrs.Add("characterId", characterSelected.CharacterId);
			NetworkAPI.DeleteCharacter(attrs);
		}
		GUILayout.EndArea();
	}
	
	void DrawCharacterCreateUI ()
	{
		GUILayout.BeginArea(new Rect(10, 100, 200, Screen.height - 100), skin.GetStyle("Window"));
		GUILayout.Label("Model:");
		/*GUILayout.BeginHorizontal();
		if (GUILayout.Button("Male")) {
			gender = "Male";
			//SetRace("HumanMale");
			selectedPrefab = 0;
		} else if (GUILayout.Button("Female")) {
			gender = "Female";
            //SetRace("HumanFemale");	
			selectedPrefab = 1;
		}
		GUILayout.EndHorizontal();*/
		for (int i = 0; i < characterPrefabs.Count; i++) {
			if (GUILayout.Button(characterPrefabs[i].name)) {
				SetCharacter(characterPrefabs[i]);
				selectedPrefab = i;
			}
		}
		
		GUILayout.Label("Race:");
		for (int i = 0; i < races.Count; i++) {
			if (GUILayout.Button(races[i])) {
				race = races[i];
			}
		}
		
		GUILayout.Label("Class:");
		for (int i = 0; i < classes.Count; i++) {
			if (GUILayout.Button(classes[i])) {
				aspect = classes[i];
			}
		}
		
		GUILayout.Label("Name:");
		characterName = GUILayout.TextField(characterName);
		if (GUILayout.Button("Create")) {
			Dictionary<string, object> properties = new Dictionary<string, object>();
			// Core properties - these are needed
			properties.Add("characterName", characterName);
			properties.Add("prefab", characterPrefabs[selectedPrefab].name);
			properties.Add("race", race);
			properties.Add("aspect", aspect);
			properties.Add("gender", gender);
			
			// Custom properties - add whatever you want
			/*properties.Add ("custom:test", 27);
			properties.Add ("custom:umaData:floatVal", 1.5f);
			properties.Add ("custom:umaData:stringVal", "hi");
			properties.Add ("custom:umaData:intVal", 7);*/
			
			// End custom property adding
			
			dialogMessage = "Please wait...";
			CharacterEntry entry = AtavismClient.Instance.NetworkHelper.CreateCharacter(properties);
			if (entry == null) {
				errorMessage = "Unknown Error";
			} else {
				if (!entry.Status) {
					if (entry.ContainsKey("errorMessage")) {
						errorMessage = (string)entry["errorMessage"];
					}
				}
			}
			dialogMessage = "";
			if (errorMessage == "") {
				loginState = LoginState.CharacterSelect;
				characterSelected = entry;
			}
		}
		if (GUILayout.Button("Cancel")) {
			CancelCharacterCreation();
			StartCharacterSelection();
		}
		GUILayout.EndArea();
		
		// Customisation Window
		/*GUILayout.BeginArea(new Rect(Screen.width - 210, 50, 200, 100), skin.GetStyle("Window"));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Body")) {
			creationState = CreationState.Body;
		} else if (GUILayout.Button("Head")) {
			creationState = CreationState.Head;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();*/
	}

	void DrawSoundMenuButton() {
		GUILayout.BeginArea(new Rect(Screen.width - 260, Screen.height - 100, 250, 50), skin.GetStyle("Window"));
		//GUILayout.BeginArea(new Rect(/*Screen.width - */150,/* Screen.height -*/ 40, 140, 30), skin.GetStyle("Window"));
		//GUILayout.Label("Name:");
		if (GUILayout.Button("Sound Options")) {
			soundMenu.SetActive(true);
			soundMenu.GetComponent<SoundMenu>().PreviousMenu = gameObject;
			gameObject.SetActive(false);
		}
		GUILayout.EndArea();
	}
	
	void DrawDialogUI() {
		GUILayout.BeginArea(CreateCenteredRect (350, 200), skin.GetStyle("Window"));
		GUILayout.Label(dialogMessage);
		GUILayout.EndArea();
	}
	
	void DrawErrorUI() {
		GUILayout.BeginArea(CreateCenteredRect (350, 200), skin.GetStyle("Window"));
		GUILayout.Label(errorMessage);
		if (GUILayout.Button("Okay")) {
			errorMessage = "";
		}
		GUILayout.EndArea();
	}
	
	void SetCharacter(GameObject prefab) {
		if (character != null) {
			Destroy(character);
		}
		character = (GameObject)UnityEngine.Object.Instantiate (prefab, spawnMarker.position, spawnMarker.rotation);
		// Hard-coded equipment property handlers. Not the ideal way to do this
		if (characterSelected != null && characterSelected.ContainsKey("weaponDisplayID")) {
			character.GetComponent<AtavismMobAppearance>().SetWeaponDisplay((string)characterSelected["weaponDisplayID"]);
		}
		this.character = character;
		
	}
	
	#region Create Account
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
	#endregion
	
	#region Character Selection
	void StartCharacterSelection() {
		characterEntries = ClientAPI.GetCharacterEntries();
		if (characterEntries.Count > 0) {
			CharacterSelected(characterEntries[0]);
		}
		loginState = LoginState.CharacterSelect;
	}
	
	void CharacterSelected(CharacterEntry entry) {
		characterSelected = entry;
		string prefabName = (string)characterSelected ["model"];
		GameObject prefab = (GameObject)Resources.Load (prefabName);
		SetCharacter(prefab);
	}
	#endregion Character Selection
	
	#region Character Creation
	void StartCharacterCreation() {
		if (character != null)
			Destroy(character);
		characterName = "";
		int randomResult = Random.Range(0, 2);
		if(randomResult == 0)
        {
            gender = "Male";
		}else{
            gender = "Female";
        } 
		//SetCharacter(npcAppearance.GenerateRandomUMA(race + gender));
		SetCharacter(characterPrefabs[selectedPrefab]);
		loginState = LoginState.CharacterCreate;
		creationState = CreationState.Body;
	}
	
	void SetRace(string race) {
		//SetCharacter(npcAppearance.SetRace(character, race, umaDna));
	}
	
	void CancelCharacterCreation() {
		Destroy(character);
		/*if (characterSelected != null) {
			race = (string)characterSelected["umaRace"];
			gender = (string)characterSelected["umaGender"];
			//SetCharacter(npcAppearance.GenerateRandomUMA(race + gender));
		}*/
	}
	#endregion Character Creation
	
	void ShowDialog(string message, bool showButton) {
		if (showButton) {
			errorMessage = message;
			dialogMessage = "";
		} else {
			dialogMessage = message;
			errorMessage = "";
		}
		/*dialogUI.SetActive(true);
		dialogString.text = message;
		dialogButton.SetActive(showButton);*/
	}
}
