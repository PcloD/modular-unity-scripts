/* Script created by Lachlan McKay 2016 ©
 * This script modifies a light's intensity over time based on a user defined curve */

using UnityEngine;
using System.Collections;

public class LightIntensityModifier : MonoBehaviour
{
	
	[Header("Main Options")]
	[Tooltip("The light that will be modified.")]
	public Light targetLight;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;

	[Header("Range Settings")]
	[Tooltip("Use this curve to define the light's intensity over time. NOTE: Do not use an amplitude higher than 1.")]
	public AnimationCurve lightIntensity = AnimationCurve.EaseInOut(0,1,1,0);
	[Tooltip("Multiplies the final intensity value.")]
	public float amplitude = 1f;

	[Header("Interpolation Settings")]
	[Tooltip("How the script will cycle through the curve.")]
	public mode interpolationMode = mode.bounce;
	public enum mode {instant, loop, bounce}
	[Tooltip("Reverse the direction of the curve cycle.")]
	public bool reverse;

	[Header("Random Settings")]
	[Tooltip("Allows you to override the above interpolation settings and just use a random value in between the curve's highest and lowest values. If disabled, the script will evaluate the curve normally.")]
	public rMode randomMode = rMode.disabled;
	public enum rMode {disabled, randomValue, perlin}
	[Tooltip("(Minimum, Maximum)")]
	public Vector2 range = new Vector2(0f,2f);
	[Tooltip("Specify the Y coordinate for perlin noise. Lights with the same YAxis coordinate will have identical flickering patterns.")]
	public float perlinYAxis = 0f;

	[Header("Time Settings")]
	[Tooltip("The amount of time it takes to complete a single cycle of the Light Intensity curve, or achieve a random value.")]
	[Range(0,9000)]
	public float cycleTime = 1f;
	[Tooltip("The amount of time to wait after completing a cycle or achieving a random value.")]
	[Range(0,9000)]
	public float waitTime = 0f;
	[Tooltip("Execute this many extra cycles before waiting.")]
	[Range(0,9000)]
	public int extraCycles = 0;

	[Header("Debug Options")]
	[Tooltip("Display errors in the console.")]
	public bool debugConsole = false;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script modifies a light's intensity over time based on a user defined curve.";
    private string ScriptTags = "light intensity modifier lights colour strength change gradient fade strobe flicker";
    private string ScriptCategory = "effect";

    //Private 
    private float phase = 0;
	private bool active = false;

	private float phaseTimer;
	private float lerpTimer;
	private float waitTimer;

	private float lastValue;
	private float targetValue;

	private string state = "lerping";
	
	void Start () {
		Setup();
	}
	
	void Setup() {
		
		if(ErrorChecking()) {
			
			if(!commandActivated) {
				active = true;
			}

			if(randomMode == rMode.randomValue) {
				UpdateRandomTarget(true);
			}
		}
	}
	
	//Checks for errors in the inspector
	bool ErrorChecking() {
		
		if(!targetLight) {
			if(debugConsole) {
				print ("ERROR: Please assign a target light on object: " + gameObject.name);
			}
			return false;
		}
		
		if(cycleTime < 0 || waitTime < 0) {
			if(debugConsole) {
				print ("ERROR: Interpolation Time / Wait Time cannot be negative on object: " + gameObject.name);
			}
			return false;
		}
		
		return true;
	}
	
	void Update () {
		
		if(active) {
			IncrementTimers();						 //Increments the phase depending on the cycleMode inspector selection
			targetLight.intensity = GetIntensity();		//Apply the color to the target light
		}	
	}
	
	//Increments the phase and timers depending on the cycleMode inspector selection
	void IncrementPhase() {

		phaseTimer += Time.deltaTime;

		switch(interpolationMode) {
		case mode.instant:
			phase = Mathf.Clamp01(Mathf.Abs(phase - 1));
			break;
		case mode.loop:
			phase = Mathf.Clamp01(Mathf.Repeat(phaseTimer / cycleTime, 1f));
			break;
		case mode.bounce:
			phase = Mathf.Clamp01(Mathf.PingPong(phaseTimer / cycleTime, 1f));
			break;
		}
		
		if(reverse) {
			phase = 1 - phase;
		}
	}

	//Increments the lerp and wait timers depending on whether the script is lerping or waiting
	void IncrementTimers() {

		switch(state) {

		case "lerping":
			
			lerpTimer += Time.deltaTime;
			IncrementPhase();

			if(lerpTimer >= cycleTime * (extraCycles + 1)) {
				state = "waiting";
				lerpTimer -= cycleTime * (extraCycles + 1);
			}
			break;
		case "waiting":

			waitTimer += Time.deltaTime;

			if(waitTimer >= waitTime) {
				if(randomMode == rMode.randomValue) {
					UpdateRandomTarget(false);
				}
				state = "lerping";
				waitTimer -= waitTime;
			}
			break;
		}
	}

	//Gets the current intensity based on the phase and random settings
	public float GetIntensity() {

		switch(randomMode) {

		case rMode.disabled:
		default:
			return lightIntensity.Evaluate(phase) * amplitude;
		case rMode.randomValue:
			return Mathf.Lerp(lastValue, targetValue, phase) * amplitude;
		case rMode.perlin:
			return Mathf.PerlinNoise(phase, perlinYAxis) * amplitude;
		}
	}

	//Updates the random values by using the GenerateRandomValue function
	void UpdateRandomTarget(bool firstTime) {

		if(firstTime) {
			lastValue = GenerateRandomValue();
			targetValue = GenerateRandomValue();
		} else {
			lastValue = GetIntensity();
			targetValue = GenerateRandomValue();
		}
	}

	//Generates a new random value using Random.Range
	float GenerateRandomValue() {

		switch(randomMode) {
		case rMode.randomValue:
			return Random.Range(range.x, range.y);
		}

		if(debugConsole) {
			print ("ERROR: The script is trying to generate a random value even though the inspector does not appear to be in Random Value mode.");
		}
		return 0;
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
