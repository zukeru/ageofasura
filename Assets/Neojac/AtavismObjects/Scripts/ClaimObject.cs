using UnityEngine;
using System.Collections;

public class ClaimObject : MonoBehaviour {

	int id;
	bool ready = false;
	Color initialColor;
	bool active = false;
	bool selected = false;
	Renderer[] renderers;
	Color[] initialColors;
	
	// Use this for initialization
	void Start () {
		if (GetComponent<Renderer>() != null) {
			initialColor = GetComponent<Renderer>().material.color;
		} else {
			renderers = GetComponentsInChildren<Renderer>();
			initialColors = new Color[renderers.Length];
			for (int i = 0; i < renderers.Length; i++) {
				initialColors[i] = renderers[i].material.color;
			}
		}
		ready = true;
	}
	
	void OnMouseOver()
	{
		if (active)
			Highlight();
	}
	
	void OnMouseExit()
	{
		if (!selected)
			ResetHighlight();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Highlight() {
		if (!ready)
			Start ();
		if (GetComponent<Renderer>() != null) {
			GetComponent<Renderer>().material.color = Color.cyan;
		} else {
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material.color = Color.cyan;
			}
		}
	}
	
	public void ResetHighlight() {
		if (GetComponent<Renderer>() != null) {
			GetComponent<Renderer>().material.color = initialColor;
		} else {
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material.color = initialColors[i];
			}
		}
	}
	
	public int ID {
		set {
			id = value;
		}
		get {
			return id;
		}
	}
	
	public bool Active
	{
		set {
			active = value;
			if (!active && !selected) {
				if (GetComponent<Renderer>() != null) 
					ResetHighlight();
			}
		}
		get {
			return active;
		}
	}
	
	public bool Selected
	{
		set {
			selected = value;
			if (selected) {
				Highlight();
			} else {
				ResetHighlight();
			}
		}
		get {
			return selected;
		}
	}
}
