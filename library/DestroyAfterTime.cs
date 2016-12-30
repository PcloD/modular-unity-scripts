/* Script created by Lachlan McKay 2016 ©
 * This script allows you to destroy an object after a period of time has elapsed */

using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{

	[Header("Main Options")]
	[Tooltip("Toggle an object instead of sending a message.")]
	public GameObject objectToDestroy;
	[Tooltip("The amount of time before the message will be sent when using Delayed One Shot or Delayed Repeating.")]
	[Range(0,900f)]
	public float delayPeriod = 1.0f;
	
	[Header("Debugging")]
	[Tooltip("Display script errors in the console.")]
	public bool debugConsole;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script allows you to destroy an object after a period of time has elapsed.";
    private string ScriptTags = "destroy remove turn off disappear delete after time timer destruct";
    private string ScriptCategory = "spawning";

    void Start () {
		Setup();
	}

	void Setup() {

		if(!objectToDestroy) {
			if(debugConsole) {
				print ("ERROR: objectToDestroy has not been assigned in the inspector on object: " + gameObject.name);
			}
		} else {
			Execute();	//Instantly send the message or toggle
		}
	}

	void Execute() {

		if(delayPeriod > 0) {
			Destroy(objectToDestroy, delayPeriod);
		} else {
			Destroy(objectToDestroy);
		}
	}
}
