/* Script created by Lachlan McKay 2016 ©
 * This script simply toggles an object's active state on or off when told to by another script or button */

using UnityEngine;
using System.Collections;

public class Toggle : MonoBehaviour {
	
	[Header("Initialisation")]
	[Tooltip("The object to turn on / off.")]
	public GameObject affectedObject;

	[Header("Debug Options")]
	public bool debugConsole;

	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script toggles an object's active state on or off when told to by another script or button. To use, simply SendMessage to this object 'ToggleActive' or 'SetActive(true/false)'.";
    private string ScriptTags = "toggle object on off message command invisible disable enable hide show display";
    private string ScriptCategory = "management";

    void Start () {
	
		if(!affectedObject) {
			affectedObject = gameObject;
		}
	}

	//Enables or disables the affectedObject's active state
	public void SetActive(bool state) {
		affectedObject.SetActive(state);
		if(debugConsole) {
			print ("Setting active state of object: " + affectedObject.name + " to: " + state + " at time: " + Time.time);
		}
	}

	//Toggles the affectedObject's active state
	public void ToggleActive() {

		bool state = !affectedObject.activeSelf;

		affectedObject.SetActive(state);
		if(debugConsole) {
			print ("Setting active state of object: " + affectedObject.name + " to: " + state + " at time: " + Time.time);
		}
	}
}
