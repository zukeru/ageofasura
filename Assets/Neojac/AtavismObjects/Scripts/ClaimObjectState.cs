using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClaimObjectState : MonoBehaviour
{
	
	public List<GameObject> coordEffects;
	string currentState = "";
	int claimID = -1;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnMouseDown ()
	{
		int nextPos = 0;
		foreach (GameObject coordEffect in coordEffects) {
			nextPos++;
			if (coordEffect.name == currentState || currentState == "") {
				if (nextPos == coordEffects.Count) {
					nextPos = 0;
				}
				ClaimObject cObject = GetComponent<ClaimObject>();
				if (cObject == null) {
					// If the ClaimObject component isn't on the object try get it from the parent
					cObject = GetComponentInParent<ClaimObject>();
				}
				//currentState = coordEffects[nextPos].name;
				Dictionary<string, object> props = new Dictionary<string, object>();
				props.Add("action", "state");
				props.Add("claimID", claimID);
				props.Add("objectID", cObject.ID);
				props.Add("state", coordEffects[nextPos].name);
				NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);
				return;
			}
		}
	}
	
	public void StateUpdated (string state) {
		if (state == null || state == "null" || state == currentState)
			return;
		currentState = state;
		Dictionary<string, object> props = new Dictionary<string, object>();
		props["gameObject"] = gameObject;
		CoordinatedEffectSystem.ExecuteCoordinatedEffect(currentState, props);
	}
	
	public void SetID(int ID) 
	{
		claimID = ID;
	}
}
