/* Script created by Lachlan McKay 2016 ©
 * This script sends a message to a target object on mouse click */

using UnityEngine;
using System.Collections;

public class MouseTrigger : MonoBehaviour {
	
	[Header("Main Options")]
	[Tooltip("The target to send message to.")]
	public Transform target;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public string message;
	public int mouseID = 0;
	public cMode clickMode;
	public enum cMode {MouseDown, MouseUp, Hold}


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
	public string ScriptDescription = "This script allows you to send messages to other scripts on mouse click.";
    private string ScriptTags = "mouse trigger click send message msg event on mouse down onmouseclick";
    private string ScriptCategory = "trigger";

    private bool active = true;

    void Setup() {
		
		if(!target) {
			if(debugConsole) {
				print ("ERROR: Target has not been assigned in the inspector on object: " + gameObject.name + ". Using self instead.");
			}
			target = transform;
		}
		
	}

	void Update() {

        if(active)
        {
            Master();
        }
	}

    void Master()
    {
        switch (clickMode)
        {

            case cMode.MouseDown:
                if (Input.GetMouseButtonDown(mouseID))
                {
                    SendUserMessage();
                }
                break;

            case cMode.MouseUp:
                if (Input.GetMouseButtonUp(mouseID))
                {
                    SendUserMessage();
                }
                break;

            case cMode.Hold:
                if (Input.GetMouseButton(mouseID))
                {
                    SendUserMessage();
                }
                break;

        }
    }

	//Executes the send message command
	void SendUserMessage() {
		
		if(!sendParameter) {
			
			target.SendMessage(message);	//Send the message without a parameter
			
		} else {
			
			//Send the message with a parameter. Detect which type based on user drop down selection.
			switch(paramType) {
				
			case pMode.BOOL:
				target.SendMessage(message, User.userBool);
				break;
			case pMode.INT:
				target.SendMessage(message, User.userInt);
				break;
			case pMode.FLOAT:
				target.SendMessage(message, User.userFloat);
				break;
			case pMode.STRING:
				target.SendMessage(message, User.userString);
				break;
			case pMode.VECTOR2:
				target.SendMessage(message, User.userVector2);
				break;
			case pMode.VECTOR3:
				target.SendMessage(message, User.userVector3);
				break;
			case pMode.VECTOR4:
				target.SendMessage(message, User.userVector4);
				break;
			case pMode.QUATERNION:
				target.SendMessage(message, User.userQuaternion);
				break;
			case pMode.TRANSFORM:
				target.SendMessage(message, User.userTransform);
				break;
			case pMode.GAMEOBJECT:
				target.SendMessage(message, User.userGameObject);
				break;
			case pMode.COLOR:
				target.SendMessage(message, User.userColor);
				break;
			case pMode.COLOR32:
				target.SendMessage(message, User.userColor32);
				break;
			case pMode.BYTE:
				target.SendMessage(message, User.userByte);
				break;
				
			}
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
