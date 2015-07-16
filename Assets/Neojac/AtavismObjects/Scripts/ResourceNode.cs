using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HarvestType {
	Axe,
	Pickaxe,
	None
}

public class ResourceNode : MonoBehaviour {

	public int id = -1;
	public List<ResourceDrop> resources;
	public int resourceCount = 1;
	int currentResourceCount;
	public HarvestType harvestTool;
	bool toolMustBeEquipped = true;
	public float timeToHarvest = 0;
	public int skillType = 0;
	public int reqSkillLevel = 0;
	public int skillLevelMax = 10;
	public float cooldown = 2;
	float cooldownEnds;
	public float refreshDuration = 60;
	public GameObject harvestCoordEffect;
	public GameObject activateCoordEffect;
	public GameObject deactivateCoordEffect;

	Color initialColor;
	bool active = true;
	bool selected = false;
	Renderer[] renderers;
	Color[] initialColors;

	// Use this for initialization
	void Start () {
		cooldownEnds = Time.time;
		currentResourceCount = resourceCount;
		gameObject.AddComponent<AtavismNode>();
		GetComponent<AtavismNode>().AddLocalProperty("harvestType", harvestTool);
		GetComponent<AtavismNode>().AddLocalProperty("targetable", false);
		GetComponent<AtavismNode>().AddLocalProperty("active", active);

		if (GetComponent<Renderer>() != null) {
			initialColor = GetComponent<Renderer>().material.color;
		} else {
			renderers = GetComponentsInChildren<Renderer>();
			initialColors = new Color[renderers.Length];
			for (int i = 0; i < renderers.Length; i++) {
				initialColors[i] = renderers[i].material.color;
			}
		}
		
		ClientAPI.ScriptObject.GetComponent<Crafting>().RegisterResourceNode(this);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void HarvestResource() {
		if (Time.time < cooldownEnds) {
			// Send error message
			string[] args = new string[1];
			args[0] = "You cannot perform that action yet.";
			AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
		} else {
			Dictionary<string, object> props = new Dictionary<string, object> ();
			props.Add ("resourceID", id);
			NetworkAPI.SendExtensionMessage (ClientAPI.GetPlayerOid(), false, "crafting.HARVEST_RESOURCE", props);
			cooldownEnds = Time.time + cooldown;
		}
	}

	public void Highlight() {
		if (GetComponent<Renderer>() != null) {
			GetComponent<Renderer>().material.color = Color.red;
		} else {
			for (int i = 0; i < renderers.Length; i++) {
				renderers[i].material.color = Color.red;
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
	}
	
	public bool ToolMustBeEquipped {
		get {
			return toolMustBeEquipped;
		}
		set {
			toolMustBeEquipped = value;
		}
	}

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ResourceNode"/> is active.
    /// </summary>
    /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = value;
            GetComponent<AtavismNode>().AddLocalProperty("active", active);
            if (GetComponent<MeshRenderer>() != null)
            {
                GetComponent<MeshRenderer>().enabled = active;
                GetComponent<MeshCollider>().enabled = active;
                foreach (Transform child in GetComponent<Transform>())
                {
                    if (child.GetComponent<MeshRenderer>() != null)
                    {
                        child.GetComponent<MeshRenderer>().enabled = active;
                    }

                    if (child.GetComponent<MeshCollider>() != null)
                    {
                        child.GetComponent<MeshCollider>().enabled = active;
                    }

                    child.gameObject.SetActive(active);
                }
            }
        }
    }

}
