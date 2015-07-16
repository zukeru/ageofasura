using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicAreaTrigger : MonoBehaviour {
	public enum Trigger {
		Collide,
		Click
	}

	public enum TrackPlayMode {
		LoopAll,
		PlayAll,
		PlayOne
	}

	public Trigger trigger;
	public float delayInSeconds = 0;
	public List<AudioClip> clips;
	public float maxVolume = 1;
	public float fadeSpeed = 1;
	public float secondsBetweenTracks = 5;
	public TrackPlayMode playMode; 
	int currentTrack = -1;
	bool playing = false;
	float currentTrackEnd = -1;
	float nextTrackStart = -1;
	bool fadingIn = false;
	bool fadingOut = false;

	void Start () { 
		gameObject.AddComponent<AudioSource>();
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject == ClientAPI.GetPlayerObject().GameObject && !playing) {
			if (playMode == TrackPlayMode.PlayAll) 
				currentTrack = -1;
			StartNextTrack();
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.gameObject == ClientAPI.GetPlayerObject().GameObject && playing) {
			playing = false;
			fadingOut = true;
			fadingIn = false;
		}
	}

	void OnClick () { 

	}
	
	void Update() {
		if (playing) {
			if (fadingIn) {
				fadeIn();
			} else if (fadingOut) {
				fadeOut();
			}

			if (Time.time > currentTrackEnd && currentTrackEnd != -1) {
				fadeOut();
				currentTrackEnd = -1;
				if (playMode == TrackPlayMode.PlayOne || (playMode == TrackPlayMode.PlayAll && currentTrack == clips.Count-1)) {
					playing = false;
					return;
				} 
				nextTrackStart = Time.time + secondsBetweenTracks;
			}
			if (Time.time > nextTrackStart && nextTrackStart != -1) {
				StartNextTrack();
			}
		}
	}

	void StartNextTrack() {
		currentTrack++;
		if (currentTrack == clips.Count)
			currentTrack = 0;

		GetComponent<AudioSource>().clip = clips[currentTrack];
		GetComponent<AudioSource>().Play();
		GetComponent<AudioSource>().volume = 0;
		currentTrackEnd = Time.time + clips[currentTrack].length - 2;
		playing = true;
		fadingIn = true;
		fadingOut = false;
		nextTrackStart = -1;
	}
	
	void fadeIn() {
		if (GetComponent<AudioSource>().volume < maxVolume) {
			GetComponent<AudioSource>().volume += 0.1f * Time.deltaTime * fadeSpeed;
		} else {
			fadingIn = false;
		}
	}
	
	void fadeOut() {
		if(GetComponent<AudioSource>().volume > 0.1) {
			GetComponent<AudioSource>().volume -= 0.1f * Time.deltaTime * fadeSpeed;
		} else {
			fadingOut = false;
		}
	}
}
