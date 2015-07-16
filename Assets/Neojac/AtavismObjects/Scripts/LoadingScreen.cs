using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour {

	public Texture2D texture;
	public int loadingScreenExtraDuration = 0;
	
	public static bool sceneReady = true;
	float loadingScreenExpiry = -1;

	// Use this for initialization
	void Start () {
		AtavismEventSystem.RegisterEvent("LOADING_SCENE_START", this);
		AtavismEventSystem.RegisterEvent("LOADING_SCENE_END", this);
		AtavismEventSystem.RegisterEvent("PLAYER_TELEPORTED", this);
	}
	
	// Update is called once per frame
	void Update () {
		if (loadingScreenExpiry < Time.time) {
			sceneReady = true;
		}
	}
	
	void OnGUI() 
	{
		if (!sceneReady) {
			GUI.depth = 1;
			Rect rect = new Rect(0, 0, Screen.width, Screen.height);
			GUI.DrawTexture(rect, texture);
		}
	}

	void OnLevelWasLoaded (int level) {
		//guiTexture.enabled = false;
		loadingScreenExpiry = Time.time + loadingScreenExtraDuration;
	}

	public void OnEvent(AtavismEventData eData) {
		if (eData.eventType == "LOADING_SCENE_START") {
			sceneReady = false;
			AtavismLogger.LogDebugMessage("Showing loading screen");
		}
		if (eData.eventType == "LOADING_SCENE_END") {
			//showLoadingScreen = false;
			GetComponent<GUITexture>().enabled = false;
			AtavismLogger.LogDebugMessage("Hiding loading screen");
		}
		if (eData.eventType == "PLAYER_TELEPORTED") {
			AtavismLogger.LogDebugMessage("Got player teleport");
			sceneReady = false;
			loadingScreenExpiry = Time.time + loadingScreenExtraDuration;
		}
	}
}
