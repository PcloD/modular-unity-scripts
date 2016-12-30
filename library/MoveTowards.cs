/* Script created by Lachlan McKay 2016 ©
 * This script allows you to make an object move towards a target object or manually specified position over time
 * Useful for simple enemies that move towards a target or location */

using UnityEngine;
using System.Collections;

public class MoveTowards : MonoBehaviour
{

	[Header("Main Options")]
	public Transform affectedObject;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated;
	
	[Header("Target Settings")]
	public Transform targetTransform;
	[Tooltip("Enable this if the target will be moving and you want the affected object to follow it.")]
	public bool continouslyUpdateTargetPosition = true;

	[Header("Speed")]
	[Range(0.0001f, 9999)]
	public float moveSpeed = 1.0f;

	[Header("Specify Vector (Overrides target transform)")]
	public bool useTargetVector;
	public Vector3 targetVector;
	
	[Header("Destination Settings")]
	public bool haltMovementOnCompletion = false;
	public bool destroyObjectOnCompletion = false;
	public bool destroyScriptOnCompletion = false;

	[Header("Debugging")]
	public bool debugConsole;

	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script moves an object's position towards either a target transform or specified vector.";
    private string ScriptTags = "move towards movement motion transform engine";
    private string ScriptCategory = "motion";

    //Private
    private Vector3 currentPos;
	private Vector3 targetPos;

	private bool active = false;


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

		//Set the currentPos vector to where the object is right now
		currentPos = affectedObject.position;

		if(useTargetVector) {

			targetPos = targetVector;	//If we are using a target vector, set targetPos vector to what the user specified

		} else {

			targetPos = GetTargetTransformPos(); //If we are using a target transform on an object, grab that object's position
		}

	}

	void Update () {
	
		if(active) {
			Master();
		}
	}

	void Master() {

		ProcessInfo();	//Grabs current position and checks if the destination is reached
		Engine ();	//Drives the affected object

	}

	//Grabs current position and checks if the destination is reached
	void ProcessInfo() {

		currentPos = affectedObject.position;

		//If using a target transform object, and the user wants to continuously update it's position, get the new targetPos
		if(continouslyUpdateTargetPosition) {
			targetPos = GetTargetTransformPos(); 
		}

		//If destination is reached
		if(currentPos == targetPos) {

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
				Destroy(gameObject.GetComponent<MoveTowards>());
			}

			//Halt movement by deactivating the script if enabled
			if(haltMovementOnCompletion) {
				print ("Reached destination. Disabling rotation.");
				active = false;
			}
		}

	}

	//Drives the affected object
	void Engine() {

		float step = moveSpeed * Time.deltaTime;						//Calculates how far the object will move towards the target this frame
		currentPos = Vector3.MoveTowards(currentPos, targetPos, step);	//Translates the currentPos vector closer to targetPos vector by the step value

		affectedObject.position = currentPos;							//Sets the affected object's position to the currentPos vector

	}

	//Returns the current position of the targetTransform object
	Vector3 GetTargetTransformPos() {
		
		if(targetTransform) {
			
			return targetTransform.position;	//Return the target object's position if the object exists
			
		} else {
			
			if(debugConsole) {
				print ("ERROR: Missing targetTransform on object: " + gameObject.name);	//Else the user has forgotten to specify a target object
			}

			return Vector3.zero;	//Return zero
		}
		
	}

	//Allows other scripts to plug in and insert a target position
	public void ReceiveNewTargetPos(Vector3 inputTargetPos, bool disableContinuousUpdate) {

		if(disableContinuousUpdate) {
			continouslyUpdateTargetPosition = false;
		}

		targetPos = inputTargetPos;
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
