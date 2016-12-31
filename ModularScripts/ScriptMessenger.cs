/* Script created by Lachlan McKay 2016 ©
 * This script allows you to send messages to other scripts or toggle them based on a timer */

using UnityEngine;
using System.Collections;

public class ScriptMessenger : MonoBehaviour {

	[Header("Main Options")]
	[Tooltip("The object that will receive the message.")]
	public Transform targetObject;
	[Tooltip("The message that will be sent to the target object.")]
	public string message = "Enter a function name for this script to SendMessage to here.";
	
	[Header("Toggle Settings")]
	[Tooltip("Toggle an object instead of sending a message.")]
	public GameObject objectToToggle;

	[Header("Timing Settings")]
	[Tooltip("Instant One Shot: Message is sent instantly on start. DelayedOneShot: Message is sent once after a period of time. Repeating: Message is sent repeatededly in between a set interval. DelayedRepeating: Message is sent repeatedly in between a set interval after a period of time. Command Activated: Message is only sent when told to by another script. ")]
	public tMode timeMode;
	public enum tMode {InstantOneShot, DelayedOneShot, Repeating, DelayedRepeating, CommandActivated}
	[Space(7)]
	[Tooltip("The amount of time before the message will be sent when using Delayed One Shot or Delayed Repeating.")]
	public float delayPeriod = 1.0f;
	[Tooltip("The amount of time between interval sends when using Repeating or Delayed Repeating.")]
	public float repeatingInterval = 1.0f;
	[Tooltip("The amount of time the message will continually repeat when using Repeating or Delayed Repeating. SET TO -1 FOR INFINITE REPEAT.")]
	public float repeatDuration = -1.0f;

	[Header("Parameter Settings")]
	[Tooltip("Send a parameter when executing the SendMessage function?")]
	public bool sendParameter;
	public pMode paramType;
	public enum pMode {BOOL, INT, FLOAT, STRING, VECTOR2, VECTOR3, VECTOR4, QUATERNION, TRANSFORM, GAMEOBJECT, COLOR, COLOR32, BYTE}

	[System.Serializable]
	public class ScriptMessengerUser
	{
		[Header("User Parameter Input")]
		public bool userBool;
		public int userInt;
		public float userFloat;
		public string userString;
		public Vector2 userVector2;
		public Vector3 userVector3;
		public Vector4 userVector4;
		public Quaternion userQuaternion;
		public Transform userTransform;
		public GameObject userGameObject;
		public Color userColor;
		public Color32 userColor32;
		public byte userByte;

	}
	
	public ScriptMessengerUser User = new ScriptMessengerUser();
	
	[Header("Debugging")]
	[Tooltip("Display script errors in the console.")]
	public bool debugConsole;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script allows you to send messages to other scripts or toggle them based on a timer.";
    private string ScriptTags = "script messenger timer message trigger";
    private string ScriptCategory = "trigger";

    void Start () {
		Setup();
	}

	void Setup() {

		if(!targetObject) {
			if(debugConsole) {
				print ("ERROR: Target Object has not been assigned in the inspector on object: " + gameObject.name + ". Using self instead.");
			}
			targetObject = transform;
		}

		switch(timeMode) {
			
		case tMode.InstantOneShot:
			Execute();	//Instantly send the message or toggle
			break;
			
		case tMode.DelayedOneShot:
			StartCoroutine("DelayedSend");	//Send the message or toggle after a delay
			break;
			
		case tMode.Repeating:
			StartCoroutine("RepeatingSend");	//Start repeating the message or toggle
			break;
			
		case tMode.DelayedRepeating:
			StartCoroutine("DelayedRepeatingSend");	//Start repeating the message or toggle after a delay
			break;
		}

		if(repeatDuration != -1) {
			StartCoroutine("StopAllRoutines");
		}

	}

	//Executes the send message command
	public void SendUserMessage() {

		if(!sendParameter) {

			targetObject.SendMessage(message);	//Send the message without a parameter

		} else {

			//Send the message with a parameter. Detect which type based on user drop down selection.
			switch(paramType) {
				
			case pMode.BOOL:
				targetObject.SendMessage(message, User.userBool);
				break;
			case pMode.INT:
				targetObject.SendMessage(message, User.userInt);
				break;
			case pMode.FLOAT:
				targetObject.SendMessage(message, User.userFloat);
				break;
			case pMode.STRING:
				targetObject.SendMessage(message, User.userString);
				break;
			case pMode.VECTOR2:
				targetObject.SendMessage(message, User.userVector2);
				break;
			case pMode.VECTOR3:
				targetObject.SendMessage(message, User.userVector3);
				break;
			case pMode.VECTOR4:
				targetObject.SendMessage(message, User.userVector4);
				break;
			case pMode.QUATERNION:
				targetObject.SendMessage(message, User.userQuaternion);
				break;
			case pMode.TRANSFORM:
				targetObject.SendMessage(message, User.userTransform);
				break;
			case pMode.GAMEOBJECT:
				targetObject.SendMessage(message, User.userGameObject);
				break;
			case pMode.COLOR:
				targetObject.SendMessage(message, User.userColor);
				break;
			case pMode.COLOR32:
				targetObject.SendMessage(message, User.userColor32);
				break;
			case pMode.BYTE:
				targetObject.SendMessage(message, User.userByte);
				break;
				
			}
		}
	}

	//COROUTINES________________________________________________________________
	
	//Sends the message after a delay
	IEnumerator DelayedSend() {
		
		yield return new WaitForSeconds(delayPeriod);
		if(debugConsole) {
			print ("Executed the sendMessage after a delay period of: " + delayPeriod + " second(s)");
		}
		Execute();
		StopCoroutine("DelayedSend");
	}
	
	//Starts repeating the message between intervals
	IEnumerator RepeatingSend() {
		for(;;) {
			if(debugConsole) {
				print ("Sending repeat message at time: " + Time.time);
			}

			Execute();
			yield return new WaitForSeconds(repeatingInterval);
		}
	}
	
	//Starts repeating the message between intervals after a delay
	IEnumerator DelayedRepeatingSend() {
		
		yield return new WaitForSeconds(delayPeriod);
		if(debugConsole) {
			print ("Started repeat sending after a delay period of: " + delayPeriod + " second(s)");
		}
		StartCoroutine("RepeatingSend");
		StopCoroutine("DelayRepeatingSend");
	}
	
	//Stops all routines after a duration
	IEnumerator StopAllRoutines() {
		
		yield return new WaitForSeconds(repeatDuration);
		if(debugConsole) {
			print ("Stopping all sending after a delay period of: " + repeatDuration + " seconds");
		}
		StopCoroutine("DelayedSend");
		StopCoroutine("RepeatingSend");
		StopCoroutine("DelayRepeatingSend");
		StopCoroutine("StopAllRoutines");
	}

	void Execute() {

		if(!objectToToggle) {
			SendUserMessage();
		} else {
			objectToToggle.SetActive(!objectToToggle.activeSelf);
		}

	}
}
