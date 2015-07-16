using UnityEngine;
using System.Collections;

public class Mailbox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		ClientAPI.ScriptObject.GetComponent<Mailing>().RequestMailList(transform.position);
	}
}
