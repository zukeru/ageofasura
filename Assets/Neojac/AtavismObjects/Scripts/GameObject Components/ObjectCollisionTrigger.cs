using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectCollisionTrigger : MonoBehaviour {
	public enum Trigger {
		Collide,
		Click
	}

	public Trigger trigger;
	public float delayInSeconds = 0;
	public List<GameObject> objects;

	void Start () { }

	void OnTriggerEnter (Collider other) {
		if (other.gameObject == ClientAPI.GetPlayerObject().GameObject) {
			EnterTrigger();
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.gameObject == ClientAPI.GetPlayerObject().GameObject) {
			LeaveTrigger();
		}
	}

	void OnClick () { 
		if (trigger == Trigger.Click) {
			EnterTrigger();
		}
	}
	
	void EnterTrigger() {
		foreach (GameObject go in objects) {
			go.SetActive(true);
		}
	}

	void LeaveTrigger() {
		foreach (GameObject go in objects) {
			go.SetActive(false);
		}
	}
}
