/* Script created by Lachlan McKay 2016 ©
 * This script generates a trail of points and has the capability of representing them as a line, primitive or prefab particle
 * This can be useful for creating a range of visual effects such as explosions, fire, smoke stacks, cloth wafting in the wind ec.
 * No points are instantiated or destroyed in this script, all points are recycled up the chain instead */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailEmitter : MonoBehaviour
{
	
	[Header("Main Options")]
	[Tooltip("The transform around which particles will emit.")]
	public Transform affectedObject;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	[Tooltip("When calculating distance moved by the affectedObject, consider world or local coordinates?")]
	public Space spaceMode = Space.World;
	
	[System.Serializable]
	public class TrailEmitterEmitter
	{
		[Header("Emitter Options")]
		[Tooltip("The maximum number of particles that can exist.")]
		public int maxParticles = 50;
		[Tooltip("The amount of time that must pass before the particle positions are updated.")]
		public float updateInterval = 0.025f;
		[Tooltip("Force that acts on all particles at all times.")]
		public Vector3 gravityForce = new Vector3(0, -50, 0);
		[Range(0,1)]
		[Tooltip("Slows particles based on their speed. Moderately expensive.")]
		public float drag = 0;

	}
	
	public TrailEmitterEmitter Emitter = new TrailEmitterEmitter();

	[System.Serializable]
	public class TrailEmitterSizeAndColor
	{
		[Header("Size / Color")]
		[Tooltip("The maximum scale the object can have.")]
		public float scale = 0.5f;
		[Tooltip("Modify the scale over time. (0 = no modification).")]
		public float incrementScale = 0f;
		[Tooltip("Use this curve to define the emitted line/object's size over its lifetime.")]
		public AnimationCurve sizeOverLifetime = AnimationCurve.EaseInOut(0,1,1,0);
		[Tooltip("Update the size of primitive/prefab objects every frame?")]
		public bool updateSize = true;
		[Space(7)]
		[Tooltip("The starting color of primitive/prefab/lineRenderer objects.")]
		public Color startColor = Color.red;
		[Tooltip("The end color of primitive/prefab/lineRenderer objects.")]
		public Color endColor = Color.clear;
		[Tooltip("The curve describing progression from start to end color over particle lifetime.")]
		public AnimationCurve colorOverLifetime = AnimationCurve.EaseInOut(0,0,1,1);
		[Tooltip("Update the color of objects every frame? (Useful for inspector tuning)")]
		public bool updateColors = true;
		[Header("Materials")]
		[Tooltip("Material to apply to primitives or lineRenderers.")]
		public Material materialToApply;
		[Tooltip("This needs to be enabled if you want seperate primitives or prefabs to have different coloured materials.")]
		public bool useMaterialArray = false;
	}
	
	public TrailEmitterSizeAndColor SizeAndColor = new TrailEmitterSizeAndColor();

	[Header("Noise Options")]
	[Tooltip("What kind of noise should be used to distort movement?")]
	public noise noiseType = noise.perlinNoise;
	public enum noise {randomRange, perlinNoise}

	[System.Serializable]
	public class TrailEmitterNoiseUniversal
	{
		[Header("Universal Options")]
		[Tooltip("How much translation impact noise will have on the trail. Bigger = more chaotic.")]
		public float noiseAmplitude = 1.2f;
		[Space(7)]
		[Tooltip("-1 to 1 multiplier range describing the minimum and maximum direction noise can have in world space.")]
		public Vector2 noiseXRange = new Vector2(-1,1);
		[Tooltip("-1 to 1 multiplier range describing the minimum and maximum direction noise can have in world space.")]
		public Vector2 noiseYRange = new Vector2(0,1);
		[Tooltip("-1 to 1 multiplier range describing the minimum and maximum direction noise can have in world space.")]
		public Vector2 noiseZRange = new Vector2(-1,1);
		[Space(7)]
		[Tooltip("Reduce noise amplitude if the Affected Object is not moving.")]
		public bool scaleAmplitudeBasedOnVelocity = false;
		[Tooltip("The minimum speed the Affected Object must be travelling to achieve full noise amplitude.")]
		public float minimumSpeed = 1f;
		[Tooltip("While stationary, the trail will have this much noise amplitude.")]
		[Range(0,1)]
		public float stationaryAmplitude = 0.1f;
	}
	
	public TrailEmitterNoiseUniversal NoiseUniversal = new TrailEmitterNoiseUniversal();


	[System.Serializable]
	public class TrailEmitterPerlinNoise
	{
		[Header("Perlin Options")]
		[Tooltip("How quickly the path of noise will evolve over time.")]
		public float noiseEvolution = 1f;
		[Tooltip("A larger plane size will result in emitters close together in world space having similar noise.")]
		public float noisePlaneSize = 1000f;
		[Tooltip("Random seed.")]
		public Vector3 noiseSeed = new Vector3(0,1000,10);
		[Tooltip("Randomize the seed on start.")]
		public bool randomizeSeed = false;
	}
	
	public TrailEmitterPerlinNoise PerlinNoise = new TrailEmitterPerlinNoise();
	
	[Header("Output Options")]
	[Tooltip("How will this scripts information be represented? Non-modifiable at runtime.")]
	public output outputType = output.primitive;
	public enum output {externalScriptOnly, lineRenderer, primitive, loadedPrefab}

	[System.Serializable]
	public class TrailEmitterLineRendererOptions
	{
		[Header("LineRenderer Options")]
		[Tooltip("Target LineRenderer to output to. If one is not specified, one will be created for you.")]
		public LineRenderer lineRenderer;
	}
	
	public TrailEmitterLineRendererOptions LineRendererOptions = new TrailEmitterLineRendererOptions();

	[System.Serializable]
	public class TrailEmitterPrimitiveOrPrefab
	{

		[Header("Primitive")]
		[Tooltip("The type of primitive to spawn.")]
		public PrimitiveType primitiveType = PrimitiveType.Quad;
		public bool removeCollider = true;

		[Header("Prefab")]
		[Tooltip("The prefab to spawn.")]
		public GameObject loadedPrefab;

		[Header("Billboard")]
		[Tooltip("Primitives will rotate to look at this transform if it exists.")]
		public Transform billboardTarget;
		[Tooltip("Override above transform and instead search for a target by name after spawning, useful for prefab trailEmitters.")]
		public string billboardSearchName;
		[Tooltip("Allows you to offset the look rotation.")]
		public Vector3 rotationOffset;

		[Header("Death")]
		[Tooltip("If this script is destroyed, also destroy all objects spawned by it.")]
		public bool destroyObjectsOnDeath = true;

	}
	
	public TrailEmitterPrimitiveOrPrefab PrimitiveOrPrefab = new TrailEmitterPrimitiveOrPrefab();


	//Debugging
	[Header("Debug Options")]
	[Tooltip("Display errors in the console.")]
	public bool debugConsole = false;

	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script generates an array of points that are affected by noise. Possible uses include: Explosion/Fire/Smoke/Ribbon Effects.";
    private string ScriptTags = "trail emitter vfx heavy weight powerful primitive line renderer point noise generate generator render";
    private string ScriptCategory = "effect";

    //Output
    [System.NonSerialized] public Vector3[] particlePositions;
	[System.NonSerialized] public Vector3[] particleVelocity;
	
	//LineRenderer
	private float lineSegment;							//The distance between each position. Used for antiflickering when using a lineRenderer component
	private Material lineMaterial;						//The material of the lineRenderer that is in use, used to prevent flickering

	//Particles
	private float timeSinceUpdate = 0;					//Time that has passed since point positions have been updated
	private int currentNumberOfParticles = 2;			//Current number of array slots that have been filled
	private bool allPointsAdded = false;				//Have all array slots been filled with information yet?
	private float noiseEvolutionTime;					//How quickly the path of perlin noise will change
	private Material storedMaterial;					//Stores the inspector loaded material in a temporary material to enable editing the color without affecting the source material

	//Objects
	private GameObject[] objects;						//Array of primitive objects or alternatively prefabs
	private Material[] objectMats;						//Array of materials attached to objects[]
	private GameObject objectParent;					//Parent GO to group spawned objects under for neatness

	//Distance moved in the last frame
	private Vector3 currentPos;							//The current position of the affectObject transform
	private Vector3 lastPos;							//The current position of the affectObject transform, at the time of the last frame

	//Clamped NoiseRange
	private Vector2 clampedNoiseRangeX;					//Prevents the user inputting a range greater than 1 or less than -1 to the script
	private Vector2 clampedNoiseRangeY;
	private Vector2 clampedNoiseRangeZ;

	//Vectors
	private Vector3 storeVector;						//Stores and iterates the velocity vector and sends it down the chain

	//Billboard
	private Transform storedBillboardTarget;				//Stores the target billboard object when using the search by name function

	//Misc
	private bool active = false;							//Whether or not the script is active
	
	void Awake () {
		Setup();
	}

	void Setup() {

		if(!affectedObject) {
			affectedObject = transform;			//If no affected object has been specified, set it to this object
		}

		particlePositions = new Vector3[Emitter.maxParticles];			//Initialize position array
		particleVelocity = new Vector3[Emitter.maxParticles];			//Initialize velocity array

		if(SizeAndColor.materialToApply == null) {
			SizeAndColor.materialToApply = new Material(Shader.Find("Particles/Additive"));			//If there is no material to apply, create a default one and use that instead (a material is required)
		}

		if(!SizeAndColor.useMaterialArray) {
			storedMaterial = new Material(SizeAndColor.materialToApply);	//Use a temporary material to avoid editing the source material's color
		}

		SetupParticles();	//Setup 

		switch(outputType) {
		case output.lineRenderer:
			SetupLineRenderer();
			break;

		case output.primitive:
		case output.loadedPrefab:
			SetupObjects();
			break;
		}

		SetupOutput();
		SetupNoise();

		if(!commandActivated) {
			active = true;
		}
	}

	void SetupParticles() {

		allPointsAdded = false;

		for (int i = 0; i < Emitter.maxParticles; i++ ) {
			
			storeVector = GenerateMoveVector();
			particleVelocity[i] = storeVector;
			particlePositions[i] = affectedObject.position;
		}
	}

	void SetupOutput() {

		for(int i = 0; i < Emitter.maxParticles; i++) {

			switch(outputType) {
				
			case output.lineRenderer:
				if(i < currentNumberOfParticles) {
					LineRendererOptions.lineRenderer.SetPosition (i, particlePositions[i]);
				}
				break;
				
			case output.primitive:
			case output.loadedPrefab:
				
				if(outputType == output.primitive) {
					objects[i] = GameObject.CreatePrimitive(PrimitiveOrPrefab.primitiveType);
					
					if(PrimitiveOrPrefab.removeCollider) {
						RemoveCollider(i);
					}

				} else if(outputType == output.loadedPrefab) {
					objects[i] = Instantiate(PrimitiveOrPrefab.loadedPrefab, affectedObject.position, Quaternion.identity) as GameObject;
				}
				objects[i].transform.position = affectedObject.position;
				objects[i].transform.localScale = new Vector3(SizeAndColor.scale, SizeAndColor.scale, SizeAndColor.scale);
				objects[i].transform.SetParent(objectParent.transform);
				objects[i].name = i.ToString();

				if(PrimitiveOrPrefab.billboardSearchName != "") {
					storedBillboardTarget = GameObject.Find(PrimitiveOrPrefab.billboardSearchName).transform;
				}
				
				ApplyCols(i);
				break;
			}
		}

	}

	void RemoveCollider(int ID) {

		if(objects[ID].GetComponent<MeshCollider>()) {
			Destroy(objects[ID].GetComponent<MeshCollider>());
		} else if(objects[ID].GetComponent<SphereCollider>()) {
			Destroy(objects[ID].GetComponent<SphereCollider>());
		} else if(objects[ID].GetComponent<BoxCollider>()) {
			Destroy(objects[ID].GetComponent<BoxCollider>());
		} else if(objects[ID].GetComponent<CapsuleCollider>()) {
			Destroy(objects[ID].GetComponent<CapsuleCollider>());
		}
	}

	void SetupLineRenderer() {

		lineSegment = (1.0f / Emitter.maxParticles);

		if(!LineRendererOptions.lineRenderer) {
			affectedObject.gameObject.AddComponent<LineRenderer>();
			LineRendererOptions.lineRenderer = affectedObject.GetComponent<LineRenderer>();
		}

		if(SizeAndColor.materialToApply) {
			LineRendererOptions.lineRenderer.material = SizeAndColor.materialToApply;
		}

		lineMaterial = LineRendererOptions.lineRenderer.material;
		LineRendererOptions.lineRenderer.SetVertexCount (currentNumberOfParticles);
		LineRendererOptions.lineRenderer.SetWidth(GetSize(0), GetSize(Emitter.maxParticles));
		LineRendererOptions.lineRenderer.SetColors(SizeAndColor.startColor, SizeAndColor.endColor);
	}

	//Sets up clamped noise ranges
	void SetupNoise() {

		clampedNoiseRangeX = new Vector2(Mathf.Clamp(NoiseUniversal.noiseXRange.x, -1,1), Mathf.Clamp(NoiseUniversal.noiseXRange.y, -1,1));
		clampedNoiseRangeY = new Vector2(Mathf.Clamp(NoiseUniversal.noiseYRange.x, -1,1), Mathf.Clamp(NoiseUniversal.noiseYRange.y, -1,1));
		clampedNoiseRangeZ = new Vector2(Mathf.Clamp(NoiseUniversal.noiseZRange.x, -1,1), Mathf.Clamp(NoiseUniversal.noiseZRange.y, -1,1));

		if(PerlinNoise.randomizeSeed) {
			PerlinNoise.noiseSeed = Random.insideUnitSphere * Random.Range(0,10000);
		}
	}

	void SetupObjects() {

		objects = new GameObject[Emitter.maxParticles];
		objectParent = new GameObject(affectedObject.name + " Particles");
		objectMats = new Material[Emitter.maxParticles];

		for(int i = 0; i < objectMats.Length; i++) { 
			objectMats[i] = new Material(SizeAndColor.materialToApply);
			objectMats[i].name = SizeAndColor.materialToApply.name + " " + i;
		}
	}

	void Update () {

		if(active) {
			StorePositions("start");
			IncrementTimers();
			Engine();
			Output();
			StorePositions("end");
		}
	}
	
	void IncrementTimers() {
		
		timeSinceUpdate += Time.deltaTime;
		noiseEvolutionTime += Time.deltaTime * PerlinNoise.noiseEvolution;

		if(SizeAndColor.scale > 0) {
			SizeAndColor.scale += SizeAndColor.incrementScale * Time.deltaTime;
		}
	}

	void Engine() {

		if(timeSinceUpdate > Emitter.updateInterval) {
			timeSinceUpdate -= Emitter.updateInterval;
				CalculatePoints ();
		}
	}
	
	void Output() {

		switch(outputType) {

		case output.lineRenderer:
			ApplyToLine ();
			break;

		case output.primitive:
		case output.loadedPrefab:
			ApplyToObjects();
			break;
		}

	}

	void CalculatePoints() {

		// Add points until the target number is reached.
		if (!allPointsAdded ) {

			currentNumberOfParticles++;

			storeVector = GenerateMoveVector();
			particleVelocity[0] = storeVector;
			particlePositions[0] = affectedObject.position;

			if(outputType == output.lineRenderer) {
				LineRendererOptions.lineRenderer.SetVertexCount (currentNumberOfParticles);
				LineRendererOptions.lineRenderer.SetPosition (0, particlePositions[0] );
			}
		}
		
		if (!allPointsAdded && (currentNumberOfParticles == Emitter.maxParticles)) {
			allPointsAdded = true;
		} else if(allPointsAdded && currentNumberOfParticles < Emitter.maxParticles) {
			allPointsAdded = false;
		}
		
		// Make each point in the line take the position and direction of the one before it (effectively removing the last point from the line and adding a new one at transform position).
		for(int i = currentNumberOfParticles - 1; i > 0; i--) {

			storeVector = particlePositions[i-1];
			particlePositions[i] = storeVector;

			storeVector = particleVelocity[i-1];
			particleVelocity[i] = storeVector;
		}

		storeVector = GenerateMoveVector();
		particleVelocity[0] = storeVector; // Remember and give origin point a direction for when it gets pulled up the chain in the next line update.

		// Update the positions
		for(int i = 1; i < currentNumberOfParticles; i++) {
			storeVector = particlePositions[i];
			storeVector += particleVelocity[i] * Time.deltaTime;
			particlePositions[i] = storeVector;
		}

		particlePositions[0] = affectedObject.position; //Set origin point to the affectedObject's position
	}


	//Outputs particle positions to lineRenderer component
	void ApplyToLine() {

		for(int i = 1; i < currentNumberOfParticles; i++) {
			LineRendererOptions.lineRenderer.SetPosition (i, particlePositions[i]);
		}
		LineRendererOptions.lineRenderer.SetPosition (0, affectedObject.position );	
		
		// If we're at the maximum number of points, tweak the offset so that the last line segment is "invisible" (i.e. off the top of the texture) when it disappears.
		// Makes the change less jarring and ensures the texture doesn't jump.
		if (allPointsAdded) {
			Vector2 storeMainTexOffset = lineMaterial.mainTextureOffset;
			storeMainTexOffset.x = lineSegment * ( timeSinceUpdate / Emitter.updateInterval );
			lineMaterial.mainTextureOffset = storeMainTexOffset;
		}
	}

	//Outputs particle positions to quad primitives
	void ApplyToObjects() {

		for(int i = 0; i < objects.Length; i++) {
            if (spaceMode == Space.World)
            {
                objects[i].transform.position = particlePositions[i];
            } else
            {
                objects[i].transform.localPosition = particlePositions[i];
            }

			if(SizeAndColor.updateSize) {
				float size = GetSize(i);
				objects[i].transform.localScale = new Vector3(size, size, size);
			}

			ApplyCols(i);

			Quaternion lookRot = Quaternion.identity;

			if(PrimitiveOrPrefab.billboardTarget|| PrimitiveOrPrefab.billboardSearchName != "") {

				if(PrimitiveOrPrefab.billboardSearchName != "") {
					
					lookRot = Quaternion.LookRotation(storedBillboardTarget.position);
				
				} else {
					lookRot = Quaternion.LookRotation(PrimitiveOrPrefab.billboardTarget.position);
				}
			}

			Vector3 finalVector = lookRot.eulerAngles;
			
			finalVector = lookRot.eulerAngles + PrimitiveOrPrefab.rotationOffset;
			objects[i].transform.rotation = Quaternion.Euler(finalVector);
		}
	}
	
	//Applies the correct colour value for a given particle to an object
	void ApplyCols(int i) {
		
		Color colToApply = GetColor(i);
		Renderer objRend = objects[i].GetComponent<Renderer>();
		
		if(SizeAndColor.materialToApply) {
			if(SizeAndColor.useMaterialArray) {										//Using a material array, set each individual object to have its own material and color
				
				objectMats[i].color = colToApply;
				objectMats[i].SetColor("_TintColor", colToApply);
				objectMats[i].SetColor("_EmissionColor", colToApply);
				
				if(objRend) {
					objRend.material = objectMats[i];
				}
				
			} else {																//Not using an array, set all objects to have one material
				if(objRend) {
					storedMaterial.color = GetColor(0);
					storedMaterial.SetColor("_TintColor", GetColor(0));
					objRend.material = storedMaterial;
				}
			}
		}
	}

	//Before destroying this script, destroy all objects
	void OnDestroy() {

		if(PrimitiveOrPrefab.destroyObjectsOnDeath) {

			if(objectParent) {
				Destroy(objectParent);
			}

			if(outputType == output.primitive || outputType == output.loadedPrefab) {

				for(int i = 0; i < objects.Length; i++) {
					Destroy(objects[i]);
					Destroy(objectMats[i]);
				}
			}
		}
	}

	//When disabling this script, disable all objects
	void OnDisable() {

		if(objectParent) {
			objectParent.SetActive(false);
		}
		
		if(outputType == output.primitive || outputType == output.loadedPrefab) {
			
			for(int i = 0; i < objects.Length; i++) {
				if(objects[i]) {
					objects[i].SetActive(false);
				}
			}
		}
	}

	//When enabling this script, enable all objects
	void OnEnable() {

		if(objectParent) {
			objectParent.SetActive(true);
		}
		
		if(outputType == output.primitive || outputType == output.loadedPrefab) {
			
			for(int i = 0; i < objects.Length; i++) {
				objects[i].SetActive(true);
			}
		}
	}

	Vector3 GenerateMoveVector() {

		Vector3 generatedVector = Vector3.zero;

		switch(noiseType) {

		case noise.randomRange:
			
			generatedVector.x = Random.Range (clampedNoiseRangeX.x, clampedNoiseRangeX.y);
			generatedVector.y = Random.Range (clampedNoiseRangeY.x, clampedNoiseRangeY.y);
			generatedVector.z = Random.Range (clampedNoiseRangeZ.x, clampedNoiseRangeZ.y);

			break;

		case noise.perlinNoise:

			generatedVector = GeneratePerlinNoise(particlePositions[0]);
			break;

		}

		generatedVector.Normalize ();
		generatedVector *= NoiseUniversal.noiseAmplitude;

		generatedVector.x -= Emitter.gravityForce.x * Time.deltaTime;
		generatedVector.y -= Emitter.gravityForce.y * Time.deltaTime;
		generatedVector.z -= Emitter.gravityForce.z * Time.deltaTime;

		generatedVector += GetDrag(generatedVector);

		if(NoiseUniversal.scaleAmplitudeBasedOnVelocity) {
			float scaledAmplitude = Mathf.Clamp01((GetDistance() / NoiseUniversal.minimumSpeed) + NoiseUniversal.stationaryAmplitude);
			Vector3 scaleVector = new Vector3(scaledAmplitude, scaledAmplitude, scaledAmplitude);
			generatedVector.Scale(scaleVector);
		}

        if(spaceMode == Space.Self)
        {
            generatedVector = transform.TransformDirection(generatedVector);
            //Vector3 testVec = transform.TransformDirection(generatedVector);
            //Vector3 testVec = affectedObject.forward;
            //Debug.DrawRay(affectedObject.position, testVec * 10f, Color.red);
        }

		return generatedVector;
	}
	
	Vector3 GeneratePerlinNoise(Vector3 worldPosition) {
		
		float xCoord = PerlinNoise.noiseSeed.x + worldPosition.x / PerlinNoise.noisePlaneSize;
		float yCoord = PerlinNoise.noiseSeed.y + worldPosition.y / PerlinNoise.noisePlaneSize;
		float zCoord = PerlinNoise.noiseSeed.z + worldPosition.z / PerlinNoise.noisePlaneSize;

		Vector3 noiseVelocity;

		noiseVelocity.x = Mathf.PerlinNoise(zCoord + noiseEvolutionTime, yCoord);
		noiseVelocity.y = Mathf.PerlinNoise(xCoord + noiseEvolutionTime, zCoord);
		noiseVelocity.z = Mathf.PerlinNoise(xCoord + noiseEvolutionTime, yCoord);
		
		noiseVelocity.x = ReMap(noiseVelocity.x, 0,1, clampedNoiseRangeX.x, clampedNoiseRangeX.y);
		noiseVelocity.y = ReMap(noiseVelocity.y, 0,1, clampedNoiseRangeY.x, clampedNoiseRangeY.y);
		noiseVelocity.z = ReMap(noiseVelocity.z, 0,1, clampedNoiseRangeZ.x, clampedNoiseRangeZ.y);
		
		return new Vector3(noiseVelocity.x, noiseVelocity.y, noiseVelocity.z);
	}

	Vector3 GetDrag(Vector3 inputVelocity) {
		if(Emitter.drag > 0) {
			Vector3 dragForce = new Vector3(GetDragForce(inputVelocity.x), GetDragForce(inputVelocity.y), GetDragForce(inputVelocity.z));
			dragForce.x = Mathf.Clamp(dragForce.x, -inputVelocity.x, inputVelocity.x);
			dragForce.y = Mathf.Clamp(dragForce.y, -inputVelocity.y, inputVelocity.y);
			dragForce.z = Mathf.Clamp(dragForce.z, -inputVelocity.z, inputVelocity.z);
			return dragForce;
		}
		return Vector3.zero;
	}

	float GetDragForce(float inputSpeed) {
		
		return (-0.5f * (inputSpeed * inputSpeed)) * Emitter.drag;
	}

	float ReMap (float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	//Returns size based on index
	float GetSize(int index) {
		float percent = (float) index/Emitter.maxParticles;
		return Mathf.Clamp01(SizeAndColor.sizeOverLifetime.Evaluate(percent)) * SizeAndColor.scale;	//Size = animation curve 0-1 value * scale
	}

	//Returns the distance moved by the affectedObject's transform over the last frame
	float GetDistance() {

		return Vector3.Distance(currentPos, lastPos);
	}

	//Returns the current position of the affectedObject transform based on the spaceMode inspector selection
	Vector3 GetPosition() {
		
		switch(spaceMode) {
			
		case Space.Self:
			return affectedObject.localPosition;
			
		case Space.World:
		default:
			return affectedObject.position;
		}
	}

	//Stores the currentposition or last position of the affectedObject's transform, for use in calculating distance moved over the last frame
	void StorePositions(string phase) {
		
		switch(phase) {
			
		case "start":
			currentPos = GetPosition();	//Update currentPos
			break;
			
		case "end":
			lastPos = GetPosition();	//Store last frame's currentPos as "lastPos"
			break;
		}
	}

	//Returns color value based on index
	Color GetColor(int index) {
		float percent = (float) index/Emitter.maxParticles;
		float tParam = Mathf.Clamp01(SizeAndColor.colorOverLifetime.Evaluate(percent));
		return Color.Lerp(SizeAndColor.startColor, SizeAndColor.endColor, tParam);
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
