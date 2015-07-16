using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
	
	void OnGUI() {
		float yaw = ClientAPI.GetPlayerObject().GameObject.transform.rotation.eulerAngles.y;
		Debug.Log("Yaw= " + yaw);
		
		if (yaw >= 337.5 || yaw < 22.5) {
			Debug.Log("North");
		}
		else if (yaw >= 292.5) {
			Debug.Log("NorthWest");
		}
		else if (yaw >= 247.5) {
			Debug.Log("West");
		}
		else if (yaw >= 202.5) {
			Debug.Log("SouthWest");
		}
		else if (yaw >= 157.5) {
			Debug.Log("South");
		}
		else if (yaw >= 112.5) {
			Debug.Log("SouthEast");
		}
		else if (yaw >= 67.5) {
			Debug.Log("East");
		}
		else if (yaw >= 22.5) {
			Debug.Log("NorthEast");
		}
		
	}

}

