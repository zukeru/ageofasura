using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoordMeleeStrikeEffect : CoordinatedEffect {
	
	public string attackType = "normal";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void Execute(Dictionary<string, object> props) {
		if (!enabled)
			return;
		AtavismLogger.LogDebugMessage("Executing CoordMeleeStrikeEffect with num props: " + props.Count);
		/*foreach (string prop in props.Keys) {
			Debug.Log(prop + ":" + props[prop]);
		}*/
		
		AtavismObjectNode target = ClientAPI.WorldManager.GetObjectNode((OID)props["targetOID"]);
		AtavismObjectNode caster = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);
        
        //if (result != "success"):
        //    ClientAPI.Write("Attack result is: %s" % (result))
		string result = (string)props["result"];
		//string attackType = (string)props["aType"];
        
        Vector3 targetLoc = target.Position;
        Vector3 casterLoc = caster.Position;
		AtavismLogger.LogDebugMessage("ATTACKEFFECT");
        
        string soundFile = "iow_soundeffect_attack3.wav";
        
        //string weaponType = MarsUnit._GetUnitProperty(caster, "weaponType", "Unarmed");
		//TODO: call sound system - play attack sound
		
		// Play attack animation
		caster.GameObject.GetComponent<AtavismMobController>().PlayMeleeAttackAnimation(attackType, result);
		
		// Play recoil type animation
		target.GameObject.GetComponent<AtavismMobController>().PlayMeleeRecoilAnimation(result);
		
		// Now destroy this object
		Destroy(gameObject, duration);
	}
}
