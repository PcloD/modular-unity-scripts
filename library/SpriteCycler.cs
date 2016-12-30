/* Script created by Lachlan McKay 2016
 * This script cycles through a sprite sheet (atlas) in order to give the appearance of animation */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;

public class SpriteCycler : MonoBehaviour, ScriptLoadingInterface
{

	[Header("Components")]
	[Tooltip("The input sprite sheet to pull individual sprite frames from.")]
	public Texture2D atlas;
	[Tooltip("The output image component to display the current sprite on.")]
    public Image targetImage;

	[Header("Playback")]
	[Tooltip("The length of time in seconds an individual sprite frame will be displayed.")]
    public float timeBetweenFrames = 0.1f;
	[Tooltip("Play backwards through the sprite frames.")]
	public bool reverseDirection = false;

	//Debugging
	[Header("Debug Options")]
	[Tooltip("Display errors in the console.")]
	public bool debugConsole = false;

	//Credits and description
	[Header("_Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script cycles through a sprite sheet (atlas) in order to give the appearance of animation.";
    private string ScriptTags = "sprite cycler cycle image animate animation 2d visual effects vfx texture";
    private string ScriptCategory = "effect";

    //Private
    private List<Sprite> spriteList;
	private Sprite[] spriteArray;
	private bool active = false;

	private float timer;
	private int currentFrame;
	private int totalFrames;
    

	void Start () {
        Setup();
	}

    void Setup() { 

		if(ErrorChecking()) {

			string atlasPath = AssetDatabase.GetAssetPath(atlas);
			Object[] atlasSprites = AssetDatabase.LoadAllAssetsAtPath(atlasPath);	//Load child sprites into an object array from the project directory

			spriteList = new List<Sprite>();

			foreach(Object sprite in atlasSprites) {								//For each object in the obtained array, add it to a new sprite list for proper handling
				if(AssetDatabase.IsSubAsset(sprite)) {
					spriteList.Add(sprite as Sprite);
				}
			}

			if(spriteList.Count > 0) {												//If we successfully pulled some sprites from the atlas, activate the script
				totalFrames = spriteList.Count;
				spriteArray = spriteList.ToArray();
				UpdateFrame();
				active = true;
			} else {
				print("ERROR: Could not find any sprite assets contained within the Atlas on object: " + gameObject);	//Else print an error
			}
		}

    }

	//Checks for errors in how the inspector has been set up by the user
	bool ErrorChecking() {

		if(!targetImage) {

			if(gameObject.GetComponent<Image>()) {
				targetImage = gameObject.GetComponent<Image>();
				if(debugConsole) {
					print("WARNING: No Target Image assigned, using this object's image component instead: " + gameObject.name);
				}
			} else {
				if(debugConsole) {
					print("ERROR: Please assign a Target Image on object: " + gameObject.name);
				}
				return false;
			}
		}

		if(!atlas) {
			if(debugConsole) {
				print("ERROR: Please assign an Atlas to display on object: " + gameObject.name);
			}
			return false;
		}

		return true;

	}


	void Update () {
	
		if(active) {
			IncrementTimers();
		}

	}

	void IncrementTimers() {

		timer += Time.deltaTime;

		if(timer > timeBetweenFrames) {
			AdvanceFrame();
			timer -= timeBetweenFrames;
		}
	}

	//Move forward or backward one frame from the current and then update
	void AdvanceFrame() {

		if(reverseDirection) {
			currentFrame = Mathf.RoundToInt(Mathf.Repeat(currentFrame - 1, totalFrames));
		} else {
			currentFrame = Mathf.RoundToInt(Mathf.Repeat(currentFrame + 1, totalFrames));
		}
		UpdateFrame();

	}

	//Tells the target image component to change to the current sprite frame
	void UpdateFrame() {

		targetImage.sprite = spriteArray[currentFrame];

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
