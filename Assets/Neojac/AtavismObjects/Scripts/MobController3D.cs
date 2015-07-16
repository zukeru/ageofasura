using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobController3D : AtavismMobController {
	
	#region Animation Fields
	
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip jumpPoseAnimation;
	public AnimationClip combatIdleAnimation;
	public AnimationClip unarmedAttackedAnimation;
	public AnimationClip deathAnimation;
	public AnimationClip swimIdleAnimation;
	public AnimationClip swimAnimation;
	public float walkMaxAnimationSpeed = 0.75f;
	public float trotMaxAnimationSpeed = 1.0f;
	public float runMaxAnimationSpeed = 1.0f;
	public float jumpAnimationSpeed = 1.15f;
	public float landAnimationSpeed = 1.0f;
	private Animation _animation;
	private Animator _animator;
	private bool useAnimator;
	
	enum CharacterState
	{
		Idle = 0,
		Walking = 1,
		Trotting = 2,
		Running = 3,
		Jumping = 4,
		SwimIdle = 5,
		SwimMove = 6
	}

	bool dead = false;
	bool inCombat = false;
	AnimationClip overrideAnimation;
	string overrideAnimationName;
	float overrideAnimationExpires;
	
	#endregion Animation Fields

	#region Movement Fields
	// Is Walk on
	bool walk = false;
	// The speed when walking
	float walkSpeed = 2.0f;
	float inAirControlAcceleration = 3.0f;
	public float runThreshold = 2.5f;
	
	// How high do we jump when pressing jump and letting go immediately
	public float jumpHeight = 1.5f;
	
	// The gravity for the character
	float gravity = 20.0f;
	// The gravity in controlled descent mode
	float speedSmoothing = 10.0f;
	float rotateSpeed = 5.0f; // was 250
	float trotAfterSeconds = 3.0f;
	bool canJump = true;
	private float jumpRepeatTime = 0.05f;
	private float jumpTimeout = 0.15f;
	private float groundedTimeout = 0.25f;
	
	// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
	private float lockCameraTimer = 0.0f;
	
	// The current move direction in x-z
	private Vector3 moveDirection = Vector3.zero;
	// The current vertical speed
	private float verticalSpeed = 0.0f;
	// The current x-z move speed
	private float moveSpeed = 0.0f;
	Vector3 direction = Vector3.zero;
	Quaternion rotation = Quaternion.identity;
	
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags; 
	
	// Are we moving backwards (This locks the camera to not do a 180 degree spin)
	private bool movingBack = false;
	// Is the user pressing any keys?
	private bool isMoving = false;
	// When did the user start walking (Used for going into trot after a while)
	private float walkTimeStart = 0.0f;
	// Last time we performed a jump
	private float lastJumpTime = -1.0f;
	
	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	private float lastJumpStartHeight = 0.0f;
	private Vector3 inAirVelocity = Vector3.zero;
	private float lastGroundedTime = 0.0f;
	private bool isControllable = true;
	private bool grounded;
	public float groundedCheckOffset = 0.2f;
	private const float groundedDistance = 0.5f;
	private const float groundDrag = 5.0f;
	public LayerMask groundLayers = 0;
	private bool mouseOverUI = false;
	bool leftButtonDown = false;
	bool rightButtonDown = false;
	bool mouseRun = false;

	protected float fallingSpeed = 0;
	protected bool isFalling = false;
	protected float fallingDistance = 0;
	protected float fallStartHeight = float.MinValue;
	
	bool underWater = false;
	
	protected int movementState = 1; // Used to switch between running (1), swimming (2) and flying (3)
	#endregion Movement Fields
	
	public GameObject friendlyTargetDecal;
	public GameObject neutralTargetDecal;
	public GameObject enemyTargetDecal;
	GameObject targetDecal;
	
	#region UI Fields
	public GUISkin customSkin = null;
	public string styleName = "Box";
	public float fadeDistance = 30.0f, hideDistance = 35.0f;
	public float maxViewAngle = 90.0f;
	public float nameHeight = 2.2f;
	string combatText = "";
	int combatTextType = 1;
	float combatTextExpiration;
	#endregion UI Fields
	
	string weaponType;
	
	// Use this for initialization
	void Start ()
	{
		useAnimator = false;
		_animation = (Animation)GetComponent ("Animation");
		if (!_animation) {
			_animator = (Animator)GetComponentInChildren (typeof(Animator));
			useAnimator = true;
			if (!_animator)
				AtavismLogger.LogInfoMessage ("The character you would like to control doesn't have animations. Moving them might look weird.");
			else
				AtavismLogger.LogDebugMessage("Got animator: " + _animator);
		}
		
		if (!idleAnimation) {
			_animation = null;
			AtavismLogger.LogDebugMessage ("No idle animation found. Turning off animations.");
		}
		if (!walkAnimation) {
			_animation = null;
			AtavismLogger.LogDebugMessage ("No walk animation found. Turning off animations.");
		}
		if (!runAnimation) {
			_animation = null;
			AtavismLogger.LogDebugMessage ("No run animation found. Turning off animations.");
		}
		if (!jumpPoseAnimation && canJump) {
			_animation = null;
			AtavismLogger.LogDebugMessage ("No jump animation found and the character has canJump enabled. Turning off animations.");
		}
		
		moveSpeed = runSpeed;
		
		// Get this mob to ignore collisions with other mobs/players
		foreach (AtavismMobNode mNode in ClientAPI.WorldManager.GetMobNodes()) {
			if (mNode.GameObject != null && mNode.GameObject.GetComponent<Collider>() != null && GetComponent<Collider>() != mNode.GameObject.GetComponent<Collider>())
				Physics.IgnoreCollision (GetComponent<Collider>(), mNode.GameObject.GetComponent<Collider>());
		}
		//if (go != gameObject)
		if (ClientAPI.GetPlayerObject() != null && !isPlayer)
			Physics.IgnoreCollision (GetComponent<Collider>(), ClientAPI.GetPlayerObject().GameObject.GetComponent<Collider>());
	}
	
	void ObjectNodeReady ()
	{
		this.oid = GetComponent<AtavismNode> ().Oid;
		GetComponent<AtavismNode>().SetMobController(this);
		GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler ("deadstate", HandleDeadState);
		GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("combatstate", HandleCombatState);
		GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("weaponType", HandleWeaponType);
		// Get deadstate property
		if (GetComponent<AtavismNode>().PropertyExists("deadstate")) {
			dead = (bool)GetComponent<AtavismNode>().GetProperty("deadstate");
		}
		// Get weaponType property
		if (GetComponent<AtavismNode>().PropertyExists("weaponType")) {
			weaponType = (string)GetComponent<AtavismNode>().GetProperty("weaponType");
			/*if (weaponType != null && weaponType != "" && weaponType != "Unarmed") {
				_animator.SetBool (weaponType, true);
			}*/
		}
		// Get MovementState property
		GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("movement_state", MovementStateHandler);
		if (GetComponent<AtavismNode>().PropertyExists("movement_state")) {
			movementState = (int)GetComponent<AtavismNode>().GetProperty("movement_state");
		}
		//GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("currentAction", null);
		transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
		
		string[] args = new string[1];
		args[0] = oid.ToString();
		AtavismEventSystem.DispatchEvent("MOB_CONTROLLER_ADDED", args);
	}
	
	void OnDestroy () {
		string[] args = new string[1];
		args[0] = oid.ToString();
		AtavismEventSystem.DispatchEvent("MOB_CONTROLLER_REMOVED", args);
		AtavismNode aNode = GetComponent<AtavismNode>();
		if (aNode != null) {
			aNode.RemoveObjectPropertyChangeHandler("deadstate", HandleDeadState);
			aNode.RemoveObjectPropertyChangeHandler("combatstate", HandleCombatState);
			aNode.RemoveObjectPropertyChangeHandler("movement_state", MovementStateHandler);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 movement = Vector3.zero;
		if (isPlayer) {
			if (!dead) {
				movement = MovePlayer ();
			} else {
				// Still need to get player movement even when dead
				AtavismInputController inputManager = ClientAPI.GetInputController();
				inputManager.GetPlayerMovement ();
				if (transform.position.y > ClientAPI.Instance.WaterHeight) {
					ApplyGravity ();
				}
				movement = new Vector3(0, verticalSpeed * Time.deltaTime, 0) + inAirVelocity;
			}
		} else if (!dead) {
			movement = MoveMob ();
		} else {
			bool isAboveTerrain = IsAboveTerrain();
			int attempts = 0;
			while (!isAboveTerrain && attempts < 20 && groundLayers > 0) {
				transform.position += new Vector3(0, 1.5f, 0);
				isAboveTerrain = IsAboveTerrain();
				attempts++;
			}
			ApplyGravity ();
			movement = new Vector3(0, verticalSpeed * Time.deltaTime, 0) + inAirVelocity;
		}
		// Move the controller
		CharacterController controller = (CharacterController)GetComponent ("CharacterController");
		collisionFlags = controller.Move (movement);
		
		// Update facing if needed
		if (!isPlayer && target != -1 && !dead) {
			AtavismObjectNode targetNode = AtavismClient.Instance.WorldManager.GetObjectNode (target);
			if (targetNode != null)
				gameObject.transform.LookAt (targetNode.GameObject.transform);
		}
		
		// ANIMATION sector
		if (_animation && !dead) {
			//Debug.Log("Using animation for mob: " + name);
			if (jumping) {
				if (!jumpingReachedApex) {
					_animation [jumpPoseAnimation.name].speed = jumpAnimationSpeed;
					_animation [jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade (jumpPoseAnimation.name);
				} else {
					_animation [jumpPoseAnimation.name].speed = -landAnimationSpeed;
					_animation [jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade (jumpPoseAnimation.name);				
				}
			} else if (movementState == 2) {
				if (controller.velocity.sqrMagnitude > 0.1) {
					_animation [swimAnimation.name].speed = Mathf.Clamp (controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
					_animation.CrossFade (swimAnimation.name);
				} else {
					_animation.CrossFade (swimIdleAnimation.name);
				}
			} else {
				if (controller.velocity.sqrMagnitude > 0.1) {
					if (controller.velocity.magnitude > runThreshold) {
						_animation [runAnimation.name].speed = Mathf.Clamp (controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
						_animation.CrossFade (runAnimation.name);
					} else {
						_animation [walkAnimation.name].speed = Mathf.Clamp (controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
						_animation.CrossFade (walkAnimation.name);	
					}
				} else if (overrideAnimation != null) {
					//_animation [overrideAnimation.name].speed = Mathf.Clamp (controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
					_animation.CrossFade (overrideAnimation.name);
					if (Time.time > overrideAnimationExpires) {
						overrideAnimation = null;
					}
				} else {
					_animation.CrossFade (idleAnimation.name);
				}
			}
		} else if (useAnimator) {
			if (_animator == null)
			{
				_animator = (Animator)GetComponentInChildren(typeof(Animator));
			}
			// Debug.Log("Using animator for mob: " + name);
			if (_animator && dead)
			{
				_animator.SetBool ("Dead", true);
				_animator.SetBool ("SpecialAttack2", false);
				_animator.SetBool ("Attack", false);
			} else if (_animator) {
				_animator.SetInteger("MovementState", movementState);
				_animator.SetBool ("Dead", false);
				_animator.SetFloat ("Speed", controller.velocity.magnitude);
				if (jumping) {
					_animator.SetBool ("Jump", true);
				} else {
					_animator.SetBool ("Jump", false);
				}
				if (weaponType != null && weaponType != "" && weaponType != "Unarmed") {
					_animator.SetBool (weaponType, true);
				}
				if (overrideAnimationName == "waving") {
					if (Time.time > overrideAnimationExpires) {
						_animator.SetBool ("Waving", false);
						overrideAnimationName = "";
					} else {
						_animator.SetBool ("Waving", true);
					}
				} else if (overrideAnimationName == "mining") {
					if (Time.time > overrideAnimationExpires) {
						_animator.SetBool ("Mining", false);
						overrideAnimationName = "";
					} else {
						_animator.SetBool ("Mining", true);
					}
				} else if (overrideAnimationName == "attack_normal") {
					if (Time.time > overrideAnimationExpires) {
						_animator.SetBool ("Attack", false);
						overrideAnimationName = "";
					} else {
						_animator.SetBool ("Wound", false);
						_animator.SetBool ("SpecialAttack2", false);
						_animator.SetBool ("Attack", true);
					}
				} else if (overrideAnimationName == "attack_special") {
					if (Time.time > overrideAnimationExpires) {
						_animator.SetBool ("SpecialAttack", false);
						overrideAnimationName = "";
					} else {
						_animator.SetBool ("SpecialAttack", true);
					}
				} else if (overrideAnimationName == "attack_special2") {
					if (Time.time > overrideAnimationExpires) {
						_animator.SetBool ("SpecialAttack2", false);
						overrideAnimationName = "";
					} else {
						_animator.SetBool ("Wound", false);
						_animator.SetBool ("Attack", false);
						_animator.SetBool ("SpecialAttack2", true);
					}
				} else if (overrideAnimationName == "wound") {
					if (Time.time > overrideAnimationExpires) {
						_animator.SetBool ("Wound", false);
						overrideAnimationName = "";
					} else {
						_animator.SetBool ("Wound", true);
					}
				} else if (overrideAnimationName != null && overrideAnimationName != "") {
					if (Time.time > overrideAnimationExpires) {
						_animator.SetBool (overrideAnimationName, false);
						overrideAnimationName = "";
					} else {
						_animator.SetBool (overrideAnimationName, true);
					}
				}
			}
		}
		// ANIMATION sector
		
		// Set rotation to the move direction
		/*if (IsGrounded ()) {
			transform.rotation = rotation;
			//transform.rotation = Quaternion.LookRotation (moveDirection);
			
		} else {
			var xzMove = movement;
			xzMove.y = 0;
			if (xzMove.sqrMagnitude > 0.001) {
				transform.rotation = Quaternion.LookRotation (xzMove);
			}
		}*/	
		
		// We are in jump mode but just became grounded
		if (IsGrounded ()) {
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping) {
				jumping = false;
				SendMessage ("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
		
		// Update camera if it is the player
		if (isPlayer) {
			AtavismInputController inputManager = ClientAPI.GetInputController();
			inputManager.RunCameraUpdate();
		}
	}
	
	public override Vector3 MoveMob ()
	{
		float timeDifference = (Time.time - lastLocTimestamp);
		if (pathInterpolator != null) {
			PathLocAndDir locAndDir = pathInterpolator.Interpolate (Time.time);
			float interpolateSpeed = pathInterpolator.Speed;
			//UnityEngine.Debug.Log("MobNode.ComputePosition: oid " + oid + ", followTerrain " + followTerrain 
			//	+ ", pathInterpolator ");// + (locAndDir == null) ? "null" : locAndDir.ToString ());
			if (locAndDir != null) {
				/*if (locAndDir.LengthLeft > 0.25f) {
					transform.forward = locAndDir.Direction;
					//transform.rotation = Quaternion.LookRotation(locAndDir.Direction);
					//transform.rotation = Quaternion.LookRotation(LastDirection.normalized);
					//UnityEngine.Debug.Log("Set rotation to: " + transform.rotation);
				}*/
				lastDirection = locAndDir.Direction;
				lastDirTimestamp = Time.time;
				lastLocTimestamp = Time.time;
				Vector3 loc = locAndDir.Location;
				if (AtavismMobNode.useMoveMobNodeForPathInterpolator) {
					Vector3 diff = loc - transform.position;
					diff.y = 0;
					//desiredDisplacement = diff * timeDifference;
					if (diff.magnitude > 1)
						diff = diff.normalized;
					desiredDisplacement = diff * interpolateSpeed * timeDifference;
					//UnityEngine.Debug.Log("displacement: " + desiredDisplacement + " with loc: " + loc + " and current position: " + transform.position);
					if (desiredDisplacement != Vector3.zero) {
						//transform.forward = locAndDir.Direction;
						transform.rotation = Quaternion.LookRotation (desiredDisplacement);
					}
				} else {
					desiredDisplacement = Vector3.zero;
				}
			} else {
				// This interpolator has expired, so get rid of it
				pathInterpolator = null;
				lastDirection = Vector3.zero;
				desiredDisplacement = Vector3.zero;
				//UnityEngine.Debug.Log("Path interpolator for mob: " + oid + " has expired");
			}
		} else {
			lastLocTimestamp = Time.time;
			Vector3 pos = transform.position + (Time.deltaTime * lastDirection);
			desiredDisplacement = pos - transform.position;
		}
		
		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		//if (!grounded) {
		ApplyGravity ();
		//UnityEngine.Debug.Log("Mob grounded? false");
		//} else {
		//	verticalSpeed = 0.0f;
		//}
		
		// Apply jumping logic
		ApplyJumping ();
		
		// Calculate actual motion
		// Multiply inAirVelocity by delta time as we don't multiply the whole movement
		inAirVelocity *= Time.deltaTime;
		//if (verticalSpeed > 0)
		//	UnityEngine.Debug.Log("Vertical speed: " + verticalSpeed);
		Vector3 movement = desiredDisplacement + new Vector3 (0, verticalSpeed * Time.deltaTime, 0) + inAirVelocity;
		//UnityEngine.Debug.Log("Moving mob: " + this.name + " by: " + movement);
		//movement *= Time.deltaTime;
		
		return movement;
	}
	
	public override Vector3 MovePlayer ()
	{
		if (Input.GetButtonDown ("Jump")) {
			lastJumpButtonTime = Time.time;
			AtavismClient.Instance.WorldManager.SendJumpStarted ();
		}
		AtavismInputController inputManager = ClientAPI.GetInputController();
		Vector3 direction = inputManager.GetPlayerMovement ();
		
		// state check
		if (transform.position.y < ClientAPI.Instance.WaterHeight) {
			if (movementState != 2) {
				// start swimming and send state update to server
				movementState = 2;
				followTerrain = false;
				if (isPlayer) {
					SendMovementState();
				}
			}
			followTerrain = false;
		} else if (movementState == 2) {
			movementState = 1;
			followTerrain = true;
			if (isPlayer) {
				SendMovementState();
			}
		}
		
		if (transform.position.y < (ClientAPI.Instance.WaterHeight - 1)) {
			if (!underWater) {
				underWater = true;
				SendUnderwaterState();
			}
		} else if (underWater) {
			underWater = false;
			SendUnderwaterState();
		}
		
		// Apply gravity if not swimming or flying
		//if (followTerrain) {
			// - extra power jump modifies gravity
			// - controlledDescent mode modifies gravity
			ApplyGravity ();
		
			// Apply jumping logic
			ApplyJumping ();
		/*} else {
			verticalSpeed = 0;
		}*/
		
		// Calculate actual motion
		direction.Normalize ();
		float speed = runSpeed;
		if (walk || movingBack)
			speed = walkSpeed;
		//Vector3 displacement = (transform.rotation * direction) * speed;
		Vector3 displacement = (direction * speed);
		Vector3 movement = displacement + new Vector3 (0, verticalSpeed, 0) + inAirVelocity;
		movement *= Time.deltaTime;
		
		// Update player direction - Used for MMO
		AtavismClient.Instance.WorldManager.Player.SetDirection (displacement, transform.position, Time.time);
		AtavismClient.Instance.WorldManager.Player.Orientation = transform.rotation;
		
		return movement;
	}
	
	void SendMovementState() {
		Dictionary<string, object> props = new Dictionary<string, object> ();
		props.Add("movement_state", movementState);
		NetworkAPI.SendExtensionMessage (ClientAPI.GetPlayerOid (), false, "ao.MOVEMENT_STATE", props);
	}
	
	void SendUnderwaterState() {
		Dictionary<string, object> props = new Dictionary<string, object> ();
		props.Add("underwater", underWater);
		NetworkAPI.SendExtensionMessage (ClientAPI.GetPlayerOid (), false, "ao.SET_UNDERWATER", props);
	}
	
	void OnGUI ()
	{
		if (this.oid == 0)
			return;
		Vector3 worldPosition = new Vector3 (GetComponent<Collider>().bounds.center.x, GetComponent<Collider>().bounds.min.y, GetComponent<Collider>().bounds.center.z);
		worldPosition += Vector3.up * nameHeight;
		float cameraDistance = (worldPosition - Camera.main.transform.position).magnitude;
		
		// If the world position is outside of the field of view or further away than hideDistance, don't render the label
		if (
			cameraDistance > hideDistance ||
			Vector3.Angle (
			Camera.main.transform.forward,
			worldPosition - Camera.main.transform.position
			) >
			maxViewAngle
			) {
			return;
		}
		
		// If the distance to the label position is greater than the fade distance, apply the needed fade to the label
		if (cameraDistance > fadeDistance) {
			GUI.color = new Color (1.0f, 1.0f, 1.0f, 1.0f - (cameraDistance - fadeDistance) / (hideDistance - fadeDistance));
		}
		
		Vector2 position = Camera.main.WorldToScreenPoint (worldPosition);
		position = new Vector2 (position.x, Screen.height - position.y);
		// Get the GUI space position
		
		GUI.skin = customSkin;
		// Set the custom skin. If no custom skin is set (null), Unity will use the default skin
		
		Vector2 size = GUI.skin.GetStyle (styleName).CalcSize (new GUIContent (gameObject.name));
		// Get the content size with the selected style
		
		Rect rect = new Rect (position.x - size.x * 0.5f, position.y - size.y, size.x, size.y);
		// Construct a rect based on the calculated position and size
		if (!isPlayer)
			GUI.Label (rect, gameObject.name);
		
		// Draw combat text if any
		if (combatText == "")
			return;
		if (Time.time > combatTextExpiration) {
			combatText = "";
			return;
		}
		// Move up another 40cm
		worldPosition += Vector3.up * 0.4f;
		position = Camera.main.WorldToScreenPoint (worldPosition);
		position = new Vector2 (position.x, Screen.height - position.y);
		size = GUI.skin.GetStyle (styleName).CalcSize (new GUIContent (combatText));
		rect = new Rect (position.x - size.x * 0.5f, position.y - size.y, size.x, size.y);
		// Construct a rect based on the calculated position and size
		if (isPlayer && !combatText.Contains("-")) {
			GUI.color = Color.green;
		} else if (isPlayer) {
			GUI.color = Color.red;
		} else {
			GUI.color = Color.yellow;
		}
		GUI.Label (rect, combatText);
		GUI.color = Color.white;
	}
	
	#region Movement
	
	public void ApplyJumping ()
	{
		// Prevent jumping too fast after each other
		if (lastJumpTime + jumpRepeatTime > Time.time) {
			return;
		}
		
		if (IsGrounded () || !followTerrain) {
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout) {
				jumpingReachedApex = false;
				AtavismLogger.LogDebugMessage ("Applying jump for mob: " + name);
				verticalSpeed = CalculateJumpVerticalSpeed (jumpHeight);
				SendMessage ("DidJump", SendMessageOptions.DontRequireReceiver);
				jumping = true;
			}
		}
	}
	
	public void ApplyGravity ()
	{
		if (isControllable) {	// don't move player at all if not controllable.
			
			// When we reach the apex of the jump we send out a message
			if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0) {
				jumpingReachedApex = true;
				SendMessage ("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			
			if (IsGrounded () || !followTerrain) {
				verticalSpeed = 0.0f;
				jumping = false;
				fallStartHeight = float.MinValue;
				fallingDistance = 0;
			} else {
				verticalSpeed -= gravity * Time.deltaTime;
				if (fallStartHeight == float.MinValue) {
					fallStartHeight = transform.position.y;
				} else {
					fallingDistance = fallStartHeight - transform.position.y;
				}
			}
		}
	}
	
	public float CalculateJumpVerticalSpeed (float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt (2 * targetJumpHeight * gravity);
	}
	
	public void OnControllerColliderHit (ControllerColliderHit hit)
	{
		//	Debug.DrawRay(hit.point, hit.normal);
		if (hit.moveDirection.y > 0.01) 
			return;
	}
	
	public float GetSpeed ()
	{
		return moveSpeed;
	}
	
	public Vector3 GetDirection ()
	{
		return moveDirection;
	}
	
	public bool IsMovingBackwards ()
	{
		return movingBack;
	}
	
	public bool IsMoving ()
	{
		return Mathf.Abs (Input.GetAxisRaw ("Vertical")) + Mathf.Abs (Input.GetAxisRaw ("Horizontal")) > 0.5;
	}
	
	public bool IsJumping ()
	{
		return jumping;
	}
	
	public bool IsGrounded ()
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
	
	public bool HasJumpReachedApex ()
	{
		return jumpingReachedApex;
	}
	
	public bool IsGroundedWithTimeout ()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}
	
	#endregion Movement
	
	#region Other Functions
	
	public override void GotDamageMessage (int messageType, string damageAmount)
	{
		AtavismLogger.LogDebugMessage ("Got damage message for " + name + " with amount: " + damageAmount);
		combatText = damageAmount;
		combatTextType = messageType;
		if (combatTextType == 1) {
			combatText = "-" + combatText;
		}
		combatTextExpiration = Time.time + 1.5f;
	}
	
	public override void PlayMeleeAttackAnimation (string attackType, string result)
	{
		if (attackType == "normal") {
			overrideAnimationName = "attack_normal";
		} else if (attackType == "special") {
			overrideAnimationName = "attack_special";
		} else if (attackType == "special2") {
			overrideAnimationName = "attack_special2";
		}
		overrideAnimation = unarmedAttackedAnimation;
		overrideAnimationExpires = Time.time + 1.0f; //overrideAnimation.length;
	}
	
	public override void PlayMeleeRecoilAnimation (string result)
	{
		overrideAnimationName = "wound";
		overrideAnimationExpires = Time.time + 0.5f;
	}
	
	public override void PlayAnimation(string animationName, float length) {
		if (_animator != null && overrideAnimationName != null && overrideAnimationName != "") {
			AtavismLogger.LogDebugMessage("clearing old animation");
			_animator.SetBool (overrideAnimationName, false);
		}
		overrideAnimationName = animationName;
		overrideAnimationExpires = Time.time + length;
		
		if (GetComponent<Animation>()) {
			if (animationName == "attack_normal" || animationName == "Attack") {
				overrideAnimation = unarmedAttackedAnimation;
			}
		}
	}
	
	public void Wave (bool wave)
	{
		overrideAnimationName = "waving";
		overrideAnimationExpires = Time.time + 2;
	}
	
	/// <summary>
	/// Called when this mob is now targeted by the player.
	/// </summary>
	public void StartTarget() {
		int targetType = 0;
		if (GetComponent<AtavismNode>().PropertyExists("targetType")) {
			targetType = (int)GetComponent<AtavismNode>().GetProperty("targetType");
		}
		
		if (targetType == 1 && friendlyTargetDecal != null) {
			targetDecal = (GameObject)Instantiate(friendlyTargetDecal, transform.position, transform.rotation);
			targetDecal.transform.parent = transform;
		} else if (targetType == 0 && neutralTargetDecal != null) {
			targetDecal = (GameObject)Instantiate(neutralTargetDecal, transform.position, transform.rotation);
			targetDecal.transform.parent = transform;
		} else if (enemyTargetDecal != null) {
			targetDecal = (GameObject)Instantiate(enemyTargetDecal, transform.position, transform.rotation);
			targetDecal.transform.parent = transform;
		}
	}
	
	/// <summary>
	/// Called when this mob is no longer targeted by the player
	/// </summary>
	public void StopTarget() {
		if (targetDecal != null) {
			Destroy(targetDecal);
			targetDecal = null;
		}
	}
	
	public void ToggleWalk() {
		walk = !walk;
	}
	
	#endregion Other Functions
	
	#region Property Handlers
	public void HandleDeadState (object sender, PropertyChangeEventArgs args)
	{
		//Debug.Log ("Got dead update: " + oid);
		dead = (bool)AtavismClient.Instance.WorldManager.GetObjectNode(oid).GetProperty("deadstate");
		if (dead) {
			// Play death animation
			if (_animation) {
				_animation.CrossFade (deathAnimation.name);
			} else if (_animator) {
				_animator.SetBool ("Dead", true);
			}
			target = -1;
		}
		//Debug.Log ("Set dead state to: " + dead);
	}
	
	bool IsAboveTerrain() {
		// Make sure the corpse isn't underground
		Ray ray = new Ray(transform.position, Vector3.down);
		RaycastHit hit;
		// Casts the ray and get the first game object hit
		return Physics.Raycast (ray, out hit, Mathf.Infinity, groundLayers);
	}
	
	public void HandleCombatState (object sender, PropertyChangeEventArgs args)
	{
		inCombat = (bool)AtavismClient.Instance.WorldManager.GetObjectNode(oid).GetProperty("combatstate");
		if (_animator != null) {
			AtavismLogger.LogDebugMessage("setting combat state for animator");
			_animator.SetBool ("Combat", inCombat);
		}
	}
	
	public void MovementStateHandler(object sender, PropertyChangeEventArgs args) {
		AtavismLogger.LogDebugMessage("Got movementstate");
		AtavismObjectNode node = (AtavismObjectNode)sender;
		int state = (int)GetComponent<AtavismNode> ().GetProperty (args.PropertyName);
		if (isPlayer && state != 2 && transform.position.y < ClientAPI.Instance.WaterHeight) {
			movementState = 2;
		} else {
			movementState = state;
		}
	}
	
	public void HandleWeaponType (object sender, PropertyChangeEventArgs args)
	{
		if (useAnimator) {
			if (_animator == null)
			{
				_animator = (Animator)GetComponentInChildren(typeof(Animator));
			}
		} else {
			return;
		}
		if (weaponType != null && weaponType != "" && weaponType != "Unarmed") {
			_animator.SetBool (weaponType, false);
		}
		weaponType = (string)AtavismClient.Instance.WorldManager.GetObjectNode(oid).GetProperty("weaponType");
		if (_animator != null && weaponType != "" && weaponType != "Unarmed") {
			AtavismLogger.LogDebugMessage("setting combat state for animator");
			_animator.SetBool (weaponType, true);
		}
	}
	#endregion Property Handlers
	
	#region Properties
	public Quaternion Rotation {
		get {
			return rotation;
		}
		set {
			rotation = value;
		}
	}
	
	public bool Walking {
		get {
			return walk;
		}
		set {
			walk = value;
		}
	}
	
	public float MobYaw {
		get {
			float yaw;
			yaw = transform.rotation.eulerAngles.y;
			return yaw;
		}
		set {
			Camera camera = Camera.main;
			Vector3 pitchYawRoll = transform.eulerAngles;
			pitchYawRoll.y = value;
			transform.eulerAngles = pitchYawRoll;
		}
	}
	#endregion Properties
}
