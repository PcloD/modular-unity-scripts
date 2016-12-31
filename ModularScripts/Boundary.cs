/* Script created by Lachlan McKay 2016 ©
 * This script allows you to clamp the transform of an object */

using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour {

	[Header("Main Options")]
	public Transform affectedObject;
	[Tooltip("Sets the type of limit to enforce.")]
	public axis axisType;
	public enum axis {Position, Rotation, Scale}
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	[Tooltip("Enable this if you want the script to execute slightly after other scripts.")]
	public bool useLateUpdate = false;

	[Header("Boundaries")]
	public Vector2 boundsX;
	public Vector2 boundsY;
	public Vector2 boundsZ;

	[Header("Debug Options")]
	public bool debugConsole;

	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script allows you to clamp the transform of an object.";
    private string ScriptTags = "boundary limit motion movement move wall transform clamp restrict";
    private string ScriptCategory = "motion";

    //Private 
    private Vector3 currentVector;
	private bool active;

	void Start () {
		active = true;
		Setup();
	}

	void Setup() {

	    if(!affectedObject) {
			affectedObject = transform;
		}

		currentVector = GetVector();
	}

	void Update () {
	
		if(!useLateUpdate) {
			Master();
		}
	}

	void LateUpdate() {

		if(useLateUpdate) {
			Master();
		}
	}

	void Master() {

		if(active) {
			ProcessInfo();
			LimitAxises();
			ApplyVector();
		}

	}

	void ProcessInfo() {

		currentVector = GetVector();

	}

	void LimitAxises() {

		currentVector.x = Mathf.Clamp(currentVector.x, boundsX.x, boundsX.y);
		currentVector.y = Mathf.Clamp(currentVector.y, boundsY.x, boundsY.y);
		currentVector.z = Mathf.Clamp(currentVector.z, boundsZ.x, boundsZ.y);

	}

	//Returns the vector
	Vector3 GetVector() {

		switch(axisType) {
			
		case axis.Position:
			return affectedObject.position;
			
		case axis.Rotation:
			return affectedObject.rotation.eulerAngles;
			
		case axis.Scale:
			return affectedObject.localScale;
			
		}

		print ("ERROR: axisType is null");
		return Vector3.zero;
	}

	void ApplyVector() {

		switch(axisType) {
			
		case axis.Position:
			affectedObject.position = currentVector;
			break;
			
		case axis.Rotation:
			affectedObject.rotation = Quaternion.Euler(currentVector);
			break;
			
		case axis.Scale:
			affectedObject.localScale = currentVector;
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
