using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tooltips : MonoBehaviour {

	static Tooltips instance;
	public GUISkin skin;
	public int uiLayer = 3;
	
	Activatable tooltipToDraw;
	string frameName;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnGUI() {
		if (tooltipToDraw == null)
			return;
		
		GUI.depth = uiLayer;
		GUI.skin = skin;
		
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.y = Screen.height - mousePosition.y;
		
		tooltipToDraw.DrawTooltip(mousePosition.x, mousePosition.y);
	}
	
	public void SetTooltip(Activatable tooltipObject, string frame) {
		if (tooltipObject == null && frame != frameName)
			return;
		tooltipToDraw = tooltipObject;
		frameName = frame;
	}

	public static Tooltips Instance {
		get {
			return instance;
		}
	}
}
