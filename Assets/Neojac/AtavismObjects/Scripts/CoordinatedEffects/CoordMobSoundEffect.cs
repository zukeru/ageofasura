using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoordMobSoundEffect : CoordinatedEffect {

	public MobSoundEvent soundEvent;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (activationTime != 0 && Time.time > activationTime) {
			Run();
		}
	}
	
	public override void Execute(Dictionary<string, object> props) {
		if (!enabled)
			return;
		base.props = props;
		AtavismLogger.LogDebugMessage("Executing CoordAnimationEffect with num props: " + props.Count);
		
		if (activationDelay == 0) {
			Run ();
		} else {
			activationTime = Time.time + activationDelay;
		}
	}
	
	public void Run() {
		AtavismObjectNode node;
		if (target == CoordinatedEffectTarget.Caster) {
			node = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);
		} else {
			node = ClientAPI.WorldManager.GetObjectNode((OID)props["targetOID"]);
		}
		
		node.GameObject.GetComponent<MobSoundSet>().PlaySoundEvent(soundEvent);
		
		// Now destroy this object
		if (destroyWhenFinished)
			Destroy(gameObject, duration);
	}
}
