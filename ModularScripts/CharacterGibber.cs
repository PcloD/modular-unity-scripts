/* Script created by Lachlan McKay 2016 ©
 * This script explodes a a transform heirarchy into "gibs" (short for giblets - limbs and other body parts) */

using UnityEngine;
using System.Collections;

public class CharacterGibber : MonoBehaviour
{
	
	[Header("Main Options")]
	[Tooltip("The transform of the target skeleton to gib.")]
	public Transform rootTransform;
	[Tooltip("When should the character explode?")]
	public execMode executionMethod;
	public enum execMode {Immediate, Delay, WaitForCommand}
	public float delayTime = 3f;
	[Tooltip("Unparent skeleton target from the rest of its chain?")]
	public bool breakSkeleton = true;
	[Tooltip("Spawn gib objects where the corresponding skeletonTarget transform is located?")]
	public bool createGibs = true;

	[System.Serializable]
	public class CharacterGibberObjects
	{
		[Header("Skeleton Objects")]
		[Tooltip("The piece(s) of the skeleton that should be disconnected.")]
		public Transform[] skeletonTargets;
		
		[Header("Giblet Objects")]
		[Tooltip("The object(s) that should be spawned at the above skeleton target points.")]
		public GameObject[] correspondingGiblets;

		[Header("Attachments (Optional)")]
		[Tooltip("Optionally attach some objects to specific created gibs (such as blood emitters).")]
		public GameObject[] attachObjects;
		[Tooltip("Must have the same length as skeletonTargets and correspondingGiblets. Specifies whether the above objects will be attached to a gib.")]
		public bool[] attachOrNot;
		
		[Header("Destroy Objects (Optional)")]
		[Tooltip("Optionally destroy some objects simultaneously (e.g. a character mesh).")]
		public GameObject[] destroyObjects;
		
	}
	
	public CharacterGibberObjects Objects = new CharacterGibberObjects();

	[System.Serializable]
	public class CharacterGibberPhysicsSettings
	{
		[Header("Physics")]
		[Tooltip("Attach a RigidBody component to gib and skeleton objects with identical settings to modelRigidbody specified below.")]
		public bool usePhysics = false;
		[Tooltip("Should physics affect the skeletonTarget bone objects?")]
		public bool affectSkeleton = false;

		[Header("Rigidbody")]
		[Tooltip("Copy all settings from specified rigidbody to use for every gib object if attachRigidBody is enabled.")]
		public Rigidbody modelRigidbody;
		[Tooltip("Destroys the model rigidbody and stores its settings on script load.")]
		public bool destroyModelRbOnLoad = false;
		
		[Header("Explosion")]
		[Tooltip("Enable/Disable explosion of gib objects")]
		public bool explodeGibs = false;
		[Tooltip("The point at which gibs will explode away from.")]
		public Transform explosionPosition;
		public float explosionPower = 100f;
		public float explosionRadius = 100f;
		public float yOffset = 0;
		
		[Header("Linear Blast")]
		public bool blastGibs = false;
		public Vector3 blastDirection;
		public float blastPower = 100f;
		
	}
	
	public CharacterGibberPhysicsSettings PhysicsSettings = new CharacterGibberPhysicsSettings();

	//Debugging
	[Header("Debug Options")]
	[Tooltip("Display errors in the console.")]
	public bool debugConsole = false;
	[Tooltip("Display gizoms in the scene editor.")]
	public bool displayGizmos = true;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script unparents a transform and spawns an object where it was disconnected. Useful for exploding characters into bodyparts upon death.";
    private string ScriptTags = "character gibber explode ragdoll gib gibbing effects vfx model rig bones";
    private string ScriptCategory = "effect";

    //Private
    //Container
    private GameObject container;
	private GameObject gibParent;
	private GameObject skeletonParent;

	//Stored
	private GameObject[] spawnedGibs;
	private Rigidbody storedRb;

	//Miscellaneous
	private bool active = false;

	void Awake() {

        if (active)
        {
            PhysicsSettings.modelRigidbody.Sleep(); //Suspend the model rigidbody so it doesnt fall down due to gravity
        }
	}

	void Start () {

        if (active)
        {
            Setup();
        }

		if(executionMethod == execMode.Immediate) {
			Execute();
		}
	}

	void Setup() {

		ErrorChecking();

		if(!rootTransform) {
			if(debugConsole) {
				print ("WARNING: No rootTransform set on object: " + gameObject.name + ". Using this object instead.");
			}
			rootTransform = transform;
		}
		
		if(!PhysicsSettings.modelRigidbody && GetComponent<Rigidbody>()) {
			PhysicsSettings.modelRigidbody = gameObject.GetComponent<Rigidbody>();
			
		} else if(!GetComponent<Rigidbody>()) {
			print ("ERROR: No modelRigidbody specified on object: " + gameObject.name);
		}
		
		storedRb = PhysicsSettings.modelRigidbody as Rigidbody;	//Store the model rigidbody's settings
		
		if(PhysicsSettings.destroyModelRbOnLoad) {
			//Destroy(PhysicsSettings.modelRigidbody);
		} else {
			PhysicsSettings.modelRigidbody.Sleep();
		}

	}

	void ErrorChecking() {

		if(Objects.skeletonTargets.Length <= 0) {
			
			print ("ERROR: SkeletonTargets must have more than 0 objects");

		} else if(Objects.correspondingGiblets.Length <= 0 && createGibs) {

			print ("ERROR: CorrespondingGiblets must have more than 0 objects if createGibs is enabled");

		} else if(Objects.skeletonTargets.Length != Objects.correspondingGiblets.Length && createGibs) {

			print ("ERROR: Skeleton and Corresponding Giblets must have the same number of objects if CreateGibs is enabled.");
		
		} else if(Objects.attachOrNot.Length != Objects.correspondingGiblets.Length) {

			print ("ERROR: attachOrNot array length must match correspondingGiblets array length");

		} else {

			active = true;
		}
	}


	public void Execute () {

		if(active) {

            SetupVars();

            SetupArrays();
			
			if(createGibs) {
				SpawnGibs();
			}
			UnparentSkeleton();
			DestroyObjects();

		}
	}

	
	void SetupVars() {
		
		container = new GameObject(gameObject.name + "-" + rootTransform.name + " Gibs/Bones");
		gibParent = new GameObject(rootTransform.name + " Gibs");
		skeletonParent = new GameObject(rootTransform.name + " Bones");
		
		gibParent.transform.SetParent(container.transform);
		skeletonParent.transform.SetParent(container.transform);
		
	}
	
	void SetupArrays() {

		spawnedGibs = new GameObject[Objects.correspondingGiblets.Length];

	}

	void UnparentSkeleton() {

		for(int i = 0; i < Objects.skeletonTargets.Length; i++) {

			Objects.skeletonTargets[i].SetParent(skeletonParent.transform);
		}
	}

	
	void DestroyObjects() {
		
		for(int i = 0; i < Objects.destroyObjects.Length; i++) {
			Destroy(Objects.destroyObjects[i]);
		}
	}

	void SpawnGibs() {

		for(int i = 0; i < Objects.correspondingGiblets.Length; i++) {

			spawnedGibs[i] = Instantiate(Objects.correspondingGiblets[i], Objects.skeletonTargets[i].transform.position, Objects.skeletonTargets[i].transform.rotation) as GameObject;
			spawnedGibs[i].transform.SetParent(gibParent.transform);

			if(PhysicsSettings.usePhysics) {

				Rigidbody rb = spawnedGibs[i].AddComponent<Rigidbody>();
				CloneRigidbody(rb);	//Set rb to have identical settings to stored rigidbody (saved from model rigidbody via inspector)

				if(PhysicsSettings.affectSkeleton) {
					Rigidbody sRb = Objects.skeletonTargets[i].gameObject.AddComponent<Rigidbody>();
					CloneRigidbody(sRb); //Set sRb to have identical settings to stored rigidbody (saved from model rigidbody via inspector)
				}
			}

			if(PhysicsSettings.explodeGibs) {
				ExplodeObject(i);
			}

			if(PhysicsSettings.blastGibs) {
				BlastObject(i);
			}

			if(Objects.attachOrNot[i]) {
				AttachObjects(i);
			}
		}
	}

	//Adds an explosion force to spawned gib objects and/or skeleton targets
	void ExplodeObject(int i) {

		if(createGibs) {
			Rigidbody eRb = spawnedGibs[i].GetComponent<Rigidbody>();
			Explode(eRb);
		}

		if(PhysicsSettings.affectSkeleton) {
			Rigidbody seRb = Objects.skeletonTargets[i].gameObject.GetComponent<Rigidbody>(); 
			Explode(seRb);
		}
	}

	//Adds an explosion force to an inputted Rigidbody
	void Explode(Rigidbody inputRb) {
		inputRb.AddExplosionForce(PhysicsSettings.explosionPower, PhysicsSettings.explosionPosition.position, PhysicsSettings.explosionRadius, PhysicsSettings.yOffset);
	}

	//Adds a force to spawned gib objects and/or skeleton targets
	void BlastObject(int i) {

		if(createGibs) {
			Rigidbody bRb = spawnedGibs[i].GetComponent<Rigidbody>();
			Blast(bRb);
		}

		if(PhysicsSettings.affectSkeleton) {
			Rigidbody sbRb = Objects.skeletonTargets[i].gameObject.GetComponent<Rigidbody>(); 
			Blast(sbRb);
		}
	}

	//Adds a force to an inputted Rigidbody
	void Blast(Rigidbody inputRb) {
		inputRb.AddForce(PhysicsSettings.blastDirection * PhysicsSettings.blastPower);
	}

	//Attaches a prefab to specified created gibs
	void AttachObjects(int ID) {

		for(int j = 0; j < Objects.attachObjects.Length; j++) {

			if(Objects.attachObjects[j]) {

				GameObject spawnedAttachment = Instantiate(Objects.attachObjects[j], spawnedGibs[ID].transform.position, Quaternion.identity) as GameObject;
				spawnedAttachment.transform.SetParent(spawnedGibs[ID].transform);

			} else {

				if(debugConsole) {
					print ("WARNING: One of your attachObject slots is null on object: " + gameObject.name);
				}
			}

		}
	}


	//Clones an inputted rigidbody to have the same settings as the stored Rigidbody
	void CloneRigidbody(Rigidbody inputRb) {
		
		inputRb.mass = storedRb.mass;
		inputRb.drag = storedRb.drag;
		inputRb.angularDrag = storedRb.angularDrag;
		inputRb.useGravity = storedRb.useGravity;
		inputRb.isKinematic = storedRb.isKinematic;
		inputRb.interpolation = storedRb.interpolation;
		inputRb.collisionDetectionMode = storedRb.collisionDetectionMode;
		inputRb.constraints = storedRb.constraints;
		
	}

	void OnDrawGizmosSelected() {

		if(displayGizmos) {

			if(PhysicsSettings.usePhysics) {
				if(PhysicsSettings.explodeGibs) {
					Color gizmoCol = Color.green;
					gizmoCol.a = 0.2f;
					Gizmos.color = gizmoCol;
					Gizmos.DrawSphere(PhysicsSettings.explosionPosition.position, PhysicsSettings.explosionRadius);
				}

				if(PhysicsSettings.blastGibs) {
					Color gizmoCol = Color.red;
					Gizmos.color = gizmoCol;
					Gizmos.DrawRay(gameObject.transform.position, PhysicsSettings.blastDirection * PhysicsSettings.blastPower);
				}
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
