/* Script created by Lachlan McKay 2016 ©
 * This script clones an object or objects and emits them as if they were particles */

using UnityEngine;
using System.Collections;

public class CloneEmitter : MonoBehaviour
{

	[Header("Initialisation")]
	[Tooltip("The object to instantiate as a clone.")]
	public GameObject targetObject;
	[Tooltip("The position to instantiate at.")]
	public Transform spawnLocation;
	[Tooltip("When calculating velocity, consider the world or local position coordinates?")]
	public Space spaceMode = Space.World;
	
	[Header("Heirarchy Control")]
	[Tooltip("Destroy the Target Object upon script start.")]
	public bool destroySourceObject = false;
	[Tooltip("If enabled, emitted clones will be parented under the object this script is attached to. If disabled, a new container object will be created at runtime.")]
	public bool parentUnderSelf = false;
	[Tooltip("Remove instances of this script on the instantiated clones? Warning: This can create a large amount of clones if disabled because of exponential growth.")]
	public bool detachCloneEmitter = true;

	[Header("Emission")]
	[Tooltip("Emit clones based on time or distance moved by the Spawn Location transform?")]
	public mode emitMode = mode.time;
	public enum mode {time, distance}
	[Range(0,1000f)]
	[Tooltip("The rate to emit clones based on either time or distance moved by the Spawn Location transform.")]
	public float emissionRate = 1;

	[Header("Clone Settings")]
	[Range(0,1000f)]
	[Tooltip("The amount of time a clone will live before dying.")]
	public float lifetime = 2f;
	[Tooltip("Clones will not be destroyed if this is enabled.")]
	public bool infiniteCloneLifetime = false;

	[Header("Material Settings")]
	[Tooltip("Attach a new material to the emitted clone objects (PARENT ONLY).")]
	public bool attachNewMaterial = false;
	[Tooltip("Attach this new material to the emitted clone objects and their children.")]
	public bool attachToAllMeshes = false;
	[Tooltip("The new material to attach to emitted clones.")]
	public Material newMaterial;

	[System.Serializable]
	public class CloneEmitterAdvancedMaterials
	{
		[Header("Advanced Materials")]
		[Tooltip("Assign each clone its own unique copy of 'New Material' (Must be enabled for advanced material options).")]
		public bool uniqueMaterials;
		[Tooltip("Spawned clones can take on a randomly generated color or a user defined color (via gradient).")]
		public colMode advancedColorMode = colMode.gradient;
		public enum colMode {none, random, gradient}
		[Tooltip("Assign the color gradient for the emitter to cycle through (left = beginning, right = end).")]
		public Gradient colorGradient;
		[Tooltip("The time taken for the emitter to cycle through the color gradient.")]
		public float cycleDuration = 5f;
		[Header("Material Color Property")]
		[Tooltip("What property on the material should be modified?")]
		public colorType colorProperty = colorType.color;
		public enum colorType {color, mainColor, tintColor, emissionColor, diffuseColor, ambientColor, specularColor, textColor}
		[Tooltip("Enter a name of a specific property if it does not appear in the above list.")]
		public string customProperty = "";
	}
	
	public CloneEmitterAdvancedMaterials AdvancedMaterials = new CloneEmitterAdvancedMaterials();

	
	[Header("Physics")]
	[Tooltip("Attach a rigidbody component to instantiated clones? Clones must have a rigidbody component for physics to be applied to them.")]
	public bool attachRigidbody;
	[Tooltip("Enable gravity on the instantiated clones' rigidbody component?")]
	public bool useGravity = true;
	[Range(-9000,9000)]
	[Tooltip("Add a linear force to instantiated clones at birth that is relative to the velocity of the Spawn Location transform.")]
	public float inheritVelocity = 0f;

	[Header("Debug Options")]
	[Tooltip("Display errors in the console.")]
	public bool debugConsole = false;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script clones an object or objects and emits them as if they were particles.";
    private string ScriptTags = "clone emitter visual effects vfx trail particle particles";
    private string ScriptCategory = "effect";

    //Private
    private float emissionTimer = 0f;
	private float emissionInterval;

	private Material[] materialArray;

	private GameObject container;
	private GameObject storedTargetObject;

	private Vector3 currentPos;
	private Vector3 lastPos;
	private Vector3 velocity;
	private float distance;

	private bool active = false;

	
	void Start () {
		Setup();
	}

	void Setup() {

		if(!targetObject) {
			targetObject = gameObject;					//Set the target object if it is null
		}
		
		if(!spawnLocation) {
			spawnLocation = targetObject.transform;		//Set the spawn location if it is null
		}

		if(attachToAllMeshes && !attachNewMaterial) {
			if(debugConsole) {
				print ("WARNING: attachNewMaterial must also be enabled if attachToAllMeshes is enabled. Correcting now on object: " + gameObject.name);
			}
			attachNewMaterial = true;
		}

		if(ErrorChecking()) {						//Only continue with setup if critical errors are not found

			emissionInterval = 1f / emissionRate;	//Calculate the time that needs to pass before a clone can be emitted

			if(!parentUnderSelf) {
				container = new GameObject(gameObject.name + " Emitted Clones");		//If clones are not going to be parented under the object this script is attached to, create a container gameobject
			}

			//Disable the source object if it is not the object this script is attached to and destroySourceObject is enabled
			if(destroySourceObject && targetObject == gameObject) {
				print ("WARNING: Cannot disable the Target Object because it is the same as the object this script is attached to, which would eliminate this script. Please specify a different target in the inspector on object: " + gameObject.name);
			} else if(destroySourceObject) {

				storedTargetObject = Instantiate(targetObject, new Vector3(-9999f, -9999f, -9999f), Quaternion.identity) as GameObject;				//Store the targetObject as a copy of itself and move it very far away (invisible)
				Destroy(storedTargetObject.GetComponent<CloneEmitter>());
				storedTargetObject.name = "(Stored)_" + targetObject.name;
				storedTargetObject.transform.SetParent(targetObject.transform.parent);

				Destroy(targetObject);	//Destroy the source object

			} else if(!destroySourceObject) {

				storedTargetObject = targetObject;	//Simply store a reference to the targetObject as we don't need to destroy the source

			}

			if(AdvancedMaterials.uniqueMaterials) {								//If each clone must be assigned its own material,
				materialArray = new Material[Mathf.FloorToInt(emissionRate * lifetime)];	//Create an array of individual materials for each clone that will be on screen

				for(int i = 0; i < materialArray.Length; i++) {

					if(AdvancedMaterials.advancedColorMode == CloneEmitterAdvancedMaterials.colMode.gradient) {
						materialArray[i] = FillMatArrayWithGradient(i);						//Set all materials to be newMaterial, but with a unique color defined by the inspector gradient
					} else {
						materialArray[i] = newMaterial;											//Set all materials to be newMaterial
					}
				}
			}

			active = true;
		}
	}
	

	//Checks for critical inspector errors and disables setup if any are found
	bool ErrorChecking() {

		if(emissionRate <= 0) {

			if(debugConsole) {
				print ("ERROR: Clones Per Second must be greater than 0 on object: " + gameObject.name);
			}
			return false;
		} else if(lifetime <= 0) {

			if(debugConsole) {
				print ("ERROR: Lifetime must be greater than 0 on object: " + gameObject.name);
			}
			return false;

		} else if(inheritVelocity != 0 && !attachRigidbody && targetObject.GetComponent<Rigidbody>() == null) {
			
			if(debugConsole) {
				print ("ERROR: To use physics settings, either the targetObject must contain a rigidbody component or you must check the 'Attach Rigidbody' checkbox on object: " + gameObject.name);
			}
			return false;

		} else if(attachNewMaterial && !newMaterial) {
			
			if(debugConsole) {
				print ("ERROR: Please assign a new material to attach to emitted clones on object: " + gameObject.name);
			}
			return false;
			
		} else if(attachNewMaterial && !attachToAllMeshes && targetObject.GetComponent<Renderer>() == null) {
			
			if(debugConsole) {
				print ("ERROR: There is no Renderer component attached to the Target Object. If you would like the script to search through all children for a Renderer component, please check the box for 'Attach To All Meshes' on object: " + gameObject.name);
			}
			return false;
			
		} else if(!CheckProperty()) {

			if(debugConsole) {
				print ("ERROR: New Material does not contain the property specified in the inspector. Please check the material settings on object: " + gameObject.name);
			}
			return false;


		} else {

			return true;
		}
	}

	//Checks to make sure that the selected color property in the inspector exists on the target material
	bool CheckProperty() {

		if(AdvancedMaterials.uniqueMaterials && AdvancedMaterials.advancedColorMode != CloneEmitterAdvancedMaterials.colMode.none) {
			if(AdvancedMaterials.customProperty == "") {
				switch(AdvancedMaterials.colorProperty) {
					
				case CloneEmitterAdvancedMaterials.colorType.color:
				case CloneEmitterAdvancedMaterials.colorType.mainColor:
				default:
					return newMaterial.HasProperty("_Color");
				case CloneEmitterAdvancedMaterials.colorType.tintColor:
					return newMaterial.HasProperty("_TintColor");
				case CloneEmitterAdvancedMaterials.colorType.ambientColor:
					return newMaterial.HasProperty("_AmbientColor");
				case CloneEmitterAdvancedMaterials.colorType.diffuseColor:
					return newMaterial.HasProperty("_DiffuseColor");
				case CloneEmitterAdvancedMaterials.colorType.emissionColor:
					return newMaterial.HasProperty("_EmissionColor");
				case CloneEmitterAdvancedMaterials.colorType.specularColor:
					return newMaterial.HasProperty("_SpecColor");
				case CloneEmitterAdvancedMaterials.colorType.textColor:
					return newMaterial.HasProperty("_TextColor");
				}
			} else {
				return newMaterial.HasProperty(AdvancedMaterials.customProperty);
			}
		} else {
			return true;
		}
	}

	void Update () {
	
		Master();
	}

	void Master() {

		if(active) {
			GetInfo("start");		//Update currentPos of the spawnLocation
			IncrementTimers();		//Increment the emissionTimer based on either time or distance travelled by the Spawn Location transform
			Engine();				//Controls emission of clones
			GetInfo("end");			//Store this frame's currentPos as "lastPos" for use in the next frame
		}
	}

	//Increment the emissionTimer based on either time or distance travelled by the Spawn Location transform
	void IncrementTimers() {

		switch(emitMode) {
			
		case mode.time:
			emissionTimer += Time.deltaTime;
			break;
			
		case mode.distance:
			emissionTimer += GetDistance();
			break;
		}
	}

	//CurrentPos and LastPos are used in both the GetDistance and GetVelocity functions but they must only be updated once at the beginning and end of each frame. This function handles those updates.
	void GetInfo(string phase) {

		switch(phase) {

		case "start":
			currentPos = GetPosition();	//Update currentPos of the spawnLocation
			break;

		case "end":
			lastPos = GetPosition();	//Store this frame's currentPos as "lastPos" for use in the next frame
			break;
		}
	}

	//Controls emission of clones
	void Engine() {

		if(emissionTimer > emissionInterval) {

			Emit();
			if(emitMode == mode.distance) {
				emissionTimer = 0f;
			} else if(emitMode == mode.time) {
				emissionTimer -= emissionInterval;
			}
		}

	}

	//Returns the distance moved by the spawnLocation transform over the last frame
	float GetDistance() {

		distance = Vector3.Distance(currentPos, lastPos);
		return distance;
	}

	//Returns the current velocity of the spawnLocation transform using the distance moved over the last frame
	Vector3 GetVelocity() {

		velocity = currentPos - lastPos;
		return velocity;
	}

	//Returns the current position of the spawn location transform based on the spaceMode inspector selection
	Vector3 GetPosition() {

		switch(spaceMode) {

		case Space.Self:
			return spawnLocation.localPosition;

		case Space.World:
		default:
			return spawnLocation.position;
		}

	}

	//Instantiates a clone of the Target Object at the Spawn Location's position
	void Emit() {

		GameObject lastInstantiatedClone = Instantiate(storedTargetObject, spawnLocation.position, spawnLocation.rotation) as GameObject;	//Instantiate the storedTargetObject at spawnLocation's position

		lastInstantiatedClone.name = storedTargetObject.name + "(Clone)";				//Set the name of the spawned clone

		if(detachCloneEmitter) {
			Destroy(lastInstantiatedClone.GetComponent<CloneEmitter>());
		}

		if(parentUnderSelf) {
			lastInstantiatedClone.transform.SetParent(transform);
		} else {
			lastInstantiatedClone.transform.SetParent(container.transform);
		}

		if(!infiniteCloneLifetime) {
			Destroy(lastInstantiatedClone, lifetime);
		}

		if(attachRigidbody) {
			lastInstantiatedClone.AddComponent<Rigidbody>();
		}

		if(inheritVelocity != 0) {
			Rigidbody rb = lastInstantiatedClone.GetComponent<Rigidbody>();
			rb.AddForce(GetVelocity() * inheritVelocity);
			rb.useGravity = useGravity;
		}

		if(attachNewMaterial) {	//If attach new material is enabled, either attach the newMaterial to Target Object or all children of Target Object

			Material matToApply;

			if(AdvancedMaterials.uniqueMaterials) {
				matToApply = GetMaterial();
			} else {
				matToApply = newMaterial;
			}

			if(attachToAllMeshes) {
				
				AttachMatsToChildren(lastInstantiatedClone, matToApply);
			} else {
				lastInstantiatedClone.GetComponent<Renderer>().material = matToApply;
			}

		}
	}

	//Finds all renderes in the children of 'parent' and applies newMaterial to them
	void AttachMatsToChildren(GameObject parent, Material mat) {

		Renderer[] childrenRenderers = parent.GetComponentsInChildren<Renderer>();

		foreach(Renderer childRenderer in childrenRenderers) {
			childRenderer.material = mat;
		}
	}

	//Grabs a unique material from the material array to apply to a clone
	Material GetMaterial() {
		
		switch(AdvancedMaterials.advancedColorMode) {
			
		case CloneEmitterAdvancedMaterials.colMode.random:
			float r = Random.value;
			float b = Random.value;
			float g = Random.value;

			Color randomCol = new Color(r,b,g);
			Material randomMat = new Material(newMaterial);


			AssignCol(randomMat, randomCol);
//			randomMat.color = randomCol;
//			randomMat.SetColor("_TintColor", randomCol);
			
			return randomMat;
			
		case CloneEmitterAdvancedMaterials.colMode.gradient:
		case CloneEmitterAdvancedMaterials.colMode.none:
		default:
			int cycleToID = (int) Mathf.Repeat(Time.time * (1f/AdvancedMaterials.cycleDuration), materialArray.Length);
			//print ("ID: " + cycleToID);
			//print ("RETURNING: " + FillMatArrayWithGradient(cycleToID).color);
			return materialArray[cycleToID];
		}
	}

	//Returns color value based on the inspector Advanced Color Mode seletion
	Material FillMatArrayWithGradient(int index) {
		
		float percent = (float) index / materialArray.Length;

		Material returnMat = new Material(newMaterial);
		Color col = AdvancedMaterials.colorGradient.Evaluate(percent);

		AssignCol(returnMat, col);

		return returnMat;
	}

	//Assigns a target color to a target material depending on the colorProperty inspector selection
	void AssignCol(Material targetMaterial, Color targetCol) {
		
		Color colToApply = targetCol;
		
		if(AdvancedMaterials.customProperty == "") {
			switch(AdvancedMaterials.colorProperty) {
				
			case CloneEmitterAdvancedMaterials.colorType.color:
			case CloneEmitterAdvancedMaterials.colorType.mainColor:
			default:
				targetMaterial.SetColor("_Color", colToApply);
				break;
			case CloneEmitterAdvancedMaterials.colorType.tintColor:
				targetMaterial.SetColor("_TintColor", colToApply);
				break;
			case CloneEmitterAdvancedMaterials.colorType.ambientColor:
				targetMaterial.SetColor("_AmbientColor", colToApply);
				break;
			case CloneEmitterAdvancedMaterials.colorType.diffuseColor:
				targetMaterial.SetColor("_DiffuseColor", colToApply);
				break;
			case CloneEmitterAdvancedMaterials.colorType.emissionColor:
				targetMaterial.SetColor("_EmissionColor", colToApply);
				break;
			case CloneEmitterAdvancedMaterials.colorType.specularColor:
				targetMaterial.SetColor("_SpecColor", colToApply);
				break;
			case CloneEmitterAdvancedMaterials.colorType.textColor:
				targetMaterial.SetColor("_TextColor", colToApply);
				break;
			}
		} else {
			targetMaterial.SetColor(AdvancedMaterials.customProperty, colToApply);
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

