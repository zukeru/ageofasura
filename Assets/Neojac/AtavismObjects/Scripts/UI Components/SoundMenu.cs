using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundMenu : MonoBehaviour
{
	
	public GUISkin skin;
	GameObject previousMenu;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	
	void OnGUI ()
	{
		GUI.skin = skin;
		//GUI.Box(new Rect(originalWidth / 2 - 100, originalHeight / 2 - 75, 200, 150), "");
		GUILayout.BeginArea (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 125, 200, 250));
		GUILayout.Label ("Music Volume:");
		GUILayout.BeginHorizontal ();
		SoundSystem.MusicVolume = GUILayout.HorizontalSlider (SoundSystem.MusicVolume, 0, 1, GUILayout.Width (100));
		GUILayout.Label (string.Format ("{0:P0}", SoundSystem.MusicVolume));
		GUILayout.EndHorizontal ();
		GUILayout.Label ("Sound Volume:");
		GUILayout.BeginHorizontal ();
		SoundSystem.SoundEffectVolume = GUILayout.HorizontalSlider (SoundSystem.SoundEffectVolume, 0, 1, GUILayout.Width (100));
		GUILayout.Label (string.Format ("{0:P0}", SoundSystem.SoundEffectVolume));
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Cancel")) {
			gameObject.SetActive(false);
			previousMenu.SetActive(true);
			SoundSystem.ResetSoundSettings ();
		}
		if (GUILayout.Button ("Save")) {
			gameObject.SetActive(false);
			previousMenu.SetActive(true);
			SoundSystem.SaveSoundSettings ();
		}
		GUILayout.EndHorizontal ();
		GUILayout.EndArea ();
	}
	
	public GameObject PreviousMenu {
		get {
			return previousMenu;
		}
		set {
			previousMenu = value;
		}
	}
}
