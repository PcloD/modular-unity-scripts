/* Script created by Lachlan McKay 2016 ©
 * This script maintains the distance between this object and a target at all times */

using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour
{

	[Header("Main Options")]
	[Tooltip("The target transform to follow.")]
	public Transform targetTransform;
	public bool useLateUpdate;

	[Header("Angle Controls")]
	[Range(-1,1)]
	public float yaw = 0;
	[Range(-1,1)]
	public float pitch = 0;
	[Range(-1,1)]
	public float roll = 0;
	public bool lookAtTarget = true;

	[Header("Position Controls")]
	[Tooltip("How far from the Target Transform should this camera be located? Works on a specific axis only if following is enabled for that axis.")]
	public Vector3 distanceFromTarget = Vector3.zero;
	[Tooltip("How far from this camera's starting position should this camera be located? Works independently of the ignore axis system.")]
	public Vector3 offsetVector = Vector3.zero;
	[Tooltip("Take into account the existing distance between this camera and the Target Transform before the scene starts.")]
	public bool addExistingDistance = true;

	[Header("Don't Follow Axis")]
	[Tooltip("Do not follow the Target Transform on the X axis.")]
	public bool ignoreX;
	[Tooltip("Do not follow the Target Transform on the Y axis.")]
	public bool ignoreY;
	[Tooltip("Do not follow the Target Transform on the Z axis.")]
	public bool ignoreZ;

	//Debugging
	[Header("Debug Options")]
	public bool debugConsole = false;
	[Tooltip("Allows manual editing of yaw/pitch/roll values")]
	public bool disableAngleControl = true;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2,2)]
	public string ScriptDescription = "This script maintains the distance between this object and a target at all times.";
    private string ScriptTags = "follow camera motion movement transform stick maintain space distance gap";
    private string ScriptCategory = "motion";

    //Private
    private bool active = false;

	private Vector3 cmdOffsetVector;

	private enum move {none, peek};
	private move maneuvre;

	private float maneuvreTimer;

	private Vector3 peekOrigin;
	private float peekTransition;
	private Vector3 peekIgnore;
	private bool atTargetPos;

	//Lerping
	private float lastYaw;
	private float lastPitch;
	private float lastRoll;
	private float targetYaw;
	private float targetPitch;
	private float targetRoll;

	private Vector3 lerpOffsetStart;
	private Vector3 lerpTargetOffset;
	private float offsetDuration;

	private float lerpOffsetStartTime;
	private Vector3 lerpRotStartTime;
	private Vector3 rotateDuration;

	private Vector3 startDistance;
	private Vector3 finalPosVector;

	private Vector3 naturalPos;
	private Quaternion naturalRot;

	void Start () {
	
		if(ErrorChecking()) {
			Setup();
		}
	}

	bool ErrorChecking() {

		if(!targetTransform) {
			if(debugConsole) {
				print("ERROR: No target transform set on object: " + gameObject.name);
			}
			return false;
		}

		return true;
	}

	void Setup() {

		if(addExistingDistance) {
			startDistance = transform.position - targetTransform.position;
		}

		lerpOffsetStart = Vector3.zero;

		naturalRot = transform.localRotation;
		naturalPos = transform.position;

		active = true;
	}

	void Update () {
	
		if(!useLateUpdate) {
			Master();
		}
	}

	void LateUpdate () {

		if(useLateUpdate) {
			Master();
		}
	}

	void Master() {
		
		if(active) {
			ModifyCam();
			Engine();
		}
	}

	void ModifyCam() {

		AngleControl();
		PositionControl();
	}

	//Moves the camera to a new desired angle if told to do so
	void AngleControl() {

		if(!disableAngleControl) {
			if(yaw != targetYaw) {
				float tParam = (Time.time - lerpRotStartTime.x) / rotateDuration.x;
				yaw = Mathf.Lerp(lastYaw, targetYaw, tParam);
			}

			if(pitch != targetPitch) {
				float tParam = (Time.time - lerpRotStartTime.y) / rotateDuration.y;
				pitch = Mathf.Lerp(lastPitch, targetPitch, tParam);
			}

			if(roll != targetRoll) {
				float tParam = (Time.time - lerpRotStartTime.z) / rotateDuration.z;
				roll = Mathf.Lerp(lastRoll, targetRoll, tParam);
			}
		}

		Vector3 natVector = naturalRot.eulerAngles;
		Vector3 rotVector = new Vector3(natVector.x + (pitch * 360), natVector.y + (yaw * 360), natVector.z + (roll * 360));

		transform.localRotation = Quaternion.Euler(rotVector);
	}

	//Moves the camera to a new desired position if told to do so
	void PositionControl() {

		if(offsetDuration > 0) {
			
			float tParam = Mathf.Clamp01((Time.time - lerpOffsetStartTime) / offsetDuration);
			cmdOffsetVector = Vector3.Lerp(lerpOffsetStart, lerpTargetOffset, tParam);

			if(tParam == 1) {
				atTargetPos = true;
			} else {
				atTargetPos = false;
			}
		}
			
		transform.position = GetFinalPosition();
	}

	//Gets the final position that the camera should be located in for this frame depending on multiple variables
	Vector3 GetFinalPosition() {

		if(!ignoreX) {
			finalPosVector.x = targetTransform.position.x + startDistance.x + distanceFromTarget.x + offsetVector.x + cmdOffsetVector.x;
		} else {
			finalPosVector.x = naturalPos.x + offsetVector.x + cmdOffsetVector.x;
		}

		if(!ignoreY) {
			finalPosVector.y = targetTransform.position.y + startDistance.y + distanceFromTarget.y + offsetVector.y + cmdOffsetVector.y;
		} else {
			finalPosVector.y = naturalPos.y + offsetVector.y + cmdOffsetVector.y;
		}

		if(!ignoreZ) {
			finalPosVector.z = targetTransform.position.z + startDistance.z + distanceFromTarget.z + offsetVector.z + cmdOffsetVector.z;
		} else {
			finalPosVector.z = naturalPos.z + offsetVector.z + cmdOffsetVector.z;
		}

		return finalPosVector;
	}

	//Receives commands to change the camera's angle to another
	public void ModifyAngle(string axis, float target, float transitionDuration) {

		switch(axis) {

		case "Yaw":
			
			lastYaw = yaw;
			targetYaw = Mathf.Clamp(target, -1, 1);
			lerpRotStartTime.x = Time.time;
			rotateDuration.x = transitionDuration;
			break;
		case "Pitch":
			
			lastPitch = pitch;
			targetPitch = Mathf.Clamp(target, -1, 1);
			lerpRotStartTime.y = Time.time;
			rotateDuration.y = transitionDuration;
			break;
		case "Roll":
			
			lastRoll = roll;
			targetRoll = Mathf.Clamp(target, -1, 1);
			lerpRotStartTime.z = Time.time;
			rotateDuration.z = transitionDuration;
			break;
		}
	}

	//Receives commands to change the camera's offset to another
	public void ModifyOffset(Vector3 offset, float transitionDuration, Vector3 ignoreXYZ) {

		lerpOffsetStart = cmdOffsetVector;

		if(ignoreXYZ.x != 1) {
			lerpTargetOffset.x = offset.x;
		} else {
			lerpTargetOffset.x = lerpOffsetStart.x;
		}

		if(ignoreXYZ.y != 1) {
			lerpTargetOffset.y = offset.y;
		} else {
			lerpTargetOffset.y = lerpOffsetStart.y;
		}

		if(ignoreXYZ.z != 1) {
			lerpTargetOffset.z = offset.z;
		} else {
			lerpTargetOffset.z = lerpOffsetStart.z;
		}

		lerpOffsetStartTime = Time.time;
		offsetDuration = transitionDuration;

	}

	public void Peek(Vector3 origin, Vector3 targetOffset, float transitionDuration, float holdDuration, Vector3 ignoreXYZ) {

		peekOrigin = origin;
		peekTransition = transitionDuration;
		peekIgnore = ignoreXYZ;

		ModifyOffset(targetOffset, peekTransition, ignoreXYZ);
		maneuvreTimer = holdDuration;
		maneuvre = move.peek;
	}

	//Controls camera maneuvres
	void Engine() {

		if(atTargetPos && maneuvreTimer > 0) {
			maneuvreTimer -= Time.deltaTime;
		}

		switch(maneuvre) {

		case move.peek:		//Peek maneuvre returns to the original position after holding in a target offset for a given duration
			if(maneuvreTimer <= 0) {
				ModifyOffset(peekOrigin, peekTransition, peekIgnore);
				maneuvre = move.none;
			}
			break;
		}
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
