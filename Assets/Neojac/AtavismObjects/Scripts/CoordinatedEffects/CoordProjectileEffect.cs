using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum ProjectileState {
	Setup,
	Moving,
	Hit
}

public class CoordProjectileEffect : CoordinatedEffect {

	public AttachmentSockets casterSlot;
	public GameObject projectileObject;
	public ParticleSystem projectileParticle;
	public AudioClip projectileSound;
	public float speed = 15; // metres per second
	public AttachmentSockets targetSlot;
	public AttachmentSockets hitSlot;
	public ParticleSystem hitParticle;
	public AudioClip hitSound;
	
	
	Transform targetTransform;
	ProjectileState state = ProjectileState.Setup;
	GameObject projectile;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (activationTime != 0 && Time.time > activationTime) {
			Run();
		}
		
		// If the projectile is moving, move it towards the target
		if (state == ProjectileState.Moving) {
			// Work out how far it should travel this frame
			float distanceToTravel = speed * Time.deltaTime;
			Vector3 newLoc = Vector3.MoveTowards(projectile.transform.position, targetTransform.position, distanceToTravel);
			projectile.transform.position = newLoc;
			AtavismLogger.LogDebugMessage("Moving " + name + " to loc: " + newLoc);
			// Check if we have hit the target
			if (newLoc == targetTransform.position) {
				state = ProjectileState.Hit;
				Destroy(projectile);
				PlayHit();
			}
		}
	}
	
	public override void Execute(Dictionary<string, object> props) {
		if (!enabled)
			return;
		base.props = props;
		AtavismLogger.LogDebugMessage("Executing " + name + " with num props: " + props.Count);
		casterOid = (OID)props["sourceOID"];
		targetOid = (OID)props["targetOID"];
		
		if (activationDelay == 0) {
			Run ();
		} else {
			activationTime = Time.time + activationDelay;
		}
	}
	
	void Run() {
		activationTime = 0;
		
		// Create a base GameObject to attach the projectile particle and/or object to
		projectile = new GameObject();
		projectile.name = name;
		
		// Set the starting position to the specified slot of the caster
		AtavismObjectNode node = ClientAPI.WorldManager.GetObjectNode(casterOid);
		Vector3 projectilePosition = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(casterSlot).position;
		projectile.transform.position = projectilePosition;
		
		// If we have a projectile particle, create an instance of it then attach it to the projectile
		if (projectileParticle != null) {
			GameObject effectParticle = (GameObject)Instantiate (projectileParticle.gameObject, projectile.transform.position, projectile.transform.rotation);
			effectParticle.transform.position = projectile.transform.position;
			effectParticle.transform.parent = projectile.transform;
		}
		
		// If we have a projectile particle, create an instance of it then attach it to the projectile
		if (projectileObject != null) {
			GameObject effectObject = (GameObject)Instantiate (projectileObject);
			effectObject.transform.position = projectile.transform.position;
			effectObject.transform.parent = projectile.transform;
		}
		
		// If we have a projectile particle, create an instance of it then attach it to the projectile
		if (projectileSound != null) {
			AudioSource audioSource = projectile.AddComponent<AudioSource>();
			audioSource.clip = projectileSound;
			audioSource.Play();
		}
		
		// Get the target transform
		node = ClientAPI.WorldManager.GetObjectNode(targetOid);
		targetTransform = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(targetSlot);
		state = ProjectileState.Moving;
	}
	
	// Plays the particles/sound for when the projectile has hit the target
	void PlayHit() {
		AtavismObjectNode node = ClientAPI.WorldManager.GetObjectNode(targetOid);
		Transform slotTransform = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(hitSlot);
		
		if (hitParticle != null) {
			GameObject effectParticle = (GameObject)Instantiate (hitParticle.gameObject, slotTransform.position, slotTransform.rotation);
			effectParticle.transform.parent = slotTransform;
			Destroy(effectParticle, duration);
		}
		
		if (hitSound != null) {
			GameObject soundObject = new GameObject();
			soundObject.transform.position = slotTransform.position;
			soundObject.transform.parent = slotTransform;
			AudioSource audioSource = soundObject.AddComponent<AudioSource>();
			audioSource.clip = hitSound;
			audioSource.Play();
			Destroy(soundObject, duration);
		}
		
		if (destroyWhenFinished)
			Destroy (gameObject, duration);
	}
}
