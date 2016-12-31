/* Script created by Lachlan McKay 2016 ©
 * This script makes an object look at a target or manually specified position */

using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour
{
	
	[Header("Main Options")]
	[Tooltip("The object that will be driven by this script.")]
	public Transform affectedObject;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated;
	[Tooltip("Enable this if the target will be moving and you want the affected object to follow it.")]
	public bool continuouslyUpdateTargetPosition = true;

	[Header("Target Method")]
	public tMode targetMode;
	public enum tMode {LoadedTransform, ManualPosition, ManualDirection}

	[System.Serializable]
	public class LookAtLoadedTransform
	{
		[Tooltip("The object that the driven object will look at.")]
		public Transform targetTransform;
	}

	public LookAtLoadedTransform LoadedTransform = new LookAtLoadedTransform();

	[System.Serializable]
	public class LookAtManualPosition
	{
		[Tooltip("The world position that the driven object will look at.")]
		public Vector3 targetVectorPos;
	}
	
	public LookAtManualPosition ManualPosition = new LookAtManualPosition();

	[System.Serializable]
	public class LookAtManualDirection
	{
		[Tooltip("The global direction vector that the driven object will rotate towards. Compatible with Rotate Towards rotation mode only.")]
		public Vector3 targetDirection;
	}
	
	public LookAtManualDirection ManualDirection = new LookAtManualDirection();

	[Header("Rotation Method")]
	public mode rotateMode;
	public enum mode {LookAt, RotateTowards}

	[System.Serializable]
	public class LookAtLookAtSettings
	{
		[Header("World Up Vector")]
		[Tooltip("Which direction the lookAt function should consider upwards. (Usually 0,1,0)")]
		public Vector3 lookAtUpVector = Vector3.up;	
	}

	public LookAtLookAtSettings LookAtSettings = new LookAtLookAtSettings();

	[System.Serializable]
	public class LookAtRotateTowardsSettings
	{

		[Header("Speed")]
		[Range(0.0001f, 9999)]
		[Tooltip("How quickly the object will rotate.")]
		public float smoothRotateSpeed = 1.0f;
		
	}
	
	public LookAtRotateTowardsSettings RotateTowardsSettings = new LookAtRotateTowardsSettings();
	
	[Header("Destination Settings")]
	[Tooltip("Stop rotating after the object has reached the target direction once.")]
	public bool haltRotationOnCompletion = false;
	[Tooltip("Destroy the object after the object has reached the target direction once.")]
	public bool destroyObjectOnCompletion = false;
	[Tooltip("Destroy this script after the object has reached the target direction once.")]
	public bool destroyScriptOnCompletion = false;
	
	[Header("Debugging")]
	[Tooltip("Display script errors in the console.")]
	public bool debugConsole;
	[Tooltip("Display the target position and direction via cube primitive and red debug ray.")]
	public bool showTargetPosition;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script rotates an object to look at either a target transform or specified vector.";
    private string ScriptTags = "look at rotation rotate movement motion transform";
    private string ScriptCategory = "motion";

    //Private_________________________________________________________________________________________________________________________________

    private Vector3 currentPos;		//The current position of the affected object
	private Vector3 currentDir;		//The current forward direction of the affected object

	private Vector3 targetPos;		//The current position of the target object (not used if using manual direction mode)
	private Vector3 targetDir;		//The direction from where the affected object is located, to where the target object is located (calculated through subtraction of vectors)

	private Vector3[] returnArray;	//Stores the target position and direction vectors in a convenient array

	private GameObject debugCube;	//Debugging primitive used to show where the target position and direction are

	private bool active = false;	//The current state of the script


	void Start () {
		
		active = true;
		Setup ();	
	}


	void Setup() {
		
		//If there is no affected object, set it to the object this script is attached to
		if(!affectedObject) {
			affectedObject = transform;
		}
		
		//If the script is not command activated, automatically run it straight away
		if(!commandActivated) {
			active = true;
		}

		if(showTargetPosition) {
			SpawnDebugCube();
		}

		returnArray = new Vector3[2];

		LoadInfo();
		
	}


	void Update () {
		
		if(active) {
			Master();
		}
	}


	void Master() {
		
		LoadInfo();	//Grabs current position and checks if the destination is reached
		Engine ();	//Drives the affected object
		CheckDestination();	//Check to see if the affected object reached the desired target rotation
		
	}
	
	//Load all info. LookAt mode is instant and only needs a targetTransform
	void LoadInfo() {

		currentPos = affectedObject.position;
		currentDir = affectedObject.forward;
		
		if(continuouslyUpdateTargetPosition) {
			//Load relevant info depending on the target setting. 
			targetPos = GetTargetVectors()[0];
			targetDir = GetTargetVectors()[1];
			
			if(showTargetPosition) {
				debugCube.transform.position = targetPos;
				Debug.DrawRay(affectedObject.position, targetDir * 100.0f, Color.red);
			}
		}
		
	}


	//Checks if the destination is reached
	void CheckDestination() {

		//If destination is reached
		if(currentDir == Vector3.ClampMagnitude(targetDir, 1)) {

			//Destroy the object if enabled
			if(destroyObjectOnCompletion) {
				if(debugConsole) {
					print ("Reached destination. Destroying object: " + affectedObject.name);
				}
				Destroy(affectedObject.gameObject);
			}
			
			//Destroy the script if enabled
			if(destroyScriptOnCompletion) {
				if(debugConsole) {
					print ("Reached destination. Destroying script.");
				}
				Destroy(gameObject.GetComponent<LookAt>());
			}
			
			//Halt rotation by deactivating the script if enabled
			if(haltRotationOnCompletion) {
				print ("Reached destination. Disabling rotation.");
				active = false;
			}
		}
		
	}


	//Drives the affected object
	void Engine() {

		switch(rotateMode) {

		case mode.LookAt:

			affectedObject.LookAt(GetTargetVectors()[0], LookAtSettings.lookAtUpVector);

			break;

		case mode.RotateTowards:

			float step = RotateTowardsSettings.smoothRotateSpeed * Time.deltaTime;	//Calculates how far the object will rotate towards the target this frame
			Vector3 newDir = Vector3.RotateTowards(currentDir, targetDir, step, 0.0f);
			affectedObject.rotation = Quaternion.LookRotation(newDir);

			break;

		}

	}


	//Returns both the position of the target (index 0), and the direction to that target (index 1)
	Vector3[] GetTargetVectors() {

		switch(targetMode) {
			
		case tMode.LoadedTransform:

			if(LoadedTransform.targetTransform) {

				returnArray[0] = LoadedTransform.targetTransform.position;	//Use the loaded transform's position to calculate a direction

			} else {

				if(debugConsole) {
					print ("ERROR: Missing targetTransform on object: " + gameObject.name);	//Else the user has forgotten to specify a target object
				}
			}

			returnArray[1] = targetPos - currentPos;					//Calculate the direction
			return returnArray;
			
		case tMode.ManualDirection:

			if(debugConsole && rotateMode == mode.LookAt) {
				print ("ERROR: Cannot use Manual Direction while in LookAt mode. Use Rotate Towards with a high speed instead.");
			}

			returnArray[0] = Vector3.zero;							//Position is not needed because the direction is already given
			returnArray[1] = ManualDirection.targetDirection;		//Return the manually user inputted direction in slot 1
			return returnArray;
			
		case tMode.ManualPosition:

			returnArray[0] = ManualPosition.targetVectorPos;		//Use the manually inputted user position to calculate a direction
			returnArray[1] = targetPos - currentPos;				//Calculate the direction
			return returnArray;
			
		}

		if(debugConsole) {
			print ("ERROR: Issue with GetTargetVectors function ignoring switch cases.");
		}
		return returnArray;
	}


	//Allows other scripts to plug in and insert a target position
	public void ReceiveNewTargetPos(Vector3 inputTargetPos, bool disableContinuousUpdate) {

		//Disable normal continuous updating
		if(disableContinuousUpdate) {
			continuouslyUpdateTargetPosition = false;
		}

		//Set the target mode to manual position
		if(targetMode != tMode.ManualPosition) {
			targetMode = tMode.ManualPosition;
		}

		targetPos = inputTargetPos;
	}

	//Allows other scripts to plug in and insert a target direction
	public void ReceiveNewTargetDir(Vector3 inputTargetDir, bool disableContinuousUpdate) {

		//Disable normal continuous updating
		if(disableContinuousUpdate) {
			continuouslyUpdateTargetPosition = false;
		}

		//Set the target mode to manual direction
		if(targetMode != tMode.ManualDirection) {
			targetMode = tMode.ManualDirection;
		}

		targetDir = inputTargetDir;
	}

	//Completely resets the script
	public void Reset() {
		active = true;
		Setup ();
	}

	//Spawns a cube primitive for use in showing where the target position is located
	void SpawnDebugCube() {

		debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		debugCube.transform.position = targetPos;
		debugCube.transform.localScale = affectedObject.lossyScale * 2.0f;
		debugCube.GetComponent<Renderer>().material.color = Color.red;
	}


    //Enables or disables the script's update function
    public void ToggleScript(bool state)
    {
        active = state;
        this.enabled = state;
        if (debugConsole)
        {
            print("Setting active state of " + this.GetType().Name + " script to: " + state + " at time: " + Time.time);
        }
    }
}
