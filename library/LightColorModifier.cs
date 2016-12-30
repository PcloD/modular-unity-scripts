/* Script created by Lachlan McKay 2016 ©
 * This script modifies a light's color value based on a user defined gradient over time */

using UnityEngine;
using System.Collections;

public class LightColorModifier : MonoBehaviour {
	
	[Header("Main Options")]
	[Tooltip("The light that will be modified.")]
	public Light targetLight;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	
	[Header("Color Gradient")]
	[Tooltip("Specify the color(s) that will be cycled through over time.")]
	public Gradient colorGradient;
	
	[Header("Cycle Settings")]
	[Tooltip("How the script will cycle through the gradient. Oneshot: Play through the gradient once and then stop. Loop: Play through the gradient then return to the beginning and play through again. Bounce: Play through the gradient forwards and then play through the gradient backwards.")]
	public cycMode cycleMode = cycMode.bounce;
	public enum cycMode {oneShot, loop, bounce}
	[Tooltip("Reverse the direction of the gradient cycle.")]
	public bool reverse;
	[Tooltip("The amount of time it takes to complete a single gradient cycle.")]
	[Range(0,9000)]
	public float cycleTime = 1f;
	
	[Header("Debug Options")]
	[Tooltip("Display errors in the console.")]
	public bool debugConsole = false;
	
	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script modifies a light's color value based on a user defined gradient over time.";
    private string ScriptTags = "light color modifier lights colour change gradient fade strobe flicker";
    private string ScriptCategory = "effect";

    //Private 
    private float phase = 0;
	private bool active = false;
	
	void Start () {
		Setup();
	}
	
	void Setup() {
		
		if(ErrorChecking()) {
			
			if(!commandActivated) {
				active = true;
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
		
		if(cycleTime <= 0) {
			if(debugConsole) {
				print ("ERROR: Cycle time must be greater than 0 on object: " + gameObject.name);
			}
			return false;
		}
		
		return true;
	}
	
	void Update () {

		if(active) {
			IncrementPhase();						 //Increments the phase depending on the cycleMode inspector selection
			targetLight.color =  GetColor();		//Apply the color to the target light
		}	
	}
	
	//Increments the phase depending on the cycleMode inspector selection
	void IncrementPhase() {
		
		switch(cycleMode) {
		case cycMode.oneShot:
			phase = Mathf.Clamp01(Time.time / cycleTime);
			break;
		case cycMode.loop:
			phase = Mathf.Clamp01(Mathf.Repeat(Time.time / cycleTime, 1f));
			break;
		case cycMode.bounce:
			phase = Mathf.Clamp01(Mathf.PingPong(Time.time / cycleTime, 1f));
			break;
		}
		
		if(reverse) {
			phase = 1 - phase;
		}
	}

	public Color GetColor() {
		return colorGradient.Evaluate(phase);
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
