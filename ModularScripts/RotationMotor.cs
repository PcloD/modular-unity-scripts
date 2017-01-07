/* Script created by Lachlan McKay 2016
 * This script rotates an object around a point on a given axis */

using UnityEngine;
using System.Collections;

public class RotationMotor : MonoBehaviour
{

	[Header("Main Options")]
	[Tooltip("Leave blank to use this object's transform.")]
	public Transform affectedObject;
	[Space(5)]
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	[Tooltip("Enable if you want to use the object's global transform values rather than what is shown in the inspector (local).")]
	public bool useGlobalTransforms = true;
	[Tooltip("Enable if you want the script to execute after other scripts have executed.")]
	public bool useLateUpdate = false;

	[Header("Engine")]
	public Transform rotateAround;
	public Vector3 rotationAxis = new Vector3(0,1,0);
	public float rotationSpeed = 10.0f;

    //Debugging
    [Header("Debug Options")]
    [Tooltip("Display errors in the console.")]
    public bool debugConsole = false;

    //Credits and description
    [Header("_Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script rotates an object around a point on a given axis.";
    private string ScriptTags = "rotate rotation motor engine movement rotation rot transform";
    private string ScriptCategory = "motion";

    private bool active;

    void Start () {
	
		if(!affectedObject) {
			affectedObject = transform;
		}

		if(!rotateAround) {
			rotateAround = affectedObject;
		}
        if(!commandActivated) { active = true; }
	}

	void Update () {
		if(!useLateUpdate) {
			Engine();
		}
	}

	void LateUpdate () {
		if(useLateUpdate) {
			Engine();
		}
	}

	void Engine() {
        if (active)
        {
            affectedObject.RotateAround(GetPosition(rotateAround), rotationAxis, rotationSpeed);
        }
	}

	Vector3 GetPosition(Transform input) {
		
		if(!useGlobalTransforms) {
			return input.localPosition;
		} else {
			return input.position;
		}
	}

	//Overrides the rotation speed variable via other script
	public void ModifySpeed(float inputSpeed) {
		rotationSpeed = inputSpeed;
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
