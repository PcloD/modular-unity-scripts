/* Script created by Lachlan McKay 2016
 * This script spawns an object at the location of a target transform
 * Useful for dealing with Unity's inability to create nested prefabs */

using UnityEngine;
using System.Collections;

public class PrefabSpawner : MonoBehaviour
{

	[Header("Loaded Objects")]
	[Tooltip("The prefab to spawn at runtime.")]
	public GameObject prefabToSpawn;
	[Tooltip("The location to spawn the prefab at (Leave blank for this object).")]
	public Transform spawnTransform;
	[Tooltip("The transform that will be the parent of the spawned object (Leave blank for this object).")]
	public Transform groupUnder;

	[Header("Options")]
	[Tooltip("Make the prefab spawn at the Spawn Transform's world or local space.")]
	public Space spaceMode = Space.World;
	[Tooltip("Make the prefab have the same rotation as the Spawn Transform.")]
	public bool mimicRotation = true;
	[Tooltip("Make the prefab have the same scale as the Spawn Transform.")]
	public bool mimicScale = false;

	[Header("Overrides")]
	[Tooltip("If Mimic Rotation is disabled and Override Rotation is enabled, Set Rotation vector will be used instead.")]
	public bool overrideRotation = false;
	[Tooltip("If Mimic Rotation is disabled and Override Rotation is enabled, this vector will be used instead.")]
	public Vector3 setRotation;
	[Space(7)]
	[Tooltip("If Mimic Scale is disabled and Override Scale is enabled, Set Scale vector will be used instead.")]
	public bool overrideScale = false;
	[Tooltip("If Mimic Scale is disabled and Override Scale is enabled, this vector will be used instead.")]
	public Vector3 setScale;

	[Header("Misc Options")]
	[Tooltip("Removes the (Clone) string from the spawned object name.")]
	public bool removeCloneFromName = true;

	[Header("Debug Options")]
	[Tooltip("Display errors in the console.")]
	public bool debugConsole = false;

	//Credits and description
	[Header("_Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script spawns an object at the location of a target transform. Useful for dealing with Unity's inability to create nested prefabs.";
    private string ScriptTags = "prefab spawn spawner object create instantiate runtime nested prefabs";
    private string ScriptCategory = "spawning";

    //Private
    private GameObject storedPrefab;

	void Start () {
		Setup();
	}

	void Setup() {

		if(ErrorChecking()) {
			Execute();
		}
	}

	//Checks that inspector settings are correctly setup
	bool ErrorChecking() {

		if(!prefabToSpawn) {
			if(debugConsole) {
				print("ERROR: Please assign a prefab to spawn on object: " + gameObject.name);
			}
			return false;
		}

		if(!spawnTransform) {
			if(debugConsole) {
				print("WARNING: No Spawn Transform specified, using this object instead: " + gameObject.name);
			}
			spawnTransform = transform;
		}

		if(!groupUnder) {
			if(debugConsole) {
				print("WARNING: No Group Under transform specified, using this object instead: " + gameObject.name);
			}
			groupUnder = transform;
		}

		return true;
	}

	//Spawns the object according to inspector settings
	void Execute() {

		if(mimicRotation) {
			storedPrefab = Instantiate(prefabToSpawn, GetSpawnLocation(), GetSpawnRotation()) as GameObject;
		} else if(overrideRotation) {
			storedPrefab = Instantiate(prefabToSpawn, GetSpawnLocation(), Quaternion.Euler(setRotation)) as GameObject;
		}

		if(mimicScale) {
			storedPrefab.transform.localScale = spawnTransform.localScale;
		} else if(overrideScale) {
			storedPrefab.transform.localScale = setScale;
		}

		if(removeCloneFromName) {
			storedPrefab.name = prefabToSpawn.name;
		}

		storedPrefab.transform.SetParent(groupUnder);
	}

	//Sets the prefab scale based on the space setting
	void SetScale() {

		switch(spaceMode) {

		case Space.Self:
			storedPrefab.transform.localScale = spawnTransform.localScale;
			break;

		case Space.World:
		default:
			storedPrefab.transform.localScale = spawnTransform.lossyScale;
			break;
		}

	}

	//Grabs the spawn location depending on space selection
	Vector3 GetSpawnLocation() {

		switch(spaceMode) {

		case Space.Self:
			return spawnTransform.localPosition;
		case Space.World:
		default:
			return spawnTransform.position;
		}
	}

	//Grabs the spawn rotation depending on space selection
	Quaternion GetSpawnRotation() {

		switch(spaceMode) {

		case Space.Self:
			return spawnTransform.localRotation;
		case Space.World:
		default:
			return spawnTransform.rotation;
		}
	}

	//Kills the prefab on command by other script
	public void KillPrefab() {
		Destroy(storedPrefab);
	}
}
