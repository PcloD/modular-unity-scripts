/* Script created by Lachlan McKay 2016
 * This script spawns an object when this object's particle system collides with a surface
 * Useful for spawning decals when a liquid particle collides with a surface */

using UnityEngine; 
using System.Collections;

public class ParticleCollision : MonoBehaviour
{

	[Header("Collision Options")]
	[Tooltip("The object to spawn on collision with a surface.")]
	public GameObject prefabToSpawn;
	[Tooltip("Destroy the object after an amount of time.")]
	public bool destroy = true;
	[Tooltip("Destroy the object after this amount of time.")]
	public float destructionTime = 5.0f;

	[Header("Spawn Transform Options")]
	[Tooltip("Spawn the object this distance away from collided surface along the collision normal.")]
	public float normalFloatDistance = 0.2f;
	[Tooltip("The maximum/minimum angle of rotation the object can have on spawning.")]
	public Vector2 randomRotation = new Vector2 (0f, 359f);
	[Tooltip("Container that will hold spawned prefabs.")]
	public Transform parentObject;

	[Header("Scaling Options")]
	[Tooltip("What type of object will be spawned (used for setting scale).")]
	public objType spawnedObjectType;
	public enum objType {noScaling, gameObject, particleSystem}
	[Tooltip("The maximum/minimum size of the object on spawning.")]
	public Vector2 randomSize = new Vector2(0f, 5f);
	[Tooltip("Enable this to force X,Y and Z scale to have the same value.")]
	public bool uniformScale = true;
	
	//Debugging
	[Header("Debug Options")]
	public bool debugConsole = false;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script spawns an object when this object's particle system collides with a surface.";
    private string ScriptTags = "particle collision particles collide colliding event spawn blood decal";
    private string ScriptCategory = "effect";

    //Private
    private ParticleSystem particleSys;
	private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];
	private Vector3 collisionHitLoc;
	private Vector3 collisionNormal;
	private GameObject spawnedPrefab;

    private bool active = false;

    void Start() { 

		Setup ();

	}

	void Setup() {

		particleSys = GetComponent<ParticleSystem>();

		if(!parentObject) {
			parentObject = new GameObject(gameObject.name + " Particle Collisions").transform;
		}
	}

	void Update() {

		if(debugConsole) {
			DebugInfo();
		}
	}

	void OnParticleCollision(GameObject other) {

        if (active)
        {

            int safeLength = particleSys.GetSafeCollisionEventSize();

            if (collisionEvents.Length < safeLength)
            {
                collisionEvents = new ParticleCollisionEvent[safeLength];
            }

            int numCollisionEvents = particleSys.GetCollisionEvents(other, collisionEvents);

            int i = 0;

            while (i < numCollisionEvents)
            {

                collisionHitLoc = collisionEvents[i].intersection;
                collisionNormal = collisionEvents[i].normal;

                Vector3 offWall = collisionNormal;
                offWall.x *= normalFloatDistance;
                offWall.y *= normalFloatDistance;
                offWall.z *= normalFloatDistance;

                Vector3 spawnLoc = collisionHitLoc + offWall;
                Vector3 spawnRot = new Vector3(0, Random.Range(randomRotation.x, randomRotation.y), 0);

                Vector3 spawnSize = Vector3.one;

                if (!uniformScale)
                {
                    spawnSize = new Vector3(Random.Range(randomSize.x, randomSize.y), Random.Range(randomSize.x, randomSize.y), Random.Range(randomSize.x, randomSize.y));
                }
                else
                {
                    float uniformSpawnSize = Random.Range(randomSize.x, randomSize.y);
                    spawnSize = new Vector3(uniformSpawnSize, uniformSpawnSize, uniformSpawnSize);
                }

                spawnedPrefab = Instantiate(prefabToSpawn, spawnLoc, Quaternion.Euler(spawnRot)) as GameObject;

                if (spawnedPrefab && destroy)
                {
                    Destroy(spawnedPrefab, destructionTime);
                }

                spawnedPrefab.transform.up = collisionNormal;

                if (parentObject)
                {
                    spawnedPrefab.transform.SetParent(parentObject);
                }

                switch (spawnedObjectType)
                {

                    case objType.gameObject:

                        spawnedPrefab.transform.localScale = spawnSize;
                        break;

                    case objType.particleSystem:

                        if (spawnedPrefab.GetComponent<ParticleManager>())
                        {
                            spawnedPrefab.GetComponent<ParticleManager>().SetArrayLoopToDefault();
                            spawnedPrefab.GetComponent<ParticleManager>().SetStartSize(Random.Range(randomSize.x, randomSize.y));
                        }
                        else
                        {
                            print("ERROR: Please attach a ParticleManager script to the prefabToSpawn on ParticleCollision script on object: " + gameObject.name + " for use in scaling the spawned particle");
                        }
                        break;
                }

                i++;
            }
        }
	}

	void DebugInfo() {

		Debug.DrawLine(gameObject.transform.position, collisionHitLoc, Color.red);
		Debug.DrawRay(collisionHitLoc, collisionNormal, Color.blue);
		
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