using UnityEngine;
using System.Collections;

public class Ability : Activatable {
	
	public int id = 0;
    //public string command = "";
    //public string style = "";
    public string rank = "";
   	public int cost = 0;
	public string costProperty = "mana";
    public int distance = 0;
    public int castTime = 0;
    public bool globalcd = false;
    public bool weaponcd = false;
    public string cooldownType = "";
    public float cooldownLength = 0;
    public string weaponReq = "";
    //public string stancereq = "";
    public TargetType targetType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public Ability Clone(GameObject go) {
		Ability clone = go.AddComponent<Ability>();
		clone.id = id;
		clone.name = name;
		clone.icon = icon;
		//clone.style = style;
		clone.rank = rank;
		clone.cost = cost;
		clone.distance = distance;
		clone.castTime = castTime;
		clone.globalcd = globalcd;
		clone.weaponcd = weaponcd;
		clone.cooldownType = cooldownType;
		clone.cooldownLength = cooldownLength;
		clone.weaponReq = weaponReq;
		clone.targetType = targetType;
		clone.tooltip = tooltip;
		return clone;
	}
	
	public override bool Activate() {
		// TODO: Enhance the target/self setting based on whether the current
		// target is friendly or an enemy
		if (ClientAPI.GetTargetObject() != null && targetType != TargetType.Self)
			NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/ability " + id);
		else
			NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/ability " + id);
		return true;
	}

	public override void DrawTooltip(float x, float y) {
		int width = 150;
		int height = 50;
		Rect tooltipRect = new Rect(x, y - height, width, height);
		GUI.Box(tooltipRect, "");
		GUI.Label(new Rect(tooltipRect.x + 5, tooltipRect.y + 5, 140, 20), name);
		GUI.Label(new Rect(tooltipRect.x + 5, tooltipRect.y + 25, 140, 20), cost + " " + costProperty);
	}
}
