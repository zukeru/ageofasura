using UnityEngine;
using System.Collections;
using Detour;

public class NavTile : MonoBehaviour {

	AtavismNavTile builder;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public AtavismNavTile Builder {
		get {
			return builder;
		}
		set {
			builder = value;
		}
	}
}
