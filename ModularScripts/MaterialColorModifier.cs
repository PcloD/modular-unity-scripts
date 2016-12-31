/* Script created by Lachlan McKay 2016 ©
 * This script modifies a material's color value based on a user defined gradient over time */

using UnityEngine;
using System.Collections;

public class MaterialColorModifier : MonoBehaviour {
	
	[Header("Main Options")]
	[Tooltip("The material that will be modified.")]
	public Material targetMaterial;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	[Tooltip("Clones the Target Material for editor use so that it is not affected between editor stop/starts. It is safe to disable this in a build.")]
	public bool useClonedMaterial = true;

	[Header("Color Gradient")]
	[Tooltip("Specify the color(s) that will be cycled through over time.")]
	public Gradient colorGradient;

	[Header("Material Color Property")]
	[Tooltip("What property on the material should be modified?")]
	public colorType colorProperty = colorType.color;
	public enum colorType {color, mainColor, tintColor, emissionColor, diffuseColor, ambientColor, specularColor, textColor}
	[Tooltip("Enter a name of a specific property if it does not appear in the above list.")]
	public string customProperty = "";

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
	public string ScriptDescription = "This script modifies a material's color value based on a user defined gradient over time.";
    private string ScriptTags = "material color colour modifier change mat col";
    private string ScriptCategory = "effect";

    //Private 
    private float phase = 0;
	private float remainOnFor = 0;
	private float startTime;

	private Color originalCol;
	private Material targetMatClone;

	private bool unEnabled = true; //Singleton that resets the script after being activated for a period of time
	private bool active = false;

	void Start () {

		startTime = Time.time;
		MatCloning();
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

		if(!targetMaterial) {
			if(debugConsole) {
				print ("ERROR: Please assign a target material on object: " + gameObject.name);
			}
			return false;
		}

		if(!CheckProperty()) {
			if(debugConsole) {
				if(customProperty == null) {
					print ("ERROR: Target material: " + targetMaterial.name + " does not contain a property with the name: '" + colorProperty.ToString() + "'. Please check the material's property fields by selecting it and then looking in the inspector, and then choose the matching field in the drop down box on object: " + gameObject.name);
				} else {
					print ("ERROR: Target material: " + targetMaterial.name + " does not contain a property with the name: '" + customProperty + "'. Please check the material's property fields by selecting it and then looking in the inspector, and then choose the matching field in the drop down box on object: " + gameObject.name);
				}
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

	//Handles cloning of the targetMaterial to avoid changes to the asset when using the editor
	void MatCloning() {

		if(useClonedMaterial) {

			Renderer[] allRenderers = (Renderer[])FindObjectsOfType(typeof(Renderer));
			targetMatClone = new Material(targetMaterial);

			foreach (Renderer R in allRenderers) {
				if (R.sharedMaterial == targetMaterial) {
					R.sharedMaterial = targetMatClone;
				}
			}
		} else {
			targetMatClone = targetMaterial;
		}
			
		originalCol = GetCol();
	}

	//Checks to make sure that the selected color property in the inspector exists on the target material
	bool CheckProperty() {

		if(customProperty == "") {
			switch(colorProperty) {
				
			case colorType.color:
			case colorType.mainColor:
			default:
				return targetMaterial.HasProperty("_Color");
			case colorType.tintColor:
				return targetMaterial.HasProperty("_TintColor");
			case colorType.ambientColor:
				return targetMaterial.HasProperty("_AmbientColor");
			case colorType.diffuseColor:
				return targetMaterial.HasProperty("_DiffuseColor");
			case colorType.emissionColor:
				return targetMaterial.HasProperty("_EmissionColor");
			case colorType.specularColor:
				return targetMaterial.HasProperty("_SpecColor");
			case colorType.textColor:
				return targetMaterial.HasProperty("_TextColor");
			}
		} else {
			return targetMaterial.HasProperty(customProperty);
		}
	}
		
	//
	void Update () {

		if(active) {
			IncrementPhase();	 //Increments the phase depending on the cycleMode inspector selection
			Execute();			//Assigns the correct gradient color to the material depending on the colorProperty inspector selection
			CheckForReset();	//Checks if we need to revert back to the original colour
		}
	}

	//Increments the phase depending on the cycleMode inspector selection
	void IncrementPhase() {

		switch(cycleMode) {
		case cycMode.oneShot:
			phase = Mathf.Clamp01((Time.time - startTime) / cycleTime);
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

	//Assigns the correct gradient color to the material depending on the colorProperty inspector selection
	void Execute() {

		Color colToApply = colorGradient.Evaluate(phase);
		AssignCol(colToApply);
	}

	//Assigns an inputted color to the targetMaterial
	public void AssignCol(Color inputCol) {

		if(customProperty == "") {
			switch(colorProperty) {

			case colorType.color:
			case colorType.mainColor:
			default:
				targetMatClone.SetColor("_Color", inputCol);
				break;
			case colorType.tintColor:
				targetMatClone.SetColor("_TintColor", inputCol);
				break;
			case colorType.ambientColor:
				targetMatClone.SetColor("_AmbientColor", inputCol);
				break;
			case colorType.diffuseColor:
				targetMatClone.SetColor("_DiffuseColor", inputCol);
				break;
			case colorType.emissionColor:
				targetMatClone.SetColor("_EmissionColor", inputCol);
				break;
			case colorType.specularColor:
				targetMatClone.SetColor("_SpecColor", inputCol);
				break;
			case colorType.textColor:
				targetMatClone.SetColor("_TextColor", inputCol);
				break;
			}
		} else {
			targetMatClone.SetColor(customProperty, inputCol);
		}

	}

	//Gets the current targetMaterial's color
	Color GetCol() {

		if(customProperty == "") {
			switch(colorProperty) {

			case colorType.color:
			case colorType.mainColor:
			default:
				return targetMatClone.GetColor("_Color");
			case colorType.tintColor:
				return targetMatClone.GetColor("_TintColor");
			case colorType.ambientColor:
				return targetMatClone.GetColor("_AmbientColor");
			case colorType.diffuseColor:
				return targetMatClone.GetColor("_DiffuseColor");
			case colorType.emissionColor:
				return targetMatClone.GetColor("_EmissionColor");
			case colorType.specularColor:
				return targetMatClone.GetColor("_SpecColor");
			case colorType.textColor:
				return targetMatClone.GetColor("_TextColor");
			}
		} else {
			return targetMatClone.GetColor(customProperty);
		}
	}

	//Checks end conditions for each loop mode
	void CheckForReset() {

		switch(cycleMode) {
		case cycMode.oneShot:
			
			if(phase >= 1) {
				Reset();
			}
			break;
		case cycMode.loop:
		case cycMode.bounce:

			if(!unEnabled) {
				if(remainOnFor > 0) {
					remainOnFor -= Time.deltaTime;
				} else {
					Reset();
					unEnabled = true;
				}
			}
			break;
		}
	}


	//Plays the gradient in oneShot mode from the current time
	public void PlayOneShot() {

		startTime = Time.time;

		cycleMode = cycMode.oneShot;
		active = true;

		if(debugConsole) {
			print ("Playing ColorModifier script on object: " + gameObject.name + " seconds at time: " + Time.time);
		}
	}
		
	//Enables the script for a period of time
	public void EnableForTime(float duration, cycMode cycleMethod) {

		startTime = Time.time;

		cycleMode = cycleMethod;
		active = true;

		remainOnFor = duration;
		unEnabled = false;

		if(debugConsole) {
			print ("Enabling active state of ColorModifier script for: " + duration + " seconds at time: " + Time.time);
		}
	}

	//Receives new gradients from other scripts
	public void ModifyGradient(Gradient inputGradient) {

		colorGradient = inputGradient;
	}

    //Resets to the original color
    void Reset() {
		AssignCol(originalCol);
		active = false;
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
