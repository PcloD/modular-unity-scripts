/* Script created by Lachlan McKay 2016 ©
 * This script rotates an object to look in the direction of its movement */

using UnityEngine;
using System.Collections;

public class RotateToVelocity : MonoBehaviour
{

	[Header("Transforms")]
	[Tooltip("The transform to get velocity from when calculating the movement direction. Leave blank for self.")]
	public Transform getVelocityFrom;
	[Tooltip("The transform to rotate to match above transform's movement direction. Leave blank for self.")]
	public Transform applyRotationTo;

	[Header("Space")]
	[Tooltip("Get velocity based on world position or local position?")]
	public Space positionSpace = Space.World;
	[Tooltip("Apply rotation locally or globally?")]
	public Space rotateSpace = Space.World;

	[Header("Settings")]
	[Tooltip("Offset the rotation of the Apply Rotation To transform.")]
	public Vector3 rotationOffset = Vector3.zero;
	[Tooltip("How quickly the Apply Rotation To transform will turn to the calculated velocity (Note: has no effect when using instant interpolation).")]
	public float rotationSpeed = 650f;
	[Tooltip("The minimum change in velocity required before the Apply Rotation To transform will actually rotate. Useful for preventing jitter.")]
	public float minimumVelocityChange = 0.0075f;

	[Header("Interpolation Method")]
	[Tooltip("How the Apply Rotation To transform will interpolate to the calculated velocity.")]
	public mode lerpMode = mode.Linear;
	public enum mode {Instant, Linear}


	[Header("Clamping")]
	[Tooltip("Clamp the rotation of the Apply Rotation To transform? Useful for ignoring velocity changes on a certain axis.")]
	public bool clampLookVector = false;
	[Tooltip("(Minimum, Maximum)")]
	public Vector2 clampX;
	[Tooltip("(Minimum, Maximum)")]
	public Vector2 clampY;
	[Tooltip("(Minimum, Maximum)")]
	public Vector2 clampZ;

	//Private
	private Vector3 velocity;
	private Vector3 lastVelocity;
	private float velocityMagDelta;
	private Vector3 currentPos;
	private Vector3 lastPos;
	private Quaternion targetRotation;

	//Debugging
	[Header("Debug Options")]
	[Tooltip("Display errors in the console.")]
	public bool debugConsole = false;
	[Tooltip("Display the velocity and target rotation in the scene view via debug rays.")]
	public bool showRays = false;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script rotates an object to look in the direction of its movement.";
    private string ScriptTags = "rotate to velocity movement rotation rot transform aim";
    private string ScriptCategory = "motion";

    private bool active;

    void Start () {
		Setup();
	}

	//Sets up inspector transforms
	void Setup() {
		
		if(!getVelocityFrom) {
			getVelocityFrom = transform;
		}
		
		if(!applyRotationTo) {
			applyRotationTo = transform;
		}

	}

	void Update () {

        if (active)
        {
            Engine();
            DebuggingInfo();
        }

	}

	//Calculates velocity and resulting target rotation based on the position delta between this frame and last frame
	void Engine() {

		currentPos = GetPosition();
		velocity = currentPos - lastPos;

		velocityMagDelta = Mathf.Abs(velocity.magnitude - lastVelocity.magnitude);

		lastVelocity = velocity;
		lastPos = GetPosition();

		if(velocity != Vector3.zero && velocityMagDelta > minimumVelocityChange) {

			targetRotation = Quaternion.Euler(Quaternion.LookRotation(velocity).eulerAngles + rotationOffset);
			Clamp();
		}

		Interpolate();

	}

	//Applies rotation to the ApplyRotationTo transform based on which interpolation method is selected
	void Interpolate() {

		switch(lerpMode) {
			
		case mode.Instant:
			
			ApplyRotation(targetRotation);
			break;
			
		case mode.Linear:
			
			float step = rotationSpeed * Time.deltaTime;	//Calculates how far the object will rotate towards the target this frame
			Quaternion newDir = Quaternion.RotateTowards(GetRotation(), targetRotation, step);
			ApplyRotation(newDir);
			break;
		}
	}

	//Clamps the targetRotation to be within the inspector clamp values
	void Clamp() {

		if(clampLookVector) {

			Vector3 targetVector = targetRotation.eulerAngles;

			targetVector.x = Mathf.Clamp(targetVector.x, clampX.x, clampX.y);
			targetVector.y = Mathf.Clamp(targetVector.y, clampY.x, clampY.y);
			targetVector.z = Mathf.Clamp(targetVector.z, clampZ.x, clampZ.y);

			targetRotation = Quaternion.Euler(targetVector);
		}
	}

	//Applies a given rotation to the applyRotation to transform based on the rotateSpace dropdown in the inspector
	void ApplyRotation(Quaternion rot) {

		switch(rotateSpace) {

		case Space.Self:

			applyRotationTo.localRotation = rot;
			break;
		case Space.World:
		default:
			applyRotationTo.rotation = rot;
			break;
		}
	}

	//Gets the current rotation of the applyRotationTo transform
	Quaternion GetRotation() {

		switch(rotateSpace) {
			
		case Space.Self:
			return applyRotationTo.localRotation;
			
		case Space.World:
		default:
			return applyRotationTo.rotation;
		}
	}

	//Gets the current position of the getVelocityFrom transform
	Vector3 GetPosition() {

		switch(positionSpace) {

		case Space.Self:
			return getVelocityFrom.localPosition;

		case Space.World:
		default:
			return getVelocityFrom.position;
		}
	}

	//Displays debugging information
	void DebuggingInfo() {

		if(showRays) {
			Debug.DrawRay(currentPos, velocity * 10f, Color.red);
			Debug.DrawRay(applyRotationTo.position, targetRotation * Vector3.forward * 10f, Color.green);
			Debug.DrawRay(applyRotationTo.position, applyRotationTo.transform.forward + rotationOffset, Color.green);
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
