using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AttachmentSockets {
	Root,
	LeftFoot,
	RightFoot,
	Pelvis,
	LeftHip,
	RightHip,
	MainHand,
	OffHand,
	Chest,
	Back,
	LeftShoulder,
	RightShoulder,
	Head,
	Mouth,
	LeftEye,
	RightEye,
	Overhead
}

public class AtavismMobAppearance : MonoBehaviour {
	
	GameObject legs;
	GameObject chest;
	GameObject hands;
	GameObject feet;
	
	// Sockets for attaching weapons (and particles)
	public Transform mainHand;
	public Transform offHand;
	public Transform mainHandRest;
	public Transform offHandRest;
	public Transform head;
	public Transform leftShoulderSocket;
	public Transform rightShoulderSocket;
	
	// Sockets for particles
	public Transform rootSocket;
	public Transform leftFootSocket;
	public Transform rightFootSocket;
	public Transform pelvisSocket;
	public Transform leftHipSocket;
	public Transform rightHipSocket;
	public Transform chestSocket;
	public Transform backSocket;
	public Transform mouthSocket;
	public Transform leftEyeSocket;
	public Transform rightEyeSocket;
	public Transform overheadSocket;
	
	
	Dictionary<Transform, GameObject> attachedItems = new Dictionary<Transform, GameObject>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public Transform GetSocketTransform(AttachmentSockets slot) {
		switch (slot) {
		case AttachmentSockets.MainHand:
			return mainHand;
		case AttachmentSockets.OffHand:
			return offHand;
		case AttachmentSockets.Head:
			return head;
		case AttachmentSockets.LeftShoulder:
			return leftShoulderSocket;
		case AttachmentSockets.RightShoulder:
			return rightShoulderSocket;
		case AttachmentSockets.Root:
			return transform;
		case AttachmentSockets.LeftFoot:
			return leftFootSocket;
		case AttachmentSockets.RightFoot:
			return rightFootSocket;
		case AttachmentSockets.Pelvis:
			return pelvisSocket;
		case AttachmentSockets.LeftHip:
			return leftHipSocket;
		case AttachmentSockets.RightHip:
			return rightHipSocket;
		case AttachmentSockets.Chest:
			return chestSocket;
		case AttachmentSockets.Back:
			return backSocket;
		case AttachmentSockets.Mouth:
			return mouthSocket;
		case AttachmentSockets.LeftEye:
			return leftEyeSocket;
		case AttachmentSockets.RightEye:
			return rightEyeSocket;
		case AttachmentSockets.Overhead:
			return overheadSocket;
		}
		return null;
	}
	
	void OnDestroy() {
		if (GetComponent<AtavismNode>()) {
			GetComponent<AtavismNode> ().RemoveObjectPropertyChangeHandler("weaponDisplayID", WeaponDisplayHandler);
			GetComponent<AtavismNode> ().RemoveObjectPropertyChangeHandler("weapon2DisplayID", Weapon2DisplayHandler);
		}
	}
	
	void ObjectNodeReady () {
		GetComponent<AtavismNode> ().RegisterObjectPropertyChangeHandler ("weaponDisplayID", WeaponDisplayHandler);
		GetComponent<AtavismNode> ().RegisterObjectPropertyChangeHandler ("weapon2DisplayID", Weapon2DisplayHandler);
		if (GetComponent<AtavismNode>().PropertyExists("weaponDisplayID")) {
			Debug.LogWarning("Got weapon display for: " + name);
			string displayID = (string)GetComponent<AtavismNode> ().GetProperty ("weaponDisplayID");
			SetWeaponDisplay(displayID);
		}
		if (GetComponent<AtavismNode>().PropertyExists("weapon2DisplayID")) {
			Debug.LogWarning("Got weapon2 display for: " + name);
			string displayID = (string)GetComponent<AtavismNode> ().GetProperty ("weapon2DisplayID");
			SetWeapon2Display(displayID);
		}
		Debug.LogWarning("Registered display properties for: " + name);
	}
	
	public void WeaponDisplayHandler(object sender, PropertyChangeEventArgs args) {
		UnityEngine.Debug.Log("Got weapon display ID");
		//ObjectNode node = (ObjectNode)sender;
		string displayID = (string)GetComponent<AtavismNode> ().GetProperty (args.PropertyName);
		SetWeaponDisplay(displayID);
	}

	public void SetWeaponDisplay(string displayID) {
		// Remove existing item
		if (attachedItems.ContainsKey(mainHand)) {
			Destroy(attachedItems[mainHand]);
			attachedItems.Remove(mainHand);
		}
		if (displayID != null && displayID != "") {
			EquipmentDisplay display = ClientAPI.ScriptObject.GetComponent<Inventory>().LoadEquipmentDisplay(displayID);
			GameObject weapon = (GameObject) Instantiate(display.model, mainHand.position, mainHand.rotation);
			weapon.transform.parent = mainHand;
			attachedItems.Add(mainHand, weapon);
		}
	}
	
	public void Weapon2DisplayHandler(object sender, PropertyChangeEventArgs args) {
		UnityEngine.Debug.Log("Got weapon 2 display ID");
		AtavismObjectNode node = (AtavismObjectNode)sender;
		string displayID = (string)GetComponent<AtavismNode> ().GetProperty (args.PropertyName);
		SetWeapon2Display(displayID);
	}

	public void SetWeapon2Display(string displayID) {
		// Remove existing item
		if (attachedItems.ContainsKey(offHand)) {
			Destroy(attachedItems[offHand]);
			attachedItems.Remove(offHand);
		}
		if (displayID != null && displayID != "") {
			EquipmentDisplay display = ClientAPI.ScriptObject.GetComponent<Inventory>().LoadEquipmentDisplay(displayID);
			GameObject weapon2 = (GameObject) Instantiate(display.model, offHand.position, offHand.rotation);
			weapon2.transform.parent = offHand;
			attachedItems.Add(offHand, weapon2);
		}
	}
}
