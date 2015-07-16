using UnityEngine;
using System.Collections;

public enum TargetType {
	Enemy,
	Friendly,
	Self,
	Ground
}

/// <summary>
/// Abstract class used by classes such as Ability and Item that are activatable by the player.
/// </summary>
public abstract class Activatable : MonoBehaviour {
	
	public string name;
	public Texture2D icon;
	float cooldownDuration;
	float cooldownStart;
	public string tooltip;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public abstract bool Activate();

	public abstract void DrawTooltip(float x, float y);
	
	public void StartCooldown(float duration) {
		cooldownStart = Time.time;
		cooldownDuration = duration;
	}
	
	public bool IsOnCooldown() {
		if (cooldownStart == -1)
			return false;
		else
			return true;
	}
	
	public float CooldownTimeLeft() {
		if (cooldownStart == -1)
			return 0;
		
		return Time.time - (cooldownStart + cooldownDuration);
	}
}
