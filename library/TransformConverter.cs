/* Script created by Lachlan McKay 2016 ©
 * This script allows you make an object have the same transform as another, but in a different world/camera/viewport space
 * Useful for displaying a healthbar above a character's head */

using UnityEngine;
using System.Collections;

public class TransformConverter : MonoBehaviour
{

	[Header("Transforms")]
	[Tooltip("Transform that the position is taken from. Leave blank for self.")]
	public Transform sourceTransform;
	[Tooltip("Transform that the position is applied to. Leave blank for self.")]
	public Transform targetTransform;

	[Header("The source transform is of type:")]
	public sourceInput sourceInputType;
	public enum sourceInput {world, screen, viewport}

	[Header("Convert transform to:")]
	public targetOutput targetOutputType;
	public enum targetOutput {world, screen, viewport}

	[Header("General Settings")]
	public bool continuousUpdate = true;

	[Header("Screen/Viewport Mode Settings")]
	public Camera sceneCamera;

    [Header("Debugging")]
    public bool debugConsole;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script allows you make an object have the same transform as another, but in a different world/camera/viewport space. Hover over any property name for a description of its function.";
    private string ScriptTags = "transform converter convert space position pos health bar ui float";
    private string ScriptCategory = "screen";

    //Private
    private bool active = false;

	private Vector3 sourcePos;
	private Vector3 targetPos;

	void Start () {
	
		Setup();
		
		if(!continuousUpdate && active) {
			ExecuteConversion();
		}
	}

	void Update () {
	
		if(continuousUpdate && active) {
			ExecuteConversion();
		}
	}

	void Setup() {

		if(!targetTransform && !sourceTransform || targetTransform == sourceTransform) {
			
			print ("Error: You must have 2 different transforms. Cannot convert and apply to self. Try using an empty gameobject as a marker.");
			
		} else {
			
			if(!targetTransform) {
				targetTransform = transform;
			}
			
			if(!sourceTransform) {
				sourceTransform = transform;
			}
            active = true;
		}
	}

	void ExecuteConversion() {

		sourcePos = sourceTransform.position;

		switch(sourceInputType) {

		case sourceInput.world:

			switch(targetOutputType) {

			case targetOutput.world:
				print ("Warning: You are pointlessly converting world coordinates to world coordinates.");
				break;

			case targetOutput.screen:
				targetTransform.position = sceneCamera.WorldToScreenPoint(sourcePos);
				break;

			case targetOutput.viewport:
				targetTransform.position = sceneCamera.WorldToViewportPoint(sourcePos);
				break;
			}

			break;

		case sourceInput.screen:

			switch(targetOutputType) {
				
			case targetOutput.world:
				targetTransform.position = sceneCamera.ScreenToWorldPoint(sourcePos);
				break;
				
			case targetOutput.screen:
				print ("Warning: You are pointlessly converting screen coordinates to screen coordinates.");
				break;
				
			case targetOutput.viewport:
				targetTransform.position = sceneCamera.ScreenToViewportPoint(sourcePos);
				break;
			}

			break;

		case sourceInput.viewport:

			switch(targetOutputType) {
				
			case targetOutput.world:
				targetTransform.position = sceneCamera.ViewportToWorldPoint(sourcePos);
				break;
				
			case targetOutput.screen:
				targetTransform.position = sceneCamera.ViewportToScreenPoint(sourcePos);
				break;
				
			case targetOutput.viewport:
				print ("Warning: You are pointlessly converting viewport coordinates to viewport coordinates.");
				break;
			}
			break;
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
