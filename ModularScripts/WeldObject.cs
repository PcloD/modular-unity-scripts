/* Script created by Lachlan McKay 2016
 * This script sticks an object to another's world position at all times */

using UnityEngine;
using System.Collections;

public class WeldObject : MonoBehaviour
{

	[Header("Main Options")]
	[Tooltip("Leave blank to use this object's transform.")]
	public Transform affectedObject;
	[Tooltip("The transform to copy to the affected object.")]
	public Transform target;

	[Header("Ignore")]
	public bool ignoreX;
	public bool ignoreY;
	public bool ignoreZ;

	//Debugging
	[Header("Debug Options")]
	public bool debugConsole = false;

	//Credits and description
	[Header("_Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script sticks an object to another's world position at all times.";
    private string ScriptTags = "weld object to target stick together glue anchor transform same occupy close";
    private string ScriptCategory = "motion";

    private bool active = false;

	void Start() {

		if(ErrorChecking()) {
		
			if(!affectedObject) {
				affectedObject = transform;
			}

			active = true;
		}
	}

	bool ErrorChecking() {

		if(!target) {
			if(debugConsole) {
				print("ERROR: No target transform set on object: " + gameObject.name);
			}
			return false;
		}

		return true;
	}

	void Update () {
	
		if(active) {
			Vector3 targetVector = target.position;

			if(ignoreX) {
				targetVector.x = affectedObject.position.x;
			}
			if(ignoreY) {
				targetVector.y = affectedObject.position.y;
			}
			if(ignoreZ) {
				targetVector.z = affectedObject.position.z;
			}

			affectedObject.position = targetVector;
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
