/* Script created by Lachlan McKay 2016 ©
 * This script grants full access over multiple shuriken particle systems at runtime */

using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour
{

	[Header("Main Options")]
	[Tooltip("Drag and drop the object that has the particle system you want to modify here. Defaults to this object if left blank.")]
	public ParticleSystem[] targetSystems;
	[Tooltip("Enable this if you want this to only be activated by another script. Disable if you want all systems to be set to inspector values at start.")]
	public bool commandActivated = true;

	[System.Serializable]
	public class ParticleManagerInspectorValues
	{

		[Header("Particle System Options")]
		[Tooltip("The space in which to simulate particles. It can be either world or local space.")]
		public ParticleSystemSimulationSpace simulationSpace = ParticleSystemSimulationSpace.Local;
		[Tooltip("	The initial color of particles when emitted.")]
		public Color startColor = Color.white;
		[Space(7)]
		[Tooltip("Enables/Disables emission of particles.")]
		public bool enableEmission = true;
        [Tooltip("Time: Emits particles at a rate over time. Distance: Emits particles at a race over space.")]
        public ParticleSystemEmissionType emissionMode = ParticleSystemEmissionType.Time;
        [Tooltip("Is the particle system looping?")]
		public bool loop = true;
		[Tooltip("If set to true, the particle system will automatically start playing on startup.")]
		public bool playOnAwake = true;
		[Space(7)]
		[Tooltip("The maximum number of particles to emit.")]
		public int maxParticles = 500;
		[Tooltip("Random seed used for the particle system emission. If set to 0, it will be assigned a random value on awake.")]
		public uint randomSeed = 0;
		[Space(7)]
		[Tooltip("How many particles should be emitted per second.")]
		public float emissionRate = 1f;
		[Tooltip("How quickly particles will fall.")]
		public float gravityMultiplier = 1f;
		[Tooltip("The playback speed of the particle system. 1 is normal playback speed.")]
		public float playbackSpeed = 1.0f;
		[Space(7)]
		[Tooltip("Start delay in seconds.")]
		public float startDelay = 0f;
		[Tooltip("The total lifetime in seconds that particles will have when emitted.")]
		public float startLifetime = 3f;
		[Tooltip("The initial rotation of particles when emitted.")]
		public float startRotation = 0f;
		[Tooltip("The initial size of particles when emitted.")]
		public float startSize = 1f;
		[Tooltip("The initial speed of particles when emitted.")]
		public float startSpeed = 1f;
		[Space(7)]
		[Tooltip("Playback position in seconds.")]
		public float playbackTime = 0f;

	}
	
	public ParticleManagerInspectorValues InspectorValues = new ParticleManagerInspectorValues();

    //Debugging
    [Header("Debug Options")]
    public bool debugConsole = false;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script controls all values of an array of particle systems at runtime. Intended use is for another script to send message to this script when a modification is desired.";
    private string ScriptTags = "particle manager particles expose control manipulate system port";
    private string ScriptCategory = "effect";

    //Private ParticleSystem Values
    private ParticleSystemSimulationSpace simulationSpace = ParticleSystemSimulationSpace.Local;
	private Color startColor = Color.white;
	private bool enableEmission = true;
    private ParticleSystemEmissionType emissionMode = ParticleSystemEmissionType.Time;
    private bool loop = true;
	private bool playOnAwake = true;
	private int maxParticles = 500;
	private uint randomSeed = 0;
	private float emissionRate = 1f;
	private float gravityMultiplier = 1f;
	private float playbackSpeed = 1.0f;
	private float startDelay = 0f;
	private float startLifetime = 3f;
	private float startRotation = 0f;
	private float startSize = 1f;
	private float startSpeed = 1f;
	private float playbackTime = 0f;

	//Array looping
	private int arrayLoopStart;
	private int arrayLoopEnd;

    private bool active;

	void Start () {
	
		Setup();
		if(!commandActivated) {
			SetAllValuesToInspector();
		}
	}

	void Setup() {

		//If a target system does not exist, use the particle system attached to this object if it exists, else produce an error
		if(targetSystems.Length == 1 && !targetSystems[0] && GetComponent<ParticleSystem>()) {

			targetSystems[0] = GetComponent<ParticleSystem>();

		} else if(targetSystems.Length == 0 && GetComponent<ParticleSystem>()) {

			targetSystems = new ParticleSystem[1];
			targetSystems[0] = GetComponent<ParticleSystem>();

		} else if(!targetSystems[0] || targetSystems.Length == 0) {
			print ("ERROR: Target System not set for ParticleManager script on object: " + gameObject.name + " and there is no default to use instead.");
		}

		SetArrayLoopToDefault();
	}

	//Sets all relevant systems' values to the inspector values
	void SetAllValuesToInspector () {

		SetEmissionRate(InspectorValues.emissionRate);
		EnableEmission(InspectorValues.enableEmission);
		SetGravity(InspectorValues.gravityMultiplier);
		SetLooping(InspectorValues.loop);
		SetMaxParticles(InspectorValues.maxParticles);
		SetPlaybackSpeed(InspectorValues.playbackSpeed);
		SetPlayOnAwake(InspectorValues.playOnAwake);
		SetRandomSeed(InspectorValues.randomSeed);
		SetSimulationSpace(InspectorValues.simulationSpace);
		SetStartColor(InspectorValues.startColor);
		SetStartDelay(InspectorValues.startDelay);
		SetStartLifetime(InspectorValues.startLifetime);
		SetStartRotation(InspectorValues.startRotation);
		SetStartSize(InspectorValues.startSize);
		SetStartSpeed(InspectorValues.startSpeed);
		SetPlaybackTime(InspectorValues.playbackTime);
	}

	//Applies all current private variables to relevant systems, or ALL systems if AllSystems is enabled
	void ApplyAllValuesToSystems(bool AllSystems) {

		int start;
		int end;

		if(AllSystems) {
			start = 0;
			end = targetSystems.Length;
		} else {
			start = arrayLoopStart;
			end = arrayLoopEnd;
		}

		for(int i = start; i < end; i++) {

            var em = targetSystems[i].emission;                     //Store emission as a temporary variable
            em.enabled = enableEmission;
            em.type = emissionMode;
            em.rate = new ParticleSystem.MinMaxCurve(emissionRate);

			targetSystems[i].gravityModifier = gravityMultiplier;
			targetSystems[i].loop = loop;
			targetSystems[i].maxParticles = maxParticles;
			targetSystems[i].playbackSpeed = playbackSpeed;
			targetSystems[i].playOnAwake = playOnAwake;
            targetSystems[i].randomSeed = randomSeed;
            targetSystems[i].simulationSpace = simulationSpace;
            targetSystems[i].startColor = startColor;
            targetSystems[i].startDelay = startDelay;
            targetSystems[i].startLifetime = startLifetime;
            targetSystems[i].startRotation = startRotation;
            targetSystems[i].startSize = startSize;
            targetSystems[i].startSpeed = startSpeed;
            targetSystems[i].time = playbackTime;
        }

    }

	//Set the default start and end IDs of particle systems that will be affected out of the TargetSystems array
	public void SetArrayLoopToDefault() {

		arrayLoopStart = 0;
		arrayLoopEnd = targetSystems.Length;
	}

	//Sets the first ID in the array of target systems to apply values to. Arrays before this ID will be ignored.
	public void SetArrayLoopStart(int inputArrLoopStart) {

		arrayLoopStart = inputArrLoopStart;
	}

	//Sets the last ID in the array of target systems to apply values to. Arrays after this ID will be ignored.
	public void SetArrayLoopEnd(int inputArrLoopEnd) {
	
		arrayLoopEnd = inputArrLoopEnd;
	}

	//Sets the emission rate of all relevant systems
	public void SetEmissionRate(float inputRate) {

		emissionRate = inputRate;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
            var em = targetSystems[i].emission;
            em.rate = new ParticleSystem.MinMaxCurve(emissionRate);
		}
	}

	//Toggles the emission of all relevant systems
	public void EnableEmission(bool state) {

		enableEmission = state;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
            var em = targetSystems[i].emission;
            em.enabled = state;
		}
	}

	//Sets the gravity multiplier of all relevant systems
	public void SetGravity(float inputGrav) {

		gravityMultiplier = inputGrav;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].gravityModifier = inputGrav;
		}
	}

	//Sets whether or all relevant systems will loop
	public void SetLooping(bool state) {

		loop = state;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].loop = state;
		}
	}

	//Sets the maximum particles allowed for all relevant systems
	public void SetMaxParticles(int inputMaxParticles) {

		maxParticles = inputMaxParticles;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].maxParticles = inputMaxParticles;
		}
	}

	//Sets the playback speed of all relevant systems
	public void SetPlaybackSpeed(float inputPlaybackSpeed) {

		playbackSpeed = inputPlaybackSpeed;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].playbackSpeed = inputPlaybackSpeed;
		}
	}

	//Sets whether relevant systems will play on awake
	public void SetPlayOnAwake(bool state) {

		playOnAwake = state;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].playOnAwake = state;
		}
	}

	//Sets the random seed of relevant systems
	public void SetRandomSeed(uint inputRandomSeed) {

		randomSeed = inputRandomSeed;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].randomSeed = inputRandomSeed;
		}
	}

	//Sets the simulation space of relevant systems
	public void SetSimulationSpace(ParticleSystemSimulationSpace inputSimSpace) {

		simulationSpace = inputSimSpace;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].simulationSpace = inputSimSpace;
		}
	}

	//Sets the start colour of relevant systems
	public void SetStartColor(Color inputStartCol) {

		startColor = inputStartCol;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].startColor = inputStartCol;
		}
	}

	//Sets the start delay of relevant systems
	public void SetStartDelay(float inputStartDelay) {

		startDelay = inputStartDelay;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].startDelay = inputStartDelay;
		}
	}

	//Sets the start lifetime of relevant systems
	public void SetStartLifetime(float inputStartLifetime) {

		startLifetime = inputStartLifetime;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].startLifetime = inputStartLifetime;
		}
	}

	//Sets the start rotation of relevant systems
	public void SetStartRotation(float inputStartRot) {

		startRotation = inputStartRot;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].startRotation = inputStartRot;
		}
	}

	//Sets the start size of relevant systems
	public void SetStartSize(float inputStartSize) {

		startSize = inputStartSize;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].startSize = inputStartSize;
		}
	}

	//Sets the start speed of relevant systems
	public void SetStartSpeed(float inputStartSpeed) {

		startSpeed = inputStartSpeed;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].startSpeed = inputStartSpeed;
		}
	}

	//Sets the current playback time of relevant systems
	public void SetPlaybackTime(float inputPlaybackTime) {

		playbackTime = inputPlaybackTime;

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].time = inputPlaybackTime;
		}
	}

	//Pauses/Unpauses all relevant systems
	public void Pause(bool state) {

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			if(state) {
				targetSystems[i].Pause();
			} else {
				targetSystems[i].Play();
			}
		}
	}

	//Stops all relevant systems
	public void Stop() {

		for(int i = arrayLoopStart; i < arrayLoopEnd; i++) {
			targetSystems[i].Stop();
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