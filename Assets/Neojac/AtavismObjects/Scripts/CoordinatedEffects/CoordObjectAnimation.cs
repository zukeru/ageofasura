using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoordObjectAnimation : CoordinatedEffect {
	
	public AnimationClip animationClip;
	public float animationLength;
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
		AtavismLogger.LogDebugMessage("Executing CoordAnimationEffect with num props: " + props.Count);
		/*foreach (string prop in props.Keys) {
			Logger.LogDebugMessage(prop + ":" + props[prop]);
		}*/
		
		if (activationDelay == 0) {
			Run ();
		} else {
			activationTime = Time.time + activationDelay;
		}
	}
	
	public void Run() {
		GameObject go = (GameObject)props["gameObject"];
		
		// Play animation
		go.GetComponent<Animation>().clip = animationClip;
		go.GetComponent<Animation>().Play();
			
		
		if (soundClip != null) {
			// Play sound clip on caster
			GameObject soundObject = new GameObject();
			soundObject.transform.position = go.transform.position;
			soundObject.transform.parent = go.transform;
			AudioSource audioSource = soundObject.AddComponent<AudioSource>();
			audioSource.clip = soundClip;
			audioSource.Play();
			Destroy(soundObject, duration);
		}
		
		// Now destroy this object
		if (destroyWhenFinished)
			Destroy(gameObject, duration);
	}
}
