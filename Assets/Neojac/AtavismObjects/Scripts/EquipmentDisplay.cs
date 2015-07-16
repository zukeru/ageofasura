using UnityEngine;
using System.Collections;

public class EquipmentDisplay : MonoBehaviour {
	
	public enum DisplayTypes {
		Weapon,
		Head,
		Shoulder,
		Chest,
		Legs,
		Hands,
		Feet,
		Back
	}
	
	int id;
	public DisplayTypes type;
	public GameObject model;
	Material material;
	/*public SlotData maleSlotElement;
	public SlotData femaleSlotElement;
	public OverlayData maleOverlayElement;
	public OverlayData femaleOverlayElement;*/

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
