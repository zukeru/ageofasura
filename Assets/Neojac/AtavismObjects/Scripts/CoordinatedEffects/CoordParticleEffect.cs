using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoordParticleEffect : CoordinatedEffect {

	public AttachmentSockets slot;
	public ParticleSystem particle;
	public AudioClip soundClip;

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
		AtavismLogger.LogDebugMessage("Executing CoordParticleEffect with num props: " + props.Count);
		/*foreach (string prop in props.Keys) {
			Debug.Log(prop + ":" + props[prop]);
		}*/
		
		if (activationDelay == 0) {
			Run ();
		} else {
			activationTime = Time.time + activationDelay;
		}
	}
	
	void Run() {
		activationTime = 0;
		Transform slotTransform;
		if (target == CoordinatedEffectTarget.Caster) {
			casterOid = (OID)props["sourceOID"];
			AtavismObjectNode node = ClientAPI.WorldManager.GetObjectNode(casterOid);
			slotTransform = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(slot);
		} else {
			// Attach to the target
			targetOid = (OID)props["targetOID"];
			AtavismObjectNode node = ClientAPI.WorldManager.GetObjectNode(targetOid);
			slotTransform = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(slot);
		}
		
		GameObject effectParticle = (GameObject)Instantiate (particle.gameObject, slotTransform.position, slotTransform.rotation);
		effectParticle.transform.parent = slotTransform;
		
		if (soundClip != null) {
			// Play sound clip on caster
			GameObject soundObject = new GameObject();
			soundObject.transform.position = slotTransform.position;
			soundObject.transform.parent = slotTransform;
			AudioSource audioSource = soundObject.AddComponent<AudioSource>();
			audioSource.clip = soundClip;
			audioSource.Play();
			Destroy(soundObject, duration);
		}
		
		Destroy(effectParticle, duration);
		if (destroyWhenFinished)
			Destroy (gameObject, duration);
	}
}
