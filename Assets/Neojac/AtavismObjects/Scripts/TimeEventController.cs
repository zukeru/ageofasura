using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeEventController : MonoBehaviour {

	public List<GameObject> objectsToToggle;
	public int activateHour;
	public int activateMinute;
	public int deactivateHour;
	public int deactivateMinute;
	bool currentlyActive = false;
	bool firstRun = true;
	int frameSkip = 10;
	int frameCount = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		frameCount++;
		if (frameCount % frameSkip != 0)
			return;
			
		if (currentlyActive) {
			if (afterDeactivateTime() && (!afterActivateTime() || deactivateHour >= activateHour) || (!afterActivateTime() && (activateHour < deactivateHour))) {
				foreach(GameObject obj in objectsToToggle) {
					obj.SetActive(false);
				}
				currentlyActive = false;
			}
		} else {
			if (afterActivateTime() && (!afterDeactivateTime() || activateHour >= deactivateHour) || (!afterDeactivateTime() && (deactivateHour < activateHour))) {
				foreach(GameObject obj in objectsToToggle) {
					obj.SetActive(true);
				}
				currentlyActive = true;
			} else if (firstRun) {
				foreach(GameObject obj in objectsToToggle) {
					obj.SetActive(false);
				}
			}
			firstRun = false;
		}
	}
	
	bool afterActivateTime() {
		if (ClientAPI.ScriptObject.GetComponent<TimeManager>().Hour > activateHour 
		    || (ClientAPI.ScriptObject.GetComponent<TimeManager>().Hour == activateHour
		    && ClientAPI.ScriptObject.GetComponent<TimeManager>().Minute >= activateMinute)) {
		    return true;
		}
		return false;
	}
	
	bool afterDeactivateTime() {
		if (ClientAPI.ScriptObject.GetComponent<TimeManager>().Hour > deactivateHour 
			|| (ClientAPI.ScriptObject.GetComponent<TimeManager>().Hour == deactivateHour
			&& ClientAPI.ScriptObject.GetComponent<TimeManager>().Minute >= deactivateMinute)) {
			return true;
		}
		return false;
	}
	
}
