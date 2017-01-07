/* Script created by Lachlan McKay 2016 ©
 * This script interpolates an object between 2 vectors randomly */

using UnityEngine;
using System.Collections;

public class RandomInterpolator : MonoBehaviour
{

	[Header("Main Options")]
	[Tooltip("Leave blank to use this object's transform.")]
	public Transform affectedObject;
	[Space(5)]
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	[Tooltip("Enable if you want to use the object's global transform values rather than what is shown in the inspector (local).")]
	public bool useGlobalTransforms = false;

	[Header("Interpolation Method")]
	[Tooltip("Choose the feel of how the object moves.")]
	public engineMode engineType;
	public enum engineMode {Precise, PreciseSmooth, FreeLerp, FreeSlerp, PhysicsBased}

	[Header("Affected Axis")]
	public axis affectedAxis;
	public enum axis {Position, Rotation, Scale}

	[System.Serializable]
	public class RandomRotatorUniversalSettings
	{
		
		[Header("Origin Vector")]
		public bool useCurrentVector;
		public Vector3 originVector;
		
		[Header("Speed (Unit/Sec)")]
		public float speed = 1.0f;
		[Tooltip("The time taken to generate a new target vector.")]
		public float randomGenerationInterval = 1.0f;
		
		[Header("Limits")]
		public Vector3 minimumVector = Vector3.zero;
		public Vector3 maximumVector = Vector3.zero;
		public bool hardClampX = false;
        public bool hardClampY = false;
        public bool hardClampZ = false;

    }
	
	public RandomRotatorUniversalSettings UniversalSettings = new RandomRotatorUniversalSettings();

	[System.Serializable]
	public class RandomRotatorFreeSettings
	{
		
		[Header("Free Interpolation")]
		[Tooltip("The maximum speed that may be achieved.")]
		public float speedClamp = 10.0f;
		[Tooltip("The time taken to interpolate towards newly generated velocity.")]
		public float smoothTime = 2.0f;
		
	}
	
	public RandomRotatorFreeSettings FreeSettings = new RandomRotatorFreeSettings();

	[System.Serializable]
	public class RandomRotatorPhysicsBasedSettings
	{
		
		[Header("Physics Settings")]
		public float mass = 1.0f;
		public float drag = 0.0f;
		public float angularDrag = 0.05f;
		public bool useGravity = false;
		public bool isKinematic = false;

		public RigidbodyInterpolation interpolateMode;
		public CollisionDetectionMode collisionDetectionMode;
		public RigidbodyConstraints constraints;

	}
	
	public RandomRotatorPhysicsBasedSettings PhysicsBased = new RandomRotatorPhysicsBasedSettings();

	//Debugging
	[Header("Debug Options")]
	public bool debugConsole = false;

	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script interpolates an object between 2 vectors randomly. Hover over any property name for a description of its function.";
    private string ScriptTags = "random interpolator interpolate interpolation move rotation rotate movement random transform";
    private string ScriptCategory = "motion";

    //Private

    //Free moving
    private Vector3 currentVelocity;
	private Vector3 targetVelocity;
	private Vector3 realVelocity;

	private float tParam;
	private	float lastGenTime;

	private Vector3 clampVector;
	private Vector3 lastVector;
	private Vector3 randomVector;

	private bool active;

	private Rigidbody rb;

	void Start () {
		Setup ();
	}
	
	void Setup() {

		//If affected object is null, set it to self
		if(!affectedObject) {
			affectedObject = transform;
		}

		if(!commandActivated) {
			active = true;
		}

		if(engineType == engineMode.PhysicsBased) {
			rb = affectedObject.gameObject.AddComponent<Rigidbody>();
			SetupRigidBody();
		}

		StartCoroutine(GenerateNewRandomVector());
	}

	void SetupRigidBody() {

		if(rb) {

			rb.mass = PhysicsBased.mass;
			rb.drag = PhysicsBased.drag;
			rb.angularDrag = PhysicsBased.angularDrag;
			rb.useGravity = PhysicsBased.useGravity;
			rb.isKinematic = PhysicsBased.isKinematic;

			rb.constraints = PhysicsBased.constraints;
			rb.interpolation = PhysicsBased.interpolateMode;
			rb.collisionDetectionMode = PhysicsBased.collisionDetectionMode;

		}

	}

	void Update () {

		if(active) {
			Engine();
			DebugInfo();	//Shows debugging information
		}

	}

	void Engine() {

		switch(affectedAxis) {

			//POSITION
		case axis.Position:
		case axis.Scale:
			
			switch(engineType) {
				
			case engineMode.Precise:

				tParam = Mathf.Clamp01((Time.time - lastGenTime) / (1.0f/UniversalSettings.speed));
				SetAxis(Vector3.Lerp(lastVector, randomVector, tParam));
				break;

			case engineMode.PreciseSmooth:

				tParam = Mathf.Clamp01((Time.time - lastGenTime) / (1.0f/UniversalSettings.speed));
				tParam = tParam * tParam;
				SetAxis(Vector3.Lerp(lastVector, randomVector, tParam));

				break;

			case engineMode.FreeLerp:

				Vector3 VelocityFL = Vector3.MoveTowards(GetAxis(), randomVector, UniversalSettings.speed * Time.deltaTime); 
				SetAxis(VelocityFL);

				break;

			case engineMode.FreeSlerp:
				
				tParam = Mathf.Clamp01((Time.time - lastGenTime) / FreeSettings.smoothTime);
				currentVelocity = Vector3.Slerp(currentVelocity, targetVelocity, tParam);

				realVelocity += currentVelocity * Time.deltaTime * UniversalSettings.speed;
				realVelocity = Vector3.ClampMagnitude(realVelocity, FreeSettings.speedClamp);
				
				SetAxis(realVelocity);
				
				break;

			case engineMode.PhysicsBased:

				if(affectedAxis == axis.Position) {

					if(useGlobalTransforms) {
						rb.AddForce(randomVector * UniversalSettings.speed);
					} else {
						rb.AddRelativeForce(randomVector * UniversalSettings.speed);
					}
				} else if(affectedAxis == axis.Scale) {
					print ("ERROR: Scale does not work with a physics based rigidbody");
				}

				break;
			}

			break;

			//ROTATION
		case axis.Rotation:

			switch(engineType) {
				
			case engineMode.Precise:

				Vector3 rotationP = Vector3.RotateTowards(affectedObject.rotation.eulerAngles, randomVector, Time.deltaTime * Mathf.Deg2Rad, UniversalSettings.speed);
				SetAxis(rotationP);

				break;
				
			case engineMode.PreciseSmooth:

				Vector3 rotationPS = Quaternion.Slerp(affectedObject.rotation, Quaternion.Euler(randomVector), Time.deltaTime * UniversalSettings.speed).eulerAngles;
				SetAxis(rotationPS);

				break;

			case engineMode.FreeLerp:

				tParam = Mathf.Clamp01((Time.time - lastGenTime) / FreeSettings.smoothTime);
				currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, tParam);
	
				realVelocity = currentVelocity * Time.deltaTime * UniversalSettings.speed;
				realVelocity = Vector3.ClampMagnitude(realVelocity, UniversalSettings.speed);
	
				affectedObject.Rotate(realVelocity);

				break;

			case engineMode.FreeSlerp:

				tParam = Mathf.Clamp01((Time.time - lastGenTime) / FreeSettings.smoothTime);
				currentVelocity = Vector3.Slerp(currentVelocity, targetVelocity, tParam);
				
				realVelocity = currentVelocity * Time.deltaTime * UniversalSettings.speed;
				realVelocity = Vector3.ClampMagnitude(realVelocity, FreeSettings.speedClamp);
				
				affectedObject.Rotate(realVelocity);

				break;

			case engineMode.PhysicsBased:

				if(useGlobalTransforms) {
					rb.AddTorque(randomVector * UniversalSettings.speed);
				} else {
					rb.AddRelativeTorque(randomVector * UniversalSettings.speed);
				}

				break;
				
			}
			break;
			
		}

		HardClamping();
		
	}

	void HardClamping() {

		//Hard Clamping
		if(UniversalSettings.hardClampX || UniversalSettings.hardClampY || UniversalSettings.hardClampZ) {
			clampVector = GetAxis();
		}

        if (UniversalSettings.hardClampX) { clampVector.x = Mathf.Clamp(clampVector.x, UniversalSettings.minimumVector.x, UniversalSettings.maximumVector.x); }
        if (UniversalSettings.hardClampY) { clampVector.y = Mathf.Clamp(clampVector.y, UniversalSettings.minimumVector.y, UniversalSettings.maximumVector.y); }
        if (UniversalSettings.hardClampZ) { clampVector.z = Mathf.Clamp(clampVector.z, UniversalSettings.minimumVector.z, UniversalSettings.maximumVector.z); }

        if (UniversalSettings.hardClampX || UniversalSettings.hardClampY || UniversalSettings.hardClampZ)
        {
            SetAxis(clampVector);
        }
    }
	
	IEnumerator GenerateNewRandomVector() {

		while(true) {

			switch(engineType) {
				
			case engineMode.Precise:
			case engineMode.PreciseSmooth:
			case engineMode.PhysicsBased:
			case engineMode.FreeLerp:
			case engineMode.FreeSlerp:

				lastVector = GetAxis();
				randomVector = new Vector3(Random.Range(UniversalSettings.minimumVector.x, UniversalSettings.maximumVector.x), Random.Range(UniversalSettings.minimumVector.y, UniversalSettings.maximumVector.y), Random.Range(UniversalSettings.minimumVector.z, UniversalSettings.maximumVector.z));
				targetVelocity = currentVelocity + randomVector;
				break;
			}

			lastGenTime = Time.time;
			yield return new WaitForSeconds(UniversalSettings.randomGenerationInterval);
		}

	}

	//Shows debugging information
	void DebugInfo() {

		if(debugConsole) {
			print ("Currently modifying transform: " + affectedObject + " on axis: " + affectedAxis);
		}
	}

	//Modifies the object's axis, whether that be it's local or global setting determined by UseGlobalTransforms
	void SetAxis(Vector3 inputVector) {

		switch(affectedAxis) {
			
		case axis.Position:
			
			if(!useGlobalTransforms) {
				affectedObject.localPosition = inputVector;
			} else {
				affectedObject.position = inputVector;
			}
			break;
			
		case axis.Rotation:
			
			Quaternion QcurrentRot = Quaternion.Euler(inputVector);
	
			if(!useGlobalTransforms) {
				affectedObject.localRotation = QcurrentRot;
			} else {
				affectedObject.rotation = QcurrentRot;
			}
			break;
			
		case axis.Scale:
			
			if(!useGlobalTransforms) {
				affectedObject.localScale = inputVector;
			} else {
				affectedObject.localScale = inputVector;
				if(debugConsole) {
					print ("Note: Global transform scale cannot be assigned, assigning local scale instead");
				}
			}
			break;
		}

	}

	//Modifies the object's axis, whether that be it's local or global setting determined by UseGlobalTransforms
	Vector3 GetAxis() {
		
		switch(affectedAxis) {
			
		case axis.Position:
			
			if(!useGlobalTransforms) {
				return affectedObject.localPosition;
			} else {
				return affectedObject.position;
			}
			
		case axis.Rotation:
			
			if(!useGlobalTransforms) {
				return affectedObject.localRotation.eulerAngles;
			} else {
				return affectedObject.rotation.eulerAngles;
			}
			
		case axis.Scale:
			
			if(!useGlobalTransforms) {
				return affectedObject.localScale;
			} else {
				return affectedObject.lossyScale;
			}
		}

		if(debugConsole) {
			print ("ERROR: GetAxis() could not determine axis");
		}
		return Vector3.zero;
	}

	//Resets the script completely
	public void Reset() {
		active = true;

		UniversalSettings.useCurrentVector = false;

		Setup ();

		if(debugConsole) {
			print ("Resetting RandomInterpolator script at time: " + Time.time);
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
