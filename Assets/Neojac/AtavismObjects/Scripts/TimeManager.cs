using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour {

	int day;
	int hour;
	int minute;
	int second;
	string currentTime = "";

	float delay = 3;
	float updateTime = -1;

	// Use this for initialization
	void Start () {
		NetworkAPI.RegisterExtensionMessageHandler("server_time", ServerTimeMessage);
	}
	
	// Update is called once per frame
	void Update () {
		if (updateTime != -1 && Time.time > updateTime) {
			GameObject skyProject = GameObject.Find(" SkySphere Manager");
			if (skyProject != null) {
				skyProject.SendMessage("SetSecond", second);
				skyProject.SendMessage("SetMinute", minute);
				skyProject.SendMessage("SetHour", hour);
				skyProject.SendMessage("SetDay", day);
			}
			updateTime = -1;
		}
	}

	public void SetCurrentTime(string timeString) {
		currentTime = timeString;
		string[] pieces = currentTime.Split(':');
		hour = int.Parse(pieces[0]);
		minute = int.Parse(pieces[1]);
	}

	public void ServerTimeMessage(Dictionary<string, object> props) {
		day = (int)props["day"];
		hour = (int)props["hour"];
		minute = (int)props["minute"];
		second = (int)props["second"];
		Debug.Log("Got Server Time Message with Day: " + day + ", hour: " + hour + ". minute: " + minute);
		updateTime = Time.time + delay;
	}

	public string CurrentTime {
		get {
			return currentTime;
		}
	}
	
	public int Hour {
		get {
			return hour;
		}
	}
	
	public int Minute {
		get {
			return minute;
		}
	}
}
