/* Script created by Loki McKay 2016 ©
 * This script randomly fires raycasts and spawns new objects on ray collision */

using UnityEngine;
using System.Collections;

public class RayProjector : MonoBehaviour
{

	[Header("Main Options")]
	[Tooltip("The object to project onto a surface.")]
	public GameObject objectToProject;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	[Tooltip("What type of object will be spawned (used for setting scale).")]
	public objType spawnedObjectType;
	public enum objType {gameObject, particleSystem}

	[Header("Spawn Transform Options")]
	[Tooltip("Spawn the object this distance away from raycasted surface along the surface normal.")]
	public float normalFloatDistance = 0.2f;
	[Tooltip("The maximum/minimum angle of rotation the object can have on spawning.")]
	public Vector2 randomRotation = new Vector2 (0f, 359f);
	[Tooltip("The maximum/minimum size of the object on spawning.")]
	public Vector2 randomSize = new Vector2(0f, 5f);
	[Tooltip("Enable this to force X,Y and Z scale to have the same value.")]
	public bool uniformScale = true;

	[Header("Destruction Settings")]
	[Tooltip("Destroy the object after an amount of time.")]
	public bool destroy = true;
	[Tooltip("Destroy the object after this amount of time.")]
	public float destructionTime = 5.0f;

	[Header("Ray Options")]
	[Tooltip("Number of rays to shoot. Randomly generated between two constants.")]
	public Vector2 raysToFire = new Vector2(40, 80);
	[Tooltip("The max distance that a ray travels.")]
	public float rayDistance;
	[Tooltip("Layers that will trigger raycast hits.")]
	public LayerMask layerMask;

	//Debugging
	[Header("Debug Options")]
	public bool showRays = false;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script randomly fires raycasts and spawns new objects on ray collision.";
    private string ScriptTags = "ray projector collision spawn prefab decal random raycast physics hit rays";
    private string ScriptCategory = "spawning";

    //Private
    private int rayCount;
	private RaycastHit hit;
	private GameObject spawnedObject;

	private Vector3[] firedRays;

	void Start () {
	
		Setup();

		if(!commandActivated && objectToProject) {
			Execute();
		} else if(!objectToProject) {
			print ("ERROR: No object to project in RayProjector script on object: " + gameObject.name);
		}
	}

	void Setup() {

		rayCount = Mathf.RoundToInt(Random.Range(raysToFire.x, raysToFire.y));
		firedRays = new Vector3[rayCount];
	}

	void Update() {
		DebuggingInfo();
	}

	void DebuggingInfo() {

		if(showRays) {

			for(int i = 0; i < firedRays.Length; i++) {
				Debug.DrawRay(transform.position, firedRays[i], Color.red);
			}
		}
	}

	void Execute() {

		int x = -1;
		
		while (x <= rayCount)
		{
			x ++;
			
			Vector3 rayDir = transform.TransformDirection (Random.onUnitSphere * rayDistance);

			if(showRays && x < firedRays.Length) {
				firedRays[x] = rayDir;
			}

			if (Physics.Raycast (transform.position, rayDir, out hit, rayDistance, layerMask)) 
			{
				Vector3 offWall = hit.normal;
				offWall.x *= normalFloatDistance;
				offWall.y *= normalFloatDistance;
				offWall.z *= normalFloatDistance;

				Vector3 spawnLoc = hit.point + offWall;
				Vector3 spawnRot = Quaternion.FromToRotation (Vector3.up, hit.normal).eulerAngles;

				Vector3 spawnSize = Vector3.one;

				if(!uniformScale) {
					spawnSize = new Vector3(Random.Range(randomSize.x, randomSize.y), Random.Range(randomSize.x, randomSize.y), Random.Range(randomSize.x, randomSize.y));
				} else {
					float uniformSpawnSize = Random.Range(randomSize.x, randomSize.y);
					spawnSize = new Vector3(uniformSpawnSize, uniformSpawnSize, uniformSpawnSize);
				}

				spawnedObject = Instantiate (objectToProject, spawnLoc, Quaternion.Euler(spawnRot)) as GameObject;

				if(spawnedObject && destroy) {
					Destroy (spawnedObject, destructionTime);
				}

				spawnedObject.transform.RotateAround (hit.point, hit.normal, Random.Range(randomRotation.x, randomRotation.y));

				switch(spawnedObjectType) {

				case objType.gameObject:

					spawnedObject.transform.localScale = spawnSize;
					break;

				case objType.particleSystem:

					if(spawnedObject.GetComponent<ParticleManager>()) {
						spawnedObject.GetComponent<ParticleManager>().SetArrayLoopToDefault();
						spawnedObject.GetComponent<ParticleManager>().SetStartSize(Random.Range(randomSize.x, randomSize.y));
					} else {
						print ("ERROR: Please attach a ParticleManager script to the objectToSpawn on RayProjector script on object: " + gameObject.name + " for use in scaling the spawned particle");
					}
					break;

				}

				spawnedObject.transform.SetParent(transform);
			}        
		}
	}
}
