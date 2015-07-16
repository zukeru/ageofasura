using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MobSoundEvent {
	Response,
	Death,
	Aggro,
	Attack,
}

public class MobSoundSet : MonoBehaviour {

	public List<AudioClip> responseSound;
	public List<AudioClip> deathSound;
	public List<AudioClip> aggroSound;
	public List<AudioClip> attackSound;
	Random rand = new Random();

	// Use this for initialization
	void Start () {
	
	}
	
	void ObjectNodeReady ()
	{
		GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler ("deadstate", HandleDeadState);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void PlaySoundEvent(MobSoundEvent soundEvent) {
		// Play sound clip on the mob
		Transform slotTransform = GetComponent<AtavismMobAppearance>().GetSocketTransform(AttachmentSockets.Root);
		GameObject soundObject = new GameObject();
		soundObject.transform.position = slotTransform.position;
		soundObject.transform.parent = slotTransform;
		float duration = 2;
		
		if (soundEvent == MobSoundEvent.Aggro) {
			if (aggroSound.Count > 0) {
				AudioSource audioSource = soundObject.AddComponent<AudioSource>();
				int soundChoice = Random.Range(0, aggroSound.Count);
				audioSource.clip = aggroSound[soundChoice];
				audioSource.Play();
			}
		} else if (soundEvent == MobSoundEvent.Attack) {
			if (attackSound.Count > 0) {
				AudioSource audioSource = soundObject.AddComponent<AudioSource>();
				int soundChoice = Random.Range(0, attackSound.Count);
				audioSource.clip = attackSound[soundChoice];
				audioSource.Play();
			}
		} else if (soundEvent == MobSoundEvent.Death) {
			if (deathSound.Count > 0) {
				AudioSource audioSource = soundObject.AddComponent<AudioSource>();
				int soundChoice = Random.Range(0, deathSound.Count);
				audioSource.clip = deathSound[soundChoice];
				audioSource.Play();
			}
		} else if (soundEvent == MobSoundEvent.Response) {
			if (responseSound.Count > 0) {
				AudioSource audioSource = soundObject.AddComponent<AudioSource>();
				int soundChoice = Random.Range(0, responseSound.Count);
				audioSource.clip = responseSound[soundChoice];
				audioSource.Play();
			}
		}
		Destroy(soundObject, duration);
	}
	
	public void HandleDeadState (object sender, PropertyChangeEventArgs args)
	{
		//Debug.Log ("Got dead update: " + oid);
		long oid = GetComponent<AtavismNode>().Oid;
		bool dead = (bool)AtavismClient.Instance.WorldManager.GetObjectNode(oid).GetProperty("deadstate");
		if (dead) {
			PlaySoundEvent(MobSoundEvent.Death);
		}
	}
}
