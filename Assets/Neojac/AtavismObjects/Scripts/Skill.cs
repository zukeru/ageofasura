using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill : MonoBehaviour {
	
	public int id = 0;
	public string skillname = "";
	public Texture2D icon;
	public int parentSkill = -1;
	public int parentSkillLevelReq = -1;
	public int playerLevelReq = 1;
	public Dictionary<int, int> abilities;
	int currentPoints;
	int currentLevel;
	int maximumLevel;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public int CurrentPoints {
		get {
			return currentPoints;
		}
		set {
			currentPoints = value;
		}
	}
	
	public int CurrentLevel {
		get {
			return currentLevel;
		}
		set {
			currentLevel = value;
		}
	}
	
	public int MaximumLevel {
		get {
			return maximumLevel;
		}
		set {
			maximumLevel = value;
		}
	}
}
