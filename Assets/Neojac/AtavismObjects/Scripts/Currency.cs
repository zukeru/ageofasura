using UnityEngine;
using System.Collections;

public class Currency : MonoBehaviour {

	public int id = -1;
	public string name = "";
	public Texture2D icon;
	int current = 0;
	
	/*public int subCurrency1ID = -1;
	public string subCurrency1Name = "";
	public Texture2D subCurrency1Icon;
	int subCurrency1Count = 0;
	public int subCurrency2ID = -1;
	public string subCurrency2Name = "";
	public Texture2D subCurrency2Icon;
	int subCurrency2Count = 0;*/

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public int Current {
		get {
			return current;
		}
		set {
			current = value;
		}
	}
	
	/*public int SubCurrency1ID {
		get {
			return subCurrency1ID;
		}
		set {
			subCurrency1ID = value;
		}
	}
	
	public int SubCurrency1Count {
		get {
			return subCurrency1Count;
		}
		set {
			subCurrency1Count = value;
		}
	}
	
	public int SubCurrency2ID {
		get {
			return subCurrency2ID;
		}
		set {
			subCurrency2ID = value;
		}
	}
	
	public int SubCurrency2Count {
		get {
			return subCurrency2Count;
		}
		set {
			subCurrency2Count = value;
		}
	}*/
}
