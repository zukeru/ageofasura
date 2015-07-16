using UnityEngine;
using System.Collections;

public class ClickToMoveInputController : AtavismInputController 
{
	private Vector3 position;
	
	public static bool attack;
	public static bool die;

	// The distance in the x-z plane to the target
	float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 8.0f;
	// How much we 
	float heightDamping = 2.0f;
	
	// Use this for initialization
	void Start () 
	{
		// Need to tell the client that this is the new active input controller
		ClientAPI.InputControllerActivated(this);
		
		// Set start point to players current location
		position = ClientAPI.GetPlayerObject().Position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (target == null)
			return;
		UpdateCamera();
	}

	public override Vector3 GetPlayerMovement() {
		if (target == null)
			return Vector3.zero;

		if(!attack&&!die)
		{
			if(Input.GetMouseButton(0))
			{
				//Locate where the player clicked on the terrain
				locatePosition();
			}
			
			//Move the player to the position
			return moveToPosition();
		}
		else
		{
			return Vector3.zero;
		}

	}
	
	public override void RunCameraUpdate() {
		UpdateCamera();
	}

	void locatePosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit, 1000))
		{
			if(hit.collider.tag!="Player"&&hit.collider.tag!="Enemy")
			{
				position = hit.point;
			}
		}
	}
	
	Vector3 moveToPosition()
	{
		//Game Object is moving
		if(Vector3.Distance(target.position, position)>1)
		{
			Quaternion newRotation = Quaternion.LookRotation(position-target.position, Vector3.forward);
			
			newRotation.x = 0f;
			newRotation.z = 0f;
			
			target.rotation = Quaternion.Slerp(target.rotation, newRotation, Time.deltaTime * 10);
			return target.forward;
			
			//animation.CrossFade(run.name);
		}
		//Game Object is not moving
		else
		{
			return Vector3.zero;
			//animation.CrossFade(idle.name);
		}
	}

	#region Camera Movement
	/// <summary>
	///   Move the camera based on the new position of the camera target (player)
	/// </summary>
	/// <param name="playerPos"></param>
	/// <param name="playerOrient"></param>
	protected void UpdateCamera ()
	{
		Camera camera = Camera.main;
		// Calculate the current rotation angles
		float wantedHeight = target.position.y + height;
		
		float currentHeight = camera.transform.position.y;
		
		// Damp the rotation around the y-axis
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		camera.transform.position = target.position;
		camera.transform.position -= Vector3.forward * distance;
		
		// Set the height of the camera
		camera.transform.position = new Vector3(camera.transform.position.x, currentHeight, camera.transform.position.z);
		
		// Always look at the target
		camera.transform.LookAt (target);
		
		//Switch Distance
		if(Input.GetKeyDown("9"))
		{
			distance = 16.0f;
			height = 16.0f;
		}
		
		if(Input.GetKeyDown("0"))
		{
			distance = 80.0f;
			height = 80.0f;
		}
	}
	#endregion Camera Movement
	
}