/* Script created by Lachlan McKay 2016 ©
 * This script moves an object in tandem with the mouse's viewport coordinates */

using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {
    
	[Header("Main Options")]
	[Tooltip("The transform around which particles will emit.")]
	public Transform affectedObject;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	[Tooltip("When specifying the center, use world or local coordinates?")]
	public Space spaceMode = Space.World;
	[Tooltip("Enable this if you want the script to execute slightly after other update functions.")]
	public bool useLateUpdate = false;

	[Header("Main Options")]
	public bool hideCursor = false;
	public Vector3 center;
	public Vector3 movementRange = new Vector3(10,10,10);

	[Header("Axis Options")]
	[Tooltip("Moving the mouse on the X axis should translate to which 3D coordinate movement?")]
	public axisX mouseXControls = axisX.X;
	public enum axisX {X, Y, Z}
	[Tooltip("Moving the mouse on the Y axis should translate to which 3D coordinate movement?")]
	public axisY mouseYControls = axisY.Z;
	public enum axisY {X, Y, Z}

	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script moves an object in tandem with the mouse's viewport coordinates.";
    private string ScriptTags = "mouse follow aim copy pointer move motion track";
    private string ScriptCategory = "motion";

    //Private
    private Vector2 currentMouseCoords;
	private Vector3 displacement;

	private bool active = false;

	void Start () {
		Setup();
	}

	void Setup() {

		if(!affectedObject) {
			affectedObject = transform;
		}

		if(ErrorChecking()) {

			if(hideCursor) {
				Cursor.visible = false;
			}
			
			if(!commandActivated) {
				active = true;
			}

		}
	}

	bool ErrorChecking() {

		if(mouseXControls == axisX.X && mouseYControls == axisY.X && mouseXControls == axisX.Y && mouseYControls == axisY.Y && mouseXControls == axisX.Z && mouseYControls == axisY.Z) {
			print ("ERROR: Mouse X and Y cannot control the same axis. Check the settings on object: " + gameObject.name);
		}

		return true;

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
			GetInput();
			Engine();
		}
	}

	void GetInput() {

		currentMouseCoords = new Vector2(Mathf.Clamp01(Input.mousePosition.x / Screen.width), Mathf.Clamp01(Input.mousePosition.y / Screen.height));
		currentMouseCoords.x = ReMap(currentMouseCoords.x, 0f, 1f, -1f, 1f);
		currentMouseCoords.y = ReMap(currentMouseCoords.y, 0f, 1f, -1f, 1f);
	}

	void Engine() {

		switch(mouseXControls) {

		case axisX.X:

			switch(mouseYControls) {
			case axisY.Y:
				displacement = new Vector3(currentMouseCoords.x * movementRange.x, currentMouseCoords.y * movementRange.y, 0);
				break;
			case axisY.Z:
				displacement = new Vector3(currentMouseCoords.x * movementRange.x, 0, currentMouseCoords.y * movementRange.z);
				break;
			}

			break;
		case axisX.Y:

			switch(mouseYControls) {
			case axisY.X:
				displacement = new Vector3(currentMouseCoords.y * movementRange.x, currentMouseCoords.x * movementRange.y, 0);
				break;
			case axisY.Z:
				displacement = new Vector3(0, currentMouseCoords.x * movementRange.y, currentMouseCoords.y * movementRange.z);
				break;
			}

			break;
		case axisX.Z:

			switch(mouseYControls) {
			case axisY.X:
				displacement = new Vector3(currentMouseCoords.y * movementRange.x, 0, currentMouseCoords.x * movementRange.z);
				break;
			case axisY.Y:
				displacement = new Vector3(0, currentMouseCoords.y * movementRange.y, currentMouseCoords.x * movementRange.z);
				break;
			}
			break;
		}

		Vector3 finalVector = center + displacement;
		affectedObject.position = finalVector;

	}

	float ReMap (float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
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
